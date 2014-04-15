using System;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware;
using SecretLabs.NETMF.Hardware.Netduino;

namespace Netduino3PiezoMorse
{
	public class Program
	{
		public static void Main()
		{
			int dashLength = 3;
			int pauseBetweenElements = 1;
			int pauseBetweenCharacters = 2;
			int pauseBetweenWords = 7;
			int duration = 90;

			PWM speaker = new PWM(Pins.GPIO_PIN_D5);

			while (true)
			{
				foreach (char curChar in "HELLO PO")
				{
					for (int index = ",ETINAMSDRGUKWOHBL,F,PJVXCQZYAA54A3AAA2AAAAAAA16AAAAAAA7AAA8A90".IndexOf(curChar); index > 0; index /= 2)
					{
						speaker.SetPulse(851u * 2, 851u);
						if ("-."[index-- % 2] == '.')
						{
							Thread.Sleep(duration);
							Debug.Print(".");
						}
						else
						{
							Thread.Sleep(duration * dashLength);
							Debug.Print("-");
						}
						speaker.SetDutyCycle(0);
						Thread.Sleep(duration * pauseBetweenElements);
					}
					Thread.Sleep(duration * pauseBetweenCharacters);
					Debug.Print(" ");
					if (curChar.Equals(' ')) Thread.Sleep(duration * pauseBetweenWords);
				}
				Thread.Sleep(1400);
			}



		}

	}
}
