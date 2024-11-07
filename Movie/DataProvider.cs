using System.Text;
using System.Globalization;
using System.Collections.Concurrent;
using System.Data;


namespace Movie
{
    internal class DataProvider
    {
        public DataProvider()
        {

        }

        internal Task ReadActorsFilmsAsync(string fileName, BlockingCollection<string> lines)
        {
            return Task.Factory.StartNew(
                () => {
                    using (StreamReader reader = new StreamReader(fileName, Encoding.UTF8))
                    {
                        string line = string.Empty;

                        reader.ReadLine();
                        while ((line = reader.ReadLine()) != null)
                        {
                            lines.Add(line);
                        }
                    }
                    lines.CompleteAdding();
                }, TaskCreationOptions.LongRunning);
        }

        internal Task CheckActorsFilmsLinesAsync(BlockingCollection<string> input, BlockingCollection<string> output) 
        {
            return Task.Factory.StartNew(
                () => {
                    foreach (string line in input.GetConsumingEnumerable()) 
                    {
                        int ind = line.IndexOf('\t', line.IndexOf('\t', line.IndexOf('\t') +1 ) + 1);
                        string role = line.Substring(ind+1, 5);

                        if (role.Equals("direc") || role.Equals("actor") || role.Equals("actre"))
                            output.Add(line);
                    }
                    output.CompleteAdding();
                }, TaskCreationOptions.LongRunning);
        }

        internal Task ParseActorsFilmsLinesAsync(BlockingCollection<string> input, BlockingCollection<string> output)
        {
            return Task.Factory.StartNew(
                () =>
                {
                    foreach (string line in input.GetConsumingEnumerable())
                    {
                        string strId = line.Substring(2, 7);
                        string s = line.Substring(10);
                        int i = s.IndexOf('\t');
                        string c = s.Substring(i + 1);
                        string strPersonId = c.Substring(2, 7);
                        string role = c.Substring(10, 5);

                        output.Add($"{strId},{strPersonId},{role}");
                    }
                    output.CompleteAdding();
                }, TaskCreationOptions.LongRunning);
        }
        internal Task AppendActorsDirectorsToMoviesDictionaryAsync(BlockingCollection<string> lines, ConcurrentDictionary<int, Film> films, ConcurrentDictionary<int, Person> actors)
        {
            return Task.Factory.StartNew(
                () => {
                    foreach (string line in lines.GetConsumingEnumerable())
                    {
                        int ind = line.IndexOf(',');
                        int filmId = Convert.ToInt32(line.Substring(0, ind));
                        string c = line.Substring(ind + 1);
                        ind = c.IndexOf(',');
                        int personId = Convert.ToInt32(c.Substring(0, ind));
                        string role = c.Substring(ind + 1);

                        if (!actors.ContainsKey(personId) || !films.ContainsKey(filmId)) continue;

                        switch (role)
                        {
                            case "direc":
                                
                                actors[personId].Job = "director";

                                films.AddOrUpdate(filmId, new Film(), (key, movie) =>
                                {
                                    movie.Director = actors[personId];
                                    return movie;
                                });

                                actors.AddOrUpdate(personId, new Person(), (key, person) =>
                                {
                                    person.Movies.Add(films[filmId]);
                                    return person;
                                });

                                break;

                            case "actre":
                                actors[personId].Job = "actress";

                                films.AddOrUpdate(filmId, new Film(), (key, movie) =>
                                {
                                    movie.Actors.Add(actors[personId]);
                                    return movie;
                                });

                                actors.AddOrUpdate(personId, new Person(), (key, person) =>
                                {
                                    person.Movies.Add(films[filmId]);
                                    return person;
                                });

                                break;

                            default:
                                actors[personId].Job = "actor";

                                films.AddOrUpdate(filmId, new Film(), (key, movie) =>
                                {
                                    movie.Actors.Add(actors[personId]);
                                    return movie;
                                });

                                actors.AddOrUpdate(personId, new Person(), (key, person) =>
                                {
                                    person.Movies.Add(films[filmId]);
                                    return person;
                                });

                                break;
                        }

                    }
                }, TaskCreationOptions.LongRunning);
        }

