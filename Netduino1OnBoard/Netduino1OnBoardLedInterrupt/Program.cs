using System;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware;
using SecretLabs.NETMF.Hardware.Netduino;

namespace Netduino1OnBoardLedInterrupt
{
	public class Program
	{
		static OutputPort led = new OutputPort(Pins.ONBOARD_LED, false);

		public static void Main()
		{

			InterruptPort button = new InterruptPort(Pins.ONBOARD_SW1, false, Port.ResistorMode.Disabled, Port.InterruptMode.InterruptEdgeBoth);
			button.OnInterrupt += new NativeEventHandler(button_OnInterrupt);

			Thread.Sleep(Timeout.Infinite);
		}

		static void button_OnInterrupt(uint data1, uint data2, DateTime time)
		{
			led.Write(data2 == 0);
		}
	}
}
