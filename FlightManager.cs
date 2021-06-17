using System;
using System.Collections.Generic;
using System.Text;

namespace BagageSorteringssystem
{
    class FlightManager
    {
        public static DateTime CurrentTime = DateTime.Now;
        public static float TimeFactor = 1;
        public void RunTime()
        {
            while (true)
            {
                CurrentTime.AddSeconds(1.0f * TimeFactor);
                Console.WriteLine(CurrentTime);

            }
        }
    }
}
