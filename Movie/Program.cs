using System.Globalization;
using Movie;

MoviesRepository repository = new MoviesRepository();

int startTime = DateTime.Now.Hour * 3600 + DateTime.Now.Minute * 60 + DateTime.Now.Second;
await repository.LoadDataAsync(
    @"C:\Users\79136\Desktop\dataset\ml-latest\MovieCodes_IMDB.tsv",
    @"C:\Users\79136\Desktop\dataset\ml-latest\ActorsDirectorsNames_IMDB.txt",
    @"C:\Users\79136\Desktop\dataset\ml-latest\TagCodes_MovieLens.csv",
    @"C:\Users\79136\Desktop\dataset\ml-latest\links_IMDB_MovieLens.csv",
    @"C:\Users\79136\Desktop\dataset\ml-latest\ActorsDirectorsCodes_IMDB.tsv",
    @"C:\Users\79136\Desktop\dataset\ml-latest\TagScores_MovieLens.csv",
    @"C:\Users\79136\Desktop\dataset\ml-latest\Ratings_IMDB.tsv"
    );
int endTime = DateTime.Now.Hour * 3600 + DateTime.Now.Minute * 60 + DateTime.Now.Second;
Console.WriteLine(endTime - startTime);

Console.WriteLine(repository.Movies[113326]);
Console.ReadKey();
