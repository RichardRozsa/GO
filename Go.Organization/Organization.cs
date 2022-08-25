using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Text;
using Go.Tools;

namespace Go
{
	public class Organization : GoImplementation {
		#region Initialization
		static private Organization	    _organization = null;
        static private ProgressEvent    _progressEvent = null;
		
		static public Organization GetOrganization(ref SortedList options, ref SortedList directories, ref ProgressEvent pe) {
            _progressEvent = pe;
            if (_organization == null)
				_organization = new Organization(ref options, ref directories);
			return _organization;
		}

		static public Organization GetOrganization() {
			if (_organization == null)
				throw new Exception("Can't instantiate Individual class without parameters");
			return _organization;
		}

		public Organization(ref SortedList options, ref SortedList directories)
			: base(ref options, ref directories) {
			var root = ConfigurationFile.GetLocalConfiguration(ref directories, "Go.Organization.xml");
            AddConfigurationOptions(root);

		    string category = "MetaDapper";
            AddOption(new Options(category, "Example", "", "", "Example", Example, ExampleSyntax, ExampleInfo));

		    category = "Administrative Tools";
			AddOption(new Options(category, "ComponentServices",				"",	"",		"Administrative Tools: Component Services",							ComponentServices, null, null));
			AddOption(new Options(category, "ComputerManagement",				"",	"",		"Administrative Tools: Computer Management",						ComputerManagement, null, null));
			AddOption(new Options(category, "DataSourcesOdbc",					"",	"",		"Administrative Tools: Data Sources (ODBC)",						DataSourcesOdbc, null, null));
			AddOption(new Options(category, "EventViewer",						"",	"",		"Administrative Tools: Event Viewer",								EventViewer, null, null));
			AddOption(new Options(category, "LocalSecurityPolicy",				"",	"",		"Administrative Tools: Local Security Policy",						LocalSecurityPolicy, null, null));
			AddOption(new Options(category, "InternetInformationServices",		"",	"",		"Administrative Tools: Internet Information Services",				InternetInformationServices, null, null));
			AddOption(new Options(category, "MicrosoftDotNetFramework11Configuration",	"",	"",		"Administrative Tools: Microsoft .Net Framework 1.1 Configuration",		MicrosoftDotNetFramework11Configuration, null, null));
			AddOption(new Options(category, "MicrosoftDotNetFramework11Wizards","",	"",		"Administrative Tools: Microsoft .Net Framework 1.1 Wizards",		MicrosoftDotNetFramework11Wizards, null, null));
			AddOption(new Options(category, "Performance",						"",	"",		"Administrative Tools: Performance",								Performance, null, null));
			AddOption(new Options(category, "ServerExtensionsAdministrator",	"",	"",		"Administrative Tools: Server Extensions Administrator",			ServerExtensionsAdministrator, null, null));
			AddOption(new Options(category, "Services",							"",	"",		"Administrative Tools: Services",									Services, null, null));

			category = "Control Panel";
			AddOption(new Options(category, "AddRemovePrograms",				"", "",		"Control Panel: Add/Remove Programs",								AddRemovePrograms, null, null));
			AddOption(new Options(category, "DisplayProperties",				"", "",		"Control Panel: Display Properties",								DisplayProperties, null, null));
			AddOption(new Options(category, "InternetOptions",					"", "",		"Control Panel: Internet Options",									InternetOptions, null, null));
			AddOption(new Options(category, "SoundAndAudio",					"", "",		"Control Panel: Sound and Audio",									SoundAndAudio, null, null));
			AddOption(new Options(category, "NetworkConnections",				"", "",		"Control Panel: Network Connections",								NetworkConnections, null, null));
			AddOption(new Options(category, "PowerOptions",						"", "",		"Control Panel: Power Options",										PowerOptions, null, null));
			AddOption(new Options(category, "UserAccounts",						"", "",		"Control Panel: User Accounts",										UserAccounts, null, null));
			AddOption(new Options(category, "SystemProperties",					"", "",		"Control Panel: System Properties",									SystemProperties, null, null));
			AddOption(new Options(category, "AutomaticUpdates",					"", "",		"Control Panel: Automatic Updates",									AutomaticUpdates, null, null));

			category = "Executables";
			AddOption(new Options(category, "IsRunning",						"", "",		"Checks if program is running",										IsRunning, IsRunningSyntax, null));
			AddOption(new Options(category, "GetProcessId",						"", "",		"Gets process id for running program",								GetProcessId, GetProcessIdSyntax, null));
			AddOption(new Options(category, "ShellApp",							"", "",		"Executes specified program with options",							ShellApp, ShellAppSyntax, null));
			AddOption(new Options(category, "ShellCmd",							"", "",		"Executes specified command-line program and captures output",		ShellCmd, ShellCmdSyntax, null));
			AddOption(new Options(category, "Mail",								"", "",		"Opens mail with options filled in",								Mail, MailSyntax, null));
			AddOption(new Options(category, "Excel",							"", "",		"Opens file with MS-Excel",											Excel, ExcelSyntax, null));
			AddOption(new Options(category, "PowerPoint",						"", "",		"Opens file with MS-Powerpoint",									PowerPoint, PowerPointSyntax, null));
			AddOption(new Options(category, "Visio",							"", "",		"Opens file with MS-Visio",											Visio, VisioSyntax, null));
			AddOption(new Options(category, "Word",								"", "",		"Opens file with MS-Word",											Word, WordSyntax, null));
			AddOption(new Options(category, "Notepad",							"", "",		"Opens file with Notepad",											Notepad, NotepadSyntax, null));
			AddOption(new Options(category, "Browser",							"", "",		"Opens file with browser",											Browser, BrowserSyntax, null));
			AddOption(new Options(category, "ExplorerPath",						"", "",		"Opens windows explorer positioned at path",						ExplorerPath, ExplorerPathSyntax, null));
			AddOption(new Options(category, "ExplorerFile",						"", "",		"Opens windows explorer positioned at file",						ExplorerFile, ExplorerFileSyntax, null));
			AddOption(new Options(category, "WinZip",							"", "",		"Opens file with WinZip",											WinZip, WinZipSyntax, null));

			category = "Assorted";
			AddOption(new Options(category, "Away",								"", "",		"Shows browser with default message 'Go Away'",						Away, AwaySyntax, null));
			AddOption(new Options(category, "Calc",								"",	"",		"Calculates mathematical expression",								Calc, CalcSyntax, null));
			AddOption(new Options(category, "FormatXml",						"", "",		"Formats XML file",													FormatXml, FormatXmlSyntax, null));
			AddOption(new Options(category, "Touch",							"", "",		"Changes date of filespec to STAMP",								Touch, TouchSyntax, null));
			AddOption(new Options(category, "Wsdl",								"", "",		"Invokes the Wsdl utility on the specified wsdlFile and generates csFile",		Wsdl, WsdlSyntax, WsdlInfo));
			AddOption(new Options(category, "Unsign",							"", "",		"Unsigns file",														Unsign, UnsignSyntax, null));
			AddOption(new Options(category, "CreateNetworkMappings",			"", "",		"Create network drive mappings",									CreateNetworkMappings, null, null));
			AddOption(new Options(category, "ReleaseNetworkMappings",			"", "",		"Releases network drive mappings",									ReleaseNetworkMappings, null, null));
		}
		#endregion Initialization

