using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HanoiMoves
{
    public class Disk
    {
        public Disk(int radius)
        {
            Radius = radius;
        }

        public int Radius { get; set; }
    }

    public class Peg
    {
        public Peg(int index, List<Disk> initialConfiguration)
        {
            Stack = new Stack<Disk>();
            Index = index;

            if (initialConfiguration == null)
                return;

            var orderedDisks = initialConfiguration.OrderByDescending(x => x.Radius);

            foreach (var disk in orderedDisks)
            {
                Stack.Push(disk);
            }
        }

        public Stack<Disk> Stack { get; set; }
        public int Index { get; set; }
    }

    public class HanoiSolver
    {
        private List<Disk> _disks = new List<Disk>();
        private List<Peg> _pegs = new List<Peg>();

        public HanoiSolver(int numDiscs, int numPegs, int[] initialConfig)
        {
            for(; numDiscs > 0; --numDiscs)
            {
                _disks.Add(new Disk(numDiscs));
            }

            for (var i = 0; i != numPegs; ++i)
            {
                var pegIndex = i + 1;
                var myDisks = new List<Disk>();

                for (var j = 0; j != initialConfig.Length; ++j)
                {
                    if (initialConfig[j] == pegIndex)
                    {
                        var disk = _disks.Where(x => x.Radius == (j + 1)).FirstOrDefault();
                        if(disk != null) myDisks.Add(disk);
                    }
                }

                _pegs.Add(new Peg(pegIndex, myDisks));
            }
        }

        public void SolveTo(int[] config)
        {
            for(int i = _disks.Count; i > 0; --i)
            {
                Move(i, config[i - 1]);
            }
        }

        private void Move(int disk, int to)
        {
            var pegTo = _pegs.FirstOrDefault(x => x.Index == to);

            if (pegTo.Stack.Any(x => x.Radius == disk))
                // already in peg
                return;

            var pegFrom = _pegs.Where(x => x.Index != to && x.Stack.Any(y => y.Radius == disk)).FirstOrDefault();
            var toFirstDisk = pegTo.Stack.Count == 0 ? 0 : pegTo.Stack.Peek().Radius;
            var fromFirstDisk = pegFrom.Stack.Count == 0 ? 0 : pegFrom.Stack.Peek().Radius;

            if ((toFirstDisk == 0 || toFirstDisk == disk + 1) && (fromFirstDisk == 0 || fromFirstDisk == disk))
            {
                // ready to move
                pegTo.Stack.Push(pegFrom.Stack.Pop());
                Console.WriteLine("{0} {1}", pegFrom.Index, pegTo.Index);
            }
            else
            {
                if (fromFirstDisk > 0 && fromFirstDisk < disk)
                {
                    // need to move disk from the top of "FROM" peg
                    Move(fromFirstDisk, GetFirstAvailablePegFor(fromFirstDisk, new int[] { to }));
                }

                if (toFirstDisk > 0 && toFirstDisk < disk)
                {
                    // need to move disk from the top of "TO" peg
                    Move(toFirstDisk, GetFirstAvailablePegFor(toFirstDisk, new int[] { pegFrom.Index }));
                }

                // let's try it again
                Move(disk, to);
            }
        }

        private int GetFirstAvailablePegFor(int disk, int[] exclude = null)
        {
            var currPeg = _pegs.Where(x => x.Stack.Any(y => y.Radius == disk)).FirstOrDefault();

            var peg = _pegs
                .Where(x => (exclude == null || !exclude.Any(y => y == x.Index) && (x.Stack.Count == 0 || (x.Index != currPeg.Index && x.Stack.Peek().Radius > disk))))
                .OrderBy(x => x.Stack.Count)
                .Select(x => (int?)x.Index)
                .FirstOrDefault();

            if (peg == null)
                return currPeg.Index; // already in place

            return peg.Value;
        }
    }

    public class Program
    {
        static void Main(string[] args)
        {
            var firstLine = Console.ReadLine();
            var firstLineSplit = firstLine.Split(' ');

            var secondLine = Console.ReadLine();
            var secondLineSplit = secondLine.Split(' ').Select(x => Int32.Parse(x)).ToArray();

            var thirdLine = Console.ReadLine();
            var thirdLineSplit = thirdLine.Split(' ').Select(x => Int32.Parse(x)).ToArray();

            Console.WriteLine("---------- SOLUTION ----------");

            var solver = new HanoiSolver(Int32.Parse(firstLineSplit[0]), Int32.Parse(firstLineSplit[1]), secondLineSplit);
            solver.SolveTo(thirdLineSplit);

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
