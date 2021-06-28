using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
namespace BagageSorteringssystem
{
    class Manager
    {
        public static Queue ArrivalBuffer = new Queue(100);
        public static Queue CheckInBuffer = new Queue(1000);
        public static Queue ReturnBuffer = new Queue(1000);
        public static Queue[] GateBuffers = new Queue[10];
       // public static List<int> AvailableGates = new List<int>();
        public static Gate[] gates = new Gate[10];
        public static FlightPlan[] flightPlans = new FlightPlan[10];
        public static Check_In[] checkins = new Check_In[10];
        public static int AvailableGates;
        public FlightManager flightMan = new FlightManager();
        public LuggageProducer luggageProducer = new LuggageProducer();
        public Sorter sorter = new Sorter();
        int lastDest;
        public bool Isrunning;
        public bool WPFRunning;
        public EventHandler InitializationHandler;

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

                        
                    }
                }

            }
           

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



            flightMan.IsRunning = true;

            //send empty event 
            InitializationHandler?.Invoke(this, new EventArgs());

            //initialize threads
            Thread luggageThread = new Thread(luggageProducer.AddToBuffer);
            Thread SorterThread = new Thread(sorter.SortLuggage);
            Thread RuntimeThread = new Thread(flightMan.RunTime);
            Thread flightCheckThread = new Thread(flightMan.CheckFlightsThreaded);

            RuntimeThread.Start();
            flightCheckThread.Start();
            luggageThread.Start();
            SorterThread.Start();

        }

        //Run simulation
        public void RunSim()
        {
            Initialize();
            Isrunning = true;

            //initialize time factor
            UpdateLocalTimeFactor();

            if (!WPFRunning)
            {
                ConsoleManager printer = new ConsoleManager();
                while (Isrunning)
                {
                    //execute a print job
                    printer.PrintData();
                    //  Console.Clear();
                    Thread.Sleep(1000);

                }
            }


        }

        //Generates a new flight
        FlightPlan FlightGenerator()
        {
            Random randNum = new Random();



            DateTime depart = DateTime.Now.AddHours(randNum.Next(2, 12));
            DateTime arrival = depart.AddHours(-2);
            string destination = DestinationGenerator();
            string flightNumber = destination[0] + randNum.Next(1000, 10000).ToString();
            int maxLuggage = randNum.Next(10, 100);


            FlightPlan flight = new FlightPlan(depart, arrival, flightNumber, destination, maxLuggage);
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
            while (randDest == lastDest)
            {
                randDest = randNum.Next(destinations.Length);
            }
            string destination = destinations[randDest];
            randDest = lastDest;
            return destination;
        }

        //update local time factor variables
        public void UpdateLocalTimeFactor()
        {
            if (Isrunning)
            {
                //update checkins time factor
                for (int i = 0; i < checkins.Length; i++)
                {
                    checkins[i].TimeFactor = (int)flightMan.TimeFactor;

                }
                /*  Monitor.Enter(checkins);
                  try
                  {
                  }
                  catch (Exception)
                  {
                      Monitor.Exit(checkins);
                  }*/

                //   update gates time factor
                for (int i = 0; i < gates.Length; i++)
                {
                    gates[i].TimeFactor = (int)flightMan.TimeFactor;
                }
                /* Monitor.Enter(gates);
                 try
                 {
                 }
                 finally
                 {
                     Monitor.Exit(gates);
                 }*/

                luggageProducer.TimeFactor = (int)flightMan.TimeFactor;
            }

        }
    }
}
