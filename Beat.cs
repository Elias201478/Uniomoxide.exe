using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;

namespace Uniomoxide
{
    internal class Beat
    {
        public static void player()
        {
            Func<double, double>[] formulas = new Func<double, double>[] //set the formulas here
            {
            // wow so pro code
            t => t/(t%55)*t,
            t => t % 2555 % (((int)t & ((int)t >> 10))),
            t => t + ((int)t & (int)t ^ (int)t | 2) - t * (((int)t >> 9) % (((int)t % 16 != 0) ? 2 : 6) & ((int)t >> 9)),
            t =>  t*(t/55)%t,
            t => ( ( t * t % 255 * t % 257 ) ),
            t => ( ( t + t % 25 + t % 217 ) ),
            t => ( t % ( t % ( t % 200 % t ) ) ),
            };

            int[] drs = new int[] { 20, 30, 30, 30, 30, 30, 30 };  //set the durations for bytebeats, 1st will play 5 seconds, 2nd will play 3...
            int[] sra = new int[] { 8000, 16000, 8000, 8000, 8000, 8000, 11025};  //frequency (sample rate)
            for (int i = 0; i < formulas.Length + 1; i++)
            {
                var formula = formulas[i];
                int dr = drs[i];
                int sr = sra[i];
                int bs = sr * dr;

                byte[] buffer = new byte[bs];
                for (int t = 0; t < bs; t++)
                {
                    buffer[t] = (byte)((int)formula(t) & 0xFF);
                }

                using (MemoryStream ms = new MemoryStream())
                using (BinaryWriter bw = new BinaryWriter(ms))
                {
                    bw.Write(new[] { (byte)'R', (byte)'I', (byte)'F', (byte)'F' });
                    bw.Write(36 + buffer.Length);
                    bw.Write(new[] { (byte)'W', (byte)'A', (byte)'V', (byte)'E' });
                    bw.Write(new[] { (byte)'f', (byte)'m', (byte)'t', (byte)' ' });
                    bw.Write(16);
                    bw.Write((short)1); // PCM
                    bw.Write((short)1); // mono
                    bw.Write(sr);  //sample rate
                    bw.Write(sr * 1 * 8 / 8);
                    bw.Write((short)(1 * 8 / 8));
                    bw.Write((short)8);  //bits per sample
                    bw.Write(new[] { (byte)'d', (byte)'a', (byte)'t', (byte)'a' });
                    bw.Write(buffer.Length);
                    bw.Write(buffer);

                    ms.Position = 0; //set the position in zero, important!

                    using (SoundPlayer player = new SoundPlayer(ms))
                    {
                        player.PlaySync();
                    }
                }
            }
        }
    }
}