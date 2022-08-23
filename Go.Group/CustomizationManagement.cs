using System;
using System.Collections;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using Go.Tools;

namespace Go
{
	class CustomizationManagement : GoImplementation
	{
		public CustomizationManagement(ref SortedList options, ref SortedList directories)
			: base(ref options, ref directories)
		{
			string category = "Test";
			AddOption(new Options(category, "Test",								"", "",		"Test go option",													new GoExecuteDelegate(Test), null, null));
		}

		public void Test(Options option)
		{
		}
	}
}