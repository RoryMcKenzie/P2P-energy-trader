using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ActressMas;

namespace Coursework1
{
    class HouseholdAgent : Agent
    {
        public override void Setup()
        {
            Send("environment", "start");
        }

        public override void ActDefault()
        {
            
        }

        public override void Act(Message message)
        {
            //Console.WriteLine(message.Format());
        }
    }
}
