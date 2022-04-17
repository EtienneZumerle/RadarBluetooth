using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Threading;
using LattePanda.Firmata;
using System.Diagnostics;



namespace Beacons
{
    internal class arduinoCOM
    {
        Arduino arduino = new Arduino();


        private readonly Thread _threadIo;


        int rssi;
        bool newRssi = false;
        //For servo
        static int step = 40;
        int startAngle = 0;
        int stopAngle = 180;


        int[] data = new int[181];

        int numberOfValues;
        int loopNumber = 1;
        int maxAngle;
        int maxVal = -1000;
        bool loopEnd = false;
        public arduinoCOM(string com) //constructor
        {

            arduino.pinMode(12, Arduino.OUTPUT);
            arduino.pinMode(9, Arduino.SERVO);
            _threadIo = new Thread(io);


            _threadIo.Start();


        }
        public void setRssi(string RSSI)
        {
            rssi = int.Parse(RSSI); //convert received rssi string to int
            //Trace.WriteLine(rssi); //for debug

            newRssi = true;

        }


        public void io()
        {

            while (true)
            {
                startAngle = 0;
                stopAngle = 180;
                maxVal = -1000;
                maxAngle = 0;
                step = 40;
                for (int i = 0; i < 181; i++)
                {
                    data[i] = 0;
                }
                arduino.servoWrite(9, 0);
                Thread.Sleep(300);
                for (int k =0; k < 6; k++)
                {
                    int i = startAngle;
                    while (i <= stopAngle)
                    {
                        if (!newRssi)
                        {
                            Thread.Sleep(100);
                        }
                        else
                        {
                            arduino.servoWrite(9, i);
                            Thread.Sleep(100);
                            data[i] = rssi;
                            newRssi = false;                           
                            i = i + step;
                        }


                    }

                    for (int j = 0; j < data.Length; j++)
                    {
                        if ((data[j] > maxVal) && (data[j] != 0))
                        {
                            maxAngle = j;
                            maxVal = data[j];
                        }
                    }
                    Trace.WriteLine(maxAngle);
                    maxVal = -1000;
                    //calculate startAngle and stopAngle
                    if (maxAngle < step)
                    {
                        startAngle = 0;
                        stopAngle = 2 * step + maxAngle;
                    }
                    else if (maxAngle > (180 - step))
                    {
                        startAngle = -(2 * step + maxAngle);
                        stopAngle = 0;
                    }
                    else
                    {
                        startAngle = maxAngle - step;
                        stopAngle = maxAngle + step;
                    }
                    if (step >= 2)
                    {
                        step = step / 2;
                    }
                }
            }
        }



    }


}
