using System;
using System.Collections.Generic;
using System.Linq;

namespace ChatLe.HttpUtility
{
    /// <summary>
    /// Headers to remove configuration
    /// </summary>
    public class CommaSeparatedListOptions
    {
        /// <summary>
        /// Gets or sets the comma separated list of header to remove
        /// </summary>
        public string Values
        {
            get { return string.Join(",", List); }
            set
            {
                List = new List<string>(value.Split(',').Select(v => v.Trim()));
            }
        }
        internal ICollection<string> List { get; set; } = new List<string>();

    }
}