using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Diagnostics;
namespace BagageSorteringssystem
{
    public class Gate
    {
       //attributes
        private FlightPlan flight;
        private int indexnumber;
        private int numLuggage;
        private string gatename;
        private int gateIndex;
        private int timeFactor;
        private Status myStatus;

        //eventhandlers
        public EventHandler luggageHandler;
        public EventHandler GateStatusHandler;
        public EventHandler FlightStatusHandler;
      
        //properties
        public int TimeFactor
        {
            get { return timeFactor; }
            set { timeFactor = value; }
        }
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

        //constructor
        public Gate()
        {
            this.flight = new FlightPlan(DateTime.Now, DateTime.Now, "xxx", "xxx", 10);
            this.timeFactor = 1;
        }

        //get current date time
        DateTime GetCurrentTime()
        {
            object timeLock = new object();
            Monitor.Enter(timeLock);
            DateTime currentTime;
            try
            {
                currentTime = FlightManager.CurrentTime;

            }
            finally
            {

                Monitor.Exit(timeLock);
            }

            return currentTime;
        }
        

        //check flight departure
        void CheckDeparture()
        {
            DateTime curTime = GetCurrentTime();
            
            if (curTime.Day == flight.DepartureTime.Day)
            {
                if (curTime.Hour >= flight.DepartureTime.Hour && curTime.Minute >= flight.DepartureTime.Minute)
                {
                    Debug.WriteLine($"current time: {curTime.Hour}:{curTime.Minute} departure time: {flight.DepartureTime.Hour}:{flight.DepartureTime.Minute}");
                    myStatus = Status.closed;
                    ResetGate();
                    Debug.WriteLine("closing gate");
                    luggageHandler?.Invoke(this, new GateEventArgs(NumLuggage, flight.MaxLuggage, MyStatus, GateIndex));
                }
            }
            // Console.WriteLine($"current: {FlightManager.CurrentTime} departure: {flight.DepartureTime}");
            //   Debug.WriteLine("current time: " + FlightManager.CurrentTime + " departure time: " + flight.DepartureTime + " arrival time: " + flight.ArrivalTime);

              Thread.Sleep(500);
        }

        //remove luggage from buffer and add count
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
                }
                finally
                {
                    Monitor.Exit(buffer);
                }

                int delay = rand.Next(250, 800);
                Thread.Sleep(delay / timeFactor);
            }


            //remove index in array
            Manager.AvailableGates -= 1;
            Thread.Sleep(1);
        }

        //reset gate to default 
        void ResetGate()
        {
            numLuggage = 0;
            GateStatusHandler?.Invoke(this, new GateEventArgs(0, 0, MyStatus, gateIndex));
            FlightStatusHandler?.Invoke(flight, new FlightPlanEventArgs(flight.IndexNumber, " ", "Departed"));
            flight = new FlightPlan(DateTime.Now, DateTime.Now, "xxx", "xxx", 10);
        }

    }
}
