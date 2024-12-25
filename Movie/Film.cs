using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movie
{
    public class Film 
    {
        public Film() 
        {
            Title = string.Empty;
            Actors = new HashSet<Person>();
            Tags = new HashSet<Tag>();

        }

        public string Title { get; set; }
        public HashSet<Person> Actors { get; set; }
        public HashSet<Tag> Tags { get; set; }
        public double Rating { get; set; }

        public override string ToString() 
        {
            string res = string.Empty;

            res += $"Title: {Title}\n";
            res += $"\nRating: {Rating}\n";
            res += "\nActors: \n";
            foreach (Person person in Actors) res += $"{person.Name}; {person.Job}\n";
            res += "\nTags: \n";
            foreach (Tag tag in Tags) res += $"{tag.TagName}\n";

            return res;
        }
    }
}
