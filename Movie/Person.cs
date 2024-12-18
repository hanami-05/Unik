using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movie
{
    public class Person
    {
        public string Name { get; set; }
        public HashSet<Film> Movies { get; set; }
        public string Job { get; set; }

        public Person() 
        {
            Movies = new HashSet<Film>();
        }

        public override string ToString() 
        {
            string res = string.Empty;

            res += $"Name: {Name}\n";
            res += $"\nJob: {Job}\n";
            res += $"\nMovies:\n";
            foreach (Film movie in Movies) res += $"{movie.Title}\n\n";

            return res;
        }
    }
}