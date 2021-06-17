using System;
using System.Threading;
namespace BagageSorteringssystem
{
    class Program
    {
        static void Main(string[] args)
        {
            Manager man = new Manager();
            FlightManager flightManager = new FlightManager();
             man.RunSim();

         //   flightManager.RunTime();


           


            Console.Read();
        }
    }
}
