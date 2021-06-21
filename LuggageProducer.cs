using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
namespace BagageSorteringssystem
{
    public class LuggageProducer
    {



        public Luggage GenerateLuggage()
        {
            Random rand = new Random();
           /* for (int i = 0; i < Manager.AvailableGates.Count; i++)
            {
                Console.WriteLine(Manager.AvailableGates[i]);
                Console.WriteLine(Manager.flightPlans[Manager.AvailableGates[i]].Destination);
            }
            Thread.Sleep(3000);*/
           
            //get first flightplan from opened check in
            int flightIndex = Manager.AvailableGates[rand.Next(Manager.AvailableGates.Count)];
            Luggage luggage = new Luggage(Manager.flightPlans[flightIndex], rand.Next(10000));
            return luggage;

        }

        public void AddToBuffer()
        {
            Random rnd = new Random();
            while (true)
            {
                Monitor.Enter(Manager.ArrivalBuffer);
                try
                {
                    if (Manager.AvailableGates.Count > 0)
                    {
                        Luggage luggage = GenerateLuggage();
                        Manager.ArrivalBuffer.Add(luggage);
                    }


                }
                finally
                {

                    Monitor.Exit(Manager.ArrivalBuffer);

                }
                Thread.Sleep(rnd.Next(500, 1000));

            }
        }
    }
}
