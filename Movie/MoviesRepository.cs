using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Movie
{
    public delegate void MovieHandler(Film? movie);
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

        private BlockingCollection<string> _moviesInput = new BlockingCollection<string>();
        private BlockingCollection<string> _moviesFiltered = new BlockingCollection<string>();
        private BlockingCollection<string> _moviesOutput = new BlockingCollection<string>();
        private BlockingCollection<string> _actorsNamesInput = new BlockingCollection<string>();
        private BlockingCollection<string> _actorsNamesFiltered = new BlockingCollection<string>();
        private BlockingCollection<string> _actorsNamesOuput = new BlockingCollection<string>();
        private BlockingCollection<string> _tagsInput = new BlockingCollection<string>();
        private BlockingCollection<string> _tagsOutput = new BlockingCollection<string>();
        private BlockingCollection<string> _codesLinksInput = new BlockingCollection<string>();
        private BlockingCollection<string> _codesLinksOuput = new BlockingCollection<string>();
        private BlockingCollection<string> _moviesActorsInput = new BlockingCollection<string>();
        private BlockingCollection<string> _moviesActorsFiltered = new BlockingCollection<string>();
        private BlockingCollection<string> _moviesActorsOutput = new BlockingCollection<string>();
        private BlockingCollection<string> _moviesRatingsInput = new BlockingCollection<string>();
        private BlockingCollection<string> _moviesRatingOutput = new BlockingCollection<string>();
        private BlockingCollection<string> _moviesTagsInput = new BlockingCollection<string>();
        private BlockingCollection<string> _moviesTagsFiltered = new BlockingCollection<string>();
        private BlockingCollection<string> _moviesTagsOutput = new BlockingCollection<string>();
        
        public Dictionary<int, Film> Movies { 
            get 
            {
                return _movies.Where(movie => !string.IsNullOrEmpty(movie.Value.Title)).ToDictionary(); 
            } 
        }
        public Dictionary<int, Person> ActorsAndDirectors { 
            get 
            {
                Dictionary<int, Person> res = _actors.Where(person => !string.IsNullOrEmpty(person.Value.Name)
                && person.Value.Movies.Any(movie => !string.IsNullOrEmpty(movie.Title))).ToDictionary();

                foreach (KeyValuePair<int, Person> item in res) 
                {
                    item.Value.Movies = item.Value.Movies
                        .Where(movie => !string.IsNullOrEmpty(movie.Title)).ToHashSet();
                }

				return res;
            }  
        }
        public Dictionary<int, Tag> Tags { 
            get 
            {
                Dictionary<int, Tag> res = _tags.Where(tag => !string.IsNullOrEmpty(tag.Value.TagName)
				&&tag.Value.Movies.Count > 0 && tag.Value.Movies.Any(movie => !string.IsNullOrEmpty(movie.Title))).ToDictionary();

                foreach (KeyValuePair<int, Tag> item in res) 
                {
                    item.Value.Movies = item.Value.Movies
						.Where(movie => !string.IsNullOrEmpty(movie.Title)).ToHashSet();
				}

                return res;
			}  
        }
        
        private ConcurrentDictionary<int, Film> _movies = new ConcurrentDictionary<int, Film>();
        private ConcurrentDictionary<int, Person> _actors = new ConcurrentDictionary<int, Person>();
        private ConcurrentDictionary<int, Tag> _tags = new ConcurrentDictionary<int, Tag>();
        private ConcurrentDictionary<int, int> _codeLinks = new ConcurrentDictionary<int, int>();

        public async Task LoadDataAsync(params string[] fileNames)
        {
            Task firstTask = Task.WhenAll(
                _provider.ReadMoviesCodesAsync(fileNames[0], _moviesInput),
                _provider.CheckMoviesCodesAsync(_moviesInput, _moviesFiltered),
                _provider.ParseMoviesCodesLinesAsync(_moviesFiltered, _moviesOutput),
                _provider.AppendToMoviesDictionaryAsync(_moviesOutput, _movies),

				_provider.ReadActorsFilmsAsync(fileNames[4], _moviesActorsInput),
			    _provider.CheckActorsFilmsLinesAsync(_moviesActorsInput, _moviesActorsFiltered),
				_provider.ParseActorsFilmsLinesAsync(_moviesActorsFiltered, _moviesActorsOutput),
				_provider.AppendActorsDirectorsToMoviesDictionaryAsync(_moviesActorsOutput, _movies, _actors),


				_provider.ReadTagsAsync(fileNames[2], _tagsInput),
                _provider.ParseTagsLinesAsync(_tagsInput, _tagsOutput),
                _provider.AppendTagsToDictionaryAsync(_tagsOutput, _tags),

                _provider.ReadCodesLinksAsync(fileNames[3], _codesLinksInput),
                _provider.ParseCodeLinksLinesAsync(_codesLinksInput, _codesLinksOuput),
                _provider.AppendCodesLinksToDictionaryAsync(_codesLinksOuput, _codeLinks)
                );

            await firstTask;

            Task secondTask = Task.WhenAll(
                   
                    _provider.ReadActorsNamesAsync(fileNames[1], _actorsNamesInput),
				    _provider.ParseActorsNamesLinesAsync(_actorsNamesInput, _actorsNamesFiltered),
                    _provider.CheckActorsNamesLinesAsync(_actorsNamesFiltered, _actorsNamesOuput, _actors),
                    _provider.AppendActorsToDictionaryAsync(_actorsNamesOuput, _actors),
				    
                    _provider.ReadMoviesTagsAsync(fileNames[5], _moviesTagsInput),
                    _provider.CheckTagsFilmsAsync(_moviesTagsInput, _moviesTagsFiltered),
                    _provider.ParseMoviesTagsLinesAsync(_moviesTagsFiltered, _moviesTagsOutput),
                    _provider.AppendTagsToMoviesDictionaryAsync(_moviesTagsOutput, _movies, _tags, _codeLinks),

                    _provider.ReadMoviesRatingsAsync(fileNames[6], _moviesRatingsInput),
                    _provider.ParseMoviesRatingLinesAsync(_moviesRatingsInput, _moviesRatingOutput),
                    _provider.AppendRatingToMoviesDictionaryAsync(_moviesRatingOutput, _movies)
                );

            await secondTask;
        }

        public async Task ReadFiles(params string[] fileNames) 
        {
            await _provider.ReadMoviesCodesAsync(fileNames[0], _moviesInput);
            await _provider.ReadActorsNamesAsync(fileNames[1], _actorsNamesInput);
            await _provider.ReadTagsAsync(fileNames[2], _tagsInput);
            await _provider.ReadCodesLinksAsync(fileNames[3], _codesLinksInput);
            await _provider.ReadActorsFilmsAsync(fileNames[4], _moviesActorsInput);
            await _provider.ReadMoviesTagsAsync(fileNames[5], _moviesTagsInput);
            await _provider.ReadMoviesRatingsAsync(fileNames[6], _moviesRatingsInput);

        }

        public async Task ReadFilesAsync(params string[] fileNames) 
        {
            await Task.WhenAll (
                _provider.ReadMoviesCodesAsync(fileNames[0], _moviesInput),
                _provider.ReadActorsNamesAsync(fileNames[1], _actorsNamesInput),
                _provider.ReadTagsAsync(fileNames[2], _tagsInput),
                _provider.ReadCodesLinksAsync(fileNames[3], _codesLinksInput),
                _provider.ReadActorsFilmsAsync(fileNames[4], _moviesActorsInput),
                _provider.ReadMoviesTagsAsync(fileNames[5], _moviesTagsInput),
                _provider.ReadMoviesRatingsAsync(fileNames[6], _moviesRatingsInput)

                );
        }
    }
}
