using System;
using System.Collections.Generic;
using System.Text;

namespace BagageSorteringssystem
{
    class Gate
    {
        private Luggage[] bagagesBuffer;
        private FlightPlan flight;

        public FlightPlan Flight
        {
            get { return flight; }
            set { flight = value; }
        }

        public Luggage[] BagagesBuffer
        {
            get { return bagagesBuffer; }
            set { bagagesBuffer = value; }
        }

        public Gate()
        {
            bagagesBuffer = new Luggage[50];
        }
    }
}
