using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace frznUpload.Client
{
    partial class SettingsForm
    {
        class HotkeyConfigControl : GroupBox
        {
            private SplitContainer SplitContainer;
            private TextBox FormatBox;
            private Button HotkeyButton;
            private Label label3;
            private Label label1;
            private ComboBox FileProviderBox;
            private Label label2;
            private CheckBox ShareBox;
            private Label label4;
            private TextBox WhitelistText;
            private CheckBox WhitelistedBox;
            private CheckBox checkBox1;
            private CheckBox FirstViewBox;
            private CheckBox PublicBox;
            
            public HotkeyConfigControl()
            {
                this.SplitContainer = new System.Windows.Forms.SplitContainer();
                this.FormatBox = new System.Windows.Forms.TextBox();
                this.HotkeyButton = new System.Windows.Forms.Button();
                this.label3 = new System.Windows.Forms.Label();
                this.label1 = new System.Windows.Forms.Label();
                this.FileProviderBox = new System.Windows.Forms.ComboBox();
                this.label2 = new System.Windows.Forms.Label();
                this.ShareBox = new System.Windows.Forms.CheckBox();
                this.label4 = new System.Windows.Forms.Label();
                this.WhitelistText = new System.Windows.Forms.TextBox();
                this.WhitelistedBox = new System.Windows.Forms.CheckBox();
                this.checkBox1 = new System.Windows.Forms.CheckBox();
                this.FirstViewBox = new System.Windows.Forms.CheckBox();
                this.PublicBox = new System.Windows.Forms.CheckBox();
                ((System.ComponentModel.ISupportInitialize)(this.SplitContainer)).BeginInit();
                this.SplitContainer.Panel1.SuspendLayout();
                this.SplitContainer.Panel2.SuspendLayout();
                this.SplitContainer.SuspendLayout();
                this.SuspendLayout();
                // 
                // SplitContainer
                // 
                this.SplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
                this.SplitContainer.IsSplitterFixed = true;
                this.SplitContainer.Location = new System.Drawing.Point(0, 0);
                this.SplitContainer.Name = "SplitContainer";
                // 
                // SplitContainer.Panel1
                // 
                this.SplitContainer.Panel1.Controls.Add(this.FormatBox);
                this.SplitContainer.Panel1.Controls.Add(this.HotkeyButton);
                this.SplitContainer.Panel1.Controls.Add(this.label3);
                this.SplitContainer.Panel1.Controls.Add(this.label1);
                this.SplitContainer.Panel1.Controls.Add(this.FileProviderBox);
                this.SplitContainer.Panel1.Controls.Add(this.label2);
                this.SplitContainer.Panel1.Controls.Add(this.ShareBox);
                // 
                // SplitContainer.Panel2
                // 
                this.SplitContainer.Panel2.Controls.Add(this.label4);
                this.SplitContainer.Panel2.Controls.Add(this.WhitelistText);
                this.SplitContainer.Panel2.Controls.Add(this.WhitelistedBox);
                this.SplitContainer.Panel2.Controls.Add(this.checkBox1);
                this.SplitContainer.Panel2.Controls.Add(this.FirstViewBox);
                this.SplitContainer.Panel2.Controls.Add(this.PublicBox);
                this.SplitContainer.Size = new System.Drawing.Size(245, 167);
                this.SplitContainer.SplitterDistance = 120;
                this.SplitContainer.TabIndex = 8;
                // 
                // FormatBox
                // 
                this.FormatBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                | System.Windows.Forms.AnchorStyles.Right)));
                this.FormatBox.Location = new System.Drawing.Point(9, 114);
                this.FormatBox.Name = "FormatBox";
                this.FormatBox.Size = new System.Drawing.Size(108, 20);
                this.FormatBox.TabIndex = 5;
                // 
                // HotkeyButton
                // 
                this.HotkeyButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                | System.Windows.Forms.AnchorStyles.Right)));
                this.HotkeyButton.Location = new System.Drawing.Point(9, 32);
                this.HotkeyButton.Name = "HotkeyButton";
                this.HotkeyButton.Size = new System.Drawing.Size(108, 23);
                this.HotkeyButton.TabIndex = 0;
                this.HotkeyButton.Text = "[none]";
                this.HotkeyButton.UseVisualStyleBackColor = true;
                // 
                // label3
                // 
                this.label3.AutoSize = true;
                this.label3.Location = new System.Drawing.Point(3, 98);
                this.label3.Name = "label3";
                this.label3.Size = new System.Drawing.Size(84, 13);
                this.label3.TabIndex = 4;
                this.label3.Text = "Filename Format";
                // 
                // label1
                // 
                this.label1.AutoSize = true;
                this.label1.Location = new System.Drawing.Point(3, 16);
                this.label1.Name = "label1";
                this.label1.Size = new System.Drawing.Size(41, 13);
                this.label1.TabIndex = 1;
                this.label1.Text = "Hotkey";
                // 
                // FileProviderBox
                // 
                this.FileProviderBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                | System.Windows.Forms.AnchorStyles.Right)));
                this.FileProviderBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
                this.FileProviderBox.FormattingEnabled = true;
                this.FileProviderBox.Items.AddRange(new object[] {
            "Clipboard",
            "ScreenClip",
            "Screenshot"});
                this.FileProviderBox.Location = new System.Drawing.Point(9, 74);
                this.FileProviderBox.Name = "FileProviderBox";
                this.FileProviderBox.Size = new System.Drawing.Size(108, 21);
                this.FileProviderBox.TabIndex = 3;
                // 
                // label2
                // 
                this.label2.AutoSize = true;
                this.label2.Location = new System.Drawing.Point(3, 58);
                this.label2.Name = "label2";
                this.label2.Size = new System.Drawing.Size(61, 13);
                this.label2.TabIndex = 2;
                this.label2.Text = "Fileprovider";
                // 
                // ShareBox
                // 
                this.ShareBox.AutoSize = true;
                this.ShareBox.Checked = true;
                this.ShareBox.CheckState = System.Windows.Forms.CheckState.Checked;
                this.ShareBox.Location = new System.Drawing.Point(10, 140);
                this.ShareBox.Name = "ShareBox";
                this.ShareBox.Size = new System.Drawing.Size(54, 17);
                this.ShareBox.TabIndex = 0;
                this.ShareBox.Text = "Share";
                this.ShareBox.UseVisualStyleBackColor = true;
                // 
                // label4
                // 
                this.label4.AutoSize = true;
                this.label4.Location = new System.Drawing.Point(3, 96);
                this.label4.Name = "label4";
                this.label4.Size = new System.Drawing.Size(47, 13);
                this.label4.TabIndex = 6;
                this.label4.Text = "Whitelist";
                // 
                // WhitelistText
                // 
                this.WhitelistText.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                | System.Windows.Forms.AnchorStyles.Right)));
                this.WhitelistText.Location = new System.Drawing.Point(6, 114);
                this.WhitelistText.Name = "WhitelistText";
                this.WhitelistText.Size = new System.Drawing.Size(112, 20);
                this.WhitelistText.TabIndex = 5;
                // 
                // WhitelistedBox
                // 
                this.WhitelistedBox.AutoSize = true;
                this.WhitelistedBox.Location = new System.Drawing.Point(6, 76);
                this.WhitelistedBox.Name = "WhitelistedBox";
                this.WhitelistedBox.Size = new System.Drawing.Size(78, 17);
                this.WhitelistedBox.TabIndex = 4;
                this.WhitelistedBox.Text = "Whitelisted";
                this.WhitelistedBox.UseVisualStyleBackColor = true;
                // 
                // checkBox1
                // 
                this.checkBox1.AutoSize = true;
                this.checkBox1.Checked = true;
                this.checkBox1.CheckState = System.Windows.Forms.CheckState.Checked;
                this.checkBox1.Location = new System.Drawing.Point(6, 54);
                this.checkBox1.Name = "checkBox1";
                this.checkBox1.Size = new System.Drawing.Size(109, 17);
                this.checkBox1.TabIndex = 3;
                this.checkBox1.Text = "Public Registered";
                this.checkBox1.UseVisualStyleBackColor = true;
                // 
                // FirstViewBox
                // 
                this.FirstViewBox.AutoSize = true;
                this.FirstViewBox.Location = new System.Drawing.Point(6, 12);
                this.FirstViewBox.Name = "FirstViewBox";
                this.FirstViewBox.Size = new System.Drawing.Size(71, 17);
                this.FirstViewBox.TabIndex = 2;
                this.FirstViewBox.Text = "First View";
                this.FirstViewBox.UseVisualStyleBackColor = true;
                // 
                // PublicBox
                // 
                this.PublicBox.AutoSize = true;
                this.PublicBox.Checked = true;
                this.PublicBox.CheckState = System.Windows.Forms.CheckState.Checked;
                this.PublicBox.Location = new System.Drawing.Point(6, 32);
                this.PublicBox.Name = "PublicBox";
                this.PublicBox.Size = new System.Drawing.Size(55, 17);
                this.PublicBox.TabIndex = 1;
                this.PublicBox.Text = "Public";
                this.PublicBox.UseVisualStyleBackColor = true;
                // 
                // SettingsForm
                // 
                this.ClientSize = new System.Drawing.Size(245, 167);
                this.Controls.Add(this.SplitContainer);
                this.Name = "SettingsForm";
                this.SplitContainer.Panel1.ResumeLayout(false);
                this.SplitContainer.Panel1.PerformLayout();
                this.SplitContainer.Panel2.ResumeLayout(false);
                this.SplitContainer.Panel2.PerformLayout();
                ((System.ComponentModel.ISupportInitialize)(this.SplitContainer)).EndInit();
                this.SplitContainer.ResumeLayout(false);
                this.ResumeLayout(false);

            }
        }
    }
    
    
}
