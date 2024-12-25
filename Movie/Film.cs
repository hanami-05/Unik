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

        public double GetSimilarity(Film film) 
        {
            double pMark = 0;
            double tMark = 0;
            HashSet<string> pNames = Actors.Select(actor => actor.Name).ToHashSet();
            HashSet<string> tNames = Tags.Select(tag => tag.TagName).ToHashSet();

            if (pNames.Count == 0) pMark = 0;
            else 
            {
				int pCount = film.Actors.Select(actor => actor.Name).Where(name => pNames.Contains(name)).Count();
                pMark = pCount/pNames.Count;
			}
            if (tNames.Count == 0) tMark = 0;
            else 
            {
				int tCount = film.Tags.Select(tag => tag.TagName).Where(name => tNames.Contains(name)).Count();
                tMark = tCount/tNames.Count;
			}

            return (0.25 * (pMark + tMark)) + (film.Rating * 0.5);
        }

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