        internal Task AppendMoviesToActorAsync(BlockingCollection<string> lines, ConcurrentDictionary<int, Person> actors, ConcurrentDictionary<int, Film> movies) 
        {
            return Task.Factory.StartNew(
                () => { 
                    foreach (string line in lines)
                    {
                        int ind = line.IndexOf(',');
                        int filmId = Convert.ToInt32(line.Substring(0, ind));
                        string c = line.Substring(ind + 1);
                        ind = c.IndexOf(',');
                        int personId = Convert.ToInt32(c.Substring(0, ind));

                        if (!actors.ContainsKey(personId) || !movies.ContainsKey(filmId)) continue;

                        actors[personId].Movies.Add(movies[filmId]);
                        string s = movies[filmId].Title;
                        var a = actors[personId].Movies;
                    }
            }, TaskCreationOptions.LongRunning);
        }

        internal Task ReadMoviesCodesAsync(string fileName, BlockingCollection<string> lines)
        {
            return Task.Run(
                () => {
                    using (StreamReader reader = new StreamReader(fileName, Encoding.UTF8))
                    {
                        string line = string.Empty;

                        reader.ReadLine();
                        while ((line = reader.ReadLine()) != null)
                        {
                            lines.Add(line);
                        }
                    }
                    lines.CompleteAdding();
                });
        } //114709

        internal Task CheckMoviesCodesAsync(BlockingCollection<string> input, BlockingCollection<string> output) 
        {
            return Task.Run(
                () => {
                    foreach (string line in input.GetConsumingEnumerable()) 
                    {
                        int ind = line.IndexOf('\t', line.IndexOf('\t', line.IndexOf('\t') + 1) + 1);
                        string lang = line.Substring(ind+1, 2);

                        if (lang.Equals("RU") || lang.Equals("EN"))
                            output.Add(line);
                    }

                    output.CompleteAdding();
                });
        }
        internal Task ParseMoviesCodesLinesAsync(BlockingCollection<string> input, BlockingCollection<string> output) 
        {
            return Task.Run(
                () => {
                    foreach (string line in input.GetConsumingEnumerable()) 
                    {
                        int id = int.Parse(line.Substring(2, 7));
                        int i = line.Substring(10).IndexOf('\t');
                        string s = line.Substring(10 + i + 1);
                        int k = s.IndexOf('\t');
                        string title = s.Substring(0, k);

                        output.Add($"{id},{title}");
                    }

                    output.CompleteAdding();
                });
            
        } 
        internal Task AppendToMoviesDictionaryAsync(BlockingCollection<string> lines, ConcurrentDictionary<int, Film> dict)
        {
            return Task.Run(
                () => {
                    foreach (string line in lines.GetConsumingEnumerable())
                    {
                        int ind = line.IndexOf(',');
                        int id = Convert.ToInt32(line.Substring(0, ind));
                        string title = line.Substring(ind + 1);

                        dict.AddOrUpdate(id, new Film() { Title = title },
                            (key, movie) => {
                                movie.Title = $"{movie.Title};{title}"; return movie;
                            });
                    }
                });
        }

        internal Task ReadMoviesRatingsAsync(string fileName, BlockingCollection<string> lines)
        {
            return Task.Run(
                () => {
                    using (StreamReader reader = new StreamReader(fileName, Encoding.UTF8))
                    {
                        string line = string.Empty;

                        reader.ReadLine();
                        while ((line = reader.ReadLine()) != null)
                        {
                            lines.Add(line);
                        }
                    }

                    lines.CompleteAdding();
                });
        }

        internal Task ParseMoviesRatingLinesAsync(BlockingCollection<string> input, BlockingCollection<string> output) 
        {
            return Task.Run(
                () => {
                    foreach (string line in input.GetConsumingEnumerable()) 
                    {
                        int id = int.Parse(line.Substring(2, 7));
                        double rate = double.Parse(line.Substring(10, 3), new NumberFormatInfo()
                        {
                            NumberDecimalSeparator = "."
                        });

                        output.Add($"{id},{rate}");
                    }

                    output.CompleteAdding();
                });
        }

        internal Task AppendRatingToMoviesDictionaryAsync(BlockingCollection<string> lines, ConcurrentDictionary<int, Film> dict) 
        {
            return Task.Run(
                () => {
                    foreach (string line in lines.GetConsumingEnumerable()) 
                    {
                        int ind = line.IndexOf(',');
                        int id = Convert.ToInt32(line.Substring(0,ind));
                        double rate = Convert.ToDouble(line.Substring(ind+1));

                        dict.AddOrUpdate(id, new Film(), (key, movie) => 
                        {
                            movie.Rating = rate;
                            return movie;
                        });
                    }
                });
        }

