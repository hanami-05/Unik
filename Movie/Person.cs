using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movie
{
    public class Person
    {
        public string Name { get; set; }
        public HashSet<Movie> Movies { get; set; }
        public string Job { get; set; }

        public Person() 
        {
            Movies = new HashSet<Movie>();
        }

    }
}