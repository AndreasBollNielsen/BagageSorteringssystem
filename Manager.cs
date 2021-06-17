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
        public static volatile Queue ReturnBuffer = new Queue(100);
        public static Gate[] gates = new Gate[10];
        public static FlightPlan[] flightPlans = new FlightPlan[10];
        static Check_In[] checkins = new Check_In[10];

        public static bool Isrunning;
        int CheckinIndex = 0;
        int GateIndex = 0;

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

            luggageThread.Start();
            SorterThread.Start();



        }

        //Run simulation
        public void RunSim()
        {
            Initialize();
            //open gates
            OpenGate();
            OpenGate();
            OpenGate();
            OpenGate();

            //open check in
            OpenCheckIn();
            OpenCheckIn();
            OpenCheckIn();

            Isrunning = true;
            ConsoleManager printer = new ConsoleManager();


            while (Isrunning)
            {
                //collect data from threads for debugging
                ConsoleData[] dataStream = new ConsoleData[GateIndex];
                for (int n = 0; n < GateIndex; n++)
                {
                    ConsoleData data = new ConsoleData();

                    //check-In buffer
                    Monitor.Enter(CheckinBuffer);
                    try
                    {
                        if (CheckinBuffer.InternalLength > 0)
                        {
                            data.checkinBuf = new string[] { " ", " ", " ", " ", " ", " ", " ", " ", " ", " " };
                            for (int i = 0; i < CheckinBuffer.InternalLength; i++)
                            {
                                if (i < data.checkinBuf.Length)
                                {
                                    data.checkinBuf[i] = CheckinBuffer.Buffer[i].LuggageId.ToString();

                                }

                            }

                        }
                    }
                    finally
                    {
                        Monitor.Exit(CheckinBuffer);

                    }

                    //from check-In counter
                    Monitor.Enter(checkins[n]);
                    try
                    {
                        data.checkInName = checkins[n].Name;
                        data.checkInStatus = checkins[n].MyStatus.ToString();
                    }
                    finally
                    {
                        Monitor.Exit(checkins[n]);
                    }

                    //from check-In to gate buffer
                    Monitor.Enter(GateBuffer);
                    try
                    {
                        data.GateBuf = new string[] { " ", " ", " ", " ", " ", " ", " ", " ", " ", " " };
                        for (int i = 0; i < GateBuffer.InternalLength; i++)
                        {
                            if (i < data.GateBuf.Length)
                            {
                                data.GateBuf[i] = GateBuffer.Buffer[i].LuggageId.ToString();

                            }
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
                        data.GateName = gates[n].GateName;
                        data.LuggageCounter = gates[n].NumLuggage.ToString() + "/" + gates[n].Flight.MaxLuggage;
                        data.GateStatus = gates[n].MyStatus.ToString();
                        data.ReturnBuf = new string[] { " ", " ", " ", " ", " ", " ", " ", " ", " ", " " };

                        
                    }
                    finally
                    {

                        Monitor.Exit(GateBuffer);
                    }

                    //from sorter buffer to return buffer
                    if (ReturnBuffer.InternalLength > 0)
                    {
                        for (int i = 0; i < ReturnBuffer.InternalLength; i++)
                        {
                            if (i < data.ReturnBuf.Length)
                            {
                                data.ReturnBuf[i] = ReturnBuffer.Buffer[i].LuggageId.ToString();

                            }

                        }

                    }

                    dataStream[n] = data;
                }

                //execute a print job
                printer.PrintData(dataStream);
                Thread.Sleep(500);
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
            int maxLuggage = randNum.Next(2, 10);


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

        //Generate a new Airline name
        string AirlineGenerator()
        {
            Random randNum = new Random();
            string[] airLines = new string[] { "SAS", "Ryanair", "LuftHansa", "EasyJet", "Virgin Atlanta", "Luxair" };
            int randAirline = randNum.Next(airLines.Length);

            string airLineName = airLines[randAirline];
            return airLineName;
        }

        //open a new checkin
        public void OpenGate()
        {
            Random rand = new Random();
            for (int i = 0; i < gates.Length; i++)
            {
                if (gates[i].MyStatus == Gate.Status.closed)
                {
                    gates[i].MyStatus = Gate.Status.open;
                    int randomFlight = rand.Next(flightPlans.Length);
                    gates[i].Flight = flightPlans[randomFlight];
                    Console.WriteLine("new flight arrived");
                    Console.WriteLine(gates[i].Flight.FlightNumber + " from flightplans " + flightPlans[randomFlight].FlightNumber);
                    GateIndex++;
                    Thread.Sleep(1000);
                    return;
                }
            }
        }

        //open new CheckIn
        public void OpenCheckIn()
        {
            for (int i = 0; i < checkins.Length; i++)
            {
                if (checkins[i].MyStatus == Check_In.Status.closed)
                {
                    checkins[i].MyStatus = Check_In.Status.open;
                    checkins[i].Name = AirlineGenerator();
                    Thread checkinThread = new Thread(checkins[i].CheckLuggage);
                    checkinThread.Start();
                    Console.WriteLine("open check-in");
                    Thread.Sleep(1000);
                    return;
                }
            }
        }
    }
}
