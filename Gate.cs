using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Diagnostics;
namespace BagageSorteringssystem
{
    public class Gate
    {
        private Queue bagagesBuffer;
        private FlightPlan flight;
        private int indexnumber;
        private int numLuggage;
        private string gatename;
        private int gateIndex;
        public EventHandler luggageHandler;
        public EventHandler GateStatusHandler;
        public int GateIndex
        {
            get { return gateIndex; }
            set { gateIndex = value; }
        }

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

        public Gate()
        {
            this.flight = new FlightPlan(DateTime.Now, DateTime.Now, "xxx", "xxx", 10);
            bagagesBuffer = new Queue(flight.MaxLuggage);
        }



      /*  public void AddToBuffer(Luggage luggage)
        {
            Random rand = new Random();
            while (MyStatus == Status.open)
            {
                Monitor.Enter(bagagesBuffer);
                try
                {
                    if (numLuggage < flight.MaxLuggage)
                    {
                        bagagesBuffer.Add(luggage);
                        Console.WriteLine("adding to buffer");
                        
                    }

                }
                finally
                {
                    Monitor.Exit(bagagesBuffer);
                }
                Thread.Sleep(rand.Next(500, 2000));
            }

            // Console.WriteLine("this flight has departed");
        }*/

        //check flight departure
        void CheckDeparture()
        {
            // Console.WriteLine($"current: {FlightManager.CurrentTime} departure: {flight.DepartureTime}");
            if (FlightManager.CurrentTime.Hour >= flight.DepartureTime.Hour && FlightManager.CurrentTime.Minute >= flight.DepartureTime.Minute)
            {

                myStatus = Status.closed;
              //  ResetGate();

            }
        }
        public void ConsumeLuggage()
        {
            Random rand = new Random();
            while (MyStatus == Status.open)
            {

                Queue buffer = Manager.GateBuffers[GateIndex];
                Monitor.Enter(buffer);
                try
                {
                    if (buffer.InternalLength > 0)
                    {
                        Luggage luggage = buffer.Remove();
                        if (numLuggage < flight.MaxLuggage)
                        {
                            NumLuggage++;

                            luggageHandler?.Invoke(this, new GateEventArgs(NumLuggage, flight.MaxLuggage, MyStatus, GateIndex));
                        }
                        //close gate if flight is ready for takeoff
                        CheckDeparture();


                    }

                    if (numLuggage >= flight.MaxLuggage)
                    {
                        myStatus = Status.closed;
                        ResetGate();
                    }
                   Thread.Sleep(rand.Next(500, 2000));

                }
                finally
                {
                    Monitor.Exit(buffer);
                }
            }

            //remove index in array
            RemoveGate();
        }

        //reset gate to default 
        void ResetGate()
        {
            numLuggage = 0;
            flight = new FlightPlan(DateTime.Now, DateTime.Now, "xxx", "xxx", 10);
            GateStatusHandler?.Invoke(this, new GateEventArgs(0, 0, MyStatus, gateIndex));
        }

        //remove available checkins
        void RemoveGate()
        {
            Monitor.Enter(Manager.AvailableGates);
            try
            {
                for (int i = 0; i < Manager.gates.Length; i++)
                {
                    if (Manager.gates[i].myStatus == Status.closed)
                    {
                        for (int j = 0; j < Manager.AvailableGates.Count; j++)
                        {
                            if (Manager.AvailableGates[j] == i)
                            {
                                Manager.AvailableGates.RemoveAt(j);
                                break;
                            }
                        }
                    }
                }
            }
            finally
            {
                Monitor.Exit(Manager.AvailableGates);
            }
        }
    }
}
