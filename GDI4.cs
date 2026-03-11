using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace Uniomoxide
{
    internal class GDI4
    {
        [DllImport("gdi32.dll")] static extern IntPtr CreateCompatibleDC(IntPtr hdc);
        [DllImport("gdi32.dll")] static extern bool DeleteDC(IntPtr hdc);
        [DllImport("gdi32.dll")] static extern IntPtr CreateDIBSection(IntPtr hdc, ref BITMAPINFO bmi, uint usage, out IntPtr bits, IntPtr h, uint offset);
        [DllImport("gdi32.dll")] static extern IntPtr SelectObject(IntPtr hdc, IntPtr obj);
        [DllImport("gdi32.dll")] static extern bool BitBlt(IntPtr d, int dx, int dy, int w, int h, IntPtr s, int sx, int sy, uint rop);
        [DllImport("gdi32.dll")] static extern bool PlgBlt(IntPtr hdc, POINT[] p, IntPtr src, int x, int y, int w, int h, IntPtr mask, int mx, int my);
        [DllImport("user32.dll")] static extern IntPtr GetDC(IntPtr hwnd);
        [DllImport("user32.dll")] static extern int ReleaseDC(IntPtr hwnd, IntPtr hdc);
        [DllImport("user32.dll")] static extern int GetSystemMetrics(int n);

        const uint SRCCOPY = 0x00CC0020;

        [StructLayout(LayoutKind.Sequential)]
        struct POINT { public int x, y; }

        [StructLayout(LayoutKind.Sequential)]
        struct BITMAPINFOHEADER
        {
            public uint biSize;
            public int biWidth, biHeight;
            public ushort biPlanes, biBitCount;
            public uint biCompression, biSizeImage;
            public int biXPelsPerMeter, biYPelsPerMeter;
            public uint biClrUsed, biClrImportant;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct BITMAPINFO
        {
            public BITMAPINFOHEADER bmiHeader;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
            public uint[] bmiColors;
        }

        static IntPtr sdc, srcdc, rotdc, outdc;
        static IntPtr srcbmp, rotbmp, outbmp;
        static IntPtr srcbits, rotbits, outbits;
        static int sw, sh;
        static double a = 0;
        static int dir = 1;
        static bool stop = false;

        static HSL RGBtoHSL(byte r, byte g, byte b)
        {
            float R = r / 255f, G = g / 255f, B = b / 255f;
            float max = Math.Max(R, Math.Max(G, B));
            float min = Math.Min(R, Math.Min(G, B));
            float L = (max + min) * 0.5f;
            float H = 0, S = 0;
            if (max != min)
            {
                float d = max - min;
                S = (L > 0.5f) ? d / (2 - max - min) : d / (max + min);
                if (max == R) H = (G - B) / d + (G < B ? 6 : 0);
                else if (max == G) H = (B - R) / d + 2;
                else H = (R - G) / d + 4;
                H /= 6;
            }
            return new HSL { h = H, s = S, l = L };
        }

        static float hue2rgb(float p, float q, float t)
        {
            if (t < 0) t += 1;
            if (t > 1) t -= 1;
            if (t < 1f / 6) return p + (q - p) * 6 * t;
            if (t < 1f / 2) return q;
            if (t < 2f / 3) return p + (q - p) * (2f / 3 - t) * 6;
            return p;
        }

        static void HSLtoRGB(HSL hsl, out byte r, out byte g, out byte b)
        {
            float H = hsl.h, S = hsl.s, L = hsl.l;
            float R, G, B;
            if (S == 0) R = G = B = L;
            else
            {
                float q = (L < 0.5f) ? L * (1 + S) : L + S - L * S;
                float p = 2 * L - q;
                R = hue2rgb(p, q, H + 1f / 3);
                G = hue2rgb(p, q, H);
                B = hue2rgb(p, q, H - 1f / 3);
            }
            r = (byte)(R * 255);
            g = (byte)(G * 255);
            b = (byte)(B * 255);
        }

        struct HSL { public float h, s, l; }

        static void rot()
        {
            POINT[] p = new POINT[3];
            double r = a * 0.017453292519943;
            double c = Math.Cos(r), s = Math.Sin(r);
            int cx = sw >> 1, cy = sh >> 1;
            int x0 = -cx, y0 = -cy;
            int x1 = sw - cx, y1 = -cy;
            int x2 = -cx, y2 = sh - cy;
            p[0].x = cx + (int)(x0 * c - y0 * s);
            p[0].y = cy + (int)(x0 * s + y0 * c);
            p[1].x = cx + (int)(x1 * c - y1 * s);
            p[1].y = cy + (int)(x1 * s + y1 * c);
            p[2].x = cx + (int)(x2 * c - y2 * s);
            p[2].y = cy + (int)(x2 * s + y2 * c);
            PlgBlt(rotdc, p, srcdc, 0, 0, sw, sh, IntPtr.Zero, 0, 0);
        }

        static void HSLThread()
        {
            DateTime startTime = DateTime.Now;
            TimeSpan maxDuration = TimeSpan.FromSeconds(30);
            int w = sw, h = sh;
            float speed = 0.08f;

            unsafe
            {
                byte* px = (byte*)outbits;

                while (DateTime.Now - startTime < maxDuration)
                {
                    for (int i = 0; i < w * h; i++)
                    {
                        byte* p = px + i * 4;
                        HSL hsl = RGBtoHSL(p[2], p[1], p[0]);
                        hsl.s *= 2f;
                        if (hsl.s > 1) hsl.s = 1;
                        hsl.l *= 1.25f;
                        if (hsl.l > 1) hsl.l = 1;
                        hsl.h += speed;
                        if (hsl.h > 1) hsl.h -= 1;
                        HSLtoRGB(hsl, out byte rr, out byte gg, out byte bb);
                        p[2] = rr;
                        p[1] = gg;
                        p[0] = bb;
                    }

                    BitBlt(sdc, 0, 0, w, h, outdc, 0, 0, SRCCOPY);
                    Thread.Sleep(1);
                }
            }
        }
        public static void ROXT()
        {
            DateTime startTime = DateTime.Now;
            TimeSpan maxDuration = TimeSpan.FromSeconds(30);    //duration of effect
            while (DateTime.Now - startTime < maxDuration)
            {
                sdc = GetDC(IntPtr.Zero);
                sw = GetSystemMetrics(0);
                sh = GetSystemMetrics(1);

                srcdc = CreateCompatibleDC(sdc);
                rotdc = CreateCompatibleDC(sdc);
                outdc = CreateCompatibleDC(sdc);

                BITMAPINFO bi = new BITMAPINFO();
                bi.bmiHeader.biSize = (uint)Marshal.SizeOf(typeof(BITMAPINFOHEADER));
                bi.bmiHeader.biWidth = sw;
                bi.bmiHeader.biHeight = -sh;
                bi.bmiHeader.biPlanes = 1;
                bi.bmiHeader.biBitCount = 32;
                bi.bmiHeader.biCompression = 0;

                srcbmp = CreateDIBSection(sdc, ref bi, 0, out srcbits, IntPtr.Zero, 0);
                rotbmp = CreateDIBSection(sdc, ref bi, 0, out rotbits, IntPtr.Zero, 0);
                outbmp = CreateDIBSection(sdc, ref bi, 0, out outbits, IntPtr.Zero, 0);

                SelectObject(srcdc, srcbmp);
                SelectObject(rotdc, rotbmp);
                SelectObject(outdc, outbmp);

                BitBlt(srcdc, 0, 0, sw, sh, sdc, 0, 0, SRCCOPY);
                BitBlt(rotdc, 0, 0, sw, sh, srcdc, 0, 0, SRCCOPY);
                BitBlt(outdc, 0, 0, sw, sh, rotdc, 0, 0, SRCCOPY);

                new Thread(HSLThread).Start();

                while (DateTime.Now - startTime < maxDuration)
                {
                    double limit = ((new Random().Next(2000) + 1) / 100.0);
                    a += dir * 0.1;
                    if (a >= limit) { a = 0; dir = -1; }
                    if (a <= -limit) { a = 0; dir = 1; }

                    rot();
                    BitBlt(outdc, 0, 0, sw, sh, rotdc, 0, 0, SRCCOPY);
                    Thread.Sleep(1);
                }
            }
        }
    }
}