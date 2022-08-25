using System;
using System.Collections;
using System.Threading;
using System.Windows.Forms;

namespace Go.Tools
{
	public class CommandLine
	{
		class SyntaxException : Exception {};

        static string[] args = null;

        public bool ParseAndExecute(SortedList options, SortedList directories, string[] arguments)
		{
            args = arguments;

			bool commandExecuted = false;
			if (arguments.Length >= 1)
			{
				// Execute command (if found)
				IDictionaryEnumerator cmdEnumerator = options.GetEnumerator();
				while (!commandExecuted && cmdEnumerator.MoveNext())
				{
					if (	arguments[0].ToUpper() == ((Options)cmdEnumerator.Value).alias1?.ToUpper()
						||	arguments[0].ToUpper() == ((Options)cmdEnumerator.Value).alias2?.ToUpper()
						||	arguments[0].ToUpper() == ((Options)cmdEnumerator.Value).alias3?.ToUpper())
					{
						commandExecuted = true;

						// Execute
						try {
							if (((Options)cmdEnumerator.Value).goExecute != null) {
							    var thread = new Thread(() => ((Options)cmdEnumerator.Value).goExecute((Options)cmdEnumerator.Value));
							    thread.Start();

                                // ((Options)cmdEnumerator.Value).goExecute((Options)cmdEnumerator.Value);
							}
						}
						catch (SyntaxException) {
							if (((Options)cmdEnumerator.Value).goSyntax != null)
								MessageBox.Show(((Options)cmdEnumerator.Value).goSyntax(false, false), "GO " + ((Options)cmdEnumerator.Value).alias1);
							else
								MessageBox.Show("GO " + ((Options)cmdEnumerator.Value).alias1, "GO " + ((Options)cmdEnumerator.Value).alias1);
						}

						// Show Info
						if (((Options)cmdEnumerator.Value).goInfo != null) {
							string txt = ((Options)cmdEnumerator.Value).goInfo();
							if (txt.StartsWith("<HTML>"))
								Shell.Browser(FileTools.WriteTempFile("GoInfo.html", txt), false, false);
							else
								MessageBox.Show(((Options)cmdEnumerator.Value).description + "\n\n" +
									txt, 
									((Options)cmdEnumerator.Value).alias1);
						}
					}
				}

				// Open explorer to alias (if found)
				if (!commandExecuted)
				{
					IDictionaryEnumerator aliasEnumerator = directories.GetEnumerator();
					while (aliasEnumerator.MoveNext())
					{
						if (arguments[0].ToUpper() == (string)aliasEnumerator.Key.ToString().ToUpper())
						{
							commandExecuted = true;
							string[] paths = 
								StringTools.CompactStringArray(((string)aliasEnumerator.Value).Split(
									new Char[] {'?'}));
							foreach (string path in paths)
								Shell.ExplorerPath(path.Trim(), false, false);
							break;
						}
					}
				}

				// TODO: assume directory.  Open explorer to command line (if found).

				// Show syntax
				if (!commandExecuted)
				{
					Syntax(options, directories);
				}

				return true;
			}
			else
				return false;
		}

