using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Diagnostics;
namespace BagageSorteringssystem
{
    class Check_In
    {
        //fields
        public enum Status { open, closed };
        private string name;
        private Gate departureGate;
        private Status myStatus;
        private int timeFactor;

        //properties
        public int TimeFactor
        {
            get { return timeFactor; }
            set { timeFactor = value; }
        }
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


        //constructor
        public Check_In(string name)
        {
            Name = name;
            this.timeFactor = 1;
        }

        // add luggage to gate buffer
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

                    Monitor.Enter(Manager.CheckInBuffer);
                    try
                    {
                        if (Manager.CheckInBuffer.InternalLength < Manager.CheckInBuffer.Length)
                        {
                            Manager.CheckInBuffer.Add(luggage);
                            //Debug.WriteLine("adding luggage");
                        }
                    }
                    finally
                    {
                        Monitor.Exit(Manager.CheckInBuffer);
                    }


                    // Debug.WriteLine("adding to checkin buffer from check in");
                }

            }
            finally
            {
                Monitor.Exit(Manager.ArrivalBuffer);
            }


            int delay = rand.Next(500, 1200);
         //   Debug.WriteLine("check in: " + delay / timeFactor);
            Thread.Sleep(delay / timeFactor);

        }

        //get gate based on flightplan
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

        //check if all gates are closed
        bool GatesStatus()
        {
            bool status = true;
            Monitor.Enter(Manager.gates);
            try
            {
                for (int i = 0; i < Manager.gates.Length; i++)
                {
                    if (Manager.gates[i].MyStatus == Gate.Status.closed)
                    {
                        status = false;
                    }
                    else
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
            return true;
        }

        //inspect luggage 
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
                            //   Debug.WriteLine("closing check in");
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

            Debug.WriteLine("no more room! closing checkin");
        }

    }
}
