using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace BagageSorteringssystem
{
    public class Gate
    {
        private Queue bagagesBuffer;
        private FlightPlan flight;
        private int indexnumber;
        private int numLuggage;
        private string gatename;

        public string GateName
        {
            get { return gatename; }
            set { gatename = value; }
        }

        public int NumLuggage
        {
            get { return numLuggage; }
            set { numLuggage = value; }
        }

        public int IndexNumber
        {
            get { return indexnumber; }
            set { indexnumber = value; }
        }

        public enum Status { open, closed };
        private Status myStatus;

        public Status MyStatus
        {
            get { return myStatus; }
            set { myStatus = value; }
        }

        public FlightPlan Flight
        {
            get { return flight; }
            set { flight = value; }
        }

        public Queue BagagesBuffer
        {
            get { return bagagesBuffer; }
            set { bagagesBuffer = value; }
        }

        public Gate(FlightPlan flight)
        {
            this.flight = flight;
            bagagesBuffer = new Queue(flight.MaxLuggage);
        }

        public void AddToBuffer()
        {
            Random rand = new Random();
            while (MyStatus == Status.open)
            {
                Monitor.Enter(Manager.GateBuffer);
                try
                {
                    if (Manager.GateBuffer.InternalLength > 0)
                    {
                        if (bagagesBuffer.InternalLength <= flight.MaxLuggage)
                        {
                            Luggage luggage = Manager.GateBuffer.Remove();
                            bagagesBuffer.Add(luggage);
                        }

                    }

                    

                }
                finally
                {
                    Monitor.Exit(Manager.GateBuffer);
                }

                Thread.Sleep(rand.Next(1500, 6000));
            }

            Console.WriteLine("this flight has departed");
        }

        public void AddLuggage(Luggage luggage)
        {
            if (numLuggage < flight.MaxLuggage)
            {
                NumLuggage++;
            }
            else
            {
                myStatus = Status.closed;
            }
           
        }

    }
}
