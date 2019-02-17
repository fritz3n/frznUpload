namespace frznUpload.Client
{
    partial class SettingsForm
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
            this.SettingsTabCtrl = new System.Windows.Forms.TabControl();
            this.HotkeyTab = new System.Windows.Forms.TabPage();
            this.HotkeyLayout = new System.Windows.Forms.FlowLayoutPanel();
            this.OtherTab = new System.Windows.Forms.TabPage();
            this.AutostartTab = new System.Windows.Forms.TabPage();
            this.autostartCheckbox = new System.Windows.Forms.CheckBox();
            this.accountTab = new System.Windows.Forms.TabPage();
            this.scanLable = new System.Windows.Forms.Label();
            this.picture_qr = new System.Windows.Forms.PictureBox();
            this.TwoFaCheckbox = new System.Windows.Forms.CheckBox();
            this.AddButton = new System.Windows.Forms.Button();
            this.ExplorerIntegrationBox = new System.Windows.Forms.CheckBox();
            this.SettingsTabCtrl.SuspendLayout();
            this.HotkeyTab.SuspendLayout();
            this.AutostartTab.SuspendLayout();
            this.accountTab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picture_qr)).BeginInit();
            this.SuspendLayout();
            // 
            // SettingsTabCtrl
            // 
            this.SettingsTabCtrl.Controls.Add(this.HotkeyTab);
            this.SettingsTabCtrl.Controls.Add(this.OtherTab);
            this.SettingsTabCtrl.Controls.Add(this.AutostartTab);
            this.SettingsTabCtrl.Controls.Add(this.accountTab);
            this.SettingsTabCtrl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SettingsTabCtrl.Location = new System.Drawing.Point(0, 0);
            this.SettingsTabCtrl.Name = "SettingsTabCtrl";
            this.SettingsTabCtrl.SelectedIndex = 0;
            this.SettingsTabCtrl.Size = new System.Drawing.Size(860, 450);
            this.SettingsTabCtrl.TabIndex = 0;
            this.SettingsTabCtrl.SelectedIndexChanged += new System.EventHandler(this.SettingsTabCtrl_TabIndexChangedAsync);
            this.SettingsTabCtrl.TabIndexChanged += new System.EventHandler(this.SettingsTabCtrl_TabIndexChangedAsync);
            // 
            // HotkeyTab
            // 
            this.HotkeyTab.BackColor = System.Drawing.SystemColors.Control;
            this.HotkeyTab.Controls.Add(this.HotkeyLayout);
            this.HotkeyTab.Location = new System.Drawing.Point(4, 22);
            this.HotkeyTab.Name = "HotkeyTab";
            this.HotkeyTab.Padding = new System.Windows.Forms.Padding(3);
            this.HotkeyTab.Size = new System.Drawing.Size(852, 424);
            this.HotkeyTab.TabIndex = 0;
            this.HotkeyTab.Text = "Hotkeys";
            // 
            // HotkeyLayout
            // 
            this.HotkeyLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.HotkeyLayout.Location = new System.Drawing.Point(3, 3);
            this.HotkeyLayout.Name = "HotkeyLayout";
            this.HotkeyLayout.Size = new System.Drawing.Size(846, 418);
            this.HotkeyLayout.TabIndex = 0;
            // 
            // OtherTab
            // 
            this.OtherTab.BackColor = System.Drawing.SystemColors.Control;
            this.OtherTab.Location = new System.Drawing.Point(4, 22);
            this.OtherTab.Name = "OtherTab";
            this.OtherTab.Padding = new System.Windows.Forms.Padding(3);
            this.OtherTab.Size = new System.Drawing.Size(852, 424);
            this.OtherTab.TabIndex = 1;
            this.OtherTab.Text = "Other";
            // 
            // AutostartTab
            // 
            this.AutostartTab.Controls.Add(this.ExplorerIntegrationBox);
            this.AutostartTab.Controls.Add(this.autostartCheckbox);
            this.AutostartTab.Location = new System.Drawing.Point(4, 22);
            this.AutostartTab.Name = "AutostartTab";
            this.AutostartTab.Padding = new System.Windows.Forms.Padding(3);
            this.AutostartTab.Size = new System.Drawing.Size(852, 424);
            this.AutostartTab.TabIndex = 2;
            this.AutostartTab.Text = "Config";
            this.AutostartTab.UseVisualStyleBackColor = true;
            // 
            // autostartCheckbox
            // 
            this.autostartCheckbox.AutoSize = true;
            this.autostartCheckbox.Location = new System.Drawing.Point(24, 31);
            this.autostartCheckbox.Name = "autostartCheckbox";
            this.autostartCheckbox.Size = new System.Drawing.Size(68, 17);
            this.autostartCheckbox.TabIndex = 0;
            this.autostartCheckbox.Text = "Autostart";
            this.autostartCheckbox.UseVisualStyleBackColor = true;
            this.autostartCheckbox.CheckedChanged += new System.EventHandler(this.CheckBox1_CheckedChanged);
            // 
            // accountTab
            // 
            this.accountTab.Controls.Add(this.scanLable);
            this.accountTab.Controls.Add(this.picture_qr);
            this.accountTab.Controls.Add(this.TwoFaCheckbox);
            this.accountTab.Location = new System.Drawing.Point(4, 22);
            this.accountTab.Name = "accountTab";
            this.accountTab.Padding = new System.Windows.Forms.Padding(3);
            this.accountTab.Size = new System.Drawing.Size(852, 424);
            this.accountTab.TabIndex = 3;
            this.accountTab.Text = "Account";
            this.accountTab.UseVisualStyleBackColor = true;
            // 
            // scanLable
            // 
            this.scanLable.AutoSize = true;
            this.scanLable.Location = new System.Drawing.Point(367, 31);
            this.scanLable.Name = "scanLable";
            this.scanLable.Size = new System.Drawing.Size(136, 13);
            this.scanLable.TabIndex = 2;
            this.scanLable.Text = "Scan this code in your app:";
            this.scanLable.Visible = false;
            // 
            // picture_qr
            // 
            this.picture_qr.Location = new System.Drawing.Point(506, 27);
            this.picture_qr.Name = "picture_qr";
            this.picture_qr.Size = new System.Drawing.Size(256, 256);
            this.picture_qr.TabIndex = 1;
            this.picture_qr.TabStop = false;
            // 
            // TwoFaCheckbox
            // 
            this.TwoFaCheckbox.AutoSize = true;
            this.TwoFaCheckbox.Enabled = false;
            this.TwoFaCheckbox.Location = new System.Drawing.Point(21, 27);
            this.TwoFaCheckbox.Name = "TwoFaCheckbox";
            this.TwoFaCheckbox.Size = new System.Drawing.Size(151, 17);
            this.TwoFaCheckbox.TabIndex = 0;
            this.TwoFaCheckbox.Text = "Two Factor Authentication";
            this.TwoFaCheckbox.UseVisualStyleBackColor = true;
            this.TwoFaCheckbox.CheckedChanged += new System.EventHandler(this.TwoFaCheckbox_CheckedChangedAsync);
            // 
            // AddButton
            // 
            this.AddButton.BackColor = System.Drawing.SystemColors.ControlLight;
            this.AddButton.Location = new System.Drawing.Point(20, 20);
            this.AddButton.Margin = new System.Windows.Forms.Padding(20);
            this.AddButton.Name = "AddButton";
            this.AddButton.Padding = new System.Windows.Forms.Padding(5);
            this.AddButton.Size = new System.Drawing.Size(32, 32);
            this.AddButton.TabIndex = 0;
            this.AddButton.Text = "+";
            this.AddButton.UseVisualStyleBackColor = false;
            this.AddButton.Click += new System.EventHandler(this.AddButton_Click);
            // 
            // ExplorerIntegrationBox
            // 
            this.ExplorerIntegrationBox.AutoSize = true;
            this.ExplorerIntegrationBox.Location = new System.Drawing.Point(24, 55);
            this.ExplorerIntegrationBox.Name = "ExplorerIntegrationBox";
            this.ExplorerIntegrationBox.Size = new System.Drawing.Size(117, 17);
            this.ExplorerIntegrationBox.TabIndex = 1;
            this.ExplorerIntegrationBox.Text = "Explorer Integration";
            this.ExplorerIntegrationBox.UseVisualStyleBackColor = true;
            this.ExplorerIntegrationBox.CheckedChanged += new System.EventHandler(this.ExplorerIntegrationBox_CheckedChanged);
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(860, 450);
            this.Controls.Add(this.SettingsTabCtrl);
            this.KeyPreview = true;
            this.Name = "SettingsForm";
            this.Text = "Settings";
            this.Deactivate += new System.EventHandler(this.SettingsForm_Deactivate);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Shown += new System.EventHandler(this.MainForm_Shown);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.SettingsForm_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.SettingsForm_KeyUp);
            this.SettingsTabCtrl.ResumeLayout(false);
            this.HotkeyTab.ResumeLayout(false);
            this.AutostartTab.ResumeLayout(false);
            this.AutostartTab.PerformLayout();
            this.accountTab.ResumeLayout(false);
            this.accountTab.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picture_qr)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl SettingsTabCtrl;
        private System.Windows.Forms.TabPage HotkeyTab;
        private System.Windows.Forms.TabPage OtherTab;
        private System.Windows.Forms.FlowLayoutPanel HotkeyLayout;
        private System.Windows.Forms.Button AddButton;
        private System.Windows.Forms.TabPage AutostartTab;
        private System.Windows.Forms.CheckBox autostartCheckbox;
        private System.Windows.Forms.TabPage accountTab;
        private System.Windows.Forms.CheckBox TwoFaCheckbox;
        private System.Windows.Forms.PictureBox picture_qr;
        private System.Windows.Forms.Label scanLable;
        private System.Windows.Forms.CheckBox ExplorerIntegrationBox;
    }
}