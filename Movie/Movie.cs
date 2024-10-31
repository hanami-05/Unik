using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movie
{
    public class Movie 
    {
        public Movie() { }

        public Movie(string title, HashSet<string> actors, string director, HashSet<string> tags, double rating) 
        {
            Title = title;
            Actors = actors;
            Director = director;
            Tags = tags;
            Rating = rating;
        }
        public string Title { get; set; }

        public HashSet<string> Actors { get; set; }
        public string Director { get; set; }
        public HashSet<string> Tags { get; set; }
        public double Rating { get; set; }
    }
}
