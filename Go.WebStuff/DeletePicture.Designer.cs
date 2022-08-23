namespace Go
{
    partial class DeletePicture
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
            this.PictureToDelete = new System.Windows.Forms.PictureBox();
            this.DeleteBTN = new System.Windows.Forms.Button();
            this.LeaveBTN = new System.Windows.Forms.Button();
            this.CancelBTN = new System.Windows.Forms.Button();
            this.FileName = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.PictureToDelete)).BeginInit();
            this.SuspendLayout();
            // 
            // PictureToDelete
            // 
            this.PictureToDelete.Location = new System.Drawing.Point(12, 12);
            this.PictureToDelete.Name = "PictureToDelete";
            this.PictureToDelete.Size = new System.Drawing.Size(520, 520);
            this.PictureToDelete.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.PictureToDelete.TabIndex = 0;
            this.PictureToDelete.TabStop = false;
            // 
            // DeleteBTN
            // 
            this.DeleteBTN.Location = new System.Drawing.Point(12, 566);
            this.DeleteBTN.Name = "DeleteBTN";
            this.DeleteBTN.Size = new System.Drawing.Size(115, 23);
            this.DeleteBTN.TabIndex = 1;
            this.DeleteBTN.Text = "&Delete Picture";
            this.DeleteBTN.UseVisualStyleBackColor = true;
            this.DeleteBTN.Click += new System.EventHandler(this.DeleteBTN_Click);
            // 
            // LeaveBTN
            // 
            this.LeaveBTN.Location = new System.Drawing.Point(211, 566);
            this.LeaveBTN.Name = "LeaveBTN";
            this.LeaveBTN.Size = new System.Drawing.Size(115, 23);
            this.LeaveBTN.TabIndex = 2;
            this.LeaveBTN.Text = "&Leave Picture";
            this.LeaveBTN.UseVisualStyleBackColor = true;
            this.LeaveBTN.Click += new System.EventHandler(this.LeaveBTN_Click);
            // 
            // CancelBTN
            // 
            this.CancelBTN.Location = new System.Drawing.Point(417, 566);
            this.CancelBTN.Name = "CancelBTN";
            this.CancelBTN.Size = new System.Drawing.Size(115, 23);
            this.CancelBTN.TabIndex = 3;
            this.CancelBTN.Text = "&Cancel";
            this.CancelBTN.UseVisualStyleBackColor = true;
            this.CancelBTN.Click += new System.EventHandler(this.CancelBTN_Click);
            // 
            // FileName
            // 
            this.FileName.AutoSize = true;
            this.FileName.Location = new System.Drawing.Point(9, 535);
            this.FileName.Name = "FileName";
            this.FileName.Size = new System.Drawing.Size(35, 13);
            this.FileName.TabIndex = 4;
            this.FileName.Text = "label1";
            // 
            // DeletePicture
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(546, 604);
            this.Controls.Add(this.FileName);
            this.Controls.Add(this.CancelBTN);
            this.Controls.Add(this.LeaveBTN);
            this.Controls.Add(this.DeleteBTN);
            this.Controls.Add(this.PictureToDelete);
            this.Name = "DeletePicture";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Delete Picture";
            ((System.ComponentModel.ISupportInitialize)(this.PictureToDelete)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox PictureToDelete;
        private System.Windows.Forms.Button DeleteBTN;
        private System.Windows.Forms.Button LeaveBTN;
        private System.Windows.Forms.Button CancelBTN;
        private System.Windows.Forms.Label FileName;
    }
}