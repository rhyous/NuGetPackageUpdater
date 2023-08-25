using Rhyous.NuGetPackageUpdater;
using Rhyous.NuGetPackageUpdater.Models;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Rhyous.NuGetPackageUpdater.Replacers
{
    internal class StringReplacer : IStringReplacer
    {

        public bool ReplaceInString(IEnumerable<Replacement> patterns, ref string text)
        {
            bool patternFoundAndTextNotTheSameAfterReplace = false;
            var originalText = text;
            foreach (var pattern in patterns)
            {
                if (!Regex.IsMatch(text, pattern.Pattern, pattern.RegexOptions))
                    continue;
                text = Regex.Replace(text, pattern.Pattern, pattern.ReplacementPattern, pattern.RegexOptions);
                if (text != originalText)
                    patternFoundAndTextNotTheSameAfterReplace = true;
            }
            return patternFoundAndTextNotTheSameAfterReplace;
        }
    }
}
