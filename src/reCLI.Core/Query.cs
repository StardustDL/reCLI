using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace reCLI.Core
{
    public class Query
    {
        public static char TermSeperator { get => ' '; }

        /// <summary>
        /// Raw query, this includes action keyword if it has
        /// We didn't recommend use this property directly. You should always use Search property.
        /// </summary>
        public string RawText { get; set; }

        /// <summary>
        /// Search part of a query.
        /// This will not include action keyword if exclusive plugin gets it, otherwise it should be same as RawQuery.
        /// Since we allow user to switch a exclusive plugin to generic plugin, 
        /// so this property will always give you the "real" query part of the query
        /// </summary>
        public string Arguments { get; set; }

        /// <summary>
        /// The raw query splited into a string array.
        /// </summary>
        public string[] Terms { get; set; }

        public string GetTrimedText() => String.Join(TermSeperator.ToString(), Terms);

        public string Keyword { get; set; }

        public override string ToString() => RawText;
    }
}
