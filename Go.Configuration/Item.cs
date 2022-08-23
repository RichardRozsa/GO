using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;
using System.Security.Principal;

namespace Go.Configuration
{
    public class Item //: IValidatableObject
    {
        //[Required]
        public string Name { get; set; }
        public string Comment { get; set; }
        public string Url { get; set; }
        public string Path { get; set; }
        public string Arguments { get; set; }
        public bool ArgumentIsFilename { get; set; }
        public string Application { get; set; }
        public string StartIn { get; set; }
        public string WindowStyle { get; set; }
        public bool RunModal { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Info { get; set; }

        //public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        //{
        //    var itemTypes = new List<string>();
        //    if (!string.IsNullOrEmpty(Url))
        //    {
        //        itemTypes.Add("Url");
        //    }
        //    if (!string.IsNullOrEmpty(Path))
        //    {
        //        itemTypes.Add("Path");
        //    }

        //    if (itemTypes.Count == 0)
        //        yield return new ValidationResult("Either Url or Path is required", new [] {"Url", "Path", "RegistryValue", "EnvironmentVariable", "Text"});
        //    if (itemTypes.Count > 1)
        //        yield return new ValidationResult("Only one Url or Path is allowed", itemTypes);
        //}
    }
}