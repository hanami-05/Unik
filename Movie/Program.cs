using System.Globalization;
using Movie;

MoviesRepository repository = new MoviesRepository();

int startTime = DateTime.Now.Hour*3600 + DateTime.Now.Minute*60 + DateTime.Now.Second;
await repository.LoadDataAsync(
    @"C:\Users\79136\Desktop\dataset\ml-latest\MovieCodes_IMDB.tsv",
    @"C:\Users\79136\Desktop\dataset\ml-latest\ActorsDirectorsNames_IMDB.txt",
    @"C:\Users\79136\Desktop\dataset\ml-latest\TagCodes_MovieLens.csv",
    @"C:\Users\79136\Desktop\dataset\ml-latest\links_IMDB_MovieLens.csv",
    @"C:\Users\79136\Desktop\dataset\ml-latest\ActorsDirectorsCodes_IMDB.tsv",
    @"C:\Users\79136\Desktop\dataset\ml-latest\TagScores_MovieLens.csv",
    @"C:\Users\79136\Desktop\dataset\ml-latest\Ratings_IMDB.tsv"
    );
int endTime = DateTime.Now.Hour*3600 + DateTime.Now.Minute*60 + DateTime.Now.Second;
Console.WriteLine($"Ready: {endTime - startTime}");

var dict = repository.Tags;
var dict2 = repository.ActorsAndDirectors;
var dict3 = repository.Movies;
int c = 0;
int k = 0;
int d = 0;
int h = 0;
foreach (var item in dict2)
{
    if (item.Value.Movies.Count >= 1) k++;
}

foreach (var item in dict) 
{
    if (item.Value.Movies.Count >= 1) c++;
}
foreach (var item in dict3) 
{
    if (item.Value.Tags.Count >= 1) h++;
    if (item.Value.Actors.Count >= 1 || item.Value.Director != null) d++;
}
Console.WriteLine(c);
Console.WriteLine(k);
Console.WriteLine(d);
Console.WriteLine(h);
Console.ReadKey();
