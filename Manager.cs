using System;
using System.Collections.Generic;
using System.Text;

namespace BagageSorteringssystem
{
    class Manager
    {
        static Luggage[] CheckinBuffer = new Luggage[100];
        static Luggage[] GateBuffer = new Luggage[50];
        static Gate[] gates = new Gate[1];
        static FlightPlan[] flightPlans = new FlightPlan[1];

        void Initialize()
        {
            FlightPlan flight = FlightGenerator();
            flightPlans[0] = flight;
        }


        void RunSim ()
        {

        }
       
        //Generates a new flight
        FlightPlan FlightGenerator()
        {
            Random randNum = new Random();
            string[] destinations = new string[] { "London", "copenhagen", "Amsterdam" };
            int randDest = randNum.Next(destinations.Length);
            DateTime depart = DateTime.Today.AddHours(randNum.Next(24));
            string destination = destinations[randDest];
            string flightNumber = destination[0] + randNum.Next(1000, 10000).ToString();
            int maxLuggage = randNum.Next(10, 100);

           
            FlightPlan flight = new FlightPlan(depart, flightNumber, destination, maxLuggage);
            return flight;

               
        }
    }
}
