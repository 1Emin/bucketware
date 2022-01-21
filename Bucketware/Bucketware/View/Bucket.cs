using Bucketware.Utils;
using EasyHook;
using GrowbrewProxy;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using Color = System.Drawing.Color;
using Memory;
using Timer = System.Windows.Forms.Timer;
using Guna.UI2.WinForms;
using Bucketware.PluginSystem;
using ENet.Managed;

namespace Bucketware.View
{
    public partial class Bucket : Form
    {
        #region variables
        //public PluginHandler ph = new PluginHandler();
        public HandleMessages messageHandler = new HandleMessages();
        public PacketSending packetSender = new PacketSending();
        public static bool visible = true;
        public static string SpamText = "`bBUCKETWARE";
        public bool CheatNick = false;
        public byte[] nickColor = new byte[4];
        public static bool locked = false;
        public static int FarmItemID = 8;
        public static bool IsAttached = false;
        public static bool DiscordRPC = true;
        public static bool modfly = false;
        public static int doortry = 0;
        public static int spunwheel;
        public static bool isSmart = false;
        public static int TimerInter = 3600;
        public static bool isSpam = false;
        Mem m = new Mem();
        //Timer t1 = new Timer();
        public static bool isGT = true;
        public string ProcessName = "Growtopia";

        public static string GetActiveWindowTitle()
        {
            const int nChars = 256;
            StringBuilder Buff = new StringBuilder(nChars);
            IntPtr handle = Imports.GetForegroundWindow();

            if (Imports.GetWindowText(handle, Buff, nChars) > 0)
            {
                return Buff.ToString();
            }
            return null;
        }