		#region Example

		static string ExampleSyntax(bool syntaxOnly, bool switchesOnly)
		{
			return "whatever syntax for this option";
		}

		public void Example(Options option)
        {
			// Example of some custom routine

			// Get XML specified values
            var examplePath = GetURL("ExamplePath");
            var exampleFilter = GetURL("ExampleFilter");

			// Do something with the values - in this case, find the latest file
            var files = Directory.GetFiles(examplePath, exampleFilter);
            var file = files.Max(e => e);

			// Open the file with Excel
            Shell.Excel(file, false, true);
        }

		static string ExampleInfo()
		{
			return 
@"Registration number: 123456789
Product Code:        ABCD-12345-ABC123

HOWTO:
- Any info you want to show the user in a popup box.
";
		}
		
		#endregion Example

		#region Administrative Tools
		public void ComponentServices(Options option) {
			Shell.ShellApp(GetDirectory("System32") + @"\Com\comexp.msc", "", false, false);
		}

		public void ComputerManagement(Options option) {
			Shell.ShellApp(GetDirectory("System32") + @"\compmgmt.msc", "/s", false, false);
		}

		public void DataSourcesOdbc(Options option) {
			Shell.ShellApp(GetDirectory("System32") + @"\odbcad32.exe", "", false, false);
		}

