using System;
using System.Text;
using System.Windows.Forms;
using Go.Tools;

namespace Go
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class GoForm : Form
	{
		private TextBox _goCommandTxt;
		private PictureBox _goBtn;
		private System.Windows.Forms.ProgressBar _progressBar;
		private Label _label1;
		private Label _label2;
		private Label _description1;
		private Label _description2;
		private Label _count;

		private bool _commandLineMode;
        private Button _openConfigIndividualBtn;
        private Button _openConfigGroupBtn;
        private Button _openConfigOrganizationBtn;
        private Label _label3;

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components	= null;

		#region Form
		public GoForm()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}
		#endregion

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GoForm));
            this._goCommandTxt = new System.Windows.Forms.TextBox();
            this._goBtn = new System.Windows.Forms.PictureBox();
            this._progressBar = new System.Windows.Forms.ProgressBar();
            this._label1 = new System.Windows.Forms.Label();
            this._label2 = new System.Windows.Forms.Label();
            this._description1 = new System.Windows.Forms.Label();
            this._description2 = new System.Windows.Forms.Label();
            this._count = new System.Windows.Forms.Label();
            this._openConfigIndividualBtn = new System.Windows.Forms.Button();
            this._openConfigGroupBtn = new System.Windows.Forms.Button();
            this._openConfigOrganizationBtn = new System.Windows.Forms.Button();
            this._label3 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this._goBtn)).BeginInit();
            this.SuspendLayout();
            // 
            // _goCommandTxt
            // 
            this._goCommandTxt.Location = new System.Drawing.Point(12, 13);
            this._goCommandTxt.Name = "_goCommandTxt";
            this._goCommandTxt.Size = new System.Drawing.Size(696, 20);
            this._goCommandTxt.TabIndex = 3;
            this._goCommandTxt.KeyUp += new System.Windows.Forms.KeyEventHandler(this.GoCommandTxt_KeyUp);
            // 
            // _goBtn
            // 
            this._goBtn.BackColor = System.Drawing.Color.Transparent;
            this._goBtn.Image = ((System.Drawing.Image)(resources.GetObject("_goBtn.Image")));
            this._goBtn.Location = new System.Drawing.Point(716, 9);
            this._goBtn.Name = "_goBtn";
            this._goBtn.Size = new System.Drawing.Size(24, 24);
            this._goBtn.TabIndex = 4;
            this._goBtn.TabStop = false;
            this._goBtn.Click += new System.EventHandler(this.GoBtn_Click);
            // 
            // _progressBar
            // 
            this._progressBar.AccessibleDescription = "";
            this._progressBar.Location = new System.Drawing.Point(12, 100);
            this._progressBar.Name = "_progressBar";
            this._progressBar.Size = new System.Drawing.Size(728, 23);
            this._progressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this._progressBar.TabIndex = 5;
            this._progressBar.Tag = "";
            this._progressBar.Visible = false;
            // 
            // _label1
            // 
            this._label1.AutoEllipsis = true;
            this._label1.AutoSize = true;
            this._label1.Location = new System.Drawing.Point(12, 45);
            this._label1.MaximumSize = new System.Drawing.Size(200, 13);
            this._label1.Name = "_label1";
            this._label1.Size = new System.Drawing.Size(33, 13);
            this._label1.TabIndex = 6;
            this._label1.Text = "From:";
            this._label1.Visible = false;
            // 
            // _label2
            // 
            this._label2.AutoEllipsis = true;
            this._label2.AutoSize = true;
            this._label2.Location = new System.Drawing.Point(12, 62);
            this._label2.MaximumSize = new System.Drawing.Size(200, 13);
            this._label2.Name = "_label2";
            this._label2.Size = new System.Drawing.Size(23, 13);
            this._label2.TabIndex = 7;
            this._label2.Text = "To:";
            this._label2.Visible = false;
            // 
            // _description1
            // 
            this._description1.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this._description1.AutoEllipsis = true;
            this._description1.AutoSize = true;
            this._description1.Location = new System.Drawing.Point(51, 45);
            this._description1.MaximumSize = new System.Drawing.Size(500, 13);
            this._description1.Name = "_description1";
            this._description1.Size = new System.Drawing.Size(28, 13);
            this._description1.TabIndex = 8;
            this._description1.Text = "Text";
            this._description1.Visible = false;
            // 
            // _description2
            // 
            this._description2.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this._description2.AutoEllipsis = true;
            this._description2.AutoSize = true;
            this._description2.Location = new System.Drawing.Point(51, 62);
            this._description2.MaximumSize = new System.Drawing.Size(500, 13);
            this._description2.Name = "_description2";
            this._description2.Size = new System.Drawing.Size(28, 13);
            this._description2.TabIndex = 9;
            this._description2.Text = "Text";
            this._description2.Visible = false;
            // 
            // _count
            // 
            this._count.AutoSize = true;
            this._count.Location = new System.Drawing.Point(12, 84);
            this._count.Name = "_count";
            this._count.Size = new System.Drawing.Size(46, 13);
            this._count.TabIndex = 10;
            this._count.Text = "0 of 100";
            this._count.Visible = false;
            // 
            // _openConfigIndividualBtn
            // 
            this._openConfigIndividualBtn.Location = new System.Drawing.Point(660, 129);
            this._openConfigIndividualBtn.Name = "_openConfigIndividualBtn";
            this._openConfigIndividualBtn.Size = new System.Drawing.Size(22, 23);
            this._openConfigIndividualBtn.TabIndex = 11;
            this._openConfigIndividualBtn.Text = "I";
            this._openConfigIndividualBtn.UseVisualStyleBackColor = true;
            this._openConfigIndividualBtn.Click += new System.EventHandler(this.OpenConfigIndividualBTN_Click);
            // 
            // _openConfigGroupBtn
            // 
            this._openConfigGroupBtn.Location = new System.Drawing.Point(688, 129);
            this._openConfigGroupBtn.Name = "_openConfigGroupBtn";
            this._openConfigGroupBtn.Size = new System.Drawing.Size(22, 23);
            this._openConfigGroupBtn.TabIndex = 12;
            this._openConfigGroupBtn.Text = "G";
            this._openConfigGroupBtn.UseVisualStyleBackColor = true;
            this._openConfigGroupBtn.Click += new System.EventHandler(this.OpenConfigGroupBTN_Click);
            // 
            // _openConfigOrganizationBtn
            // 
            this._openConfigOrganizationBtn.Location = new System.Drawing.Point(716, 129);
            this._openConfigOrganizationBtn.Name = "_openConfigOrganizationBtn";
            this._openConfigOrganizationBtn.Size = new System.Drawing.Size(22, 23);
            this._openConfigOrganizationBtn.TabIndex = 13;
            this._openConfigOrganizationBtn.Text = "O";
            this._openConfigOrganizationBtn.UseVisualStyleBackColor = true;
            this._openConfigOrganizationBtn.Click += new System.EventHandler(this.OpenConfigOrganizationBTN_Click);
            // 
            // _label3
            // 
            this._label3.AutoSize = true;
            this._label3.Location = new System.Drawing.Point(537, 134);
            this._label3.Name = "_label3";
            this._label3.Size = new System.Drawing.Size(117, 13);
            this._label3.TabIndex = 14;
            this._label3.Text = "Open Configuration File";
            // 
            // GoForm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(752, 160);
            this.Controls.Add(this._description2);
            this.Controls.Add(this._description1);
            this.Controls.Add(this._label3);
            this.Controls.Add(this._openConfigOrganizationBtn);
            this.Controls.Add(this._openConfigGroupBtn);
            this.Controls.Add(this._openConfigIndividualBtn);
            this.Controls.Add(this._count);
            this.Controls.Add(this._label2);
            this.Controls.Add(this._label1);
            this.Controls.Add(this._progressBar);
            this.Controls.Add(this._goBtn);
            this.Controls.Add(this._goCommandTxt);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "GoForm";
            this.Text = "Go";
            this.Load += new System.EventHandler(this.GoForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this._goBtn)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		static Framework _framework;

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			try
			{
				GoForm goForm = new GoForm();
				_framework = new Framework(goForm.UpdateProgress);
				string[] arguments = Environment.GetCommandLineArgs();
				if (arguments.Length >= 2)
				{
					StringBuilder args = new StringBuilder();
					for (int i = 1; i < arguments.Length; i++)
					{
						if (i > 1)
							args.Append(" ");
						args.Append(arguments[i]); 
					}
					goForm._goCommandTxt.Text = args.ToString();
				}

				Application.Run(goForm);
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Go");
			}

			Environment.Exit(0);
		}

		private void GoBtn_Click(object sender, EventArgs e) {
			Cursor = Cursors.WaitCursor;
			try 
			{
				CommandLine cl = new CommandLine();
				cl.ParseAndExecute(_framework.Options, _framework.Directories, StringTools.CompactStringArray(_goCommandTxt.Text.Split(' ')));
			}
			catch (Exception ex)
			{
				// TODO: Does output need to be HTML'ized?
				MessageBox.Show(ex.Message, "Go");
			}
			UpdateProgress(this, new ProgressEventArgs(100, false).Reset());
			Cursor = Cursors.Default;
			if (_commandLineMode)
				Application.Exit();
		}

	    private void UpdateProgress(ProgressEventArgs pea)
	    {
            const int padWidth = 10;
	
            if (pea.Current == 0)
	        {
	            _progressBar.Visible = false;
	            _count.Visible = false;
	            _label1.Visible = false;
	            _label2.Visible = false;
	            _description1.Visible = false;
	            _description2.Visible = false;
	        }
	        else
	        {
	            pea.Maximum = Math.Max(pea.Current, pea.Maximum);
	            _progressBar.Maximum = pea.Maximum;
	            _progressBar.Value = pea.Current;
	            _progressBar.Visible = true;

                if (pea.ShowSuccess)
	                _count.Text = $"{pea.Current} of {pea.Maximum} (Success: {pea.SuccessCount})";
                else
                    _count.Text = $"{pea.Current} of {pea.Maximum}";
                _count.Visible = true;

	            if (!string.IsNullOrEmpty(pea.Label1))
	            {
	                _label1.Text = pea.Label1;
	                _label1.Visible = true;
                    _description1.Left = _label1.Left + _label1.Width + padWidth;

	                if (string.IsNullOrEmpty(pea.Description1))
	                    _description1.Text = string.Empty;
                    else
                    {
                        _description1.Text = pea.Description1;
                        _description1.Visible = true;
                    }
                }

	            if (!string.IsNullOrEmpty(pea.Label2))
	            {
	                _label2.Text = pea.Label2;
	                _label2.Visible = true;
                    _description2.Left = _label2.Left + _label2.Width + padWidth;

	                if (string.IsNullOrEmpty(pea.Description2))
	                    _description2.Text = string.Empty;
                    else
                    {
                        _description2.Text = pea.Description2;
                        _description2.Visible = true;
                    }
                }
	        }

	        Refresh();
	    }

        private void UpdateProgress(object sender, ProgressEventArgs pea)
		{
		    if (InvokeRequired)
		    {
		        BeginInvoke(new MethodInvoker(() => UpdateProgress(pea)));
		    }
		    else
		    {
		        UpdateProgress(pea);
		    }
        }

		private void GoCommandTxt_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
				GoBtn_Click(sender, e);
		}

		private void GoForm_Load(object sender, EventArgs e)
		{
			if (_goCommandTxt.Text.Length > 0)
			{
				_commandLineMode = true;
				GoBtn_Click(sender, e);
			}
		}

        private void OpenConfigIndividualBTN_Click(object sender, EventArgs e)
        {
            Shell.OpenAndBackupConfigurationFile("Go.Individual.xml");
        }

        private void OpenConfigGroupBTN_Click(object sender, EventArgs e)
        {
            Shell.OpenAndBackupConfigurationFile("Go.Group.xml");
        }

        private void OpenConfigOrganizationBTN_Click(object sender, EventArgs e)
        {
            Shell.OpenAndBackupConfigurationFile("Go.Organization.xml");
        }
	}
}
