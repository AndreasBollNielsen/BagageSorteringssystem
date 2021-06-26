using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Diagnostics;
namespace BagageSorteringssystem
{
    public class LuggageProducer
    {
        public EventHandler LuggageChanged;


        public Luggage GenerateLuggage()
        {
            Random rand = new Random();
           

            //get first flightplan from opened check in
            int flightIndex = Manager.AvailableGates[rand.Next(Manager.AvailableGates.Count)];
            Luggage luggage = new Luggage(Manager.flightPlans[flightIndex], rand.Next(10000));
            return luggage;

        }

        public void AddToBuffer()
        {
            Random rnd = new Random();
            int luggagecount = 99;
            while (true)
            {
                Monitor.Enter(Manager.ArrivalBuffer);
                try
                {
                    if (Manager.AvailableGates.Count > 0)
                    {
                        if(Manager.ArrivalBuffer.InternalLength == Manager.ArrivalBuffer.Length -1)
                        {
                            Monitor.Wait(Manager.ArrivalBuffer);
                        }

                        Luggage luggage = GenerateLuggage();
                        Manager.ArrivalBuffer.Add(luggage);
                        luggagecount = Manager.ArrivalBuffer.InternalLength;
                       // Debug.WriteLine("adding luggage");
                        //invoke event
                        LuggageChanged?.Invoke(this, new LuggageCounterEventArgs(luggagecount));
                    }


                }
                finally
                {

                    Monitor.Exit(Manager.ArrivalBuffer);

                }
                int delay = rnd.Next(50, 250);
                Thread.Sleep(delay);

            }
        }
    }
}
