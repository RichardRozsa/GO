using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web.Script.Serialization;
using System.Windows.Forms;
using System.Xml.Schema;
using Go.Configuration;
using Go.Tools;

namespace Go
{
	public class Individual : GoImplementation
	{
		#region Initialization
		static private Individual	    _individual = null;
		static private ProgressEvent    _progressEvent = null;
		
		static public Individual GetIndividual(ref SortedList options, ref SortedList directories, ref ProgressEvent pe)
		{
			_progressEvent = pe;
			return _individual ?? (_individual = new Individual(ref options, ref directories));
		}

		static public Individual GetIndividual()
		{
			if (_individual == null)
				throw new Exception("Can't instantiate Individual class without parameters");
			return _individual;
		}

		private Individual(ref SortedList options, ref SortedList directories)
			: base(ref options, ref directories)
		{
			var root = ConfigurationFile.GetLocalConfiguration(ref directories, "Go.Individual.xml");
            AddConfigurationOptions(root);
			
			var category = "Test";
			AddOption(new Options(category, "Test",								"", "",		"Test go option",													Test, null, null));

			category = "File shortcuts";
			AddOption(new Options(category, "GetUrl",							"", "",		"Get URL contents",													GetUrl, GetUrlSyntax, null));

            category = "Information";
			AddOption(new Options(category, "ShowUpdatedSites",                 "", "",     "Show only internet sites that have been updated",                  ShowUpdatedSites, ShowUpdatedSitesSyntax, null));
			AddOption(new Options(category, "Pirate",                           "", "",     "Search BinSearch.info, Yabse.com or Kat.ph for configured list of movies", Pirate, PirateSyntax, null));
            AddOption(new Options(category, "PirateBay",                        "", "",     "Open PirateBay and PirateBay proxies pages",                       PirateBay, null, null));
            AddOption(new Options(category, "FreeGeoIP",						"", "",		"Get Geo information from IP Address",								FreeGeoIP, FreeGeoIPSyntax, null));
        }

		#endregion Initialization

		#region Test

		public void Test(Options option)
        {
			var argOptions = new SortedList();
			// Define parameters with default values:
			//		argOptions["-U"] = "";      // -U : Required url.
			//		argOptions["-Q"] = false;   // -Q : Optional quiet mode
			CommandLine.EvalCommandLine(ref argOptions);
			// Extract user-provided command line options:
			//		var url = (string)argOptions["-U"];
			//		var quietMode = (bool)argOptions["-Q"];
			// Validate command line values:

			// Your test code here.
		}

		#endregion Test

		#region File shortcuts

		public string GetUrlSyntax(bool syntaxOnly, bool switchesOnly)
	    {
	        const string syntax = "-U {url} [-F {file}] [-Q]";

	        if (switchesOnly)
	            return syntax;
	        return syntax + @"
-U   :: URL.  Retrieves URL contents.
-F   :: Optional File.  If specified, writes contents to file. Otherwise, shows in browser.
-Q   :: Quite mode.";
	    }
        public void GetUrl(Options option)
	    {
	        var argOptions = new SortedList();
	        argOptions["-U"] = "";      // -U : Required url.
	        argOptions["-F"] = "";      // -F : Optional file to write URL contents to.
	        argOptions["-Q"] = false;   // -Q : Optional quiet mode
	        CommandLine.EvalCommandLine(ref argOptions);
	        var url = (string)argOptions["-U"];
	        var file = (string)argOptions["-F"];
	        var quietMode = (bool) argOptions["-Q"];

	        if (string.IsNullOrEmpty(url))
	        {
	            MessageBox.Show("Required -U <url> parameter missing.", "GO", MessageBoxButtons.OK);
                return;
            }

	        var showInBrowser = false;
	        if (string.IsNullOrEmpty(file))
	        {
	            showInBrowser = true;
	            file = @"%TEMP%\Go_GetUrl.html";
	        }
	        var expandedFile = Environment.ExpandEnvironmentVariables(file);
            
            var contents = UrlTools.GetUrl(url);
	        var contentBytes = Encoding.UTF8.GetBytes(contents);
            File.WriteAllBytes(expandedFile, contentBytes);

	        if (showInBrowser)
	        {
	            Shell.Browser(expandedFile, false, false);
	        }
            else if (!quietMode)
            {
	            MessageBox.Show(string.Format("Url {0} stored to file {1}", url, file), "Go", MessageBoxButtons.OK);
	        }
	    }

