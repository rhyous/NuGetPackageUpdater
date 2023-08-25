using System.Text.RegularExpressions;

namespace Rhyous.NuGetPackageUpdater.Models
{
    public class Replacement
    {
        public string Pattern { get; set; }
        public string ReplacementPattern { get; set; }
        public RegexOptions RegexOptions { get; set; }
    }
}
