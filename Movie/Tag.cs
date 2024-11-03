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
        public HashSet<Movie> Movies { get; set; }

        public Tag() 
        {
            Movies = new HashSet<Movie>();
        }    
    }
}