        private static Random random = new Random();
        public static string RandomString(int length, bool onlynumbers, bool onlyletters)
        {
            string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            if (onlynumbers is true)
            {
                chars = "0123456789";
            }
            if (onlyletters is true)
            {
                chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            }
            if (onlyletters && onlynumbers is false)
            {
                chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            }
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        #endregion variables
        public Bucket()
        {
            InitializeComponent();
            ProxyHost();
            CheckForIllegalCrossThreadCalls = false;
        }

        private void Bucket_Load(object sender, System.EventArgs e)
        {
           guna2ComboBox1.StartIndex = 0;
            this.TopMost = true;
            this.BringToFront();
            GrowbrewProxy.Program.LoadProxy();
            timer4.Start();
            Thread.Sleep(400);
            ShowServers();
            GrowbrewProxy.Program.Start();
            Attach.RunWorkerAsync();
            //LoadOverlay(); Disabled for high cpu usage
            CheckGT.Start();
            CheckKeys.Start();
            GetWindow.Start();
            CheckThings.Start();
            
        }
        #region MenuButtons
        private void guna2Button1_Click(object sender, System.EventArgs e)
        {
            guna2Button1.Checked = true;
            guna2Button2.Checked = false;
            guna2Button3.Checked = false;
            guna2Button4.Checked = false;
            guna2Button5.Checked = false;
            guna2Button6.Checked = false;
            tabControl1.SelectedIndex = 0;
        }

        private void guna2Button2_Click(object sender, System.EventArgs e)
        {
            guna2Button1.Checked = false;
            guna2Button2.Checked = true;
            guna2Button3.Checked = false;
            guna2Button4.Checked = false;
            guna2Button5.Checked = false;
            guna2Button6.Checked = false;
            tabControl1.SelectedIndex = 1;
        }

        private void guna2Button3_Click(object sender, System.EventArgs e)
        {
            guna2Button1.Checked = false;
            guna2Button2.Checked = false;
            guna2Button3.Checked = true;
            guna2Button4.Checked = false;
            guna2Button5.Checked = false;
            guna2Button6.Checked = false;
            tabControl1.SelectedIndex = 2;
        }

        private void guna2Button4_Click(object sender, System.EventArgs e)
        {
            guna2Button1.Checked = false;
            guna2Button2.Checked = false;
            guna2Button3.Checked = false;
            guna2Button4.Checked = true;
            guna2Button5.Checked = false;
            guna2Button6.Checked = false;
            tabControl1.SelectedIndex = 3;
        }

        private void guna2Button5_Click(object sender, System.EventArgs e)
        {
            guna2Button1.Checked = false;
            guna2Button2.Checked = false;
            guna2Button3.Checked = false;
            guna2Button4.Checked = false;
            guna2Button5.Checked = true;
            guna2Button6.Checked = false;
            tabControl1.SelectedIndex = 4;
        }

        private void guna2Button6_Click(object sender, System.EventArgs e)
        {
            guna2Button1.Checked = false;
            guna2Button2.Checked = false;
            guna2Button3.Checked = false;
            guna2Button4.Checked = false;
            guna2Button5.Checked = false;
            guna2Button6.Checked = true;
            tabControl1.SelectedIndex = 5;
        }
        #endregion

        private void guna2TrackBar1_Scroll(object sender, ScrollEventArgs e)
        {
            RangeText.Text = guna2TrackBar1.Value.ToString();
            int range = (int)guna2TrackBar1.Value;
            GrowbrewProxy.Program.globalUserData.auto_range = range;
        }
        public static bool oneblock = false;
        private int val = 2;
        private void guna2TrackBar2_Scroll(object sender, ScrollEventArgs e)
        {
            val = (int)guna2TrackBar2.Value;
            FarmRangeText.Text = val.ToString();
            if (val == 0)
            {
                oneblock = false;
                GrowbrewProxy.Program.globalUserData.rok = 1;
                GrowbrewProxy.Program.globalUserData.irok1 = 0;
                GrowbrewProxy.Program.globalUserData.irok2 = 0;
                GrowbrewProxy.Program.globalUserData.xrok1 = 0;
                GrowbrewProxy.Program.globalUserData.xrok2 = 0;

            }
            else if (val == 1)
            {
                oneblock = true;
                GrowbrewProxy.Program.globalUserData.rok = 2;
                GrowbrewProxy.Program.globalUserData.irok1 = 0;
                GrowbrewProxy.Program.globalUserData.irok2 = 1;
                GrowbrewProxy.Program.globalUserData.xrok1 = 1;
                GrowbrewProxy.Program.globalUserData.xrok2 = 2;
            }
            else if (val == 2)
            {
                oneblock = false;
                //startFromOwnTilePos = true;
                GrowbrewProxy.Program.globalUserData.rok = 2;
                GrowbrewProxy.Program.globalUserData.irok1 = 0;
                GrowbrewProxy.Program.globalUserData.irok2 = 1;
                GrowbrewProxy.Program.globalUserData.xrok1 = 1;
                GrowbrewProxy.Program.globalUserData.xrok2 = 2;
            }
            else if (val == 3)
            {
                oneblock = false;
                GrowbrewProxy.Program.globalUserData.rok = 3;
                GrowbrewProxy.Program.globalUserData.irok1 = 0;
                GrowbrewProxy.Program.globalUserData.irok2 = 1;
                GrowbrewProxy.Program.globalUserData.xrok1 = 1;
                GrowbrewProxy.Program.globalUserData.xrok2 = 2;
            }
        }
        int r = 244, g = 65, b = 65;
        int rgbTransitionState = 0;
        int doTransitionRed()
        {
            if (b >= 250)
            {
                r -= 1; // red uses -1 / +1, doing it cuz red is a more dominant color imo

                if (r <= 65)
                {
                    rgbTransitionState = 1;
                }
            }

            if (b <= 65)
            {
                r += 1;

                if (r >= 250)
                {
                    rgbTransitionState = 1;
                }
            }
            return r;
        }

        int doTransitionGreen()
        {
            if (r <= 65)
            {
                g += 2;

                if (g >= 250)
                {
                    rgbTransitionState = 2;
                }
            }

            if (r >= 250)
            {
                g -= 2;

                if (g <= 65)
                {
                    rgbTransitionState = 2;
                }
            }
            return g;
        }
        int doTransitionBlue()
        {
            if (g <= 65)
            {
                b += 2;

                if (b >= 250)
                {
                    rgbTransitionState = 0;
                }
            }

            if (g >= 250)
            {
                b -= 2;

                if (b <= 65)
                {
                    rgbTransitionState = 0;
                }
            }
            return b;
        }
        void doRGBEverything()
        {
            while (true)
            {
                switch (rgbTransitionState)
                {
                    case 0:
                        Invoke(new Action(() =>
                        {
                            _Interface.Colour = Color.FromArgb(doTransitionRed(), g, b);
                        }));
                        break;
                    case 1:
                        Invoke(new Action(() =>
                        {
                            _Interface.Colour = Color.FromArgb(r, doTransitionGreen(), b);
                        }));
                        break;
                    case 2:
                        Invoke(new Action(() =>
                        {
                            _Interface.Colour = Color.FromArgb(r, g, doTransitionBlue());
                        }));
                        break;
                }
                Thread.Sleep(60);
            }
        }
        private string ChannelName;
        private HookInterface _Interface = new HookInterface();

        private void guna2TrackBar3_Scroll(object sender, ScrollEventArgs e)
        {
            WalkSpeedText.Text = guna2TrackBar3.Value.ToString();
            SpeedVal = (int)guna2TrackBar3.Value;
            GrowbrewProxy.Program.SetCustomGS((int)guna2TrackBar3.Value, (int)guna2TrackBar4.Value);
        }

        private void guna2Button7_Click(object sender, EventArgs e)
        {
            TimerInter += 1;
            guna2TextBox3.Text = TimerInter.ToString();
        }

        private void guna2Button16_Click(object sender, EventArgs e)
        {
            Process.Start("https://dsc.gg/sahyui");
        }

        private void guna2Button18_Click(object sender, EventArgs e)
        {
            Process.Start("https://github.com/1Emin");
        }

        private void guna2Button17_Click(object sender, EventArgs e)
        {
            Process.Start("https://dsc.gg/sahyui");
        }
        #region hosts
        private void ProxyHost()
        {
            try
            {
                StreamWriter sws = new StreamWriter(@"C:\Windows\System32\Drivers\etc\hosts");
                sws.Write("127.0.0.1 growtopia1.com" + Environment.NewLine + "127.0.0.1 growtopia2.com");
                sws.Close();
            }
            catch
            {

            }

        }
        private void GrowtopiaHost()
        {
            try
            {
                StreamWriter sws = new StreamWriter(@"C:\Windows\System32\Drivers\etc\hosts");
                sws.Write(" ");
                sws.Close();
            }
            catch
            {

            }

        }
        #endregion hosts
        private void CheckGT_Tick(object sender, EventArgs e)
        {
            Process[] pname = Process.GetProcessesByName("Growtopia");
            if (pname.Length > 0)
            {

            }
            else
            {
                GrowtopiaHost();
                Application.Exit();
                Process[] procs = Process.GetProcessesByName("Bucketware");
                foreach (Process p in procs) { p.Kill(); }
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (isGT is true)
            {
                if (Keyboard.IsKeyToggled(Key.Insert))
                {
                    this.Hide();
                    GetWindow.Stop();
                }
                else
                {
                    this.Show();
                    GetWindow.Start();
                }
            }
        }

        private void GetWindow_Tick(object sender, EventArgs e)
        {
            if (GetActiveWindowTitle() == "Growtopia")
            {
                isGT = true;
                this.Show();
            }
            else
            {
                if (GetActiveWindowTitle() == "Bucketware")
                {
                    return;
                }
                isGT = false;
                this.Hide();
            }
        }

        private void CheckThings_Tick(object sender, EventArgs e)
        {
            ItemDatabase.ItemDefinition itemDef = ItemDatabase.GetItemDef(FarmItemID);
            label37.Text = "Detected Item: " + itemDef.itemName;
            if (modfly is true)
            {
                if (Keyboard.IsKeyDown(Key.S))
                {
                    m.WriteMemory(Address.ModFly, "bytes", "0x74 0x5D");
                }
                else
                {
                    m.WriteMemory(Address.ModFly, "bytes", "0x90 0x90");
                }
            }
        }

        private void SmartHandler_Tick(object sender, EventArgs e)
        {
            if (guna2TextBox2.Text.Length < 10)
            {
                guna2TextBox3.Text = "1600";
                TimerInter = 1600;
            }
            if (guna2TextBox2.Text.Length < 21)
            {
                guna2TextBox3.Text = "3500";
                TimerInter = 3600;
            }
            else if (guna2TextBox2.Text.Length < 41)
            {
                guna2TextBox3.Text = "6000";
                TimerInter = 6000;
            }
            else if (guna2TextBox2.Text.Length < 71)
            {
                guna2TextBox3.Text = "9600";
                TimerInter = 9600;
            }
        }

        private void guna2Button8_Click(object sender, EventArgs e)
        {
            TimerInter -= 1;
            guna2TextBox3.Text = TimerInter.ToString();
        }

        private void guna2CustomCheckBox16_Click(object sender, EventArgs e)
        {
            isSmart = !isSmart;
            if (guna2CustomCheckBox16.Checked)
                SmartHandler.Start();
            else
                SmartHandler.Stop();
        }

        private void guna2CustomCheckBox26_Click(object sender, EventArgs e)
        {
            if (guna2CustomCheckBox26.Checked)
            {
                isSpam = true;
                if (guna2ComboBox1.SelectedIndex is 0)
                {
                    Task.Run(() => GrowbrewProxy.Program.Spammer(guna2TextBox2.Text));
                    GrowbrewProxy.Program.Spam = true;
                }
                else if (guna2ComboBox1.SelectedIndex is 1)
                {
                    Task.Run(() => GrowbrewProxy.Program.EmoteSpammer());
                    GrowbrewProxy.Program.Emote = true;
                }
            }
            else
            {
                isSpam = false;
                GrowbrewProxy.Program.Spam = false;
                GrowbrewProxy.Program.Emote = false;
            }
        }

        private void guna2TextBox3_TextChanged(object sender, EventArgs e)
        {
            TimerInter = int.Parse(guna2TextBox3.Text);
        }
        
        private void guna2CustomCheckBox1_Click(object sender, EventArgs e)
        {
            if (guna2CustomCheckBox1.Checked)
            {
                GrowbrewProxy.Program.globalUserData.isAutofarming = true;
                Task.Run(() => GrowbrewProxy.Program.doAutofarm(FarmItemID, GrowbrewProxy.Program.globalUserData.cheat_Autofarm_magplant_mode, false));
            }
            else
            {
                GrowbrewProxy.Program.globalUserData.isAutofarming = false;
            }
        }

        private void guna2CustomCheckBox3_Click(object sender, EventArgs e)
        {
            GrowbrewProxy.Program.globalUserData.cheat_Autofarm_magplant_mode = !GrowbrewProxy.Program.globalUserData.cheat_Autofarm_magplant_mode;
        }

        private void guna2CustomCheckBox2_Click(object sender, EventArgs e)
        {
            GrowbrewProxy.Program.globalUserData.cheat_autocollect = !GrowbrewProxy.Program.globalUserData.cheat_autocollect;
        }
        #region cheataddress
        public struct Addresses
        {
            public string AntiSlide;
            public string AntiBounce;
            public string ModFly;
            public string AntiLava;
            public string AntiCheckPoint;
            public string AntiPunch;
            public string AntiRespawn;
            public string AntiSpike;
            public string FastFall;
            public string ForceFps;
            public string NightVision;
            public string Slide;
            public string Tractor;
            public string NoClip;
            public string Gravity;
            public string HighJump;
            public string Ghost;
            public string Growz;
            public string FastInWater;
            public string FastDrop;
            public string FastPickup;
            public string AntiState;
            public string SlowMo;
            public string AutoJump;
            public string FrogMode;
            public string Ceme;
            public string AutoPunch;
            public string WaterVisual;
            public string SmallJump;
            public string DevMode;
            public string AntiKnockBack;
            public string MoveWHileDead;
            public string airPin;
            public string moonWalk1;
            public string moonWalk2;
            public string moonWalk3;
            public string moon;
            public string moon2;
            public string antione;
            public string antione2;
            public string Bypass;
        }
        public static Addresses Address = new Addresses();
        public async void LoadAddresses()
        {
            IsAttached = true;
            var PBP = await AoBScan("0F 85 18 01 00 00 C7 86 ?? ?? 00 00 01 00 00 00 48 8B CE E8 ?? ?? FF FF 84 C0 0F 84 FE 00 00 00 F3 0F 10 46 18 F3 0F 11 44 24 50 F3 0F 10 4E 1C F3 0F 11 4C 24 54 F3 0F 10 46 08 F3 0F 11 44 24 30 F3 0F 10 4E 0C F3 0F 11 4C 24 34 E8 ?? ?? ?? FF 41 B9 01 00 00 00 E9 AB 00 00 00 44 38 BE ?? ?? 00 00");
            var AIC = await AoBScan("76 08 32 C0 48 83 C4 30 5B C3 F3 0F 10 05 ?? ?? ?? 00 F3 0F 59 05 ?? ?? ?? 00 0F 29 74 24 20 F3 0F 10 35 ?? ?? ?? 00 F3 0F 5C F0");
            var BBP = await AoBScan("75 08 85 C9 0F 85 8B 00 00 00 C7 05 ?? ?? ?? 00 01 00 00 00 48 8B 8B ?? 01 00 00 48 85 C9 74 75 E8 ?? ?? ?? 00 84 C0 74 6C 48 C7 44 24 40 00 00 00 00 48 C7 44 24 30 00 00 00 00");

            m.WriteMemory(PBP, "bytes", "0x90 0x90 0x90 0x90 0x90");
            m.WriteMemory(AIC, "bytes", "0x90 0x90");
            m.WriteMemory(BBP, "bytes", "0x90 0x90");
            Address.Bypass = await AoBScan("75 08 85 C9 0F 85 8B 00 00 00");
            Address.AntiSlide = await AoBScan("75 03 45 8B FC 48 8B 45 07 44 88 BF ?? ?? 00 00 0F B7 58 04 E8 ?? ?? ?? FF 8B 8F ?? ?? 00 00 4C 8B BC 24 A0 00 00 00 4C 8B B4 24 E0 00 00 00 48 8B 40 08 48 8B B4 24 D8 00 00 00 48 69 D3 ?? ?? 00 00");
            Address.AntiBounce = await AoBScan("0F 28 FA F3 0F 58 BC 24 ?0 00 00 00 0F 28 C7 F3 0F 58 C6 F3 0F 11 44 24 ?C 0F 57 C9 F3 0F 7F 48 80 48 C7 40 90 00 00 00 00 4C 8D 40 80 48 8D 54 24 ?0");
            Address.ModFly = await AoBScan("74 5D 8B 4F 14 83 F9 07 75 05 40 84 ED 75 50 0F B6 47 0A 66 0F 6E D0 0F 5B D2");
            Address.AntiLava = await AoBScan("75 07 83 8B ?? ?? 00 00 40 48 8D 8B ?? ?? 00 00 33 D2 48 83 C4 40 5B");
            Address.AntiCheckPoint = await AoBScan("83 7C 02 04 1B 0F 94 C0 48 83 C4 20 5B C3");
            Address.AntiPunch = await AoBScan("0F 85 C0 00 00 00 E8 ?? ?? ?? FF 48 8B C8 E8 ?? ?? ?? FF 48 8B C8 48 8D 55 C7 E8 ?? ?? 00 00 48 8B D0 49 8B CE E8 ?? ?? ?? 00 84 C0");
            Address.AntiRespawn = await AoBScan("80 7B 24 00 75 10 8B 43 28 01 43 14 83 C0 03 01 43 58 83 43 18 03 66 0F 6E 43 28 0F 5B C0");
            Address.AntiSpike = await AoBScan("0F 85 67 05 00 00 48 85 ?? ?? 32 48 8B CA E8 ?? ?? 04 00 B9 FE 18 00 00 66 3B C1");
            Address.FastFall = await AoBScan("0F 84 ?? 00 00 00 F3 0F 10 81 ?? 01 00 00 F3 0F 10 89 ?? ?? 00 00 F3 41 0F 59 C0 F3 0F 5C C8 41 0F 2F CA 76 2A F3 0F 10 89 ?? 01 00 00 0F 28 C1 F3 41 0F 58 CA F3 41 0F 59 C0 F3 41 0F 58 C2 F3 0F 11 89 ?? ?? 00 00 F3 0F 11 81 ?? ?? 00 00 ?? ?? ?? ?? ?? ?? 00");
            Address.ForceFps = await AoBScan("0F 84 ?? 01 00 00 E8 ?? ?? ?? 00 48 8B D8 8B BE ?? 02 00 00 E8 ?? ?? ?? 00 C1 E8 0A 8B C8 0F 57 C0 F3 48 0F 2A C1 F3 0F 10 35 ?? ?? ?? 00 F3 0F 59 C6 0F 5A F8");
            Address.NightVision = await AoBScan("75 06 48 83 C4 20 5B C3 B8 F8 0D 00 00 66 39 43 12 75 67 48 8B 4B 20 48 8B 01 48 3B C1");
            Address.Slide = await AoBScan("75 0E B8 2A 15 00 00 66 39 87 ?? ?? 00 00 75 03 45 8B FC");
            Address.Tractor = await AoBScan("0F 85 ?? 00 00 00 49 8D 8E ?? ?? 00 00 E8 ?? ?? ?? FF 84 C0 0F 84 ?? 00 00 00 48 83 7E 28 00 0F 84 ?? 00 00 00 48 8B CE E8 ?? ?? 04 00 0F B7 D8");
            Address.NoClip = await AoBScan("75 0C 32 C0 48 83 C4 20 41 5F 41 5E 5F C3 45 84 C9 74 0D 8B 41 14 83 F8 02 74 E7 83 F8 07 74 E2 8B 41 14 83 F8 05");
            Address.Gravity = await AoBScan("0F 84 ?? 01 00 00 80 BB ?? ?? 00 00 00 F3 0F 10 05 ?? ?? ?? 00 F3");
            Address.HighJump = await AoBScan("75 1D F3 0F 10 0D ?? ?? ?? 00 F3 0F 10 05 ?? ?? ?? 00 F3 41 0F 59 C8 F3 0F 5C C1 F3 0F 59 F8 80 BB A1 01 00 00 00 0F 84 ?? 01 00 00 80 BB ?? ?? 00 00 00");
            Address.Ghost = await AoBScan("74 05 E8 ?? ?? FF FF 48 8B CE E8 ?? ?? ?? ?? 48 83 BE 20 01 00 00 00 75 08 48 8B CE E8 ?? ?? 00 00 4C 8D 9C 24 28 01 00 00 41 0F 28 73 E8");
            Address.Growz = await AoBScan("F3 0F 5C D1 0F 28 F2 F3 44 0F 58 C2 F3 41 0F 58 F3 74 22 F6 87 88 01 00 00 01 75 19 F3 0F 10 87 F4 01 00 00 45 0F 28 C4 0F 28 F0 F3 44 0F 58 C0 F3 41 0F 58 F3 80 BF DE 00 00 00 00");
            Address.FastInWater = await AoBScan("F6 87 88 01 00 00 01 75 19 F3 0F 10 87 F4 01 00 00 45 0F 28 C4 0F 28 F0 F3 44 0F 58 C0 F3 41 0F 58 F3 80 BF DE 00 00 00 00 74 27");
            Address.FastDrop = await AoBScan("E8 ?? ?? ?? FF 89 43 28 48 8D 43 10 48 8B 5C 24 70 48 8B 6C 24 78 48 8B B4 24 80 00 00 00 48 83 C4 50 41 5F 41 5E 5F C3");
            Address.FastPickup = await AoBScan("73 19 80 63 13 FE C7 43 1C 00 00 00 00 F3 0F 10 05 ?? ?? ?? 00 48 83 C4 30 5B C3 F6 43 13 01");
            Address.AntiState = await AoBScan("0F 84 ?? 16 00 00 48 8B D6 48 8B C8 E8 ?? ?? 0F 00 E9 ?? 16 00 00 E8 ?? ?? ?? FF 48 8D 88 60 01 00 00 8B 56 04 E8 ?? ?? 10 00 48 8B D8 48 85 C0");
            Address.SlowMo = await AoBScan("72 A7 48 8B AC 24 88 00 00 00 4C 8B 74 24 60 0F 57 C0 48 8B 7C 24 68 48 85 F6 F3 48 0F 2A C6 48 8B B4 24 90 00 00 00");
            Address.AutoJump = await AoBScan("74 09 80 BB A9 01 00 00 00 74 2B 80 BB 30 01 00 00 00 0F 85 ?? 02 00 00 F3 0F 10 83 C8 00 00 00 0F 2F C6 0F 86 ?? 02 00 00 80 BB A9 01 00 00 00");
            Address.FrogMode = await AoBScan("0F 84 EF 00 00 00 80 BE 30 01 00 00 00 75 3C 80 BE A0 01 00 00 00 75 3C F3 0F 10 86 00 01 00 00 F3 41 0F 59 C0 F3 0F 10 96 F4 00 00 00 F3 0F 5C D0 F3 41 0F 59 D4 44 8B 8E 3C 01 00 00");
            Address.Ceme = await AoBScan("0F B7 47 04 44 0F B6 77 30 89 5F 40 66 85 C0 75 04 0F B7 47 06 0F B7 D8");
            Address.AutoPunch = await AoBScan("74 5F F3 0F 10 88 ?? ?? 00 00 0F 28 C1 F3 41 0F 58 C8 F3 0F 59 C7 F3 0F 11 88 ?? ?? 00 00 F3 41 0F 58 C0 F3 0F 11 80 ?? ?? 00 00 48 8B 43 08");
            Address.WaterVisual = await AoBScan("74 1E 45 0F B6 46 0B 48 8D 8E 58 03 00 00 41 0F B6 56 0A 41 B1 01 C6 44 24 20 00 E8 ?? ?? ?? FF C6 86 A1 01 00 00 00");
            Address.SmallJump = await AoBScan("75 12 B8 2C 15 00 00 66 39 83 CC 02 00 00 0F 85 C1 00 00 00 80 BB DE 00 00 00 00 0F 85 B4 00 00 00 48 85 F6");
            Address.DevMode = await AoBScan("74 5F E8 ?? ?? ?? FF 48 69 D6 ?? 06 00 00 48 8B 40 08 83 7C 02 04 00 75 25 41 83 C9 FF");
            Address.AntiKnockBack = await AoBScan("48 8B D9 F3 0F 10 49 0C 49 8B F8 F3 0F 5C 02 F3 0F 5C 4A 04 48 8D 4C 24 40 0F 29 74 24 20 F3 0F 11 44 24 40 F3 0F 11 4C 24 44");
            Address.MoveWHileDead = await AoBScan("74 7D 48 89 5C 24 10");
            Address.airPin = await AoBScan("75 18 4D 8B CE");
            Address.moonWalk1 = await AoBScan("F3 0F 11 11 0F 28 D7");
            Address.moonWalk2 = await AoBScan("F3 0F 11 53 20");
            Address.moonWalk3 = await AoBScan("F3 0F 11 41 04 E8");
            Address.moon = await AoBScan("74 0D F3 0F 59 D7");
            Address.moon2 = await AoBScan("F3 0F 59 D7 F3 0F 5C DA");
            Address.antione = await AoBScan("79 0F 0F B6 45 08");
            Address.antione2 = await AoBScan("45 85 F6 0F 84 C9 00 00 00");
            return;
        }
        
        public async Task<string> AoBScan(string Aob)
        {
            IEnumerable<long> AoBScanResults = await m.AoBScan(Aob);
            long SingleAoBScanResult = AoBScanResults.FirstOrDefault();
            return SingleAoBScanResult.ToString("X");
        }
        #endregion
        private void Attach_DoWork(object sender, DoWorkEventArgs e)
        {
            while (true)
            {
                int pID = m.GetProcIdFromName("Growtopia");
                bool openProc = false;
                if (pID > 0) openProc = m.OpenProcess(pID);
                if (openProc) if (IsAttached == false) LoadAddresses();
                    else if (pID == 0) IsAttached = false;
                Thread.Sleep(50);
            }
        }
        public void WriteMemory(string address, string method, string val)
        {
            m.WriteMemory(address, method, val);
        }
        private void guna2Button9_Click(object sender, EventArgs e)
        {
            /*File.ReadLines(Application.StartupPath + "/itemsid.txt")
                .TakeWhile(line => !line.Contains(guna2TextBox11.Text));*/
            foreach (var line in File.ReadLines(Application.StartupPath + "/itemsid.txt"))
            {
                if (line.Contains(guna2TextBox11.Text))
                {
                    try
                    {
                        var dog = line.Replace(guna2TextBox11.Text, "");
                        string eo = dog.Trim();
                        //eo.Replace(" Seed", "");
                        int ok = int.Parse(eo);
                        FarmItemID = ok;
                    }
                    catch
                    {

                    }
                }
            }

        }

        private void guna2CustomCheckBox8_Click(object sender, EventArgs e)
        {
            GrowbrewProxy.Program.globalUserData.unlimitedZoom = !GrowbrewProxy.Program.globalUserData.unlimitedZoom;
        }

        private void guna2CustomCheckBox7_Click(object sender, EventArgs e)
        {
            if (guna2CustomCheckBox7.Checked)
            {
                m.WriteMemory(Address.AntiBounce, "bytes", "90 90 90");
            }
            else
            {
                m.WriteMemory(Address.AntiBounce, "bytes", "0F 28 FA");
            }
        }

        private void guna2CustomCheckBox6_Click(object sender, EventArgs e)
        {
            if (guna2CustomCheckBox6.Checked)
            {
                modfly = true;
                m.WriteMemory(Address.ModFly, "bytes", "0x90 0x90");
                m.WriteMemory(Address.moonWalk1, "bytes", "90 90 90 90");
                m.WriteMemory(Address.moonWalk2, "bytes", "90 90 90 90 90");
                m.WriteMemory(Address.moonWalk3, "bytes", "90 90 90 90 90");
            }
            else
            {
                modfly = false;
                m.WriteMemory(Address.ModFly, "bytes", "0x74 0x5D");
                m.WriteMemory(Address.moonWalk1, "bytes", "F3 0F 11 11");
                m.WriteMemory(Address.moonWalk2, "bytes", "F3 0F 11 53 20");
                m.WriteMemory(Address.moonWalk3, "bytes", "F3 0F 11 41 04");
            }
        }

        private void guna2CustomCheckBox5_Click(object sender, EventArgs e)
        {
            if (guna2CustomCheckBox5.Checked)
            {
                m.WriteMemory(Address.NoClip, "bytes", "90 90");
            }
            else
            {
                m.WriteMemory(Address.NoClip, "bytes", "75 0C");
            }
        }

        private void guna2CustomCheckBox9_Click(object sender, EventArgs e)
        {
            GrowbrewProxy.Program.globalUserData.cheat_magplant = !GrowbrewProxy.Program.globalUserData.cheat_magplant;
        }

        private void guna2CustomCheckBox10_Click(object sender, EventArgs e)
        {
            if (guna2CustomCheckBox10.Checked)
            {
                m.WriteMemory(Address.AntiSlide, "bytes", "90 90");
            }
            else
            {
                m.WriteMemory(Address.AntiSlide, "bytes", "75 03");
            }
        }

        private void guna2CustomCheckBox11_Click(object sender, EventArgs e)
        {
            if (guna2CustomCheckBox11.Checked)
            {
                m.WriteMemory(Address.Growz, "bytes", "90 90 90 90");
                m.WriteMemory(Address.FastInWater, "bytes", "90 90 90 90 90 90 90");
                m.WriteMemory(Address.Gravity, "bytes", "90 90 90 90 90 90");
                m.WriteMemory(Address.AntiBounce, "bytes", "90 90 90");
                m.WriteMemory(Address.AntiSlide, "bytes", "90 90");
                m.WriteMemory(Address.NightVision, "bytes", "90 90");
            }
            else
            {
                m.WriteMemory(Address.AntiBounce, "bytes", "0F 28 FA");
                m.WriteMemory(Address.Gravity, "bytes", "0F 84 17 01 00 00");
                m.WriteMemory(Address.Growz, "bytes", "F3 0F 5C D1");
                m.WriteMemory(Address.AntiSlide, "bytes", "75 03");
                m.WriteMemory(Address.NightVision, "bytes", "75 06");
                m.WriteMemory(Address.FastInWater, "bytes", "F6 87 88 01 00 00 01");
            }
        }

        private void guna2CustomCheckBox12_Click(object sender, EventArgs e)
        {
            if (guna2CustomCheckBox12.Checked)
            {
                m.WriteMemory(Address.Growz, "bytes", "90 90 90 90");
            }
            else
            {
                m.WriteMemory(Address.Growz, "bytes", "F3 0F 5C D1");
            }
        }

        private void guna2CustomCheckBox13_Click(object sender, EventArgs e)
        {
            if (guna2CustomCheckBox13.Checked)
            {
                m.WriteMemory(Address.AntiSpike, "bytes", "0F 84 67 05 00 00");
            }
            else
            {
                m.WriteMemory(Address.AntiSpike, "bytes", "0F 85 67 05 00 00");
            }
        }

        private void guna2CustomCheckBox14_Click(object sender, EventArgs e)
        {
            if (guna2CustomCheckBox14.Checked)
            {
                modfly = true;
                m.WriteMemory(Address.NoClip, "bytes", "90 90");
                m.WriteMemory(Address.ModFly, "bytes", "90 90");
                m.WriteMemory(Address.Ghost, "bytes", "73 05");
            }
            else
            {
                modfly = false;
                m.WriteMemory(Address.NoClip, "bytes", "75 0C");
                m.WriteMemory(Address.ModFly, "bytes", "0x74 0x5D");
                m.WriteMemory(Address.Ghost, "bytes", "74 05");
            }
        }

        private void guna2CustomCheckBox4_Click(object sender, EventArgs e)
        {
            if (guna2CustomCheckBox4.Checked)
            {
                m.WriteMemory(Address.AntiCheckPoint, "bytes", "90 90 90 90 90");
            }
            else
            {
                m.WriteMemory(Address.AntiCheckPoint, "bytes", "83 7C 02 04 1B");
            }
        }

        private void guna2CustomCheckBox24_Click(object sender, EventArgs e)
        {
            HandleMessages.SkinChanger = !HandleMessages.SkinChanger;
        }

        private void guna2CustomCheckBox23_Click(object sender, EventArgs e)
        {
            if (guna2CustomCheckBox23.Checked)
            {
                m.WriteMemory(Address.FrogMode, "bytes", "90 90 90 90 90 90");
            }
            else
            {
                m.WriteMemory(Address.FrogMode, "bytes", "0F 84 EF 00 00 00");
            }
        }

        private void guna2CustomCheckBox22_Click(object sender, EventArgs e)
        {
            if (guna2CustomCheckBox22.Checked)
            {
                m.WriteMemory(Address.AntiRespawn, "bytes", "90 90 90 90");
                m.WriteMemory(Address.FrogMode, "bytes", "90 90 90 90 90 90");
            }
            else
            {
                m.WriteMemory(Address.AntiRespawn, "bytes", "80 7B 24 00");
                m.WriteMemory(Address.FrogMode, "bytes", "0F 84 EF 00 00 00");
            }
        }

        private void guna2CustomCheckBox21_Click(object sender, EventArgs e)
        {
            if (guna2CustomCheckBox21.Checked)
            {
                m.WriteMemory(Address.AntiKnockBack, "bytes", "90 90 90");
            }
            else
            {
                m.WriteMemory(Address.AntiKnockBack, "bytes", "48 8B D9");
            }
        }

        private void guna2CustomCheckBox20_Click(object sender, EventArgs e)
        {
            GrowbrewProxy.Program.TradeAll = !GrowbrewProxy.Program.TradeAll;
        }

        private void guna2CustomCheckBox17_Click(object sender, EventArgs e)
        {
            GrowbrewProxy.Program.globalUserData.cheat_leavemod = !GrowbrewProxy.Program.globalUserData.cheat_leavemod;
        }
        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        const int SW_HIDE = 0;
        const int SW_SHOW = 5;
        private void guna2CustomCheckBox18_Click(object sender, EventArgs e)
        {
            if (guna2CustomCheckBox28.Checked is true)
            {
                var handle = GetConsoleWindow();
                ShowWindow(handle, SW_HIDE);
            }
            else
            {
                var handle = GetConsoleWindow();
                ShowWindow(handle, SW_SHOW);
            }
        }

        private void guna2CustomCheckBox19_Click(object sender, EventArgs e)
        {
            GrowbrewProxy.Program.globalUserData.cheat_Banothers = !GrowbrewProxy.Program.globalUserData.cheat_Banothers;
        }

        private void guna2CustomCheckBox25_Click(object sender, EventArgs e)
{
            if (guna2CustomCheckBox25.Checked)
            {
                GrowbrewProxy.Program.globalUserData.cheat_casinohack = true;
            }
            else
            {
                GrowbrewProxy.Program.globalUserData.cheat_casinohack = false;
            }
        }
        int csnNumber = 0;
        private void guna2TextBox4_TextChanged(object sender, EventArgs e)
        {
            csnNumber = int.Parse(guna2TextBox4.Text);
            if (csnNumber > 36)
            {
                guna2TextBox4.Text = "36";
                csnNumber = 36;
            }
            spunwheel = csnNumber;
        }

        private void guna2Button11_Click(object sender, EventArgs e)
        {
            csnNumber += 1;
            guna2TextBox4.Text = csnNumber.ToString();
        }

        private void guna2Button10_Click(object sender, EventArgs e)
        {
            csnNumber -= 1;
            guna2TextBox4.Text = csnNumber.ToString();
        }
        private int tileX;
        private int tileY;
        private void guna2CustomCheckBox27_Click(object sender, EventArgs e)
        {
            tileX = GrowbrewProxy.Program.GetTileX();
            tileY = GrowbrewProxy.Program.GetTileY();
            GrowbrewProxy.Program.globalUserData.cheat_password = !GrowbrewProxy.Program.globalUserData.cheat_password;
            if (guna2CustomCheckBox27.Checked)
            {
                packetSender.SendPacket(3, "action|log\n|msg|`1Starting bruteforcing password door on tile: " + tileX + ", " + tileY, GrowbrewProxy.Program.proxyPeer);
                m.WriteMemory(Address.AutoPunch, "bytes", "90 90");
                timer3.Start();
            }
            else
            {

                doortry = 0;
                guna2TextBox5.Text = "0";
                m.WriteMemory(Address.AutoPunch, "bytes", "74 5F");
                timer3.Stop();
            }
        }

        private void timer3_Tick(object sender, EventArgs e)
        {
            doortry += 1;
            guna2TextBox5.Text = doortry.ToString();
            World worldMap = new World();
            //packetSender.SendPacket(3, "action|log\n|msg|`1Current tile: " + tileX + ", " + tileY, GrowbrewProxy.Program.proxyPeer);
            packetSender.SendPacket(2, "action|dialog_return\ndialog_name|password_reply\ntilex|" + tileX + "|\ntiley|" + tileY + "|\npassword|" + doortry, GrowbrewProxy.Program.realPeer);
        }

        private void guna2CustomCheckBox28_Click(object sender, EventArgs e)
        {
            if (guna2CustomCheckBox28.Checked)
            {
                timer3.Interval = 350;
            }
            else
            {
                timer3.Interval = 115;
            }
        }

        private void guna2TextBox5_TextChanged(object sender, EventArgs e)
        {
            if (guna2TextBox5.Text == string.Empty)
            {
                guna2TextBox5.Text = "0";
            }
            doortry = int.Parse(guna2TextBox5.Text);
        }

        private void guna2Button13_Click(object sender, EventArgs e)
        {
            doortry += 1;
            guna2TextBox5.Text = doortry.ToString();
        }

        private void guna2Button12_Click(object sender, EventArgs e)
        {
            doortry -= 1;
            guna2TextBox5.Text = doortry.ToString();
        }

        private void timer4_Tick(object sender, EventArgs e)
        {
            if (GrowbrewProxy.Program.globalUserData.actived is true)
            {
                if (GrowbrewProxy.Program.globalUserData.cheat_password is false)
                {
                    try
                    {
                        guna2CustomCheckBox27.Checked = false;
                        doortry = 0;
                        guna2TextBox5.Text = "0";
                        m.WriteMemory(Address.AutoPunch, "bytes", "74 5F");
                        timer3.Stop();
                        GrowbrewProxy.Program.globalUserData.actived = false;
                    }
                    catch
                    {

                    }

                }
            }
        }

        private void guna2CustomCheckBox29_Click(object sender, EventArgs e)
        {
            GrowbrewProxy.Program.globalUserData.autojoin = !GrowbrewProxy.Program.globalUserData.autojoin;
        }

        private void guna2TextBox6_TextChanged(object sender, EventArgs e)
        {
            GrowbrewProxy.Program.globalUserData.autojoinworld = guna2TextBox6.Text;
        }

        private void guna2CustomCheckBox30_Click(object sender, EventArgs e)
        {
            GrowbrewProxy.Program.globalUserData.cheat_msgnew = !GrowbrewProxy.Program.globalUserData.cheat_msgnew;
        }

        private void guna2TextBox7_TextChanged(object sender, EventArgs e)
        {
            GrowbrewProxy.Program.globalUserData.cheat_msgnew_text = guna2TextBox7.Text;
        }

        private void guna2Button14_Click(object sender, EventArgs e)
        {
            messageHandler.FakeBan("You've been `4BANNED `0from Growtopia for 730 days");
            messageHandler.isBanned = true;
        }

        private void guna2Button15_Click(object sender, EventArgs e)
        {
            Task.Run(() => GrowbrewProxy.Program.doDropAllInventory());
        }

        private void guna2CustomCheckBox31_Click(object sender, EventArgs e)
        {
            if (guna2CustomCheckBox31.Checked is true)
            {
                int WorldLength = int.Parse(guna2TextBox9.Text);
                string worldname = RandomString(WorldLength, false, false);
                messageHandler.packetSender.SendPacket(3, "action|join_request\nname|" + worldname, GrowbrewProxy.Program.realPeer);
                guna2TextBox8.Text = worldname;
                timer2.Start();
            }
            else
            {
                timer2.Stop();
            }
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            int WorldLength = int.Parse(guna2TextBox9.Text);
            string worldname = RandomString(WorldLength, false, false);
            messageHandler.packetSender.SendPacket(3, "action|join_request\nname|" + worldname, GrowbrewProxy.Program.realPeer);
            guna2TextBox8.Text = worldname;
        }

        private void guna2TextBox10_TextChanged(object sender, EventArgs e)
        {
            GrowbrewProxy.Program.globalUserData.country = guna2TextBox10.Text;
        }

        private void guna2TextBox9_TextChanged(object sender, EventArgs e)
        {

        }
        private void ShowServers()
        {
            StackPanelPlugin.Controls.Clear();

            for (int i = 0; i < PluginHandler.pluginInformations.Count; i++)
            {

                // Create a new UserControlServerAccount
                PluginSelect userControlplugin = new PluginSelect();
                userControlplugin.index = i;
                userControlplugin.PluginName = PluginHandler.pluginInformations[i].pluginName;
                userControlplugin.Description = PluginHandler.pluginInformations[i].description;
                userControlplugin.CreatedBy = PluginHandler.pluginInformations[i].createdBy;

                // Add to StackPanelServer
                StackPanelPlugin.Controls.Add(userControlplugin);
            }

            // Disable the selected server
            //StackPanelPlugin.Children[InvisibleManCore.index].IsEnabled = false;
        }
        int SpeedVal = 300;
        private void guna2CustomCheckBox33_Click(object sender, EventArgs e)
        {
            
        }

        private void guna2CustomCheckBox34_Click(object sender, EventArgs e)
        {

        }

        private void guna2Button19_Click(object sender, EventArgs e)
        {
        }

        private void guna2TrackBar4_Scroll(object sender, ScrollEventArgs e)
        {
            label27.Text = guna2TrackBar4.Value.ToString();
            GrowbrewProxy.Program.SetCustomGS((int)guna2TrackBar3.Value, (int)guna2TrackBar4.Value);
        }

        private void guna2Panel6_Paint(object sender, PaintEventArgs e)
        {

        }

        private void guna2CustomCheckBox33_Click_1(object sender, EventArgs e)
        {
            GrowbrewProxy.Program.globalUserData.blockCollecting = !GrowbrewProxy.Program.globalUserData.blockCollecting;
        }

        private void guna2CustomCheckBox34_Click_1(object sender, EventArgs e)
        {
            GrowbrewProxy.Program.globalUserData.enableAutoreconnect = guna2CustomCheckBox34.Checked;
        }

        private void guna2CustomCheckBox35_Click(object sender, EventArgs e)
        {
            GrowbrewProxy.Program.logallpackettypes = !GrowbrewProxy.Program.logallpackettypes;
        }

        private void annoyPlayers_Tick(object sender, EventArgs e)
        {
            TankPacket tPacket = new TankPacket();
            int tileX = messageHandler.worldMap.player.X / 32, tileY = (messageHandler.worldMap.player.Y / 32) + 1;
            //17
            tPacket.PacketType = 46;
            tPacket.X = tileX;
            tPacket.Y = tileY;
            tPacket.NetID = 0;
            tPacket.SecondaryNetID = tileX;
            tPacket.ExtDataMask = tileY;
            tPacket.MainValue = 3728;
            tPacket.PunchX = tileX;
            tPacket.PunchY = tileY;

            Player[] players = messageHandler.worldMap.players.ToArray();

            messageHandler.packetSender.SendPacketRaw(4, tPacket.PackForSendingRaw(), GrowbrewProxy.Program.realPeer, ENetPacketFlags.Reliable);
            foreach (var p in players)
            {

                tileX = p.X / 32;
                tileY = p.Y / 32;

                tPacket.X = tileX;
                tPacket.Y = tileY;
                tPacket.NetID = p.netID;
                tPacket.SecondaryNetID = tileX;
                tPacket.ExtDataMask = tileY;
                tPacket.MainValue = 82;
                tPacket.PunchX = tileX;
                tPacket.PunchY = tileY;

                byte[] packet = tPacket.PackForSendingRaw();
                //rn++;

                messageHandler.packetSender.SendPacketRaw(4, packet, GrowbrewProxy.Program.realPeer, ENetPacketFlags.Reliable);

                //Thread.Sleep(100);

            }
            GrowbrewProxy.Program.realPeer.Ping();
        }

        private void guna2CustomCheckBox36_Click(object sender, EventArgs e)
        {
            if (guna2CustomCheckBox36.Checked)
                annoyPlayers.Start();
            else
                annoyPlayers.Stop();
        }

        private void guna2CustomCheckBox36_Click_1(object sender, EventArgs e)
        {
            if (guna2CustomCheckBox36.Checked)
            {
                m.WriteMemory(Address.MoveWHileDead, "bytes", "75 7D");
            }
            else
            {
                m.WriteMemory(Address.MoveWHileDead, "bytes", "74 7D");
            }
        }

        private void guna2CustomCheckBox37_Click(object sender, EventArgs e)
        {
            if (guna2CustomCheckBox37.Checked)
            {
                m.WriteMemory(Address.airPin, "bytes", "90 90");
            }
            else
            {
                m.WriteMemory(Address.airPin, "bytes", "75 18");
            }
        }

        private void guna2CustomCheckBox38_Click(object sender, EventArgs e)
        {
            if (guna2CustomCheckBox38.Checked)
            {
                m.WriteMemory(Address.moonWalk1, "bytes", "90 90 90 90");
                m.WriteMemory(Address.moonWalk2, "bytes", "90 90 90 90 90");
                m.WriteMemory(Address.moonWalk3, "bytes", "90 90 90 90 90");
            }
            else
            {
                m.WriteMemory(Address.moonWalk1, "bytes", "F3 0F 11 11");
                m.WriteMemory(Address.moonWalk2, "bytes", "F3 0F 11 53 20");
                m.WriteMemory(Address.moonWalk3, "bytes", "F3 0F 11 41 04");
            }
        }

        private void guna2CustomCheckBox39_Click(object sender, EventArgs e)
        {
            if (guna2CustomCheckBox39.Checked)
            {
                m.WriteMemory(Address.moon, "bytes", "90 90");
                m.WriteMemory(Address.moon2, "bytes", "F3 0F 59 C7");
            }
            else
            {
                m.WriteMemory(Address.moon, "bytes", "74 0D");
                m.WriteMemory(Address.moon2, "bytes", "F3 0F 59 D7");
            }
        }

        private void guna2CustomCheckBox40_Click(object sender, EventArgs e)
        {
            if (guna2CustomCheckBox40.Checked)
            {
                m.WriteMemory(Address.antione, "bytes", "90 90");
                m.WriteMemory(Address.antione2, "bytes", "90 90 90");
            }
            else
            {
                m.WriteMemory(Address.antione, "bytes", "79 0F");
                m.WriteMemory(Address.antione2, "bytes", "45 85 F6");
            }
        }

        private void Bucket_FormClosing(object sender, FormClosingEventArgs e)
        {
            GrowtopiaHost();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            m.WriteMemory(Address.Bypass, "bytes", "90 90");
        }

        private void guna2CustomCheckBox41_Click(object sender, EventArgs e)
        {
            if (guna2CustomCheckBox41.Checked)
            {
                m.WriteMemory(Address.Slide, "bytes", "90 90");
            }
        }

        private void guna2TextBox11_TextChanged(object sender, EventArgs e)
        {

        }

        private void guna2CustomCheckBox32_Click(object sender, EventArgs e)
        {
            LoadOverlay();
        }

        private void LoadOverlay()
        {
            string _process = "Growtopia";
            _Interface.OverlayText = "BUCKETWARE";
            //_Interface.Colour = Color.White;
            Task.Run(() => doRGBEverything());
            _Interface.OverlaySize = Convert.ToInt32(50);
            try
            {
                Process p = Process.GetProcessesByName(_process)[0];
                RemoteHooking.IpcCreateServer<HookInterface>(ref ChannelName, System.Runtime.Remoting.WellKnownObjectMode.SingleCall, _Interface);
                RemoteHooking.Inject(p.Id, "Bucketware.Hook.dll", "Bucketware.Hook.dll", ChannelName);
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR: " + ex.Message + "\r\n" + ex.StackTrace);
            }
            /*new Thread(() =>
            {
                string _process = "Growtopia";
                _Interface.OverlayText = "";
                //_Interface.Colour = Color.White;
                Task.Run(() => doRGBEverything());
                _Interface.OverlaySize = Convert.ToInt32(20);
                try
                {
                    Process p = Process.GetProcessesByName(_process)[0];
                    RemoteHooking.IpcCreateServer<HookInterface>(ref ChannelName, System.Runtime.Remoting.WellKnownObjectMode.SingleCall, _Interface);
                    RemoteHooking.Inject(p.Id, "Bucketware.Hook.dll", "Bucketware.Hook.dll", ChannelName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("ERROR: " + ex.Message + "\r\n" + ex.StackTrace);
                }

            }).Start();*/
        }
    }
}
