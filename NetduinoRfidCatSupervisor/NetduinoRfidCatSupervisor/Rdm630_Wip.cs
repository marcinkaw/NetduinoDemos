#region Usings
using System.IO.Ports;
using SecretLabs.NETMF.Hardware.Netduino;
using Microsoft.SPOT.Hardware;
#endregion

namespace NetduinoRfidCatSupervisor
{
	public class Rdm630_Wip
	{
		#region Private Fields
		private readonly SerialPort _SerialPort;
        private string _LastSuccessfullRead;
		#endregion

        #region Publics
        public event NativeEventHandler DataReceived;

        public string TagId
        {
            get { return _LastSuccessfullRead; }
        }
        #endregion

        #region Constructors
        public Rdm630_Wip()
		{
			_SerialPort = new SerialPort(SerialPorts.COM1.ToString(), 9600, Parity.None, 8, StopBits.One);
			_SerialPort.ReadTimeout = 1000;
			_SerialPort.DataReceived += new SerialDataReceivedEventHandler(SerialPort_DataReceived);
			_SerialPort.Open();
		}
		#endregion

		#region Event Handler Delegates
		private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
		{
			// Alle Bytes lessen und in einen Puffer schreiben
			byte[] readBuffer = new byte[_SerialPort.BytesToRead];
			_SerialPort.Read(readBuffer, 0, readBuffer.Length);

			// Bytes im Puffer prüfen:
			// Die Länge des Puffer muss 14 sein
			// Das erste Byte (Index 0) muss 2 sein
			// Das vierzehnte Byte (Index 13) muss 3 sein
			if ((readBuffer.Length != 14) && (readBuffer[0] != 2) || (readBuffer[13] != 3)) return;

			// Die Prüfsummer ergibt sich aus: readBuffer[1] XOR readBuffer[2] XOR ... readBuffer[11]
			byte checksum = readBuffer[1];
			// Die zehn Datenbytes (Index 1 bis 10) aus dem Puffer lesen, Prüfsumme bilden und in ASCII Zeichen übersetzen
			string data = "";
			for (int index = 1; index < 11; ++index)
			{
				// Calculate checksum
				if (index > 1) checksum = (checksum ^= readBuffer[index]);
				// Byte in Zeichen übersetzen und zum Datenstring hinzufügen
				data += (char)readBuffer[index];
			}

			// Prüfsumme mit dem zwöften und dreizenhten Byte (Index 11 und 12) vergleichen
			if (checksum != (readBuffer[11] + readBuffer[12])) return;

			// Die Daten sind gültig und können nun weiterverarbeitet werden
			MyProcessData(data);
		}
		#endregion

		#region My Methods
		private void MyProcessData(string data)
		{
			// Hier werden die empfangenen Daten weiterverarbeitet
            _LastSuccessfullRead = data;
		}
		#endregion
	}
}
