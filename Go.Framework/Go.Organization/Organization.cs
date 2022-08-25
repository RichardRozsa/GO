using System;
using System.Collections;
using System.Windows.Forms;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Xml;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Win32;

namespace Go
{
	public class Organization : GoImplementation {
		#region Initialization
		static private Organization	organization = null;
		
		static public Organization GetOrganization(ref SortedList options, ref SortedList directories) {
			if (organization == null)
				organization = new Organization(ref options, ref directories);
			return organization;
		}

		static public Organization GetOrganization() {
			if (organization == null)
				throw new Exception("Can't instantiate Individual class without parameters");
			return organization;
		}

		public Organization(ref SortedList options, ref SortedList directories)
			: base(ref options, ref directories) {
			ConfigurationFile.GetLocalConfiguration(ref directories, "Go.Organization.xml");
			
			string category;

			category = "Administrative Tools";
			AddOption(new Options(category, "ComponentServices",				"",	"",		"Administrative Tools: Component Services",							new GoExecuteDelegate(ComponentServices), null, null));
			AddOption(new Options(category, "ComputerManagement",				"",	"",		"Administrative Tools: Computer Management",						new GoExecuteDelegate(ComputerManagement), null, null));
			AddOption(new Options(category, "DataSourcesODBC",					"",	"",		"Administrative Tools: Data Sources (ODBC)",						new GoExecuteDelegate(DataSourcesODBC), null, null));
			AddOption(new Options(category, "EventViewer",						"",	"",		"Administrative Tools: Event Viewer",								new GoExecuteDelegate(EventViewer), null, null));
			AddOption(new Options(category, "LocalSecurityPolicy",				"",	"",		"Administrative Tools: Local Security Policy",						new GoExecuteDelegate(LocalSecurityPolicy), null, null));
			AddOption(new Options(category, "InternetInformationServices",		"",	"",		"Administrative Tools: Internet Information Services",				new GoExecuteDelegate(InternetInformationServices), null, null));
			AddOption(new Options(category, "MicrosoftDotNetFramework11Configuration",	"",	"",		"Administrative Tools: Microsoft .Net Framework 1.1 Configuration",		new GoExecuteDelegate(MicrosoftDotNetFramework11Configuration), null, null));
			AddOption(new Options(category, "MicrosoftDotNetFramework11Wizards","",	"",		"Administrative Tools: Microsoft .Net Framework 1.1 Wizards",		new GoExecuteDelegate(MicrosoftDotNetFramework11Wizards), null, null));
			AddOption(new Options(category, "Performance",						"",	"",		"Administrative Tools: Performance",								new GoExecuteDelegate(Performance), null, null));
			AddOption(new Options(category, "ServerExtensionsAdministrator",	"",	"",		"Administrative Tools: Server Extensions Administrator",			new GoExecuteDelegate(ServerExtensionsAdministrator), null, null));
			AddOption(new Options(category, "Services",							"",	"",		"Administrative Tools: Services",									new GoExecuteDelegate(Services), null, null));

			category = "Control Panel";
			AddOption(new Options(category, "AddRemovePrograms",				"", "",		"Control Panel: Add/Remove Programs",								new GoExecuteDelegate(AddRemovePrograms), null, null));
			AddOption(new Options(category, "DisplayProperties",				"", "",		"Control Panel: Display Properties",								new GoExecuteDelegate(DisplayProperties), null, null));
			AddOption(new Options(category, "InternetOptions",					"", "",		"Control Panel: Internet Options",									new GoExecuteDelegate(InternetOptions), null, null));
			AddOption(new Options(category, "SoundAndAudio",					"", "",		"Control Panel: Sound and Audio",									new GoExecuteDelegate(SoundAndAudio), null, null));
			AddOption(new Options(category, "NetworkConnections",				"", "",		"Control Panel: Network Connections",								new GoExecuteDelegate(NetworkConnections), null, null));
			AddOption(new Options(category, "PowerOptions",						"", "",		"Control Panel: Power Options",										new GoExecuteDelegate(PowerOptions), null, null));
			AddOption(new Options(category, "UserAccounts",						"", "",		"Control Panel: User Accounts",										new GoExecuteDelegate(UserAccounts), null, null));
			AddOption(new Options(category, "SystemProperties",					"", "",		"Control Panel: System Properties",									new GoExecuteDelegate(SystemProperties), null, null));
			AddOption(new Options(category, "AutomaticUpdates",					"", "",		"Control Panel: Automatic Updates",									new GoExecuteDelegate(AutomaticUpdates), null, null));

			category = "Directory shortcuts";
			AddOption(new Options(category, "LatestBuild",						"", "",		"Change to latest build directory",									new GoExecuteDelegate(LatestBuild), null, null));

			category = "Executables";
			AddOption(new Options(category, "IsRunning",						"", "",		"Checks if program is running",										new GoExecuteDelegate(IsRunning), new GoSyntaxDelegate(IsRunningSyntax), null));
			AddOption(new Options(category, "GetProcessID",						"", "",		"Gets process id for running program",								new GoExecuteDelegate(GetProcessID), new GoSyntaxDelegate(GetProcessIDSyntax), null));
			AddOption(new Options(category, "ShellApp",							"", "",		"Executes specified program with options",							new GoExecuteDelegate(ShellApp), new GoSyntaxDelegate(ShellAppSyntax), null));
			AddOption(new Options(category, "ShellCmd",							"", "",		"Executes specified command-line program and captures output",		new GoExecuteDelegate(ShellCmd), new GoSyntaxDelegate(ShellCmdSyntax), null));
			AddOption(new Options(category, "Mail",								"", "",		"Opens mail with options filled in",								new GoExecuteDelegate(Mail), new GoSyntaxDelegate(MailSyntax), null));
			AddOption(new Options(category, "Excel",							"", "",		"Opens file with MS-Excel",											new GoExecuteDelegate(Excel), new GoSyntaxDelegate(ExcelSyntax), null));
			AddOption(new Options(category, "PowerPoint",						"", "",		"Opens file with MS-Powerpoint",									new GoExecuteDelegate(PowerPoint), new GoSyntaxDelegate(PowerPointSyntax), null));
			AddOption(new Options(category, "Visio",							"", "",		"Opens file with MS-Visio",											new GoExecuteDelegate(Visio), new GoSyntaxDelegate(VisioSyntax), null));
			AddOption(new Options(category, "Word",								"", "",		"Opens file with MS-Word",											new GoExecuteDelegate(Word), new GoSyntaxDelegate(WordSyntax), null));
			AddOption(new Options(category, "Notepad",							"", "",		"Opens file with Notepad",											new GoExecuteDelegate(Notepad), new GoSyntaxDelegate(NotepadSyntax), null));
			AddOption(new Options(category, "Browser",							"", "",		"Opens file with browser",											new GoExecuteDelegate(Browser), new GoSyntaxDelegate(BrowserSyntax), null));
			AddOption(new Options(category, "ExplorerPath",						"", "",		"Opens windows explorer positioned at path",						new GoExecuteDelegate(ExplorerPath), new GoSyntaxDelegate(ExplorerPathSyntax), null));
			AddOption(new Options(category, "ExplorerFile",						"", "",		"Opens windows explorer positioned at file",						new GoExecuteDelegate(ExplorerFile), new GoSyntaxDelegate(ExplorerFileSyntax), null));
			AddOption(new Options(category, "ClearQuest",						"", "",		"Opens ClearQuest if it's not already running",						new GoExecuteDelegate(ClearQuest), null, null));
			AddOption(new Options(category, "WinZip",							"", "",		"Opens file with WinZip",											new GoExecuteDelegate(WinZip), new GoSyntaxDelegate(WinZipSyntax), null));

			category = "File shortcuts";
			AddOption(new Options(category, "CustomerList",						"", "",		"Build list of Customers from network",								new GoExecuteDelegate(CustomerList), new GoSyntaxDelegate(CustomerListSyntax), null));
			AddOption(new Options(category, "CustomerPage",						"", "",		"Display customer page for DebtorNr.",								new GoExecuteDelegate(CustomerPage), new GoSyntaxDelegate(CustomerPageSyntax), null));
			AddOption(new Options(category, "ReportPage",						"", "",		"Display report page",												new GoExecuteDelegate(ReportPage), null, null));
			AddOption(new Options(category, "SupportPage",						"", "",		"Display standard Support pages",									new GoExecuteDelegate(SupportPage), null, null));

			category = "Information";
			AddOption(new Options(category, "ClearQuestInfo",					"", "",		"Clear Quest information",											null, null, new GoInfoDelegate(ClearQuestInfo)));
			AddOption(new Options(category, "CrystalReports",					"", "",		"Crystal Reports registration information and HOWTO",				null, null, new GoInfoDelegate(CrystalReportsInfo)));
			AddOption(new Options(category, "DatabaseScripts",					"", "",		"Database scripts",													null, null, new GoInfoDelegate(DatabaseScriptsInfo)));
			AddOption(new Options(category, "DONE",								"", "",		"Hour's registration",												new GoExecuteDelegate(DONE), null, new GoInfoDelegate(DONEInfo)));
			AddOption(new Options(category, "DotNet",							"", "",		".Net / IIS Information",											null, null, new GoInfoDelegate(DotNetInfo)));
			AddOption(new Options(category, "ERASEBackOffice",					"", "",		"ERASE Back Office",												null, null, new GoInfoDelegate(ERASEBackOfficeInfo)));
			AddOption(new Options(category, "FTPInfo",							"", "",		"FTP Login Info",													null, null, new GoInfoDelegate(FTPInfo)));
			AddOption(new Options(category, "Intranet",							"", "",		"Intranet",															new GoExecuteDelegate(Intranet), null, new GoInfoDelegate(IntranetInfo)));
			AddOption(new Options(category, "Intranet2",						"", "",		"Intranet maintained by Niek",										new GoExecuteDelegate(Intranet2), null, new GoInfoDelegate(Intranet2Info)));
			AddOption(new Options(category, "MSDN",								"", "",		"MSDN information.",												null, null, new GoInfoDelegate(MSDNInfo)));
			AddOption(new Options(category, "SourceSafe",						"", "",		"SourceSafe databases",												null, null, new GoInfoDelegate(SourceSafeInfo)));
			AddOption(new Options(category, "Training",							"", "",		"Training",															new GoExecuteDelegate(Training), null, new GoInfoDelegate(TrainingInfo)));

			category = "Installation";
			AddOption(new Options(category, "InstallSP2",						"", "",		"Install ERASE 4.1.3 SP2",											new GoExecuteDelegate(InstallSP2), null, null));
			AddOption(new Options(category, "MakeBackOfficeLicenceFile",		"", "",		"Generate Back Office License File",								new GoExecuteDelegate(MakeBackOfficeLicenceFile), new GoSyntaxDelegate(MakeBackOfficeLicenceFileSyntax), null));

			category = "Servers";
			AddOption(new Options(category, "Server",							"", "",		"Open explorer to server and show any user info",					new GoExecuteDelegate(Server), new GoSyntaxDelegate(ServerSyntax), null));
			AddOption(new Options(category, "User",								"", "",		"Open explorer to server and show any user info",					new GoExecuteDelegate(Server), new GoSyntaxDelegate(ServerSyntax), null));
			AddOption(new Options(category, "ShowServerInfo",					"", "",		"Show credentials for a server",									new GoExecuteDelegate(ShowServerInfo), new GoSyntaxDelegate(ShowServerInfoSyntax), null));
			AddOption(new Options(category, "Ping",								"", "",		"Ping",																new GoExecuteDelegate(Ping), new GoSyntaxDelegate(PingSyntax), null));
			AddOption(new Options(category, "NetSend",							"", "",		"NetSend",															new GoExecuteDelegate(NetSend), new GoSyntaxDelegate(NetSendSyntax), null));

			category = "Assorted";
			AddOption(new Options(category, "Away",								"", "",		"Shows browser with default message 'Go Away'",						new GoExecuteDelegate(Away), new GoSyntaxDelegate(AwaySyntax), null));
			AddOption(new Options(category, "Calc",								"",	"",		"Calculates mathematical expression",								new GoExecuteDelegate(Calc), new GoSyntaxDelegate(CalcSyntax), null));
			AddOption(new Options(category, "FormatXML",						"", "",		"Formats XML file",													new GoExecuteDelegate(FormatXML), new GoSyntaxDelegate(FormatXMLSyntax), null));
			AddOption(new Options(category, "Touch",							"", "",		"Changes date of filespec to STAMP",								new GoExecuteDelegate(Touch), new GoSyntaxDelegate(TouchSyntax), null));
			AddOption(new Options(category, "Wsdl",								"", "",		"Invokes the Wsdl utility on the specified wsdlFile and generates csFile",		new GoExecuteDelegate(Wsdl), new GoSyntaxDelegate(WsdlSyntax), new GoInfoDelegate(WsdlInfo)));
			AddOption(new Options(category, "Unsign",							"", "",		"Unsigns file",														new GoExecuteDelegate(Unsign), new GoSyntaxDelegate(UnsignSyntax), null));
			AddOption(new Options(category, "CreateNetworkMappings",			"", "",		"Create network drive mappings",									new GoExecuteDelegate(CreateNetworkMappings), null, null));
			AddOption(new Options(category, "ReleaseNetworkMappings",			"", "",		"Releases network drive mappings",									new GoExecuteDelegate(ReleaseNetworkMappings), null, null));
			AddOption(new Options(category, "UpdateCRC",						"", "",		"Update erasedb.crc CRC file in current directory",					new GoExecuteDelegate(UpdateCRC), new GoSyntaxDelegate(UpdateCRCSyntax), null));

			category = "Stuff";
			AddOption(new Options(category, "GetDebtorName",					"",	"",		"Shows the company name associated with a debtor number",			new GoExecuteDelegate(GetDebtorName), new GoSyntaxDelegate(GetDebtorNameSyntax), null));
			AddOption(new Options(category, "Hotfix",							"",	"",		"Open the hotfix spreadsheet and show info",						new GoExecuteDelegate(Hotfix), null, new GoInfoDelegate(HotfixInfo)));
			AddOption(new Options(category, "Licenses",							"",	"",		"Show PM license information",										new GoExecuteDelegate(Licenses), null, null));
			AddOption(new Options(category, "ReadmeFilelist",					"",	"",		"Builds filelist from Deployment directory in current directory",	new GoExecuteDelegate(ReadmeFilelist), null, null));
			AddOption(new Options(category, "RequestDebtorNr",					"",	"",		"Request new Debtor number",										new GoExecuteDelegate(RequestDebtorNr), null, null));
			AddOption(new Options(category, "RequestLicense",					"",	"",		"Request License",													new GoExecuteDelegate(RequestLicense), new GoSyntaxDelegate(RequestLicenseSyntax), null));
			AddOption(new Options(category, "RequestOfficeSupplies",			"",	"",		"Request Office Supplies",											new GoExecuteDelegate(RequestOfficeSupplies), null, null));
			AddOption(new Options(category, "RequestRefNr",						"",	"",		"Request new Reference number",										new GoExecuteDelegate(RequestRefNr), null, null));
			AddOption(new Options(category, "ShowContracts",					"",	"",		"Show Contracts",													new GoExecuteDelegate(ShowContracts), null, null));
			AddOption(new Options(category, "ShowInvoices",						"",	"",		"Show Invoices",													new GoExecuteDelegate(ShowInvoices), null, null));
		}
		#endregion Initialization