		static public void EvalCommandLine(ref SortedList argOptions)
		{
			// Process all the command-line arguments.
			for (int i = 1; i < args.Length; i++)
			{
				// Check for syntax request.
				if (args[i].Substring(0, 1) == "?")
					throw new SyntaxException();

				// Ensure the arg begins with a dash or slash.
				if (args[i].Substring(0, 1).IndexOfAny("-/".ToCharArray()) == -1)
				{
					throw new Exception("Unknown option " + args[i]);
				}

				// Check for syntax request.
				string tok = args[i].Substring(1);
				if (tok == "?" || tok == "h" || tok == "H")
					throw new SyntaxException();

				// Evaluate all the argOptions.
				bool	validArg = true;
				IDictionaryEnumerator argOptionEnumerator = argOptions.GetEnumerator();
				while (argOptionEnumerator.MoveNext())
				{
					validArg = false;
					string	key = (string)argOptionEnumerator.Key;
					if (	key.Substring(0, 1).IndexOfAny("-/".ToCharArray()) != -1
						&&	args[i].Substring(1).ToUpper().StartsWith(key.Substring(1).ToUpper()))
					{
						validArg = true;
			
						if		(argOptionEnumerator.Value.GetType().ToString().CompareTo("System.Boolean") == 0)
							argOptions[key] = true;
						else if (argOptionEnumerator.Value.GetType().ToString().CompareTo("System.String") == 0)
						{
							if (args[i].Length > key.Length)
								argOptions[key] = args[i].Substring(key.Length);
							else if (i + 1 < args.Length)
								argOptions[key] = args[++i];
							else
							{
								throw new Exception("Missing rest of option for argument " + args[i]);
							}
						}
                        else if (argOptionEnumerator.Value.GetType().ToString().CompareTo("System.Int32") == 0)
                        {
                            if (args[i].Length > key.Length)
                                argOptions[key] = Convert.ToInt32(args[i].Substring(key.Length));
                            else if (i + 1 < args.Length)
                                argOptions[key] = Convert.ToInt32(args[++i]);
                            else
                            {
                                throw new Exception("Missing rest of option for argument " + args[i]);
                            }
                        }
                        else
                        {
                            throw new Exception("Unsupported type " + argOptionEnumerator.Value.GetType() + " for argument " + args[i]);
                        }
						break;
					}
				}
				if (!validArg)
				{
					throw new Exception("Unknown option " + args[i]);
				}
			}
		}
        
        #region Syntax
		private void Syntax(SortedList options, SortedList directories)
		{
			string syntax = @"<HTML>
<BODY>
<H3>Go  v2.0  Richard Rozsa</H3>
<H3>Syntax:</H3>
&nbsp;&nbsp;&nbsp;&nbsp;Go &lt;command&gt;/&lt;Alias&gt;/&lt;path&gt;<BR>
<BR><BR>
<H3>Commands:</H3>
<TABLE>
";

			string category = "";
			IDictionaryEnumerator cmdEnumerator = options.GetEnumerator();
			while (cmdEnumerator.MoveNext())
			{
				if (category != ((Options)cmdEnumerator.Value).category)
				{
					//					if (lastCategory.Length != 0)
					//						syntax += "</TABLE>";
					category = ((Options)cmdEnumerator.Value).category;
					syntax += "<TR><TD>&nbsp;</TD><TD colspan='2'><font size='4'><B><I><BR>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + category + "<BR>&nbsp;</I></B></font></TD></TR>";
					//					syntax += "<TABLE>";
				}
				syntax += "<TR><TD>&nbsp;</TD><TD valign=\"top\">";
				syntax += ((Options)cmdEnumerator.Value).alias1;
				syntax += "</TD><TD valign=\"top\">";
				syntax += ((Options)cmdEnumerator.Value).description;
				if (((Options)cmdEnumerator.Value).alias2.Length > 0 && ((Options)cmdEnumerator.Value).alias3.Length == 0)
				{
					syntax += " (alias 1 = " + ((Options)cmdEnumerator.Value).alias2 + ")";
				}
				if (((Options)cmdEnumerator.Value).alias2.Length > 0 && ((Options)cmdEnumerator.Value).alias3.Length > 0)
				{
					syntax += " (alias 1 = " + ((Options)cmdEnumerator.Value).alias2 + ", ";
					syntax += "alias 2 = " + ((Options)cmdEnumerator.Value).alias3 + ")";
				}
				syntax += "<BR><font size='2' face='Courier New'>" + StringTools.Text2Html(((Options)cmdEnumerator.Value).GetSyntax(true, false)) + "</font>";
				syntax += "</TD>";

				syntax += "</TR>";
			}
			
			syntax += @"</TABLE>
<BR><BR>
<H3>Aliases:</H3>
<TABLE>
";
			IDictionaryEnumerator aliasEnumerator = directories.GetEnumerator();
			while (aliasEnumerator.MoveNext())
			{
				syntax += "<TR><TD>&nbsp;</TD><TD>";
				syntax += (string)aliasEnumerator.Key;
				syntax += "</TD></TR>";
			}

			syntax += @"</TABLE>
</BODY>
</HTML>";

			Shell.Browser(FileTools.WriteTempFile("GoSyntax.html", syntax), false, false);
		}
		#endregion Syntax
    }
}
