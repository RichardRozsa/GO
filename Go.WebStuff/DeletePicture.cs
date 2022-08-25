using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Go
{
    public partial class DeletePicture : Form
    {
        public DeletePicture(string pictureFileName)
        {
            InitializeComponent();

            FileName.Text = pictureFileName;
            PictureToDelete.Image = Image.FromFile(pictureFileName);
        }

        private void DeleteBTN_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Yes;
            PictureToDelete.Image.Dispose();
            Dispose();
        }

        private void LeaveBTN_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.No;
            PictureToDelete.Image.Dispose();
            Dispose();
        }

        private void CancelBTN_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            PictureToDelete.Image.Dispose();
            Dispose();
        }
    }
}