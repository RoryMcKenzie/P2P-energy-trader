using System;
using System.IO;
using System.Text;
using ActressMas;

namespace Coursework1
{
    class Program
    {
        static void Main(string[] args)
        {
            var env = new MyEnv(noTurns: 100);
            var e = new EnvironmentAgent(); env.Add(e, "environment");

            int noHouseholds = 10;

            for (int i = 0; i < noHouseholds; i++)
            {
                var householdAgent = new HouseholdAgent();
                env.Add(householdAgent, $"household{i + 1}");
            }

            var org1 = new CNPorganiser(); env.Add(org1, "organiser");

            var sellerFilepath = @"C:\Users\rorym\Documents\General\Uni\MAS\seller_results.csv";
            var buyerFilepath = @"C:\Users\rorym\Documents\General\Uni\MAS\buyer_results.csv";

            env.Start();
          
            File.AppendAllText(sellerFilepath, Globals.sellercsv.ToString());
            File.AppendAllText(sellerFilepath, "\n");

            File.AppendAllText(buyerFilepath, Globals.buyercsv.ToString());
            File.AppendAllText(buyerFilepath, "\n");
            //Console.ReadLine();
        }
    }

    public static class Globals
    {
        public static StringBuilder buyercsv = new StringBuilder();
        public static StringBuilder sellercsv = new StringBuilder();
    }

    public class MyEnv : EnvironmentMas
    {
        public MyEnv(int noTurns = 0, int delayAfterTurn = 0, bool randomOrder = true, Random rand = null, bool parallel = false)
            : base(noTurns, delayAfterTurn, randomOrder, rand, parallel)
        {
            Memory["Turn"] = 0;
        }

        public override void TurnFinished(int turn)
        {
            Memory["Turn"] = turn + 1;  // turn is updated after TurnFinished
        }
    }
}
