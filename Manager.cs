using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
namespace BagageSorteringssystem
{
    class Manager
    {
        public static volatile Queue CheckinBuffer = new Queue(100);
        public static volatile Queue GateBuffer = new Queue(100);
        public static Gate[] gates = new Gate[1];
        public static FlightPlan[] flightPlans = new FlightPlan[1];
        static Check_In[] checkins = new Check_In[1];

        //instantiate flightplan, gate & check_in
        public void Initialize()
        {
            //instantiate flightplans
            for (int i = 0; i < flightPlans.Length; i++)
            {
                FlightPlan flight = FlightGenerator();
                flightPlans[i] = flight;



            }

            //instantiate gates
            for (int j = 0; j < gates.Length; j++)
            {
                Gate gate = new Gate(flightPlans[j]);
                gate.IndexNumber = j;
                gates[j] = gate;
            }

            //instantiate check-ins
            for (int n = 0; n < checkins.Length; n++)
            {
                Check_In check_In = new Check_In("SAS");
                check_In.DepartureGate = gates[n];
                check_In.MyStatus = Check_In.Status.open;
                checkins[n] = check_In;
            }

            LuggageProducer luggageProducer = new LuggageProducer();
            Sorter sorter = new Sorter();
            //initialize threads
            for (int i = 0; i < checkins.Length; i++)
            {
                Thread checkinThread = new Thread(checkins[i].CheckLuggage);
                checkinThread.Start();

            }

            Thread luggageThread = new Thread(luggageProducer.AddToBuffer);
            Thread SorterThread = new Thread(sorter.SortLuggage);

            luggageThread.Start();
            SorterThread.Start();



        }


        public void RunSim()
        {
            Initialize();

            while (true)
            {
                //checkin buffer
                Monitor.Enter(CheckinBuffer);
                try
                {
                    Console.WriteLine("checkin buffer-------Gate_buffer-----------Flight buffer");
                    for (int i = 0; i < CheckinBuffer.InternalLength; i++)
                    {
                        string output = CheckinBuffer.Buffer[i].LuggageId.ToString();
                        Console.Write(output.PadLeft(0, ' ') + "\n");

                    }
                }
                finally
                {

                    Monitor.Exit(CheckinBuffer);
                }


                //from checkin to gate buffer
                Monitor.Enter(GateBuffer);
                try
                {

                    for (int i = 0; i < GateBuffer.InternalLength; i++)
                    {
                        string output = GateBuffer.Buffer[i].LuggageId.ToString() + " status: " + checkins[0].MyStatus;
                        Console.Write(output.PadLeft(25, ' ') + "\n");

                    }
                }
                finally
                {

                    Monitor.Exit(GateBuffer);
                }

                //from gate buffer to flight
                Monitor.Enter(GateBuffer);

                try
                {
                    string output = gates[0].NumLuggage.ToString() + "/" + gates[0].Flight.MaxLuggage + "status " + gates[0].MyStatus;
                    Console.Write(output.PadLeft(50, ' ') + "\n");
                }
                finally
                {

                    Monitor.Exit(GateBuffer);
                }

                Thread.Sleep(1000);
                // Console.Clear();
            }
        }

        //Generates a new flight
        FlightPlan FlightGenerator()
        {
            Random randNum = new Random();
            string[] destinations = new string[] { "London", "copenhagen", "Amsterdam", "Bruxelles", "Florida", "Helsinki" };
            int randDest = randNum.Next(destinations.Length);
            DateTime depart = DateTime.Today.AddHours(randNum.Next(24));
            string destination = destinations[randDest];
            string flightNumber = destination[0] + randNum.Next(1000, 10000).ToString();
            int maxLuggage = randNum.Next(5, 10);


            FlightPlan flight = new FlightPlan(depart, flightNumber, destination, maxLuggage);
            return flight;


        }
    }
}
