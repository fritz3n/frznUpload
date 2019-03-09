﻿namespace frznUpload.Client
{
    partial class MainForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.StatusLabel = new System.Windows.Forms.Label();
            this.ProgressBar = new System.Windows.Forms.ProgressBar();
            this.AccountButton = new System.Windows.Forms.Button();
            this.TabController = new System.Windows.Forms.TabControl();
            this.UploadTab = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.ShareGroupBox = new System.Windows.Forms.GroupBox();
            this.CopyButton = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.LinkText = new System.Windows.Forms.TextBox();
            this.SharePanel = new System.Windows.Forms.TableLayoutPanel();
            this.FirstViewBox = new System.Windows.Forms.CheckBox();
            this.PublicBox = new System.Windows.Forms.CheckBox();
            this.WhitelistText = new System.Windows.Forms.TextBox();
            this.PublicRegisteredBox = new System.Windows.Forms.CheckBox();
            this.WhitelistedBox = new System.Windows.Forms.CheckBox();
            this.ShareBox = new System.Windows.Forms.CheckBox();
            this.FileGroupBox = new System.Windows.Forms.GroupBox();
            this.UploadButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.PathText = new System.Windows.Forms.TextBox();
            this.FilenameText = new System.Windows.Forms.TextBox();
            this.BrowseButton = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.FileTab = new System.Windows.Forms.TabPage();
            this.FileView = new System.Windows.Forms.ListView();
            this.FilenameHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.SizeHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.contextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.SettingsButton = new System.Windows.Forms.Button();
            this.TabController.SuspendLayout();
            this.UploadTab.SuspendLayout();
            this.tableLayoutPanel.SuspendLayout();
            this.ShareGroupBox.SuspendLayout();
            this.SharePanel.SuspendLayout();
            this.FileGroupBox.SuspendLayout();
            this.FileTab.SuspendLayout();
            this.contextMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // StatusLabel
            // 
            this.StatusLabel.Location = new System.Drawing.Point(13, 425);
            this.StatusLabel.Name = "StatusLabel";
            this.StatusLabel.Size = new System.Drawing.Size(132, 16);
            this.StatusLabel.TabIndex = 0;
            this.StatusLabel.Text = "Hello";
            // 
            // ProgressBar
            // 
            this.ProgressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ProgressBar.Location = new System.Drawing.Point(167, 420);
            this.ProgressBar.Name = "ProgressBar";
            this.ProgressBar.Size = new System.Drawing.Size(466, 23);
            this.ProgressBar.TabIndex = 1;
            // 
            // AccountButton
            // 
            this.AccountButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.AccountButton.Location = new System.Drawing.Point(639, 420);
            this.AccountButton.Name = "AccountButton";
            this.AccountButton.Size = new System.Drawing.Size(68, 23);
            this.AccountButton.TabIndex = 2;
            this.AccountButton.Text = "Account";
            this.AccountButton.UseVisualStyleBackColor = true;
            this.AccountButton.Click += new System.EventHandler(this.AccountButton_Click);
            // 
            // TabController
            // 
            this.TabController.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TabController.CausesValidation = false;
            this.TabController.Controls.Add(this.UploadTab);
            this.TabController.Controls.Add(this.FileTab);
            this.TabController.HotTrack = true;
            this.TabController.Location = new System.Drawing.Point(13, 13);
            this.TabController.Name = "TabController";
            this.TabController.SelectedIndex = 0;
            this.TabController.Size = new System.Drawing.Size(775, 401);
            this.TabController.TabIndex = 3;
            this.TabController.Selecting += new System.Windows.Forms.TabControlCancelEventHandler(this.Tab_Selecting);
            // 
            // UploadTab
            // 
            this.UploadTab.BackColor = System.Drawing.SystemColors.Control;
            this.UploadTab.Controls.Add(this.tableLayoutPanel);
            this.UploadTab.Location = new System.Drawing.Point(4, 22);
            this.UploadTab.Name = "UploadTab";
            this.UploadTab.Padding = new System.Windows.Forms.Padding(3);
            this.UploadTab.Size = new System.Drawing.Size(767, 375);
            this.UploadTab.TabIndex = 0;
            this.UploadTab.Text = "Upload";
            // 
            // tableLayoutPanel
            // 
            this.tableLayoutPanel.ColumnCount = 2;
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel.Controls.Add(this.ShareGroupBox, 1, 0);
            this.tableLayoutPanel.Controls.Add(this.FileGroupBox, 0, 0);
            this.tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            this.tableLayoutPanel.RowCount = 1;
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel.Size = new System.Drawing.Size(761, 369);
            this.tableLayoutPanel.TabIndex = 8;
            // 
            // ShareGroupBox
            // 
            this.ShareGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ShareGroupBox.Controls.Add(this.CopyButton);
            this.ShareGroupBox.Controls.Add(this.label3);
            this.ShareGroupBox.Controls.Add(this.LinkText);
            this.ShareGroupBox.Controls.Add(this.SharePanel);
            this.ShareGroupBox.Controls.Add(this.ShareBox);
            this.ShareGroupBox.Location = new System.Drawing.Point(383, 3);
            this.ShareGroupBox.Name = "ShareGroupBox";
            this.ShareGroupBox.Size = new System.Drawing.Size(375, 176);
            this.ShareGroupBox.TabIndex = 6;
            this.ShareGroupBox.TabStop = false;
            this.ShareGroupBox.Text = "Sharing";
            // 
            // CopyButton
            // 
            this.CopyButton.Location = new System.Drawing.Point(294, 150);
            this.CopyButton.Name = "CopyButton";
            this.CopyButton.Size = new System.Drawing.Size(75, 23);
            this.CopyButton.TabIndex = 9;
            this.CopyButton.Text = "Copy";
            this.CopyButton.UseVisualStyleBackColor = true;
            this.CopyButton.Click += new System.EventHandler(this.CopyButton_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(7, 136);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(27, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "Link";
            // 
            // LinkText
            // 
            this.LinkText.Location = new System.Drawing.Point(6, 152);
            this.LinkText.Name = "LinkText";
            this.LinkText.ReadOnly = true;
            this.LinkText.Size = new System.Drawing.Size(282, 20);
            this.LinkText.TabIndex = 7;
            // 
            // SharePanel
            // 
            this.SharePanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.SharePanel.ColumnCount = 2;
            this.SharePanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.SharePanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.SharePanel.Controls.Add(this.FirstViewBox, 0, 0);
            this.SharePanel.Controls.Add(this.PublicBox, 0, 1);
            this.SharePanel.Controls.Add(this.WhitelistText, 1, 1);
            this.SharePanel.Controls.Add(this.PublicRegisteredBox, 0, 2);
            this.SharePanel.Controls.Add(this.WhitelistedBox, 1, 0);
            this.SharePanel.Location = new System.Drawing.Point(6, 43);
            this.SharePanel.Name = "SharePanel";
            this.SharePanel.RowCount = 3;
            this.SharePanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.SharePanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.SharePanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
            this.SharePanel.Size = new System.Drawing.Size(369, 86);
            this.SharePanel.TabIndex = 6;
            // 
            // FirstViewBox
            // 
            this.FirstViewBox.AutoSize = true;
            this.FirstViewBox.Location = new System.Drawing.Point(3, 3);
            this.FirstViewBox.Name = "FirstViewBox";
            this.FirstViewBox.Size = new System.Drawing.Size(71, 17);
            this.FirstViewBox.TabIndex = 0;
            this.FirstViewBox.Text = "First View";
            this.FirstViewBox.UseVisualStyleBackColor = true;
            // 
            // PublicBox
            // 
            this.PublicBox.AutoSize = true;
            this.PublicBox.Checked = true;
            this.PublicBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.PublicBox.Location = new System.Drawing.Point(3, 31);
            this.PublicBox.Name = "PublicBox";
            this.PublicBox.Size = new System.Drawing.Size(55, 17);
            this.PublicBox.TabIndex = 1;
            this.PublicBox.Text = "Public";
            this.PublicBox.UseVisualStyleBackColor = true;
            // 
            // WhitelistText
            // 
            this.WhitelistText.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.WhitelistText.Enabled = false;
            this.WhitelistText.Location = new System.Drawing.Point(187, 31);
            this.WhitelistText.Name = "WhitelistText";
            this.WhitelistText.Size = new System.Drawing.Size(179, 20);
            this.WhitelistText.TabIndex = 4;
            // 
            // PublicRegisteredBox
            // 
            this.PublicRegisteredBox.AutoSize = true;
            this.PublicRegisteredBox.Checked = true;
            this.PublicRegisteredBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.PublicRegisteredBox.Location = new System.Drawing.Point(3, 59);
            this.PublicRegisteredBox.Name = "PublicRegisteredBox";
            this.PublicRegisteredBox.Size = new System.Drawing.Size(121, 17);
            this.PublicRegisteredBox.TabIndex = 2;
            this.PublicRegisteredBox.Text = "Public to Registered";
            this.PublicRegisteredBox.UseVisualStyleBackColor = true;
            // 
            // WhitelistedBox
            // 
            this.WhitelistedBox.AutoSize = true;
            this.WhitelistedBox.Location = new System.Drawing.Point(187, 3);
            this.WhitelistedBox.Name = "WhitelistedBox";
            this.WhitelistedBox.Size = new System.Drawing.Size(110, 17);
            this.WhitelistedBox.TabIndex = 3;
            this.WhitelistedBox.Text = "Whitelist enabled:";
            this.WhitelistedBox.UseVisualStyleBackColor = true;
            this.WhitelistedBox.CheckedChanged += new System.EventHandler(this.WhitelistedBox_CheckedChanged);
            // 
            // ShareBox
            // 
            this.ShareBox.AutoSize = true;
            this.ShareBox.Checked = true;
            this.ShareBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ShareBox.Location = new System.Drawing.Point(6, 19);
            this.ShareBox.Name = "ShareBox";
            this.ShareBox.Size = new System.Drawing.Size(54, 17);
            this.ShareBox.TabIndex = 5;
            this.ShareBox.Text = "Share";
            this.ShareBox.UseVisualStyleBackColor = true;
            this.ShareBox.CheckedChanged += new System.EventHandler(this.ShareBox_CheckedChanged);
            // 
            // FileGroupBox
            // 
            this.FileGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.FileGroupBox.Controls.Add(this.UploadButton);
            this.FileGroupBox.Controls.Add(this.label1);
            this.FileGroupBox.Controls.Add(this.PathText);
            this.FileGroupBox.Controls.Add(this.FilenameText);
            this.FileGroupBox.Controls.Add(this.BrowseButton);
            this.FileGroupBox.Controls.Add(this.label2);
            this.FileGroupBox.Location = new System.Drawing.Point(3, 3);
            this.FileGroupBox.Name = "FileGroupBox";
            this.FileGroupBox.Size = new System.Drawing.Size(374, 176);
            this.FileGroupBox.TabIndex = 7;
            this.FileGroupBox.TabStop = false;
            this.FileGroupBox.Text = "File";
            // 
            // UploadButton
            // 
            this.UploadButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.UploadButton.Enabled = false;
            this.UploadButton.Location = new System.Drawing.Point(232, 77);
            this.UploadButton.Name = "UploadButton";
            this.UploadButton.Size = new System.Drawing.Size(130, 23);
            this.UploadButton.TabIndex = 5;
            this.UploadButton.Text = "Upload";
            this.UploadButton.UseVisualStyleBackColor = true;
            this.UploadButton.Click += new System.EventHandler(this.UploadButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Path";
            // 
            // PathText
            // 
            this.PathText.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PathText.Location = new System.Drawing.Point(9, 36);
            this.PathText.Name = "PathText";
            this.PathText.Size = new System.Drawing.Size(278, 20);
            this.PathText.TabIndex = 0;
            this.PathText.TextChanged += new System.EventHandler(this.PathText_TextChanged);
            // 
            // FilenameText
            // 
            this.FilenameText.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.FilenameText.Location = new System.Drawing.Point(9, 79);
            this.FilenameText.Name = "FilenameText";
            this.FilenameText.Size = new System.Drawing.Size(217, 20);
            this.FilenameText.TabIndex = 4;
            // 
            // BrowseButton
            // 
            this.BrowseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.BrowseButton.Location = new System.Drawing.Point(293, 36);
            this.BrowseButton.Name = "BrowseButton";
            this.BrowseButton.Size = new System.Drawing.Size(75, 20);
            this.BrowseButton.TabIndex = 2;
            this.BrowseButton.Text = "Browse...";
            this.BrowseButton.UseVisualStyleBackColor = true;
            this.BrowseButton.Click += new System.EventHandler(this.BrowseButton_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 63);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(49, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Filename";
            // 
            // FileTab
            // 
            this.FileTab.BackColor = System.Drawing.SystemColors.Control;
            this.FileTab.Controls.Add(this.FileView);
            this.FileTab.Location = new System.Drawing.Point(4, 22);
            this.FileTab.Name = "FileTab";
            this.FileTab.Padding = new System.Windows.Forms.Padding(3);
            this.FileTab.Size = new System.Drawing.Size(767, 375);
            this.FileTab.TabIndex = 1;
            this.FileTab.Text = "Files";
            // 
            // FileView
            // 
            this.FileView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.FilenameHeader,
            this.SizeHeader});
            this.FileView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.FileView.FullRowSelect = true;
            this.FileView.Location = new System.Drawing.Point(3, 3);
            this.FileView.Name = "FileView";
            this.FileView.Size = new System.Drawing.Size(761, 369);
            this.FileView.Sorting = System.Windows.Forms.SortOrder.Descending;
            this.FileView.TabIndex = 0;
            this.FileView.UseCompatibleStateImageBehavior = false;
            this.FileView.View = System.Windows.Forms.View.Details;
            this.FileView.MouseClick += new System.Windows.Forms.MouseEventHandler(this.FileView_MouseClick);
            // 
            // FilenameHeader
            // 
            this.FilenameHeader.Text = "Filename";
            this.FilenameHeader.Width = 465;
            // 
            // SizeHeader
            // 
            this.SizeHeader.Text = "Size";
            this.SizeHeader.Width = 122;
            // 
            // openFileDialog
            // 
            this.openFileDialog.FileName = "openFileDialog";
            // 
            // contextMenu
            // 
            this.contextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.deleteToolStripMenuItem});
            this.contextMenu.Name = "contextMenu";
            this.contextMenu.Size = new System.Drawing.Size(132, 26);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Delete;
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(131, 22);
            this.deleteToolStripMenuItem.Text = "Delete";
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.DeleteToolStripMenuItem_ClickAsync);
            // 
            // SettingsButton
            // 
            this.SettingsButton.Location = new System.Drawing.Point(713, 420);
            this.SettingsButton.Name = "SettingsButton";
            this.SettingsButton.Size = new System.Drawing.Size(75, 23);
            this.SettingsButton.TabIndex = 4;
            this.SettingsButton.Text = "Settings";
            this.SettingsButton.UseVisualStyleBackColor = true;
            this.SettingsButton.Click += new System.EventHandler(this.SettingsButton_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.SettingsButton);
            this.Controls.Add(this.TabController);
            this.Controls.Add(this.AccountButton);
            this.Controls.Add(this.ProgressBar);
            this.Controls.Add(this.StatusLabel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.Text = "frznUpload";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.TabController.ResumeLayout(false);
            this.UploadTab.ResumeLayout(false);
            this.tableLayoutPanel.ResumeLayout(false);
            this.ShareGroupBox.ResumeLayout(false);
            this.ShareGroupBox.PerformLayout();
            this.SharePanel.ResumeLayout(false);
            this.SharePanel.PerformLayout();
            this.FileGroupBox.ResumeLayout(false);
            this.FileGroupBox.PerformLayout();
            this.FileTab.ResumeLayout(false);
            this.contextMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label StatusLabel;
        private System.Windows.Forms.ProgressBar ProgressBar;
        private System.Windows.Forms.Button AccountButton;
        private System.Windows.Forms.TabControl TabController;
        private System.Windows.Forms.TabPage UploadTab;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox PathText;
        private System.Windows.Forms.TabPage FileTab;
        private System.Windows.Forms.TextBox FilenameText;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button BrowseButton;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.GroupBox FileGroupBox;
        private System.Windows.Forms.Button UploadButton;
        private System.Windows.Forms.GroupBox ShareGroupBox;
        private System.Windows.Forms.TableLayoutPanel SharePanel;
        private System.Windows.Forms.CheckBox FirstViewBox;
        private System.Windows.Forms.CheckBox PublicBox;
        private System.Windows.Forms.CheckBox PublicRegisteredBox;
        private System.Windows.Forms.CheckBox WhitelistedBox;
        private System.Windows.Forms.CheckBox ShareBox;
        private System.Windows.Forms.TextBox WhitelistText;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox LinkText;
        private System.Windows.Forms.Button CopyButton;
        private System.Windows.Forms.ListView FileView;
        private System.Windows.Forms.ColumnHeader FilenameHeader;
        private System.Windows.Forms.ColumnHeader SizeHeader;
        private System.Windows.Forms.ContextMenuStrip contextMenu;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private System.Windows.Forms.Button SettingsButton;
    }
}
