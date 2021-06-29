using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace BagageSorteringssystem
{
    public class FlightPlan
    {
        //fields
        private DateTime departureTime;
        private DateTime arrivalTime;
        private string status;
        private string flightNumber;
        private Gate departureGate;
        private string destination;
        private int maxLuggage;
        private int indexNumber;

        //properties
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

        //constructor
        public FlightPlan(DateTime departureTime,DateTime arrivaltime, string flightNumber, string destination, int maxLuggage)
        {
            this.departureTime = departureTime;
            this.flightNumber = flightNumber;
            this.destination = destination;
            this.maxLuggage = maxLuggage;
            this.arrivalTime = arrivaltime;
            this.status = "closed";
        }

        // get gate based on flightnumber or destination
        public int GetGate(FlightPlan flight,bool statusCheck)
        {
            int gateIndex = -1;
            for (int i = 0; i < Manager.gates.Length; i++)
            {
                if (Manager.gates[i].Flight.FlightNumber == flight.FlightNumber || Manager.gates[i].Flight.destination == flight.destination)
                {
                     //decision if gate status should be used for condition
                    if(statusCheck)
                    {
                        //return gate index if gate is open
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
