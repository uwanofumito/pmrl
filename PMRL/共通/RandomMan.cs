using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMRL
{
    public class RandomMan
    {
        // ランダムなbool値を次々生成

        public static bool getBitRand()
        {
            return (rand.Next(2) == 1 ? true : false);
        }

        public static double getRand(double max)
        {
            return rand.NextDouble() * max;
        }

        public static int getRand(int max)
        {
            return rand.Next(max);
        }

        public static void setSeed(int seed)
        {
            rand = new Random(seed);
        }

        public static Random rand = new Random();
    }
}
