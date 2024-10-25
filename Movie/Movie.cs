using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movie
{
    public record class Movie (string Title, HashSet<string> Actors, string Director, HashSet<string> Tags
        , double Rate);
}
