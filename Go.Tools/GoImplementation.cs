using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Collections;
using System.Windows.Forms;
using Go.Configuration;

namespace Go.Tools
{
	public class GoImplementation
	{
		private SortedList options		= null;
		private SortedList directories	= null;

		public GoImplementation(ref SortedList options, ref SortedList directories)
		{
			this.options = options;
			this.directories = directories;
		}

		protected SortedList Options
		{
			get { return options; }
		}

		protected SortedList Directories
		{
			get { return directories; }
		}

		protected string GetDirectory(string alias)
		{
			if (directories.Contains(alias))
				return StringTools.CompactStringArray(((string)directories[alias]).Split(new Char[] {'?'}))[0].Trim();
			throw new Exception(string.Format("Unknown alias [{0}]", alias));
		}

		protected string[] GetDirectoryList(string alias)
		{
			if (directories.Contains(alias))
				return StringTools.CompactStringArray(((string)directories[alias]).Split(new Char[] {'?'}));
			throw new Exception(string.Format("Unknown alias [{0}]", alias));
		}

        protected string GetURL(string alias)
        {
            if (directories.Contains(alias))
                return ((string)directories[alias]).Trim();
            throw new Exception(string.Format("Unknown alias [{0}]", alias));
        }

        protected string GetConfigurationString(string alias)
        {
            return null;
        }

	    protected Options GetOption(string alias)
	    {
	        var key = directories.Keys.Cast<string>().FirstOrDefault(e => e.EndsWith(alias));
	        if (!string.IsNullOrEmpty(key))
	        {
	            return (Options) directories[key];
	        }
            
	        return null;
	    }

		protected void AddOption(Options option)
		{
			options[option.category + option.alias1] = option;
		}

	    protected void AddConfigurationOptions(Root root)
	    {
            // Add each configuration as an option.
	        var lastCategory = "~!~!~";
            foreach (var section in root.LocationAndValueSections)
            {
                var category = section.Title;
                if (lastCategory != category)
                {
                    var currentSection = section;

                    var sb = new StringBuilder();
                    foreach (var item in currentSection.Items)
                    {
                        var info = FormatInfo(item);
                        if (!string.IsNullOrEmpty(info))
                            sb.AppendLine(info);
                    }
                    var infoContent = sb.ToString();
                    GoInfoDelegate infoDelegate = null;
                    if (!string.IsNullOrEmpty(infoContent))
                        infoDelegate = (() => infoContent);

                    AddOption(new Options(category, category, "", "", category, (options) =>
                    {
                        foreach (var item in currentSection.Items)
                        {
                            if (!string.IsNullOrEmpty(item.Url))
                                Shell.Browser(item.Url, false, false);
                            if (!string.IsNullOrEmpty(item.Path))
                                Shell.ExplorerPath(item.Path, false, false);
                            if (!string.IsNullOrEmpty(item.Application))
                                Shell.ShellApp(item.Application, null, false, item.RunModal, item.StartIn, ParseProcessWindowStyle(item.WindowStyle));
                        }
                    }, null, infoDelegate));
                    lastCategory = category;
                }
                foreach (var item in section.Items)
                {
                    var tmpItem = item;
                    GoInfoDelegate infoDelegate = null;
                    var info = FormatInfo(item);
                    if (info != null)
                        infoDelegate = () => FormatInfo(tmpItem);

                    if (!string.IsNullOrEmpty(item.Url))
                        AddOption(new Options(category, tmpItem.Name, "", "", tmpItem.Comment ?? tmpItem.Name,
                            (options) => Shell.Browser(tmpItem.Url, false, false),
                            null,
                            infoDelegate));
                    if (!string.IsNullOrEmpty(item.Path))
                        AddOption(new Options(category, tmpItem.Name, "", "", tmpItem.Comment ?? tmpItem.Name,
                            (options) => Shell.ExplorerPath(tmpItem.Path, false, false),
                            null,
                            infoDelegate));
                    if (!string.IsNullOrEmpty(tmpItem.Application))
                        AddOption(new Options(category, tmpItem.Name, "", "", tmpItem.Comment ?? tmpItem.Name,
                            (options) => Shell.ShellApp(tmpItem.Application, tmpItem.Arguments, tmpItem.ArgumentIsFilename, tmpItem.RunModal, tmpItem.StartIn, ParseProcessWindowStyle(tmpItem.WindowStyle)),
                            null,
                            infoDelegate));
                }
            }
	    }

	    private ProcessWindowStyle ParseProcessWindowStyle(string windowStyle)
	    {
	        var processWindowStyle = ProcessWindowStyle.Normal;
            if (!string.IsNullOrEmpty(windowStyle))
            {
                if (!Enum.TryParse(windowStyle, true, out processWindowStyle))
                    processWindowStyle = ProcessWindowStyle.Normal;
            }
	        return processWindowStyle;
	    }

        private string FormatInfo(Item item)
        {
            if (!string.IsNullOrEmpty(item.Username) || !string.IsNullOrEmpty(item.Password))
            {
                try
                {
                    Clipboard.SetText(item.Password);
                }
                catch (Exception ex)
                {
                }

                if (string.IsNullOrEmpty(item.Info))
                {
                    return string.Format(@"{0} login credentials:
Username: {1}
Password: {2}
(or Ctrl-V to paste password)", item.Comment ?? item.Name, item.Username, item.Password);
                }
                return string.Format(@"{0} login credentials:
Username: {1}
Password: {2}
(or Ctrl-V to paste password)
{3}", item.Comment ?? item.Name, item.Username, item.Password, item.Info);
            }

            if (!string.IsNullOrEmpty(item.Info))
                return string.Format(@"{0} info:
{1}", item.Comment ?? item.Name, item.Info);

            return null;
        }

		private bool docMode = false;
		private ArrayList docLines = null;

		protected bool DocMode {
			get { return docMode; }
			set { docMode = value; }
		}

		protected void InitDoc()
		{
			if (docLines == null)
				docLines = new ArrayList();
			docLines.Clear();
		}

		protected bool Doc(string line)
		{
			bool docMode = DocMode;

			if (docMode) {
				docLines.Add(line);
			}
			return docMode;
		}

		protected void ShowDoc(string name)
		{
			StringBuilder text = new StringBuilder();
			text.AppendFormat(@"<HTML>
<BODY>
<H1>){0}</H1>
<TABLE>
<TR><TD>Step</TD><TD>Description</TD></TR>
", name);
			int lineNumber = 0;
			IEnumerator myEnumerator = docLines.GetEnumerator();
			while (myEnumerator.MoveNext()) {
				text.AppendFormat("<TR><TD>{0}</TD><TD>{1}</TD></TR>\n", ++lineNumber, myEnumerator.Current);
			}
			text.Append(@"</TABLE>
</BODY>
</HTML>");
		}
	}
}
