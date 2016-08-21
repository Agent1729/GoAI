using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;

namespace GoAI
{
	class Program
	{

		static void Main(string[] args)
		{
			Game g;
			Random r = new Random();
			bool output = true;
			int contests = 100;
			int generations = 1;
			int boardwidth = 9;
			int boardheight = 9;
			//PlayerConsoleAI aip1 = new PlayerConsoleAI();
			//PlayerConsoleAI aip2 = new PlayerConsoleAI();
			//RandomAI ai2 = new RandomAI();

			int numAI = 2;
			AI[] ai = new AI[numAI];
			String[] files = new String[numAI];
			List<List<double>> aivals;
			//ai[0] = new PlayerConsoleAI();
			ai[0] = new AI1(); files[0] = "ai1.txt";
			ai[1] = new AI1(); files[1] = "ai1.txt";

			for (int generation = 0; generation < generations; generation++)
			{
				//Console.WriteLine("Generation " + generation + "/" + generations);
				aivals = new List<List<double>>();
				//Read in ai
				for (int i = 0; i < numAI; i++)
				{
					aivals.Add(new List<double>());
					aivals[i] = setupAI(ai[i], aivals[i], files[i], r);
				}

				//Setup wins variable
				int winner;
				int[] wins = new int[numAI];
				for (int i = 0; i < numAI; i++)
					wins[i] = 0;

				//Play the games
				for (int i = 0; i < contests; i++)
				{
					Console.WriteLine("Generation " + generation + "/" + generations + "\t\tGame " + i + "/" + contests);
					g = new Game();
					winner = g.play(ai[0], ai[1], boardwidth, boardheight);
					if(winner>=0)
						wins[winner - 1]++;
					//Console.ReadLine();
				}

				//Find winner
				int max = wins[0];
				winner = 0;
				for (int i = 1; i < numAI; i++)
					if (wins[i] > max)
					{
						winner = i;
						max = wins[i];
					}

				//Output results
				Console.Clear();
				Console.WriteLine("\n\n" + wins[0] + " " + wins[1] + " " + winner);
				if (output) writeAiVals(files[winner], aivals[winner]);
				//Console.ReadLine();
			}
		}

		static List<double> setupAI(AI ai, List<double> aivals, String fileName, Random r)
		{
			aivals = new List<double>();
			readInAiVals(fileName, aivals);
			Console.Write("Got from " + fileName + ": "); foreach (double d in aivals) Console.Write("" + d + ","); Console.WriteLine();
			randomizeVals(aivals, r);
			Console.Write("Randomized to: "); foreach (double d in aivals) Console.Write("" + d + ","); Console.WriteLine();
			ai.setAI(1, aivals);
			return aivals;
		}

		static bool readInAiVals(String file, List<double> aivals)
		{
			bool passed = true;
			try
			{
				using (StreamReader reader = new StreamReader(file))
				{
					string line;
					int i = 0;
					while ((line = reader.ReadLine()) != null)
					{
						string[] parts = line.Split(',');
						foreach(string s in parts)
						{
							double d = 1;
							if (Double.TryParse(s, out d))
								aivals.Add(d);
						}
					}
					
				}
			}
			catch (Exception e)
			{
				//Console.WriteLine(e.Message);
				//Console.WriteLine(e.StackTrace);
				//Console.ReadLine();
				//using (StreamWriter writer = new StreamWriter(file))
				//{
				//	writer.Write("-1");
				//}
				passed = false;
			}
			if (aivals.Count == 0)
				passed = false;
			if(!passed)
				for (int i = 0; i < 20; i++)
					aivals.Add(1);
			return true;
		}

		static bool writeAiVals(String file, List<double> aivals)
		{
			using (StreamWriter writer = new StreamWriter(file))
			{
				for (int i = 0; i < aivals.Count - 1; i++)
					writer.Write("" + aivals[i] + ",");
				if (aivals.Count - 1 > 0)
					writer.Write("" + aivals[aivals.Count - 1]);
			}
			return true;
		}

		static void randomizeVals(List<double> aivals, Random r)
		{
			//Random r = new Random();
			double mult;
			for (int i = 0; i < aivals.Count; i++)
			{
				mult = r.NextDouble() * .20 + .9;
				aivals[i] *= mult;
			}
		}
	}
}