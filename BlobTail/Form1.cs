using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using BlobTail.Exceptions;
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
            public static readonly int TextRead = 1;
            public static readonly int TextReadToEnd = 2;
            public static readonly int AtEnd = 3;

            public static readonly int ErrorThreshold = 50;

            public static readonly int UnknownError = 51;
            public static readonly int BlobNotFoundError = 52;
        }

        private readonly BackgroundWorker _tailWorker;
        private readonly object _resetLock = new object();
        private bool _doReset = false;
        private CloudBlob _currentBlob;

        private CloudBlobClient _blobClient;
        private CloudBlobClient BlobClient => _blobClient ?? (_blobClient = StorageAccount.CreateCloudBlobClient());

        private CloudStorageAccount _storageAccount;

        public CloudStorageAccount StorageAccount
        {
            get { return _storageAccount; }
            set
            {
                _storageAccount = value;
                _blobClient = null;
            }
        }

        public Form1()
        {
            InitializeComponent();

            _tailWorker = new BackgroundWorker {WorkerSupportsCancellation = false};
            _tailWorker.DoWork += TailWorker_DoWork;
            _tailWorker.ProgressChanged += TailWorker_ProgressChanged;
            _tailWorker.WorkerReportsProgress = true;
            var parameters = new object[] {};
            _tailWorker.RunWorkerAsync(parameters);

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
            BlobList.Items.Clear();
            ContainerList.Items.Clear();
            ContainerList.Items.Add("Select...");
            foreach (var container in BlobClient.ListContainers())
            {
                ContainerList.Items.Add(container.Name);
            }
            ContainerList.SelectedIndex = 0;
        }

        private void EnumerateBlobs()
        {
            BlobList.Items.Clear();
            var container = GetSelectedContainer();
            if (container == null) return;

            var blobs = GetBlobs(container);
            //.ListBlobs()
            //.OfType<CloudBlob>()
            //.OrderBy(b => b.Properties.LastModified);

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

        private void RefreshBlobsInCurrentContainer()
        {
            BlobListLabel.ForeColor = Color.White;
            BlobListLabel.Refresh();
            var container = GetSelectedContainer();
            if (container == null) return;
            var blobs = GetBlobs(container);
            if (blobs.Count() != BlobList.Items.Count)
            {
                ResetTail();
                EnumerateBlobs();
                //StartTail();
                return;
            }
            BlobListLabel.ForeColor = SystemColors.ControlText;
            BlobListLabel.Refresh();
            return;
        }

        private void StartTail()
        {
            _currentBlob = GetSelectedBlob();
            if (_currentBlob == null)
                return;

            LogText.Text = "";
            _doReset = true;

            ConnectionString.Enabled = false;
            ConnectButton.Enabled = false;
            StopButton.Enabled = true;
        }

        private void ResetTail()
        {
            lock (_resetLock)
            {
                _currentBlob = null;
                _doReset = true;
                LogText.Text = "";

                ConnectionString.Enabled = true;
                ConnectButton.Enabled = true;
                StopButton.Enabled = false;
            }
        }

        private void TailWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            const int bufferSize = 4096;
            //var parameters = e.Argument as object[];
            //var blob = (CloudBlob) parameters[0];
            var bytesReceived = (long) 0;

            do
            {
                if (_tailWorker.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }

                while (_currentBlob == null)
                {
                    Thread.Sleep(SleepInterval);
                }

                if (_doReset)
                {
                    lock (_resetLock)
                    {
                        bytesReceived = 0;
                        _doReset = false;
                    }
                }

                int status; 
                var text = string.Empty;
                var bytesFetched = 0;

                try
                {
                    if (!_currentBlob.Exists())
                    {
                        throw new BlobNotFoundException($"The blob \"{_currentBlob?.Name ?? "[unknown]"}\" does not exist.");
                    }

                    _currentBlob.FetchAttributes();
                    var blobSize = _currentBlob.Properties.Length;
                    var bytesToDownload = (blobSize - bytesReceived <= bufferSize)
                        ? (int) (_currentBlob.Properties.Length - bytesReceived)
                        : bufferSize;

                    var buffer = new byte[bytesToDownload];
                    if (bytesToDownload > 0)
                    {
                        bytesFetched = _currentBlob.DownloadRangeToByteArray(buffer, 0, bytesReceived, bytesToDownload);
                        bytesReceived += bytesFetched;
                        text = (System.Text.Encoding.ASCII.GetString(buffer));
                    }

                    if (bytesFetched == 0)
                        status = TailStatus.AtEnd;
                    else if (bytesFetched == bufferSize)
                        status = TailStatus.TextRead;
                    else
                        status = TailStatus.TextReadToEnd;
                }
                catch (BlobNotFoundException blobNotFoundEx)
                {
                    _currentBlob = null;
                    status = TailStatus.BlobNotFoundError;
                    text = string.IsNullOrWhiteSpace(blobNotFoundEx.Message) 
                        ? "The blob does not exist."
                        : blobNotFoundEx.Message;

                }
                catch (Exception ex)
                {
                    _currentBlob = null;
                    status = TailStatus.UnknownError;
                    text = ex.Message;
                }

                _tailWorker.ReportProgress(status, text);

                if (bytesFetched < bufferSize && status < TailStatus.ErrorThreshold)
                {
                    var slept = 0;
                    while (slept < BlobCheckChangedInterval && (!_doReset))
                    {
                        Thread.Sleep(SleepInterval);
                        slept += SleepInterval;
                    }
                }

            } while (true);
        }
        private void TailWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (
                e.ProgressPercentage == TailStatus.TextRead 
                || e.ProgressPercentage == TailStatus.TextReadToEnd
                )
            {
                LogText.AppendText(e.UserState.ToString());
            }
            else if (e.ProgressPercentage == TailStatus.AtEnd)
            {
                RefreshBlobsInCurrentContainer();
            }
            else if (e.ProgressPercentage > TailStatus.ErrorThreshold)
            {
                ResetTail();
                MessageBox.Show(e.UserState?.ToString() ?? "Unknown error.");
                EnumerateBlobs();
            }
        }

        private void ConnectButton_Click(object sender, EventArgs e)
        {
            try
            {
                StorageAccount = CloudStorageAccount.Parse(ConnectionString.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }

            EnumerateContainers();
        }


        private void ContainerList_SelectedIndexChanged(object sender, EventArgs e)
        {
            ResetTail();
            EnumerateBlobs();
        }

        private void BlobList_SelectedIndexChanged(object sender, EventArgs e)
        {
            StartTail();
        }

        private void StopButton_Click(object sender, EventArgs e)
        {
            ResetTail();
            EnumerateContainers();
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

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Settings.Default["StorageAccountConnectionString"] = ConnectionString.Text;
            Settings.Default.Save();
        }


        private static IOrderedEnumerable<CloudBlob> GetBlobs(CloudBlobContainer container)
        {
            return container
                .ListBlobs()
                .OfType<CloudBlob>()
                .OrderBy(b => b.Properties.LastModified);
        }

    }
}