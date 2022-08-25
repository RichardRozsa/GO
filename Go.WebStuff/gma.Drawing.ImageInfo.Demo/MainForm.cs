// project created on 30.01.2004 at 19:22
using System;
using System.Windows.Forms;
using System.Drawing; 
using gma.Drawing.ImageInfo; 

namespace gma.Drawing.ImageInfo.Demo
{
	class MainForm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Splitter splitter2;
		private System.Windows.Forms.Button button;
		private System.Windows.Forms.Panel panelImg;
		private System.Windows.Forms.ColumnHeader propertyName;
		private System.Windows.Forms.Panel panel;
		private System.Windows.Forms.Splitter splitter;
		private System.Windows.Forms.PropertyGrid propertyGrid;
		private System.Windows.Forms.Panel panelBottom;
		private System.Windows.Forms.ListView listView;
		private System.Windows.Forms.Button buttonLoad;
		private System.Windows.Forms.Panel panelTop;
		private System.Windows.Forms.ColumnHeader propertyValue;
		private System.Windows.Forms.OpenFileDialog openFileDialog;
		private System.Windows.Forms.PictureBox pictureBox;
		public MainForm()
		{
			InitializeComponent();
		}
	
		Info inf;
		// THIS METHOD IS MAINTAINED BY THE FORM DESIGNER
		// DO NOT EDIT IT MANUALLY! YOUR CHANGES ARE LIKELY TO BE LOST
		void ButtonLoadClick(object sender, System.EventArgs e)
		{
			if (openFileDialog.ShowDialog()==DialogResult.OK)
			{
				pictureBox.Image=Image.FromFile(openFileDialog.FileName); 
				adjustPictureBoxSize();
				
				inf=new Info(pictureBox.Image);
				propertyGrid.SelectedObject = inf;
				listView.Items.Clear(); 
				foreach (string propertyname in inf.PropertyItems.Keys)
				{
					ListViewItem item1 = new ListViewItem(propertyname,0);
				    item1.SubItems.Add((inf.PropertyItems[propertyname]).ToString());
					listView.Items.Add(item1);
				}
			}
		}
		
		void ButtonClick(object sender, System.EventArgs e)
		{
			if (inf!=null) 
				MessageBox.Show(inf.EquipModel);
		}


		void PanelImgSizeChanged(object sender, System.EventArgs e)
		{
			if (panelImg.Width<=10) panelImg.Width=11;
			if (panelImg.Height<=10) panelImg.Height=11;
			adjustPictureBoxSize();
		}
		
		void adjustPictureBoxSize()
		{
			if (pictureBox.Image==null) return;
			if( (pictureBox.Image.Width / panelImg.Width) > (pictureBox.Image.Height / panelImg.Height))
				{
					//Adjust according width
					pictureBox.Width = panelImg.Width-10;
					pictureBox.Height = pictureBox.Width * pictureBox.Image.Height / pictureBox.Image.Width;
				}
			else
				{
					//Adjust according height 
					pictureBox.Height = panelImg.Height-10;
					pictureBox.Width = pictureBox.Height * pictureBox.Image.Width / pictureBox.Image.Height;
				}
				
			pictureBox.Top = (panelImg.Height - pictureBox.Height) / 2;
			pictureBox.Left =(panelImg.Width - pictureBox.Width) / 2;
			

		}
		
