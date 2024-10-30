using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Movie
{
    public delegate void MovieHandler(Movie? movie);
    public class MoviesRepository
    {
        private DataProvider _provider;
        /*private Dictionary<int, HashSet<string>> _actorsCodes;
        private Dictionary<string, int> _moviesCodes;
        private Dictionary<int, double> _moviesRatings;
        private Dictionary<int, string> _actorsNames;
        private Dictionary<int, int> _codesLinks;
        private Dictionary<int, string> _tags;
        private Dictionary<int, HashSet<int>> _moviesTags;*/
        
        public MoviesRepository() 
        {
            _provider = new DataProvider();
        }

        private void FillData(params string[] filesNames) 
        {
            /*Parallel.Invoke
                (
                    () => { _actorsCodes = _provider.GetActorsCodes(filesNames[0]); },
                    () => { _moviesCodes = _provider.GetMoviesCodes(filesNames[1]); },
                    () => { _moviesRatings = _provider.GetMoviesRatings(filesNames[2]); },
                    () => { _actorsNames = _provider.GetActorsNames(filesNames[3]); },
                    () => { _codesLinks = _provider.GetCodesLinks(filesNames[4]); },
                    () => { _tags = _provider.GetTags(filesNames[5]); },
                    () => { _moviesTags = _provider.GetMoviesTags(filesNames[6]); }
                );*/
        }

        public async Task FillDataAsync(params string[] filesNames) 
        {
            //_actorsCodes = await _provider.GetActorsCodesAsync(filesNames[0]);
            /*_moviesCodes = await _provider.GetMoviesCodesAsync(filesNames[1]);
            _moviesRatings = await _provider.GetMoviesRatingsAsync(filesNames[2]);
            _actorsNames = await _provider.GetActorsNamesAsync(filesNames[3]);
            _codesLinks = await _provider.GetCodesLinksAsync(filesNames[4]);
            _tags = await _provider.GetTagsAsync(filesNames[5]);
            _moviesTags = await _provider.GetMoviesTagsAsync(filesNames[6]);*/

            /* var task1 = _provider.GetActorsCodesAsync(filesNames[0]);
             var task2 = _provider.GetMoviesCodesAsync(filesNames[1]);
             var task3 = _provider.GetMoviesRatingsAsync(filesNames[2]);
             var task4 = _provider.GetActorsNamesAsync(filesNames[3]);
             var task5 = _provider.GetCodesLinksAsync(filesNames[4]);
             var task6 = _provider.GetTagsAsync(filesNames[5]);
             var task7 = _provider.GetMoviesTagsAsync(filesNames[6]);
             await Task.WhenAll(task7, task2, task3, task4, task5, task6, task7);

             _actorsCodes = task1.Result;
             _moviesCodes = task2.Result;
             _moviesRatings = task3.Result;
             _actorsNames = task4.Result;
             _codesLinks = task5.Result;
             _tags = task6.Result;
             _moviesTags = task7.Result;*/

            /*Task task = new Task
                (
                    () => FillData(filesNames)
                );
            task.Start();
            await task;*/

        }

        private ConcurrentDictionary<int, string> _moviesTitles = new ConcurrentDictionary<int, string>();
        private ConcurrentDictionary<int, string> _actorsNames = new ConcurrentDictionary<int, string>();
        private ConcurrentDictionary<int, int> _codeLinks = new ConcurrentDictionary<int, int>();
        private ConcurrentDictionary<int, string> _tagNames = new ConcurrentDictionary<int, string>();
        private ConcurrentDictionary<string, HashSet<string>> _moviesActors = new ConcurrentDictionary<string, HashSet<string>>();
        private ConcurrentDictionary<string, double> _moviesRating = new ConcurrentDictionary<string, double>();
        private ConcurrentDictionary<string, HashSet<string>> _moviesTags = new ConcurrentDictionary<string, HashSet<string>>();

        private BlockingCollection<string> _moviesLines = new BlockingCollection<string>();
        private BlockingCollection<string> _actorsNamesLines = new BlockingCollection<string>();
        private BlockingCollection<string> _codesLinksLines = new BlockingCollection<string>();
        private BlockingCollection<string> _tagsLines = new BlockingCollection<string>();

        private BlockingCollection<string> _moviesActorsLines = new BlockingCollection<string>();
        private BlockingCollection<string> _moviesRatingLines = new BlockingCollection<string>();
        private BlockingCollection<string> _moviesTagsLines = new BlockingCollection<string>();

        private Task FillMoviesDictionaryAsync() 
        {
            foreach (string line in _moviesLines.GetConsumingEnumerable()) 
            {
                int ind = line.IndexOf(',');
                int id = Convert.ToInt32(line.Substring(0, ind));
                string title = line.Substring(ind + 1);
                _moviesTitles.AddOrUpdate(id, title, (key, value) => value);
            }
        }
        
        public MovieHandler? MovieAction;

        public int GetCount() 
        {
            return _actorsCodes.Count + _actorsNames.Count + _codesLinks.Count + _tags.Count + _moviesTags.Count
                + _moviesRatings.Count + _moviesCodes.Count;
        }

        public Movie? GetMovieByTitle(string title) 
        {
            int code = _moviesCodes.GetValueOrDefault(title);

            if (code != 0)
            {
                HashSet<string> actors = new HashSet<string>
                    (
                        _actorsCodes.GetValueOrDefault(code).Where(person => person[0] == 'a')
                        .Select(person => _actorsNames[int.Parse(person.Substring(2))])
                    );
                string director = _actorsNames[int.Parse(_actorsCodes.GetValueOrDefault(code)
                    .Where(person => person[0] == 'd').FirstOrDefault().Substring(2))];
                    
                int movieLensId = _codesLinks.GetValueOrDefault(code);
                HashSet<string> tags = new HashSet<string>();
                
                if (movieLensId != 0) 
                {
                    tags = new HashSet<string>
                        (
                            _moviesTags.GetValueOrDefault(movieLensId).Select(tag => _tags[tag])
                        );
                }

                double rating = _moviesRatings.GetValueOrDefault(code);

                Movie result = new Movie(title, actors, director, tags, rating);

                MovieAction?.Invoke(result);

                return result;
            }

            MovieAction?.Invoke(null);
            return null;
        }
    }
}
