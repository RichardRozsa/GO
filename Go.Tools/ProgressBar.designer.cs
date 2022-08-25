namespace Go.Tools
{
    partial class ProgressBar
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProgressBar));
            this.ProgressBarCtrl = new System.Windows.Forms.ProgressBar();
            this.Label1 = new System.Windows.Forms.Label();
            this.Label2 = new System.Windows.Forms.Label();
            this.Description1 = new System.Windows.Forms.Label();
            this.Description2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // ProgressBarCtrl
            // 
            this.ProgressBarCtrl.AccessibleDescription = "";
            this.ProgressBarCtrl.Location = new System.Drawing.Point(12, 9);
            this.ProgressBarCtrl.Name = "ProgressBarCtrl";
            this.ProgressBarCtrl.Size = new System.Drawing.Size(656, 23);
            this.ProgressBarCtrl.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.ProgressBarCtrl.TabIndex = 0;
            // 
            // Label1
            // 
            this.Label1.AutoSize = true;
            this.Label1.Location = new System.Drawing.Point(12, 44);
            this.Label1.Name = "Label1";
            this.Label1.Size = new System.Drawing.Size(33, 13);
            this.Label1.TabIndex = 1;
            this.Label1.Text = "From:";
            this.Label1.Visible = false;
            // 
            // Label2
            // 
            this.Label2.AutoSize = true;
            this.Label2.Location = new System.Drawing.Point(12, 67);
            this.Label2.Name = "Label2";
            this.Label2.Size = new System.Drawing.Size(23, 13);
            this.Label2.TabIndex = 2;
            this.Label2.Text = "To:";
            this.Label2.Visible = false;
            // 
            // Description1
            // 
            this.Description1.AutoSize = true;
            this.Description1.Location = new System.Drawing.Point(51, 44);
            this.Description1.Name = "Description1";
            this.Description1.Size = new System.Drawing.Size(0, 13);
            this.Description1.TabIndex = 3;
            this.Description1.Visible = false;
            // 
            // Description2
            // 
            this.Description2.AutoSize = true;
            this.Description2.Location = new System.Drawing.Point(51, 67);
            this.Description2.Name = "Description2";
            this.Description2.Size = new System.Drawing.Size(0, 13);
            this.Description2.TabIndex = 4;
            this.Description2.Visible = false;
            // 
            // ProgressBar
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(680, 101);
            this.Controls.Add(this.Description2);
            this.Controls.Add(this.Description1);
            this.Controls.Add(this.Label2);
            this.Controls.Add(this.Label1);
            this.Controls.Add(this.ProgressBarCtrl);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ProgressBar";
            this.Text = "GO Processing...";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ProgressBar ProgressBarCtrl;
        private System.Windows.Forms.Label Label1;
        private System.Windows.Forms.Label Label2;
        private System.Windows.Forms.Label Description1;
        private System.Windows.Forms.Label Description2;
    }
}