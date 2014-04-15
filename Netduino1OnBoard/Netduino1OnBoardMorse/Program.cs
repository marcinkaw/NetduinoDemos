using System;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware;
using SecretLabs.NETMF.Hardware.Netduino;

namespace _04_Netduino1OnBoardMorse
{
	public class Program
	{
		public static void Main()
		{
			OutputPort led = new OutputPort(Pins.ONBOARD_LED, false);

			while (true)
			{
				foreach (char t in "HELLO WORLD")
				{
					for (int i = ",ETIANMSURWDKGOHVF,L,PJBXCYZQ,,54,3,,,2,,,,,,,16,,,,,,,7,,,8,90".IndexOf(t); i > 0; i /= 2)
					{
						led.Write(true);
						if ("-."[i-- % 2] == '.')
							Thread.Sleep(100);
						else
							Thread.Sleep(300);
						led.Write(false);
						Thread.Sleep(100);
					}
					Thread.Sleep(300);
					if (t.Equals(' ')) Thread.Sleep(400);
				}

				Thread.Sleep(1400);
			}
		}

	}
}
