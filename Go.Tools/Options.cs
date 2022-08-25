using System;
using System.Windows.Forms;

namespace Go.Tools
{
	public delegate void   GoExecuteDelegate(Options option);
	public delegate string GoSyntaxDelegate(bool syntaxOnly, bool switchesOnly);
	public delegate string GoInfoDelegate();

	public class Options
	{
		public Options(string category, string alias1, string alias2, string alias3, string description, GoExecuteDelegate goExecute, GoSyntaxDelegate goSyntax, GoInfoDelegate goInfo)
		{
			this.category		= category;
			this.alias1			= alias1;
			this.alias2			= alias2;
			this.alias3			= alias3;
			this.description	= description;
			this.goExecute		= goExecute;
			this.goSyntax		= goSyntax;
			this.goInfo			= goInfo;
		}
		public string				category;
		public string				alias1;
		public string				alias2;
		public string				alias3;
		public string				description;
		public GoExecuteDelegate	goExecute;	
		public GoSyntaxDelegate		goSyntax;	
		public GoInfoDelegate		goInfo;

		public string GetSyntax(bool syntaxOnly, bool switchesOnly)
		{
			string syntax = "";
			if (goSyntax != null)
				syntax = goSyntax(syntaxOnly, switchesOnly);
			return "Syntax: GO " + alias1 + " " + syntax;
		}

		public void ShowSyntax()
		{
			ShowSyntax("");
		}

		public void ShowSyntax(string errorMsg)
		{
			bool	errors = false;
			string	errorSep = "";

			MessageBoxIcon	mbi = MessageBoxIcon.Information;
			if (errorMsg.Length > 0)
			{
				errors = true;
				errorMsg = "Error: " + errorMsg;
				errorSep = "\n\n";
				mbi = MessageBoxIcon.Error;
			}

			MessageBox.Show(errorMsg + errorSep + description + "\n\n" + GetSyntax(false, false),
				alias1,
				MessageBoxButtons.OK,
				mbi);

			if (errors)
				throw new Exception(errorMsg);
		}
	}
}
