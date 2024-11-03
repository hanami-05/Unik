using Continuations;
using Microsoft.VisualBasic;

Random random = new Random();

int[] randomValues = [random.Next(0, 2), random.Next(0, 2), random.Next(0, 2), random.Next(0, 2)];
bool[] flags = randomValues.Select(Convert.ToBoolean).ToArray();

//flags = [false, false, true, false];

StartLogic appStartLogic = new StartLogic();

foreach (bool code in flags) Console.Write($"{code} ");
Console.WriteLine();

int startTime = DateTime.Now.Hour * 3600 + DateTime.Now.Minute * 60 + DateTime.Now.Second;

try { await appStartLogic.SetUpAcync(flags); }
catch { }

int endTime = DateTime.Now.Hour * 3600 + DateTime.Now.Minute * 60 + DateTime.Now.Second;
Console.WriteLine(endTime - startTime);