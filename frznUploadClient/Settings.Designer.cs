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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.HotkeyTab = new System.Windows.Forms.TabPage();
            this.OtherTab = new System.Windows.Forms.TabPage();
            this.HotkeyLayout = new System.Windows.Forms.FlowLayoutPanel();
            this.tabControl1.SuspendLayout();
            this.HotkeyTab.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.HotkeyTab);
            this.tabControl1.Controls.Add(this.OtherTab);
            this.tabControl1.Location = new System.Drawing.Point(12, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(776, 426);
            this.tabControl1.TabIndex = 0;
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
            // HotkeyLayout
            // 
            this.HotkeyLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.HotkeyLayout.Location = new System.Drawing.Point(3, 3);
            this.HotkeyLayout.Name = "HotkeyLayout";
            this.HotkeyLayout.Size = new System.Drawing.Size(762, 394);
            this.HotkeyLayout.TabIndex = 0;
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.tabControl1);
            this.Name = "SettingsForm";
            this.Text = "Settings";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Shown += new System.EventHandler(this.MainForm_Shown);
            this.tabControl1.ResumeLayout(false);
            this.HotkeyTab.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage HotkeyTab;
        private System.Windows.Forms.TabPage OtherTab;
        private System.Windows.Forms.FlowLayoutPanel HotkeyLayout;
    }
}