using System;

namespace PerfTestClient
{
    class Program
    {
        static void Main(string[] args)
        {
            const int agentsCount = 10;
            var agents = new PerfTestAgent[agentsCount];
            for (int i = 0; i < agentsCount; i++)
            {
                agents[i] = new PerfTestAgent(i);
                agents[i].Start();
            }

            Console.ReadKey();
        }   
    }
}