		public string ShowUpdatedSitesSyntax(bool syntaxOnly, bool switchesOnly)
		{
			const string syntax = "[-U {url}]";

			if (switchesOnly)
				return syntax;
			return syntax + @"
-U   :: Optional URL.  If no URL specified, uses configured url(s).";
		}
		public void ShowUpdatedSites(Options option)
		{
			var argOptions = new SortedList();
			argOptions["-U"] = "";   // -S : Optional url.  If no URL specified, uses configured url(s).
			CommandLine.EvalCommandLine(ref argOptions);
			var url = (string)argOptions["-U"];

			if (string.IsNullOrEmpty(url))
			{
				for (var i = 1; ; i++)
				{
					url = GetURL("ShowUpdatedSite" + i);
					if (string.IsNullOrEmpty(url))
						break;
					ShowUpdatedSiteForUrl(url);
				}
			}
			else
			{
				ShowUpdatedSiteForUrl(url);
			}
		}
		private void ShowUpdatedSiteForUrl(string url)
		{
			var html = UrlTools.GetUrl(url);
			var urlPath = GetDirectory("UpdatedSiteCacheDir") + "\\" + url.Replace(':', '_').Replace('/', '_').Replace(';', '_').Replace('?', '_').Replace('&', '_');
			if (!urlPath.ToLower().EndsWith("htm") && !urlPath.ToLower().EndsWith("html"))
				urlPath += ".html";

			var writeHtml = false;
			if (!File.Exists(urlPath))
			{
				writeHtml = true;
			}
			else
			{
				using (StreamReader sr = File.OpenText(urlPath))
				{
					string strbuf = sr.ReadToEnd();
					sr.Close();
					if (strbuf != html)
					{
						writeHtml = true;
					}
				}
			}
			if (writeHtml)
			{
				FileTools.WriteFile(urlPath, html);
				Shell.Browser(url, false, false);
			}
		}

        public void PirateBay(Options option)
        {
            var directories = new SortedList();
            var root = ConfigurationFile.GetLocalConfiguration(ref directories, "Go.Individual.xml");
            
            Shell.Browser(string.Format(root.Programs.TorrentFormatString1, string.Empty), false, false);
            Shell.Browser(root.Programs.PirateBayProxies, false, false);
        }

        public string PirateSyntax(bool syntaxOnly, bool switchesOnly)
		{
			const string syntax = "[-B] [-Y] [-D {number of days back to search}] [-W] [-G <# of tabs per browser> (default 100)] [-A]";

			if (switchesOnly)
				return syntax;
            return syntax + @"
-B   :: Search with www.BinSearch.info
-Y   :: Search with www.Yabse.com
-T   :: Search with Torrent (default: true)
-D   :: Filter age in days (ex: -D 3) (default = Show All)
-W   :: Load search items in multiple windows (rather than with tabs)
-G   :: Group (tabs per browser) (ex: -G 3) (default = 100)
-A   :: Show all search results (even if nothing found)
-C   :: Use configured torrent format string rather than getting it dynamically
-Q   :: Quiet output
-M   :: Override Monthly Search now
-S   :: Show which items would be searched for";
        }
		public void Pirate(Options option)
		{
			var argOptions = new SortedList();
			argOptions["-B"] = false; // -B : BinSearch
			argOptions["-Y"] = false; // -Y : Yabse
			argOptions["-T"] = true; // -T : Torrent
			argOptions["-D"] = 0; // -D : Days (default = Show All)
			argOptions["-W"] = false; // -W : Load comics in multiple windows
			argOptions["-G"] = 100; // -G : Group (tabs per browser)
			argOptions["-A"] = false; // -A : Show all search results (even if nothing found).
            argOptions["-C"] = false; // -C : Use configured format string rather than getting it from piratebay.
            argOptions["-Q"] = true; // -Q : Quiet output
            argOptions["-M"] = false; // -M : Override Monthly Search now
            argOptions["-S"] = false; // -S : Show which items would be searched for
			CommandLine.EvalCommandLine(ref argOptions);
			var binSearch = (bool) argOptions["-B"];
			var yabse = (bool) argOptions["-Y"];
			var torrent = (bool) argOptions["-T"];
			var days = (int) argOptions["-D"];
			var windows = (bool) argOptions["-W"];
			var tabGroup = (int) argOptions["-G"];
			var showAll = (bool) argOptions["-A"];
            var configured = (bool)argOptions["-C"];
            var quiet = (bool) argOptions["-Q"];
            var monthlyOverride = (bool) argOptions["-M"];
            var showOnly = (bool) argOptions["-S"];

			Pirate(binSearch, yabse, torrent, days, windows, tabGroup, showAll, quiet, monthlyOverride, showOnly);
		}
		public void PirateDefaults()
		{
			Pirate(false, false, true, 3, false, 100, false, true, false, false);
		}

