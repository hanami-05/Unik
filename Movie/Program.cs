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

int c = 0;
int k = 0;
foreach (var item in dict2)
{
    if (item.Value.Movies.Count >= 1) k++;
}

foreach (var item in dict) 
{
    if (item.Value.Movies.Count >= 1) c++;
}
Console.WriteLine(c);
Console.WriteLine(k);
Console.ReadKey();
