using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movie
{
    public class Movie 
    {
        public Movie() 
        {
            Actors = new HashSet<Person>();
            Tags = new HashSet<Tag>();

        }

        public string Title { get; set; }
        public HashSet<Person> Actors { get; set; }
        public Person Director { get; set; }
        public HashSet<Tag> Tags { get; set; }
        public double Rating { get; set; }
    }
}