		public void EventViewer(Options option) {
			Shell.ShellApp(GetDirectory("System32") + @"\eventvwr.msc", "/s", false, false);
		}

		public void LocalSecurityPolicy(Options option) {
			Shell.ShellApp(GetDirectory("System32") + @"\secpol.msc", "/s", false, false);
		}

		public void InternetInformationServices(Options option) {
			Shell.ShellApp(GetDirectory("System32") + @"\inetsrv\iis.msc", "/s", false, false);
		}

		public void MicrosoftDotNetFramework11Configuration(Options option) {
			Shell.ShellApp(GetDirectory("Windows") + @"\Microsoft.NET\Framework\v1.1.4322\mscorcfg.msc", "", false, false);
		}

		public void MicrosoftDotNetFramework11Wizards(Options option) {
			Shell.ShellApp(GetDirectory("Windows") + @"\Microsoft.NET\Framework\v1.1.4322\ConfigWizards.exe", "", false, false);
		}

		public void Performance(Options option) {
			Shell.ShellApp(GetDirectory("System32") + @"\perfmon.msc", "/s", false, false);
		}

		public void ServerExtensionsAdministrator(Options option) {
			Shell.ShellApp(@"C:\Program Files\Common Files\Microsoft Shared\web server extensions\40\bin\fpmmc.msc", "", false, false);
		}

		public void Services(Options option) {
			Shell.ShellApp(GetDirectory("System32") + @"\services.msc", "/s", false, false);
		}
		#endregion Administrative Tools

		#region Control Panel
		public void AddRemovePrograms(Options option) {
			Shell.ShellApp("appwiz.cpl", "", false, false);
		}

		public void DisplayProperties(Options option) {
			Shell.ShellApp("desk.cpl", "", false, false);
		}

		public void InternetOptions(Options option) {
			Shell.ShellApp("inetcpl.cpl", "", false, false);
		}
 
		public void SoundAndAudio(Options option) {
			Shell.ShellApp("mmsys.cpl", "", false, false);
		}
 
		public void NetworkConnections(Options option) {
			Shell.ShellApp("ncpa.cpl", "", false, false);
		}

		public void PowerOptions(Options option) {
			Shell.ShellApp("powercfg.cpl", "", false, false);
		}

		public void UserAccounts(Options option) {
			Shell.ShellApp("nusrmgr.cpl", "", false, false);
		}

		public void SystemProperties(Options option) {
			Shell.ShellApp("sysdm.cpl", "", false, false);
		}
 
		public void AutomaticUpdates(Options option) {
			Shell.ShellApp("wuaucpl.cpl", "", false, false);
		}
		#endregion Control Panel

		#region Executables
		public string IsRunningSyntax(bool syntaxOnly, bool switchesOnly) {
			return "-F <filenameWithoutExtension>";
		}
		public void IsRunning(Options option) {
			var argOptions = new SortedList();
			argOptions["-F"]		= "";		// <-F filenameWithoutExtension>
			CommandLine.EvalCommandLine(ref argOptions);
			var filenameWithoutExtension	= (string)argOptions["-F"];

			if (filenameWithoutExtension.Length == 0)
				throw new Exception("Error: -F <filenameWithoutExtension> option missing");

			var ret = Shell.IsRunning(filenameWithoutExtension);
			MessageBox.Show(filenameWithoutExtension + " is " + (ret ? "" : "not ") + "running", "IsRunning");
		}

		public string GetProcessIdSyntax(bool syntaxOnly, bool switchesOnly) {
			return "-F <filenameWithoutExtension>";
		}
		public void GetProcessId(Options option) {
			var argOptions = new SortedList();
			argOptions["-F"]		= "";		// <-F filenameWithoutExtension>
			CommandLine.EvalCommandLine(ref argOptions);
			var filenameWithoutExtension	= (string)argOptions["-F"];

			if (filenameWithoutExtension.Length == 0)
				throw new Exception("Error: -F <filenameWithoutExtension> option missing");

			var ret = Shell.GetProcessId(filenameWithoutExtension);
			MessageBox.Show(filenameWithoutExtension + " has been identified with process " + ret, "GetProcessId");
		}

