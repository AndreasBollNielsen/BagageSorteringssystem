using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
namespace BagageSorteringssystem
{
    class FlightManager
    {
        public static DateTime CurrentTime = DateTime.Now;
        public static float TimeFactor = 10;

        public void RunTime()
        {
            while (true)
            {
                CurrentTime = CurrentTime.AddMinutes(TimeFactor);
                // Console.WriteLine(CurrentTime.ToString());
                Thread.Sleep(1000);
            }
        }

        public void CheckFlights()
        {
            foreach (FlightPlan flight in Manager.flightPlans)
            {
                DateTime depart = flight.DepartureTime;
              /*  int hour = 0;

                hour = depart.Hour - 2;
                int day = depart.Day;
                if (hour < 0)
                {
                    day -= 1;
                    hour = 23 - hour;
                }
                if(hour >22)
                {
                    day += 1;
                    hour = 0 + hour;
                }*/
                DateTime arrival = new DateTime(depart.Year, depart.Month, depart.Day, depart.Hour, depart.Minute, depart.Second);
                arrival.AddHours(-1);
                //   Console.WriteLine("currentTime: " + CurrentTime.ToString() +" arrival: " + arrival.ToString());
                if (CurrentTime.Hour == arrival.Hour && CurrentTime.Minute == arrival.Minute)
                {
                    // Console.WriteLine(flight.Destination);
                    //open check in if none are open
                    if (CheckClosedCheckIns() > 0)
                    {
                        OpenCheckIn();
                    }


                    OpenGate(flight.IndexNumber);
                    Thread.Sleep(100);
                }
            }

            //open new check in if too many passengers
            if (Manager.ArrivalBuffer.InternalLength == 50 || Manager.ArrivalBuffer.InternalLength == 80)
            {
                OpenCheckIn();
                Thread.Sleep(100);
            }
        }

        //test threaded
        public void CheckFlightsThreaded()
        {
            while (true)
            {
                Monitor.Enter(Manager.flightPlans);
                try
                {
                    foreach (FlightPlan flight in Manager.flightPlans)
                    {
                        DateTime depart = flight.DepartureTime;

                        DateTime arrival = depart;
                        arrival.AddHours(-1);
                      //  Console.WriteLine("currentTime: " + CurrentTime.ToString() + " arrival: " + arrival.ToString());
                       // Thread.Sleep(500);
                        if (CurrentTime.Hour == arrival.Hour && CurrentTime.Minute == arrival.Minute)
                        {
                            //open check in if none are open
                            if (CheckClosedCheckIns() > 0)
                            {
                               OpenCheckIn();

                            }

                          //  Console.WriteLine("opening for flights!");
                            OpenGate(flight.IndexNumber);
                            Thread.Sleep(100);
                        }


                    }

                    //open new check in if too many passengers
                    if (Manager.ArrivalBuffer.InternalLength == 25 || Manager.ArrivalBuffer.InternalLength == 50)
                    {
                        OpenCheckIn();
                        Thread.Sleep(100);
                    }
                }
                finally
                {
                    Monitor.Exit(Manager.flightPlans);
                }
                Thread.Sleep(500);
            }
           
        }

        //open new Gate
        void OpenGate(int flightIndex)
        {
            Random rand = new Random();
            for (int i = 0; i < Manager.gates.Length; i++)
            {
                if (Manager.gates[i].MyStatus == Gate.Status.closed)
                {
                    Manager.gates[i].MyStatus = Gate.Status.open;
                    Manager.gates[i].Flight = Manager.flightPlans[flightIndex];
                    Manager.gates[i].GateIndex = i;
                    Queue gatebuffer = new Queue(100);
                    Manager.GateBuffers[i] = gatebuffer;
                    AddFlightIndex(flightIndex);


                    Thread gateThread = new Thread(Manager.gates[i].ConsumeLuggage);
                    gateThread.Start();
                    Console.WriteLine("new flight arrived");

                    return;
                }
            }
        }

        //open new CheckIn
        void OpenCheckIn()
        {
            for (int i = 0; i < Manager.checkins.Length; i++)
            {
                if (Manager.checkins[i].MyStatus == Check_In.Status.closed)
                {
                    Manager.checkins[i].MyStatus = Check_In.Status.open;
                    Manager.checkins[i].Name = AirlineGenerator();
                    Thread checkinThread = new Thread(Manager.checkins[i].CheckLuggage);
                    checkinThread.Start();

                    Console.WriteLine("open check-in");
                  
                    return;
                }
            }
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

        void AddFlightIndex(int flightIndex)
        {
            Monitor.Enter(Manager.AvailableGates);
            try
            {
                Manager.AvailableGates.Add(flightIndex);
            }
            finally
            {
                Monitor.Exit(Manager.AvailableGates);
            }
        }

        //count how many check ins are available
        int CheckClosedCheckIns()
        {
            int counter = 0;
            Monitor.Enter(Manager.checkins);
            try
            {
                for (int i = 0; i < Manager.checkins.Length; i++)
                {
                    if (Manager.checkins[i].MyStatus == Check_In.Status.closed)
                    {
                        counter++;
                    }
                    else
                    {
                        counter = 0;
                        break;
                    }
                }
            }
            finally
            {
                Monitor.Exit(Manager.checkins);
            }

           

           // Console.WriteLine(counter);
          //  Thread.Sleep(10000);
            return counter;
        }
    }
}
