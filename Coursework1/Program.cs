using System;
using ActressMas;

namespace Coursework1
{
    class Program
    {
        static void Main(string[] args)
        {
            var env = new EnvironmentMas(noTurns: 100);
            var e = new EnvironmentAgent(); env.Add(e, "environment");

            int noHouseholds = 10;

            for (int i = 0; i < noHouseholds; i++)
            {
                var householdAgent = new HouseholdAgent();
                env.Add(householdAgent, $"household{i + 1}");
            }

            var org1 = new CNPorganiser(); env.Add(org1, "organiser");

            env.Start();
            Console.ReadLine();
        }
    }
}
