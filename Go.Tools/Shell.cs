using System;
using System.Threading;
using System.Diagnostics;
using System.IO;
using SHDocVw;

// Contains the Internet Explorer reference

namespace Go.Tools
{
	public class Shell
	{
		#region Process
		static public bool IsRunning(string filenameWithoutExtension)
		{
			return GetProcessId(filenameWithoutExtension) != 0 ? true : false;
		}

		static public Int32 GetProcessId(string filenameWithoutExtension)
		{
			Process[] processes = Process.GetProcesses();
			foreach (Process process in processes)
			{
				if (string.Compare(filenameWithoutExtension, process.ProcessName, true) == 0)
					return process.Id;
			}
			return 0;
		}
		#endregion

		#region Shell
		/// <summary>
		/// Shell using windows file associations in specified window style (default: Normal).
		/// </summary>
		/// <returns>
		/// Returns exit code from process.
		/// </returns>
		static public Int32 ShellApp(string executable, string arguments, bool argumentIsFilename, bool wait, string workingDirectory = null, ProcessWindowStyle windowStyle = ProcessWindowStyle.Normal)
		{
			if (!wait && windowStyle == ProcessWindowStyle.Normal)
			{
				Process.Start(executable, argumentIsFilename ? "\"" + arguments + "\"" : arguments);
				return 0;
			}

			var processStartInfo = new ProcessStartInfo
			{
				FileName		= executable,
				Arguments		= argumentIsFilename ? "\"" + arguments + "\"" : arguments,
				UseShellExecute	= true,
				WindowStyle		= windowStyle
			};
			if (!string.IsNullOrEmpty(workingDirectory))
				processStartInfo.WorkingDirectory = workingDirectory;

			var proc = Process.Start(processStartInfo);
			if (proc != null && wait)
			{
                proc.WaitForExit();
				return proc.ExitCode;
			}
			return 0;
		}

		/// <summary>
		/// Shell without using windows file associations in minimized window style.
		/// Output from StdOut and StdErr is returned in output variable.
		/// </summary>
		/// <returns>
		/// Returns exit code from process.
		/// </returns>
		static public Int32 ShellCmd(string executable, string arguments, bool argumentIsFilename, out string output)
		{
			Process proc = new Process();
			proc.StartInfo.FileName					= executable;
			proc.StartInfo.Arguments				= argumentIsFilename ? "\"" + arguments + "\"" : arguments;
			proc.StartInfo.UseShellExecute			= false;
			proc.StartInfo.RedirectStandardOutput	= true;
			proc.StartInfo.RedirectStandardError	= true;
			proc.StartInfo.WindowStyle				= ProcessWindowStyle.Minimized;
			proc.Start();
			output = proc.StandardOutput.ReadToEnd();
			proc.WaitForExit();
			return proc.ExitCode;
		}
		#endregion

		#region Common Shell Programs
		#region Mail
		static public void MailTo(string to, string cc, string bcc, string subject, string body)
		{
			string mailFormat;

			mailFormat = "mailto:";
			if (to.Length > 0)
				mailFormat += StringTools.Text2Address(to);
			if (cc.Length > 0)
				mailFormat += "&Cc=" + StringTools.Text2Address(cc);
			if (bcc.Length > 0)
				mailFormat += "&Bcc=" + StringTools.Text2Address(bcc);
			if (subject.Length > 0)
				mailFormat += "&Subject=" + StringTools.Text2Html(subject, true);
			if (body.Length > 0)
				mailFormat += "&Body=" + StringTools.Text2Html(body, false);


			ShellApp(mailFormat, "", false, false);
		}
		#endregion

		#region MS-Office
		static public Int32 Excel(string filename, bool wait, bool maximized)
		{
			return ShellApp("excel.exe", filename, true, wait, null, maximized ? ProcessWindowStyle.Maximized : ProcessWindowStyle.Normal);
		}

		static public Int32 PowerPoint(string filename, bool wait, bool maximized)
		{
			return ShellApp("powerpnt.exe", filename, true, wait, null, maximized ? ProcessWindowStyle.Maximized : ProcessWindowStyle.Normal);
		}

		static public Int32 Visio(string filename, bool wait, bool maximized)
		{
			return ShellApp("visio.exe", filename, true, wait, null, maximized ? ProcessWindowStyle.Maximized : ProcessWindowStyle.Normal);
		}

		static public Int32 Word(string filename, bool wait, bool maximized)
		{
			return ShellApp("winword.exe", filename, true, wait, null, maximized ? ProcessWindowStyle.Maximized : ProcessWindowStyle.Normal);
		}
		#endregion

		#region Assorted
		static public Int32 Quicken(bool wait, bool maximized)
		{
			string path = RegistryRoots.GetValue(@"HKEY_CLASSES_ROOT\qw\shell\open\command", "command");
		    if (!string.IsNullOrEmpty(path))
		    {
                if (path.Contains(" -"))
		            path = path.Substring(0, path.IndexOf(" -"));
		        if (path.Contains(" %1"))
		            path = path.Substring(0, path.IndexOf(" %1"));
		    }
			if (string.IsNullOrEmpty(path))
				return 1;

			return ShellApp(path, null, false, wait, null, maximized ? ProcessWindowStyle.Maximized : ProcessWindowStyle.Normal);
		}
		#endregion Assorted

		#region OS
		static public Int32 Notepad(string filename, bool wait, bool maximized)
		{
			return ShellApp("notepad++.exe", filename, true, wait, null, maximized ? ProcessWindowStyle.Maximized : ProcessWindowStyle.Normal);
		}

