using System.Diagnostics;
using System.IO.Enumeration;
using System.Text.RegularExpressions;
using System.Text;
using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;
using System.Timers;

namespace PLINQ 
{
    internal class Program
    {
        static string path = @"C:\Users\79136\Desktop\books";
        static void Main(string[] args)
        {   
            string[] files = Directory.GetFiles(path);
			int startTime = DateTime.Now.Hour * 3600000 + DateTime.Now.Minute * 60000 + DateTime.Now.Second * 1000
				+ DateTime.Now.Millisecond;

			foreach (string file in files) 
            {
                Console.WriteLine(file);
                Console.WriteLine();
                ReadFileLinesWithLINQ(file);
            }

            int endTime = DateTime.Now.Hour * 3600000 + DateTime.Now.Minute * 60000 + DateTime.Now.Second * 1000
                + DateTime.Now.Millisecond;
            Console.WriteLine($"Total LINQ Time: {endTime - startTime}");
            Console.WriteLine();
            Console.WriteLine();
            
            startTime = DateTime.Now.Hour * 3600000 + DateTime.Now.Minute * 60000 + DateTime.Now.Second * 1000
				+ DateTime.Now.Millisecond;

			foreach (string file in files)
			{
				Console.WriteLine(file);
				Console.WriteLine();
				ReadFileLinesWithPLINQ(file);
			}

			endTime = DateTime.Now.Hour * 3600000 + DateTime.Now.Minute * 60000 + DateTime.Now.Second * 1000
			   + DateTime.Now.Millisecond;
			Console.WriteLine($"Total PLINQ Time: {endTime - startTime}");
		}
        static void ReadFileLinesWithLINQ(string fileName) 
        {
			using (StreamReader reader = new StreamReader(fileName, Encoding.UTF8))
			{
				string[] sentences = reader.ReadToEnd().Split('\n');
				int linesCount = sentences.Length;

				Console.WriteLine($"Lines count: {linesCount}");
				Console.WriteLine();

				var plinqQuery = sentences
					.SelectMany(sentence => Regex.Split(sentence, @"\W+"))
					.Where(word => !string.IsNullOrWhiteSpace(word))
					.GroupBy(word => word.ToLower())
					.Select(group => new
					{
						Word = group.Key,
						Count = group.Count()
					})
					.OrderByDescending(el => {
						if (el.Word.Length >= 5) return el.Count;
						return -1;
					})
					;

				int wordsCount = plinqQuery.Sum(el => el.Count);
				Console.WriteLine($"Words count: {wordsCount}");
				Console.WriteLine();

				int count = 0;
				foreach (var item in plinqQuery)
				{
					if (count == 10) break;
					Console.WriteLine($"{item.Word} : {item.Count}");
					count++;
				}

				Console.WriteLine();
			}
		} 

        static void ReadFileLinesWithPLINQ(string fileName)
        {
            using (StreamReader reader = new StreamReader(fileName, Encoding.UTF8)) 
            {
                string[] sentences = reader.ReadToEnd().Split('\n');
                int linesCount = sentences.Length;

                Console.WriteLine($"Lines count: {linesCount}");
                Console.WriteLine();

                var plinqQuery = sentences
                    .AsParallel()
                    .SelectMany(sentence => Regex.Split(sentence, @"\W+"))
                    .Where(word => !string.IsNullOrWhiteSpace(word))
                    .GroupBy(word => word.ToLower())
                    .Select(group => new
                    {
                        Word = group.Key,
                        Count = group.Count()
                    })
                    .OrderByDescending(el => {
                        if (el.Word.Length >= 5) return el.Count;
                        return -1;
                     })
                    ;

                int wordsCount = plinqQuery.Sum(el => el.Count);
                Console.WriteLine($"Words count: {wordsCount}");
                Console.WriteLine();

                int count = 0;
                foreach (var item in plinqQuery) 
                {
                    if (count == 10) break;
                    Console.WriteLine($"{item.Word} : {item.Count}");
                    count++;
                }

                Console.WriteLine();
            } 
        }
    }
}