using System.Collections.Generic;

namespace Go.Configuration
{
    public class Root
    {
        public List<Section> LocationAndValueSections { get; set; }
        public Programs Programs { get; set; }
    }
}
