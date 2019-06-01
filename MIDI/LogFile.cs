using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab4
{
    class LogFile
    {
        private byte[] ADData;
        private byte[] PWMData;
        private string str = "{";
        
        public byte[] AD
        {
            get
            {
                return ADData;
            }
            set
            {               
                ADData = value;                
            }
        }

        public byte[] PWM
        {
            get
            {
                return PWMData;
            }
            set
            {
                PWMData = value;
            }
        }

        public string getString
        {
            get
            {
                return str;
            }
            set
            {                
                str = value;
            }
        }


        public void setString()
        {
            string t1 = "";
            string t2 = "";
            for (int i = 0; i < ADData.Length; i++)
            {
                if (i == ADData.Length - 1)
                {
                    t1 += string.Format("{0:X2}", ADData[i]);
                }
                else
                {
                    t1 += string.Format("{0:X2}", ADData[i]) + " ";
                }
            }

            for (int i = 0; i < PWMData.Length; i++)
            {
                if (i == PWMData.Length - 1)
                {
                    t2 += string.Format("{0:X2}", PWMData[i]);
                }

                else
                {
                    t2 += string.Format("{0:X2}", PWMData[i]) + " ";
                }
            }
            str += "AD:" + t1 + "  " + "PWM:" + t2 + "\n";
        }
    }
}
