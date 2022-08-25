// ProgressEvent.cs

using System;
using System.Windows.Forms;

namespace Go.Tools
{
	// Class that contains the data for the progress event.
    // Derives from System.EventArgs.
    public class ProgressEventArgs : EventArgs
    {
        private string _description1;
        private string _description2;

        public int Maximum { get; set; }

        public int Current { get; set; }

        public int SuccessCount { get; set; }

        public bool ShowSuccess { get; set; }

        public string Label1 { get; set; }

        public string Label2 { get; set; }

        public string Description1
        {
            get
            {
                if (Label1 == null)
                {
                    _description1 = null;
                }
                return _description1;
            }
            set { _description1 = value; }
        }

        public string Description2
        {
            get
            {
                if (Label2 == null)
                {
                    _description2 = null;
                }
                return _description2;
            }
            set { _description2 = value; }
        }

        // Constructors
        public ProgressEventArgs(int maximum, bool showSuccess, string label1 = null, string label2 = null)
        {
            Reset();
            Maximum = maximum;
            SuccessCount = 0;
            ShowSuccess = showSuccess;
            Label1 = string.Empty;
            Label2 = string.Empty;
            if (!string.IsNullOrEmpty(label1))
                Label1 = label1 + " ";
            if (!string.IsNullOrEmpty(label2))
                Label2 = label2 + " ";
        }

        public ProgressEventArgs UpdateProgress(int current, string description1, string description2)
        {
            Current = current;
            Description1 = description1;
            Description2 = description2;
            return this;
        }

        public ProgressEventArgs UpdateProgress(int current, string description1)
        {
            return UpdateProgress(current, description1, null);
        }

        public ProgressEventArgs UpdateProgress(int current)
        {
            return UpdateProgress(current, null, null);
        }

        public ProgressEventArgs Reset()
        {
            Maximum = 100;
            Current = 0;
            Label1 = null;
            Label2 = null;
            Description1 = null;
            Description2 = null;
            return this;
        }
    };

    // Delegate declaration.
    public delegate void ProgressEventHandler(object sender, ProgressEventArgs e);

    public class ProgressEvent
    {
        // The event member that is of type ProgressEventHandler.
        public event ProgressEventHandler Progress;

        // The protected OnProgress method raises the event by invoking 
        // the delegates. The sender is always this, the current instance 
        // of the class.
        public virtual void OnProgress(ProgressEventArgs e)
        {
            if (Progress != null)
            {
                // Invokes the delegates. 
                Progress(this, e);
            }
        }
    };
}