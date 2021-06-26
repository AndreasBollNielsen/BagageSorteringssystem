using System;
using System.Collections.Generic;
using System.Text;
namespace BagageSorteringssystem
{
    class CheckInChangedEventArgs: EventArgs
    {
       
        private string name;
        private Check_In.Status  myStatus;
        private int index;

        public int Index
        {
            get { return index; }
            set { index = value; }
        }


        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public Check_In.Status MyStatus
        {
            get { return myStatus; }
            set
            {
                myStatus = value;

            }
        }

        public CheckInChangedEventArgs(string name, Check_In.Status myStatus, int index)
        {
            this.name = name;
            this.myStatus = myStatus;
            this.index = index;
        }
    }
}
