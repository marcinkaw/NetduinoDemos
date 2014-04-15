using System;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware;
using SecretLabs.NETMF.Hardware.Netduino;

namespace NetduinoApplication5
{
    public class Program
    {
        static InterruptPort sw;
        static uint firstPosition;
        static uint lastPosition;
        static PWM servo;

        static OutputPort led;
            

        public static void Main()
        {
            led = new OutputPort(Pins.ONBOARD_LED, false);

            firstPosition = 1000;
            lastPosition = 2000;

            servo = new PWM(Pins.GPIO_PIN_D5);

            sw = new InterruptPort(Pins.ONBOARD_SW1, false, Port.ResistorMode.Disabled, Port.InterruptMode.InterruptEdgeHigh);

            sw.OnInterrupt += new NativeEventHandler(sw_OnInterrupt);

            Thread.Sleep(Timeout.Infinite);

        }

        static void sw_OnInterrupt(uint data1, uint data2, DateTime time)
        {
            led.Write(true);

            // write your code here
            
            // move through the full range of positions
            for (uint currentPosition = firstPosition;
                 currentPosition <= lastPosition;
                 currentPosition += 10)
            {
                // move the servo to the new position.
                servo.SetPulse(20000, currentPosition);
                Thread.Sleep(100);
            }
            // return to first position and wait a half second.
            servo.SetPulse(20000, firstPosition);

            led.Write(false);
        }

    }
}
