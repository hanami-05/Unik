using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Xml.XPath;
using System.ComponentModel.DataAnnotations;
using System.Collections.Concurrent;
using System.Security.Cryptography;

namespace Movie
{
    internal class DataProvider
    {
        public DataProvider()
        {

        }

        private void GetActorsCodes(string fileName, BlockingCollection<string> lines)
        {
            using (StreamReader reader = new StreamReader(fileName, Encoding.UTF8))
            {
                string line = string.Empty;

                reader.ReadLine();
                while ((line = reader.ReadLine()) != null)
                {

                    //string[] values = line.Split('\t',4);
                    //14325969 
                    //14325926
                    //14325870
                    //14329098
                    //14305569
                    string strId = line.Substring(2, 7);
                    string s = line.Substring(10);
                    int i = s.IndexOf('\t');
                    string c = s.Substring(i + 1);
                    string strPersonId = c.Substring(2, 7);
                    string role = c.Substring(10, 2);

                    if (/*line.Contains("actor") || line.Contains("director") || line.Contains("actress")*/
                        role.Equals("ac") || role.Equals("di")
                        )
                    {
                        lines.TryAdd($"{strId},{role + strPersonId}");
                    }
                }
            }
        }

        private void ReadMoviesCodes(string fileName, BlockingCollection<string> lines)
        {
            using (StreamReader reader = new StreamReader(fileName, Encoding.UTF8))
            {
                string line = string.Empty;

                reader.ReadLine();
                while ((line = reader.ReadLine()) != null)
                {
                    int id = int.Parse(line.Substring(2, 7));
                    int i = line.Substring(10).IndexOf('\t');
                    string s = line.Substring(i + 1);
                    int k = s.IndexOf('\t');
                    string title = s.Substring(0, k);
                    string l = s.Substring(k + 1);
                    int ind = l.IndexOf('\t');
                    string lang = l.Substring(0, ind);

                    if (!lang.Equals("RU") || !lang.Equals("EN")) continue;

                    lines.TryAdd($"{id},{title}");
                }
            }
        }

        internal Task ReadMoviesCodesAsync(string fileName, BlockingCollection<string> lines)
        {
            return Task.Factory.StartNew(() =>
            {
                ReadMoviesCodes(fileName, lines);
                lines.CompleteAdding();
            }, TaskCreationOptions.LongRunning);
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

        internal Dictionary<int, double> GetMoviesRatings(string fileName)
        {
            Dictionary<int, double> result = new Dictionary<int, double>();

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

                    result.Add(id, rate);
                }
            }

            return result;
        }

        private void ReadActorsNames(string fileName, BlockingCollection<string> lines)
        {
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
        }

        internal Task ReadActorsNamesAsync(string fileName, BlockingCollection<string> lines)
        {
            return Task.Factory.StartNew(
                () => {
                    ReadActorsNames(fileName, lines);
                    lines.CompleteAdding();
                }, TaskCreationOptions.LongRunning);
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
        private void ReadCodesLinks(string fileName, BlockingCollection<string> lines)
        {
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
        }

        internal Task ReadCodesLinksAsync(string fileName, BlockingCollection<string> lines)
        {
            return Task.Run(
               () => {
                   ReadCodesLinks(fileName, lines);
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

        private void ReadTags(string fileName, BlockingCollection<string> lines)
        {
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
        }

        internal Task ReadTagsAsync(string fileName, BlockingCollection<string> lines) 
        {
            return Task.Run(
                () => {
                    ReadTags(fileName, lines);
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

        internal Dictionary<int, HashSet<int>> GetMoviesTags(string fileName)
        {
            Dictionary<int, HashSet<int>> result = new Dictionary<int, HashSet<int>>();

            using (StreamReader reader = new StreamReader(fileName, Encoding.UTF8))
            {
                string line = string.Empty;

                reader.ReadLine();
                while ((line = reader.ReadLine()) != null)
                {
                    double d = double.Parse(line.Substring(line.IndexOf('.') - 1), new NumberFormatInfo() 
                    {
                        NumberDecimalSeparator = "."
                    });
                    if (d <= 0.5) continue;

                    int i = line.IndexOf(',');

                    int movieId = int.Parse(line.Substring(0, i));
                    string s = line.Substring(i + 1);
                    int tagId = int.Parse(s.Substring(0, s.IndexOf(',')));

                    if (result.ContainsKey(movieId)) result[movieId].Add(tagId);
                    else
                    {
                        result.Add(movieId, new HashSet<int>() { tagId });
                    }
                }
            }

            return result;
        }
    }
}
