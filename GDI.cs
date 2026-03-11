using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace Uniomoxide
{
    public struct BITMAPINFOHEADER
    {
        public uint biSize;
        public int biWidth;
        public int biHeight;
        public ushort biPlanes;
        public ushort biBitCount;
        public uint biCompression;
        public uint biSizeImage;
        public int biXPelsPerMeter;
        public int biYPelsPerMeter;
        public uint biClrUsed;
        public uint biClrImportant;
    }

    public struct BITMAPINFO
    {
        public BITMAPINFOHEADER bmiHeader;
        public uint bmiColors;
    }

    public struct HSL
    {
        public float h;
        public float s;
        public float l;
    }
    internal class GDI
    {
        [DllImport("gdi32.dll")]
        static extern IntPtr CreateDIBSection(IntPtr hdc, ref BITMAPINFO pbmi, uint iUsage, out IntPtr ppvBits, IntPtr hSection, uint dwOffset);

        [DllImport("gdi32.dll")]
        static extern IntPtr CreateCompatibleDC(IntPtr hdc);

        [DllImport("gdi32.dll")]
        static extern IntPtr SelectObject(IntPtr hdc, IntPtr h);

        [DllImport("gdi32.dll")]
        static extern bool BitBlt(IntPtr hdc, int x, int y, int cx, int cy, IntPtr hdcSrc, int x1, int y1, uint rop);

        [DllImport("user32.dll")]
        static extern IntPtr GetDC(IntPtr hwnd);

        const int BI_RGB = 0;
        const uint SRCCOPY = 0x00CC0020;

        static int w = 1920;
        static int hgt = 1080;

        static HSL RGBtoHSL(byte r, byte g, byte b)
        {
            float rf = r / 255f;
            float gf = g / 255f;
            float bf = b / 255f;

            float max = Math.Max(rf, Math.Max(gf, bf));
            float min = Math.Min(rf, Math.Min(gf, bf));
            float h = 0;
            float s;
            float l = (max + min) / 2f;

            if (max == min)
            {
                h = 0;
                s = 0;
            }
            else
            {
                float d = max - min;
                s = l > 0.5f ? d / (2f - max - min) : d / (max + min);

                if (max == rf) h = (gf - bf) / d + (gf < bf ? 6 : 0);
                else if (max == gf) h = (bf - rf) / d + 2;
                else h = (rf - gf) / d + 4;

                h /= 6f;
            }

            return new HSL { h = h, s = s, l = l };
        }

        static void HSLtoRGB(HSL hsl, out byte r, out byte g, out byte b)
        {
            float h = hsl.h;
            float s = hsl.s;
            float l = hsl.l;

            float rF, gF, bF;

            if (s == 0)
            {
                rF = gF = bF = l;
            }
            else
            {
                float q = l < 0.5f ? l * (1 + s) : l + s - l * s;
                float p = 2 * l - q;

                rF = HueToRGB(p, q, h + 1f / 3f);
                gF = HueToRGB(p, q, h);
                bF = HueToRGB(p, q, h - 1f / 3f);
            }

            r = (byte)(rF * 255);
            g = (byte)(gF * 255);
            b = (byte)(bF * 255);
        }

        static float HueToRGB(float p, float q, float t)
        {
            if (t < 0) t += 1;
            if (t > 1) t -= 1;
            if (t < 1f / 6f) return p + (q - p) * 6 * t;
            if (t < 1f / 2f) return q;
            if (t < 2f / 3f) return p + (q - p) * (2f / 3f - t) * 6;
            return p;
        }

        static IntPtr g_hdcScreen;
        static IntPtr g_hdcMem;
        static IntPtr g_hbmTemp;
        static int g_w;
        static int g_h;

        public static void GDIBow()
        {
            DateTime startTime = DateTime.Now;
            TimeSpan maxDuration = TimeSpan.FromSeconds(140);    //duration of effect
            while (DateTime.Now - startTime < maxDuration)
            {
                IntPtr screen = GetDC(IntPtr.Zero);

                BITMAPINFO bi = new BITMAPINFO();
                bi.bmiHeader.biSize = (uint)Marshal.SizeOf(typeof(BITMAPINFOHEADER));
                bi.bmiHeader.biWidth = w;
                bi.bmiHeader.biHeight = -hgt;
                bi.bmiHeader.biPlanes = 1;
                bi.bmiHeader.biBitCount = 32;
                bi.bmiHeader.biCompression = BI_RGB;

                IntPtr bits;
                IntPtr bmp = CreateDIBSection(screen, ref bi, 0, out bits, IntPtr.Zero, 0);

                IntPtr memdc = CreateCompatibleDC(screen);
                SelectObject(memdc, bmp);

                BitBlt(memdc, 0, 0, w, hgt, screen, 0, 0, SRCCOPY);

                int total = w * hgt;
                int[] px = new int[total];
                Marshal.Copy(bits, px, 0, total);

                Random rng = new Random();
                float speed = 0.08f;

                while (DateTime.Now - startTime < maxDuration)
                {
                    for (int i = 0; i < total; i++)
                    {
                        int p = px[i];
                        byte b = (byte)(p & 0xFF);
                        byte g = (byte)((p >> 8) & 0xFF);
                        byte r = (byte)((p >> 16) & 0xFF);

                        HSL hsl = RGBtoHSL(r, g, b);
                        hsl.h += speed;
                        if (hsl.h > 1) hsl.h -= 1;

                        HSLtoRGB(hsl, out r, out g, out b);
                        px[i] = b | (g << 8) | (r << 16);
                    }

                    Marshal.Copy(px, 0, bits, total);
                    BitBlt(screen, 0, 0, w, hgt, memdc, 0, 0, SRCCOPY);
                    Thread.Sleep(1);
                }
            }
        }
        public static void GDIBow2()
        {
            DateTime startTime = DateTime.Now;
            TimeSpan maxDuration = TimeSpan.FromSeconds(30);    //duration of effect
            while (DateTime.Now - startTime < maxDuration)
            {
                IntPtr screen = GetDC(IntPtr.Zero);

                BITMAPINFO bi = new BITMAPINFO();
                bi.bmiHeader.biSize = (uint)Marshal.SizeOf(typeof(BITMAPINFOHEADER));
                bi.bmiHeader.biWidth = w;
                bi.bmiHeader.biHeight = -hgt;
                bi.bmiHeader.biPlanes = 1;
                bi.bmiHeader.biBitCount = 32;
                bi.bmiHeader.biCompression = BI_RGB;

                IntPtr bits;
                IntPtr bmp = CreateDIBSection(screen, ref bi, 0, out bits, IntPtr.Zero, 0);

                IntPtr memdc = CreateCompatibleDC(screen);
                SelectObject(memdc, bmp);

                BitBlt(memdc, 0, 0, w, hgt, screen, 0, 0, SRCCOPY);

                int total = w * hgt;
                int[] px = new int[total];
                Marshal.Copy(bits, px, 0, total);

                Random rng = new Random();
                float speed = 0.08f;

                while (DateTime.Now - startTime < maxDuration)
                {
                    for (int i = 0; i < total; i++)
                    {
                        int p = px[i];
                        byte b = (byte)(p & 0xFF);
                        byte g = (byte)((p >> 14) & 0xFF);
                        byte r = (byte)((p >> 16) & 0xFF);

                        HSL hsl = RGBtoHSL(r, g, b);
                        hsl.h += speed;
                        if (hsl.h > 1) hsl.h -= 5;

                        HSLtoRGB(hsl, out r, out g, out b);
                        px[i] = g | (g << 14) | (r << 16);
                    }

                    Marshal.Copy(px, 0, bits, total);
                    BitBlt(screen, 0, 0, w, hgt, memdc, 0, 0, SRCCOPY);
                    Thread.Sleep(1);
                }
            }
        }

        public static void GDIBow3()
        {
            DateTime startTime = DateTime.Now;
            TimeSpan maxDuration = TimeSpan.FromSeconds(30);    //duration of effect
            while (DateTime.Now - startTime < maxDuration)
            {
                IntPtr screen = GetDC(IntPtr.Zero);

                BITMAPINFO bi = new BITMAPINFO();
                bi.bmiHeader.biSize = (uint)Marshal.SizeOf(typeof(BITMAPINFOHEADER));
                bi.bmiHeader.biWidth = w;
                bi.bmiHeader.biHeight = -hgt;
                bi.bmiHeader.biPlanes = 1;
                bi.bmiHeader.biBitCount = 32;
                bi.bmiHeader.biCompression = BI_RGB;

                IntPtr bits;
                IntPtr bmp = CreateDIBSection(screen, ref bi, 0, out bits, IntPtr.Zero, 0);

                IntPtr memdc = CreateCompatibleDC(screen);
                SelectObject(memdc, bmp);

                BitBlt(memdc, 0, 0, w, hgt, screen, 0, 0, SRCCOPY);

                int total = w * hgt;
                int[] px = new int[total];
                Marshal.Copy(bits, px, 0, total);

                Random rng = new Random();
                float speed = 0.1f;

                while (DateTime.Now - startTime < maxDuration)
                {
                    for (int i = 0; i < total; i++)
                    {
                        int p = px[i];
                        byte b = (byte)(p & 0xFF);
                        byte g = (byte)((p / 8) & 0xFF);
                        byte r = (byte)((p >> 16) & 0xFF);
                        byte gj = (byte)((p >> 16) & (0xFF));

                        HSL hsl = RGBtoHSL(r, g, b);
                        hsl.h += speed;
                        if (hsl.h > 1) hsl.h -= 1;

                        HSLtoRGB(hsl, out r, out g, out b);
                        px[i] = b | (r << 8) | (gj << 16);
                    }

                    Marshal.Copy(px, 0, bits, total);
                    BitBlt(screen, 0, 0, w, hgt, memdc, 0, 0, SRCCOPY);
                    Thread.Sleep(1);
                }
            }
        }
    }
}