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
        public static Gate[] gates = new Gate[2];
        public static FlightPlan[] flightPlans = new FlightPlan[2];
        static Check_In[] checkins = new Check_In[1];

        //instantiate flightplan, gate & check_in
        public void Initialize()
        {
            //instantiate flightplans
            for (int i = 0; i < flightPlans.Length; i++)
            {
                FlightPlan flight = FlightGenerator();
                flight.IndexNumber = i;
                flightPlans[i] = flight;



            }

            //instantiate gates
            for (int j = 0; j < gates.Length; j++)
            {
                Gate gate = new Gate(flightPlans[j]);
                gate.IndexNumber = j;
                gate.GateName = GateGenerator();
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

        //Run simulation
        public void RunSim()
        {
            Initialize();
            ConsoleManager printer = new ConsoleManager();
           

            while (true)
            {
                string[] checkingBufferOutput = new string[] { " ", " ", " ", " ", " ", " ", " ", " ", " ", " " };
                string[] gateBufferBufferOutput = new string[] { " ", " ", " ", " ", " ", " ", " ", " ", " ", " " };
                string[] Gate1 = new string[] { " ", " ", " " };
                string[] Gate2 = new string[] { " ", " ", " " };

                //checkin buffer
                /*  Monitor.Enter(CheckinBuffer);
                  try
                  {
                      if (CheckinBuffer.InternalLength > 0)
                      {
                          for (int i = 0; i < CheckinBuffer.InternalLength; i++)
                          {
                              if (i < checkingBufferOutput.Length)
                              {
                                  checkingBufferOutput[i] = CheckinBuffer.Buffer[i].LuggageId.ToString();

                              }

                          }
                      }

                  }
                  finally
                  {
                      Monitor.Exit(CheckinBuffer);

                  }*/


                //from checkin to gate buffer

                Monitor.Enter(GateBuffer);
                try
                {

                    for (int i = 0; i < GateBuffer.InternalLength; i++)
                    {
                        gateBufferBufferOutput[i] = GateBuffer.Buffer[i].LuggageId.ToString();
                       
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

                    Gate1[0] = gates[0].GateName;
                    Gate1[1] = gates[0].NumLuggage.ToString() + "/" + gates[0].Flight.MaxLuggage;
                    Gate1[2] = gates[0].MyStatus.ToString();
                    Gate2[0] = gates[1].GateName;
                    Gate2[1] = gates[1].NumLuggage.ToString() + "/" + gates[1].Flight.MaxLuggage;
                    Gate2[2] = gates[1].MyStatus.ToString();

                    Console.WriteLine(gates[0].NumLuggage);

                }
                finally
                {

                    Monitor.Exit(GateBuffer);
                }


                printer.PrintData(checkingBufferOutput, gateBufferBufferOutput, Gate1, Gate2);
                Thread.Sleep(1000);
                Console.Clear();



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

        //Generate a new gate name
        string GateGenerator()
        {
            Random randNum = new Random();
            string[] gates = new string[] { "A", "B", "C", "D", "E", "F" };
            int randDest = randNum.Next(gates.Length);

            string gateLetter = gates[randDest];
            string gateName = gateLetter + randNum.Next(0, 10 + 1).ToString();
            return gateName;
        }
    }
}
