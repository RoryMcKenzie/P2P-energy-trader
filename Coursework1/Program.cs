using System;
using System.IO;
using System.Text;
using System.Diagnostics;
using ActressMas;

namespace Coursework1
{
    class Program
    {
        static void Main(string[] args)
        {

            //Stopwatch was used to time program for report
            //Stopwatch watch = new Stopwatch();
            //watch.Start();

            var env = new EnvironmentMas(noTurns: 1000, randomOrder: false, parallel: false);
            EnvironmentAgent e = new EnvironmentAgent(); env.Add(e, "environment");

            for (int i = 0; i < Globals.householdAgentNo; i++)
            {
                var householdAgent = new HouseholdAgent();
                env.Add(householdAgent, $"household{i + 1}");
            }

            CNPorganiser org1 = new CNPorganiser(); env.Add(org1, "organiser");

            Globals.broadcastNo = Globals.householdAgentNo + 1; //households + organiser + environmentagent, but -1 because broadcast doesn't include sender

            //Below is code to write to a csv file

            /* string sellerFilepath = @"C:\Users\rorym\Documents\General\Uni\MAS\seller_amountToSell.csv";
            string buyerFilepath = @"C:\Users\rorym\Documents\General\Uni\MAS\buyer_amountToSell.csv"; 
            string countFilepath = @"C:\Users\rorym\Documents\General\Uni\MAS\count_50.csv"; */

            env.Start();

            /*File.AppendAllText(sellerFilepath, Globals.sellercsv.ToString());
            File.AppendAllText(sellerFilepath, "\n");

            File.AppendAllText(buyerFilepath, Globals.buyercsv.ToString());
            File.AppendAllText(buyerFilepath, "\n"); */

            //watch.Stop();

            //string total = Globals.messageCount + "," + watch.ElapsedMilliseconds.ToString();

            //Console.WriteLine(watch.ElapsedMilliseconds.ToString());

            //Console.WriteLine("message count: " + Globals.messageCount);

           //File.AppendAllText(countFilepath, total);
           //File.AppendAllText(countFilepath, "\n");

            Console.ReadLine();
        }
    }

    public static class Globals
    {
        public static int householdAgentNo = 10;

        //Used for writing to csv
        //public static StringBuilder buyercsv = new StringBuilder();
        //public static StringBuilder sellercsv = new StringBuilder();
       
        //Used for counting messages for scaling purposes in report
        public static int messageCount = 0;
        public static int broadcastNo = 0;
    }
}