		static public Int32 Browser(string url, bool wait, bool maximized)
		{
			return BrowserChrome(url, wait, maximized);
		}

		static public Int32 BrowserIE(string url, bool wait, bool maximized)
		{
			if (Directory.Exists(@"C:\Program Files (x86)\Internet Explorer"))
				return ShellApp(@"C:\Program Files (x86)\Internet Explorer\iexplore.exe", url, true, wait, null, maximized ? ProcessWindowStyle.Maximized : ProcessWindowStyle.Normal);
			return ShellApp("iexplore.exe", url, true, wait, null, maximized ? ProcessWindowStyle.Maximized : ProcessWindowStyle.Normal);
		}

		static public Int32 BrowserChrome(string url, bool wait, bool maximized)
		{
			return BrowserChrome(new[] {url}, wait, maximized);
		}

		static public Int32 Browser(string[] urls, bool wait, bool maximized)
		{
			return BrowserChrome(urls, wait, maximized);
		}

		static public Int32 BrowserChrome(string[] urls, bool wait, bool maximized)
		{
			const string executablePath = @"C:\Program Files (x86)\Google\Chrome\Application";
			var executable = executablePath + @"\chrome.exe";

			if (!Directory.Exists(executablePath))
				executable = "chrome.exe";

			var ret = 0;
			var delay = 500;
			foreach (var url in urls)
			{
				ret = ShellApp(executable, url, true, wait, null, maximized ? ProcessWindowStyle.Maximized : ProcessWindowStyle.Normal);
				if (ret != 0)
					break;
				Thread.Sleep(delay);
				if (delay <= 5000)
					delay += 500;
			}
			return ret;
		}

		static public Int32 BrowserIE(string[] urls, bool wait, bool maximized)
		{
//            var sb = new StringBuilder();
//            var prefix = "";
//            foreach (var url in urls)
//            {
//                sb.AppendFormat("{0}{1}", prefix, url);
//                prefix = " ";
//            }
//            return Browser(sb.ToString(), wait, maximized);

			InternetExplorer oIE = null;
			Type oType = Type.GetTypeFromProgID("InternetExplorer.Application");
			if (oType != null)
			{
				oIE = (InternetExplorer)Activator.CreateInstance(oType);
			}

			if (oIE != null)
			{
				oIE.Visible = true;

				object oEmpty = String.Empty;
				object oFlags = oEmpty;
				object oOpenInBackgroundTab = 0x1000;

				bool first = true;
				foreach (string url in urls)
				{
					if (!string.IsNullOrEmpty(url))
					{
						object oUrl = url;
						if (!first)
							oFlags = oOpenInBackgroundTab;
						oIE.Navigate2(ref oUrl, ref oFlags, ref oEmpty, ref oEmpty, ref oEmpty);
						first = false;
					}
				}
			}

			return 0;
		}

		static public Int32 ExplorerPath(string path, bool wait, bool maximized)
		{
			// Generate batch file for changing directory locally
			string contents = "";
			if (path.Length >= 2 && path.Substring(1, 1) == ":")
				contents = path.Substring(0, 2) + "\n";
			contents += "cd \"" + path + "\"";
			FileTools.WriteTempFile("GoCD.bat", contents);

			return ShellApp("explorer.exe", string.Format("/e,{0}", path), false, wait, null, maximized ? ProcessWindowStyle.Maximized : ProcessWindowStyle.Normal);
		}

		static public Int32 ExplorerFile(string filename, bool wait, bool maximized)
		{
			return ShellApp("explorer.exe", string.Format("/select,{0}", filename), false, wait, null, maximized ? ProcessWindowStyle.Maximized : ProcessWindowStyle.Normal);
		}
		#endregion

        #region Configuration Files

        public static void OpenAndBackupConfigurationFile(string xmlFilename)
        {
            var goDir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

            var editCopy = goDir + "\\" + xmlFilename;
            var backupCopy = goDir + "\\" + Path.GetFileNameWithoutExtension(xmlFilename) + ".bak";

            string sourceCopy = null;
            if (goDir.EndsWith("\\GO\\bin\\Debug", StringComparison.InvariantCultureIgnoreCase))
            {
                var sourceDir = goDir.Substring(0, goDir.Length - 10) + "\\" + Path.GetFileNameWithoutExtension(xmlFilename);
                sourceCopy = sourceDir + "\\" + xmlFilename;
            }

            Shell.Notepad(editCopy, true, false);

            if (File.Exists(backupCopy))
                File.Delete(backupCopy);
            File.Copy(editCopy, backupCopy);

            if (!string.IsNullOrEmpty(sourceCopy))
            {
                if (File.Exists(sourceCopy))
                    File.Delete(sourceCopy);
                File.Copy(editCopy, sourceCopy);
            }
        }

        #endregion Configuration Files

        #region Development Tools
        static public Int32 Merge(string filename1, string filename2, bool wait, bool maximized)
		{
			return ShellApp("merge.exe", string.Format("\"{0}\" \"{1}\"", filename1, filename2), false, wait, null, maximized ? ProcessWindowStyle.Maximized : ProcessWindowStyle.Normal);
		}

        static public Int32 BeyondCompare(string path1, string path2, bool wait)
        {
            return ShellApp(@"C:\Program Files (x86)\Beyond Compare 3\BCompare.exe", string.Format("\"{0}\" \"{1}\"", path1, path2), false, wait);
        }
		#endregion
		#endregion
	}
}
