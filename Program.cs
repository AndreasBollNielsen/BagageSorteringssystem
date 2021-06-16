using System;
using System.Threading;
namespace BagageSorteringssystem
{
    class Program
    {
        static void Main(string[] args)
        {
            Manager man = new Manager();

            // man.RunSim();


            ConsoleManager console = new ConsoleManager();
            console.PrintData();


            Console.Read();
        }
    }
}
