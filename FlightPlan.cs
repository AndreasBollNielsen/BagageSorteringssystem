using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace BagageSorteringssystem
{
    public class FlightPlan
    {
        private DateTime departureTime;
        private DateTime arrivalTime;
        private string status;

        public string Status
        {
            get { return status; }
            set { status = value; }
        }

        public DateTime ArrivalTime
        {
            get { return arrivalTime; }
            set { arrivalTime = value; }
        }

        private string flightNumber;
        private Gate departureGate;
        private string destination;
        private int maxLuggage;
        private int indexNumber;

        public int IndexNumber
        {
            get { return indexNumber; }
            set { indexNumber = value; }
        }

        public int MaxLuggage
        {
            get { return maxLuggage; }
            set { maxLuggage = value; }
        }

        public string Destination
        {
            get { return destination; }
            set { destination = value; }
        }

        public Gate DepartureGate
        {
            get { return departureGate; }
            set { departureGate = value; }
        }

        public string FlightNumber
        {
            get { return flightNumber; }
            set { flightNumber = value; }
        }

        public DateTime DepartureTime
        {
            get { return departureTime; }
            set { departureTime = value; }
        }

        public FlightPlan(DateTime departureTime,DateTime arrivaltime, string flightNumber, string destination, int maxLuggage)
        {
            this.departureTime = departureTime;
            this.flightNumber = flightNumber;
            this.destination = destination;
            this.maxLuggage = maxLuggage;
            this.arrivalTime = arrivaltime;
            this.status = "closed";
        }

        public int GetGate(FlightPlan flight,bool statusCheck)
        {
            int gateIndex = -1;
            for (int i = 0; i < Manager.gates.Length; i++)
            {
                if (Manager.gates[i].Flight.FlightNumber == flight.FlightNumber || Manager.gates[i].Flight.destination == flight.destination)
                {
                      //   Debug.WriteLine("found id " + i + " flight number " + flight.FlightNumber + " gate status " + Manager.gates[i].MyStatus);
                    if(statusCheck)
                    {
                        if (Manager.gates[i].MyStatus == Gate.Status.open)
                        {
                            gateIndex = i;
                            return gateIndex;
                           
                        }
                    }
                    else
                    {
                        gateIndex = i;
                        return gateIndex;
                    }
                   

                }
              
            }

         
            return gateIndex;
        }
    }
}
