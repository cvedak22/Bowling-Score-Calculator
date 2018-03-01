using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bowling
{
    public class Chance
    {
        public string Player { get; set; }
        public bool IsFoul { get; set; }
        public int Pins { get; set; }

        public static Chance FromLine(string line)
        {
            var chunks = line.Split();
            int pins = 0;
            bool isFoul = false;
            // assuming file is valid, if second part of string is number, then it is not foul
            isFoul = !Int32.TryParse(chunks[1], out pins);

            var chance = new Chance
            {
                Player = chunks[0],
                IsFoul = isFoul,
                Pins = pins
            };
            return chance;
        }
    }

    public class Frame
    {
        public string Player { get; set; }
        public List<Chance> Chances { get; set; }
        public int Score { get; set; }
        public bool IsSpare { get; set; }
        public bool IsStrike { get; set; }

        public Frame()
        {
            Chances = new List<Chance>();
        }

        public static List<Frame> MakeFromChancesList(IEnumerable<Chance> chances)
        {
            var result = new List<Frame>();
            foreach (var c in chances)
            {
                // if list is empty or player name is changed, or it was a strike, or no more chances available on current frame, start new frame
                var isNewMove = result.Count() == 0 || result.Last().Player != c.Player || (result.Count() < 10 && (result.Last().Chances.Count > 1 || result.Last().Chances.Select(x => x.Pins).Sum() == 10));
                var frame = isNewMove ? new Frame() : result.Last();
                frame.Player = c.Player;
                frame.Chances.Add(c);
                if (isNewMove)
                {
                    result.Add(frame);
                }
            }
            return result;
        }
    }

    public class Game
    {
        public List<Frame> Frames { get; set; }

        public Game()
        {
            Frames = new List<Frame>();
        }

        private void CalculateScores()
        {
            var goupedByPlayer = Frames.GroupBy(x => x.Player).Select(x => x.ToList()).ToList();
            foreach (var playerFrames in goupedByPlayer)
            {
                foreach (var frame in playerFrames)
                {
                    // if was 1 chance and all pins are hit, it's strike
                    if (frame.Chances.Count == 1 && frame.Chances.Select(x => x.Pins).Sum() == 10)
                    {
                        frame.IsStrike = true;
                    }
                    else if (frame.Chances.Select(x => x.Pins).Sum() == 10)
                    {
                        frame.IsSpare = true;
                    }
                }
            }

            foreach (var playerFrames in goupedByPlayer)
            {
                var sum = 0;
                foreach (var frame in playerFrames.Select((x, i) => new { f = x, i = i }))
                {
                    if (frame.f.IsStrike)
                    {
                        sum += 10;
                        sum += playerFrames.Skip(frame.i + 1).SelectMany(x => x.Chances.Select(c => c.Pins)).Take(2).Sum();
                    }
                    else if (frame.f.IsSpare)
                    {
                        sum += 10;
                        sum += playerFrames.Skip(frame.i + 1).SelectMany(x => x.Chances.Select(c => c.Pins)).Take(1).Sum();
                    }
                    else
                    {
                        sum += frame.f.Chances.Select(x => x.Pins).Sum();
                    }
                    frame.f.Score = sum;
                }
            }

        }

        public static Game FromLines(string[] lines)
        {
            var game = new Game();
            var chances = lines.Select(x => Chance.FromLine(x));
            var frames = Frame.MakeFromChancesList(chances);
            game.Frames = frames;
            game.CalculateScores();
            return game;
        }

        public void Print()
        {
            Console.Write("Frame");
            foreach (var i in Enumerable.Range(1, 10))
            {
                Console.Write("\t\t" + i);
            }
            Console.WriteLine();

            var goupedByPlayer = Frames.GroupBy(x => x.Player);
            foreach (var frames in goupedByPlayer)
            {
                Console.WriteLine(frames.Key);

                Console.Write("Pinfalls\t");
                foreach (var frame in frames)
                {
                    if (frame.IsStrike)
                    {
                        Console.Write("\tX\t");
                    }
                    else if (frame.IsSpare)
                    {
                        Console.Write(frame.Chances.First().Pins);
                        Console.Write("\t/\t");
                    }
                    else
                    {
                        foreach (var chance in frame.Chances)
                        {
                            if (chance.Pins == 10)
                            {
                                Console.Write("X\t");
                            }
                            else if (chance.IsFoul)
                            {
                                Console.Write("F\t");
                            }
                            else
                            {
                                Console.Write(chance.Pins + "\t");
                            }
                        }
                    }
                }
                Console.WriteLine();
                Console.Write("Score\t\t");
                foreach (var frame in frames)
                {
                    Console.Write(frame.Score + "\t\t");
                }
                Console.WriteLine();

            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Maximize the window and press enter to view the scores!!");
            Console.ReadLine();
            var filename = args[0];
            var lines = File.ReadAllLines(filename);
            var game = Game.FromLines(lines);
            game.Print();
            Console.WriteLine("Thank you! Press enter to exit from the window.");
            Console.ReadLine();
        }
    }
}
