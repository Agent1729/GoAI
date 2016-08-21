using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoAI
{
	public class Game
	{
		public int cx = 4;
		public int cy = 4;
		public int player = 1;
		public int width;
		public int height;
		public bool passed = false;
		public bool end = false;
		public int turn = 0;

		public int play(AI ai1, AI ai2, int sizex, int sizey)
		{
			width = sizex;
			height = sizey;
			Board b = new Board(sizex, sizey);
			TwoInts move;

			do
			{
				//Console.Clear();
				//b.printScore(false);
				//b.printBoard(cx, cy);
				//b.printGroups();
				//b.printTerritories();

				move = new TwoInts(-1,-1);
				if (player == 1)
					move = ai1.takeTurn(b);
				if (player == 2)
					move = ai2.takeTurn(b);

				if (move.a == -1 && move.b == -1) pass();
				else if (move.a == -2 && move.b == -2) break;
				else
				{
					passed = false;
					cx = move.a;
					cy = move.b;
					placePiece(b);
				}
				if (end)
					break;
				//Console.WriteLine("Turn " + turn + "\tScores: " + b.playerScore[0] + " " + b.playerScore[1]);
				turn++;
				if (turn > b.width * b.height * 2)
					return -1;
			} while (move.a != -2 && move.b != -2);

			//removeDeadGroups(b);
			printFinal(b);
			return getWinner(b);
		}
		void placePiece(Board b)
		{
			if(b.placePiece(cx, cy, player, true))
			{
				player = 3 - player;
			}
		}
		void pass()
		{
			if (!passed)
			{
				player = 3 - player;
				passed = true;
			}
			else
			{
				end = true;
			}
		}
		void removeDeadGroups(Board b)
		{
			List<Group> deadGroups = b.findDeadGroups();
			foreach (Group g in deadGroups)
			{
				g.kill(b);
			}
			b.groups=b.groups.Except(deadGroups).ToList();
		}
		int getWinner(Board b) { return b.getWinner(); }



		void printFinal(Board b)
		{
			Console.Clear();
			b.printBoard(cx, cy);
			b.printScore(true);
			b.printWinner();
		}
	}
}