		/*
		cat=0 url parameter controls filtering by category in the return JSON.
		The category is a 1 or 3 digit numeric code.  0 for "All" is 1 digit.  All others are 3 digits.
		The first character of the 3 digit code gives the category group, seen below in optgroup tags -
		the number being the first digit of option tags below it.
		<select name="cat" id="cat">
			<option value="0">All</option>
			<optgroup label="Audio">
			<option value="101">Music</option>
			<option value="102">Audio books</option>
			<option value="103">Sound clips</option>
			<option value="104">FLAC</option>
			<option value="199">Other</option>
			</optgroup>
			<optgroup label="Video">
			<option value="201">Movies</option>
			<option value="202">Movies DVDR</option>
			<option value="203">Music videos</option>
			<option value="204">Movie clips</option>
			<option value="205">TV shows</option>
			<option value="206">Handheld</option>
			<option value="207">HD - Movies</option>
			<option value="208">HD - TV shows</option>
			<option value="209">3D</option>
			<option value="299">Other</option>
			</optgroup>
			<optgroup label="Applications">
			<option value="301">Windows</option>
			<option value="302">Mac</option>
			<option value="303">UNIX</option>
			<option value="304">Handheld</option>
			<option value="305">IOS (iPad/iPhone)</option>
			<option value="306">Android</option>
			<option value="399">Other OS</option>
			</optgroup>
			<optgroup label="Games">
			<option value="401">PC</option>
			<option value="402">Mac</option>
			<option value="403">PSx</option>
			<option value="404">XBOX360</option>
			<option value="405">Wii</option>
			<option value="406">Handheld</option>
			<option value="407">IOS (iPad/iPhone)</option>
			<option value="408">Android</option>
			<option value="499">Other</option>
			</optgroup>
			<optgroup label="Porn">
			<option value="501">Movies</option>
			<option value="502">Movies DVDR</option>
			<option value="503">Pictures</option>
			<option value="504">Games</option>
			<option value="505">HD - Movies</option>
			<option value="506">Movie clips</option>
			<option value="599">Other</option>
			</optgroup>
			<optgroup label="Other">
			<option value="601">E-books</option>
			<option value="602">Comics</option>
			<option value="603">Pictures</option>
			<option value="604">Covers</option>
			<option value="605">Physibles</option>
			<option value="699">Other</option>
			</optgroup>
		</select>
		 */
		public void Pirate(bool binSearch, bool yabse, bool torrent, int days, bool windows, int tabGroup, bool showAll, bool quiet, bool monthlyOverride, bool showOnly)
		{
			if (!binSearch && !yabse && !torrent)
				return;

			if (days < 0)
				days = 0;

			// To determine if any results are found:
			// 1) Download page using URL like
			//      http://www.binsearch.info/?q=Antonia+Line&max=250&adv_age=1&server=
			//          or
			//      http://www.yabsearch.nl/search/Antonia+Line?co=n&results=200&sizemax=0&sizemin=0
			//    where
			//	    Antonia+Line
			//    is the search string.
			// 2) size = number of downloaded bytes.
			// 3) searchLength = number of bytes in search string.
			// 4) If no results are found, size after removing search string will be 3727 (for binsearch) and 3999 for yabnews
			//    or size - (searchLength * 3).

			var total = 10;
			var pea = new ProgressEventArgs(total, true, "Processing:", "");
			_progressEvent.OnProgress(pea);

            var processed = 0;
			_progressEvent.OnProgress(pea.UpdateProgress(++processed, "Reading program list file..."));
			var directories = new SortedList();
            var root = ConfigurationFile.GetLocalConfiguration(ref directories, "Go.Individual.xml");

            // TODO: REMOVE: _progressEvent.OnProgress(pea.UpdateProgress(++processed, $"Filtering list of {root.Programs.SearchList.Count} programs..."));
			//MessageBox.Show("pause");
			var programs = new ProgramList(root.Programs.SearchList, quiet, monthlyOverride);
			var programList = programs.ToList();
			var programCount = programs.CurrentCount;
			total = 0;
			if (binSearch)
				total += programCount;
			if (yabse)
				total += programCount;
			if (torrent)
				total += programCount;
			var pageNotFoundCount = 0;

            if (showOnly)
            {
                var tempProgramListLog = Path.Combine(System.IO.Path.GetTempPath(), "ProgramList.log");
				File.WriteAllLines(tempProgramListLog, programList);
                Shell.Notepad(tempProgramListLog, false, false);
                _progressEvent.OnProgress(pea.Reset());
                return;
            }

			// TODO: REMOVE: _progressEvent.OnProgress(pea.UpdateProgress(++processed, $"{programCount} programs to search for..."));
			//MessageBox.Show("Pause");
			_progressEvent.OnProgress(pea.Reset());
			pea = new ProgressEventArgs(total + processed, true, "Search:", "Found:");
			_progressEvent.OnProgress(pea);
			_progressEvent.OnProgress(pea.UpdateProgress(processed, "Searching for downloads..."));
			if (binSearch)
			{
				_progressEvent.OnProgress(pea.UpdateProgress(processed, "Start searching for downloads on BinSearch..."));

				var age = string.Empty;
                if (days > 0)
                    age = days.ToString(CultureInfo.InvariantCulture);

                SearchForPrograms(programList, pea, ref processed, root.Programs.BinSearchFormatString, "<i>No results</i>", null, null, "+", age, showAll, ref pageNotFoundCount);
            }
            if (yabse)
			{
				_progressEvent.OnProgress(pea.UpdateProgress(processed, "Start searching for downloads on Yabse..."));

				var age = string.Empty;
                if (days > 0)
                    age = days.ToString(CultureInfo.InvariantCulture);

                SearchForPrograms(programList, pea, ref processed, root.Programs.YabseFormatString, "Er zijn geen zoekresultaten", null, null, "+", age, showAll, ref pageNotFoundCount);
            }
            if (torrent)
			{
				_progressEvent.OnProgress(pea.UpdateProgress(processed, "Start searching for downloads on Torrents..."));

				var age = string.Empty;
                if (days > 0)
                    age = string.Format("age:{0}week", (days / 7) + 1);

                var torrentProxy = root.Programs.TorrentProxy;
                var torrentUI = root.Programs.TorrentUI;
                var torrentAPI = root.Programs.TorrentAPI;
				var torrentNotFoundMessage = root.Programs.TorrentNotFoundMessage;

                var torrentUIFormatString = torrentProxy + torrentUI;
                var torrentAPIFormatString = torrentProxy + torrentAPI;

                if (false)
                {
			        var fastestTorrentSiteContent = UrlTools.GetUrl("https://thepiratebay-proxylist.org/");
			        if (!string.IsNullOrEmpty(fastestTorrentSiteContent))
			        {
			            const string prefix = "<td class=\"url\" title=\"URL\" data-href=\"";
			            const string suffix = "\">";
			            var start = fastestTorrentSiteContent.IndexOf(prefix);
			            if (start >= 0)
			            {
			                start += prefix.Length;
			                var end = fastestTorrentSiteContent.Substring(start).IndexOf(suffix);
			                if (end >= 0)
			                {
                                torrentUIFormatString = fastestTorrentSiteContent.Substring(start, end) + "/search/{0}/0/99/0";
			                }
			            }
			        }
                }

				SearchForPrograms(programList, pea, ref processed, torrentAPIFormatString, torrentNotFoundMessage, "did not match any documents", torrentUIFormatString, "%20", age, showAll, ref pageNotFoundCount);
			}

			if (!windows && pea.SuccessCount > 0)
			{
                Shell.OpenAndBackupConfigurationFile("Go.Individual.xml");
			}

			_progressEvent.OnProgress(pea.Reset());

			if (pageNotFoundCount > 1)
			{
				MessageBox.Show(string.Format("There were {0} HTTP errors while searching.", pageNotFoundCount), "GO", MessageBoxButtons.OK);
			}
		}