		#region Administrative Tools
		public void ComponentServices(Options option) {
			Shell.ShellApp(GetDirectory("System32") + @"\Com\comexp.msc", "", false, false);
		}

		public void ComputerManagement(Options option) {
			Shell.ShellApp(GetDirectory("System32") + @"\compmgmt.msc", "/s", false, false);
		}

		public void DataSourcesODBC(Options option) {
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

		#region Directory Shortcuts
		public void LatestBuild(Options option) {
			string[] directories = Directory.GetDirectories(GetDirectory("SoftwareBuilds"));
			SortedList sl = new SortedList();
			foreach (string directory in directories)
				sl[directory] = true;

			directories = Directory.GetDirectories((string)sl.GetKey(sl.Count - 1));
			sl.Clear();
			foreach (string directory in directories)
				sl[directory] = true;

			Shell.ExplorerPath((string)sl.GetKey(sl.Count - 1), false, false);
		}
		#endregion

		#region File shortcuts
		private void NotifyImplementationOfDuplicateDebtorNr(string debtorNr) {
			Shell.MailTo("kbenhaddaoui", "dprischink", "", "Duplicate debtor number: " + debtorNr,
				string.Format(@"Beste Khalid,

There is a duplicate debtor number [{0}] in the directory [{1}].  Can you please chase this down and resolve this so there are unique numbers per customer?

Thanks,

Angela.", debtorNr, GetDirectory("Customers")));
		}

		public SortedList GetCustomerImplementationDirList(string fileMask) {
			string[] dirs = null;
			if (fileMask.Length == 0)
				dirs = Directory.GetDirectories(GetDirectory("Customers"));
			else
				dirs = Directory.GetDirectories(GetDirectory("Customers"), fileMask);

			Regex regexDebtorNrPrefix = new Regex("^[0-9][0-9][0-9][0-9][0-9][ -]");
			SortedList sl = new SortedList();
			foreach (string dir in dirs) {
				string subdir = dir.Substring(dir.LastIndexOf(@"\") + 1);
				if (regexDebtorNrPrefix.IsMatch(subdir)) {
					subdir = subdir.Replace('\xEB', 'e');
					if (sl.Contains(subdir.Substring(0, 5)))
						NotifyImplementationOfDuplicateDebtorNr(subdir.Substring(0, 5));
					else
						sl.Add(subdir.Substring(0, 5), subdir.Substring(6));
				}
			}
			return sl;
		}

		/// <summary>
		/// Finds the RFC directory in the local Src\Customizations directory tree.
		/// </summary>
		/// <param name="debtorNr">If provided, filters on the associated DebtorNr directory</param>
		/// <param name="rfcNr">Any number is fine - the function will format it to 4 digits</param>
		/// <returns>Returns fully qualified directory path or empty string if not found</returns>
		public string GetImplementationProjectDir(string debtorNr, string project) {
			if (debtorNr.Length == 0)
				throw new Exception("No debtor number provided");
			if (project.Length == 0)
				throw new Exception("No project provided");

			SortedList projectDirs = GetImplementationProjectDirs(debtorNr, project);
			if (projectDirs.Count == 0)
				return "";
			else
				return (string)projectDirs.GetKey(projectDirs.Count - 1);
		}

		public SortedList GetImplementationProjectDirs(string debtorNr, string projectFilter) {
			if (debtorNr.Length == 0)
				throw new Exception("No debtor number provided");

			SortedList projectDirs = new SortedList();
			string[] dirs = Directory.GetDirectories(GetDirectory("Customers"), debtorNr + "*");
			foreach (string dir in dirs) {
				string[] subdirs = Directory.GetDirectories(dir, projectFilter + "*");
				foreach (string subdir in subdirs) {
					projectDirs.Add(subdir, subdir);
				}
			}

			return projectDirs;
		}

		static string	CustomerListSyntax(bool syntaxOnly, bool switchesOnly) {
			string syntax = "[-Q] [-C]";
			if (switchesOnly)
				return syntax;
			else
				return syntax + @"
-Q : Quiet
-C : to show only if changed";
		}
		public void		CustomerList(Options option) {
			SortedList argOptions = new SortedList();
			argOptions["-Q"] = false;	// {-Q : Quiet}
			argOptions["-C"] = false;	// {-C : to show only if changed}
			CommandLine.EvalCommandLine(ref argOptions);
			bool	quietMode		= (bool)argOptions["-Q"];
			bool	showChangedMode	= (bool)argOptions["-C"];

			CustomerList(quietMode, showChangedMode, "");
		}

		public SortedList CustomerList(bool quietMode, bool showChangedMode, string compareListTxtFilename) {

			TextFileContents customerListTxt	= new TextFileContents(
				GetDirectory("StatusOverview") + @"\Archive\CustomerList.txt");
			TextFileContents compareListTxt		= new TextFileContents(
				GetDirectory("StatusOverview") + @"\Archive\" + 
				(compareListTxtFilename.Length == 0 ? "CustomerList.txt" : compareListTxtFilename));
			TextFileContents customerListXml	= new TextFileContents(
				GetDirectory("Customers") + @"\CustomerList.xml");

			// Read the special list from disk so we can compare contents later.
			compareListTxt.Read();

			// Generate txt and xml customer list files in strings.
			string txtList = "";
			string xmlList = "<Customers>\r\n";
			SortedList customerImpDirList = GetCustomerImplementationDirList("");
			IDictionaryEnumerator customersEnumerator = customerImpDirList.GetEnumerator();
			while (customersEnumerator.MoveNext()) {
				txtList += (string)customersEnumerator.Key + "\t" + (string)customersEnumerator.Value + "\r\n";
				xmlList += "\t<Customer>\r\n";
				xmlList += "\t\t<DebtorNr>" + (string)customersEnumerator.Key + "</DebtorNr>\r\n";
				xmlList += "\t\t<Name>" + (string)customersEnumerator.Value + "</Name>\r\n";
				xmlList += "\t</Customer>\r\n";
			}
			xmlList += "</Customers>\r\n";

			// Write strings to files.
			customerListTxt.Contents = txtList;
			customerListXml.Contents = xmlList;

			// Replace the compareListTxt with the new file (but don't update the Contents).
			customerListTxt.Copy(compareListTxt.Filename, true);

			if (!quietMode) {
				// Compare last version with new version.
				if (!showChangedMode || (showChangedMode && !customerListTxt.Compare(compareListTxt)))
					Shell.Notepad(compareListTxt.Filename, false, false);
			}

			return customerImpDirList;
		}

		static string	CustomerPageSyntax(bool syntaxOnly, bool switchesOnly) { return "-D {DebtorNr}"; }
		public void		CustomerPage(Options option) {
			SortedList argOptions = new SortedList();
			argOptions["-D"] = "";	// {-D DebtorNr}
			CommandLine.EvalCommandLine(ref argOptions);
			string debtorNr	= (string)argOptions["-D"];	// {DebtorNr}

			CustomerPage(debtorNr);
		}

		public void CustomerPage(string debtorNr) {
			if (debtorNr.Length == 0)
				ReportPage();
			else {
				string[] dirs = Directory.GetDirectories(GetDirectory("Customer_Reporting"));
				if (dirs.Length == 0)
					MessageBox.Show(string.Format("No page for debtor number [{0}]", debtorNr));
				else {
					string dir = GetDirectory("Customer_Reporting") + @"\" + dirs[dirs.Length - 1];
					string[] files = Directory.GetFiles(dir, debtorNr + "*.htm");
					if (files.Length == 0)
						MessageBox.Show(string.Format("No page for debtor number [{0}]", debtorNr));
					else
						Shell.Browser(dir + @"\" + files[files.Length - 1], false, false);
				}
			}
		}

		public void ReportPage(Options option) {
			ReportPage();
		}

		private void ReportPage() {
			Shell.Browser(@"N:\Reporting\Reporting start page.htm", false, false);
		}

		public void SupportPage(Options option) {
			Shell.Browser(@"N:\Dep_Support\Dashboard\Customers.htm", false, false);
		}
		#endregion File shortcuts

		#region Executables
		public string IsRunningSyntax(bool syntaxOnly, bool switchesOnly) {
			return "-F {filenameWithoutExtension}";
		}
		public void IsRunning(Options option) {
			SortedList argOptions = new SortedList();
			argOptions["-F"]		= "";		// {-F filenameWithoutExtension}
			CommandLine.EvalCommandLine(ref argOptions);
			string	filenameWithoutExtension	= (string)argOptions["-F"];

			if (filenameWithoutExtension.Length == 0)
				throw new Exception("Error: -F {filenameWithoutExtension} option missing");

			bool ret = Shell.IsRunning(filenameWithoutExtension);
			MessageBox.Show(filenameWithoutExtension + " is " + (ret ? "" : "not ") + "running", "IsRunning");
		}

		public string GetProcessIDSyntax(bool syntaxOnly, bool switchesOnly) {
			return "-F {filenameWithoutExtension}";
		}
		public void GetProcessID(Options option) {
			SortedList argOptions = new SortedList();
			argOptions["-F"]		= "";		// {-F filenameWithoutExtension}
			CommandLine.EvalCommandLine(ref argOptions);
			string	filenameWithoutExtension	= (string)argOptions["-F"];

			if (filenameWithoutExtension.Length == 0)
				throw new Exception("Error: -F {filenameWithoutExtension} option missing");

			int ret = Shell.GetProcessID(filenameWithoutExtension);
			MessageBox.Show(filenameWithoutExtension + " has been identified with process " + ret, "GetProcessID");
		}

		public string ShellAppSyntax(bool syntaxOnly, bool switchesOnly) {
			string ret = "-E {executable} [-A {arguments} [-F]] [-W]";
			if (!switchesOnly)
				ret += @"
-F argument indicates -A {arguments} is a filename
-W arument indicates Go should wait until executable exits";

			return ret;
		}
		public void ShellApp(Options option) {
			SortedList argOptions = new SortedList();
			argOptions["-E"] = "";		// {-E executable}
			argOptions["-A"] = "";		// {-A arguments}
			argOptions["-F"] = false;	// {-F}
			argOptions["-W"] = false;	// {-W}
			CommandLine.EvalCommandLine(ref argOptions);
			string	executable			= (string)argOptions["-E"];
			string	arguments			= (string)argOptions["-A"];
			bool	argumentIsFilename	= (bool)argOptions["-F"];
			bool	wait				= (bool)argOptions["-W"];

			if (executable.Length == 0)
				throw new Exception("Error: -E {executable} option missing");
			if (arguments.Length == 0 && argumentIsFilename)
				throw new Exception("Error: -F option specified without -A {arguments} option");

			Shell.ShellApp(executable, arguments, argumentIsFilename, wait, ProcessWindowStyle.Normal);
		}

		public string ShellCmdSyntax(bool syntaxOnly, bool switchesOnly) {
			string ret = "-E {executable} [-A {arguments} [-F]]";
			if (!switchesOnly)
				ret += @"
-F parameter indicates -A {arguments} is a filename";

			return ret;
		}
		public void ShellCmd(Options option) {
			SortedList argOptions = new SortedList();
			argOptions["-E"] = "";		// {-E executable}
			argOptions["-A"] = "";		// {-A arguments}
			argOptions["-F"] = false;	// {-F}
			CommandLine.EvalCommandLine(ref argOptions);
			string	executable			= (string)argOptions["-E"];
			string	arguments			= (string)argOptions["-A"];
			bool	argumentIsFilename	= (bool)argOptions["-F"];

			if (executable.Length == 0)
				throw new Exception("Error: -E {executable} option missing");
			if (arguments.Length == 0 && argumentIsFilename)
				throw new Exception("Error: -F option specified without -A {arguments} option");

			string output;
			Shell.ShellCmd(executable, arguments, argumentIsFilename, out output);
			MessageBox.Show(output, "Output from " + executable + " " + arguments);
		}

		public string MailSyntax(bool syntaxOnly, bool switchesOnly) {
			return "-To {alias list} [-Cc {alias list}] [-Bcc {alias list}] [-Subject {subject text}] [-Body {body text}]";
		}
		public void Mail(Options option) {
			SortedList argOptions = new SortedList();
			argOptions["-To"]		= "";		// {-To to}
			argOptions["-Cc"]		= "";		// {-Cc cc}
			argOptions["-Bcc"]		= "";		// {-Bcc bcc}
			argOptions["-Subject"]	= "";		// {-Subject subject}
			argOptions["-Body"]		= "";		// {-Body body}
			CommandLine.EvalCommandLine(ref argOptions);
			string	to		= (string)argOptions["-To"];
			string	cc		= (string)argOptions["-Cc"];
			string	bcc		= (string)argOptions["-Bcc"];
			string	subject	= (string)argOptions["-Subject"];
			string	body	= (string)argOptions["-Body"];

			if (to.Length == 0)
				throw new Exception("Error: -To {alias list} option missing");

			Shell.MailTo(to, cc, bcc, subject, body);
		}

		public string ExcelSyntax(bool syntaxOnly, bool switchesOnly) { return "[-F {Filename}]"; }
		public void Excel(Options option) {
			SortedList argOptions = new SortedList();
			argOptions["-F"] = "";		// {-F filename}
			CommandLine.EvalCommandLine(ref argOptions);
			string	filename = (string)argOptions["-F"];

			Shell.Excel(filename, false, false);
		}

		public string PowerPointSyntax(bool syntaxOnly, bool switchesOnly) { return "[-F {Filename}]"; }
		public void PowerPoint(Options option) {
			SortedList argOptions = new SortedList();
			argOptions["-F"] = "";		// {-F filename}
			CommandLine.EvalCommandLine(ref argOptions);
			string	filename = (string)argOptions["-F"];

			Shell.PowerPoint(filename, false, false);
		}

		public string VisioSyntax(bool syntaxOnly, bool switchesOnly) { return "[-F {Filename}]"; }
		public void Visio(Options option) {
			SortedList argOptions = new SortedList();
			argOptions["-F"] = "";		// {-F filename}
			CommandLine.EvalCommandLine(ref argOptions);
			string	filename = (string)argOptions["-F"];

			Shell.Visio(filename, false, false);
		}

		public string WordSyntax(bool syntaxOnly, bool switchesOnly) { return "[-F {Filename}]"; }
		public void Word(Options option) {
			SortedList argOptions = new SortedList();
			argOptions["-F"] = "";		// {-F filename}
			CommandLine.EvalCommandLine(ref argOptions);
			string	filename = (string)argOptions["-F"];

			Shell.Word(filename, false, false);
		}

		public string NotepadSyntax(bool syntaxOnly, bool switchesOnly) { return "[-F {Filename}]"; }
		public void Notepad(Options option) {
			SortedList argOptions = new SortedList();
			argOptions["-F"] = "";		// {-F filename}
			CommandLine.EvalCommandLine(ref argOptions);
			string	filename = (string)argOptions["-F"];

			Shell.Notepad(filename, false, false);
		}

		public string BrowserSyntax(bool syntaxOnly, bool switchesOnly) { return "[-F {Filename}]"; }
		public void Browser(Options option) {
			SortedList argOptions = new SortedList();
			argOptions["-F"] = "";		// {-F filename}
			CommandLine.EvalCommandLine(ref argOptions);
			string	filename = (string)argOptions["-F"];

			Shell.Browser(filename, false, false);
		}

		public string ExplorerPathSyntax(bool syntaxOnly, bool switchesOnly) { return "-P {Path}"; }
		public void ExplorerPath(Options option) {
			SortedList argOptions = new SortedList();
			argOptions["-P"] = "";		// {-P path}
			CommandLine.EvalCommandLine(ref argOptions);
			string	path = (string)argOptions["-P"];

			if (path.Length == 0)
				throw new Exception("Error: -P {path} option missing");
			
			Shell.ExplorerPath(path, false, false);
		}

		public string ExplorerFileSyntax(bool syntaxOnly, bool switchesOnly) { return "-F {Filespec}"; }
		public void ExplorerFile(Options option) {
			SortedList argOptions = new SortedList();
			argOptions["-F"] = "";		// {-F filespec}
			CommandLine.EvalCommandLine(ref argOptions);
			string	filespec = (string)argOptions["-F"];

			if (filespec.Length == 0)
				throw new Exception("Error: -F {filespec} option missing");

			Shell.ExplorerFile(filespec, false, false);
		}

		public void ClearQuest(Options option)
		{
			ClearQuest();
		}

		public Int32 ClearQuest()
		{
			if (!Shell.IsRunning("clearquest"))
				return Shell.ShellApp(GetDirectory("ClearQuest") + @"\clearquest.exe", "", false, false, ProcessWindowStyle.Normal);
			else
				return 0;
		}

		public string WinZipSyntax(bool syntaxOnly, bool switchesOnly) { return "[-F {filename}]"; }
		public void WinZip(Options option) {
			SortedList argOptions = new SortedList();
			argOptions["-F"] = "";		// {-F filename}
			CommandLine.EvalCommandLine(ref argOptions);
			string	filename = (string)argOptions["-F"];

			WinZip(filename);
		}

		public Int32 WinZip(string filename)
		{
			return Shell.ShellApp(GetDirectory("WinZipDir") + @"\WINZIP32.EXE", filename, true, true, ProcessWindowStyle.Normal);
		}
		#endregion Executables

		#region Information
		static string ClearQuestInfo() {
			return @"On homepage on left.
Username: rrozsa
Password:
Access rights maintained by Michael Boer.
Direct access: http://net.intranet.nl/docs/net/ClearQuest/ClearQuest_StartPage.htm
               http://netsrv01/cqweb/logon/default.asp";
			// Browser(@"http://net.intranet.nl/docs/net/ClearQuest/ClearQuest_StartPage.htm");
		}

		static string CrystalReportsInfo() {
			return @"Registration number: 6574801861
Product Code:        AAP50-GS00008-NFA00AP

HOWTO:
- Don't bind database in report with NT Authentication - Use SQL Authentication
- Use ERASE default database names.

If report comes up in an uneditable state, try the following:
- Ensure report file is not read-only.
- Ensure report is located in C# Project directory.
- Create new C# project and add Crystal Report.
   - If wizard starts, you should now be able (in this DevStudio session) to work with your report.
   - If empty report is created without the wizard, repeat adding reports or possibly restarting
     DevStudio, creating project and creating report. This may be required even 8 times until
     environment cooperates.

Best Practices:
- Create empty Solution called Reports in C:\Inetpub\wwwroot\AML\Reports
- Temporarily move the C:\Inetpub\wwwroot\AML\Reports\AIM directory to .\AIM2
- Create new C# project in C:\Inetpub\wwwroot\AML\Reports\AIM
- Move the contents of AIM2 back to the new AIM directory and delete AIM2
- Add all the reports to the AIM project
- Configure the SourceSafe Working Directory for the AIM project to C:\Inetpub\wwwroot\AML\Reports\AIM
- Configure the SourceSafe Working Directory for any reports in RFC's to C:\Inetpub\wwwroot\AML\Reports\AIM";
		}

		static string DatabaseScriptsInfo() {
			return @"Database scripts can be found in SourceSafe.
Inspect the history to discover which scripts apply to a particular version or hotfix.
Here is a list of locations for the various databases:
AIMEEDB		$/.../Erase Enterprise AIM/Implementation/Database/Builds/Scripts";
		}

		static string DONEInfo() {
			return @"From Intranet / Navigation / click on Hour registration

To view previously recorded items, click on View tasks
    User:                   Your network login
    Aantal dagen geleden:   How many days back
    Aantal dagen zien:      For how many days

To record new items, click on Submit tasks

Travel expenses: Woon-Werk Verkeer

Weekly time sheet (to be turned in to Claudia):
    Query / Printable Time and Billing
    Attach any receipts.";
		}

		public void DONE(Options option) {
			Shell.Browser(@"https://done.neteconomy.com/", false, false);
		}

		static string DotNetInfo() {
			return @"To reset IIS: iisreset";
		}

		static string ERASEBackOfficeInfo() {
			return @"ERASE Back Office Admin.
User:     pu
Password: alive";
		}

		static string FTPInfo() {
			return @"ftp.neteconomy.com/support or ftp.neteconomy.com/supportupload
Login:    edrgroup/ftpnetsupport
Password: Logmein2491am";
		}

		static string	IntranetInfo() { return @"Main intranet site: http://net.intranet.nl/"; }
		public void		Intranet(Options option) {
			Shell.Browser(@"http://net.intranet.nl/", false, false);
		}

		static string	Intranet2Info() { return @"Second intranet site maintained by Niek: http://intra.edrgroup.net"; }
		public void		Intranet2(Options option) {
			Shell.Browser(@"http://intra.edrgroup.net", false, false);
		}

		static string MSDNInfo() {
			return 
				@"support.microsoft.com
MSDN Subscriber ID: 200761708
Access ID: ?
(020) 500-1500
(0800) 022-7261";
		}

		static string SourceSafeInfo() {
			return 
				@"Richard Hoogenboom or ICT can grant rights

Username           Your network username
Password           Uses NT authentication

Main database	\\edrgfs01\Neteconomy\Dep_Development\SourceControl\srcsafe.ini
Customers		\\edrgfs01\Neteconomy\Dep_Implementation\Customer Configurations\srcsafe.ini
Backoffice v3/v4   \\netsrv01\ERASE Project\SSDB\srcsafe.ini
ERASE Project	\\NETSRV01: $/ERASE/ERASE2001";
		}

		static string TrainingInfo() {
			return @"Alerts with prefix T26 are for me to play with:
Currently configured with Dutch legislation pack so can only report with Dutch MOT report.

Access rights maintained by: Fritz van der Blij
User Name: trainee26
Password:  password
URL:       http://pmdemo/training/
Training manual:
ERASE PowerPoint presentation:
           N:\Dep_Development\Training and Documentation\Training\ERASE v4\Compliance Manager 413 User Training.ppt";
		}
		public void	Training(Options option) {
			Shell.ExplorerFile(@"N:\Dep_Development\Training and Documentation\Training\ERASE v4\ERASE Compliance Manager 413 User Training.pdf", false, false);
			Shell.ExplorerFile(@"N:\Dep_Development\Training and Documentation\Training\ERASE v4\Compliance Manager 413 User Training.ppt", false, false);
			Shell.Browser(@"http://pmdemo/training/", false, false);
		}
		#endregion Information

		#region Installation
		public void		InstallSP2(Options option) {
			// Search for registry key indicating Back Office is installed.
			RegistryRoots regRoots = new RegistryRoots();
			string serverName = regRoots.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\NetEconomy\ERASE\ConfigServer");
			
			// Search for registry key indicating Front Office is installed.
			string rootDirectory = regRoots.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\NetEconomy\Framework\4.0\RootDirectory");

			// Search for registry key indicating Front Office is installed.
			string eraseSchedulerPath = regRoots.GetValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\ERASE Scheduler\ImagePath");

			string msg = "";
			bool	newInstall = true;
			if (serverName.Length > 0) {
				newInstall = false;
				msg += "ERASE Back Office is already installed.\r\n";
			}
			if (rootDirectory.Length > 0 || eraseSchedulerPath.Length > 0) {
				newInstall = false;
				msg += "ERASE Front Office is already installed.\r\n";
			}

			if (!newInstall) {
				if (DialogResult.Yes != MessageBox.Show(msg + "New installation?", "Go InstallSP2", MessageBoxButtons.YesNoCancel))
					throw new Exception("Error: Existing installation not supported yet");
			}

			// Validate pre-requisits are installed.
			msg = "";
			if (!IsSQLInstalled())
				msg += "You must install SQL Server first.\r\n";
			if (!IsIISInstalled())
				msg += "You must install IIS first.\r\n";
			if (!IsOfficeInstalled())
				msg += "You must install MS-Office first.\r\n";
			if (!IsAdobeAcrobatInstalled())
				msg += "You must install Adobe Acrobat Reader first.\r\n";

			if (msg.Length > 0)
				throw new Exception("Error: Software needs to be installed before ERASE can be installed.\r\n\r\n"
					+ msg);

			MessageBox.Show("Remove any old databases before starting installation", "Go InstallSP2");

			// Install ERASE Back Office
			Shell.ShellApp(GetDirectory("Release413SR2") + @"\ERASE Back Office\Setup.exe", "", false, false);

			// DBExec("AccoutDB script.sql");
			// DBExec("MainDB script.sql")
			// DBExec("MainDB CustomNodes script.sql");
			// DBExec("ImportDB script.sql");
			// DBExec("AggregationDB script.sql");
			// DBExec("AIMEEDB Base.sql");
			// DBExec("AIMEEDB Country Specifics.sql");
			// DBExec("Finance Alert Base.sql");
			// DBExec("Finance Alert Parameters.sql");

		}

		static string	MakeBackOfficeLicenceFileSyntax(bool syntaxOnly, bool switchesOnly) {
			return "-D {Dst File} -U {User Name} -M {Machine Name} -P {Database Path} -E1 {Environment 1} -P1 {Database Prefix 1} [-E2...9 {Environment 2...9} -P2...9 {Database Prefix 2...9}]";
		}
		public void MakeBackOfficeLicenceFile(Options option) {
			SortedList argOptions = new SortedList();
			argOptions["-D"] = "";		// -D {Dst File}
			argOptions["-U"] = "";		// -U {User Name}
			argOptions["-M"] = "";		// -M {Machine Name}
			argOptions["-P"] = "";		// -P {Database Path}
			argOptions["-E1"]= "";		// -E1 {Environment 1}
			argOptions["-P1"]= "";		// -P1 {Database Prefix 1}
			argOptions["-E2"]= "";		// -E2 {Environment 2}
			argOptions["-P2"]= "";		// -P2 {Database Prefix 2}
			argOptions["-E3"]= "";		// -E3 {Environment 3}
			argOptions["-P3"]= "";		// -P3 {Database Prefix 3}
			argOptions["-E4"]= "";		// -E4 {Environment 4}
			argOptions["-P4"]= "";		// -P4 {Database Prefix 4}
			argOptions["-E5"]= "";		// -E5 {Environment 5}
			argOptions["-P5"]= "";		// -P5 {Database Prefix 5}
			argOptions["-E6"]= "";		// -E6 {Environment 6}
			argOptions["-P6"]= "";		// -P6 {Database Prefix 6}
			argOptions["-E7"]= "";		// -E7 {Environment 7}
			argOptions["-P7"]= "";		// -P7 {Database Prefix 7}
			argOptions["-E8"]= "";		// -E8 {Environment 8}
			argOptions["-P8"]= "";		// -P8 {Database Prefix 8}
			argOptions["-E9"]= "";		// -E9 {Environment 9}
			argOptions["-P9"]= "";		// -P9 {Database Prefix 9}
			CommandLine.EvalCommandLine(ref argOptions);
			string	dstFile		= (string)argOptions["-D"];
			string	user		= (string)argOptions["-U"];
			string	machineName	= (string)argOptions["-M"];
			string	databasePath= (string)argOptions["-P"];
			string	environment1= (string)argOptions["-E1"];
			string	dbPrefix1	= (string)argOptions["-P1"];
			string	environment2= (string)argOptions["-E2"];
			string	dbPrefix2	= (string)argOptions["-P2"];
			string	environment3= (string)argOptions["-E3"];
			string	dbPrefix3	= (string)argOptions["-P3"];
			string	environment4= (string)argOptions["-E4"];
			string	dbPrefix4	= (string)argOptions["-P4"];
			string	environment5= (string)argOptions["-E5"];
			string	dbPrefix5	= (string)argOptions["-P5"];
			string	environment6= (string)argOptions["-E6"];
			string	dbPrefix6	= (string)argOptions["-P6"];
			string	environment7= (string)argOptions["-E7"];
			string	dbPrefix7	= (string)argOptions["-P7"];
			string	environment8= (string)argOptions["-E8"];
			string	dbPrefix8	= (string)argOptions["-P8"];
			string	environment9= (string)argOptions["-E9"];
			string	dbPrefix9	= (string)argOptions["-P9"];

			/*
			dstFile = @"C:\Test\Licenses\t.xml";
			user = "Richard Rozsa";
			machineName = "LAP0066";
			databasePath = @"C:\ERASE Data";
			environment1 = "Standard Test Env for 4.1.3 SR5";
			dbPrefix1 = "v413sr5_";
			environment2 = "Abbey Test Env";
			dbPrefix2 = "Abbey_";
			*/

			DateTimeComponents dtc = new DateTimeComponents();
			DateTimeComponents nextYear = new DateTimeComponents(DateTime.Now.AddYears(5));

			string contents = String.Format(@"<License>
<!-- License requested by {0} on {1} -->
<!-- License valid from {2} until {3} version independant -->
<!-- License for testing purposes                                    -->
	<Environments>
		<ConfigDBStore id=""ConfigDB"" dbname=""ConfigDB"" domain="""" machine=""{4}"">
			<DBData id=""Database"" datapath=""{5}"" size=""1""/>
			<DBData id=""Log"" datapath=""{5}"" size=""1""/>
		</ConfigDBStore>
", 				user, 
				dtc.FormattedDateTimeDay, 
				dtc.FormattedDate, 
				nextYear.FormattedDate, 
				machineName,
				databasePath);

			string key = null;
			string hash1 = null;
			string hash2 = null;
			for (int env = 1; env <= 9; env++) {
				key = "uB!1EEFoyKiGLqFsBvjHM5xOAN";
				hash1 = "10202030303300330303033123455";
				hash2 = "22333344445555567678878888888";

				string environment = null;
				string dbPrefix = null;
				switch (env) {
				case 1: 
					environment = environment1;
					dbPrefix = dbPrefix1;
					break;
				case 2:
					environment = environment2;
					dbPrefix = dbPrefix2;
					break;
				case 3:
					environment = environment3;
					dbPrefix = dbPrefix3;
					break;
				case 4:
					environment = environment4;
					dbPrefix = dbPrefix4;
					break;
				case 5:
					environment = environment5;
					dbPrefix = dbPrefix5;
					break;
				case 6:
					environment = environment6;
					dbPrefix = dbPrefix6;
					break;
				case 7:
					environment = environment7;
					dbPrefix = dbPrefix7;
					break;
				case 8:
					environment = environment8;
					dbPrefix = dbPrefix8;
					break;
				case 9:
					environment = environment9;
					dbPrefix = dbPrefix9;
					break;
				}

				if (environment.Length > 0) {
					contents += String.Format(
						@"		<Environment id=""{0}"" name=""{6}"" key=""{1}"">
			<ClientSoftwareHashes>
				<HashValue value=""{2}""/>
				<HashValue value=""{3}""/>
			</ClientSoftwareHashes>
			<DBStore id=""MainDB"" dbname=""{7}MainDB"" domain="""" machine=""{4}"">
				<DBData id=""Database"" datapath=""{5}"" size=""1""/>
				<DBData id=""Log"" datapath=""{5}"" size=""1""/>
			</DBStore>
			<DBStore id=""AccountDB"" dbname=""{7}AccountDB"" domain="""" machine=""{4}"">
				<DBData id=""Database"" datapath=""{5}"" size=""1""/>
				<DBData id=""Log"" datapath=""{5}"" size=""1""/>
			</DBStore>
			<DBStore id=""ImportDB"" dbname=""{7}ImportDB"" domain="""" machine=""{4}"">
				<DBData id=""Database"" datapath=""{5}"" size=""1""/>
				<DBData id=""Log"" datapath=""{5}"" size=""1""/>
			</DBStore>
			<DBStore id=""TransactDB"" dbname=""{7}TransactDB"" domain="""" machine=""{4}"">
				<DBData id=""Database"" datapath=""{5}"" size=""1""/>
				<DBData id=""Log"" datapath=""{5}"" size=""1""/>
				<DBData id=""FilegroupM02"" datapath=""{5}"" size=""1""/>
				<DBData id=""FilegroupM03"" datapath=""{5}"" size=""1""/>
				<DBData id=""FilegroupM04"" datapath=""{5}"" size=""1""/>
				<DBData id=""FilegroupM05"" datapath=""{5}"" size=""1""/>
				<DBData id=""FilegroupM06"" datapath=""{5}"" size=""1""/>
				<DBData id=""FilegroupM07"" datapath=""{5}"" size=""1""/>
				<DBData id=""FilegroupM08"" datapath=""{5}"" size=""1""/>
				<DBData id=""FilegroupM09"" datapath=""{5}"" size=""1""/>
				<DBData id=""FilegroupM10"" datapath=""{5}"" size=""1""/>
				<DBData id=""FilegroupM11"" datapath=""{5}"" size=""1""/>
				<DBData id=""FilegroupM12"" datapath=""{5}"" size=""1""/>
				<DBData id=""FilegroupM13"" datapath=""{5}"" size=""1""/>
				<DBData id=""FilegroupM14"" datapath=""{5}"" size=""1""/>
				<DBData id=""FilegroupM15"" datapath=""{5}"" size=""1""/>
			</DBStore>
			<DBStore id=""MiscDB"" dbname=""{7}MiscDB"" domain="""" machine=""{4}"">
				<DBData id=""Database"" datapath=""{5}"" size=""1""/>
				<DBData id=""Log"" datapath=""{5}"" size=""1""/>
			</DBStore>
			<DBStore id=""IndexDB"" dbname=""{7}IndexDB"" domain="""" machine=""{4}"">
				<DBData id=""Database"" datapath=""{5}"" size=""1""/>
				<DBData id=""Log"" datapath=""{5}"" size=""1""/>
			</DBStore>
			<DBStore id=""DestDB"" dbname=""{7}DestDB"" domain="""" machine=""{4}"">
				<DBData id=""Database"" datapath=""{5}"" size=""1""/>
				<DBData id=""Log"" datapath=""{5}"" size=""1""/>
			</DBStore>
			<DBStore id=""AggregationDB"" dbname=""{7}AggregationDB"" domain="""" machine=""{4}"">
				<DBData id=""Database"" datapath=""{5}"" size=""1""/>
				<DBData id=""Log"" datapath=""{5}"" size=""1""/>
			</DBStore>
			<DBStore id=""MatchingDB"" dbname=""{7}MatchingDB"" domain="""" machine=""{4}"">
				<DBData id=""Database"" datapath=""{5}"" size=""1""/>
				<DBData id=""Log"" datapath=""{5}"" size=""1""/>
			</DBStore>
			<DBStore id=""CustomDB"" dbname=""{7}CustomDB"" domain="""" machine=""{4}"">
				<DBData id=""Database"" datapath=""{5}"" size=""1""/>
				<DBData id=""Log"" datapath=""{5}"" size=""1""/>
			</DBStore>
		</Environment>
",						env,
						key, 				
						hash1, 
						hash2, 
						machineName, 
						databasePath, 
						environment, 
						dbPrefix);
				}
			}

			contents += 
@"	</Environments>
</License>";
			
			TextFileContents tfc = new TextFileContents(dstFile);
			tfc.Contents = contents;
			tfc = null;
		}

		public bool IsSQLInstalled() {
			RegistryRoots regRoots = new RegistryRoots();
			return (regRoots.GetValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\MSSQLSERVER\ImagePath").Length > 0);
		}

		public bool IsIISInstalled() {
			RegistryRoots regRoots = new RegistryRoots();
			return (regRoots.GetValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\IISADMIN\ImagePath").Length > 0);
		}

		public bool IsOfficeInstalled() {
			return (GetDirectory("MSOffice").Length > 0);
		}

		public bool IsAdobeAcrobatInstalled() {
			RegistryRoots regRoots = new RegistryRoots();
			return (regRoots.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Adobe\Adobe Acrobat\6.0\InstallPath").Length > 0);
		}
		#endregion Installation

		#region Servers/Users
		static string	ServerSyntax(bool syntaxOnly, bool switchesOnly) {
			string syntax = "[-H] [-Q] [-U {user}] [-S {server}] [-M {msg}]";
			if (switchesOnly)
				return syntax;
			else
				return syntax + @"
-H : list of servers
-Q : quiet mode (no machine ping)
-U : by user alias or name
-S : by server
-M : net send message";
		}
		public void		Server(Options option) {
			SortedList argOptions = new SortedList();
			argOptions["-H"] = false;	// {-h : list of servers
			argOptions["-Q"] = false;	// {-q : quiet mode (no machine ping)
			argOptions["-U"] = "";		// {-u : by user alias or name}
			argOptions["-S"] = "";		// {-s : by server}
			argOptions["-M"] = "";		// {-m : message}
			CommandLine.EvalCommandLine(ref argOptions);
			bool	help		= (bool)argOptions["-H"];
			bool	quiet		= (bool)argOptions["-Q"];
			string	alias		= (string)argOptions["-U"];
			string	server		= (string)argOptions["-S"];
			string	msg			= (string)argOptions["-M"];

			bool isUser = (option.alias1.ToUpper() == "User".ToUpper());

			int id = -1;
			int serverID = -1;
			int userID = -1;

			if (help)
				ServerHelp(isUser, quiet);
			else {
				if (server.Length > 0) {
					ServerList sl = ServerList.GetServerList();
					serverID = id = sl.GetID(server);
					if (id >= 0)
						ShowUserInfo(quiet, true, sl.GetServer(id), "", "", sl.GetUncPath(id), sl.GetUsername(id), sl.GetPassword(id), "", "", "", "", "", "", msg);
				}

				if (alias.Length > 0) {
					UserList ul = UserList.GetUserList();
					userID = ul.GetID(alias);
					if (userID >= 0)
						ShowUserInfo(quiet, true, ul.GetAlias(id), ul.GetFirstname(id), ul.GetLastname(id), ul.GetMachineName(id), "", "", ul.GetFunction(id), ul.GetInternalNr(id), ul.GetPrivateNr(id), ul.GetMobileNr(id), ul.GetEMail(id), ul.GetDepartmentList(id), msg);

					SortedList firstnames = ul.GetFirstNames(alias);
					if (firstnames.Count > 0) {
						for (int i = 0; i < firstnames.Count; i++) {
							alias = (string)firstnames.GetKey(i);
							userID = id = ul.GetID(alias);
							ShowUserInfo(quiet, true, ul.GetAlias(id), ul.GetFirstname(id), ul.GetLastname(id), ul.GetMachineName(id), "", "", ul.GetFunction(id), ul.GetInternalNr(id), ul.GetPrivateNr(id), ul.GetMobileNr(id), ul.GetEMail(id), ul.GetDepartmentList(id), msg);
						}
					}
					SortedList lastnames = ul.GetLastNames(alias);
					if (lastnames.Count > 0) {
						for (int i = 0; i < lastnames.Count; i++) {
							alias = (string)lastnames.GetKey(i);
							userID = id = ul.GetID(alias);
							ShowUserInfo(quiet, true, ul.GetAlias(id), ul.GetFirstname(id), ul.GetLastname(id), ul.GetMachineName(id), "", "", ul.GetFunction(id), ul.GetInternalNr(id), ul.GetPrivateNr(id), ul.GetMobileNr(id), ul.GetEMail(id), ul.GetDepartmentList(id), msg);
						}
					}
				}

				if (userID == -1 && serverID == -1) {
					if (server.Length > 0)
						ShowUserInfo(quiet, true, alias, "", "", server, "", "", "", "", "", "", "", "", msg);
					else
						throw new Exception("No or unknown -U alias");
				}
			}
		}

		private void ServerHelp(bool showUsers, bool quiet) {
			string help = @"<HTML>
";
			if (showUsers)
				help = @"<H1>Known users and servers:</H1>
";
			else
				help = @"<H1>Known servers:</H1>
";
			help += @"<TABLE>
";
			help += ShowUserInfoHeader();
			UserList ul = UserList.GetUserList();
			for (int id = 0; id < ul.Count; id++) {
				if (showUsers || (!showUsers && ul.GetMachineName(id).Length > 0))
					help += ShowUserInfo(quiet, false, ul.GetAlias(id), ul.GetFirstname(id), ul.GetLastname(id), ul.GetMachineName(id), "", "", ul.GetFunction(id), ul.GetInternalNr(id), ul.GetPrivateNr(id), ul.GetMobileNr(id), ul.GetEMail(id), ul.GetDepartmentList(id), "");
			}
			ServerList sl = ServerList.GetServerList();
			for (int id = 0; id < sl.Count; id++) {
				help += ShowUserInfo(quiet, false, "", "", "", sl.GetServer(id), sl.GetUsername(id), sl.GetPassword(id), "", "", "", "", "", "", "");
			}

			help += @"</TABLE>
</HTML>";
			string filepath = FileTools.WriteTempFile("Servers.html", help);
			Shell.Browser(filepath, false, false);
		}

		private void ShowServers(string server) {
			if (Ping(server, "", ""))
				MessageBox.Show("  (On ) " + server, "GO ShowServers");
			else
				MessageBox.Show("  (Off) " + server, "GO ShowServers");
		}

		private void ServerBranch(string alias, string server) {
			if (server.Length > 0)
			ShowServerInfo(server);
			if (Ping(server, "", ""))
				Shell.ExplorerPath(@"\\" + server, false, false);
		}

		static string	ShowServerInfoSyntax(bool syntaxOnly, bool switchesOnly) { return "-S {Server}"; }
		public void ShowServerInfo(Options option) {
			SortedList argOptions = new SortedList();
			argOptions["-S"] = "";	// -S {Server}
			CommandLine.EvalCommandLine(ref argOptions);
			string server	= (string)argOptions["-S"];

			ShowServerInfo(server);
		}

		private void ShowServerInfo(string server) {
			ServerList sl = ServerList.GetServerList();
			int id = sl.GetID(server);
			if (id >= 0) {
				string username = sl.GetUsername(id);
				string password = sl.GetPassword(id);
				string status = "";
				if (server.Length > 0) {
					if (Ping(server, "", "")) {
						Shell.ExplorerPath(@"\\" + server, false, false);
						status = "(On )";
					}
					else
						status = "(Off)";
				}

				string info = String.Format(@"Server: {0} {1}
Username: {2}
Password: {3}", sl.GetServer(id), status, username, password);
				MessageBox.Show(info, "GO ServerInfo");
			}
			else
				MessageBox.Show("Unknown server " + server, "GO ServerInfo");
		}

		private string ShowUserInfoHeader() {
			return @"<TR>
<TD>ALIAS</TD>
<TD>NAME</TD>
<TD>MACHINE</TD>
<TD>STATUS</TD>
<TD>USERNAME</TD>
<TD>PASSWORD</TD>
<TD>FUNCTION</TD>
<TD>INTERNAL NR</TD>
<TD>PRIVATE NR</TD>
<TD>MOBILE NR</TD>
<TD>E-MAIL</TD>
<TD>DEPARTMENT LIST</TD>
<!--TD>MAIL</TD-->
</TR>
";
		}
		private string ShowUserInfo(bool quiet, bool msgBox, string alias, string firstname, string lastname, string server, string username, string password, string function, string internalNr, string privateNr, string mobileNr, string email, string departmentList, string msg) {
			string status = "";
			string htmlStatus = "";
			if (server.Length > 0) {
				if (server.StartsWith(@"\\"))
					server = server.Substring(2);
				string pingServer = server;
				int i = pingServer.IndexOf('\\');
				if (i >= 0)
					pingServer = pingServer.Substring(0, i);

				if (quiet || Ping(pingServer, firstname, lastname)) {
					if (!quiet) {
						status = "(On )";
						htmlStatus = "(<font color='Green'>On </font>)";
					}
					if (msgBox)
						Shell.ExplorerPath(@"\\" + server, false, false);
					if (msg.Length > 0)
						NetSend(server, msg, firstname, lastname);
				}
				else {
					status = "(Off)";
					htmlStatus = "(<font color='Red'>Off</font>)";
				}
			}

			if (msgBox) {
				if (alias.Length > 0) {
					string info = String.Format(@"Alias: {0}
First name: {1}
Last name: {2}
Machine name: {3} {4}
Username: {5}
Password: {6}
Function: {7}
Internal Nr: {8}
Private Nr:  {9}
Mobile Nr:   {10}
E-Mail: {11}
Department List: {12}", alias, firstname, lastname, server, status, username, password, function, internalNr, privateNr, mobileNr, email, departmentList);
					MessageBox.Show(info, "GO ShowUserInfo");
				}
				else if (server.Length == 0) {
					MessageBox.Show("Unknown alias " + alias, "GO ShowUserInfo");
				}
			}

			return String.Format(@"<TR>
<TD>{0}</TD>
<TD>{1} {2}</TD>
<TD>{3}</TD>
<TD>{4}</TD>
<TD>{5}</TD>
<TD>{6}</TD>
<TD>{7}</TD>
<TD>{8}</TD>
<TD>{9}</TD>
<TD>{10}</TD>
<TD>{11}</TD>
<TD>{12}</TD>
<!--TD>{13}</TD-->
</TR>
", alias, firstname, lastname, server, htmlStatus, username, password, function, internalNr, privateNr, mobileNr, email, departmentList
				, email.Length > 0 ? (alias + "@NetEconomy.com").ToUpper() == email.ToUpper() ? "Yes" : "No" : ""
				);
		}

		public string	NetSendSyntax(bool syntaxOnly, bool switchesOnly) { return "-S {Server} [-F {Firstname} -L {Lastname}] -M {Message}"; }
		public void NetSend(Options option) {
			SortedList argOptions = new SortedList();
			argOptions["-S"] = "";		// {-S : server}
			argOptions["-F"] = "";		// {-F : firstname}
			argOptions["-L"] = "";		// {-L : lastname}
			argOptions["-M"] = "";		// {-M : message}
			CommandLine.EvalCommandLine(ref argOptions);
			string	server			= (string)argOptions["-S"];
			string	firstname		= (string)argOptions["-F"];
			string	lastname		= (string)argOptions["-L"];
			string	msg				= (string)argOptions["-M"];

			if (server.Length == 0)
				throw new Exception("No -S server provided");
			if (msg.Length == 0)
				throw new Exception("No -M message provided");

			NetSend(server, msg, firstname, lastname);
		}

		public bool NetSend(string server, string msg, string firstname, string lastname) {
			string cmd = string.Format(" {0} {1} says: {2}", firstname, lastname, msg);
			bool ret = Ping(server, firstname, lastname);
			if (ret) {
				string output = null;
				ret = Shell.ShellCmd("net", "send " + server + cmd, false, out output) == 0 ? true : false;
			}
			if (ret)
				MessageBox.Show("Message sent to [" + server + "]:" + cmd, "GO NetSend");
			else
				MessageBox.Show("Message NOT sent to [" + server + "]:" + cmd, "GO NetSend");
	
			return ret;
		}

		public string	PingSyntax(bool syntaxOnly, bool switchesOnly) { return "-S {Server} [-F {Firstname} -L {Lastname}]"; }
		public void Ping(Options option) {
			SortedList argOptions = new SortedList();
			argOptions["-S"] = "";		// {-S : server/user to ping}
			argOptions["-F"] = "";		// {-F : firstname}
			argOptions["-L"] = "";		// {-L : lastname}
			CommandLine.EvalCommandLine(ref argOptions);
			string	server			= (string)argOptions["-S"];
			string	firstname		= (string)argOptions["-F"];
			string	lastname		= (string)argOptions["-L"];

			bool response = Ping(server, firstname, lastname);
			PingMsg(response, server, firstname, lastname);
		}

		private bool Ping(string server, string firstname, string lastname) {
			bool response = false;

			if (server.Length > 0) {
				string output = null;
				int ret = Shell.ShellCmd("ping", "-n 1 -w 500 " + server, false, out output);
				if (output.IndexOf("Received = 1") >= 0)
					response = true;;
			}
			return response;
		}

		private void PingMsg(bool response, string server, string firstname, string lastname) {
			if (firstname.Length == 0)
				MessageBox.Show(string.Format("Computer ({0}) is {1}currently logged in.", server, response ? "" : "not "));
			else
				MessageBox.Show(string.Format("{0} {1}'s computer ({2}) is {3}currently logged in.", firstname, lastname, server, response ? "" : "not "));
		}
		#endregion Servers/Users

		#region Servers
		public class ServerList {
			private static ServerList serverList = null;
			public static ServerList GetServerList()
			{
				if (serverList == null)
					serverList = new ServerList();
				return serverList;
			}

			private class ServerData {
				public ServerData(string uncpath, string username, string password) {
					this.uncpath	= uncpath;
					this.username	= username;
					this.password	= password;
				}

				public string uncpath;
				public string username;
				public string password;
			}

			private SortedList servers;

			private ServerList() {
				servers = new SortedList();

				servers.Add("NETECONOMY",	new ServerData(@"\\edrgfs01\NetEconomy",	"", ""));
				servers.Add("MASTERS",		new ServerData(@"\\edrgfs01\Masters",		"", ""));
				servers.Add("WISSEL",		new ServerData(@"\\edrgfs01\Wissel",		"", ""));
				servers.Add("ACT",			new ServerData(@"\\edrgsw02\Act database",	"", ""));
				servers.Add("ACT_FILES",	new ServerData(@"\\edrgfs01\Act_files",		"", ""));
				servers.Add("SCHNELL",		new ServerData(@"\\SCHNELL",				"Administrator", "alive"));
			}

			public int Count { get { return servers.Count; } }
			public int GetID(string server) { return servers.IndexOfKey(server.ToUpper()); }
			public string GetServer(int id) {
				if (id >= 0 && id < servers.Count)
					return (string)servers.GetKey(id);
				return "";
			}
			public string GetUncPath(string server) { return GetUncPath(GetID(server)); }
			public string GetUncPath(int id) {
				if (id >= 0 && id < servers.Count)
					return ((ServerData)servers.GetByIndex(id)).uncpath;
				return "";
			}
			public string GetUsername(string server) { return GetUsername(GetID(server)); }
			public string GetUsername(int id) {
				if (id >= 0 && id < servers.Count)
					return ((ServerData)servers.GetByIndex(id)).username;
				return "";
			}
			public string GetPassword(string server) { return GetPassword(GetID(server)); }
			public string GetPassword(int id) {
				if (id >= 0 && id < servers.Count)
					return ((ServerData)servers.GetByIndex(id)).password;
				return "";
			}
		};
		#endregion Servers

		#region Users
		public class UserList
		{
			private static UserList userList = null;
			public static UserList GetUserList()
			{
				if (userList == null)
					userList = new UserList();
				return userList;
			}

			private class UserData {
				public UserData(string firstName, 
								string lastName, 
								string function, 
								string machineName, 
								string internalNr,
								string privateNr,
								string mobileNr,
								string email,
								string birthday,
								string departmentList)
				{
					this.firstName		= firstName;
					this.lastName		= lastName;
					this.function		= function;
					this.machineName	= machineName;
					this.internalNr		= internalNr;
					this.privateNr		= privateNr;
					this.mobileNr		= mobileNr;
					this.email			= email;
					this.birthdate		= birthdate;
					this.departmentList	= departmentList;
				}

				public string firstName;
				public string lastName;
				public string function;
				public string machineName;
				public string internalNr;
				public string privateNr;
				public string mobileNr;
				public string email;
				public string birthdate;
				public string departmentList;
			}

			private SortedList users;

			private UserList()
			{
				users = new SortedList();

				/*
				users.Add("bblokdijk",		new UserData("Ben",				"Blokdijk",					"",										"",				"",					"",					"",					"",								"",				""));
				users.Add("ccarvalho",		new UserData("Camilo Fernandes de", "Carvalho",				"Implementation Consultant",			"",				"",					"",					"+31 (6) 21-51-5209","",							"19-Apr",		"");
				users.Add("cdieles",		new UserData("Claudine",		"Dieles",					"Sales",								"",				"",					"",					"",					"",								"23-Oct",		"");
				users.Add("cjaakke",		new UserData("Claudia",			"Jaakke",					"",										"",				"",					"",					"",					"",								"12-Apr-1970",	""));
				users.Add("cneelissen",		new UserData("Carlijn",			"Neelissen",				"Marketing Coordinator",				"",				"",					"",					"",					"",								"25-Jul",		""));
				users.Add("cparry",			new UserData("Dr. Chris",		"Parry",					"Pre-Sales Consultant",					"",				"",					"",					"",					"",								"3-Apr",		""));
				users.Add("crijswijk",		new UserData("Cok",				"van Rijswijk",				"",										"",				"",					"",					"",					"",								"",				""));
				users.Add("cverhoef",		new UserData("Corine",			"Verhoef",					"",										"",				"",					"",					"",					"",								"",				""));
				users.Add("dlange",			new UserData("Duco",			"de Lange",					"",										"",				"",					"",					"",					"",								"3-Feb-1969",	""));
				users.Add("ekoelman",		new UserData("Esther",			"Koelman",					"",										"PC0130A",		"",					"",					"",					"",								"",				""));
				users.Add("evliet",			new UserData("Eimert van der",	"Vliet",					"Salary Administrator",					"",				"+31 (70) 452-5407","+31 (71) 523-5588","",					"evliet@edrgroup.net",			"27-Jun",		""));
				users.Add("hboonstoppel",	new UserData("Hans",			"Boonstoppel",				"Director Strategic Alliances",			"",				"",					"+31 (182) 39-91-91","+31 (6) 53-32-8672","",							"29-Oct",		""));
				users.Add("hleeuwen",		new UserData("Hans",			"van Leeuwen",				"",										"",				"",					"",					"",					"",								"1-Oct",		""));
				users.Add("izasarsky",		new UserData("Ivan",			"Zasarsky",					"",										"",				"",					"",					"",					"",								"",				""));
				users.Add("jbone",			new UserData("John",			"Bone",						"",										"",				"",					"",					"",					"",								"3-Aug",		""));
				users.Add("jvermeulen",		new UserData("Jolanda",			"Vermeulen",				"Personnel Officer",					"",				"",					"+31 (70) 452-5253","",					"jverspeek@edrgroup.net",		"17-Mar",		""));
				users.Add("jwesbeek",		new UserData("Jorg",			"Wesbeek",					"",										"LAP0050",		"",					"",					"",					"",								"",				""));
				users.Add("lvorster",		new UserData("Lucie",			"Vorster",					"Personnel Officer",					"",				"+31 (70) 452-5255","",					"",					"lvorster@edrgroup.net",		"20-Mar-1975",	""));
				users.Add("maukes",			new UserData("Marco",			"Aukes",					"",										"",				"",					"",					"",					"",								"",				""));
				users.Add("mschrauwen",		new UserData("Melanie",			"Schrauwen",				"",										"",				"",					"",					"",					"",								"10-Jun",		""));
				users.Add("mvacquier",		new UserData("Marco",			"Vacquier",					"Quality Assurance Engineer",			"",				"",					"",					"",					"",								"14-Sep-1967",	""));
				users.Add("mwijnen",		new UserData("Mike",			"Wijnen",					"Software Engineer",					"PC0034",		"5441",				"",					"06-52065548",		"mwijnen@edrgroup.nl",			"11-May-1971",	"Neteconomy Research & Development"));
				users.Add("nbreukelman",	new UserData("Nicole",			"Breukelman",				"IT Administration Assistent",			"",				"",					"",					"+31 (6) 13-13-1364","",							"26 May",		""));
				users.Add("ngarg",			new UserData("Naveen",			"Garg",						"",										"",				"",					"",					"+31 (6) 19-79-0233","",							"6-Aug",		""));
				users.Add("nhamoen",		new UserData("Niels",			"Hamoen",					"",										"PC0102",		"",					"",					"",					"",								"",				""));
				users.Add("pdijkstra",		new UserData("Pietie",			"Dijkstra",					"",										"",				"",					"",					"",					"",								"",				""));
				users.Add("pjenkins",		new UserData("Peter",			"Jenkins",					"",										"",				"",					"",					"",					"",								"", 			"Neteconomy Sales & Marketing"));
				users.Add("pvoormeulen",	new UserData("Pim",				"Voormeulen",				"",										"PC0211",		"",					"",					"",					"",								"8-Aug-1964",	""));
				users.Add("rbodaan",		new UserData("Robin",			"Bodaan",					"",										"",				"",					"",					"",					"",								"17-Aug",		""));
				users.Add("rsaleem",		new UserData("Raza",			"Saleem",					"",										"",				"",					"",					"",					"",								"",				""));
				users.Add("rverheij",		new UserData("Richard",			"Verheij",					"General Management",					"",				"+31 (70) 452-5431","",					"+31 (6) 53-15-1469","rverheij@edrgroup.nl",		"14-Mar",		""));
				users.Add("rvlaanderen",	new UserData("Ronald",			"van Vlaanderen",			"",										"",				"",					"",					"",					"",								"24-Jul",		""));
				users.Add("sbruijn",		new UserData("Saskia",			"de Bruijn",				"",										"",				"",					"",					"",					"",								"",				""));
				users.Add("sspilleman",		new UserData("Sander",			"Spilleman",				"",										"",				"",					"",					"",					"",								"",				""));
				users.Add("swielen",		new UserData("Stefan",			"van der Wielen",			"",										"",				"",					"",					"",					"",								"",				""));
				users.Add("wjalink",		new UserData("Wolter",			"Jalink",					"",										"",				"",					"",					"",					"",								"",				""));
				users.Add("wjongsma",		new UserData("Will",			"Jongsma",					"",										"",				"",					"",					"",					"",								"",				""));
				*/

				// TODO: Populate from LDAP / Exchange / Phone List / 

				users.Add("astewartbrown",	new UserData("Alan",			"Stewart-Brown",			"Sales Director UK",					"",				"",					"",					"",					"astewartbrown@neteconomy.com",	"",				"Neteconomy Implementation"));
				users.Add("abrugts",		new UserData("Arthur",			"Brugts",					"Software Engineer",					"",				"5491",				"",					"",					"abrugts@neteconomy.com",		"19-Nov",		"Neteconomy Research & Development"));
				users.Add("acornelissen",	new UserData("Anton",			"Cornelissen",				"Project Leader",						"",				"5485",				"",					"+31 6 17448444",	"acornelissen@neteconomy.com",	"25-Apr",		"Neteconomy Implementation"));
				users.Add("adavies",		new UserData("Andrew",			"Davies",					"Sales Director US",					"",				"",					"",					"+ 1917 8253910",	"",								"",				"Neteconomy Sales & Marketing"));
				users.Add("aherk",			new UserData("Angela",			"van Herk",					"Personal Assistent/ Partner Manager",	"",				"5488",				"",					"06-51046984",		"aherk@neteconomy.com",			"26-Jun",		"Neteconomy Sales & Marketing"));
				users.Add("aholland",		new UserData("Alison",			"Holland",					"Director Worldwide Marketing",			"",				"+15088936066",		"",					"+15082874939",		"aholland@neteconomy.com",		"25-Oct",		"Neteconomy Sales & Marketing"));
				users.Add("asekeris",		new UserData("Anton",			"Sekeris",					"Software Engineer",					"",				"5466",				"",					"+31 638539449",	"asekeris@neteconomy.com",		"22-Sep-1974",	"Neteconomy Research & Development"));
				users.Add("aleuven",		new UserData("Annemiek",		"van Leuven",				"HR Officer",							"",				"5478",				"",					"+31 6 53274123",	"aleuven@neteconomy.com",		"",				"Neteconomy P&O"));
				users.Add("aoudheusden",	new UserData("Andre",			"van Oudheusden",			"Service Manager Telecom",				"",				"5467",				"070-3877417",		"06-26010795",		"aoudheusden@neteconomy.com",	"12-Jul",		"Neteconomy Erase Telecom"));
				users.Add("aschouw",		new UserData("Arnoud",			"Schouw",					"Project Leader",						"",				"5489",				"",					"06-13131364",		"",								"",				"Neteconomy Implementation"));
				users.Add("bchatterton",	new UserData("Barbara",			"Chatterton",				"Technical Writer",						"",				"5472",				"",					"",					"bchatterton@neteconomy.com",	"1-Jun",		"Neteconomy Research & Development"));
				users.Add("bkuntz",			new UserData("Bas",				"Kuntz",					"Interim Manager/CEO",					"",				"5481",				"",					"+31 6-53382840",	"skuntz@neteconomy.com",		"27-Feb",		"Neteconomy  Management"));
				users.Add("bscheffers",		new UserData("Ben",				"Scheffers",				"Business Analist",						"BSCHEFFERS",	"5468",				"",					"+31 6-54954416",	"bscheffers@neteconomy.com",	"20-Apr-1969",	"Neteconomy Implementation"));
				users.Add("cdixit",			new UserData("Chintamani",		"Dixit",					"Implementation Engineer",				"",				"5485",				"",					"+31 6-28404689",	"cdixit@neteconomy.com",		"30-Juli-1974",	"Neteconomy Implementation"));
				users.Add("chagenaars",		new UserData("Carlos",			"Hagenaars",				"Product Manager",						"CHAGENAARS",	"5470",				"",					"+31 6 43987159",	"chagenaars@neteconomy.com",	"16-Feb",		"Neteconomy Research & Development"));
				users.Add("cruigrok",		new UserData("Caspar",			"Ruigrok",					"Implementation Consultant",			"",				"5473",				"",					"06-13 13 12 68",	"cruigrok@neteconomy.com",		"",				"Neteconomy Implementation"));
				users.Add("dbisoendial",	new UserData("Dinesh",			"Bisoendial",				"Software Engineer",					"",				"",					"",					"",					"dbisoendial@neteconomy.com",	"",				"Neteconomy Research & Development"));
				users.Add("ddekkers",		new UserData("Dave",			"Dekkers",					"Business Analist",						"",				"5452",				"",					"06-27032924",		"ddekkers@neteconomy.com",		"1-Jun-1972",	"Neteconomy Implementation"));
				users.Add("dprischink",		new UserData("Deniz",			"Prischink",				"VP Implementation",					"",				"5477",				"",					"+ 31 6-27032923",	"dprischink@neteconomy.com",	"23-Oct",		"Neteconomy  Management;Neteconomy Implementation"));
				users.Add("dslodki",		new UserData("David",			"Slodki",					"Pre Sales Manager",					"",				"",					"",					"+1 718 757 8328",	"dslodki@neteconomy.com",		"",				""));
				users.Add("eblok",			new UserData("Eric",			"Blok",						"QA Engineer",							"",				"5419",				"",					"",					"eblok@neteconomy.com",			"24-Mar-1969",	"Neteconomy Research & Development"));
				users.Add("edopheide",		new UserData("Ella",			"Dopheide",					"Personal Assistent",					"",				"5479",				"",					"",					"edopheide@neteconomy.com",		"5-Apr",		"Neteconomy  Management"));
				users.Add("emiddelkoop",	new UserData("Erik",			"Middelkoop",				"Trainer",								"",				"",					"",					"+ 31 6-55144282",	"emiddelkoop@neteconomy.com",	"",				"External"));
				users.Add("fbly",			new UserData("Frits",			"van der Bly",				"Trainer",								"PC0185",		"5417",				"",					"+31 621551947",	"fbly@neteconomy.com",			"17-apr-1956",	"Neteconomy Research & Development"));
				users.Add("fcoolen",		new UserData("Frans",			"Coolen",					"QA Engineer",							"",				"5419",				"",					"",					"",								"1 Oct",		"Neteconomy Research & Development"));
				users.Add("froelfzema",		new UserData("Ferrie",			"Roelfzema",				"RFP Desk Manager",						"",				"5487",				"",					"+31613131361",		"froelfzema@neteconomy.com",	"19-May",		"Neteconomy Sales & Marketing"));
				users.Add("fsamwel",		new UserData("Frank",			"Samwel",					"Senior Quality Assurance Engineer",	"",				"5458",				"",					"",					"fsamwel@neteconomy.com",		"",				"Neteconomy Research & Development"));
				users.Add("gmolenaar",		new UserData("Gertjan",			"Molenaar",					"Financial Controller a.i.",			"",				"5463",				"",					"",					"",								"",				"Neteconomy Finance"));
				users.Add("gsenden",		new UserData("Ger",				"Senden",					"Software Engineer",					"PC0285",		"5469",				"",					"",					"gsenden@neteconomy.com",		"28-May-1975",	"Neteconomy Research & Development"));
				users.Add("hbarenholz",		new UserData("Henry",			"Barenholz",				"VP Sales & Marketing",					"",				"5482",				"",					"+31(0)653155381",	"",								"20-Apr",		"Neteconomy  Management;Neteconomy Business Development;Neteconomy Sales & Marketing"));
				users.Add("hpernot",		new UserData("Harold",			"Pernot",					"Product Manager",						"",				"5466",				"070-3620738",		"06-25014105",		"hpernot@neteconomy.com",		"17-Dec",		"Neteconomy Research & Development"));
				users.Add("icthelpdesk",	new UserData("",				"Icthelpdesk",				"Helpdesk",								"",				"5450",				"",					"",					"icthelpdesk@edrgroup.net",		"",				""));
				users.Add("jdekker",		new UserData("Jeroen",			"Dekker",					"Junior Product Manager",				"",				"5459",				"",					"",					"jdekker@neteconomy.com",		"",				"Neteconomy Product Management"));
				users.Add("jdorssers",		new UserData("Johan",			"Dorssers",					"Software Engineer",					"PC0155",		"5465",				"",					"+31 6-54288113",	"jdorssers@neteconomy.com",		"21-Jun-1966",	"Neteconomy Research & Development"));
				users.Add("jhodes",			new UserData("Jonathan",		"Hodes",					"CFO",									"",				"5474",				"",					"0031 6 27032921",	"jhodes@neteconomy.com",		"10-May",		"Neteconomy  Management;Neteconomy Finance"));
				users.Add("jjanssens",		new UserData("Jeroen",			"Janssens",					"Product Specialist",					"",				"5441",				"",					"",					"jjanssens@neteconomy.com",		"17-Sep-1962",	""));
				users.Add("jkooijman",		new UserData("Joyce",			"Kooijman",					"Marketing & Communicatie Coordinator",	"",				"5486",				"",					"+31 6 27032913",	"",								"28-Dec",		"Neteconomy Sales & Marketing"));
				users.Add("jlange",			new UserData("Jan Willem",		"de Lange",					"VP Development",						"",				"5483",				"",					"06-10946293",		"jlange@neteconomy.com",		"11-Aug",		"Neteconomy  Management;Neteconomy Research & Development"));
				users.Add("jleeuwen",		new UserData("Johan",			"van Leeuwen",				"",										"",				"",					"",					"",					"",								"",				"External"));
				users.Add("jlintelo",		new UserData("Juriaan",			"Lintelo",					"",										"",				"",					"",					"",					"",								"",				"External"));
				users.Add("kbenhaddaoui",	new UserData("Khalid",			"Benhaddaoui",				"Assistent Controller",					"",				"5414",				"",					"",					"kbenhaddaoui@neteconomy.com",	"3-Apr-1971",	"Neteconomy Finance"));
				users.Add("lgeest",			new UserData("Luigi",			"van Geest",				"Sales Director Continental Europe",	"",				"5418",				"",					"+ 31 6-52631667",	"lgeest@neteconomy.com",		"18-Jul",		"Neteconomy Sales & Marketing"));
				users.Add("lmarchese",		new UserData("Luciano",			"Marchese",					"Customer Support Engineer",			"",				"5462",				"",					"",					"lmarchese@neteconomy.com",		"",				"Neteconomy Support"));
				users.Add("lquerton",		new UserData("Luc",				"Querton",					"Channel and Partner Director",			"",				"",					"",					"+33 686836797",	"lquerton@neteconomy.com",		"",				"Neteconomy Sales & Marketing"));
				users.Add("mblom",			new UserData("Michel",			"Blom",						"Technical Product Manager",			"",				"",					"",					"",					"mblom@neteconomy.com",			"",				"Neteconomy Research & Development"));
				users.Add("mboer",			new UserData("Michael",			"Boer",						"Quality Assurance Manager",			"MBOER",		"5416",				"",					"",					"mboer@neteconomy.com",			"6-Aug-1975",	"Neteconomy Research & Development"));
				users.Add("mcousins",		new UserData("Melanie",			"Cousins",					"UK Sales Assistant",					"",				"00441189026762",	"",					"+ 44 7798754445",	"mcousins@neteconomy.com",		"30-Jan",		"Neteconomy Sales & Marketing"));
				users.Add("miking",			new UserData("Marcel",			"Iking",					"Support Manager",						"",				"5492",				"",					"06-10940005",		"miking@neteconomy.nl",			"17-Oct-1964",	"Neteconomy Support"));
				users.Add("mpeereboom",		new UserData("Michiel",			"Peereboom",				"Customer Support Engineer",			"",				"5447",				"",					"",					"",								"18-May-1975",	"Neteconomy Support"));
				users.Add("mpeeters",		new UserData("Maarten",			"Peeters",					"Software Engineer",					"",				"5491",				"",					"",					"mpeeters@neteconomy.com",		"13-Jul-1977",	"Neteconomy Research & Development"));
				users.Add("mverheij",		new UserData("Max",				"Verheij",					"",										"",				"",					"",					"",					"",								"",				"Neteconomy Sales & Marketing"));
				users.Add("nbosch",			new UserData("Niek",			"ten Bosch",				"Pre Sales Manager Europe",				"",				"5476",				"+31628128390",		"001 6467855144",	"",								"5-Aug-1974",	"Neteconomy Sales & Marketing"));
				users.Add("nluuring",		new UserData("Niek",			"Luuring",					"Software Engineer",					"PC0299",		"5472",				"",					"",					"nluuring@neteconomy.com",		"5-Sep",		"Neteconomy Research & Development"));
				users.Add("priethoven",		new UserData("Pablo",			"Riethoven",				"Software Engineer",					"",				"5465",				"",					"",					"",								"",				"Neteconomy Sales & Marketing"));
				users.Add("pkwakernaak",	new UserData("Peter",			"Kwakernaak",				"VP Business Development",				"",				"5494",				"",					"+31654394102",		"pkwakernaak@neteconomy.com",	"",				"Neteconomy  Management;Neteconomy Sales & Marketing"));
				users.Add("psteendijk",		new UserData("Peter",			"Steendijk",				"Sales Director",						"",				"+60(3)21684227",	"",					"+60122359025",		"psteendijk@neteconomy.com",	"25-Jun",		"Neteconomy Sales & Marketing"));
				users.Add("receptie",		new UserData("Secretariaat",	"",							"",										"",				"",					"",					"",					"",								"",				"EDRGroup"));
				users.Add("rchadha",		new UserData("Rishi",			"Chadha",					"Implementation Consultant",			"",				"",					"",					"",					"rchadha@neteconomy.com",		"",				"Neteconomy Implementation"));
				users.Add("rhoogenboom",	new UserData("Richard",			"Hoogenboom",				"Implementation Consultant",			"IBM_T40P",		"5461",				"070-3640998",		"06-54288075",		"rhoogenboom@neteconomy.com",	"17-Oct-1974",	"Neteconomy Implementation"));
				users.Add("rmccarthy",		new UserData("Richard",			"McCarthy",					"VP Product Marketing",					"",				"5446",				"",					"06 13131746",		"rmccarthy@neteconomy.com",		"",				"Neteconomy  Management"));
				users.Add("rovervest",		new UserData("Rob",				"van Overvest",				"Software Engineer",					"",				"5467",				"070-3683033",		"",					"rovervest@neteconomy.com",		"24-Sep",		"Neteconomy Erase Telecom"));
				users.Add("rrozsa",			new UserData("Richard",			"Rozsa",					"Customization Manager",				"LAP0066",		"5471",				"0172-652509",		"06 - 51569738",	"rrozsa@neteconomy.com",		"4-Dec-1960",	"Neteconomy Research & Development"));
				users.Add("sgonesh",		new UserData("Sharita",			"Gonesh",					"IT Administrative Assistant",			"",				"5457",				"",					"",					"sgonesh@neteconomy.com",		"30-Sep-1975",	"Neteconomy Implementation"));
				users.Add("shempenius",		new UserData("Sikke",			"Hempenius",				"Product Specialist",					"",				"5470",				"",					"06-47226647",		"",								"18-Jun-1979",	"Neteconomy Research & Development"));
				users.Add("smulukutla",		new UserData("Sarat",			"Mulukutla",				"Pre-Sales",							"",				"0060 321684227",	"",					"0091 9820288732",	"smulukutla@neteconomy.com",	"15-Apr",		"Neteconomy Sales & Marketing"));
				users.Add("srichardson",	new UserData("Simon",			"Richardson",				"Pre Sales Consultant",					"",				"+441189026763",	"",					"+44 7940781060",	"",								"29-Jan",		"Neteconomy Sales & Marketing"));
				users.Add("srietbroek",		new UserData("Saskia",			"Rietbroek",				"Financial Crime Advisor",				"",				"",					"+ 305 428 0959",	"+ 305 608 7888",	"",								"",				"Neteconomy Sales & Marketing"));
				users.Add("srolefes",		new UserData("Samantha",		"Rolefes",					"Product Manager",						"PC0143",		"5496",				"",					"06-21853430",		"srolefes@neteconomy.com",		"2-Jul",		"Neteconomy Product Management"));
				users.Add("svliet",			new UserData("Stan",			"Harmsen van der Vliet",	"Manager NetAcademy",					"",				"5448",				"",					"+31 06 13131745",	"svliet@neteconomy.com",		"",				"Neteconomy  Management"));
				users.Add("szijden",		new UserData("Stefan",			"van der Zijden",			"VP Operations",						"",				"5498",				"",					"",					"szijden@neteconomy.com",		"",				"Neteconomy Implementation;Neteconomy Support"));
				users.Add("tcauwelaert",	new UserData("Thomas",			"van Cauwelaert",			"EMEA Channels Manager",				"",				"+33 622438274",	"",					"",					"tcauwelaert@neteconomy.com",	"",				"Neteconomy Sales & Marketing"));
				users.Add("tgroot",			new UserData("Ties",			"de Groot",					"Customer Support Engineer",			"",				"5480",				"",					"06-41549257",		"tgroot@neteconomy.com",		"4-Jun-1964",	"Neteconomy Support"));
				users.Add("trietdijk",		new UserData("Tames",			"Rietdijk",					"",										"",				"",					"",					"",					"",								"",				"External"));
				users.Add("wwildt",			new UserData("Wim",				"de Wildt",					"Account Manager",						"",				"5443",				"",					"06-22383920",		"",								"21-Oct",		"Neteconomy Sales & Marketing"));
			}																																	

			public int Count { get { return users.Count; } }
			public int GetID(string alias) { return users.IndexOfKey(alias.ToLower()); }
			public SortedList GetFirstNames(string firstname) {
				SortedList	sl = new SortedList();
				IDictionaryEnumerator usersEnum = users.GetEnumerator();
				while (usersEnum.MoveNext()) {
					if (((UserData)usersEnum.Value).firstName.ToUpper() == firstname.ToUpper())
						sl.Add(usersEnum.Key, ((UserData)usersEnum.Value).firstName);
				}
				return sl;
			}
			public SortedList GetLastNames(string lastname) {
				SortedList	sl = new SortedList();
				IDictionaryEnumerator usersEnum = users.GetEnumerator();
				while (usersEnum.MoveNext()) {
					if (((UserData)usersEnum.Value).lastName.ToUpper() == lastname.ToUpper())
						sl.Add(usersEnum.Key, ((UserData)usersEnum.Value).lastName);
				}
				return sl;
			}
			public string GetAlias(int id) {
				if (id >= 0 && id < users.Count)
					return (string)users.GetKey(id);
				return "";
			}
			public string GetFirstname(string alias) { return GetFirstname(GetID(alias)); }
			public string GetFirstname(int id) {
				if (id >= 0 && id < users.Count)
					return ((UserData)users.GetByIndex(id)).firstName;
				return "";
			}
			public string GetLastname(string alias) { return GetLastname(GetID(alias)); }
			public string GetLastname(int id) {
				if (id >= 0 && id < users.Count)
					return ((UserData)users.GetByIndex(id)).lastName;
				return "";
			}
			public string GetFunction(string alias) { return GetFunction(GetID(alias)); }
			public string GetFunction(int id) {
				if (id >= 0 && id < users.Count)
					return ((UserData)users.GetByIndex(id)).function;
				return "";
			}
			public string GetMachineName(string alias) { return GetMachineName(GetID(alias)); }
			public string GetMachineName(int id) {
				if (id >= 0 && id < users.Count)
					return ((UserData)users.GetByIndex(id)).machineName;
				return "";
			}
			public string GetInternalNr(string alias) { return GetInternalNr(GetID(alias)); }
			public string GetInternalNr(int id) {
				if (id >= 0 && id < users.Count)
					return ((UserData)users.GetByIndex(id)).internalNr;
				return "";
			}
			public string GetPrivateNr(string alias) { return GetPrivateNr(GetID(alias)); }
			public string GetPrivateNr(int id) {
				if (id >= 0 && id < users.Count)
					return ((UserData)users.GetByIndex(id)).privateNr;
				return "";
			}
			public string GetMobileNr(string alias) { return GetMobileNr(GetID(alias)); }
			public string GetMobileNr(int id) {
				if (id >= 0 && id < users.Count)
					return ((UserData)users.GetByIndex(id)).mobileNr;
				return "";
			}
			public string GetEMail(string alias) { return GetEMail(GetID(alias)); }
			public string GetEMail(int id) {
				if (id >= 0 && id < users.Count)
					return ((UserData)users.GetByIndex(id)).email;
				return "";
			}
			public string GetBirthdate(string alias) { return GetBirthdate(GetID(alias)); }
			public string GetBirthdate(int id) {
				if (id >= 0 && id < users.Count)
					return ((UserData)users.GetByIndex(id)).birthdate;
				return "";
			}
			public string GetDepartmentList(string alias) { return GetDepartmentList(GetID(alias)); }
			public string GetDepartmentList(int id) {
				if (id >= 0 && id < users.Count)
					return ((UserData)users.GetByIndex(id)).departmentList;
				return "";
			}
		};
		#endregion Users

		#region Assorted
		static string	AwaySyntax(bool syntaxOnly, bool switchesOnly) { return "[-M {message default=Go Away}] -S [{size 1-6}]"; }
		public void		Away(Options option)
		{
			SortedList argOptions = new SortedList();
			argOptions["-M"] = "";		// {-M message}
			argOptions["-S"] = "";		// {-S <H#> size}
			CommandLine.EvalCommandLine(ref argOptions);
			string	message		= (string)argOptions["-M"];
			string	size		= (string)argOptions["-S"];

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
			if (switchesOnly)
				return "Calc -E expression";
			else {
				Calculator calc = new Calculator();
				return calc.Syntax();
			}
		}
		public void		Calc(Options option) {
			SortedList argOptions = new SortedList();
			argOptions["-E"] = "";		// {-E expression}
			CommandLine.EvalCommandLine(ref argOptions);
			string	expression	= (string)argOptions["-E"];

			Calculator calc = new Calculator();
			double ret = calc.Evaluate(expression);
			MessageBox.Show(expression + " = " + calc.FormatNumber(ret), "GO Calculator");
		}

		static string	FormatXMLSyntax(bool syntaxOnly, bool switchesOnly) { return "-S {srcFile} -D {dstFile}"; }
		public void		FormatXML(Options option) {
			SortedList argOptions = new SortedList();
			argOptions["-S"] = "";		// {-S srcFile}
			argOptions["-D"] = "";		// {-D dstFile}
			CommandLine.EvalCommandLine(ref argOptions);
			string	srcFile		= (string)argOptions["-S"];
			string	dstFile		= (string)argOptions["-D"];

			if (srcFile.Length == 0)
				throw new Exception("Missing -S {SrcFile} parameter");
			if (!File.Exists(srcFile))
				throw new Exception("Missing -S " + srcFile + " file");
			if (dstFile.Length == 0)
				throw new Exception("Missing -D {DstFile} parameter");

			TextFileContents tfc = new TextFileContents(srcFile);
			string contents = tfc.Contents;
			tfc = null;

			string output = "";
			string[] lines = contents.Split(new char[] {'\r', '\n'});
			int indent = -1;
			foreach (string roLine in lines) {
				string line = StringTools.AllTrim(roLine);
				int i = line.IndexOf('>');
				for ( ; i >= 0; ) {
					if (line.Substring(line.IndexOf('<') + 1, 1) == "/")
						indent--;
					for (int n = 0; n < indent; n++)
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

			tfc = new TextFileContents(dstFile);
			tfc.Contents = output;
			tfc = null;
		}

		static string	TouchSyntax(bool syntaxOnly, bool switchesOnly)
		{
			string syntax = "-T {timestamp} -F {filespec}";
			if (switchesOnly)
				return syntax;
			else
				return syntax + @"
 timestamp format: {anything C# can interpret}";
		}
		public void		Touch(Options option)
		{
			SortedList argOptions = new SortedList();
			argOptions["-T"] = "";		// [-t STAMP]
			argOptions["-F"] = "";		// {-F filespec}
			CommandLine.EvalCommandLine(ref argOptions);
			string	stamp		= (string)argOptions["-T"];
			string	filespec	= (string)argOptions["-F"];

			if (filespec.Length == 0)
				throw new Exception("No -F filespec");
			if (stamp.Length == 0)
				throw new Exception("No -T stamp");

			DateTime dt = Convert.ToDateTime(stamp);
			FileTools.TouchFilename(filespec, dt);
		}
		
		static string	UnsignSyntax(bool syntaxOnly, bool switchesOnly) { return "-F {file}"; }
		public void		Unsign(Options option)
		{
			SortedList argOptions = new SortedList();
			argOptions["-F"] = "";		// {-F file}
			CommandLine.EvalCommandLine(ref argOptions);
			string	file		= (string)argOptions["-F"];

			if (file.Length == 0)
				throw new Exception("No -F file");
			if (!File.Exists(file))
				throw new Exception("File " + file + " doesn't exist");

			string dst = GetDirectory("Temp") + @"\license.b64";
			File.Copy(file, dst, true);
			WinZip(dst);
			File.Delete(dst);
		}

		public void CreateNetworkMappings(Options option)
		{
			string output = null;
			Shell.ShellCmd("net", "use G: \"\\\\Schnell\\g$\"", false, out output);
			Shell.ShellCmd("net", "use G: \"\\\\Schnell\\g$\"", false, out output);
			Shell.ShellCmd("net", "use H: \"\\\\Edrgfs01\\Wissel\"", false, out output);
			Shell.ShellCmd("net", "use I: \"\\\\Edrgfs01\\Masters\"", false, out output);
			Shell.ShellCmd("net", "use N: \"\\\\Edrgfs01\\NetEconomy\"", false, out output);
			Shell.ShellCmd("net", "use S: \"\\\\netsuppdc01\\Software\"", false, out output);
			Shell.ShellCmd("net", "use T: \"\\\\Edrgfs01\\NetEconomy\\Templates\"", false, out output);
		}

		public void ReleaseNetworkMappings(Options option)
		{
			string output = null;
			Shell.ShellCmd("net", "use G: /D", false, out output);
			Shell.ShellCmd("net", "use H: /D", false, out output);
			Shell.ShellCmd("net", "use I: /D", false, out output);
			Shell.ShellCmd("net", "use N: /D", false, out output);
			Shell.ShellCmd("net", "use S: /D", false, out output);
			Shell.ShellCmd("net", "use T: /D", false, out output);
		}

		static string	UpdateCRCSyntax(bool syntaxOnly, bool switchesOnly) { return "-E {ERASE Environment Name}"; }
		public void UpdateCRC(Options option)
		{
			SortedList argOptions = new SortedList();
			argOptions["-E"] = "";		// -E {ERASE Environment Name}
			CommandLine.EvalCommandLine(ref argOptions);
			string	env		= (string)argOptions["-E"];

			bool envNameSelected = true;
			if (env.Length == 0) {
				envNameSelected = false;
				env = "Standard Test Env for 4.1.4";
			}

			string vbsTxt = @"' Adjust EnvironmentCollection parameter to match Name value in ERASE Environment Setup" + Environment.NewLine;
			vbsTxt += "Set EnvironmentCollection = CreateObject( \"EnvCore.EnvironmentCollection\" ) Set DatabaseCheck = CreateObject( \"EnvCore.DatabaseCheck\" )" + Environment.NewLine;
			vbsTxt += "EnvironmentCollection.LoadLocal" + Environment.NewLine;
			vbsTxt += String.Format("Set Environment = EnvironmentCollection( \"{0}\" )", env) + Environment.NewLine;
			vbsTxt += Environment.NewLine;
			vbsTxt += "CheckSum = DatabaseCheck.Check( Environment, \"new_crc_data.crc\", 1 )" + Environment.NewLine;
			vbsTxt += Environment.NewLine;
			vbsTxt += "Set Databasecheck = Nothing" + Environment.NewLine;
			vbsTxt += "Set Environment = Nothing" + Environment.NewLine;
			vbsTxt += "Set EnvironmentsCollection = Nothing" + Environment.NewLine;

			string vbsPath = FileTools.WriteTempFile("UpdateCRC.vbs", vbsTxt);
			if (!envNameSelected) {
				MessageBox.Show("About to launch ERASE Environment Setup to get Name of environment...", "GO UpdateCRC");
				Shell.ShellApp(GetDirectory("BackOffice") + @"\ERASE Environment Setup.exe", "", false, false); 
				Shell.Notepad(vbsPath, true, false);
			}
			if (File.Exists("erasedb.crc")) {
				if (File.Exists("new_crc_data.crc"))
					File.Delete("new_crc_data.crc");
				Shell.ShellApp(vbsPath, "", false, true);
				if (!File.Exists("new_crc_data.crc"))
					MessageBox.Show("Error: CRC file not generated!", "GO UpdateCRC");
				if (File.Exists("new_crc_data.crc"))
					Shell.Notepad("new_crc_data.crc", false, false);
				Shell.Notepad("erasedb.crc", false, false);
				MessageBox.Show("erasedb.crc updated", "GO UpdateCRC");
			}
		}

		static string	WsdlInfo()
		{
			return @"This may not be needed as we we can generate directly from java without wsdl files.
Also, conversion done here may not be necessary as localhost:10000 is default.";
		}
		static string	WsdlSyntax(bool syntaxOnly, bool switchesOnly) { return "-W {wsdlFile} -C {csFile} -N {namespace}"; }
		public void		Wsdl(Options option)
		{
			SortedList argOptions = new SortedList();
			argOptions["-W"] = "";		// {-W wsdlFile}
			argOptions["-C"] = "";		// {-C csFile}
			argOptions["-N"] = "";		// {-N namespace}
			CommandLine.EvalCommandLine(ref argOptions);
			string	wsdlFile		= (string)argOptions["-W"];
			string	csFile			= (string)argOptions["-C"];
			string	namespaceString	= (string)argOptions["-N"];

			if (wsdlFile.Length == 0)
				throw new Exception("Missing -W " + wsdlFile + "wsdlFile");
			if (csFile.Length == 0)
				throw new Exception("Missing -C " + csFile + " csFile");
			if (namespaceString.Length == 0)
				throw new Exception("Missing -N " + namespaceString + " namespaceString");
			if (!File.Exists(wsdlFile))
				throw new Exception("File " + wsdlFile + " not found");

			string csTempPath = Environment.ExpandEnvironmentVariables("%TEMP%") + @"\wsdl.cs";
			string output = null;
			Shell.ShellCmd("wsdl.exe", 
				String.Format("/language:CS /out:\"{0}\" /namespace:{1} {2}", 
					csTempPath, namespaceString, wsdlFile), 
				false, out output);

			TextFileContents tfc = new TextFileContents(csTempPath);
			string csContents = tfc.Contents;
			tfc = null;

			csContents.Replace(	"this.Url = \"http://sagitta.virgil.nl:51000", 
								"this.Url = \"http://localhost:10000");

			File.Delete(csTempPath);
			tfc = new TextFileContents(csFile);
			tfc.Contents = csContents;
			tfc = null;

			MessageBox.Show("Done generating " + csFile + " WSDL file.", "GO wsdl");
		}
		#endregion Assorted

		#region Stuff
		public string	GetDebtorNameSyntax(bool syntaxOnly, bool switchesOnly) { return "-C {DebtorNr}"; }
		public void		GetDebtorName(Options option)
		{
			SortedList argOptions = new SortedList();
			argOptions["-C"] = "";		// {-C DebtorNr}
			CommandLine.EvalCommandLine(ref argOptions);
			string	debtorNr	= (string)argOptions["-C"];

			if (debtorNr.Length == 0)
				throw new Exception("Error: Missing -F DebtorNr argument");

			string debtorName = GetDebtorName(debtorNr);
			MessageBox.Show(string.Format("Debtor {0}: {1}", debtorNr, debtorName));
		}

		public string GetDebtorName(string debtorNr)
		{
			CustomerList(true, false, "");
			string filename = GetDirectory("Customers") + @"\CustomerList.xml";
			string s = "";

			try
			{
				FileStream fsReadXml = new FileStream(filename, FileMode.Open);
				XmlTextReader myXmlReader = new System.Xml.XmlTextReader(fsReadXml);
				DataSet ds = new DataSet("Customer DataSet");
				ds.ReadXml(myXmlReader);
				myXmlReader.Close();

				bool found = false;
				foreach(DataTable dt in ds.Tables)
				{
					foreach(DataRow dr in dt.Rows)
					{
						foreach(DataColumn dc in dt.Columns)
						{
							s = StringTools.AllTrim((string)dr[dc]);
							if (dc.ColumnName == "DebtorNr" && debtorNr == s)
								found = true;
							if (found && dc.ColumnName == "Name")
								return s;
						}
					}
				}
			}
			catch 
			{
				MessageBox.Show("Unable to read CustomerList file");
				throw;
			}

			return "<Unknown>";
		}

		public void Licenses(Options option)
		{
			Shell.Excel(@"N:\Dep_Product_Management\Licences & Configurations\Licenses\Licenses.xls", false, false);
			Shell.ExplorerPath(@"N:\Dep_Product_Management\Licences & Configurations\Licenses\External Licenses", false, false);
		}

		public void ReadmeFilelist(Options option)
		{
			SortedList argOptions = new SortedList();
			argOptions["-D"] = "";		// -D {Deployment Directory}
			CommandLine.EvalCommandLine(ref argOptions);
			string	directory	= (string)argOptions["-D"];

			if (directory.Length > 0 && !Directory.Exists(directory))
				throw new Exception("Directory " + directory + " doesn't exist");
			else
				directory = Environment.CurrentDirectory;

			SortedList files = new SortedList();
			DirectoryTools.FindFiles(ref files, directory, "", true);

			StringBuilder text = new StringBuilder();
			text.AppendFormat(@"Insert the following text in the README.TXT for directory:
  {0}

--------------
DELIVERABLE(S)
--------------

", directory);

			int iDirCnt = 0;
			string lastDir = "~";
			IDictionaryEnumerator fileEnum = null;
			for (int i = 1; i <= 2; i++) {
				fileEnum = files.GetEnumerator();
				while (fileEnum.MoveNext()) {
					bool process = false;
					string file = (string)fileEnum.Key;
					string filename = file.Substring(file.LastIndexOf('\\') + 1);
					string dir = file.Substring(directory.Length + 1, file.Length - (directory.Length + 1) - filename.Length);
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
								string sDirCnt = (++iDirCnt).ToString() + ".    ";
								text.AppendFormat(@"{0}{1} folder
", sDirCnt.Substring(0, 4), dir);
								lastDir = dir;
							}
							DateTimeComponents dtc = new DateTimeComponents(File.GetLastWriteTime(file));
							text.AppendFormat(
								@"    - Filename: {0}
    - Filedate: {1} {2}

", filename, dtc.FormattedDate, dtc.FormattedTime);
						}
					}
				}
			}

			string readmePath = FileTools.WriteTempFile("ReadmeFilelist.txt", text.ToString());
			Shell.Notepad(readmePath, false, false);
		}

		public void RequestDebtorNr(Options option)
		{
			Shell.MailTo("kbenhaddaoui", "jhodes", "", "Request for new Debtor number...", @"Beste Khalid,

Can you please allocate a new Debtor number for me?

---Basis
Naam:

---Algemeen
Adres Land:
Adres 1:
Adres 2:
Adres 3:
Postcode:
Plaats:
BTW-nummer:
[Telefoon:]
[Fax:]

---Contact
Naam:
Functieomschrijving:
[Telefoon:]
[E-mail:]

[Notities:]


Thanks and all the best,

Angela.");
		}

		public string	RequestLicenseSyntax(bool syntaxOnly, bool switchesOnly) { return "[-C {DebtorNr}]"; }
		public void		RequestLicense(Options option)
		{
			SortedList argOptions = new SortedList();
			argOptions["-C"] = "";		// [{-C DebtorNr}]
			CommandLine.EvalCommandLine(ref argOptions);
			string	debtorNr	= (string)argOptions["-C"];

			string requestForm = null;
			string[] paths;

			if (debtorNr.Length > 0) {
				requestForm = "Compliance Manager license request for " + debtorNr + ".xls";
				Shell.Excel(GetDirectory("Licenses") + @"\Licenses.xls", false, false);
				paths = Directory.GetDirectories(GetDirectory("Licenses"), debtorNr + "*");
				foreach (string dir in paths)
					Shell.ExplorerPath(dir, false, false);
			}
			else {
				requestForm = "Compliance Manager license request form (version 1.3).xls";
			}

			paths = Directory.GetFiles(GetDirectory("Licenses"), "Compliance Manager license request form*.xls");
			string filename = "";
			foreach (string path in paths) {
				filename = path;
			}
			string dst = GetDirectory("Temp") + @"\" + requestForm;
			File.Copy(filename, dst, true);
			Shell.Excel(dst, false, false);
			Shell.MailTo("chagenaars;shempenius", "chagenaars", "", "Request for License...", @"Beste Carlos/Sikke,

Can you please provide a license based on the attached request form?

Thanks and all the best,

Richard.
" + dst);
		}

		public void RequestOfficeSupplies(Options option)
		{
			Shell.MailTo("secretariaat@edrcreditservices.nl", "", "", "Request for Office Supplies...", @"Beste Secretariaat,

Can you please provide the following office supplies:

!!! Fill out Request Form Office Supplies - Copy and paste here !!!

Thanks and all the best,

Angela.");
			Shell.Browser(@"http://net.intranet.nl/showprocedure.asp?filename=net/downloads/request form office supplies.xls", false, false);
		}

		public void RequestRefNr(Options option)
		{
			Shell.MailTo("Secretariaat", "", "", "Request for reference number...", @"!!!Attach quotation!!!
Beste Secretariaat,

Can you please assign a reference number to the attached quotation?
Please print TWO copies on NetEconomy letterhead and e-mail the updated document back to me.

Thanks and all the best,

Angela.");
			string filename = FileTools.WriteTempFile("CheckList.txt",
				@"
Request for reference number check list:

_X_ Request reference number from Secretariaat (2 copies and return updated doc by e-mail).
___ Replace document on N:\Dep_Implementation with copy from Secretariaat (copy with reference number).
___ Pick up two copies at reception downstairs.
___ Ensure there's a reference number in the printed copies.
___ Sign both copies.
___ Scan signed copy and store on Dep_Implementation.
___ Give both copies (and any attachments) to reception to be mailed.");
			Shell.Notepad(filename, false, false);
		}

		public void ShowInvoices(Options option)
		{
			ShowInvoices();
		}

		public void ShowInvoices()
		{
			string dstFile = @"N:\Reporting\Invoices overview\Invoices.htm";

			SortedList customerList = CustomerList(true, false, "");

			string invoicesDir = GetDirectory("Invoices");

			SortedList files = new SortedList();
			string[] directories = Directory.GetFiles(invoicesDir, "INV-*");
			foreach (string directory in directories) {
				string dir = directory.Substring(directory.LastIndexOf('\\') + 1);
				if (dir.ToUpper() != "INV-THUMBS.DB") {
					DateTimeComponents dtc = new DateTimeComponents(File.GetLastWriteTime(directory));
					string fmt = String.Format("{0:00000} {1:000000} {2} {3}", 
						dir.Substring(11, 5), 
						dir.Substring(4, 6), 
						dtc.FormattedDate, 
						dir);
					files[fmt] = fmt;
				}
			}

			string dst = invoicesDir + "\\";
			string html = @"<HTML>
<BODY>
<H1>Invoices:</H1>
<BR>
";
			string lastDebtorNr = "~";
			for (int i = 0; i < files.Count; i++) {
				string data = (string)files.GetByIndex(i);
				string debtorNr = data.Substring(0, 5);
				string indexNr = data.Substring(6, 6);
				string date = data.Substring(13, 10);
				string filename = data.Substring(24);

				if (debtorNr != lastDebtorNr) {
					if (lastDebtorNr != "~")
						html += "</TABLE>" + Environment.NewLine;
					lastDebtorNr = debtorNr;

					string debtorName = "";
					int iDebtorNr = customerList.IndexOfKey(debtorNr);
					if (iDebtorNr != -1)
						debtorName = (string)customerList.GetByIndex(iDebtorNr) + ":";

					html += "<BR><H2>" + debtorNr + ": " + debtorName + "</H2>" + Environment.NewLine;
					html += "<TABLE>" + Environment.NewLine;
				}
				html += String.Format(@"<TR>
<TD>{0}</TD>
<TD>&nbsp;&nbsp;&nbsp;</TD>
<TD><a href='{1}\{2}'>{2}</a></TD>
</TR>
", date, invoicesDir, filename); 
			}
			html += @"</TABLE>
</BODY>
</HTML>
";
			{
				TextFileContents tfc = new TextFileContents(dstFile);
				tfc.Contents = html;
				tfc = null;
			}

			Shell.Browser(dstFile, false, false);
		}

		public void ShowContracts(Options option)
		{
			SortedList customerList = CustomerList(true, false, "");

			SortedList files = new SortedList();
			DirectoryTools.FindFiles(ref files, GetDirectory("Contracts"), "", true);

			SortedList lines = new SortedList();
			IDictionaryEnumerator fileEnum = files.GetEnumerator();
			while (fileEnum.MoveNext()) {
				string file = (string)fileEnum.Value;
				string filename = file.Substring(file.LastIndexOf('\\') + 1);

				int c = filename.IndexOf('-');
				if (c == -1)
					c = filename.IndexOf('_');
				int d = filename.IndexOf('-', c + 1);
				if (d == -1)
					d = filename.IndexOf('_', c + 1);
				if (c != -1 && d != -1 && d - c == 5) {
					string contractNr = filename.Substring(c + 1, 4);
					string debtorNr   = filename.Substring(d + 1, 5);
					if (StringTools.IsAllNumeric(contractNr) && StringTools.IsAllNumeric(debtorNr)) {
						lines[debtorNr + contractNr + filename] = file;
					}
					else {
						lines["00000" + "0000" + filename] = file;
					}
				}
				else {
					lines["00000" + "0000" + filename] = file;
				}
			}

			string html = @"<HTML>
<BODY>
<H1>Contracts</H1><BR>
";
			string lastDebtorNr = "~";
			IDictionaryEnumerator lineEnum = lines.GetEnumerator();
			while (lineEnum.MoveNext()) {
				string line = (string)lineEnum.Key;
				string debtorNr = line.Substring(0, 5);
				string filename = line.Substring(9);

				if (debtorNr != lastDebtorNr) {
					if (lastDebtorNr != "~")
						html += "</TABLE>" + Environment.NewLine;

					string debtorName = "";
					int iDebtorNr = customerList.IndexOfKey(debtorNr);
					if (iDebtorNr != -1)
						debtorName = (string)customerList.GetByIndex(iDebtorNr) + ":";

					html += "<BR><H2>" + debtorNr + ": " + debtorName + "</H2>" + Environment.NewLine;
					html += "<TABLE>" + Environment.NewLine;
					lastDebtorNr = debtorNr;
				}

				DateTimeComponents dtc = new DateTimeComponents(File.GetLastWriteTime((string)lineEnum.Value));
				html += String.Format(@"<TR>
<TD>{0}</TD>
<TD><a href='{1}'>{2}</a></TD>
</TR>
", dtc.FormattedDate, (string)lineEnum.Value, filename);
			}
			
			html += @"</TABLE>
</BODY>
</HTML>
";
			string htmlFile = FileTools.WriteTempFile("Contracts.html", html);
			Shell.Browser(htmlFile, false, false);
		}

		public string	HotfixInfo()
		{
			return @"The 'SOURCE CLEARQUEST' page is populated with issues with the word 'HOTFIX' in Assigning | Additional Release Info field.

To automatically populate the SOURCE CLEARQUEST page:
Enable automatic refresh
User:     ClearQuestRead
Password: cqr";
		}
		public void		Hotfix(Options option)
		{
			Shell.Excel(GetDirectory("Hotfix"), false, false);
//			Shell.Browser(GetDirectory("Hotfix"), false, false);
		}
		#endregion Stuff
	}
}
