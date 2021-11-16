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
       
            var h1 = new HouseholdAgent(); env.Add(h1, "agent1");
            var h2 = new HouseholdAgent(); env.Add(h2, "agent2");
            var h3 = new HouseholdAgent(); env.Add(h3, "agent3");
            var h4 = new HouseholdAgent(); env.Add(h4, "agent4");
            var h5 = new HouseholdAgent(); env.Add(h5, "agent5");
            var h6 = new HouseholdAgent(); env.Add(h6, "agent6");

            var org1 = new CNPorganiser(); env.Add(org1, "organiser1");

            env.Start();
            Console.ReadLine();
        }
    }
}