		public string ShellAppSyntax(bool syntaxOnly, bool switchesOnly) {
			var ret = "-E <executable> [-A <arguments> [-F]] [-W]";
			if (!switchesOnly)
				ret += @"
-F argument indicates -A <arguments> is a filename
-W arument indicates Go should wait until executable exits";

			return ret;
		}
		public void ShellApp(Options option) {
			var argOptions = new SortedList();
			argOptions["-E"] = "";		// -E <executable>
			argOptions["-A"] = "";		// -A <arguments>
			argOptions["-F"] = false;	// -F
			argOptions["-W"] = false;	// -W
			CommandLine.EvalCommandLine(ref argOptions);
			var	executable			= (string)argOptions["-E"];
			var	arguments			= (string)argOptions["-A"];
			var	argumentIsFilename	= (bool)argOptions["-F"];
			var	wait				= (bool)argOptions["-W"];

			if (executable.Length == 0)
				throw new Exception("Error: -E <executable> option missing");
			if (arguments.Length == 0 && argumentIsFilename)
				throw new Exception("Error: -F option specified without -A <arguments> option");

			Shell.ShellApp(executable, arguments, argumentIsFilename, wait, null, ProcessWindowStyle.Normal);
		}

		public string ShellCmdSyntax(bool syntaxOnly, bool switchesOnly) {
			var ret = "-E <executable> [-A <arguments> [-F]]";
			if (!switchesOnly)
				ret += @"
-F parameter indicates -A <arguments> is a filename";

			return ret;
		}
		public void ShellCmd(Options option) {
			var argOptions = new SortedList();
			argOptions["-E"] = "";		// -E <executable>
			argOptions["-A"] = "";		// -A <arguments>
            argOptions["-F"] = false;	// -F
			CommandLine.EvalCommandLine(ref argOptions);
			var	executable			= (string)argOptions["-E"];
			var	arguments			= (string)argOptions["-A"];
			var	argumentIsFilename	= (bool)argOptions["-F"];

			if (executable.Length == 0)
				throw new Exception("Error: -E <executable> option missing");
			if (arguments.Length == 0 && argumentIsFilename)
				throw new Exception("Error: -F option specified without -A <arguments> option");

			string output;
			Shell.ShellCmd(executable, arguments, argumentIsFilename, out output);
			MessageBox.Show(output, "Output from " + executable + " " + arguments);
		}

		public string MailSyntax(bool syntaxOnly, bool switchesOnly) {
			return "-To <alias list> [-Cc <alias list>] [-Bcc <alias list>] [-Subject <subject text>] [-Body <body text>]";
		}
		public void Mail(Options option) {
			var argOptions = new SortedList();
			argOptions["-To"]		= "";		// -To <alias list>
			argOptions["-Cc"]		= "";		// -Cc <alias list>
			argOptions["-Bcc"]		= "";		// -Bcc <alias list>
			argOptions["-Subject"]	= "";		// -Subject <subject text>
			argOptions["-Body"]		= "";		// -Body <body text>
			CommandLine.EvalCommandLine(ref argOptions);
			var	to		= (string)argOptions["-To"];
			var	cc		= (string)argOptions["-Cc"];
			var	bcc		= (string)argOptions["-Bcc"];
			var	subject	= (string)argOptions["-Subject"];
			var	body	= (string)argOptions["-Body"];

			if (to.Length == 0)
				throw new Exception("Error: -To <alias list> option missing");

			Shell.MailTo(to, cc, bcc, subject, body);
		}

		public string ExcelSyntax(bool syntaxOnly, bool switchesOnly) { return "[-F <Filename>]"; }
		public void Excel(Options option) {
			var argOptions = new SortedList();
			argOptions["-F"] = "";		// -F <filename>
			CommandLine.EvalCommandLine(ref argOptions);
			var	filename = (string)argOptions["-F"];

			Shell.Excel(filename, false, false);
		}

