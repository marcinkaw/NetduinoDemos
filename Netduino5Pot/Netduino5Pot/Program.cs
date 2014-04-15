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
        //// changing the blinking speed with potentiomenter
        //public static void Main()
        //{
        //    OutputPort led = new OutputPort(Pins.GPIO_PIN_D0, false);
            
        //    AnalogInput pot = new AnalogInput(Pins.GPIO_PIN_A0);

        //    int potValue = 0;

        //    pot.SetRange(100, 250);
        //    while (true)
        //    {
        //        potValue = pot.Read();
        //        led.Write(true);
        
        //        Thread.Sleep(potValue);
        //        led.Write(false);
        
        //        Thread.Sleep(potValue);
        //    }
        //}

        // Dimming the LED light with potetniometer
        public static void Main()
        {
            PWM led = new PWM(Pins.GPIO_PIN_D5);

            AnalogInput pot = new AnalogInput(Pins.GPIO_PIN_A0);

            int potValue = 0;

            pot.SetRange(0, 100);

            while (true)
            {
                potValue = pot.Read();
                led.SetDutyCycle((uint)potValue);

            }
        }
    }
}
