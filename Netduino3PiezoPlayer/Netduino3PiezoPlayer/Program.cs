using System;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware;
using SecretLabs.NETMF.Hardware.Netduino;

namespace NetduinoApplication4
{
    public class Program
    {
        // store the notes on the music scale and their associated pulse lengths
        static System.Collections.Hashtable scale = new System.Collections.Hashtable();
        static string song;
        static int beatsPerMinute;
        static int beatTimeInMilliseconds;
        static int pauseTimeInMilliseconds;
        static PWM speaker;
            
        public static void Main()
        {
            // write your code here

            // low octave
            scale.Add("c", 1915u);
            scale.Add("d", 1700u);
            scale.Add("e", 1519u);
            scale.Add("f", 1432u);
            scale.Add("g", 1275u);
            scale.Add("a", 1136u);
            scale.Add("b", 1014u);
            // high octave
            scale.Add("C", 956u);
            scale.Add("D", 851u);
            scale.Add("E", 758u);
            // silence ("hold note")
            scale.Add("h", 0u);
            
            beatsPerMinute = 90;
            beatTimeInMilliseconds = 60000 / beatsPerMinute; // 60,000 milliseconds per minute

            pauseTimeInMilliseconds = (int)(beatTimeInMilliseconds * 0.1);
            // define the song (letter of note followed by length of note)
            //song = "C1C1C1g1a1a1g2E1E1D1D1C2";
            song = "c1d1e1f1g1a1b1";

            // define the speaker
            speaker = new PWM(Pins.GPIO_PIN_D5);
            // interpret and play the song

            InterruptPort sw = new InterruptPort(Pins.ONBOARD_SW1, false, Port.ResistorMode.Disabled, Port.InterruptMode.InterruptEdgeHigh);
            sw.OnInterrupt += new NativeEventHandler(sw_OnInterrupt);


            
            

            Thread.Sleep(Timeout.Infinite);

        }

        static void sw_OnInterrupt(uint data1, uint data2, DateTime time)
        {
            PlaySong();
        }

        private static void PlaySong()
        {
            for (int i = 0; i < song.Length; i += 2)
            {
                // extract each note and its length in beats
                string note = song.Substring(i, 1);
                int beatCount = int.Parse(song.Substring(i + 1, 1));
                // look up the note duration (in microseconds)
                uint noteDuration = (uint)scale[note];
                // play the note for the desired number of beats
                speaker.SetPulse(noteDuration * 2, noteDuration);
                Thread.Sleep(
                  beatTimeInMilliseconds * beatCount - pauseTimeInMilliseconds);
                // pause for 1/10th of a beat in between every note.
                speaker.SetDutyCycle(0);
                Thread.Sleep(pauseTimeInMilliseconds);
            }
        }

    }
}
