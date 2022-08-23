using System.Collections;
using Go.Tools;

namespace Go
{
	public class Framework
	{
		private SortedList		options			= null;
		private SortedList		directories		= null;
        private ProgressEvent   progressEvent   = null;
		private Organization	organization	= null;
		private Group			group			= null;
		private Individual		individual		= null;
        private WebStuff        webstuff        = null;

		public Framework(ProgressEventHandler peh)
		{
            progressEvent   = new ProgressEvent();
            progressEvent.Progress += peh;
            LoadConfigurations();
		}

        public void LoadConfigurations()
        {
            options = new SortedList();
            directories = new SortedList();
            organization = Organization.GetOrganization(ref options, ref directories, ref progressEvent);
            group = Group.GetGroup(ref options, ref directories, ref progressEvent);
            individual = Individual.GetIndividual(ref options, ref directories, ref progressEvent);
            webstuff = WebStuff.GetWebStuff(ref options, ref directories, ref progressEvent);
        }

        public SortedList Options
		{
			get { return options; }
		}

		public SortedList Directories
		{
			get { return directories; }
		}
	}
}
