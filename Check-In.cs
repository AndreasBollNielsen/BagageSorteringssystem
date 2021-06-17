using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace BagageSorteringssystem
{
    class Check_In
    {
        private string name;
        private Gate departureGate;

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
        public enum Status { open, closed };
        private Status myStatus;

        public Status MyStatus
        {
            get { return myStatus; }
            set { myStatus = value; }
        }

        public Check_In(string name)
        {
            Name = name;
        }

        void AddToGateBuffer()
        {
            Random rand = new Random();

            Monitor.Enter(Manager.CheckinBuffer);
            try
            {
                if (Manager.CheckinBuffer.InternalLength > 0)
                {
                    
                    Luggage luggage = Manager.CheckinBuffer.Remove();

                    

                    //add luggage to gate buffer
                    luggage.Flight.DepartureGate = Manager.gates[luggage.Flight.IndexNumber];
                    Manager.GateBuffer.Add(luggage);


                }

            }
            finally
            {
                Monitor.Exit(Manager.CheckinBuffer);
            }
            Thread.Sleep(rand.Next(1000, 2500));

        }

        public int Getgate(FlightPlan flight)
        {
            int gateIndex = 0;
            Monitor.Enter(Manager.gates);
            try
            {
                for (int i = 0; i < Manager.gates.Length; i++)
                {
                    if(Manager.gates[i].Flight.FlightNumber == flight.FlightNumber)
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

        public void CheckLuggage()
        {
            bool isRunning = false;
            while (myStatus == Status.open)
            {
             
              //check if gate is open
                Monitor.Enter(Manager.gates);
                try
                {
                    if (Manager.CheckinBuffer.InternalLength > 0)
                    {
                        FlightPlan flight = Manager.CheckinBuffer.Inspect();
                        int index = Getgate(flight);
                        
                        Gate newgate = Manager.gates[index];
                        if (newgate.MyStatus == Gate.Status.open)
                        {
                            // gate = newgate;
                            isRunning = true;
                        }

                        //    Console.WriteLine(isRunning);

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

            Console.WriteLine("no more room! closing checkin");
        }
    }
}