        private bool IsActiveEpisode(Episode episode)
        {
            return (string.IsNullOrEmpty(episode.Status) || episode.Status == "Search") && (!episode.ReleaseDate.HasValue || episode.ReleaseDate.Value <= DateTime.Now);
        }

        private bool IsActiveMovie(Program program)
        {
            return (program.Episodes == null || program.Episodes.Count == 0);
        }

        private void SearchForPrograms(List<string> programs, ProgressEventArgs pea, ref int processed, string searchFormatString, string notFoundString1, string notFoundString2, string displayFormatString, string spaceChar, string age, bool showAll, ref int pageNotFoundCount)
        {
			foreach (var program in programs)
            {
                processed++;

				_progressEvent.OnProgress(pea.UpdateProgress(processed, program));

				SearchForProgram(pea, processed, program, searchFormatString, notFoundString1, notFoundString2, displayFormatString, spaceChar, age, showAll, ref pageNotFoundCount);
            }
		}

		private void SearchForProgram(ProgressEventArgs pea, int item, string searchText, string searchFormatString, string notFoundString1, string notFoundString2, string displayFormatString, string spaceChar, string age, bool showAll, ref int pageNotFoundCount)
        {
            if (displayFormatString == null)
                displayFormatString = searchFormatString;
            var cleanedSearchText = searchText.Replace(" ", spaceChar);
			var searchUrl = string.Format(searchFormatString, cleanedSearchText, age);
            var displayUrl = string.Format(displayFormatString, cleanedSearchText, age);
            var found = showAll;
            if (!showAll)
            {
				for (var tryCount = 0; tryCount < 20; tryCount++)
                {
					try
					{
						var content = UrlTools.GetUrl(searchUrl);

						// Test for empty string.
						if (content.Length < 4)
						{
							pageNotFoundCount++;
							found = false;
						}

						// Test for a 404 error.
						else if (content.Length > 4 && !content.StartsWith("2") && content[3] == ':')
						//else if (content.Contains("Page not found") && content.Contains("404"))
						{
							pageNotFoundCount++;
							found = false;
						}

						else
						{
							// Test for "Not Found" message(s) in content.
							if (!string.IsNullOrEmpty(notFoundString2))
								found = !(content.Contains(notFoundString1) || content.Contains(notFoundString2));
							else
								found = !content.Contains(notFoundString1);
							break;
						}
					}
					catch (Exception ex)
					{
						found = false;
					}
				}
			}
            if (!found)
            {
                _progressEvent.OnProgress(pea.UpdateProgress(item, searchText, "None"));
            }
            else
            {
                pea.SuccessCount++;
                _progressEvent.OnProgress(pea.UpdateProgress(item, searchText, "Yes"));

                if (pea.SuccessCount == 1)
                {
                    var option = GetOption("CouchPotato");
                    if (option != null)
                        option.goExecute(null);

                    var thread = new Thread(() => Shell.OpenAndBackupConfigurationFile("Go.Individual.xml"));
                    thread.Start();
                }
                Shell.Browser(displayUrl, false, false);
            }
	    }
        
