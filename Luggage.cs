using System;
using System.Collections.Generic;
using System.Text;

namespace BagageSorteringssystem
{
    public class Luggage
    {
        //fields
        private FlightPlan flight;
        private int luggageId;

        //properties
        public int LuggageId
        {
            get { return luggageId; }
            set { luggageId = value; }
        }
        public FlightPlan Flight
        {
            get { return flight; }
            set { flight = value; }
        }

        //constructor
        public Luggage(FlightPlan flight,int _id)
        {
            this.flight = flight;
            this.luggageId = _id;
        }
    }
}