		public string PowerPointSyntax(bool syntaxOnly, bool switchesOnly) { return "[-F <Filename>]"; }
		public void PowerPoint(Options option) {
			var argOptions = new SortedList();
			argOptions["-F"] = "";		// -F <filename>
			CommandLine.EvalCommandLine(ref argOptions);
			var	filename = (string)argOptions["-F"];

			Shell.PowerPoint(filename, false, false);
		}

		public string VisioSyntax(bool syntaxOnly, bool switchesOnly) { return "[-F <Filename>]"; }
		public void Visio(Options option) {
			var argOptions = new SortedList();
			argOptions["-F"] = "";		// -F <filename>
			CommandLine.EvalCommandLine(ref argOptions);
			var	filename = (string)argOptions["-F"];

			Shell.Visio(filename, false, false);
		}

		public string WordSyntax(bool syntaxOnly, bool switchesOnly) { return "[-F <Filename>]"; }
		public void Word(Options option) {
			var argOptions = new SortedList();
			argOptions["-F"] = "";		// -F <filename>
			CommandLine.EvalCommandLine(ref argOptions);
			var	filename = (string)argOptions["-F"];

			Shell.Word(filename, false, false);
		}

		public string NotepadSyntax(bool syntaxOnly, bool switchesOnly) { return "[-F <Filename>]"; }
		public void Notepad(Options option) {
			var argOptions = new SortedList();
			argOptions["-F"] = "";		// -F <filename>
			CommandLine.EvalCommandLine(ref argOptions);
			var	filename = (string)argOptions["-F"];

			Shell.Notepad(filename, false, false);
		}

		public string BrowserSyntax(bool syntaxOnly, bool switchesOnly) { return "[-F <Filename>]"; }
		public void Browser(Options option) {
			var argOptions = new SortedList();
			argOptions["-F"] = "";		// -F <filename>
			CommandLine.EvalCommandLine(ref argOptions);
			var	filename = (string)argOptions["-F"];

			Shell.Browser(filename, false, false);
		}

		public string ExplorerPathSyntax(bool syntaxOnly, bool switchesOnly) { return "-P <Path>"; }
		public void ExplorerPath(Options option) {
			var argOptions = new SortedList();
			argOptions["-P"] = "";		// -P <path>
			CommandLine.EvalCommandLine(ref argOptions);
			var	path = (string)argOptions["-P"];

			if (path.Length == 0)
				throw new Exception("Error: -P <path> option missing");
			
			Shell.ExplorerPath(path, false, false);
		}

		public string ExplorerFileSyntax(bool syntaxOnly, bool switchesOnly) { return "-F <Filespec>"; }
		public void ExplorerFile(Options option) {
			var argOptions = new SortedList();
			argOptions["-F"] = "";		// -F <filespec>
			CommandLine.EvalCommandLine(ref argOptions);
			var	filespec = (string)argOptions["-F"];

			if (filespec.Length == 0)
				throw new Exception("Error: -F <filespec> option missing");

			Shell.ExplorerFile(filespec, false, false);
		}

		public string WinZipSyntax(bool syntaxOnly, bool switchesOnly) { return "[-F <filename>]"; }
		public void WinZip(Options option) {
			var argOptions = new SortedList();
			argOptions["-F"] = "";		// -F <filename>
			CommandLine.EvalCommandLine(ref argOptions);
			var	filename = (string)argOptions["-F"];

			WinZip(filename);
		}

		public Int32 WinZip(string filename)
		{
			return Shell.ShellApp(GetDirectory("WinZipDir") + @"\WINZIP32.EXE", filename, true, true, null, ProcessWindowStyle.Normal);
		}
		#endregion Executables

		#region Servers/Users

		public string	NetSendSyntax(bool syntaxOnly, bool switchesOnly) { return "-S <Server> [-F <Firstname> -L <Lastname>] -M <Message>"; }
		public void NetSend(Options option) {
			var argOptions = new SortedList();
			argOptions["-S"] = "";		// -S : server
			argOptions["-F"] = "";		// -F : firstname
			argOptions["-L"] = "";		// -L : lastname
			argOptions["-M"] = "";		// -M : message
			CommandLine.EvalCommandLine(ref argOptions);
			var	server			= (string)argOptions["-S"];
			var	firstname		= (string)argOptions["-F"];
			var	lastname		= (string)argOptions["-L"];
			var	msg				= (string)argOptions["-M"];

			if (server.Length == 0)
				throw new Exception("No -S server provided");
			if (msg.Length == 0)
				throw new Exception("No -M message provided");

			NetSend(server, msg, firstname, lastname);
		}

