using System;
using System.Windows.Forms;

namespace Go.Tools
{
	public class Debug
	{
		static public void KnownErrors(string func, string msg)
		{
			if (DialogResult.Yes != MessageBox.Show(string.Format(@"There are some known errors in function {0}:

{1}

Continue anyway?", func, msg), "Go: !!! Known Errors !!!", MessageBoxButtons.YesNo))
				throw new Exception(string.Format("User stopped rather than continue function {0} with known errors", func));
		}
	}
}
