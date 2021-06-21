﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
namespace BagageSorteringssystem
{
    class Manager
    {
        public static volatile Queue ArrivalBuffer = new Queue(100);
        public static volatile Queue CheckInBuffer = new Queue(100);
        public static volatile Queue ReturnBuffer = new Queue(100);
        public static volatile Queue[] GateBuffers = new Queue[10];
        public static List<int> AvailableGates = new List<int>();
        public static Gate[] gates = new Gate[10];
        public static FlightPlan[] flightPlans = new FlightPlan[10];
        public static Check_In[] checkins = new Check_In[10];
        FlightManager flightMan = new FlightManager();
        int LastDest;
        public static bool Isrunning;
        // int CheckinIndex = 0;
        //  public static int GateIndex = 0;

        //instantiate flightplan, gate & check_in
        public void Initialize()
        {
            //instantiate flightplans
            for (int i = 0; i < flightPlans.Length; i++)
            {
                FlightPlan flight = FlightGenerator();
                flight.IndexNumber = i;
                flightPlans[i] = flight;

                //check similar destination
                if (i > 0)
                {
                    if (flightPlans[i - 1].Destination == flight.Destination)
                    {
                        while (flightPlans[i - 1].Destination == flight.Destination)
                        {
                            flight.Destination = DestinationGenerator();
                        }
                            Console.WriteLine(flight.Destination);
                            Thread.Sleep(1000);
                    }
                }

                //  Console.WriteLine($"destination {flight.Destination} departure {flight.DepartureTime}");
            }
            Thread.Sleep(500);
            //instantiate gates
            for (int j = 0; j < gates.Length; j++)
            {
                Gate gate = new Gate();
                gate.IndexNumber = j;
                gate.GateName = GateGenerator();
                gate.MyStatus = Gate.Status.closed;
                gates[j] = gate;
            }

            //instantiate check-ins
            for (int n = 0; n < checkins.Length; n++)
            {
                Check_In check_In = new Check_In("SAS");
                check_In.MyStatus = Check_In.Status.closed;
                checkins[n] = check_In;
            }

            LuggageProducer luggageProducer = new LuggageProducer();
            Sorter sorter = new Sorter();

            //initialize threads
            Thread luggageThread = new Thread(luggageProducer.AddToBuffer);
            Thread SorterThread = new Thread(sorter.SortLuggage);
            Thread flightmanThread = new Thread(flightMan.RunTime);
            Thread flightCheckThread = new Thread(flightMan.CheckFlightsThreaded);
            flightmanThread.Start();
            flightCheckThread.Start();
            luggageThread.Start();
            SorterThread.Start();


        }

        //Run simulation
        public void RunSim()
        {
            Initialize();
            Isrunning = true;
            ConsoleManager printer = new ConsoleManager();



            while (Isrunning)
            {
                //  flightMan.CheckFlights();

                //execute a print job
                printer.PrintData();
                Thread.Sleep(1000);
                  Console.Clear();
            }
        }

        //Generates a new flight
        FlightPlan FlightGenerator()
        {
            Random randNum = new Random();
            //  string[] destinations = new string[] { "London", "copenhagen", "Amsterdam", "Bruxelles", "Florida", "Helsinki" };
            //   int randDest = randNum.Next(destinations.Length);
            DateTime depart = DateTime.Now.AddHours(randNum.Next(2, 12));
            string destination = DestinationGenerator();
            string flightNumber = destination[0] + randNum.Next(1000, 10000).ToString();
            int maxLuggage = randNum.Next(10, 100);


            FlightPlan flight = new FlightPlan(depart, flightNumber, destination, maxLuggage);
            return flight;


        }

        //Generate a new gate name
        string GateGenerator()
        {
            Random randNum = new Random();
            string[] gates = new string[] { "A", "B", "C", "D", "E", "F" };
            int randGate = randNum.Next(gates.Length);

            string gateLetter = gates[randGate];
            string gateName = gateLetter + randNum.Next(0, 10 + 1).ToString();
            return gateName;
        }

        //Generate a new destination
        string DestinationGenerator()
        {
            Random randNum = new Random();
            string[] destinations = new string[] { "London", "copenhagen", "Amsterdam", "Bruxelles", "Florida", "Helsinki" };
            int randDest = randNum.Next(destinations.Length);

            //select a new number if same as last time
            while (randDest == LastDest)
            {
                randDest = randNum.Next(destinations.Length);
            }
            string destination = destinations[randDest];
            randDest = LastDest;
            return destination;
        }

    }
}
