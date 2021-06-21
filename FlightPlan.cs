using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace BagageSorteringssystem
{
    public class FlightPlan
    {
        private DateTime departureTime;
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

        public FlightPlan(DateTime departureTime, string flightNumber, string destination, int maxLuggage)
        {
            this.departureTime = departureTime;
            this.flightNumber = flightNumber;
            this.destination = destination;
            this.maxLuggage = maxLuggage;
        }

        public int GetGate(FlightPlan flight)
        {
            int gateIndex = -1;
            for (int i = 0; i < Manager.gates.Length; i++)
            {
                if (Manager.gates[i].Flight.FlightNumber == flight.FlightNumber /*|| Manager.gates[i].Flight.destination == flight.destination*/)
                {
                    if(Manager.gates[i].MyStatus == Gate.Status.open)
                    {
                        gateIndex = i;
                        // Console.WriteLine("found id " + i + " flight number " + flight.FlightNumber);
                        return gateIndex;
                        //break;
                    }

                }
              
            }

         
            return gateIndex;
        }
    }
}