		void InitializeComponent() {
			this.pictureBox = new System.Windows.Forms.PictureBox();
			this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
			this.propertyValue = new System.Windows.Forms.ColumnHeader();
			this.panelTop = new System.Windows.Forms.Panel();
			this.buttonLoad = new System.Windows.Forms.Button();
			this.listView = new System.Windows.Forms.ListView();
			this.panelBottom = new System.Windows.Forms.Panel();
			this.propertyGrid = new System.Windows.Forms.PropertyGrid();
			this.splitter = new System.Windows.Forms.Splitter();
			this.panel = new System.Windows.Forms.Panel();
			this.propertyName = new System.Windows.Forms.ColumnHeader();
			this.panelImg = new System.Windows.Forms.Panel();
			this.button = new System.Windows.Forms.Button();
			this.splitter2 = new System.Windows.Forms.Splitter();
			this.panelTop.SuspendLayout();
			this.panelBottom.SuspendLayout();
			this.panel.SuspendLayout();
			this.panelImg.SuspendLayout();
			this.SuspendLayout();
			// 
			// pictureBox
			// 
			this.pictureBox.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
			this.pictureBox.Location = new System.Drawing.Point(16, 16);
			this.pictureBox.Name = "pictureBox";
			this.pictureBox.Size = new System.Drawing.Size(144, 144);
			this.pictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.pictureBox.TabIndex = 2;
			this.pictureBox.TabStop = false;
			// 
			// propertyValue
			// 
			this.propertyValue.Text = "Value";
			this.propertyValue.Width = 175;
			// 
			// panelTop
			// 
			this.panelTop.Controls.Add(this.button);
			this.panelTop.Controls.Add(this.buttonLoad);
			this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
			this.panelTop.Location = new System.Drawing.Point(0, 0);
			this.panelTop.Name = "panelTop";
			this.panelTop.Size = new System.Drawing.Size(632, 32);
			this.panelTop.TabIndex = 0;
			// 
			// buttonLoad
			// 
			this.buttonLoad.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.buttonLoad.Location = new System.Drawing.Point(5, 5);
			this.buttonLoad.Name = "buttonLoad";
			this.buttonLoad.TabIndex = 0;
			this.buttonLoad.Text = "Load";
			this.buttonLoad.Click += new System.EventHandler(this.ButtonLoadClick);
			// 
			// listView
			// 
			this.listView.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.listView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
						this.propertyName,
						this.propertyValue});
			this.listView.Dock = System.Windows.Forms.DockStyle.Fill;
			this.listView.Location = new System.Drawing.Point(0, 187);
			this.listView.MultiSelect = false;
			this.listView.Name = "listView";
			this.listView.Size = new System.Drawing.Size(300, 186);
			this.listView.TabIndex = 3;
			this.listView.View = System.Windows.Forms.View.Details;
			// 
			// panelBottom
			// 
			this.panelBottom.BackColor = System.Drawing.SystemColors.Control;
			this.panelBottom.Controls.Add(this.panelImg);
			this.panelBottom.Controls.Add(this.splitter2);
			this.panelBottom.Controls.Add(this.panel);
			this.panelBottom.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panelBottom.Location = new System.Drawing.Point(0, 32);
			this.panelBottom.Name = "panelBottom";
			this.panelBottom.Size = new System.Drawing.Size(632, 373);
			this.panelBottom.TabIndex = 1;
			// 
			// propertyGrid
			// 
			this.propertyGrid.CommandsBackColor = System.Drawing.SystemColors.ActiveCaptionText;
			this.propertyGrid.CommandsVisibleIfAvailable = true;
			this.propertyGrid.Dock = System.Windows.Forms.DockStyle.Top;
			this.propertyGrid.HelpVisible = false;
			this.propertyGrid.LargeButtons = false;
			this.propertyGrid.LineColor = System.Drawing.SystemColors.ScrollBar;
			this.propertyGrid.Location = new System.Drawing.Point(0, 0);
			this.propertyGrid.Name = "propertyGrid";
			this.propertyGrid.PropertySort = System.Windows.Forms.PropertySort.Alphabetical;
			this.propertyGrid.Size = new System.Drawing.Size(300, 184);
			this.propertyGrid.TabIndex = 1;
			this.propertyGrid.Text = "propertyGrid";
			this.propertyGrid.ToolbarVisible = false;
			this.propertyGrid.ViewBackColor = System.Drawing.SystemColors.Window;
			this.propertyGrid.ViewForeColor = System.Drawing.SystemColors.WindowText;
			// 
			// splitter
			// 
			this.splitter.Cursor = System.Windows.Forms.Cursors.HSplit;
			this.splitter.Dock = System.Windows.Forms.DockStyle.Top;
			this.splitter.Location = new System.Drawing.Point(0, 184);
			this.splitter.Name = "splitter";
			this.splitter.Size = new System.Drawing.Size(300, 3);
			this.splitter.TabIndex = 2;
			this.splitter.TabStop = false;
			// 
			// panel
			// 
			this.panel.Controls.Add(this.listView);
			this.panel.Controls.Add(this.splitter);
			this.panel.Controls.Add(this.propertyGrid);
			this.panel.Dock = System.Windows.Forms.DockStyle.Right;
			this.panel.Location = new System.Drawing.Point(332, 0);
			this.panel.Name = "panel";
			this.panel.Size = new System.Drawing.Size(300, 373);
			this.panel.TabIndex = 3;
			// 
			// propertyName
			// 
			this.propertyName.Text = "Property";
			this.propertyName.Width = 103;
			// 
			// panelImg
			// 
			this.panelImg.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
			this.panelImg.Controls.Add(this.pictureBox);
			this.panelImg.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panelImg.Location = new System.Drawing.Point(0, 0);
			this.panelImg.Name = "panelImg";
			this.panelImg.Size = new System.Drawing.Size(329, 373);
			this.panelImg.TabIndex = 5;
			this.panelImg.SizeChanged += new System.EventHandler(this.PanelImgSizeChanged);
			// 
			// button
			// 
			this.button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.button.Location = new System.Drawing.Point(88, 5);
			this.button.Name = "button";
			this.button.TabIndex = 1;
			this.button.Text = "Show";
			this.button.Click += new System.EventHandler(this.ButtonClick);
			// 
			// splitter2
			// 
			this.splitter2.Dock = System.Windows.Forms.DockStyle.Right;
			this.splitter2.Location = new System.Drawing.Point(329, 0);
			this.splitter2.Name = "splitter2";
			this.splitter2.Size = new System.Drawing.Size(3, 373);
			this.splitter2.TabIndex = 4;
			this.splitter2.TabStop = false;
			// 
			// MainForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(632, 405);
			this.Controls.Add(this.panelBottom);
			this.Controls.Add(this.panelTop);
			this.Name = "MainForm";
			this.Text = "This is my form";
			this.panelTop.ResumeLayout(false);
			this.panelBottom.ResumeLayout(false);
			this.panel.ResumeLayout(false);
			this.panelImg.ResumeLayout(false);
			this.ResumeLayout(false);
		}
			
		[STAThread]
		public static void Main(string[] args)
		{
			Application.Run(new MainForm());
		}
	}			
}
