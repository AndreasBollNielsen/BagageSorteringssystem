using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Diagnostics;
namespace BagageSorteringssystem
{
    class FlightManager
    {
        //properties & eventhandlers
        public static DateTime CurrentTime = DateTime.Now;
        public float TimeFactor = 1;
        public EventHandler TimeHandler;
        public EventHandler[] Checkinhandler = new EventHandler[10];
        public EventHandler[] Gatehandler = new EventHandler[10];
        public EventHandler[] FlightHandler = new EventHandler[10];
        public bool IsRunning;
        object timeLock = new object();
     
        //increase current time
        public void RunTime()
        {
            while (IsRunning)
            {
                //increase time by time factor
                Monitor.Enter(timeLock);
                try
                {
                    CurrentTime = CurrentTime.AddMinutes(1);
                }
                finally
                {
                    Monitor.Exit(timeLock);
                }

               //add eventhandler
                TimeHandler?.Invoke(this, new TimeChangedEvent(CurrentTime));

                float delay = 1000 / TimeFactor;
                Thread.Sleep((int)delay);
            }
        }

        //check flights
        public void CheckFlightsThreaded()
        {
            while (IsRunning)
            {

                Monitor.Enter(Manager.flightPlans);
                try
                {
                    //loop through flightplans
                    foreach (FlightPlan flight in Manager.flightPlans)
                    {
                        //get arrival time for flight
                        DateTime arrival = flight.ArrivalTime;

                        //open gate if new flight has arrived
                        if (CurrentTime.Day == arrival.Day)
                        {
                            if (CurrentTime.Hour >= arrival.Hour && CurrentTime.Minute >= arrival.Minute && flight.Status == "closed")
                            {
                                //open check in if none are open
                                if (CheckClosedCheckIns() > 0)
                                {
                                    OpenCheckIn();

                                }

                                //open new gate
                                OpenGate(flight.IndexNumber);

                            }

                        }

                    }

                    //open new check in if too many passengers
                    if (Manager.ArrivalBuffer.InternalLength >= 50 || Manager.ArrivalBuffer.InternalLength == 75)
                    {
                        //  Debug.WriteLine("opening new check in");
                        OpenCheckIn();
                        Thread.Sleep(1000);
                    }
                }
                finally
                {
                    Monitor.Exit(Manager.flightPlans);
                }


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
                    //set new gate info
                    Monitor.Enter(Manager.gates[i]);
                    try
                    {
                        Manager.gates[i].MyStatus = Gate.Status.open;
                        Manager.gates[i].Flight = Manager.flightPlans[flightIndex];
                        Manager.gates[i].GateIndex = i;
                        Queue gatebuffer = new Queue(100);
                        Manager.GateBuffers[i] = gatebuffer;
                    }
                    finally
                    {
                        Monitor.Exit(Manager.gates[i]);
                    }


                    //set flightplan status
                    Monitor.Enter(Manager.flightPlans[flightIndex]);
                    try
                    {
                        Manager.flightPlans[flightIndex].DepartureGate = Manager.gates[i];
                        Manager.flightPlans[flightIndex].Status = "arrived";
                    }
                    finally
                    {
                        Monitor.Exit(Manager.flightPlans[flightIndex]);
                    }


                    //add to counter
                    Manager.AvailableGates++;

                    //create event
                    Gatehandler[i]?.Invoke(this, new GateEventArgs(Manager.gates[i].NumLuggage, Manager.gates[i].Flight.MaxLuggage, Manager.gates[i].MyStatus, i));
                    FlightHandler[flightIndex]?.Invoke(Manager.flightPlans[flightIndex], new FlightPlanEventArgs(flightIndex, Manager.gates[i].GateName, "Boarding"));


                    //create thread
                    Thread gateThread = new Thread(Manager.gates[i].ConsumeLuggage);
                    gateThread.Start();


                    Thread.Sleep(500);
                    Console.WriteLine("new flight arrived");
                    return;
                }
            }
        }

        //open new CheckIn
        public void OpenCheckIn()
        {
            for (int i = 0; i < Manager.checkins.Length; i++)
            {
                //enable new check in
                if (Manager.checkins[i].MyStatus == Check_In.Status.closed)
                {
                    //set airline name & status
                    Manager.checkins[i].MyStatus = Check_In.Status.open;
                    Manager.checkins[i].Name = AirlineGenerator();

                    //invoke event
                    Checkinhandler[i]?.Invoke(Manager.checkins[i], new CheckInChangedEventArgs(Manager.checkins[i].Name, Manager.checkins[i].MyStatus, i));

                    //start new thread
                    Thread checkinThread = new Thread(Manager.checkins[i].CheckLuggage);
                    checkinThread.Start();

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

        //count how many check ins are available
        int CheckClosedCheckIns()
        {
            int counter = 0;
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

            /* Monitor.Enter(Manager.checkins);
             try
             {
             }
             finally
             {
                 Monitor.Exit(Manager.checkins);
             }*/

            return counter;
        }
    }
}
