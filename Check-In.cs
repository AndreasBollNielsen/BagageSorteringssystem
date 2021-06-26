using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Diagnostics;
namespace BagageSorteringssystem
{
     class Check_In
    {
        public enum Status { open, closed };
        private string name;
        private Gate departureGate;
        private Status myStatus;


        public Gate DepartureGate
        {
            get { return departureGate; }
            set { departureGate = value; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public Status MyStatus
        {
            get { return myStatus; }
            set
            {
                myStatus = value;

            }
        }

        public Check_In(string name)
        {
            Name = name;
        }

        void AddToGateBuffer()
        {
            Random rand = new Random();

            Monitor.Enter(Manager.ArrivalBuffer);
            try
            {
                //awake luggage producer 
                if (Manager.ArrivalBuffer.InternalLength < Manager.ArrivalBuffer.Length)
                {
                   // Debug.WriteLine("pulsing");
                    Monitor.PulseAll(Manager.ArrivalBuffer);
                }

                if (Manager.ArrivalBuffer.InternalLength > 0)
                {
                    Luggage luggage = Manager.ArrivalBuffer.Remove();
                    //add luggage to gate buffer
                    luggage.Flight.DepartureGate = Manager.gates[luggage.Flight.IndexNumber];
                    Manager.CheckInBuffer.Add(luggage);
                }

            }
            finally
            {
                Monitor.Exit(Manager.ArrivalBuffer);
            }

            //get current time factor
            int factor = 1;
                 factor = (int)FlightManager.TimeFactor;
           /* Monitor.Enter(FlightManager.TimeFactor);
            try
            {
            }
           finally
            {
                Monitor.Exit(FlightManager.TimeFactor);
            }*/
            int delay = rand.Next(1000, 2500);
          Thread.Sleep(delay/ factor);

        }

        public int Getgate(FlightPlan flight)
        {
            int gateIndex = 0;
            Monitor.Enter(Manager.gates);
            try
            {
                for (int i = 0; i < Manager.gates.Length; i++)
                {
                    if (Manager.gates[i].Flight.FlightNumber == flight.FlightNumber)
                    {
                        gateIndex = i;
                        break;
                    }
                }
            }
            finally
            {
                Monitor.Exit(Manager.gates);
            }
            return gateIndex;
        }

        bool GatesStatus()
        {
            bool status = false;
            Monitor.Enter(Manager.gates);
            try
            {
                for (int i = 0; i < Manager.gates.Length; i++)
                {
                    if (Manager.gates[i].MyStatus == Gate.Status.open)
                    {
                        status = true;
                        break;
                    }
                }
            }
            finally
            {
                Monitor.Exit(Manager.gates);
            }
            return status;
        }

        public void CheckLuggage()
        {
            bool isRunning = false;
            while (myStatus == Status.open)
            {

                //check if gate is open
                Monitor.Enter(Manager.gates);
                try
                {
                    if (Manager.ArrivalBuffer.InternalLength > 0)
                    {
                        FlightPlan flight = Manager.ArrivalBuffer.Inspect();
                        int index = Getgate(flight);
                        Gate newgate = Manager.gates[index];

                        if (GatesStatus())
                        {
                            isRunning = true;
                        }
                        else
                        {
                            myStatus = Status.closed;
                            isRunning = false;
                            Console.WriteLine("closing check in");
                           // Thread.Sleep(3000);
                        }

                    }
                }
                finally
                {
                    Monitor.Enter(Manager.gates);
                }

                //add luggage to gate if open
                if (isRunning)
                {
                    AddToGateBuffer();
                    
                }

            }



            // Console.WriteLine("no more room! closing checkin");
        }


    }
}
