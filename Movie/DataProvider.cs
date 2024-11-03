using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Xml.XPath;
using System.ComponentModel.DataAnnotations;
using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json.Serialization;

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
            return Task.Run(
                () => {
                    foreach (string line in input.GetConsumingEnumerable()) 
                    {
                        int ind = line.IndexOf('\t', line.IndexOf('\t', line.IndexOf('\t') +1 ) + 1);
                        string role = line.Substring(ind+1, 5);

                        if (role.Equals("direc") || role.Equals("actor") || role.Equals("actre")) 
                        {
                            string strId = line.Substring(2, 7);
                            string s = line.Substring(10);
                            int i = s.IndexOf('\t');
                            string c = s.Substring(i + 1);
                            string strPersonId = c.Substring(2, 7);
                            
                            output.Add($"{strId},{strPersonId},{role}");
                        }
                    }
                    output.CompleteAdding();
                });
        }
        internal Task AppendActorsDirectorsToMoviesDictionaryAsync(BlockingCollection<string> lines, ConcurrentDictionary<int, Movie> dict, ConcurrentDictionary<int, Person> actors)
        {
            return Task.Run(
                () => {
                    foreach (string line in lines.GetConsumingEnumerable())
                    {
                        int ind = line.IndexOf(',');
                        int filmId = Convert.ToInt32(line.Substring(0, ind));
                        string c = line.Substring(ind + 1);
                        ind = c.IndexOf(',');
                        int personId = Convert.ToInt32(c.Substring(0, ind));
                        string role = c.Substring(ind + 1);

                        if (!actors.ContainsKey(personId)) continue;

                        switch (role)
                        {
                            case "direc":
                                actors[personId].Job = "director";

                                dict.AddOrUpdate(filmId, new Movie(), (key, movie) =>
                                {
                                    movie.Director = actors[personId];
                                    return movie;
                                });
                                break;

                            case "actre":
                                actors[personId].Job = "actress";

                                dict.AddOrUpdate(filmId, new Movie(), (key, movie) =>
                                {
                                    movie.Actors.Add(actors[personId]);
                                    return movie;
                                });


                                break;

                            default:
                                actors[personId].Job = "actor";

                                dict.AddOrUpdate(filmId, new Movie(), (key, movie) =>
                                {
                                    movie.Actors.Add(actors[personId]);
                                    return movie;
                                });

                                break;
                        }

                    }
                });
        }

        internal Task AppendMoviesToActorAsync(BlockingCollection<string> lines, ConcurrentDictionary<int, Person> actors, ConcurrentDictionary<int, Movie> movies) 
        {
            return Task.Run(
                () => { 
                    foreach (string line in lines.GetConsumingEnumerable())
                    {
                        int ind = line.IndexOf(',');
                        int filmId = Convert.ToInt32(line.Substring(0, ind));
                        string c = line.Substring(ind + 1);
                        ind = c.IndexOf(',');
                        int personId = Convert.ToInt32(c.Substring(0, ind));
                        string role = c.Substring(ind + 1);

                        if (!actors.ContainsKey(personId) || !movies.ContainsKey(filmId)) continue;

                        actors[personId].Movies.Add(movies[filmId]);
                    }
            });
        }

        internal Task ReadMoviesCodesAsync(string fileName, BlockingCollection<string> lines)
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

        internal Task CheckMoviesCodesAsync(BlockingCollection<string> input, BlockingCollection<string> output) 
        {
            return Task.Run(
                () => {
                    foreach (string line in input.GetConsumingEnumerable()) 
                    {
                        int ind = line.IndexOf('\t', line.IndexOf('\t', line.IndexOf('\t') + 1) + 1);
                        string lang = line.Substring(ind+1, 2);

                        if (lang.Equals("RU") || lang.Equals("EN")) 
                        {
                            int id = int.Parse(line.Substring(2, 7));
                            int i = line.Substring(10).IndexOf('\t');
                            string s = line.Substring(i + 1);
                            int k = s.IndexOf('\t');
                            string title = s.Substring(0, k);
                            
                            output.Add($"{id},{title}");
                        }
                    }

                    output.CompleteAdding();
                });
        }
        internal Task AppendToMoviesDictionaryAsync(BlockingCollection<string> lines, ConcurrentDictionary<int, Movie> dict)
        {
            return Task.Run(
                () => {
                    foreach (string line in lines.GetConsumingEnumerable())
                    {
                        int ind = line.IndexOf(',');
                        int id = Convert.ToInt32(line.Substring(0, ind));
                        string title = line.Substring(ind + 1);

                        dict.AddOrUpdate(id, new Movie() { Title = title },
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

                            int id = int.Parse(line.Substring(2, 7));
                            double rate = double.Parse(line.Substring(10, 3), new NumberFormatInfo()
                            {
                                NumberDecimalSeparator = "."
                            });

                            lines.Add($"{id},{rate}");
                        }
                    }

                    lines.CompleteAdding();
                });
        }

        internal Task AppendRatingTomoviesDictionaryAsync(BlockingCollection<string> lines, ConcurrentDictionary<int, Movie> dict) 
        {
            return Task.Run(
                () => {
                    foreach (string line in lines.GetConsumingEnumerable()) 
                    {
                        int ind = line.IndexOf(',');
                        int id = Convert.ToInt32(line.Substring(0,ind));
                        double rate = Convert.ToDouble(line.Substring(ind+1));

                        dict.AddOrUpdate(id, new Movie(), (key, movie) => 
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
                            int id = int.Parse(line.Substring(2, 7));

                            string s = line.Substring(10);
                            string name = s.Substring(0, s.IndexOf('\t'));

                            lines.Add($"{id},{name}");
                        }
                    }

                    lines.CompleteAdding();
                });
        }

        internal Task AppendActorsToDictionaryAsync(BlockingCollection<string> lines, ConcurrentDictionary<int, Person> dict)
        {
            return Task.Run(
               () => {
                   foreach (string line in lines.GetConsumingEnumerable())
                   {
                       int ind = line.IndexOf(',');
                       int id = Convert.ToInt32(line.Substring(0, ind));
                       string name = line.Substring(ind + 1);

                       dict.AddOrUpdate(id, new Person() { Name = name }, (key, value) => value);
                   }
               });
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
                            int i = line.IndexOf(',');
                            int movieLensId = int.Parse(line.Substring(0, i));
                            string s = line.Substring(i + 1);
                            int imdbId = int.Parse(s.Substring(0, s.IndexOf(',')));

                            lines.Add($"{movieLensId},{imdbId}");
                        }
                    }

                    lines.CompleteAdding();
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
                            int i = line.IndexOf(',');
                            int id = int.Parse(line.Substring(0, i));
                            string tag = line.Substring(i + 1);

                            lines.Add($"{id},{tag}");
                        }
                    }

                    lines.CompleteAdding();
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
            return Task.Run(
                () => {

                    foreach (string line in input.GetConsumingEnumerable()) 
                    {
                        double d = double.Parse(line.Substring(line.IndexOf('.') - 1, 4), new NumberFormatInfo() 
                        {
                            NumberDecimalSeparator = "."
                        });

                        if (d > 0.5) 
                        {
                            int i = line.IndexOf(',');

                            int movieId = int.Parse(line.Substring(0, i));
                            string s = line.Substring(i + 1);
                            int tagId = int.Parse(s.Substring(0, s.IndexOf(',')));
                            output.Add($"{movieId},{tagId}");
                        }
                    }

                    output.CompleteAdding();
                });
        }

        internal Task AppendTagsToMoviesDictionaryAsync(BlockingCollection<string> lines, ConcurrentDictionary<int, Movie> dict, ConcurrentDictionary<int, Tag> tags, ConcurrentDictionary<int, int> codeLinks) 
        {
            return Task.Run(
                () => {
                    foreach (string line in lines.GetConsumingEnumerable()) 
                    {
                        int ind = line.IndexOf(',');
                        int id = Convert.ToInt32(line.Substring(0, ind));
                        int tagId = Convert.ToInt32(line.Substring(ind+1));

                        if (!codeLinks.ContainsKey(id) || !tags.ContainsKey(tagId)) continue;
                        id = codeLinks[id];

                        dict.AddOrUpdate(id, new Movie(), (key, movie) => 
                        {
                            movie.Tags.Add(tags[tagId]);
                            return movie;
                        });
                    }
                });
        }

        internal Task AppendMoviesToTagAsync(BlockingCollection<string> lines, ConcurrentDictionary<int, Tag> tags, ConcurrentDictionary<int, Movie> movies, ConcurrentDictionary<int, int> codeLinks)  
        {
            return Task.Run(
                () => {
                    foreach (string line in lines.GetConsumingEnumerable()) 
                    {
                        int ind = line.IndexOf(',');
                        int id = Convert.ToInt32(line.Substring(0, ind));
                        int tagId = Convert.ToInt32(line.Substring(ind + 1));

                        if (!codeLinks.ContainsKey(id) || !tags.ContainsKey(tagId)) continue;
                        id = codeLinks[id];

                        tags[tagId].Movies.Add(movies[id]);
                    }
                });
        }
    }
}