		public bool NetSend(string server, string msg, string firstname, string lastname) {
			var cmd = string.Format(" {0} {1} says: {2}", firstname, lastname, msg);
			var ret = Ping(server, firstname, lastname);
			if (ret) {
				string output;
				ret = Shell.ShellCmd("net", "send " + server + cmd, false, out output) == 0 ? true : false;
			}
			if (ret)
				MessageBox.Show("Message sent to [" + server + "]:" + cmd, "GO NetSend");
			else
				MessageBox.Show("Message NOT sent to [" + server + "]:" + cmd, "GO NetSend");
	
			return ret;
		}

		public string	PingSyntax(bool syntaxOnly, bool switchesOnly) { return "-S <Server> [-F <Firstname> -L <Lastname>]"; }
		public void Ping(Options option) {
			var argOptions = new SortedList();
			argOptions["-S"] = "";		// -S : server/user to ping
			argOptions["-F"] = "";		// -F : firstname
			argOptions["-L"] = "";		// -L : lastname
			CommandLine.EvalCommandLine(ref argOptions);
			var	server			= (string)argOptions["-S"];
			var	firstname		= (string)argOptions["-F"];
			var	lastname		= (string)argOptions["-L"];

			var response = Ping(server, firstname, lastname);
			PingMsg(response, server, firstname, lastname);
		}

		private static bool Ping(string server, string firstname, string lastname) {
			var response = false;

			if (server.Length > 0) {
				string output;
				var ret = Shell.ShellCmd("ping", "-n 1 -w 500 " + server, false, out output);
				if (output.IndexOf("Received = 1") >= 0)
					response = true;
			}
			return response;
		}

		private static void PingMsg(bool response, string server, string firstname, string lastname) {
			if (firstname.Length == 0)
				MessageBox.Show(string.Format("Computer ({0}) is {1}currently logged in.", server, response ? "" : "not "));
			else
				MessageBox.Show(string.Format("{0} {1}'s computer ({2}) is {3}currently logged in.", firstname, lastname, server, response ? "" : "not "));
		}
		#endregion Servers/Users