        #endregion File shortcuts

        #region Information

        public string FreeGeoIPSyntax(bool syntaxOnly, bool switchesOnly)
		{
			const string syntax = "[-I <ip address>]";

			if (switchesOnly)
				return syntax;
			return syntax + @"

FreeGeoIP.net is a free service that returns Geo information from IP Addresses.  If an IP Address is not provided, information about your IP Address is returned.";
		}

		public class FreeGeoIPContent
		{
			public string ip { get; set; }
			public string country_code { get; set; }
			public string country_name { get; set; }
			public string region_code { get; set; }
			public string region_name { get; set; }
			public string city { get; set; }
			public string zipcode { get; set; }
			public string latitude { get; set; }
			public string longitude { get; set; }
			public string metro_code { get; set; }
			public string area_code { get; set; }
		}

		public void FreeGeoIP(Options option)
		{
			var argOptions = new SortedList();
			argOptions["-I"] = string.Empty;
			CommandLine.EvalCommandLine(ref argOptions);
			var ipAddress = (string) argOptions["-I"];

			var url = "http://www.freegeoip.net/json/" + ipAddress;
			var content = UrlTools.GetUrl(url);

			var jss = new JavaScriptSerializer();
			var ipContent = jss.Deserialize<FreeGeoIPContent>(content);

			var sb = new StringBuilder();
			sb.AppendLine("FreeGeoIP.net results:");
			sb.AppendLine();
			sb.Append("IP Address:\t");
			sb.AppendLine(ipContent.ip);
			sb.Append("Country Code:\t");
			sb.AppendLine(ipContent.country_code);
			sb.Append("Country Name:\t");
			sb.AppendLine(ipContent.country_name);
			sb.Append("Region Code:\t");
			sb.AppendLine(ipContent.region_code);
			sb.Append("Region Name:\t");
			sb.AppendLine(ipContent.region_name);
			sb.Append("City:\t\t");
			sb.AppendLine(ipContent.city);
			sb.Append("Zipcode:\t\t");
			sb.AppendLine(ipContent.zipcode);
			sb.Append("Latitude:\t\t");
			sb.AppendLine(ipContent.latitude);
			sb.Append("Longitude:\t");
			sb.AppendLine(ipContent.longitude);
			sb.Append("Metro Code:\t");
			sb.AppendLine(ipContent.metro_code);
			sb.Append("Area Code:\t");
			sb.AppendLine(ipContent.area_code);
			MessageBox.Show(sb.ToString(), "GO", MessageBoxButtons.OK);
		}

		#endregion Information
	}
}
