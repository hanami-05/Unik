using Continuations;
using Microsoft.VisualBasic;

Random random = new Random();

int[] exceptionsCodes = [random.Next(0,4), random.Next(0, 4), random.Next(0, 4), random.Next(0, 4), random.Next(0, 4),
    random.Next(0,4), random.Next(0,4)];
//exceptionsCodes = [3,2,1,2,3,3,0];
StartLogic appStartLogic = new StartLogic();

foreach (int code in exceptionsCodes) Console.Write($"{code} ");
Console.WriteLine();

int startTime = DateTime.Now.Hour * 3600 + DateTime.Now.Minute * 60 + DateTime.Now.Second;
try
{
    await appStartLogic.SetUpAcync(exceptionsCodes);
}
catch (Exception e) { Console.WriteLine(e.Message); }


int endTime = DateTime.Now.Hour * 3600 + DateTime.Now.Minute * 60 + DateTime.Now.Second;
Console.WriteLine(endTime - startTime);