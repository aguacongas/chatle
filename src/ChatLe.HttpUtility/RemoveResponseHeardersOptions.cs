using System;
using System.Collections.Generic;
using System.Linq;

namespace ChatLe.HttpUtility
{
    public class RemoveResponseHeardersOptions
    {

        public string HeadersList
        {
            get { return string.Join(",", Headers); }
            set
            {
                Headers = new List<string>(value.Split(',').Select(v => v.Trim()));
            }
        }
        internal ICollection<string> Headers { get; set; } = new List<string>();

    }
}