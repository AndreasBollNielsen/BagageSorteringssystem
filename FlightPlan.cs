using System;
using System.Collections.Generic;
using System.Text;

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

        public Gate GetGate()
        {
            return departureGate;
        }
    }
}
