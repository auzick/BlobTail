namespace BlobTail
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.ConnectionString = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.BlobListLabel = new System.Windows.Forms.Label();
            this.LogText = new System.Windows.Forms.TextBox();
            this.ConnectButton = new System.Windows.Forms.Button();
            this.ContainerList = new System.Windows.Forms.ComboBox();
            this.BlobList = new System.Windows.Forms.ComboBox();
            this.StopButton = new System.Windows.Forms.Button();
            this.FontBigger = new System.Windows.Forms.Label();
            this.FontSmaller = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(11, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(89, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Connection string";
            // 
            // ConnectionString
            // 
            this.ConnectionString.Location = new System.Drawing.Point(106, 12);
            this.ConnectionString.Name = "ConnectionString";
            this.ConnectionString.Size = new System.Drawing.Size(374, 20);
            this.ConnectionString.TabIndex = 1;
            this.ConnectionString.Text = global::BlobTail.Properties.Settings.Default.StorageAccountConnectionString;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(11, 41);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(52, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Container";
            // 
            // BlobListLabel
            // 
            this.BlobListLabel.AutoSize = true;
            this.BlobListLabel.Location = new System.Drawing.Point(11, 71);
            this.BlobListLabel.Name = "BlobListLabel";
            this.BlobListLabel.Size = new System.Drawing.Size(28, 13);
            this.BlobListLabel.TabIndex = 4;
            this.BlobListLabel.Text = "Blob";
            // 
            // LogText
            // 
            this.LogText.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.LogText.Location = new System.Drawing.Point(14, 95);
            this.LogText.Multiline = true;
            this.LogText.Name = "LogText";
            this.LogText.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.LogText.Size = new System.Drawing.Size(835, 324);
            this.LogText.TabIndex = 6;
            // 
            // ConnectButton
            // 
            this.ConnectButton.Location = new System.Drawing.Point(486, 12);
            this.ConnectButton.Name = "ConnectButton";
            this.ConnectButton.Size = new System.Drawing.Size(75, 23);
            this.ConnectButton.TabIndex = 8;
            this.ConnectButton.Text = "Connect";
            this.ConnectButton.UseVisualStyleBackColor = true;
            this.ConnectButton.Click += new System.EventHandler(this.ConnectButton_Click);
            // 
            // ContainerList
            // 
            this.ContainerList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ContainerList.FormattingEnabled = true;
            this.ContainerList.Location = new System.Drawing.Point(106, 41);
            this.ContainerList.Name = "ContainerList";
            this.ContainerList.Size = new System.Drawing.Size(374, 21);
            this.ContainerList.TabIndex = 9;
            this.ContainerList.SelectedIndexChanged += new System.EventHandler(this.ContainerList_SelectedIndexChanged);
            // 
            // BlobList
            // 
            this.BlobList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.BlobList.FormattingEnabled = true;
            this.BlobList.Location = new System.Drawing.Point(106, 68);
            this.BlobList.Name = "BlobList";
            this.BlobList.Size = new System.Drawing.Size(374, 21);
            this.BlobList.TabIndex = 10;
            this.BlobList.SelectedIndexChanged += new System.EventHandler(this.BlobList_SelectedIndexChanged);
            // 
            // StopButton
            // 
            this.StopButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.StopButton.Location = new System.Drawing.Point(14, 439);
            this.StopButton.Name = "StopButton";
            this.StopButton.Size = new System.Drawing.Size(75, 23);
            this.StopButton.TabIndex = 11;
            this.StopButton.Text = "Stop";
            this.StopButton.UseVisualStyleBackColor = true;
            this.StopButton.Click += new System.EventHandler(this.StopButton_Click);
            // 
            // FontBigger
            // 
            this.FontBigger.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.FontBigger.AutoSize = true;
            this.FontBigger.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FontBigger.Location = new System.Drawing.Point(814, 69);
            this.FontBigger.Name = "FontBigger";
            this.FontBigger.Size = new System.Drawing.Size(19, 20);
            this.FontBigger.TabIndex = 12;
            this.FontBigger.Text = "+";
            this.FontBigger.Click += new System.EventHandler(this.FontBigger_Click);
            // 
            // FontSmaller
            // 
            this.FontSmaller.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.FontSmaller.AutoSize = true;
            this.FontSmaller.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FontSmaller.Location = new System.Drawing.Point(833, 69);
            this.FontSmaller.Name = "FontSmaller";
            this.FontSmaller.Size = new System.Drawing.Size(15, 20);
            this.FontSmaller.TabIndex = 13;
            this.FontSmaller.Text = "-";
            this.FontSmaller.Click += new System.EventHandler(this.FontSmaller_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(861, 474);
            this.Controls.Add(this.FontSmaller);
            this.Controls.Add(this.FontBigger);
            this.Controls.Add(this.StopButton);
            this.Controls.Add(this.BlobList);
            this.Controls.Add(this.ContainerList);
            this.Controls.Add(this.ConnectButton);
            this.Controls.Add(this.LogText);
            this.Controls.Add(this.BlobListLabel);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.ConnectionString);
            this.Controls.Add(this.label1);
            this.Name = "Form1";
            this.Text = "BlobTail";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox ConnectionString;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label BlobListLabel;
        private System.Windows.Forms.TextBox LogText;
        private System.Windows.Forms.Button ConnectButton;
        private System.Windows.Forms.ComboBox ContainerList;
        private System.Windows.Forms.ComboBox BlobList;
        private System.Windows.Forms.Button StopButton;
        private System.Windows.Forms.Label FontBigger;
        private System.Windows.Forms.Label FontSmaller;
    }
}

