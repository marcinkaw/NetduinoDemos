using System;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware;
using SecretLabs.NETMF.Hardware.Netduino;
using System.IO.Ports;
using System.Collections;

namespace NetduinoRfidCatSupervisor
{
    public class Program
    {
        private static Hashtable Cats;
        private static Rdm630 RFID;

        public static void Main()
        {
            Cats = new Hashtable(2);
            Cats.Add("0100447476", new OutputPort(Pins.GPIO_PIN_D8, false));
            Cats.Add("0100037390", new OutputPort(Pins.GPIO_PIN_D10, false));
            Cats.Add("010006EFE0", new OutputPort(Pins.GPIO_PIN_D12, false));

            RFID = new Rdm630("COM1");
            RFID.DataReceived += new NativeEventHandler(RFID_DataReceived);

            while (true)
            {
                foreach(OutputPort catLed in Cats.Values)
                {
                    catLed.Write(catLed.Read());
                }
                Thread.Sleep(250);
            }
        }

        /// <param name="Time">Date and time of the event</param>
        static void RFID_DataReceived(uint Unused1, uint Unused2, DateTime Time)
        {
            Debug.Print("Cat with TagId " + RFID.Tag + " passed by.");
            ((OutputPort)Cats[RFID.Tag]).Write(!((OutputPort)Cats[RFID.Tag]).Read());
            
        }

        //// InBoardLed
        //public static OutputPort onBoardLed;

        //public static SerialPort SerialPort;

        //public static void Main()
        //{
        //    onBoardLed = new OutputPort(Pins.ONBOARD_LED, false);

        //    //Reader reader = new Reader();
        //    //reader.DataReceived += new NativeEventHandler(reader_DataReceived);
        //    SerialPort = new SerialPort(SerialPorts.COM1.ToString(), 9600, Parity.None, 8, StopBits.One);

        //    // NOTE I don;t think its neccessary
        //    SerialPort.ReadTimeout = 5000;

        //    SerialPort.DataReceived += new SerialDataReceivedEventHandler(SerialPort_DataReceived);

        //    Thread.Sleep(Timeout.Infinite);
        //}

        //static void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        //{
        //    try
        //    {
        //        Debug.Print(SerialPort.BytesToRead.ToString());

        //        byte[] buffer = new byte[SerialPort.BytesToRead];
        //        SerialPort.Read(buffer, 0, SerialPort.BytesToRead);

        //        for (int ci = 0; ci < buffer.Length; ci++)
        //        {
        //            if (buffer[ci] == 2)
        //            {
        //                // Get the tag string
        //                string cardNumberString = string.Empty;
        //                for (int i = 1; i < 11; i++)
        //                {
        //                    if (buffer.Length > ci + i)
        //                    {
        //                        cardNumberString += (char)buffer[ci+i];
        //                    }
        //                    ci++;
        //                }
        //                Debug.Print(cardNumberString);

        //                switch (cardNumberString)
        //                {
        //                    case "x": // CAT no1
        //                        break;
        //                    case "y": // CAT no2
        //                        break;
        //                    default:
        //                        // INTRUDER ;)
        //                        break;
        //                }
        //            }
        //        }

        //    }
        //    catch (Exception er)
        //    {
        //        Debug.Print(er.Message);
        //    }

        //    onBoardLed.Write(true);
        //    Thread.Sleep(500);
        //    onBoardLed.Write(false);
        //}



    }
}