		#region Assorted
		static string	AwaySyntax(bool syntaxOnly, bool switchesOnly) { return "[-M <message default=Go Away>] -S [<size 1-6>]"; }
		public void		Away(Options option)
		{
			var argOptions = new SortedList();
			argOptions["-M"] = "";		// -M message
			argOptions["-S"] = "";		// -S <H#> size
			CommandLine.EvalCommandLine(ref argOptions);
			var	message		= (string)argOptions["-M"];
			var	size		= (string)argOptions["-S"];

			if (size.Length == 0)
				size = "1";

			try {
				int i = Convert.ToInt32(size);
				if (i < 1 || i > 6)
					throw new Exception("Invalid -S size where values between 1-6 are allowed [" + size + "]");
			}
			catch (Exception) {
				throw new Exception("Invalid -S size value [" + size + "]");
			}

			if (message.Length == 0)
				message = "Go Away";

			string contents = String.Format(@"<HTML>
<BODY>
<CENTER>
<H{0}>{1}</H{0}>
</CENTER>
</BODY>
</HTML>", size, message);

			string file = FileTools.WriteTempFile("Go Away.html", contents);
			Shell.Browser(file, false, true);
		}

		static string	CalcSyntax(bool syntaxOnly, bool switchesOnly){
            var syntax = "-E <expression>";
			if (!switchesOnly)
            {
				var calc = new Calculator();
                syntax += calc.Syntax();
			}
            return syntax;
		}
		public void		Calc(Options option) {
			var argOptions = new SortedList();
			argOptions["-E"] = "";		// -E <expression>
			CommandLine.EvalCommandLine(ref argOptions);
			var	expression	= (string)argOptions["-E"];

			var calc = new Calculator();
			var ret = calc.Evaluate(expression);
			MessageBox.Show(expression + " = " + calc.FormatNumber(ret), "GO Calculator");
		}

		static string	FormatXmlSyntax(bool syntaxOnly, bool switchesOnly) { return "-S <srcFile> -D <dstFile>"; }
		public void		FormatXml(Options option) {
			var argOptions = new SortedList();
			argOptions["-S"] = "";		// -S <srcFile>
			argOptions["-D"] = "";		// -D <dstFile>
			CommandLine.EvalCommandLine(ref argOptions);
			var	srcFile		= (string)argOptions["-S"];
			var	dstFile		= (string)argOptions["-D"];

			if (srcFile.Length == 0)
				throw new Exception("Missing -S <SrcFile> parameter");
			if (!File.Exists(srcFile))
				throw new Exception("Missing -S " + srcFile + " file");
			if (dstFile.Length == 0)
				throw new Exception("Missing -D <DstFile> parameter");

			var tfc = new TextFileContents(srcFile);
			var contents = tfc.Contents;
			tfc = null;

			var output = "";
			var lines = contents.Split(new char[] {'\r', '\n'});
			var indent = -1;
			foreach (var roLine in lines) {
				var line = StringTools.AllTrim(roLine);
				var i = line.IndexOf('>');
				for ( ; i >= 0; ) {
					if (line.Substring(line.IndexOf('<') + 1, 1) == "/")
						indent--;
					for (var n = 0; n < indent; n++)
						output += "\t";
					output += line.Substring(0, i + 1) + Environment.NewLine;

					indent++;
					if (line.Substring(line.IndexOf('<') + 1, 1) == "/" || line.Substring(i - 1, 1) == "/")
						indent--;
					line = line.Substring(i + 1);
					i = line.IndexOf('>');
				}
				if (line.Length > 0)
					output += line + Environment.NewLine;
			}

			tfc = new TextFileContents(dstFile) {Contents = output};
		    tfc = null;
		}

		static string	TouchSyntax(bool syntaxOnly, bool switchesOnly)
		{
			const string syntax = "-T <timestamp> -F <filespec>";
			if (switchesOnly)
				return syntax;
			return syntax + @"
 timestamp format: {anything C# can interpret}";
		}
		public void		Touch(Options option)
		{
			var argOptions = new SortedList();
			argOptions["-T"] = "";		// [-T <STAMP>]
			argOptions["-F"] = "";		// -F <filespec>
			CommandLine.EvalCommandLine(ref argOptions);
			var	stamp		= (string)argOptions["-T"];
			var	filespec	= (string)argOptions["-F"];

			if (filespec.Length == 0)
				throw new Exception("No -F <filespec>");
			if (stamp.Length == 0)
				throw new Exception("No -T <stamp>");

			var dt = Convert.ToDateTime(stamp);
			FileTools.TouchFilename(filespec, dt);
		}
		
		static string	UnsignSyntax(bool syntaxOnly, bool switchesOnly) { return "-F <file>"; }
		public void		Unsign(Options option)
		{
			var argOptions = new SortedList();
			argOptions["-F"] = "";		// -F <file>
			CommandLine.EvalCommandLine(ref argOptions);
			var	file		= (string)argOptions["-F"];

			if (file.Length == 0)
				throw new Exception("No -F file");
			if (!File.Exists(file))
				throw new Exception("File " + file + " doesn't exist");

			var dst = GetDirectory("Temp") + @"\license.b64";
			File.Copy(file, dst, true);
			WinZip(dst);
			File.Delete(dst);
		}

		public void CreateNetworkMappings(Options option)
		{
			string output;
			Shell.ShellCmd("net", "use G: \"\\\\server\\share\"", false, out output);
		}

		public void ReleaseNetworkMappings(Options option)
		{
			string output;
			Shell.ShellCmd("net", "use G: /D", false, out output);
		}

		static string	WsdlInfo()
		{
			return @"This may not be needed as we we can generate directly from java without wsdl files.
Also, conversion done here may not be necessary as localhost:10000 is default.";
		}
		static string	WsdlSyntax(bool syntaxOnly, bool switchesOnly) { return "-W <wsdlFile> -C <csFile> -N <namespace>"; }
		public void		Wsdl(Options option)
		{
			var argOptions = new SortedList();
			argOptions["-W"] = "";		// -W <wsdlFile>
			argOptions["-C"] = "";		// -C <csFile>
			argOptions["-N"] = "";		// -N <namespace>
			CommandLine.EvalCommandLine(ref argOptions);
			var	wsdlFile		= (string)argOptions["-W"];
			var	csFile			= (string)argOptions["-C"];
			var	namespaceString	= (string)argOptions["-N"];

			if (wsdlFile.Length == 0)
				throw new Exception("Missing -W " + wsdlFile + "wsdlFile");
			if (csFile.Length == 0)
				throw new Exception("Missing -C " + csFile + " csFile");
			if (namespaceString.Length == 0)
				throw new Exception("Missing -N " + namespaceString + " namespaceString");
			if (!File.Exists(wsdlFile))
				throw new Exception("File " + wsdlFile + " not found");

			var csTempPath = Environment.ExpandEnvironmentVariables("%TEMP%") + @"\wsdl.cs";
			string output;
			Shell.ShellCmd("wsdl.exe", 
				String.Format("/language:CS /out:\"{0}\" /namespace:{1} {2}", 
					csTempPath, namespaceString, wsdlFile), 
				false, out output);

			var tfc = new TextFileContents(csTempPath);
			var csContents = tfc.Contents;
			tfc = null;

			csContents.Replace(	"this.Url = \"http://sagitta.virgil.nl:51000", 
								"this.Url = \"http://localhost:10000");

			File.Delete(csTempPath);
			tfc = new TextFileContents(csFile) {Contents = csContents};
		    tfc = null;

			MessageBox.Show("Done generating " + csFile + " WSDL file.", "GO wsdl");
		}
		#endregion Assorted

		#region Stuff
		public void ReadmeFilelist(Options option)
		{
			var argOptions = new SortedList();
			argOptions["-D"] = "";		// -D <Deployment Directory>
			CommandLine.EvalCommandLine(ref argOptions);
			var	directory	= (string)argOptions["-D"];

			if (directory.Length > 0 && !Directory.Exists(directory))
				throw new Exception("Directory " + directory + " doesn't exist");
			directory = Environment.CurrentDirectory;

			var files = new SortedList();
			DirectoryTools.FindFiles(ref files, directory, "", true);

			var text = new StringBuilder();
			text.AppendFormat(@"Insert the following text in the README.TXT for directory:
  {0}

--------------
DELIVERABLE(S)
--------------

", directory);

			int iDirCnt = 0;
			string lastDir = "~";
			IDictionaryEnumerator fileEnum;
			for (var i = 1; i <= 2; i++) {
				fileEnum = files.GetEnumerator();
				while (fileEnum.MoveNext()) {
					var process = false;
					var file = (string)fileEnum.Key;
					var filename = file.Substring(file.LastIndexOf('\\') + 1);
					var dir = file.Substring(directory.Length + 1, file.Length - (directory.Length + 1) - filename.Length);
					if (dir.EndsWith("\\"))
						dir = dir.Substring(0, dir.Length - 1);
					if (dir.Length == 0) {
						dir = "Root";
						if (i == 1)
							process = true;
					}
					else {
						if (i == 2)
							process = true;
					}

					if (process) {
						if (filename.ToUpper() != "README.TXT" && filename.ToUpper() != "VSSVER.SCC") {
							if (dir != lastDir) {
								string sDirCnt = (++iDirCnt) + ".    ";
								text.AppendFormat(@"{0}{1} folder
", sDirCnt.Substring(0, 4), dir);
								lastDir = dir;
							}
							var dtc = new DateTimeComponents(File.GetLastWriteTime(file));
							text.AppendFormat(
								@"    - Filename: {0}
    - Filedate: {1} {2}

", filename, dtc.FormattedDate, dtc.FormattedTime);
						}
					}
				}
			}

			var readmePath = FileTools.WriteTempFile("ReadmeFilelist.txt", text.ToString());
			Shell.Notepad(readmePath, false, false);
		}
		#endregion Stuff
	}
}
