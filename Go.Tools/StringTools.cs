using System;

namespace Go.Tools
{
	public class StringTools
	{
		static public string AllTrim(string s)
		{
			s = s.Replace("\r", "");
			s = s.Replace("\n", "");
			s = s.Replace("\t", "");
			return s.Trim();
		}

		static public bool IsAllDigits(string s)
		{
			bool isDigit = (s.Length > 0);
			for (int i = 0; i < s.Length; i++)
			{
				isDigit = isDigit && Char.IsDigit(s, i);
			}
			return isDigit;
		}

		static public bool IsAllNumeric(string s)
		{
			bool isNumeric = (s.Length > 0);
			for (int i = 0; i < s.Length; i++)
			{
				isNumeric = isNumeric && (Char.IsDigit(s, i) || s[i] == '.');
			}
			return isNumeric;
		}

		static public string[] CompactStringArray(string[] tokens)
		{
			int cTokens = 0;
			foreach (string token in tokens)
			{
				if (token.Length > 0)
					cTokens++;
			}

			if (cTokens < tokens.Length)
			{
				if (cTokens == 0)
					return new string[0];

				string[] newTokens = new string[cTokens];
				int i = 0;
				foreach (string token in tokens)
				{
					if (token.Length > 0)
						newTokens[i++] = token;
				}
				return newTokens;
			}

			return tokens;
		}
		
		static public string Text2Address(string text)
		{
			string address = text;

			address = address.Replace(" ", ";");
			address = address.Replace(";;", ";");

			return address;
		}

		static public string Text2Html(string text)
		{
			string html = text;

            html = html.Replace("&", "&amp;");
            html = html.Replace("<", "&lt;");
            html = html.Replace(">", "&gt;");
			html = html.Replace("\r\n", "<BR>");
			html = html.Replace("\r", "<BR>");
			html = html.Replace("\n", "<BR>");
			html = html.Replace("\t", "&nbsp;&nbsp;&nbsp;&nbsp;");
			html = html.Replace("©", "&copy;");
			html = html.Replace("\"", "&quot;");

			return html;
		}

		static public string Text2Html(string text, bool makeSingleLine)
		{
			string html = text;

			html = html.Replace(" ", "%20");
			html = html.Replace("?", "%3f");
			html = html.Replace("\"", "%22");
			if (makeSingleLine)
			{
				html = html.Replace("\r\n", "%20");
				html = html.Replace("\r", "%20");
				html = html.Replace("\n", "%20");
				while (html.IndexOf("%20%20") != -1)
					html = html.Replace("%20%20", "%20");
			}
			else
			{
				html = html.Replace("\r\n", "%0d%0a");
				html = html.Replace("\r", "%0d%0a");
				html = html.Replace("\n", "%0d%0a");
			}

			return html;
		}

		static public void SplitPath(string path, out string drive, out string directory, out string file, out string ext)
		{
			int i;
			
			drive		= "";
			directory	= "";
			file		= "";
			ext			= "";

			if (path.Length >= 2 && path[1] == ':')
			{
				drive = path.Substring(0, 2);
				directory = path.Substring(2);
			}
			else if (path.Length >= 2 && path.Substring(0, 2) == @"\\")
			{
				i = path.IndexOf(@"\", 2);
				if (i == -1)
				{
					drive = path;
				}
				else
				{
					i = path.IndexOf(@"\", i + 1);
					if (i == -1)
					{
						drive = path;
					}
					else
					{
						drive = path.Substring(0, i);
						directory = path.Substring(i);
					}
				}
			}
			else
				directory = path;

			i = directory.LastIndexOf(@"\");
			if (i == -1)
			{
				file = directory;
				directory = "";
			}
			else
			{
				file = directory.Substring(i + 1);
				directory = directory.Substring(0, i);
			}

			i = file.IndexOf(".");
			if (i != -1)
			{
				ext = file.Substring(i);
				file = file.Substring(0, i);
			}
		}

		static public void MakePath(out string path, string drive, string directory, string directory2, string file, string ext)
		{
			string dir = "";
			if (directory != null)
				dir = directory.Trim();
			if (directory2 != null && directory2.Length > 0)
				dir += (@"\" + directory2.Trim());
 
			MakePath(out path, drive, dir, file, ext);
		}

		static public void MakePath(out string path, string drive, string directory, string file, string ext)
		{
			string localDrive		= "";
			string localDirectory	= "";
			string localFile		= "";
			string localExt			= "";

			if (drive != null)
				localDrive		= drive.Trim();
			if (directory != null)
				localDirectory	= directory.Trim();
			if (file != null)
				localFile		= file.Trim();
			if (ext != null)
				localExt		= ext.Trim();

			path = "";
			if (localDrive.Length > 0)
				path += localDrive;
			if (localDirectory.Length > 0)
			{
				if (	path.Length > 0 && path.Substring(path.Length - 1, 1) != @":"
					&&	localDirectory.Substring(0, 1) != @"\")
					path += @"\";
				path += localDirectory;
			}
			if (localFile.Length > 0)
			{
				if (	path.Length > 0 && path.Substring(path.Length - 1, 1) != @"\" 
					&&	localFile.Substring(0, 1) != @"\")
					path += @"\";
				path += localFile;
			}
			if (localExt.Length > 0)
			{
				if (!path.EndsWith(".") && !localExt.StartsWith("."))
					path += ".";
				if (path.EndsWith(".")	&& localExt.StartsWith("."))
				{
					if (localExt.Length > 1)
						path += localExt.Substring(1);
					else
						path += localExt;
				}
				else
					path += localExt;
			}
		}
	}
}