        internal Task ReadActorsNamesAsync(string fileName, BlockingCollection<string> lines)
        {
            return Task.Factory.StartNew(
                () => {
                    using (StreamReader reader = new StreamReader(fileName, Encoding.UTF8))
                    {
                        string line = string.Empty;

                        reader.ReadLine();
                        while ((line = reader.ReadLine()) != null)
                        {
                            lines.Add(line);
                        }
                    }

                    lines.CompleteAdding();
                }, TaskCreationOptions.LongRunning);
        }

        internal Task ParseActorsNamesLinesAsync(BlockingCollection<string> input, BlockingCollection<string> output) 
        {
            return Task.Factory.StartNew(
                () => {
                    foreach (string line in input.GetConsumingEnumerable()) 
                    {
                        int id = int.Parse(line.Substring(2, 7));

                        string s = line.Substring(10);
                        string name = s.Substring(0, s.IndexOf('\t'));

                        output.Add($"{id},{name}");
                    }

                    output.CompleteAdding();
                }, TaskCreationOptions.LongRunning);
        }

        internal Task AppendActorsToDictionaryAsync(BlockingCollection<string> lines, ConcurrentDictionary<int, Person> dict)
        {
            return Task.Factory.StartNew(
               () => {
                   foreach (string line in lines.GetConsumingEnumerable())
                   {
                       int ind = line.IndexOf(',');
                       int id = Convert.ToInt32(line.Substring(0, ind));
                       string name = line.Substring(ind + 1);

                       dict.AddOrUpdate(id, new Person() { Name = name }, (key, value) => value);
                   }
               }, TaskCreationOptions.LongRunning);
        }
        internal Task ReadCodesLinksAsync(string fileName, BlockingCollection<string> lines)
        {
            return Task.Run(
                () => {
                    using (StreamReader reader = new StreamReader(fileName, Encoding.UTF8))
                    {
                        string line = string.Empty;

                        reader.ReadLine();
                        while ((line = reader.ReadLine()) != null)
                        {
                            lines.Add(line);
                        }
                    }

                    lines.CompleteAdding();
                });
        }

        internal Task ParseCodeLinksLinesAsync(BlockingCollection<string> input, BlockingCollection<string> output) 
        {
            return Task.Run(
                () => {
                    foreach (string line in input.GetConsumingEnumerable()) 
                    {
                        int i = line.IndexOf(',');
                        int movieLensId = int.Parse(line.Substring(0, i));
                        string s = line.Substring(i + 1);
                        int imdbId = int.Parse(s.Substring(0, s.IndexOf(',')));

                        output.Add($"{movieLensId},{imdbId}");
                    }

                    output.CompleteAdding();
                });
        }

        internal Task AppendCodesLinksToDictionaryAsync(BlockingCollection<string> lines, ConcurrentDictionary<int, int> dict) 
        {
            return Task.Run(
                () => {
                    foreach (string line in lines.GetConsumingEnumerable()) 
                    {
                        int ind = line.IndexOf(',');
                        int movieLensId = Convert.ToInt32(line.Substring(0, ind));
                        int imdbId = Convert.ToInt32(line.Substring(ind+1));

                        dict.AddOrUpdate(movieLensId, imdbId, (key, value) => value);
                    }
                });
        }

        internal Task ReadTagsAsync(string fileName, BlockingCollection<string> lines)
        {
            return Task.Run(
                () => {
                    using (StreamReader reader = new StreamReader(fileName, Encoding.UTF8))
                    {
                        string line = string.Empty;

                        reader.ReadLine();
                        while ((line = reader.ReadLine()) != null)
                        {
                            lines.Add(line);
                        }
                    }

                    lines.CompleteAdding();
                });
        }

