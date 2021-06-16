﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace BagageSorteringssystem
{
    class Check_In
    {
        private string name;
        private Gate departureGate;

        public Gate DepartureGate
        {
            get { return departureGate; }
            set { departureGate = value; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        public enum Status { open, closed };
        private Status myStatus;

        public Status MyStatus
        {
            get { return myStatus; }
            set { myStatus = value; }
        }

        public Check_In(string name)
        {
            Name = name;
        }

        void AddToGateBuffer()
        {
            Random rand = new Random();

           


              Monitor.Enter(Manager.CheckinBuffer);
              try
              {
                if (Manager.CheckinBuffer.InternalLength > 0)
                {
                    Luggage luggage = Manager.CheckinBuffer.Remove();

                    //find the correct gate
                    departureGate = Manager.gates[luggage.Flight.IndexNumber];

                    luggage.Flight.DepartureGate = departureGate;
                    Manager.GateBuffer.Add(luggage);

                }

            }
              finally
              {

                  Monitor.Exit(Manager.CheckinBuffer);
              }
            Thread.Sleep(rand.Next(1000, 2500));





        }

        public void CheckLuggage()
        {

            while (myStatus == Status.open)
            {
               /* Monitor.Enter(Manager.CheckinBuffer);
                try
                {
                    if (Manager.CheckinBuffer.InternalLength > 0)
                    {
                        departureGate.Flight = Manager.CheckinBuffer.Inspect();

                    }

                   
                }
                finally
                {
                    Monitor.Enter(Manager.CheckinBuffer);

                }*/


                Monitor.Enter(departureGate);
                try
                {
                    Console.WriteLine(departureGate.NumLuggage);
                        AddToGateBuffer();

                    if (departureGate.NumLuggage < departureGate.Flight.MaxLuggage)
                    {

                    }
                    else
                    {
                        myStatus = Status.closed;
                    }

                }
                finally
                {
                    Monitor.Enter(departureGate);

                }

            }

            Console.WriteLine("no more room! closing checkin");
        }
    }
}
