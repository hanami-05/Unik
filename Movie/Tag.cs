using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movie
{
    public class Tag 
    {
        public string TagName { get; set; }
        public HashSet<Film> Movies { get; set; }

        public Tag() 
        {
            Movies = new HashSet<Film>();
        }

        public override string ToString() 
        {
            string res = string.Empty;
            res += $"Tag Name: {TagName}\n";
            res += "Movies:\n";
            foreach (Film movie in Movies) res += $"{movie.Title}\n\n";

            return res;
        }
    }
}
