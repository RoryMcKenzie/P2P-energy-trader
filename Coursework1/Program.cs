using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

            env.Start();
            Console.ReadLine();
        }
    }
}
