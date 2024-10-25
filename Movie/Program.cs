using System.Globalization;
using Movie;
MoviesRepository repository = new MoviesRepository();
repository.MovieAction += PrintMovie;

int startTime = DateTime.Now.Hour * 3600 + DateTime.Now.Minute * 60 + DateTime.Now.Second;

Task fillDataTask = repository.FillDataAsync
    (
        @"C:\Users\79136\Desktop\dataset\ml-latest\ActorsDirectorsCodes_IMDB.tsv",
        @"C:\Users\79136\Desktop\dataset\ml-latest\MovieCodes_IMDB.tsv",
        @"C:\Users\79136\Desktop\dataset\ml-latest\Ratings_IMDB.tsv",
        @"C:\Users\79136\Desktop\dataset\ml-latest\ActorsDirectorsNames_IMDB.txt",
        @"C:\Users\79136\Desktop\dataset\ml-latest\links_IMDB_MovieLens.csv",
        @"C:\Users\79136\Desktop\dataset\ml-latest\TagCodes_MovieLens.csv",
        @"C:\Users\79136\Desktop\dataset\ml-latest\TagScores_MovieLens.csv"
    );

Console.WriteLine("Enter film name: ");
string title = Console.ReadLine();

await fillDataTask;
int endTime = DateTime.Now.Hour * 3600 + DateTime.Now.Minute * 60 + DateTime.Now.Second;

Console.WriteLine(endTime - startTime);

Movie.Movie? movie = repository.GetMovieByTitle(title);
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