        internal Task ParseTagsLinesAsync(BlockingCollection<string> input, BlockingCollection<string> output) 
        {
            return Task.Run(
                () => {
                    foreach (string line in input.GetConsumingEnumerable()) 
                    {
                        int i = line.IndexOf(',');
                        int id = int.Parse(line.Substring(0, i));
                        string tag = line.Substring(i + 1);

                        output.Add($"{id},{tag}");
                    }

                    output.CompleteAdding();
                });
        }
        internal Task AppendTagsToDictionaryAsync(BlockingCollection<string> lines, ConcurrentDictionary<int, Tag> dict) 
        {
            return Task.Run(
                () => {
                    foreach (string line in lines.GetConsumingEnumerable()) 
                    {
                        int ind = line.IndexOf(',');
                        int id = Convert.ToInt32(line.Substring(0, ind));
                        string tagName = line.Substring(ind + 1);

                        dict.AddOrUpdate(id, new Tag() { TagName = tagName}, (key, value) => value);
                    }
                });
        }

        internal Task ReadMoviesTagsAsync(string fileName, BlockingCollection<string> lines)
        {
            return Task.Factory.StartNew(
                () => {
                    using (StreamReader reader = new StreamReader(fileName, Encoding.UTF8))
                    {
                        string line = string.Empty;

                        reader.ReadLine();
                        while ((line = reader.ReadLine()) != null)
                        {
                            lines.Add(line);
                        }
                    }

                    lines.CompleteAdding();
                }, TaskCreationOptions.LongRunning);
        }

        internal Task CheckTagsFilmsAsync(BlockingCollection<string> input, BlockingCollection<string> output) 
        {
            return Task.Factory.StartNew(
                () => {

                    foreach (string line in input.GetConsumingEnumerable()) 
                    {
                        double d = double.Parse(line.Substring(line.IndexOf('.') - 1, 3), new NumberFormatInfo() 
                        {
                            NumberDecimalSeparator = "."
                        });

                        if (d > 0.5) output.Add(line);
                    }

                    output.CompleteAdding();
                }, TaskCreationOptions.LongRunning);
        }

        internal Task ParseMoviesTagsLinesAsync(BlockingCollection<string> input, BlockingCollection<string> output) 
        {
            return Task.Factory.StartNew(() => {
                foreach (string line in input.GetConsumingEnumerable())
                {
                    int i = line.IndexOf(',');

                    int movieId = int.Parse(line.Substring(0, i));
                    string s = line.Substring(i + 1);
                    int tagId = int.Parse(s.Substring(0, s.IndexOf(',')));
                    output.Add($"{movieId},{tagId}");
                }
                output.CompleteAdding();
            }, TaskCreationOptions.LongRunning);
        }
        internal Task AppendTagsToMoviesDictionaryAsync(BlockingCollection<string> lines, ConcurrentDictionary<int, Film> films, ConcurrentDictionary<int, Tag> tags, ConcurrentDictionary<int, int> codeLinks) 
        {
            return Task.Factory.StartNew(
                () => {
                    foreach (string line in lines.GetConsumingEnumerable()) 
                    {
                        int ind = line.IndexOf(',');
                        int id = Convert.ToInt32(line.Substring(0, ind));
                        int tagId = Convert.ToInt32(line.Substring(ind+1));

                        if (!codeLinks.ContainsKey(id) || !tags.ContainsKey(tagId)) continue;
                        id = codeLinks[id];

                        if (!films.ContainsKey(id)) continue;

                        films.AddOrUpdate(id, new Film(), (key, movie) => 
                        {
                            movie.Tags.Add(tags[tagId]);
                            return movie;
                        });

                        tags.AddOrUpdate(tagId, new Tag(), (key, tag) =>
                        {
                            tag.Movies.Add(films[id]);
                            return tag;
                        });
                    }
                }, TaskCreationOptions.LongRunning);
        }

        internal Task AppendMoviesToTagAsync(BlockingCollection<string> lines, ConcurrentDictionary<int, Tag> tags, ConcurrentDictionary<int, Film> movies, ConcurrentDictionary<int, int> codeLinks)  
        {
            return Task.Factory.StartNew(
                () => {
                    foreach (string line in lines) 
                    {
                        int ind = line.IndexOf(',');
                        int id = Convert.ToInt32(line.Substring(0, ind));
                        int tagId = Convert.ToInt32(line.Substring(ind + 1));

                        if (!codeLinks.ContainsKey(id) || !tags.ContainsKey(tagId)) continue;
                        
                        id = codeLinks[id];
                        if (!movies.ContainsKey(id)) continue;
                        
                        tags[tagId].Movies.Add(movies[id]);
                    }
                }, TaskCreationOptions.LongRunning);
        }
    }
}
