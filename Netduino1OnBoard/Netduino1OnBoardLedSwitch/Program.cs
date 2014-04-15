using System;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware;
using SecretLabs.NETMF.Hardware.Netduino;

namespace Netduino1OnBoardLedSwitch
{
	public class Program
	{
		public static void Main()
		{
			OutputPort led = new OutputPort(Pins.ONBOARD_LED, false);

			InputPort button = new InputPort(Pins.ONBOARD_SW1, false, Port.ResistorMode.Disabled);

			bool isPressed;

			while (true)
			{
				isPressed = button.Read();

				led.Write(!isPressed);
			}

		}

	}
}
