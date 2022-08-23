using System;
using System.Collections;
using Go.Tools;

namespace Go
{
	public class Group : GoImplementation
	{
		#region Initialization
		static private Group	        _group;
        static private ProgressEvent    _progressEvent;
		
		static public Group GetGroup(ref SortedList options, ref SortedList directories, ref ProgressEvent pe)
		{
            _progressEvent = pe;
 			if (_group == null)
				_group = new Group(ref options, ref directories);
			return _group;
		}

		static public Group GetGroup()
		{
			if (_group == null)
				throw new Exception("Can't instantiate Group class without parameters");
			return _group;
		}

		private Group(ref SortedList options, ref SortedList directories)
			: base(ref options, ref directories)
		{
			var root = ConfigurationFile.GetLocalConfiguration(ref directories, "Go.Group.xml");
            AddConfigurationOptions(root);

		    string category = "<your section here>";
			AddOption(new Options(category, "<YourOptionHere>",					"",	"",		"<Description for Option>",		    YourOptionHere, YourOptionHereSyntax, YourOptionHereInfo));
		}
		#endregion Initialization

        #region Your section here
        public string YourOptionHereSyntax(bool syntaxOnly, bool switchesOnly)
        {
            return "<syntax...>";
        }
        public void YourOptionHere(Options option)
        {
        }
        static string YourOptionHereInfo()
        {
            return @"<Your info...>";
        }
        #endregion
    }
}
