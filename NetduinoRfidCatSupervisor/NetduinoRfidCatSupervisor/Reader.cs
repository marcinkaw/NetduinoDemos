using System;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware;
using SecretLabs.NETMF.Hardware.Netduino;
using System.IO.Ports;

namespace NetduinoRfidCatSupervisor
{
    
    class Reader
    {
        // NOTE From Microsoft.SPOT.Hardware.SerialPort.dll
        public SerialPort SerialPort;

        public event NativeEventHandler DataReceived;


        public Reader()
        {
                    // NOTE portName = Valid port name
            // NOTE boundRoute = 9600 (TTL Electricity Level RS232 format)
            // NOTE Parity = Parity.None - no parity check
            // NOTE stopBits = one stop bit between units of data
            // NOTE info from documentation RFID-reader-RDM630-Spec.pdf - 9600bps,N,8,1 
            this.SerialPort = new SerialPort(SerialPorts.COM1.ToString(), 9600, Parity.None, 8, StopBits.One);

            // NOTE I don;t think its neccessary
            this.SerialPort.ReadTimeout = 5000;

            this.SerialPort.DataReceived += new SerialDataReceivedEventHandler(SerialPort_DataReceived);

            this.SerialPort.Open();
        }

        void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                Debug.Print(this.SerialPort.BytesToRead.ToString());

                //if (this.SerialPort.BytesToRead != 14)
                //{
                //    this.SerialPort.DiscardInBuffer();
                //    return;
                //}

                

                byte[] buffer = new byte[this.SerialPort.BytesToRead];
                this.SerialPort.Read(buffer, 0, SerialPort.BytesToRead);

                //string s = string.Empty;

                //for(int i = 0; i < SerialPort.BytesToRead; i++)
                //{
                //    s += buffer[i];
                //}

                //Debug.Print(s);

                // Check output
                // byte 0 schould be 2
                // byte 13 schould be 3
                if (buffer[0] != 2 || buffer[13] != 3) return;

                //// Card Number Data Charasters
                //char[] cardNumberDataChars = new char[10];
                //for (int i = 0; i < 10; i++)
                //{
                //    // Copy data character
                //    cardNumberDataChars[i] = (char)buffer[i+1];
                //}
                //string cardNumberString = new string(cardNumberDataChars);
                //Debug.Print(cardNumberString);

                // Calculate and check the checksume
                //// byte 12 gets the chekcsum from b[1] XOR b[2] XOR ... b[11]
                //byte checksum = buffer[1];
                //for (int i = 2; i < 11; i++)
                //{
                //    // Calculate checksum
                //    checksum = checksum ^= buffer[i];
                //}
                //// checksum is send in b[12]
                ////if (checksum != buffer[12]) return;

                // Get the tag string
                string cardNumberString = string.Empty;
                for (int i = 1; i < 11; i++)
                {
                    cardNumberString += (char)buffer[i];
                }
                Debug.Print(cardNumberString);

                switch (cardNumberString)
                {
                    case "x": // CAT no1
                        break;
                    case "y": // CAT no2
                        break;
                    default:
                        // INTRUDER ;)
                        break;
                }

                DataReceived(0, 0, DateTime.Now);

            }
            catch (Exception er)
            {
                Debug.Print(er.Message);
            }

        }
    }
}
