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
            this.SettingsTabCtrl.SuspendLayout();
            this.HotkeyTab.SuspendLayout();
            this.AutostartTab.SuspendLayout();
            this.SuspendLayout();
            // 
            // SettingsTabCtrl
            // 
            this.SettingsTabCtrl.Controls.Add(this.HotkeyTab);
            this.SettingsTabCtrl.Controls.Add(this.OtherTab);
            this.SettingsTabCtrl.Controls.Add(this.AutostartTab);
            this.SettingsTabCtrl.Location = new System.Drawing.Point(12, 12);
            this.SettingsTabCtrl.Name = "SettingsTabCtrl";
            this.SettingsTabCtrl.SelectedIndex = 0;
            this.SettingsTabCtrl.Size = new System.Drawing.Size(776, 426);
            this.SettingsTabCtrl.TabIndex = 0;
            // 
            // HotkeyTab
            // 
            this.HotkeyTab.BackColor = System.Drawing.SystemColors.Control;
            this.HotkeyTab.Controls.Add(this.HotkeyLayout);
            this.HotkeyTab.Location = new System.Drawing.Point(4, 22);
            this.HotkeyTab.Name = "HotkeyTab";
            this.HotkeyTab.Padding = new System.Windows.Forms.Padding(3);
            this.HotkeyTab.Size = new System.Drawing.Size(768, 400);
            this.HotkeyTab.TabIndex = 0;
            this.HotkeyTab.Text = "Hotkeys";
            // 
            // HotkeyLayout
            // 
            this.HotkeyLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.HotkeyLayout.Location = new System.Drawing.Point(3, 3);
            this.HotkeyLayout.Name = "HotkeyLayout";
            this.HotkeyLayout.Size = new System.Drawing.Size(762, 394);
            this.HotkeyLayout.TabIndex = 0;
            // 
            // OtherTab
            // 
            this.OtherTab.BackColor = System.Drawing.SystemColors.Control;
            this.OtherTab.Location = new System.Drawing.Point(4, 22);
            this.OtherTab.Name = "OtherTab";
            this.OtherTab.Padding = new System.Windows.Forms.Padding(3);
            this.OtherTab.Size = new System.Drawing.Size(768, 400);
            this.OtherTab.TabIndex = 1;
            this.OtherTab.Text = "Other";
            // 
            // AutostartTab
            // 
            this.AutostartTab.Controls.Add(this.autostartCheckbox);
            this.AutostartTab.Location = new System.Drawing.Point(4, 22);
            this.AutostartTab.Name = "AutostartTab";
            this.AutostartTab.Padding = new System.Windows.Forms.Padding(3);
            this.AutostartTab.Size = new System.Drawing.Size(768, 400);
            this.AutostartTab.TabIndex = 2;
            this.AutostartTab.Text = "Autostart";
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
            this.autostartCheckbox.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.SettingsTabCtrl);
            this.Name = "SettingsForm";
            this.Text = "Settings";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Shown += new System.EventHandler(this.MainForm_Shown);
            this.SettingsTabCtrl.ResumeLayout(false);
            this.HotkeyTab.ResumeLayout(false);
            this.AutostartTab.ResumeLayout(false);
            this.AutostartTab.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl SettingsTabCtrl;
        private System.Windows.Forms.TabPage HotkeyTab;
        private System.Windows.Forms.TabPage OtherTab;
        private System.Windows.Forms.FlowLayoutPanel HotkeyLayout;
        private System.Windows.Forms.TabPage AutostartTab;
        private System.Windows.Forms.CheckBox autostartCheckbox;
    }
}