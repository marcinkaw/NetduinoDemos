using System;
using System.Collections;
using System.IO.Ports;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware;
using SecretLabs.NETMF.Hardware.Netduino;

namespace NetduinoRfidCatMonitoringSystem
{
    public class Program
    {
        private static DateTime _LastDataReceivedAt;
        private static byte[] _PermanentBuffer;
        private static int _PermantIndex;
        private static SerialPort _SerialPort;
        private static Hashtable _TagIdLeds;

        public static void Main()
        {
            _TagIdLeds = new Hashtable(2);
            _TagIdLeds.Add("0006140626", new OutputPort(Pins.GPIO_PIN_D8, false)); // Blue > #1
            _TagIdLeds.Add("0006052425", new OutputPort(Pins.GPIO_PIN_D10, false)); // Yellow > #2
            _TagIdLeds.Add("0000454624", new OutputPort(Pins.GPIO_PIN_D12, false)); // Red > #3

            _LastDataReceivedAt = DateTime.Now;
            _PermanentBuffer = new byte[14];
            _PermantIndex = 0;

            InterruptPort interruptPort =
                new InterruptPort(
                    Pins.ONBOARD_SW1,
                    false,
                    Port.ResistorMode.Disabled,
                    Port.InterruptMode.InterruptEdgeHigh);

            interruptPort.OnInterrupt += (data1, data2, time) =>
            {
                // Press reset button to switch all LEDs off
                foreach (OutputPort outputPort in _TagIdLeds.Values)
                {
                    outputPort.Write(false);
                }
            };

            _SerialPort = new SerialPort(SerialPorts.COM1.ToString(), 9600, Parity.None, 8, StopBits.One);
            _SerialPort.ReadTimeout = 1000;
            _SerialPort.DataReceived += new SerialDataReceivedEventHandler(SerialPort_DataReceived);
            _SerialPort.Open();

            while (true)
            {
                // Infinite loop
            }
        }

        private static void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            DateTime dataReceivedAt = DateTime.Now;

            byte[] readBuffer = new byte[_SerialPort.BytesToRead];
            _SerialPort.Read(readBuffer, 0, readBuffer.Length);

            Debug.Print("Data received. Buffer length: " + readBuffer.Length.ToString());
            for (int index1 = 0; index1 < readBuffer.Length; index1++)
            {
                _PermanentBuffer[_PermantIndex] = readBuffer[index1];
                _PermantIndex++;
                if (_PermantIndex == 14)
                {
                    Debug.Print("14 bytes completed.");
                    _PermantIndex = 0;

                    TimeSpan interval = (dataReceivedAt - _LastDataReceivedAt);
                    int ellapsedMilliseconds = 
                        (interval.Days * 24 * 60 * 60 * 1000) +
                        (interval.Hours * 60 * 60 * 1000) +
                        (interval.Minutes * 60 * 1000) +
                        (interval.Seconds * 1000) +
                        interval.Milliseconds;
                    Debug.Print("Last data received at: " + _LastDataReceivedAt.ToString("dd.MM.yyyy hh:mm:ss.fff"));
                    Debug.Print("Data received at: " + dataReceivedAt.ToString("dd.MM.yyyy hh:mm:ss.fff"));
                    Debug.Print("Ellapsed milliseconds: " + ellapsedMilliseconds.ToString());
                    if (ellapsedMilliseconds > 1000)
                    {
                        _LastDataReceivedAt = dataReceivedAt;
                        string data;
                        if (ValidateBuffer(out data))
                        {
                            ProcessData(data);
                        }
                        else
                        {
                            Debug.Print("Data validation failed.");
                        }
                    }
                    else
                    {
                        Debug.Print("Receiving interval is to short.");
                    }
                    break;
                }
            }
        }

        private static void ProcessData(string data)
        {
            if (_TagIdLeds.Contains(data))
            {
                Debug.Print("Received tag is supported: " + data);
                ((OutputPort)_TagIdLeds[data]).Write(!((OutputPort)_TagIdLeds[data]).Read());
            }
            else
            {
                Debug.Print("Received tag is not supported: " + data);
            }
        }

        private static bool ValidateBuffer(out string data)
        {
            data = "";

            if ((_PermanentBuffer[0] != 2) || (_PermanentBuffer[13] != 3)) return false;

            uint checksum1 = 0;
            string lastValue = null;
            for (int index2 = 1; index2 < 11; ++index2)
            {
                string value = ((char)_PermanentBuffer[index2]).ToString();
                data += value;
                if (index2 % 2 == 0) checksum1 ^= Hex2Dec(lastValue + value);
                lastValue = value;
            }

            uint checksum2 = Hex2Dec(((char)_PermanentBuffer[11]).ToString() + ((char)_PermanentBuffer[12]).ToString());
            if (checksum1 != checksum2) return false;

            data = ZeroFill(Hex2Dec(data.Substring(2)).ToString(), 10);
            return true;
        }

        public static uint Hex2Dec(string hexNumber)
        {
            hexNumber = hexNumber.ToUpper();
            string conversionTable = "0123456789ABCDEF";
            uint retValue = 0;
            uint multiplier = 1;

            for (int index = hexNumber.Length - 1; index >= 0; --index)
            {
                retValue += (uint)(multiplier * (conversionTable.IndexOf(hexNumber[index])));
                multiplier = (uint)(multiplier * conversionTable.Length);
            }

            return retValue;
        }

        public static string ZeroFill(string number, int digits)
        {
            bool negative = false;
            if (number.Substring(0, 1) == "-")
            {
                negative = true;
                number = number.Substring(1);
            }

            for (int Counter = number.Length; Counter < digits; ++Counter)
            {
                number = "0" + number;
            }
            if (negative) number = "-" + number;
            return number;
        }
    }
}
