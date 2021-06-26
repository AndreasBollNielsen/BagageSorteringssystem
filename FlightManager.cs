using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Diagnostics;
namespace BagageSorteringssystem
{
    class FlightManager
    {
        public static DateTime CurrentTime = DateTime.Now;
        public static float TimeFactor = 10;
        public EventHandler TimeHandler;
        public EventHandler[] Checkinhandler = new EventHandler[10];
        public EventHandler[] Gatehandler = new EventHandler[10];
        public EventHandler[] FlightHandler = new EventHandler[10];
        public bool IsRunning;
        public void RunTime()
        {
            while (IsRunning)
            {
                // CurrentTime = CurrentTime.AddMinutes(TimeFactor);
                CurrentTime = CurrentTime.AddMinutes(1);
                // Console.WriteLine(CurrentTime.ToString());
                TimeHandler?.Invoke(this, new TimeChangedEvent(CurrentTime));


                float delay = 1000 / TimeFactor;
                Thread.Sleep((int)delay);
            }
        }

        //test threaded
        public void CheckFlightsThreaded()
        {
            while (IsRunning)
            {
                Monitor.Enter(Manager.flightPlans);
                try
                {
                    foreach (FlightPlan flight in Manager.flightPlans)
                    {
                        DateTime arrival = flight.ArrivalTime;
                        //  Console.WriteLine("currentTime: " + CurrentTime.ToString() + " arrival: " + arrival.ToString());
                        // Thread.Sleep(1000);
                        if (CurrentTime.Day == arrival.Day)
                        {
                            if (CurrentTime.Hour >= arrival.Hour && CurrentTime.Minute >= arrival.Minute && flight.DepartureGate == null)
                            {
                                //open check in if none are open
                                if (CheckClosedCheckIns() > 0)
                                {
                                    OpenCheckIn();

                                }

                                Debug.WriteLine("current day: " + CurrentTime.Day + " arrival day: " + arrival.Day);
                                Debug.WriteLine("currentTime: " + CurrentTime.ToString() + " arrival: " + arrival.ToString());
                                Debug.WriteLine("opening for flights!");
                                OpenGate(flight.IndexNumber);

                                //add flight eventhandler
                                FlightHandler[flight.IndexNumber]?.Invoke(flight, new FlightPlanEventArgs(flight.IndexNumber));

                            }

                        }
                        //  Thread.Sleep(1000);



                    }

                    //open new check in if too many passengers
                    if (Manager.ArrivalBuffer.InternalLength == 10 || Manager.ArrivalBuffer.InternalLength == 50)
                    {
                      /*  Debug.WriteLine("opening new check in");
                        OpenCheckIn();
                        Thread.Sleep(1000);*/
                    }
                }
                finally
                {
                    Monitor.Exit(Manager.flightPlans);
                }

                // Thread.Sleep(2000);
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
                    Manager.flightPlans[flightIndex].DepartureGate = Manager.gates[i];
                    AddFlightIndex(flightIndex);
                    Debug.WriteLine("opening new Gate");

                    //create event
                    Gatehandler[i]?.Invoke(this, new GateEventArgs(Manager.gates[i].NumLuggage, Manager.gates[i].Flight.MaxLuggage, Manager.gates[i].MyStatus, i));

                    Thread gateThread = new Thread(Manager.gates[i].ConsumeLuggage);
                    gateThread.Start();
                    //   Console.WriteLine("new flight arrived");
                    return;
                }
            }
        }

        //open new CheckIn
        public void OpenCheckIn()
        {
            for (int i = 0; i < Manager.checkins.Length; i++)
            {
                if (Manager.checkins[i].MyStatus == Check_In.Status.closed)
                {
                    Manager.checkins[i].MyStatus = Check_In.Status.open;
                    Manager.checkins[i].Name = AirlineGenerator();
                    Thread checkinThread = new Thread(Manager.checkins[i].CheckLuggage);

                    //invoke event
                    Checkinhandler[i]?.Invoke(Manager.checkins[i], new CheckInChangedEventArgs(Manager.checkins[i].Name, Manager.checkins[i].MyStatus, i));

                    checkinThread.Start();
                    Debug.WriteLine("open check-in");
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
                Debug.WriteLine("adding available");
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
