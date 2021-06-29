using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Diagnostics;
namespace BagageSorteringssystem
{
    public class LuggageProducer
    {
        //fields
        private int timeFactor;

        //eventhandler
        public EventHandler LuggageChanged;

        //properties
        public int TimeFactor
        {
            get { return timeFactor; }
            set { timeFactor = value; }
        }

        //constructor
        public LuggageProducer()
        {
            this.timeFactor = 1;
        }

        //get flightplan closest to current time
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

        //generate new  luggage
        public Luggage GenerateLuggage()
        {
            //initialize random number, just for sanity check ;)
            Random rand = new Random();
            int flightIndex = rand.Next(10);

            //get array of flight indexes closest to current time
            int[] flights = GetClosestFlightplan();

            
            //select random flight index from array
            if (flights.Length > 0)
            {
                flightIndex = flights[rand.Next(flights.Length)];
            }

            //generate  new luggage based on flight data
            Luggage luggage = new Luggage(Manager.flightPlans[flightIndex], rand.Next(10000));

            return luggage;
        }

        //adding new luggage to arrival buffer
        public void AddToBuffer()
        {
            Random rnd = new Random();
            int luggagecount = 99;
            while (true)
            {
                Monitor.Enter(Manager.ArrivalBuffer);
                try
                {
                    //produce luggage if any gates are open
                    if (Manager.AvailableGates > 0)
                    {
                        // halt thread if arrival buffer is full
                        if (Manager.ArrivalBuffer.InternalLength == Manager.ArrivalBuffer.Length - 1)
                        {
                            Monitor.Wait(Manager.ArrivalBuffer);
                        }

                        //add new luggage to arrival buffer
                        Luggage luggage = GenerateLuggage();
                        Manager.ArrivalBuffer.Add(luggage);
                        luggagecount = Manager.ArrivalBuffer.InternalLength;
                        
                        //invoke event
                        LuggageChanged?.Invoke(this, new LuggageCounterEventArgs(luggagecount));
                    }
                }
                finally
                {
                    Monitor.Exit(Manager.ArrivalBuffer);
                }
                int delay = rnd.Next(500, 1500);
                Thread.Sleep(delay / timeFactor);
            }
        }
    }
}
