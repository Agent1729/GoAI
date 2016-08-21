using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoAI
{
	public class PlayerConsoleAI : AI
	{
		public int cx = -1;
		public int cy = -1;
		TwoInts move = null;

		public override TwoInts takeTurn(Board b)
		{
			if (cx == -1) cx = b.width / 2;
			if (cy == -1) cy = b.height / 2;
			Char c=' ';
			move = null;
			while (move == null)
			{
				printConsole(b);

				c = getKeyboardKey();

				if (c == 'a') moveLeft(b);
				else if (c == 'd') moveRight(b);
				else if (c == 'w') moveUp(b);
				else if (c == 's') moveDown(b);
				else if (c == 'e') move = new TwoInts(cx, cy);
				else if (c == 'p') move = new TwoInts(-1, -1);
				else if (c == 'q') move = new TwoInts(-2, -2);
			}
			return move;
		}

		void moveLeft(Board b) { if (cx > 0) cx--; }
		void moveRight(Board b) { if (cx < b.width - 1) cx++; }
		void moveDown(Board b) { if (cy < b.height - 1) cy++; }
		void moveUp(Board b) { if (cy > 0) cy--; }
		void placePiece(Board b)
		{
			move = new TwoInts(cx, cy);
		}
		void pass()
		{
			move = new TwoInts(-1, -1);
		}
		char getKeyboardKey()
		{
			char c=' ';
			while (!Console.KeyAvailable);
			ConsoleKey ck = Console.ReadKey(true).Key;
			if (ck == ConsoleKey.LeftArrow) c = 'a';
			else if (ck == ConsoleKey.RightArrow) c = 'd';
			else if (ck == ConsoleKey.UpArrow) c = 'w';
			else if (ck == ConsoleKey.DownArrow) c = 's';
			else if (ck == ConsoleKey.Spacebar) c = 'e';
			else if (ck == ConsoleKey.Enter) c = 'p';
			else if (ck == ConsoleKey.Escape) c = 'q';
			return c;
		}
		void printConsole(Board b)
		{
			Console.Clear();
			b.printScore(false);
			b.printBoard(cx, cy);
		}
	}
}
