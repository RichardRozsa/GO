using System.Windows.Forms;

namespace Go.Tools
{
    public delegate void GoProgressBarExecuteDelegate(ProgressBar progressBar, Options option);

    public partial class ProgressBar : Form
    {
//         ProgressBar(string title, int maximumCount)
//         {
//             ProgressBar(title, "", "", maximumCount);
//         }
// 
//         ProgressBar(string title, string label1, int maximumCount)
//         {
//             ProgressBar(title, label1, "", maximumCount);
//         }

        public ProgressBar(GoProgressBarExecuteDelegate goExecute, Options options)
        {
            InitializeComponent();

            if (goExecute != null)
                goExecute(this, options);
        }

        public void EnableProgressBar(string title, string label1, string label2, int maximumCount)
        {
            this.Text = title;

            if (label1 != null && label1.Length > 0)
            {
                Label1.Text = label1;
                Label1.Visible = true;
            }
            else
                Label1.Visible = false;

            if (label2 != null && label2.Length > 0)
            {
                Label2.Text = label2;
                Label2.Visible = true;
            }
            else
                Label2.Visible = false;

            ProgressBarCtrl.Value = 0;
            ProgressBarCtrl.Maximum = maximumCount;

            this.Show();
        }

        public void UpdateProgress(int currentCount)
        {
            UpdateProgress("", "", currentCount);
        }

        public void UpdateProgress(string description1, int currentCount)
        {
            UpdateProgress(description1, "", currentCount);
        }

        public void UpdateProgress(string description1, string description2, int currentCount)
        {
            Description1.Text = description1;
            Description2.Text = description2;
            ProgressBarCtrl.Value = currentCount;
        }
    }
}
