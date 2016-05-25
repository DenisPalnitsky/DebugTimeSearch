using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MyCompany.VariableExplorer.Model
{
    class StringFilter
    {
        private Regex _filter;


        public StringFilter(string filter)
        {
            _filter = new Regex(WildcardToRegex(filter), RegexOptions.IgnoreCase);
        }

        public bool IsMatching (string stringToMatch)
        {
            return _filter.IsMatch(stringToMatch);
        }

        private static string WildcardToRegex(string pattern)
        {
            if (!pattern.Contains('?') && !pattern.Contains('*'))   
            {
                pattern = "*" + pattern + "*";                
            }

            return "^" + Regex.Escape(pattern)
                                  .Replace(@"\*", ".*")
                                  .Replace(@"\?", ".")
                           + "$";
        }

    }
}
