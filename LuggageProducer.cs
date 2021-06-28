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
        private int timeFactor;

        public LuggageProducer()
        {
            this.timeFactor = 1;
        }

        public int TimeFactor
        {
            get { return timeFactor; }
            set { timeFactor = value; }
        }

        int[] GetClosestFlightplan()
        {
            List<int> fligts = new List<int>();
           
            Monitor.Enter(Manager.flightPlans);
            try
            {
                int currentHour = FlightManager.CurrentTime.Hour;
               
                for (int i = 0; i < Manager.flightPlans.Length; i++)
                {
                    int timedifference = Manager.flightPlans[i].ArrivalTime.Hour - currentHour;

                    if (timedifference <= 2)
                    {
                        fligts.Add(i);
                    }
                }
            }
           finally
            {
                Monitor.Exit(Manager.flightPlans);
            }
           
            return fligts.ToArray();
        }
        public Luggage GenerateLuggage()
        {
            Random rand = new Random();
            int flightIndex = rand.Next(10);
              int[] flights = GetClosestFlightplan();

            //get first flightplan from opened check in
            //  flightIndex = Manager.AvailableGates[rand.Next(Manager.AvailableGates.Count)];
            if (flights.Length > 0)
            {
                flightIndex = flights[rand.Next(flights.Length)];
            }

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
                    if (Manager.AvailableGates > 0)
                    {
                        if (Manager.ArrivalBuffer.InternalLength == Manager.ArrivalBuffer.Length - 1)
                        {
                            Monitor.Wait(Manager.ArrivalBuffer);
                        }

                        Luggage luggage = GenerateLuggage();
                        Manager.ArrivalBuffer.Add(luggage);
                        luggagecount = Manager.ArrivalBuffer.InternalLength;
                      //   Debug.WriteLine("adding luggage");
                        //invoke event
                        LuggageChanged?.Invoke(this, new LuggageCounterEventArgs(luggagecount));
                    }


                }
                finally
                {

                    Monitor.Exit(Manager.ArrivalBuffer);

                }
                int delay = rnd.Next(500, 1500);
                // Debug.WriteLine("producer: " + delay / timeFactor);
                Thread.Sleep(delay / timeFactor);

            }
        }
    }
}
