namespace frznUpload.Client
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
            this.StatusLabel = new System.Windows.Forms.Label();
            this.ProgressBar = new System.Windows.Forms.ProgressBar();
            this.AccountButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // StatusLabel
            // 
            this.StatusLabel.Location = new System.Drawing.Point(13, 425);
            this.StatusLabel.Name = "StatusLabel";
            this.StatusLabel.Size = new System.Drawing.Size(132, 16);
            this.StatusLabel.TabIndex = 0;
            this.StatusLabel.Text = "label1";
            // 
            // ProgressBar
            // 
            this.ProgressBar.Location = new System.Drawing.Point(152, 425);
            this.ProgressBar.Name = "ProgressBar";
            this.ProgressBar.Size = new System.Drawing.Size(498, 16);
            this.ProgressBar.TabIndex = 1;
            // 
            // AccountButton
            // 
            this.AccountButton.Location = new System.Drawing.Point(679, 420);
            this.AccountButton.Name = "AccountButton";
            this.AccountButton.Size = new System.Drawing.Size(33, 23);
            this.AccountButton.TabIndex = 2;
            this.AccountButton.Text = "Ac";
            this.AccountButton.UseVisualStyleBackColor = true;
            this.AccountButton.Click += new System.EventHandler(this.AccountButton_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.AccountButton);
            this.Controls.Add(this.ProgressBar);
            this.Controls.Add(this.StatusLabel);
            this.Name = "MainForm";
            this.Text = "frznUpload";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label StatusLabel;
        private System.Windows.Forms.ProgressBar ProgressBar;
        private System.Windows.Forms.Button AccountButton;
    }
}

