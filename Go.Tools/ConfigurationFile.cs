using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Go.Configuration;
using Microsoft.Win32;

namespace Go.Tools
{
	public class ConfigurationFile
	{
		static public Root GetLocalConfiguration(ref SortedList directories, string filename)
		{
			string goXmlFilename = Path.Combine(Application.StartupPath, filename);

			try
			{
				// Read configuration file and get list of paths.
			    var root = Configuration.Configuration.GetConfiguration(goXmlFilename);
				var items = root.LocationAndValueSections.Select(s => s.Items);
				foreach (var item in items)
			    {
			        var pathItems = item.Where(i => !string.IsNullOrEmpty(i.Path));
			        foreach (var pathItem in pathItems)
			        {
			            directories[pathItem.Name] = pathItem.Path;
			        }
			    }

				//var fsReadXml = new FileStream(goXmlFilename, FileMode.Open);
				//var myXmlReader = new XmlTextReader(fsReadXml);
				//var ds = new DataSet("Go DataSet");
				//ds.ReadXml(myXmlReader);
				//myXmlReader.Close();

				//foreach(DataTable dt in ds.Tables)
				//{
				//    foreach(DataRow dr in dt.Rows)
				//    {
				//        foreach(DataColumn dc in dt.Columns)
				//        {
				//            directories[dc.ColumnName] = StringTools.AllTrim(dr[dc].ToString());
				//        }
				//    }
				//}

				// Expand Aliases, Registry keys, Environment variables and OR conditions until all resolved.
				bool driveLettersFirst = true;
				for (bool changed = true; changed; )
				{
					changed = false;

					IDictionaryEnumerator dirEnumerator = directories.GetEnumerator();
					while (dirEnumerator.MoveNext())
					{
						var	alias		= (string)dirEnumerator.Key;
						var	directory	= (string)dirEnumerator.Value;
						var thisChanged = false;

						// Expand embedded environment variables.
						while (directory.IndexOf("%", StringComparison.Ordinal) != -1)
						{
							string newDir = Environment.ExpandEnvironmentVariables(directory);
							if (String.Compare(newDir, directory, StringComparison.Ordinal) == 0)
								break;
							directory = newDir;
							thisChanged = true;
							changed = true;
						}

						// Expand embedded Aliases and Registry values.
					    while (true) 
						{
						    int openPos;
						    int closePos;
						    if (!FindOpenCloseTokens(directory, out openPos, out closePos))
								break;
							string token = GetToken(directory, openPos, closePos);

							// Before processing other tokens, first handle OR'd expressions
							// with first token that begins with a constant drive letter.
							// This is so the Directory.Exists check won't repeat more than 1x
							// as checking a network drive when off-line is expensive.
							int orPos;
							if (driveLettersFirst)
							{
								orPos = token.IndexOf("||", StringComparison.Ordinal);
								if (orPos == -1 || token.Substring(1, 1) != ":")
									break;
								if (!FindOuterMostCloseToken(directory, openPos, ref closePos))
									break;
							}
							else
							{
								FindInnerMostOpenToken(directory, ref openPos, closePos);
							}
							token = GetToken(directory, openPos, closePos);

							string newValue	= null;

							// Get Registry value.
							var tokenIsRegistryKey = false;
							var keyPath	= token;
							RegistryKey	regRoot;
							if (RegistryRoots.GetRoot(ref keyPath, out regRoot))
							{
								try {
									tokenIsRegistryKey = true;
									var keyName = keyPath.Substring(keyPath.LastIndexOf("\\", StringComparison.Ordinal) + 1);
									keyPath = keyPath.Substring(0, keyPath.Length - keyName.Length - 1);

									var rkey = regRoot.OpenSubKey(keyPath);
									newValue = (rkey == null)
                                        ? ""
                                        : rkey.GetValue(keyName)?.ToString();
								}
								catch(Exception) {
									newValue = "";
								}
							}

							// Get Alias (with OR handling) value.
							if (!tokenIsRegistryKey)
							{
								// Get the token value.
								if (directories.Contains(token))
									newValue = (string)directories[token];
								else
								{
									// Evaluate OR'd paths (if 1st exists, use it. Otherwise use 2nd)
									orPos = token.IndexOf("||", StringComparison.Ordinal);
									if (orPos != -1)
									{
										string dirChoice1 = token.Substring(0, orPos).Trim();
										string dirChoice2 = token.Substring(orPos + 2).Trim();
										if		(dirChoice2.Length == 0)
											newValue = dirChoice1;
										else if (dirChoice1.Length == 0)
											newValue = dirChoice2;
										else if (Directory.Exists(dirChoice1))
											newValue = dirChoice1;
										else
											newValue = dirChoice2;
									}
								}
							}

							if (newValue == null)
								break;
							directory = ReplaceTokenWithValue(directory, newValue, openPos, closePos);
							thisChanged = true;
							changed = true;
						}

						if (thisChanged)
						{
							directories[alias] = directory;
							break;
						}
					}
					if (driveLettersFirst && !changed)
					{
						driveLettersFirst = false;
						changed = true;
					}
				}

				return root;
			}
			catch (Exception ex)
			{
				MessageBox.Show(string.Format("Unable to read configuration file [{0}] ({1})", goXmlFilename, ex.Message));
				throw;
			}
		}

		static private bool FindOpenCloseTokens(string line, out int openPos, out int closePos)
		{
			closePos = -1;
			openPos = line.IndexOf("{", StringComparison.Ordinal);
			if (openPos == -1)
				return false;
			closePos = line.IndexOf("}", openPos + 1, StringComparison.Ordinal);
			return (closePos != -1);
		}

		static private void FindInnerMostOpenToken(string line, ref int openPos, int closePos)
		{
			for ( ; ; )
			{
				int i = line.IndexOf("{", openPos + 1, closePos - openPos - 1, StringComparison.Ordinal);
				if (i == -1)
					break;
				openPos = i;
			}
		}

		static private bool FindOuterMostCloseToken(string line, int openPos, ref int closePos)
		{
			for (int innerOpenPos = openPos; ; )
			{
				innerOpenPos = line.IndexOf("{", innerOpenPos + 1, closePos - innerOpenPos - 1, StringComparison.Ordinal);
				if (innerOpenPos == -1)
					return true;
				int i = line.IndexOf("}", closePos + 1, StringComparison.Ordinal);
				if (i == -1)
					return false;
				closePos = i;
			}
		}

		static private string GetToken(string line, int openPos, int closePos)
		{
			return line.Substring(openPos + 1, closePos - openPos - 1).Trim();
		}

		static private string ReplaceTokenWithValue(string line, string tokenValue, int openPos, int closePos)
		{
			if (openPos == 0)
				return tokenValue + line.Substring(closePos + 1).Trim();
			return line.Substring(0, openPos) + tokenValue + line.Substring(closePos + 1).Trim();
		}
	}
}
