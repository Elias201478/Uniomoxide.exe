using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace Uniomoxide
{


    internal class GDI3
    {
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct BITMAPINFOHEADER
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

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct RGBQUAD
        {
            public byte rgbBlue;
            public byte rgbGreen;
            public byte rgbRed;
            public byte rgbReserved;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct BITMAPINFO
        {
            public BITMAPINFOHEADER bmiHeader;
            public RGBQUAD bmiColors;
        }

        const int SM_CXSCREEN = 0;
        const int SM_CYSCREEN = 1;
        const uint BI_RGB = 0;
        const int SRCCOPY = 0x00CC0020;

        [DllImport("user32.dll")]
        static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport("user32.dll")]
        static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

        [DllImport("user32.dll")]
        static extern int GetSystemMetrics(int nIndex);

        [DllImport("gdi32.dll")]
        static extern IntPtr CreateCompatibleDC(IntPtr hdc);

        [DllImport("gdi32.dll")]
        static extern bool DeleteDC(IntPtr hdc);

        [DllImport("gdi32.dll")]
        static extern IntPtr CreateDIBSection(IntPtr hdc, ref BITMAPINFO pbmi, uint iUsage, out IntPtr ppvBits, IntPtr hSection, uint dwOffset);

        [DllImport("gdi32.dll")]
        static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);

        [DllImport("gdi32.dll")]
        static extern bool DeleteObject(IntPtr hObject);

        [DllImport("gdi32.dll")]
        static extern bool BitBlt(IntPtr hdcDest, int nXDest, int nYDest, int nWidth, int nHeight, IntPtr hdcSrc, int nXSrc, int nYSrc, int dwRop);

        [DllImport("user32.dll")]
        static extern short GetAsyncKeyState(int vKey);

        static volatile int PixelStart = 2;
        static volatile bool Running = true;

        static void PixelThread()
        {
            const int maxBlock = 256;
            const int intervalMs = 800;
            while (Running)
            {
                int v = PixelStart;
                v += 2;
                if (v > maxBlock) v = 2;
                PixelStart = v;
                Thread.Sleep(intervalMs);
            }
        }

        static void PixelateBuffer(uint[] px, int w, int h, int block)
        {
            if (px == null || block <= 1) return;
            for (int y = 0; y < h; y += block)
            {
                for (int x = 0; x < w; x += block)
                {
                    uint r = 0, g = 0, b = 0;
                    int count = 0;
                    int yyMax = Math.Min(block, h - y);
                    int xxMax = Math.Min(block, w - x);
                    for (int yy = 0; yy < yyMax; ++yy)
                    {
                        int baseIdx = (y + yy) * w + x;
                        for (int xx = 0; xx < xxMax; ++xx)
                        {
                            uint p = px[baseIdx + xx];
                            b += (p >> 0) & 0xFF;
                            g += (p >> 8) & 0xFF;
                            r += (p >> 16) & 0xFF;
                            ++count;
                        }
                    }
                    if (count == 0) continue;
                    byte rb = (byte)(r / count);
                    byte gb = (byte)(g / count);
                    byte bb = (byte)(b / count);
                    uint color = ((uint)rb << 16) | ((uint)gb << 8) | (uint)bb;
                    for (int yy = 0; yy < yyMax; ++yy)
                    {
                        int baseIdx = (y + yy) * w + x;
                        for (int xx = 0; xx < xxMax; ++xx)
                        {
                            px[baseIdx + xx] = color;
                        }
                    }
                }
            }
        }

        public static void Init()
        {
            IntPtr screen = GetDC(IntPtr.Zero);
            if (screen == IntPtr.Zero) return;
            int w = GetSystemMetrics(SM_CXSCREEN);
        }
    }
}