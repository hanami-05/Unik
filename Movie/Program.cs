using System.Globalization;
using Movie;
MoviesRepository repository = new MoviesRepository();
repository.MovieAction += PrintMovie;

Console.ReadKey();


static void PrintMovie(Movie.Movie? movie) 
{
    if (movie == null) 
    {
        Console.WriteLine("film does not exists");
        return;
    }

    string res = string.Empty;
    res += $"Title: {movie.Title}\n";
    res += $"Actors: ";
    foreach (string actor in movie.Actors) res += $"{actor}; ";
    res += '\n';
    res += $"Director: {movie.Director}\n";
    res += $"Tags: ";
    foreach (string tag in movie.Tags) res += $"{tag}; ";
    res += '\n';
    res += $"Rating: {movie.Rate}";
}