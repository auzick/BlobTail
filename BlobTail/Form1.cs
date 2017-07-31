using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using BlobTail.Properties;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace BlobTail
{
    public partial class Form1 : Form
    {
        public const int BlobCheckChangedInterval = 3000;
        public const int SleepInterval = 200;

        public static class TailStatus
        {
            public static readonly int TextRead = 0;
            public static readonly int TextReadToEnd = 1;
            public static readonly int AtEnd = 2;
        }

        private BackgroundWorker _tailWorker;
        private bool _resetTail = false;
        private CloudBlob _currentBlob;

        private CloudBlobClient _blobClient;
        private CloudBlobClient BlobClient => _blobClient ?? (_blobClient = StorageAccount.CreateCloudBlobClient());

        private CloudStorageAccount _storageAccount;
        public CloudStorageAccount StorageAccount {
            get
            {
                return _storageAccount;
            }
            set
            {
                _storageAccount = value;
                _blobClient = null;
            }
        }

        public Form1()
        {
            InitializeComponent();
            ConnectionString.Text = Settings.Default["StorageAccountConnectionString"].ToString();
            if (!string.IsNullOrWhiteSpace(ConnectionString.Text))
            {
                StorageAccount = CloudStorageAccount.Parse(ConnectionString.Text);
                EnumerateContainers();
            }
            ConnectButton.Enabled = true;
            StopButton.Enabled = false;
        }

        private void EnumerateContainers()
        {
            LogText.Text = "";
            BlobList.Items.Clear();
            ContainerList.Items.Clear();
            ContainerList.Items.Add("Select...");
            foreach (var container in BlobClient.ListContainers())
            {
                ContainerList.Items.Add(container.Name);
            }
        }

        private void EnumerateBlobs()
        {
            BlobList.Items.Clear();
            var container = GetSelectedContainer();
            if (container == null) return;

            var blobs = container
                .ListBlobs()
                .OfType<CloudBlob>()
                .OrderBy(b => b.Properties.LastModified);

            foreach (var blob in blobs)
            {
                if (blob?.Properties.BlobType == BlobType.AppendBlob)
                    BlobList.Items.Add(blob.Name);
            }
            if (BlobList.Items.Count > 0)
            {
                BlobList.SelectedIndex = BlobList.Items.Count - 1;
            }
        }

        private CloudBlobContainer GetSelectedContainer()
        {
            var name = ContainerList.SelectedItem.ToString();
            return name == "Select..." ? null : BlobClient.GetContainerReference(name);
        }

        private CloudBlob GetSelectedBlob()
        {
            var name = BlobList.SelectedItem.ToString();
            if (name == "Select...") return null;
            var container = GetSelectedContainer();
            return container?.GetBlobReference(name);
        }

        private bool RefreshBlobsInCurrentContainer()
        {
            BlobListLabel.ForeColor = Color.White;
            BlobListLabel.Refresh();
            var container = GetSelectedContainer();
            if (container == null) return false;
            var blobs = container.ListBlobs();
            if (blobs.Count() > BlobList.Items.Count)
            {
                //_tailWorker.CancelAsync();
                _currentBlob = null;
                _resetTail = true;
                EnumerateBlobs();
                return true;
            }
            BlobListLabel.ForeColor = SystemColors.ControlText;
            BlobListLabel.Refresh();
            return false;
        }

        private bool SetupConnection()
        {
            try
            {
                StorageAccount = CloudStorageAccount.Parse(ConnectionString.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
            return true;
        }

        private void TailBlob()
        {
            LogText.Text = "";
            _currentBlob = GetSelectedBlob();
            _resetTail = true;

            if (_currentBlob == null)
                return;

            if (_tailWorker == null)
            {
                _tailWorker = new BackgroundWorker {WorkerSupportsCancellation = false};
                _tailWorker.DoWork += TailWorker_DoWork;
                _tailWorker.ProgressChanged += TailWorker_ProgressChanged;
                _tailWorker.WorkerReportsProgress = true;
                //_tailWorker.WorkerSupportsCancellation = true;
                //var parameters = new object[] { blob };
                var parameters = new object[] { };
                _tailWorker.RunWorkerAsync(parameters);
            }
            ConnectButton.Enabled = false;
            StopButton.Enabled = true;
        }

        private void TailWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage == TailStatus.TextRead || e.ProgressPercentage == TailStatus.TextReadToEnd)
            {
                LogText.AppendText(e.UserState.ToString());
            }
            else if (e.ProgressPercentage == TailStatus.AtEnd)
            {
                RefreshBlobsInCurrentContainer();
            }
        }

        private void ConnectButton_Click(object sender, EventArgs e)
        {
            if (SetupConnection())
            {
                EnumerateContainers();
            }
        }

        private void ContainerList_SelectedIndexChanged(object sender, EventArgs e)
        {
            LogText.Text = "";
            EnumerateBlobs();
        }

        private void BlobList_SelectedIndexChanged(object sender, EventArgs e)
        {
            LogText.Text = "";
            TailBlob();
        }

        private void TailWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            var bufferSize = 4096;
            //var parameters = e.Argument as object[];
            //var blob = (CloudBlob) parameters[0];
            var bytesReceived = (long)0;

            do
            {
                //if (_tailWorker.CancellationPending)
                //{
                //    e.Cancel = true;
                //    return;
                //}

                while (_currentBlob == null)
                {
                    Thread.Sleep(SleepInterval);
                }

                if (_resetTail)
                {
                    bytesReceived = 0;
                    _resetTail = false;
                }

                _currentBlob.FetchAttributes();
                var blobSize = _currentBlob.Properties.Length;
                var bytesToDownload = (blobSize - bytesReceived <= bufferSize)
                    ? (int)(_currentBlob.Properties.Length - bytesReceived)
                    : bufferSize;

                var bytesFetched = 0;
                var text = string.Empty;
                var buffer = new byte[bytesToDownload];
                if (bytesToDownload > 0)
                {
                    bytesFetched = _currentBlob.DownloadRangeToByteArray(buffer, 0, bytesReceived, bytesToDownload);
                    bytesReceived += bytesFetched;
                    text = (System.Text.Encoding.ASCII.GetString(buffer));
                }

                int status;
                if (bytesFetched == 0)
                    status = TailStatus.AtEnd;
                else if (bytesFetched == bufferSize)
                    status = TailStatus.TextRead;
                else
                    status = TailStatus.TextReadToEnd;

                _tailWorker.ReportProgress(status, text);

                if (bytesFetched < bufferSize)
                {
                    var slept = 0;
                    while (slept < BlobCheckChangedInterval && (!_resetTail))
                    {
                        Thread.Sleep(SleepInterval);
                        slept += SleepInterval;
                    }
                }

            } while (true);
        }

        private void StopButton_Click(object sender, EventArgs e)
        {
            try
            {
                //_tailWorker.CancelAsync();
                _currentBlob = null;
                _resetTail = true;
                EnumerateContainers();
                ConnectButton.Enabled = true;
            }
            catch (Exception)
            {
                // ignored
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Settings.Default["StorageAccountConnectionString"] = ConnectionString.Text;
            Settings.Default.Save();
        }

        private void FontBigger_Click(object sender, EventArgs e)
        {
            var font = LogText.Font;
            LogText.Font = new Font(font.FontFamily, font.Size + 2);
        }

        private void FontSmaller_Click(object sender, EventArgs e)
        {
            var font = LogText.Font;
            LogText.Font = new Font(font.FontFamily, font.Size - 2);
        }
    }

}