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
            Luggage luggage = new Luggage(Manager.flightPlans[rand.Next(Manager.flightPlans.Length)], rand.Next(10000));

            return luggage;
        }

        public void AddToBuffer()
        {
            Random rnd = new Random();
            while (true)
            {
                Monitor.Enter(Manager.CheckinBuffer);
                try
                {
                    Luggage luggage = GenerateLuggage();
                    Manager.CheckinBuffer.Add(luggage);

                }
                finally
                {

                    Monitor.Exit(Manager.CheckinBuffer);

                }
                Thread.Sleep(rnd.Next(1000, 2000));

            }
        }
    }
}
