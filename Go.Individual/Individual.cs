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

			category = "Active";
			AddOption(new Options(category, "SickBeard",						"", "",		"Sick Beard - TV downloads",										SickBeard, null, null));
			AddOption(new Options(category, "Backup",                           "", "",     "Backup critical files to external hard-drive J:",                  Backup, null, null));
			AddOption(new Options(category, "ConfigIndividual",                 "", "",     "Edit Go.Individual.xml and backup",                                ConfigIndividual, null, null));

			category = "File shortcuts";
			AddOption(new Options(category, "Glossary",							"", "",		"Open glossary",													Glossary, null, null));
			AddOption(new Options(category, "GetUrl",							"", "",		"Get URL contents",													GetUrl, GetUrlSyntax, null));

            category = "Information";
			AddOption(new Options(category, "Movies",                           "", "",     "Open movies files and post to internet",                           Movies, null, null));
			AddOption(new Options(category, "ShowUpdatedSites",                 "", "",     "Show only internet sites that have been updated",                  ShowUpdatedSites, ShowUpdatedSitesSyntax, null));
			AddOption(new Options(category, "Pirate",                           "", "",     "Search BinSearch.info, Yabse.com or Kat.ph for configured list of movies", Pirate, PirateSyntax, null));
            AddOption(new Options(category, "PirateBay",                        "", "",     "Open PirateBay and PirateBay proxies pages",                       PirateBay, null, null));
            AddOption(new Options(category, "FreeGeoIP",						"", "",		"Get Geo information from IP Address",								FreeGeoIP, FreeGeoIPSyntax, null));
            AddOption(new Options(category, "CheckForNewInstalledPrograms",     "", "",     "Check For New Installed Programs",                                 CheckForNewInstalledPrograms, null, null));
            
            category = "Banking";
			AddOption(new Options(category, "Banks",                            "", "",     "Open bank web pages",                                              Banks, BanksSyntax, null));

			category = "Taxes";
		    AddOption(new Options(category, "UpdateBudget",                     "", "",     "Open files for updating weekly budget",                            UpdateBudget, null, null));
		    AddOption(new Options(category, "IncomeExpenseReport",              "", "",     "Open current IncomeExpenseReport",                                 CurrentIncomeExpenseReport, null, null));
			AddOption(new Options(category, "Taxes",                            "", "",     "Open Cedi and Richard's tax spreadsheets and supporting files",    Taxes, null, null));
        }
		#endregion Initialization

		#region Test
        const string TestUrlRoot = "http://www.cooperationinternationalegeneve.ch";
        public void Test(Options option)
        {

        }

        public void CheckForNewInstalledPrograms(Options option)
        {
            var filename = "InstalledPrograms.log";
            var tempFilename = "InstalledPrograms.new";

            if (!File.Exists(filename))
            {
                var ret = MessageBox.Show("This appears to be the first time this function has been run on this computer.  Create an initial log for comparison later?",
                    "GO", MessageBoxButtons.OKCancel, MessageBoxIcon.Asterisk);
                if (ret != DialogResult.OK)
                    return;

                CheckForNewInstalledPrograms(filename);
            }
            else
            {
                var bytes = File.ReadAllBytes(filename);
                var encoding = new UTF8Encoding();
                var oldContents = encoding.GetString(bytes);
                oldContents = oldContents.Substring(oldContents.IndexOf("\n"));
                
                var newContents = CheckForNewInstalledPrograms(tempFilename);
                newContents = newContents.Substring(newContents.IndexOf("\n"));

                if (oldContents == newContents)
                {
                    MessageBox.Show("No new programs have been installed since the last time", "GO", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    File.Delete(tempFilename);
                }
                else
                {
                    var ret = MessageBox.Show("New programs have been installed since the last time.  See what's changed?", "GO", MessageBoxButtons.OKCancel, MessageBoxIcon.Asterisk);
                    if (ret == DialogResult.OK)
                    {
                        Shell.BeyondCompare(filename, tempFilename, true);
                    }
                    File.Delete(filename);
                    File.Move(tempFilename, filename);
                }
            }
        }

        private string CheckForNewInstalledPrograms(string filename)
        {
            var root = "HKEY_LOCAL_MACHINE\\SOFTWARE\\WOW6432Node\\Microsoft\\Windows\\CurrentVersion\\Uninstall";
            var keyName = "DisplayName";

            var list = RegistryRoots.GetNames(root, keyName).OrderBy(e => e).ToList();
            var sb = new StringBuilder();
            sb.AppendFormat("Installed Programs as of {0:dd-MMM-yyyy HH:mm}\n\n", DateTime.Now);
            foreach (var item in list)
            {
                sb.AppendLine(item);
            }
            var text = sb.ToString();

            if (filename != null)
            {
                var encoding = new UTF8Encoding();
                var bytes = encoding.GetBytes(text);
                File.WriteAllBytes(filename, bytes);
            }

            return text;
        }

        public void OldTest(Options option)
        {
            //const string urlFormat = "http://www.cooperationinternationalegeneve.ch/categories/non-governmental-organizations?page={0}";
            const string fileFormat = @"D:\Downloads\File_{0}.html";
            //const string filename = @"D:\Downloads\Non-governmental organizations _ Genève Internationale.html";

            //var bytes = File.ReadAllBytes(filename);
            //var contents = Encoding.UTF8.GetString(bytes);

            var organizations = new List<string>();
		    int i;
if (false)
{
      //      for (; ; i++)
		    //{
		    //    var contents = UrlTools.GetUrl(string.Format(urlFormat, i));

		    //    if (!TestParse(contents, organizations))
		    //        break;
		    //}
}
else
{
    for (i = 1; ; i++)
    {
        var file = string.Format(fileFormat, i);
        if (!File.Exists(file))
            break;
        organizations.Add(file);
    }
}

		    var sb = new StringBuilder();
		    sb.Append(GetReportLineCsvHeader());
		    //sb.AppendLine("<xml>");
            byte[] bytes;
		    i = 1;
		    foreach (var org in organizations)
		    {
		        string contents;
		        if (false)
		        {
                    //contents = UrlTools.GetUrl(TestUrlRoot + "/" + org);

                    //var startPos = contents.IndexOf("<div id=\"page\">");
                    //if (startPos >= 0)
                    //{
                    //    const string endPosContents = "<div class='afternode'>";
                    //    var endPos = contents.IndexOf(endPosContents, startPos);
                    //    if (endPos >= 0)
                    //    {
                    //        bytes = Encoding.UTF8.GetBytes(contents.Substring(startPos, endPos - startPos + endPosContents.Length));
                    //        File.WriteAllBytes(string.Format(fileFormat, i), bytes);
                    //    }
                    //}
                }
		        else
		        {
		            bytes = File.ReadAllBytes(string.Format(fileFormat, i));
		            contents = Encoding.UTF8.GetString(bytes);
		        }

		        var model = TestParseOrg(contents, i);
		        if (model == null)
		        {
		            sb.AppendLine("ERROR: Line " + i);
		        }
		        else
		        {
		            sb.Append(GetReportLineCsv(model, i));
		        }

		        i++;
		    }
            //sb.AppendLine("</xml>");

		    var retBytes = Encoding.UTF8.GetBytes(sb.ToString());
            File.WriteAllBytes(@"D:\Downloads\t.xml", retBytes);
			MessageBox.Show("Test...");
		}

	    private void WriteProperty(StringBuilder sb, string elementName, string value)
	    {
            sb.AppendFormat("\t\t<{0}>{1}</{0}>{2}", elementName, string.IsNullOrEmpty(value) ? "ERROR! Empty!" : value, Environment.NewLine);
        }
	    private string GetReportLineXml(OrganizationModel model, int recordNr)
	    {
            var sb = new StringBuilder();
	        sb.AppendLine("\t<organization>");
            WriteProperty(sb, "record", recordNr.ToString());
            WriteProperty(sb, "title", model.Title);
            WriteProperty(sb, "logoUrl", model.LogoUrl);
            WriteProperty(sb, "logoWidth", model.LogoWidth);
            WriteProperty(sb, "logoHeight", model.LogoHeight);
            WriteProperty(sb, "description", model.Description);
            WriteProperty(sb, "source", model.Source);
            WriteProperty(sb, "website", model.Website);
            WriteProperty(sb, "websiteText", model.WebsiteText);
            WriteProperty(sb, "facebookUrl", model.FacebookUrl);
            WriteProperty(sb, "facebookTitle", model.FacebookTitle);
            WriteProperty(sb, "twitterUrl", model.TwitterUrl);
            WriteProperty(sb, "twitterTitle", model.TwitterTitle);
            WriteProperty(sb, "line1", model.Line1);
            WriteProperty(sb, "street", model.Street);
            WriteProperty(sb, "locality", model.Locality);
            WriteProperty(sb, "postalCode", model.PostalCode);
            WriteProperty(sb, "country", model.Country);
            WriteProperty(sb, "phone", model.Phone);
            WriteProperty(sb, "fax", model.Fax);
            WriteProperty(sb, "mapUrl", model.MapUrl);

	        var i = 0;
	        for ( ; i < model.ThemeMapUrls.Count; i++)
	        {
	            WriteProperty(sb, "themeMapUrl" + (i + 1), model.ThemeMapUrls[i]);
                WriteProperty(sb, "themeMapText" + (i + 1), model.ThemeMapTexts[i]);
            }
	        for (; i < 5; i++)
	        {
                WriteProperty(sb, "themeMapUrl" + (i + 1), Na);
                WriteProperty(sb, "themeMapText" + (i + 1), Na);
	        }
            sb.AppendLine("\t</organization>");
            return sb.ToString();
	    }

	    private string GetReportLineCsvHeader()
	    {
            var sb = new StringBuilder();
            sb.Append("RecordNr\t");
            sb.Append("Title\t");
            sb.Append("LogoUrl\t");
            sb.Append("LogoWidth\t");
            sb.Append("LogoHeight\t");
            sb.Append("Description\t");
            sb.Append("Source\t");
            sb.Append("Website\t");
            sb.Append("WebsiteText\t");
            sb.Append("FacebookUrl\t");
            sb.Append("FacebookTitle\t");
            sb.Append("TwitterUrl\t");
            sb.Append("TwitterTitle\t");
            sb.Append("Line1\t");
            sb.Append("Street\t");
            sb.Append("Locality\t");
            sb.Append("PostalCode\t");
            sb.Append("Country\t");
            sb.Append("Phone\t");
            sb.Append("Fax\t");
            sb.Append("MapUrl\t");
            for (var i = 0; i < 5; i++)
            {
                sb.Append("ThemeMapUrl" + (i + 1) + "\t");
                sb.Append("ThemeMapText" + (i + 1) + "\t");
            }
	        sb.AppendLine("");

	        return sb.ToString();
	    }
        private string GetReportLineCsv(OrganizationModel model, int recordNr)
        {
            var sb = new StringBuilder();
            sb.Append(recordNr.ToString(CultureInfo.InvariantCulture) + "\t");
            sb.Append(model.Title + "\t");
            sb.Append(model.LogoUrl + "\t");
            sb.Append(model.LogoWidth + "\t");
            sb.Append(model.LogoHeight + "\t");
            sb.Append(model.Description + "\t");
            sb.Append(model.Source + "\t");
            sb.Append(model.Website + "\t");
            sb.Append(model.WebsiteText + "\t");
            sb.Append(model.FacebookUrl + "\t");
            sb.Append(model.FacebookTitle + "\t");
            sb.Append(model.TwitterUrl + "\t");
            sb.Append(model.TwitterTitle + "\t");
            sb.Append(model.Line1 + "\t");
            sb.Append(model.Street + "\t");
            sb.Append(model.Locality + "\t");
            sb.Append(model.PostalCode + "\t");
            sb.Append(model.Country + "\t");
            sb.Append(model.Phone + "\t");
            sb.Append(model.Fax + "\t");
            sb.Append(model.MapUrl + "\t");

            var i = 0;
            for (; i < model.ThemeMapUrls.Count; i++)
            {
                sb.Append(model.ThemeMapUrls[i] + "\t");
                sb.Append(model.ThemeMapTexts[i] + "\t");
            }
            for (; i < 5; i++)
            {
                sb.Append(Na + "\t");
                sb.Append(Na + "\t");
            }
            sb.AppendLine("");

            return sb.ToString();
        }
        public class OrganizationModel
	    {
	        public string Title { get; set; }
	        public string Source { get; set; }
	        public string LogoUrl { get; set; }
	        public string LogoWidth { get; set; }
            public string LogoHeight { get; set; }
            public string Description { get; set; }
	        public string Website { get; set; }
            public string WebsiteText { get; set; }
            public string FacebookUrl { get; set; }
            public string FacebookTitle { get; set; }
            public string TwitterUrl { get; set; }
            public string TwitterTitle { get; set; }
            public string Line1 { get; set; }
            public string Street { get; set; }
            public string PostalCode { get; set; }
            public string Locality { get; set; }
            public string Country { get; set; }
            public string Phone { get; set; }
            public string Fax { get; set; }
            public string MapUrl { get; set; }
	        public List<string> ThemeMapUrls { get; set; }
	        public List<string> ThemeMapTexts { get; set; }
	    }

        //private string GetValueFromContents(string contents, ref int startPos, string beginToken, string endToken)
        //{
        //    startPos = contents.IndexOf(beginToken, startPos);
        //    if (startPos < 0)
        //        return null;
        //    var endPos = contents.IndexOf(endToken, startPos + beginToken.Length);
        //    if (endPos < 0)
        //        return null;
        //    var value = contents.Substring(startPos + beginToken.Length, endPos - startPos - beginToken.Length);
        //    if (string.IsNullOrEmpty(value))
        //        return null;
        //    return value.Trim(new char[] {' ', '\t', '\r', '\n'});
        //}

	    private void SkipPastWhitespace(string contents, ref int startPos)
	    {
            for ( ; startPos < contents.Length && " \t\r\n".Contains(contents[startPos]); startPos++) { }
        }

	    private bool GetTextEndingWithToken(string contents, string token, ref int startPos, out string value)
	    {
	        var endPos = contents.IndexOf(token, startPos, StringComparison.Ordinal);
	        if (endPos >= 0)
	        {
	            value = contents.Substring(startPos, endPos - startPos).Trim();
	            value = System.Net.WebUtility.HtmlDecode(value);
	            if (string.IsNullOrEmpty(value))
	                value = Na;
	            startPos = endPos + token.Length;
                SkipPastWhitespace(contents, ref startPos);
	            return true;
	        }

	        value = null;
	        return false;
	    }

	    private bool ValidateNextText(string contents, string text, ref int startPos)
	    {
	        if (contents.Substring(startPos, text.Length).Equals(text))
	        {
	            startPos += text.Length;
                SkipPastWhitespace(contents, ref startPos);
	            return true;
	        }

	        return false;
	    }

        const string Na = "n/a";
	    private int themeMapCount = 0;

	    private OrganizationModel TestParseOrg(string contents, int recordNr)
	    {
	        string garbage = null;
	        string title = null;
	        string source = null;
	        string logoUrl = null;
	        string logoWidth = null;
	        string logoHeight = null;
	        //string node = null;
	        string description = null;
	        string website = null;
	        string websiteText = null;
	        string facebookUrl = null;
	        string facebookTitle = null;
	        string twitterUrl = null;
	        string twitterTitle = null;
	        string line1 = null;
	        string street = null;
	        string postalCode = null;
	        string locality = null;
	        string country = null;
	        string phone = null;
	        string fax = null;
	        string mapUrl = null;
            List<string> themeMapUrls = new List<string>();
            List<string> themeMapTexts = new List<string>();

	        var startPos = contents.IndexOf("<div id=\"page\">");
	        if (startPos < 0)
	            return null;
	        if (!ValidateNextText(contents, "<div id=\"page\">", ref startPos))
	            return null;

	        if (!ValidateNextText(contents, "<div class=\"limiter clear-block\">", ref startPos))
	            return null;
	        if (!ValidateNextText(contents, "<div id=\"main\" class=\"clear-block\">", ref startPos))
	            return null;
	        if (!ValidateNextText(contents, "<div id=\"leftcenter\">", ref startPos))
	            return null;
	        if (!ValidateNextText(contents, "<div id=\"content\" class=\"clear-block\">", ref startPos))
	            return null;

	        if (!ValidateNextText(contents, "<h1 class=\"page-title \">", ref startPos))
	        {
	            if (!ValidateNextText(contents, "<h1 class=\"page-title  main-title-with-logo\">", ref startPos))
	                return null;
	            if (
	                !GetTextEndingWithToken(contents,
	                    "<div id='navigation_link'>&nbsp;<span class='hiddenover navigation_link' id='print_link' class='navigation_link' onclick='window.print();'></span></div>",
	                    ref startPos, out title))
	                return null;
	            if (!ValidateNextText(contents, "</h1>", ref startPos))
	                return null;
	            if (!ValidateNextText(contents, "<div class='logo'><img src=\"", ref startPos))
	                return null;
	            if (
	                !GetTextEndingWithToken(contents,
	                    "\" alt=\"\" title=\"\"  class=\"imagecache imagecache-logo_organisation\" width=\"", ref startPos,
	                    out logoUrl))
	                return null;
	            if (!GetTextEndingWithToken(contents, "\" height=\"", ref startPos, out logoWidth))
	                return null;
	            if (!GetTextEndingWithToken(contents, "\" /></div>", ref startPos, out logoHeight))
	                return null;
	        }
	        else
	        {
	            logoUrl = Na;
	            logoWidth = Na;
	            logoHeight = Na;
	            if (
	                !GetTextEndingWithToken(contents,
	                    "<div id='navigation_link'>&nbsp;<span class='hiddenover navigation_link' id='print_link' class='navigation_link' onclick='window.print();'></span></div>",
	                    ref startPos, out title))
	                return null;
	            if (!ValidateNextText(contents, "</h1>", ref startPos))
	                return null;
	        }

	        if (!ValidateNextText(contents, "<div class='c_wrap'><div  id=\"node-", ref startPos))
	            return null;
	        var body = true;
	        if (
	            !GetTextEndingWithToken(contents,
	                "\" class=\"node node-organisation node-organisation-page clear-block content_body ", ref startPos,
	                out garbage))
	        {
	            if (
	                !GetTextEndingWithToken(contents,
	                    "\" class=\"node node-organisation node-organisation-page sticky clear-block content_body ",
	                    ref startPos, out garbage))
	            {
	                if (
	                    !GetTextEndingWithToken(contents,
	                        "\" class=\"node node-organisation node-organisation-page clear-block emptybody ", ref startPos,
	                        out garbage))
	                    return null;
	                body = false;
	            }
	        }
	        if (!GetTextEndingWithToken(contents, "\">", ref startPos, out garbage))
	            return null;

	        if (!ValidateNextText(contents, "<div class=\"node-content clear-block\">", ref startPos))
	            return null;

	        if (body)
	        {
	            if (!ValidateNextText(contents, "<p>", ref startPos))
	            {
	                if (!ValidateNextText(contents, "<p style=\"text-align: left;\">", ref startPos))
	                {
	                    if (
	                        !ValidateNextText(contents,
	                            "<!--[if gte mso 9]><xml> <o:OfficeDocumentSettings> <o:AllowPNG /> </o:OfficeDocumentSettings> </xml><![endif]--> <p class=\"MsoNormal\"><span style=\"font-family: \"Arial\",\"sans-serif\"; mso-ansi-language: EN-AU;\">",
	                            ref startPos))
	                        return null;
	                }
	            }
	            if (!GetTextEndingWithToken(contents, "</p>", ref startPos, out description))
	                return null;
	            description = description
	                .Replace("<span style=\"white-space: pre;\">", "")
	                .Replace("<span> </span>", "")
	                .Replace("<span>", "")
	                .Replace("</span>", "")
	                .Replace("<br />", "")
	                .Trim();
	            ValidateNextText(contents, "<p class='empty_para'>&nbsp;</p>", ref startPos);
	            ValidateNextText(contents, "<p><span><br /></span></p>", ref startPos);
	            if (ValidateNextText(contents, "<!--[if gte mso 9]>", ref startPos))
	            {
	                if (!GetTextEndingWithToken(contents, "</p>", ref startPos, out garbage))
	                    return null;
	            }
	        }
	        else
	        {
	            description = Na;
	        }

	        if (ValidateNextText(contents, "<div class=\"field field-type-text field-field-source\">", ref startPos))
	        {
	            if (!ValidateNextText(contents, "<div class=\"field-items\">", ref startPos))
	                return null;
	            if (!ValidateNextText(contents, "<div class=\"field-item odd\">", ref startPos))
	                return null;
	            if (!ValidateNextText(contents, "<div class=\"field-label-inline-first\">", ref startPos))
	                return null;
	            if (!ValidateNextText(contents, "Source:&nbsp;</div>", ref startPos))
	                return null;
	            if (!ValidateNextText(contents, "<span class='item_content'>", ref startPos))
	                return null;
	            if (!GetTextEndingWithToken(contents, "</span>", ref startPos, out source))
	                return null;
	            if (!ValidateNextText(contents, "</div>", ref startPos))
	                return null;
	            if (!ValidateNextText(contents, "</div>", ref startPos))
	                return null;
	            if (!ValidateNextText(contents, "</div>", ref startPos))
	                return null;
	        }
	        else
	        {
	            source = Na;
	        }

	        if (ValidateNextText(contents, "<div class=\"field field-type-link field-field-link\">", ref startPos))
	        {
	            if (!ValidateNextText(contents, "<div class=\"field-items\">", ref startPos))
	                return null;
	            if (!ValidateNextText(contents, "<div class=\"field-item odd\">", ref startPos))
	                return null;
	            if (!ValidateNextText(contents, "<div class=\"field-label-inline-first\">", ref startPos))
	                return null;
	            if (!ValidateNextText(contents, "Website:&nbsp;</div>", ref startPos))
	                return null;
	            if (!ValidateNextText(contents, "<span class='item_content'>", ref startPos))
	                return null;
	            if (!ValidateNextText(contents, "<a href=\"", ref startPos))
	                return null;
	            if (!GetTextEndingWithToken(contents, "\"", ref startPos, out website))
	                return null;
	            ValidateNextText(contents, "target=\"_blank\"", ref startPos);
	            ValidateNextText(contents, "rel=\"nofollow\"", ref startPos);
	            if (!ValidateNextText(contents, ">", ref startPos))
	                return null;
	            if (!GetTextEndingWithToken(contents, "</a>", ref startPos, out websiteText))
	                return null;

	            if (!ValidateNextText(contents, "</span>", ref startPos))
	                return null;
	            if (!ValidateNextText(contents, "</div>", ref startPos))
	                return null;
	            if (!ValidateNextText(contents, "</div>", ref startPos))
	                return null;
	            if (!ValidateNextText(contents, "</div>", ref startPos))
	                return null;
	        }
	        else
	        {
	            website = Na;
	            websiteText = Na;
	        }

	        if (ValidateNextText(contents, "<div class=\"field field-type-link field-field-facebook-url\">", ref startPos))
	        {
	            if (!ValidateNextText(contents, "<div class=\"field-items\">", ref startPos))
	                return null;
	            if (!ValidateNextText(contents, "<div class=\"field-item odd\">", ref startPos))
	                return null;
	            if (!ValidateNextText(contents, "<div class=\"field-label-inline-first\">", ref startPos))
	                return null;
	            if (!ValidateNextText(contents, "Facebook:&nbsp;</div>", ref startPos))
	                return null;
	            if (!ValidateNextText(contents, "<span class='item_content'>", ref startPos))
	                return null;
	            if (!ValidateNextText(contents, "<a href=\"", ref startPos))
	                return null;
	            if (
	                !GetTextEndingWithToken(contents, "\" target=\"_blank\" rel=\"nofollow\">", ref startPos,
	                    out facebookUrl))
	                return null;
	            if (!GetTextEndingWithToken(contents, "</a>", ref startPos, out facebookTitle))
	                return null;
	            if (!ValidateNextText(contents, "</span>", ref startPos))
	                return null;
	            if (!ValidateNextText(contents, "</div>", ref startPos))
	                return null;
	            if (!ValidateNextText(contents, "</div>", ref startPos))
	                return null;
	            if (!ValidateNextText(contents, "</div>", ref startPos))
	                return null;
	        }
	        else
	        {
	            facebookUrl = Na;
	            facebookTitle = Na;
	        }

	        if (ValidateNextText(contents, "<div class=\"field field-type-link field-field-twitter-url\">", ref startPos))
	        {
	            if (!ValidateNextText(contents, "<div class=\"field-items\">", ref startPos))
	                return null;
	            if (!ValidateNextText(contents, "<div class=\"field-item odd\">", ref startPos))
	                return null;
	            if (!ValidateNextText(contents, "<div class=\"field-label-inline-first\">", ref startPos))
	                return null;
	            if (!ValidateNextText(contents, "Twitter:&nbsp;</div>", ref startPos))
	                return null;
	            if (!ValidateNextText(contents, "<span class='item_content'>", ref startPos))
	                return null;
	            if (!ValidateNextText(contents, "<a href=\"", ref startPos))
	                return null;
	            if (
	                !GetTextEndingWithToken(contents, "\" target=\"_blank\" rel=\"nofollow\">", ref startPos,
	                    out twitterUrl))
	                return null;
	            if (!GetTextEndingWithToken(contents, "</a>", ref startPos, out twitterTitle))
	                return null;
	            if (!ValidateNextText(contents, "</span>", ref startPos))
	                return null;
	            if (!ValidateNextText(contents, "</div>", ref startPos))
	                return null;
	            if (!ValidateNextText(contents, "</div>", ref startPos))
	                return null;
	            if (!ValidateNextText(contents, "</div>", ref startPos))
	                return null;
	        }
	        else
	        {
	            twitterUrl = Na;
	            twitterTitle = Na;
	        }

	        if (!ValidateNextText(contents, "<h3 class='location'>Address</h3>", ref startPos))
	            return null;
	        if (!ValidateNextText(contents, "<div class=\"location vcard\"><div class=\"adr\">", ref startPos))
	            return null;

	        string startAddress;
	        if (GetTextEndingWithToken(contents, "<span class=\"postal-code\">", ref startPos, out startAddress))
	        {
	            var sPos = 0;
	            if (!GetTextEndingWithToken(startAddress, "<div class=\"street-address\">", ref sPos, out line1))
	            {
	                line1 = startAddress;
	                street = Na;
	            }
	            else
	            {
	                if (!GetTextEndingWithToken(startAddress, "</div>", ref sPos, out street))
	                    street = Na;
	            }
	        }
	        else
	        {
	            line1 = Na;
	            street = Na;
	        }

	        if (!GetTextEndingWithToken(contents, "</span>", ref startPos, out postalCode))
	            return null;

	        if (!ValidateNextText(contents, "<span class=\"locality\">", ref startPos))
	            return null;
	        if (!GetTextEndingWithToken(contents, "</span>", ref startPos, out locality))
	            return null;

	        if (!ValidateNextText(contents, "<div class=\"country-name\">", ref startPos))
	            return null;
	        if (!GetTextEndingWithToken(contents, "</div>", ref startPos, out country))
	            return null;

	        if (ValidateNextText(contents, "<div class=\"phone\">Tel:", ref startPos))
	        {
	            if (!GetTextEndingWithToken(contents, "</div>", ref startPos, out phone))
	                return null;
	        }
	        else
	        {
	            phone = Na;
	        }

	        if (ValidateNextText(contents, "<div class=\"fax\">Fax:", ref startPos))
	        {
	            if (!GetTextEndingWithToken(contents, "</div>", ref startPos, out fax))
	                return null;
	        }
	        else
	        {
	            fax = Na;
	        }
	        if (!ValidateNextText(contents, "</div>", ref startPos))
	            return null;
	        if (!ValidateNextText(contents, "</div>", ref startPos))
	            return null;

	        if (recordNr == 10)
	        {
	        }
	        if (
	            !ValidateNextText(contents,
	                "<h3 class='map_view_container'>Map view</h3><div class='map_link_container'><a href=\"", ref startPos))
	            return null;
	        if (
	            !GetTextEndingWithToken(contents,
	                "\" class=\"map_link categ_map_link\">View this organization on the map</a>", ref startPos, out mapUrl))
	            return null;
	        if (mapUrl != Na)
	            mapUrl = TestUrlRoot + mapUrl;

	        for (; ;)
	        {
	            ValidateNextText(contents, "<br>", ref startPos);
	            ValidateNextText(contents, ",", ref startPos);
                if (ValidateNextText(contents, "</div>", ref startPos))
                    break;
                if (!ValidateNextText(contents, "<a href=\"", ref startPos))
	                return null;
	            string themeMapUrl;
                if (!GetTextEndingWithToken(contents, "\" class=\"map_link theme_map_link\" title=\"See all organizations with similar themes on the map\">", ref startPos, out themeMapUrl))
	            {
	                if (!GetTextEndingWithToken(contents, "\" class=\"map_link theme_map_link\" title=\"See all organisations with similar themes on the map\">", ref startPos, out themeMapUrl))
                        return null;
	            }
	            if (themeMapUrl != Na)
	                themeMapUrl = TestUrlRoot + themeMapUrl;
                themeMapUrls.Add(themeMapUrl);
	            string themeMapText;
                if (!GetTextEndingWithToken(contents, "</a>", ref startPos, out themeMapText))
                    return null;
                themeMapTexts.Add(themeMapText);
	        }

            if (!ValidateNextText(contents, "</div>", ref startPos))
	            return null;
	        if (!ValidateNextText(contents, "</div>", ref startPos))
	            return null;
	        if (!ValidateNextText(contents, "<div class='afternode'>", ref startPos))
	            return null;

            return new OrganizationModel
	        {
	            Title = title,
	            Source = source,
	            LogoUrl = logoUrl,
                LogoWidth = logoWidth,
                LogoHeight = logoHeight,
	            Description = description,
	            Website = website,
                WebsiteText = websiteText,
	            FacebookUrl = facebookUrl,
                FacebookTitle = facebookTitle,
                TwitterUrl = twitterUrl,
                TwitterTitle = twitterTitle,
                Line1 = line1,
                Street = street,
                PostalCode = postalCode,
                Locality = locality,
                Country = country,
                Phone = phone,
                Fax = fax,
                MapUrl = mapUrl,
                ThemeMapUrls = themeMapUrls,
                ThemeMapTexts = themeMapTexts
	        };
	    }
	    private bool TestParse(string contents, List<string> organizations)
	    {
            // Get list of links.
            var startPos = -1;
            do
            {
                startPos = contents.IndexOf("node-organisation-teaser", startPos + 1, StringComparison.Ordinal);
                if (startPos < 0)
                    break;
                var linkPos = contents.IndexOf("<a", startPos, StringComparison.Ordinal);
                if (linkPos >= 0)
                {
                    linkPos = contents.IndexOf("href=\"", linkPos, StringComparison.Ordinal);
                    if (linkPos >= 0)
                    {
                        var endPos = contents.IndexOf("\"", linkPos + 6, StringComparison.Ordinal);
                        if (endPos >= 0)
                        {
                            var url = contents.Substring(linkPos + 6, endPos - linkPos - 6);
                            if (organizations.Contains(url))
                                return false;
                            organizations.Add(url);
                        }
                    }
                }
            } while (true);

	        return true;
	    }
		#endregion Test

		#region Active

		public void SickBeard(Options option)
		{
			var path = @"C:\Program Files (x86)\SickBeard-win32-alpha-build502\SickBeard.exe";
			Shell.ShellApp(path, null, false, false);
		}

		public void Backup(Options option)
		{
			var pea = new ProgressEventArgs(1, false, "Source:", "Destination:");
			_progressEvent.OnProgress(pea);
			var copiedFiles = 0;
			var replacedFiles = 0;

			DirectoryTools.SynchronizeTree(@"D:\Documents", @"J:\Backup\Rozsa-Server\D_DRIVE\Documents", ref copiedFiles, ref replacedFiles, _progressEvent, pea);
			DirectoryTools.SynchronizeTree(@"D:\Internet", @"J:\Backup\Rozsa-Server\D_DRIVE\Internet", ref copiedFiles, ref replacedFiles, _progressEvent, pea);
			DirectoryTools.SynchronizeTree(@"D:\Music", @"J:\Music", ref copiedFiles, ref replacedFiles, _progressEvent, pea);
			DirectoryTools.SynchronizeTree(@"D:\Pictures", @"J:\Pictures", ref copiedFiles, ref replacedFiles, _progressEvent, pea);

			_progressEvent.OnProgress(pea.Reset());
			MessageBox.Show(string.Format("{0} files copied, {1} files replaced", copiedFiles, replacedFiles));
		}

		public void ConfigIndividual(Options option)
		{
            Shell.OpenAndBackupConfigurationFile("Go.Individual.xml");
		}

		#endregion Active

		#region File shortcuts

		public void Glossary(Options option)
		{
			Shell.Word(GetDirectory("MyDocuments") + @"\Glossary.doc", false, false);
		}

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

        public void Movies(Options option)
		{
			Shell.Excel(@"D:\Projects\_WebSites\www.rozsa.net\family\richard\Movies.xls", false, false);
			Shell.Notepad(@"D:\Projects\_WebSites\www.rozsa.net\videos.html", false, false);
			Shell.Notepad(@"D:\Projects\_WebSites\www.rozsa.net\family\richard\Movies.html", true, false);
			string ftpScript = FileTools.WriteTempFile("UploadMovies.ftp",
@"open rozsa.net
rozsa
Bismillah!
cd www
lcd D:\Projects\_WebSites\www.rozsa.net
put videos.html
cd family/richard
lcd D:\Projects\_WebSites\www.rozsa.net\family\richard
put movies.html
quit
");
			string output;
			Shell.ShellCmd("ftp.exe", string.Format("-s:\"{0}\"", ftpScript), false, out output);
			File.Delete(ftpScript);
			Shell.Browser("http://www.rozsa.net/family/richard/movies.html", false, false);
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
                    }
                }
                catch (Exception ex)
                {
                    found = false;
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

		#region Banking

		public string BanksSyntax(bool syntaxOnly, bool switchesOnly)
		{
			const string syntax = "[-W]";

			if (switchesOnly)
				return syntax;
			return syntax + @"

-W   :: Websites only.";
		}
		public void Banks(Options option)
		{
			var argOptions = new SortedList();
			argOptions["-W"] = false;   // -W : Websites only
			CommandLine.EvalCommandLine(ref argOptions);
			var websitesOnly = (bool)argOptions["-W"];

            //Shell.Browser(new[] {
            //    FirstTech(),
            //    AbnAmro(),
            //    AbnAmroMastercard(),
            //    Amex(),
            //    PayPal()
            //}, false, false);

			if (!websitesOnly)
			{
			    Shell.Quicken(false, false);
				Shell.ExplorerPath(@"D:\Downloads", false, false);
				Shell.Excel(@"D:\Documents\BUDGET\20070514 Financial State.xls", false, false);

				// Shell.Excel(@"D:\Documents\BUDGET\Cedi's Quarterly Tax Calculator.xlsx", false, false);
				Shell.Excel(@"D:\Documents\BUDGET\Richard's Quarterly Tax Calculator.xlsx", false, false);
			}
		}

        #endregion Banking

        #region Taxes
        public void UpdateBudget(Options option)
        {
            Shell.Quicken(false, true);
            CreateAndOpenCurrentIncomeExpenseReport();
            CreateAndOpenBudgetDirectory();
        }

	    private void CreateAndOpenBudgetDirectory()
	    {
	        const string yearSearch = "????";
	        var thisYear = string.Format("{0:yyyy}", DateTime.Now);
	
            // Get Budget directory from configuration.
            var path = GetDirectory("BudgetDir");
	        if (string.IsNullOrEmpty(path))
	        {
	            MessageBox.Show("BudgetDir location is not defined in Individual.xml", "GO", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
	            return;
	        }

            // Get current year directory and create if necessary.
	        var yearDirectories = Directory.GetDirectories(path, yearSearch).OrderByDescending(e => e).ToList();
	        var yearDirectory = yearDirectories.SingleOrDefault(e => e.EndsWith("\\" + thisYear));
	        if (string.IsNullOrEmpty(yearDirectory))
	        {
	            yearDirectory = Path.Combine(path, thisYear);
	            Directory.CreateDirectory(yearDirectory);
	        }

	        Shell.ExplorerPath(yearDirectory, false, false);
	    }

	    public void CurrentIncomeExpenseReport(Options option)
	    {
	        CreateAndOpenCurrentIncomeExpenseReport();
	    }

        private void CreateAndOpenCurrentIncomeExpenseReport()
	    {
            const string yearSearch = "????";
            const string incomeExpenseReportSearch = "RRC Solutions - Income-Expense Report ????-??.xlsx";
            var thisYear = string.Format("{0:yyyy}", DateTime.Now);
            var incomeExpenseReportFilename = string.Format("RRC Solutions - Income-Expense Report {0:yyyy}-{0:MM}.xlsx", DateTime.Now);

            // Get Taxes directory from configuration.
            var path = GetDirectory("BudgetTaxesDir");
            if (string.IsNullOrEmpty(path))
            {
                MessageBox.Show("BudgetTaxesDir location is not defined in Individual.xml", "GO", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                return;
            }

            // Get current year directory and create if necessary.
            var yearDirectories = Directory.GetDirectories(path, yearSearch).OrderByDescending(e => e).ToList();
            var yearDirectory = yearDirectories.SingleOrDefault(e => e.EndsWith("\\" + thisYear));
            if (string.IsNullOrEmpty(yearDirectory))
            {
                yearDirectory = Path.Combine(path, thisYear);
                Directory.CreateDirectory(yearDirectory);
            }

            // Ensure incomeExpenseReportFilename exists.
            var currentYearFilePath = Path.Combine(yearDirectory, incomeExpenseReportFilename);
            if (!File.Exists(currentYearFilePath))
            {
                // Copy the latest version available.
                var copied = false;
                foreach (var yearDir in yearDirectories)
                {
                    var lastFile = Directory.GetFiles(yearDir, incomeExpenseReportSearch).OrderByDescending(e => e).FirstOrDefault(e => !string.IsNullOrEmpty(e));
                    if (lastFile != null)
                    {
                        File.Copy(lastFile, currentYearFilePath);
                        copied = true;
                        break;
                    }
                }

                if (!copied)
                {
                    MessageBox.Show("No file found with search string: " + incomeExpenseReportSearch, "GO", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    return;
                }
            }

            Shell.Excel(currentYearFilePath, false, true);
        }

        public void Taxes(Options option)
		{
			Shell.Quicken(false, false);
			Shell.Excel(@"D:\Documents\BUDGET\Cedi's Quarterly Tax Calculator.xlsx", false, false);
			Shell.Excel(@"D:\Documents\BUDGET\Richard's Quarterly Tax Calculator.xlsx", false, false);
		}
		#endregion Taxes
	}
}
