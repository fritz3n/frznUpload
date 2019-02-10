using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace frznUpload.Client
{
    partial class HotkeyConfigControl
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

        private void Initialize()
        {
            SplitContainer = new SplitContainer();
            FormatBox = new TextBox();
            HotkeyButton = new Button();
            label3 = new Label();
            label1 = new Label();
            FileProviderBox = new ComboBox();
            label2 = new Label();
            ShareBox = new CheckBox();
            label4 = new Label();
            WhitelistText = new TextBox();
            WhitelistedBox = new CheckBox();
            checkBox1 = new CheckBox();
            FirstViewBox = new CheckBox();
            PublicBox = new CheckBox();
            ((System.ComponentModel.ISupportInitialize)(SplitContainer)).BeginInit();
            SplitContainer.Panel1.SuspendLayout();
            SplitContainer.Panel2.SuspendLayout();
            SplitContainer.SuspendLayout();
            SuspendLayout();
            // 
            // SplitContainer
            // 
            SplitContainer.Dock = DockStyle.Fill;
            SplitContainer.IsSplitterFixed = true;
            SplitContainer.Location = new System.Drawing.Point(0, 0);
            SplitContainer.Name = "SplitContainer";
            // 
            // SplitContainer.Panel1
            // 
            SplitContainer.Panel1.Controls.Add(FormatBox);
            SplitContainer.Panel1.Controls.Add(HotkeyButton);
            SplitContainer.Panel1.Controls.Add(label3);
            SplitContainer.Panel1.Controls.Add(label1);
            SplitContainer.Panel1.Controls.Add(FileProviderBox);
            SplitContainer.Panel1.Controls.Add(label2);
            SplitContainer.Panel1.Controls.Add(ShareBox);
            // 
            // SplitContainer.Panel2
            // 
            SplitContainer.Panel2.Controls.Add(label4);
            SplitContainer.Panel2.Controls.Add(WhitelistText);
            SplitContainer.Panel2.Controls.Add(WhitelistedBox);
            SplitContainer.Panel2.Controls.Add(checkBox1);
            SplitContainer.Panel2.Controls.Add(FirstViewBox);
            SplitContainer.Panel2.Controls.Add(PublicBox);
            SplitContainer.Size = new System.Drawing.Size(245, 167);
            SplitContainer.SplitterDistance = 120;
            SplitContainer.TabIndex = 8;
            // 
            // FormatBox
            // 
            FormatBox.Anchor = ((AnchorStyles)(((AnchorStyles.Top | AnchorStyles.Left)
            | AnchorStyles.Right)));
            FormatBox.Location = new System.Drawing.Point(9, 114);
            FormatBox.Name = "FormatBox";
            FormatBox.Size = new System.Drawing.Size(108, 20);
            FormatBox.TabIndex = 5;
            // 
            // HotkeyButton
            // 
            HotkeyButton.Anchor = ((AnchorStyles)(((AnchorStyles.Top | AnchorStyles.Left)
            | AnchorStyles.Right)));
            HotkeyButton.Location = new System.Drawing.Point(9, 32);
            HotkeyButton.Name = "HotkeyButton";
            HotkeyButton.Size = new System.Drawing.Size(108, 23);
            HotkeyButton.TabIndex = 0;
            HotkeyButton.Text = "[none]";
            HotkeyButton.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(3, 98);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(84, 13);
            label3.TabIndex = 4;
            label3.Text = "Filename Format";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(3, 16);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(41, 13);
            label1.TabIndex = 1;
            label1.Text = "Hotkey";
            // 
            // FileProviderBox
            // 
            FileProviderBox.Anchor = ((AnchorStyles)(((AnchorStyles.Top | AnchorStyles.Left)
            | AnchorStyles.Right)));
            FileProviderBox.DropDownStyle = ComboBoxStyle.DropDownList;
            FileProviderBox.FormattingEnabled = true;
            FileProviderBox.Items.AddRange(new object[] {
            "Clipboard",
            "ScreenClip",
            "Screenshot"});
            FileProviderBox.Location = new System.Drawing.Point(9, 74);
            FileProviderBox.Name = "FileProviderBox";
            FileProviderBox.Size = new System.Drawing.Size(108, 21);
            FileProviderBox.TabIndex = 3;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(3, 58);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(61, 13);
            label2.TabIndex = 2;
            label2.Text = "Fileprovider";
            // 
            // ShareBox
            // 
            ShareBox.AutoSize = true;
            ShareBox.Checked = true;
            ShareBox.CheckState = CheckState.Checked;
            ShareBox.Location = new System.Drawing.Point(10, 140);
            ShareBox.Name = "ShareBox";
            ShareBox.Size = new System.Drawing.Size(54, 17);
            ShareBox.TabIndex = 0;
            ShareBox.Text = "Share";
            ShareBox.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new System.Drawing.Point(3, 96);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(47, 13);
            label4.TabIndex = 6;
            label4.Text = "Whitelist";
            // 
            // WhitelistText
            // 
            WhitelistText.Anchor = ((AnchorStyles)(((AnchorStyles.Top | AnchorStyles.Left)
            | AnchorStyles.Right)));
            WhitelistText.Location = new System.Drawing.Point(6, 114);
            WhitelistText.Name = "WhitelistText";
            WhitelistText.Size = new System.Drawing.Size(112, 20);
            WhitelistText.TabIndex = 5;
            // 
            // WhitelistedBox
            // 
            WhitelistedBox.AutoSize = true;
            WhitelistedBox.Location = new System.Drawing.Point(6, 76);
            WhitelistedBox.Name = "WhitelistedBox";
            WhitelistedBox.Size = new System.Drawing.Size(78, 17);
            WhitelistedBox.TabIndex = 4;
            WhitelistedBox.Text = "Whitelisted";
            WhitelistedBox.UseVisualStyleBackColor = true;
            // 
            // checkBox1
            // 
            checkBox1.AutoSize = true;
            checkBox1.Checked = true;
            checkBox1.CheckState = CheckState.Checked;
            checkBox1.Location = new System.Drawing.Point(6, 54);
            checkBox1.Name = "checkBox1";
            checkBox1.Size = new System.Drawing.Size(109, 17);
            checkBox1.TabIndex = 3;
            checkBox1.Text = "Public Registered";
            checkBox1.UseVisualStyleBackColor = true;
            // 
            // FirstViewBox
            // 
            FirstViewBox.AutoSize = true;
            FirstViewBox.Location = new System.Drawing.Point(6, 12);
            FirstViewBox.Name = "FirstViewBox";
            FirstViewBox.Size = new System.Drawing.Size(71, 17);
            FirstViewBox.TabIndex = 2;
            FirstViewBox.Text = "First View";
            FirstViewBox.UseVisualStyleBackColor = true;
            // 
            // PublicBox
            // 
            PublicBox.AutoSize = true;
            PublicBox.Checked = true;
            PublicBox.CheckState = CheckState.Checked;
            PublicBox.Location = new System.Drawing.Point(6, 32);
            PublicBox.Name = "PublicBox";
            PublicBox.Size = new System.Drawing.Size(55, 17);
            PublicBox.TabIndex = 1;
            PublicBox.Text = "Public";
            PublicBox.UseVisualStyleBackColor = true;
            // 
            // SettingsForm
            // 
            ClientSize = new System.Drawing.Size(245, 167);
            Controls.Add(SplitContainer);
            Name = "SettingsForm";
            SplitContainer.Panel1.ResumeLayout(false);
            SplitContainer.Panel1.PerformLayout();
            SplitContainer.Panel2.ResumeLayout(false);
            SplitContainer.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(SplitContainer)).EndInit();
            SplitContainer.ResumeLayout(false);
            ResumeLayout(false);

        }
    }
}
