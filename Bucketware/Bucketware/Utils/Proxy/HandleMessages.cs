using Bucketware.PluginSystem;
using ENet.Managed;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Bucketware.View;

namespace GrowbrewProxy
{


    public class HandleMessages
    {


        PluginHandler ph = new PluginHandler();
        private delegate void SafeCallDelegate(string text);
        public PacketSending packetSender = new PacketSending();
        VariantList variant = new VariantList();
        public bool FirstTime = true;
        public World worldMap = new World();
        //private bool OnPlayerPos = true;

        bool isSwitchingServers = false;
        public static List<ENetPeer> peers = new List<ENetPeer>();
        public bool enteredGame = false;
        public bool serverRelogReq = false;
        int checkPeerUsability(ENetPeer peer)
        {
            if (peer.IsNull) return -1;
            if (peer.State != ENetPeerState.Connected) return -3;

            return 0;
        }

        NetTypes.NetMessages GetMessageType(byte[] data)
        {
            uint messageType = uint.MaxValue - 1;
            if (data.Length > 4)
                messageType = BitConverter.ToUInt32(data, 0);
            return (NetTypes.NetMessages)messageType;
        }

        NetTypes.PacketTypes GetPacketType(byte[] packetData)
        {
            return (NetTypes.PacketTypes)packetData[0];
        }

        public struct GamePacket
        {
            public byte[] data;
            public int len;
            public int indexes;
        }

        public GamePacket appendFloat(GamePacket p, float val)
        {
            byte[] data = new byte[p.len + 2 + 4];
            Array.Copy(p.data, 0, data, 0, p.len);
            byte[] num = BitConverter.GetBytes(val);
            data[p.len] = (byte)p.indexes;
            data[p.len + 1] = 1;
            Array.Copy(num, 0, data, p.len + 2, 4);
            p.len = p.len + 2 + 4;
            p.indexes++;
            p.data = data;
            return p;
        }

        public GamePacket appendFloat(GamePacket p, float val, float val2)
        {
            byte[] data = new byte[p.len + 2 + 8];
            Array.Copy(p.data, 0, data, 0, p.len);
            byte[] fl1 = BitConverter.GetBytes(val);
            byte[] fl2 = BitConverter.GetBytes(val2);
            data[p.len] = (byte)p.indexes;
            data[p.len + 1] = 3;
            Array.Copy(fl1, 0, data, p.len + 2, 4);
            Array.Copy(fl2, 0, data, p.len + 6, 4);
            p.len = p.len + 2 + 8;
            p.indexes++;
            p.data = data;
            return p;
        }
        public GamePacket appendFloat(GamePacket p, float val, float val2, float val3)
        {
            byte[] data = new byte[p.len + 2 + 12];
            Array.Copy(p.data, 0, data, 0, p.len);
            byte[] fl1 = BitConverter.GetBytes(val);
            byte[] fl2 = BitConverter.GetBytes(val2);
            byte[] fl3 = BitConverter.GetBytes(val3);
            data[p.len] = (byte)p.indexes;
            data[p.len + 1] = 3;
            Array.Copy(fl1, 0, data, p.len + 2, 4);
            Array.Copy(fl2, 0, data, p.len + 6, 4);
            Array.Copy(fl3, 0, data, p.len + 10, 4);
            p.len = p.len + 2 + 12;
            p.indexes++;
            p.data = data;
            return p;
        }

        public GamePacket appendInt(GamePacket p, Int32 val)
        {
            byte[] data = new byte[p.len + 2 + 4];
            Array.Copy(p.data, 0, data, 0, p.len);
            byte[] num = BitConverter.GetBytes(val);
            data[p.len] = (byte)p.indexes;
            data[p.len + 1] = 9;
            Array.Copy(num, 0, data, p.len + 2, 4);
            p.len = p.len + 2 + 4;
            p.indexes++;
            p.data = data;
            return p;
        }

        public GamePacket appendIntx(GamePacket p, Int32 val)
        {
            byte[] data = new byte[p.len + 2 + 4];
            Array.Copy(p.data, 0, data, 0, p.len);
            byte[] num = BitConverter.GetBytes(val);
            data[p.len] = (byte)p.indexes;
            data[p.len + 1] = 5;
            Array.Copy(num, 0, data, p.len + 2, 4);
            p.len = p.len + 2 + 4;
            p.indexes++;
            p.data = data;
            return p;
        }

        public GamePacket appendString(GamePacket p, string str)
        {
            byte[] data = new byte[p.len + 2 + str.Length + 4];
            Array.Copy(p.data, 0, data, 0, p.len);
            byte[] strn = Encoding.ASCII.GetBytes(str);
            data[p.len] = (byte)p.indexes;
            data[p.len + 1] = 2;
            byte[] len = BitConverter.GetBytes(str.Length);
            Array.Copy(len, 0, data, p.len + 2, 4);
            Array.Copy(strn, 0, data, p.len + 6, str.Length);
            p.len = p.len + 2 + str.Length + 4;
            p.indexes++;
            p.data = data;
            return p;
        }

        public GamePacket createPacket()
        {
            byte[] data = new byte[61];
            string asdf = "0400000001000000FFFFFFFF00000000080000000000000000000000000000000000000000000000000000000000000000000000000000000000000000";
            for (int i = 0; i < asdf.Length; i += 2)
            {
                byte x = ch2n(asdf[i]);
                x = (byte)(x << 4);
                x += ch2n(asdf[i + 1]);
                data[i / 2] = x;
                if (asdf.Length > 61 * 2) throw new Exception("?");
            }
            GamePacket packet;
            packet.data = data;
            packet.len = 61;
            packet.indexes = 0;
            return packet;
        }

        public GamePacket packetEnd(GamePacket p)
        {
            byte[] n = new byte[p.len + 1];
            Array.Copy(p.data, 0, n, 0, p.len);
            p.data = n;
            p.data[p.len] = 0;
            p.len += 1;
            p.data[56] = (byte)p.indexes;
            p.data[60] = (byte)p.indexes;
            //*(BYTE*)(p.data + 60) = p.indexes;
            return p;
        }

        public byte ch2n(char x)
        {
            switch (x)
            {
                case '0':
                    return 0;
                case '1':
                    return 1;
                case '2':
                    return 2;
                case '3':
                    return 3;
                case '4':
                    return 4;
                case '5':
                    return 5;
                case '6':
                    return 6;
                case '7':
                    return 7;
                case '8':
                    return 8;
                case '9':
                    return 9;
                case 'A':
                    return 10;
                case 'B':
                    return 11;
                case 'C':
                    return 12;
                case 'D':
                    return 13;
                case 'E':
                    return 14;
                case 'F':
                    return 15;
            }

            return 0;
        }
        public bool isSpamDect = false;
        private int OperateVariant(VariantList.VarList vList, object botPeer)
        {
            switch (vList.FunctionName)
            {
                case "OnConsoleMessage":
                    {
                        string m = (string)vList.functionArgs[1];
                        if ((m.Contains("lagged out,") || m.Contains("experiencing high load")) && !m.Contains("<") && !m.Contains("["))
                        {
                            GamePacketProton variantPacket2 = new GamePacketProton();
                            variantPacket2.AppendString("OnReconnect");
                            packetSender.SendData(variantPacket2.GetBytes(), Program.proxyPeer);
                        }
                        if (m.Contains("Spam detected"))
                        {
                            Program.Spam = false;
                            packetSender.SendPacket(3, "action|log\n|msg|`1Got spam detect spammer is stopped for 10sec", Program.proxyPeer);
                            isSpamDect = true;
                            Task.Run(() => Program.CheckSpam());
                        }

                        break;
                    }
                case "OnSetPos":
                    {
                        if (Program.globalUserData.cheat_password is true)
                        {
                            Program.globalUserData.actived = true;
                            packetSender.SendPacket(3, "action|log\n|msg|`1The correct number was about: `2" + Bucket.doortry, Program.proxyPeer);
                            Program.globalUserData.cheat_password = false;
                        }
                        break;
                    }
                case "OnRequestWorldSelectMenu":
                    {
                        if (Program.globalUserData.autojoin is true)
                        {
                            packetSender.SendPacket(3, "action|join_request\nname|" + Program.globalUserData.autojoinworld, Program.realPeer);
                        }
                        break;
                    }
                case "OnSuperMainStartAcceptLogonHrdxs47254722215a":
                    {
                        if (isBanned is true)
                        {
                            /*GamePacketProton variantPacket = new GamePacketProton();
                            variantPacket.AppendString("OnConsoleMessage");
                            variantPacket.AppendString("`4Sorry, this account (`&" + Program.globalUserData.tankIDName + "`4) has been suspended.");
                            packetSender.SendData(variantPacket.GetBytes(), Program.proxyPeer);*/
                            //packetSender.SendPacket(3, "action|log\n|msg|`4Sorry, this account (`&" + Program.globalUserData.tankIDName+ "`4) has been suspended.", Program.proxyPeer);
                            return -1;
                        }
                        if (Program.skipCache && botPeer == null)
                        {
                            Program.LogText += ("[" + DateTime.UtcNow + "] (CLIENT): Skipping potential caching (will make world list disappear)...");
                            GamePacketProton gp = new GamePacketProton(); // variant list
                            gp.AppendString("OnRequestWorldSelectMenu");
                            packetSender.SendData(gp.GetBytes(), Program.proxyPeer);
                        }
                        if (botPeer != null)
                        {
                            Console.WriteLine("BOT PEER IS ENTERING THE GAME...");
                            packetSender.SendPacket(3, "action|enter_game\n", (ENetPeer)botPeer);
                        }
                        return -1;
                    }
                case "OnZoomCamera":
                    {
                        Program.LogText += ("[" + DateTime.UtcNow + "] (SERVER): Camera zoom parameters (" + vList.functionArgs.Length + "): v1: " + ((float)vList.functionArgs[1] / 1000).ToString() + " v2: " + vList.functionArgs[2].ToString());
                        return -1;
                    }
                case "onShowCaptcha":
                    ((string)vList.functionArgs[1]).Replace("PROCESS_LOGON_PACKET_TEXT_42", "");// make captcha completable
                    try
                    {
                        string[] lines = ((string)vList.functionArgs[1]).Split('\n');
                        foreach (string line in lines)
                        {
                            if (line.Contains("+"))
                            {
                                string line2 = line.Replace(" ", "");
                                int a1, a2;
                                string[] splitByPipe = line2.Split('|');
                                string[] splitByPlus = splitByPipe[1].Split('+');
                                a1 = int.Parse(splitByPlus[0]);
                                a2 = int.Parse(splitByPlus[1]);
                                int result = a1 + a2;
                                string resultingPacket = "action|dialog_return\ndialog_name|captcha_submit\ncaptcha_answer|" + result.ToString() + "\n";
                                packetSender.SendPacket(2, resultingPacket, Program.realPeer);
                                packetSender.SendPacket(3, "action|log\n|msg|`bAuto Captha: `1Captha answer was `2" + result.ToString(), Program.proxyPeer);
                            }
                        }
                        return -1;
                    }
                    catch
                    {
                        return -1; // Give this to user.
                    }
                case "OnDialogRequest":
                    Program.LogText += ("[" + DateTime.UtcNow + "] (SERVER): OnDialogRequest called, logging its params here:\n" +
                           (string)vList.functionArgs[1] + "\n");
                    if (!((string)vList.functionArgs[1]).ToLower().Contains("captcha")) return -1; // Send Client Dialog
                    ((string)vList.functionArgs[1]).Replace("PROCESS_LOGON_PACKET_TEXT_42", "");// make captcha completable
                    try
                    {
                        string[] lines = ((string)vList.functionArgs[1]).Split('\n');
                        foreach (string line in lines)
                        {
                            if (line.Contains("+"))
                            {
                                string line2 = line.Replace(" ", "");
                                int a1, a2;
                                string[] splitByPipe = line2.Split('|');
                                string[] splitByPlus = splitByPipe[1].Split('+');
                                a1 = int.Parse(splitByPlus[0]);
                                a2 = int.Parse(splitByPlus[1]);
                                int result = a1 + a2;
                                string resultingPacket = "action|dialog_return\ndialog_name|captcha_submit\ncaptcha_answer|" + result.ToString() + "\n";
                                packetSender.SendPacket(2, resultingPacket, Program.realPeer);
                                packetSender.SendPacket(3, "action|log\n|msg|`bAuto Captha: `1Captha answer was `2" + result.ToString(), Program.proxyPeer);
                            }
                        }
                        return -1;
                    }
                    catch
                    {
                        return -1; // Give this to user.
                    }

                case "OnSendToServer":
                    {
                        // TODO FIX THIS AND MIRROR ALL PACKETS AND SOME BUG FIXES.

                        string ip = (string)vList.functionArgs[4];
                        string doorid = "";

                        if (ip.Contains("|"))
                        {
                            doorid = ip.Substring(ip.IndexOf("|") + 1);
                            ip = ip.Substring(0, ip.IndexOf("|"));
                        }

                        int port = (int)vList.functionArgs[1];
                        int userID = (int)vList.functionArgs[3];
                        int token = (int)vList.functionArgs[2];

                        Program.LogText += ("[" + DateTime.UtcNow + "] (SERVER): OnSendToServer (func call used for server switching/sub-servers) " +
                                "IP: " +
                                ip + " PORT: " + port
                                + " UserId: " + userID
                                + " Session-Token: " + token + "\n");
                        /*GamePacketProton variantPacket = new GamePacketProton();
                        variantPacket.AppendString("OnConsoleMessage");
                        variantPacket.AppendString("`1(BUCKETWARE)`e Switching subserver...``");
                        packetSender.SendData(variantPacket.GetBytes(), Program.proxyPeer);*/

                        Program.globalUserData.Growtopia_IP = token < 0 ? Program.globalUserData.Growtopia_Master_IP : ip;
                        Program.globalUserData.Growtopia_Port = token < 0 ? Program.globalUserData.Growtopia_Master_Port : port;
                        Program.globalUserData.isSwitchingServer = true;
                        Program.globalUserData.token = token;
                        Program.globalUserData.lmode = 1;
                        Program.globalUserData.userID = userID;
                        Program.globalUserData.doorid = doorid;

                        packetSender.SendPacket(3, "action|quit", Program.realPeer);
                        Program.realPeer.Disconnect(0);

                        return -1;
                    }
                case "OnSpawn":
                    {
                        Player p = new Player();
                        worldMap.playerCount++;
                        string onspawnStr = (string)vList.functionArgs[1];
                        string[] tk = onspawnStr.Split('|');
                        string[] lines = onspawnStr.Split('\n');
                        bool localplayer = false;

                        foreach (string line in lines)
                        {
                            string[] lineToken = line.Split('|');
                            if (lineToken.Length != 2) continue;
                            switch (lineToken[0])
                            {
                                case "netID":
                                    p.netID = Convert.ToInt32(lineToken[1]);
                                    break;
                                case "userID":
                                    p.userID = Convert.ToInt32(lineToken[1]);
                                    break;
                                case "name":
                                    p.name = lineToken[1];
                                    break;
                                case "country":
                                    p.country = lineToken[1];
                                    break;
                                case "invis":
                                    p.invis = Convert.ToInt32(lineToken[1]);
                                    break;
                                case "mstate":
                                    p.mstate = Convert.ToInt32(lineToken[1]);
                                    break;
                                case "smstate":
                                    p.mstate = Convert.ToInt32(lineToken[1]);
                                    break;
                                case "posXY":
                                    if (lineToken.Length == 3) // exactly 3 not more not less
                                    {
                                        p.X = Convert.ToInt32(lineToken[1]);
                                        p.Y = Convert.ToInt32(lineToken[2]);
                                    }
                                    break;
                                case "type":
                                    if (lineToken[1] == "local") localplayer = true;
                                    break;

                            }
                        }
                        if (Program.globalUserData.cheat_msgnew)
                        {
                            string pName = p.name.Substring(2);
                            pName = pName.Substring(0, pName.Length - 2);
                            packetSender.SendPacket((int)NetTypes.NetMessages.GENERIC_TEXT, "action|input\n|text|/msg " + pName + " " + Program.globalUserData.cheat_msgnew_text, Program.realPeer);
                        }
                        worldMap.players.Add(p);

                        if (p.mstate > 0 || p.smstate > 0 || p.invis > 0)
                        {
                            if (Program.globalUserData.cheat_autoworldban_mod) banEveryoneInWorld();
                            if (Program.globalUserData.cheat_leavemod) Leavemod();
                            GamePacketProton variantPacket = new GamePacketProton();
                            variantPacket.AppendString("OnConsoleMessage");
                            variantPacket.AppendString("`b(BUCKETWARE) `4A moderator or developer seems to have joined your world!``");
                            Program.LogText += ("[" + DateTime.UtcNow + "] (PROXY): A moderator or developer seems to have joined your world!");
                        }
                        if (Program.globalUserData.cheat_Banothers)
                        {
                            banEveryoneInWorld();
                        }
                        if (localplayer)
                        {

                            string lestring = (string)vList.functionArgs[1];

                            string[] avatardata = lestring.Split('\n');
                            string modified_avatardata = string.Empty;

                            foreach (string av in avatardata)
                            {
                                if (av.Length <= 0) continue;

                                string key = av.Substring(0, av.IndexOf('|'));
                                string value = av.Substring(av.IndexOf('|') + 1);

                                switch (key)
                                {
                                    case "mstate": // unlimited punch/place range edit smstate
                                        value = "1";
                                        break;
                                }

                                modified_avatardata += key + "|" + value + "\n";
                            }

                            //lestring = lestring.Replace("mstate|0", "mstate|1");

                            if (Program.globalUserData.unlimitedZoom)
                            {
                                GamePacketProton gp = new GamePacketProton();
                                gp.AppendString("OnSpawn");
                                gp.AppendString(modified_avatardata);
                                gp.delay = (int)vList.delay;
                                gp.NetID = vList.netID;

                                packetSender.SendData(gp.GetBytes(), Program.proxyPeer);
                            }
                            Program.LogText += ("[" + DateTime.UtcNow + "] (PROXY): World player objects loaded! Your NetID:  " + p.netID + " -- Your UserID: " + p.userID + "\n");
                            GamePacketProton variantPacket = new GamePacketProton();
                            variantPacket.AppendString("OnConsoleMessage");
                            variantPacket.AppendString("`1(BUCKETWARE)`e World player objects loaded! Your NetID:  " + p.netID + " -- Your UserID: " + p.userID + "``");
                            if (FirstTime is true)
                            {
                                GamePacket p22 = packetEnd(appendString(appendString(createPacket(), "OnDialogRequest"), "set_default_color|`o\n\nadd_label_with_icon|big|`wWelcome to `eBUCKETWARE``|left|822|\n\nadd_spacer|small|\nadd_label_with_icon|small|`wVersion: `e4.0``|left|7234|\n\nadd_spacer|small|\nadd_label_with_icon|small|`wDiscord Server: `ediscord.io/bucketware|left|7474|\n\nadd_spacer|small|\nadd_label_with_icon|small|`wDownload only from `ebucketware.ga``|left|7656||\nadd_spacer|small|\nadd_quick_exit|"));
                                packetSender.SendData(p22.data, Program.proxyPeer);
                                FirstTime = false;
                            }
                            //packetSender.SendPacket(3, "action|input\n|text|`1(BUCKETWARE)`e" + Program.globalUserData.tankIDName + " Joined World: " + worldMap.currentWorld + " NetID: " + worldMap.netID + " Players: " + worldMap.playerCount, Program.proxyPeer);

                            worldMap.netID = p.netID;
                            worldMap.userID = p.userID;
                            return -2;
                        }
                        else
                        {
                            return p.netID;
                        }
                    }
                case "OnRemove":
                    {
                        int netID = -1;

                        string onremovestr = (string)vList.functionArgs[1];
                        string[] lineToken = onremovestr.Split('|');
                        if (lineToken[0] != "netID") break;

                        int.TryParse(lineToken[1], out netID);
                        for (int i = 0; i < worldMap.players.Count; i++)
                        {
                            if (worldMap.players[i].netID == netID)
                            {
                                worldMap.players.RemoveAt(i);
                                break;
                            }
                        }

                        return netID;
                    }
                default:
                    return -1;
            }
            return 0;
        }
        string GetProperGenericText(byte[] data)
        {
            string growtopia_text = string.Empty;
            if (data.Length > 5)
            {
                int len = data.Length - 5;
                byte[] croppedData = new byte[len];
                Array.Copy(data, 4, croppedData, 0, len);
                growtopia_text = Encoding.ASCII.GetString(croppedData);
            }
            return growtopia_text;
        }
        private void SwitchServers(ref ENetPeer peer, string ip, int port, int lmode = 0, int userid = 0, int token = 0)
        {
            Program.globalUserData.Growtopia_IP = token < 0 ? Program.globalUserData.Growtopia_Master_IP : ip;
            Program.globalUserData.Growtopia_Port = token < 0 ? Program.globalUserData.Growtopia_Master_Port : port;
            isSwitchingServers = true;

            Program.ConnectToServer(ref peer, Program.globalUserData);
        }
        public void Leavemod()
        {
            packetSender.SendPacket(3, "action|join_request\nname|exit", Program.realPeer);
        }
        public static string GenerateColor(int length = 1)
        {
            const string chars = "4b";

            var random = new Random();
            var randomString = new string(Enumerable.Repeat(chars, length)
                                                    .Select(s => s[random.Next(s.Length)]).ToArray());
            return randomString;
        }
        void banEveryoneInWorld()
        {
            foreach (Player p in worldMap.players)
            {
                string pName = p.name.Substring(2);
                pName = pName.Substring(0, pName.Length - 2);
                packetSender.SendPacket((int)NetTypes.NetMessages.GENERIC_TEXT, "action|input\n|text|/ban " + pName, Program.realPeer);
            }
        }
        public bool isBanned = false;
        public void FakeBan(string text)
        {
            packetSender.SendPacket(3, "action|log\n|msg|`#** `$The Ancient Ones `ohave used `#Ban `oon `2" + Program.globalUserData.tankIDName + "`o! `#**", Program.proxyPeer);
            packetSender.SendPacket(3, "action|log\n|msg|`oWarning from `4System`o: You've been `4BANNED `ofrom Growtopia for 730 days", Program.proxyPeer);
            GamePacket ps2 = packetEnd(appendInt(appendString(appendString(appendString(appendString(createPacket(), "OnAddNotification"), "interface/atomic_button.rttex"), "`0Warning from `4System`0: " + text), "audio/hub_open.wav"), 0));
            packetSender.SendData(ps2.data, Program.proxyPeer);
            bool dog = true;
            while (dog)
            {
                Thread.Sleep(600);
                Program.proxyPeer.DisconnectNow(0);
                dog = false;
            }
        }
        public void SendAON(string text)
        {
            GamePacket ps2 = packetEnd(appendInt(appendString(appendString(appendString(appendString(createPacket(), "OnAddNotification"), "interface/atomic_button.rttex"), "`0Warning from `4Bucketware`0: " + text), "audio/hub_open.wav"), 0));
            packetSender.SendData(ps2.data, Program.proxyPeer);
        }
        void msgAllInWorld()
        {
            foreach (Player p in worldMap.players)
            {
                string pName = p.name.Substring(2);
                pName = pName.Substring(0, pName.Length - 2);
                packetSender.SendPacket((int)NetTypes.NetMessages.GENERIC_TEXT, "action|input\n|text|/msg " + pName + " KYS", Program.realPeer);
            }
        }
        bool IsBitSet(int b, int pos)
        {
            return (b & (1 << pos)) != 0;
        }

        public static bool SkinChanger = false;
        public static System.Timers.Timer newTimer = new System.Timers.Timer();
        public string HandlePacketFromClient(ref ENetPeer peer, ENetPacket packet) // Why string? Oh yeah, it's the best thing to also return a string response for anything you want!
        {

            if (peer.IsNull) return "";
            if (peer.State != ENetPeerState.Connected) return "";
            if (Program.realPeer.IsNull) return "";
            if (Program.realPeer.State != ENetPeerState.Connected) return "";

            bool respondToBotPeers = true;
            byte[] data = packet.Data.ToArray();

            string log = string.Empty;

            //PluginHandler ph = new PluginHandler();
            switch ((NetTypes.NetMessages)data[0])
            {
                case NetTypes.NetMessages.GENERIC_TEXT:
                    string str = GetProperGenericText(data);

                    Program.LogText += ("[" + DateTime.UtcNow + "] (CLIENT): String package fetched (GENERIC_TEXT):\n" + str + "\n");
                    if (str.StartsWith("action|"))
                    {
                        string actionExecuted = str.Substring(7, str.Length - 7);
                        string inputPH = "input\n|text|";
                        if (actionExecuted.StartsWith("enter_game"))
                        {
                            respondToBotPeers = true;
                            if (Program.globalUserData.blockEnterGame) return "Blocked enter_game packet!";
                            enteredGame = true;
                        }
                        else if (actionExecuted.StartsWith(inputPH))
                        {
                            string text = actionExecuted.Substring(inputPH.Length);

                            if (text.Length > 0)
                            {
                                if (text.StartsWith("/")) // bAd hAcK - but also lazy, so i'll be doing this.
                                {

                                    switch (text)
                                    {
                                        case "/banworld":
                                            {
                                                banEveryoneInWorld();
                                                return "called /banworld, attempting to ban everyone who is in world (requires admin/owner)";
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                    switch (text)
                                    {
                                        case "/tile":
                                            {
                                                Player playerObject = worldMap.player;
                                                float tilex = playerObject.X / 32;
                                                float tiley = playerObject.Y / 32;
                                                packetSender.SendPacket(3, "action|log\n|msg|`1Current tile: " + tilex + ", " + tiley, Program.proxyPeer);
                                                return "123";
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                    switch (text)
                                    {
                                        case "/beta":
                                            {
                                                GamePacket p2 = packetEnd(appendInt(appendString(createPacket(), "OnSetBetaMode"), 1));
                                                packetSender.SendData(p2.data, Program.proxyPeer);
                                            }
                                            break;
                                        default:
                                            break;
                                    }

                                    switch (text)
                                    {
                                        case "/mod":
                                            {
                                                bool isMod = false;
                                                if (isMod is false)
                                                {
                                                    GamePacket p2 = packetEnd(appendInt(appendString(createPacket(), "OnInvis"), 1));
                                                    packetSender.SendData(p2.data, Program.proxyPeer);
                                                    Program.globalUserData.skinColor[0] = 110; // A - transparency
                                                    Program.globalUserData.skinColor[1] = 255;
                                                    Program.globalUserData.skinColor[2] = 255;
                                                    Program.globalUserData.skinColor[3] = 255;

                                                    GamePacketProton variantPacket = new GamePacketProton();
                                                    variantPacket.AppendString("OnChangeSkin");
                                                    variantPacket.AppendUInt(BitConverter.ToUInt32(Program.globalUserData.skinColor, 0));
                                                    variantPacket.NetID = worldMap.netID;
                                                    //variantPacket.delay = 100;
                                                    packetSender.SendData(variantPacket.GetBytes(), Program.proxyPeer);
                                                    packetSender.SendPacket(3, "action|log\nmsg|`oYou cast a mod spell!", Program.proxyPeer);
                                                    packetSender.SendPacket(2, "action|input\n|text|/cheer", Program.realPeer);
                                                    GamePacket ps3 = packetEnd(appendInt(appendString(appendString(appendString(appendString(createPacket(), "OnAddNotification"), "interface/star.rttex"), "You cast a mod spell!"), "audio/magic.wav"), 0));
                                                    packetSender.SendData(ps3.data, Program.proxyPeer);
                                                    TankPacket datx = new TankPacket();
                                                    datx.PacketType = (int)NetTypes.PacketTypes.PARTICLE_EFF;
                                                    datx.X = worldMap.player.X;
                                                    datx.Y = worldMap.player.Y;
                                                    datx.YSpeed = (int)90;
                                                    datx.XSpeed = (float)90;
                                                    datx.MainValue = (int)90;
                                                    packetSender.SendPacketRaw(4, datx.PackForSendingRaw(), Program.proxyPeer);
                                                    isMod = true;
                                                }
                                                else
                                                {

                                                }
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        // for (int i = 0; i < 1000; i++) packetSender.SendPacket(2, "action|refresh_item_data\n", MainForm.realPeer);
                        respondToBotPeers = false;
                        string[] lines = str.Split('\n');

                        string tankIDName = "";
                        foreach (string line in lines)
                        {
                            string[] lineToken = line.Split('|');
                            if (lineToken.Length != 2) continue;
                            switch (lineToken[0])
                            {
                                case "tankIDName":
                                    tankIDName = lineToken[1];
                                    break;
                                case "tankIDPass":
                                    Program.globalUserData.tankIDPass = lineToken[1];
                                    break;
                                case "requestedName":
                                    Program.globalUserData.requestedName = lineToken[1];
                                    break;
                                case "token":
                                    Program.globalUserData.token = int.Parse(lineToken[1]);
                                    break;
                                case "user":
                                    Program.globalUserData.userID = int.Parse(lineToken[1]);
                                    break;
                                case "lmode":
                                    Program.globalUserData.lmode = int.Parse(lineToken[1]);
                                    break;

                            }
                        }
                        Program.globalUserData.tankIDName = tankIDName;
                        
                        bool hasAcc = false;

                        packetSender.SendPacket((int)NetTypes.NetMessages.GENERIC_TEXT, Program.CreateLogonPacket(), Program.realPeer);
                        return "Sent logon packet!"; // handling logon over proxy
                    }
                    break;
                case NetTypes.NetMessages.GAME_MESSAGE:
                    string str2 = GetProperGenericText(data);
                    Program.LogText += ("[" + DateTime.UtcNow + "] (CLIENT): String package fetched (GAME_MESSAGE):\n" + str2 + "\n");
                    if (str2.StartsWith("action|"))
                    {
                        string actionExecuted = str2.Substring(7);
                        if (actionExecuted.StartsWith("quit") && !actionExecuted.StartsWith("quit_to_exit"))
                        {

                            // super multibotting will not mirror all packets in here (the "quit" action), cuz i found it unnecessary, although, you can enable that by pasting the code that does it.
                            respondToBotPeers = true;
                            Program.globalUserData.token = -1;
                            Program.globalUserData.Growtopia_IP = Program.globalUserData.Growtopia_Master_IP;
                            Program.globalUserData.Growtopia_Port = Program.globalUserData.Growtopia_Master_Port;

                            if (Program.realPeer != null)
                            {
                                if (!Program.realPeer.IsNull)
                                    if (Program.realPeer.State != ENetPeerState.Disconnected) Program.realPeer.Disconnect(0);
                            }
                            if (Program.proxyPeer != null)
                            {
                                if (!Program.proxyPeer.IsNull)
                                    if (Program.proxyPeer.State == ENetPeerState.Connected) Program.proxyPeer.Disconnect(0);
                            }
                        }
                        else if (actionExecuted.StartsWith("join_request\nname|")) // ghetto fetching of worldname
                        {
                            string[] rest = actionExecuted.Substring(18).Split('\n');
                            string joinWorldName = rest[0];
                            Console.WriteLine($"Joining world {joinWorldName}...");
                        }
                    }
                    break;
                case NetTypes.NetMessages.GAME_PACKET:
                    {
                        TankPacket p = TankPacket.UnpackFromPacket(data);

                        switch ((NetTypes.PacketTypes)(byte)p.PacketType)
                        {
                            case NetTypes.PacketTypes.APP_INTEGRITY_FAIL:  /*rn definitely just blocking autoban packets, 
                                usually a failure of an app integrity is never good 
                                and usually used for security stuff*/
                                return "Possible autoban packet with id (25) from your GT Client has been blocked."; // remember, returning anything will interrupt sending this packet. To Edit packets, load/parse them and you may just resend them like normally after fetching their bytes.
                            case NetTypes.PacketTypes.PLAYER_LOGIC_UPDATE:
                                if (p.PunchX > 0 || p.PunchY > 0)
                                {

                                    if (Program.globalUserData.cheat_casinohack)
                                    {
                                        int spun = Bucket.spunwheel;
                                        string color = "b";
                                        bool punched = true;
                                        bool punchonce = false;

                                        if (spun.Equals(0))
                                        {
                                            color = "2";
                                        }
                                        else
                                        {
                                            color = GenerateColor(1);
                                        }

                                        while (punched)
                                        {
                                            if (!punchonce is true)
                                            {
                                                punchonce = true;
                                                Thread.Sleep(3000);
                                                packetSender.SendPacket(2, "action|input\n|text|[" + Program.globalUserData.tankIDName + " spun the wheel and got `" + color + spun + "`w!]", Program.realPeer);
                                                punched = false;
                                                punchonce = false;
                                            }
                                        }
                                    }
                                    if (Program.globalUserData.cheat_selectdoor is true)
                                    {
                                        Program.globalUserData.cheat_door_x = p.PunchX;
                                        Program.globalUserData.cheat_door_y = p.PunchY;
                                        Program.globalUserData.cheat_selectdoor = false;
                                        packetSender.SendPacket(3, "action|log\n|msg|`eX: " + p.PunchX + " Y: " + p.PunchY, Program.proxyPeer);
                                    }
                                    Program.LogText += ("[" + DateTime.UtcNow + "] (PROXY): PunchX/PunchY detected, pX: " + p.PunchX.ToString() + " pY: " + p.PunchY.ToString() + "\n");
                                }
                                Program.globalUserData.isFacingSwapped = IsBitSet(p.CharacterState, 4);

                                worldMap.player.X = (int)p.X;
                                worldMap.player.Y = (int)p.Y;
                                break;
                            case NetTypes.PacketTypes.PING_REPLY:
                                {
                                    //SpoofedPingReply(p);
                                    return "Blocked ping reply!";
                                }
                            case NetTypes.PacketTypes.TILE_CHANGE_REQ:
                                respondToBotPeers = true;

                                if (p.MainValue == 32)
                                {
                                    /*MessageBox.Show("Log of potentially wanted received GAME_PACKET Data:" +
                    "\npackettype: " + data[4].ToString() +
                    "\npadding byte 1|2|3: " + data[5].ToString() + "|" + data[6].ToString() + "|" + data[7].ToString() +
                    "\nnetID: " + p.NetID +
                    "\nsecondnetid: " + p.SecondaryNetID +
                    "\ncharacterstate (prob 8): " + p.CharacterState +
                    "\nwaterspeed / offs 16: " + p.Padding +
                    "\nmainval: " + p.MainValue +
                    "\nX|Y: " + p.X + "|" + p.Y +
                    "\nXSpeed: " + p.XSpeed +
                    "\nYSpeed: " + p.YSpeed +
                    "\nSecondaryPadding: " + p.SecondaryPadding +
                    "\nPunchX|PunchY: " + p.PunchX + "|" + p.PunchY);*/

                                    Program.globalUserData.lastWrenchX = (short)p.PunchX;
                                    Program.globalUserData.lastWrenchY = (short)p.PunchY;
                                }
                                else if (p.MainValue == 18 && Program.globalUserData.cheat_casinohack)
                                {
                                    // playingo
                                    p.SecondaryPadding = -4;
                                    p.ExtDataMask |= 1 << 24; // 28
                                    p.Padding = 2;
                                    packetSender.SendPacketRaw(4, p.PackForSendingRaw(), Program.realPeer);
                                    return "";
                                }
                                break;
                            case NetTypes.PacketTypes.ITEM_ACTIVATE_OBJ: // just incase, to keep better track of items incase something goes wrong
                                worldMap.dropped_ITEMUID = p.MainValue;
                                if (Program.globalUserData.blockCollecting) return "";
                                break;
                            default:
                                //MainForm.LogText += ("[" + DateTime.UtcNow + "] (CLIENT): Got Packet Type: " + p.PacketType.ToString() + "\n");
                                break;
                        }

                        if (data[4] > 23)
                        {
                            log = $"(CLIENT) Log of potentially wanted received GAME_PACKET Data:" +
                        "\npackettype: " + data[4].ToString() +
                        "\npadding byte 1|2|3: " + data[5].ToString() + "|" + data[6].ToString() + "|" + data[7].ToString() +
                        "\nnetID: " + p.NetID +
                        "\nsecondnetid: " + p.SecondaryNetID +
                        "\ncharacterstate (prob 8): " + p.CharacterState +
                        "\nwaterspeed / offs 16: " + p.Padding +
                        "\nmainval: " + p.MainValue +
                        "\nX|Y: " + p.X + "|" + p.Y +
                        "\nXSpeed: " + p.XSpeed +
                        "\nYSpeed: " + p.YSpeed +
                        "\nSecondaryPadding: " + p.SecondaryPadding +
                        "\nPunchX|PunchY: " + p.PunchX + "|" + p.PunchY;

                        }
                    }

                    break;
                case NetTypes.NetMessages.TRACK:
                    return "Packet with messagetype used for tracking was blocked!";
                case NetTypes.NetMessages.LOG_REQ:
                    return "Log request packet from client was blocked!";
                default:
                    break;
            }

            packetSender.SendData(data, Program.realPeer);

            return log;

        }
        private void SpoofedPingReply(TankPacket tPacket)
        {
            if (worldMap == null) return;
            TankPacket p = new TankPacket();
            p.PacketType = (int)NetTypes.PacketTypes.PING_REPLY;
            p.PunchX = (int)1000.0f;
            p.PunchY = (int)250.0f;
            p.X = 64.0f;
            p.Y = 64.0f;
            p.MainValue = tPacket.MainValue; // GetTickCount()
            p.SecondaryNetID = (int)Program.HashBytes(BitConverter.GetBytes(tPacket.MainValue)); // HashString of it

            // rest is 0 by default to not get detected by ac.
            packetSender.SendPacketRaw((int)NetTypes.NetMessages.GAME_PACKET, p.PackForSendingRaw(), Program.realPeer);
        }

        public string HandlePacketFromServer(ref ENetPeer peer, ENetPacket packet)
        {

            if (Program.proxyPeer.IsNull) return "HandlePacketFromServer() -> Proxy peer is null!";
            if (Program.proxyPeer.State != ENetPeerState.Connected) return $"HandlePacketFromServer() -> proxyPeer is not connected: state = {Program.proxyPeer.State}";
            if (peer.IsNull) return "HandlePacketFromServer() -> peer.IsNull is true!";
            if (peer.State != ENetPeerState.Connected) return "HandlePacketFromServer() -> peer.State was not ENetPeerState.Connected!";

            byte[] data = packet.Data.ToArray();

            NetTypes.NetMessages msgType = (NetTypes.NetMessages)data[0]; // more performance.
            switch (msgType)
            {
                case NetTypes.NetMessages.SERVER_HELLO:
                    {
                        Program.LogText += ("[" + DateTime.UtcNow + "] (SERVER): Initial logon accepted." + "\n");
                        Program.UserData ud;

                        if (peer.TryGetUserData(out ud))
                            packetSender.SendPacket(2, Program.CreateLogonPacket(ud.tankIDName, ud.tankIDPass, ud.userID, ud.token, ud.doorid), peer);

                        break;
                    }
                case NetTypes.NetMessages.GAME_MESSAGE:

                    string str = GetProperGenericText(data);
                    Program.LogText += ("[" + DateTime.UtcNow + "] (SERVER): A game_msg packet was sent: " + str + "\n");

                    if (str.Contains("Server requesting that you re-logon"))
                    {
                        if (isBanned is true)
                        {
                            GamePacketProton variantPacket = new GamePacketProton();
                            variantPacket.AppendString("OnConsoleMessage");
                            variantPacket.AppendString("`4Sorry, this account (`&" + Program.globalUserData.tankIDName + "`4) has been suspended.");
                            packetSender.SendData(variantPacket.GetBytes(), Program.proxyPeer);
                            Program.realPeer.DisconnectNow(0);
                        }
                        else
                        {
                            Program.globalUserData.token = -1;
                            Program.globalUserData.Growtopia_IP = Program.globalUserData.Growtopia_Master_IP;
                            Program.globalUserData.Growtopia_Port = Program.globalUserData.Growtopia_Master_Port;
                            Program.globalUserData.isSwitchingServer = true;

                            Program.realPeer.Disconnect(0);
                        }
                    }

                    break;
                case NetTypes.NetMessages.GAME_PACKET:

                    byte[] tankPacket = VariantList.get_struct_data(data);
                    if (tankPacket == null) break;

                    byte tankPacketType = tankPacket[0];
                    NetTypes.PacketTypes packetType = (NetTypes.PacketTypes)tankPacketType;
                    if (Program.logallpackettypes)
                    {
                        GamePacketProton gp = new GamePacketProton();
                        gp.AppendString("OnConsoleMessage");
                        gp.AppendString("`1BUCKETWARE: `wPacket TYPE: `1" + tankPacketType.ToString());
                        packetSender.SendData(gp.GetBytes(), Program.proxyPeer);
                        if (tankPacketType > 18) File.WriteAllBytes("newpacket.dat", tankPacket);
                    }

                    switch (packetType)
                    {

                        case NetTypes.PacketTypes.PLAYER_LOGIC_UPDATE:
                            {
                                TankPacket p = TankPacket.UnpackFromPacket(data);
                                foreach (Player pl in worldMap.players)
                                {
                                    if (pl.netID == p.NetID)
                                    {
                                        pl.X = (int)p.X;
                                        pl.Y = (int)p.Y;
                                        break;
                                    }
                                }
                                break;
                            }
                        case NetTypes.PacketTypes.INVENTORY_STATE:
                            {
                                if (!Program.globalUserData.dontSerializeInventory)
                                    worldMap.player.SerializePlayerInventory(VariantList.get_extended_data(tankPacket));
                                break;
                            }
                        case NetTypes.PacketTypes.TILE_CHANGE_REQ:
                            {
                                TankPacket p = TankPacket.UnpackFromPacket(data);

                                if (worldMap == null)
                                {
                                    Program.LogText += ("[" + DateTime.UtcNow + "] (PROXY): (ERROR) World map was null." + "\n");
                                    break;
                                }
                                byte tileX = (byte)p.PunchX;
                                byte tileY = (byte)p.PunchY;
                                ushort item = (ushort)p.MainValue;

                                if (tileX >= worldMap.width) break;
                                else if (tileY >= worldMap.height) break;

                                ItemDatabase.ItemDefinition itemDef = ItemDatabase.GetItemDef(item);

                                if (ItemDatabase.isBackground(item))
                                {
                                    worldMap.tiles[tileX + (tileY * worldMap.width)].bg = item;
                                }
                                else
                                {
                                    worldMap.tiles[tileX + (tileY * worldMap.width)].fg = item;
                                }

                                break;
                            }
                        case NetTypes.PacketTypes.CALL_FUNCTION:
                            VariantList.VarList VarListFetched = VariantList.GetCall(VariantList.get_extended_data(tankPacket));
                            VarListFetched.netID = BitConverter.ToInt32(tankPacket, 4); // add netid
                            VarListFetched.delay = BitConverter.ToUInt32(tankPacket, 20); // add keep track of delay modifier

                            bool isABot = false;
                            Program.UserData ud = null;

                            int netID = OperateVariant(VarListFetched, isABot ? (object)peer : null); // box enetpeer obj to generic obj
                            string argText = string.Empty;

                            for (int i = 0; i < VarListFetched.functionArgs.Count(); i++)
                            {
                                argText += " [" + i.ToString() + "]: " + (string)VarListFetched.functionArgs[i].ToString();
                            }

                            Program.LogText += ("[" + DateTime.UtcNow + "] (SERVER): A function call was requested, see log infos below:\nFunction Name: " + VarListFetched.FunctionName + " parameters: " + argText + " \n");

                            if (VarListFetched.FunctionName == "OnSendToServer") return "Server switching forced, not continuing as Proxy Client has to deal with this.";
                            if (VarListFetched.FunctionName == "onShowCaptcha") return "Received captcha solving request, instantly bypassed it so it doesnt show up on client side.";
                            if (VarListFetched.FunctionName == "OnDialogRequest" && ((string)VarListFetched.functionArgs[1]).ToLower().Contains("captcha")) return "Received captcha solving request, instantly bypassed it so it doesnt show up on client side.";
                            //if (VarListFetched.FunctionName == "OnDialogRequest" && ((string)VarListFetched.functionArgs[1]).ToLower().Contains("gazette")) return "Received gazette, skipping it...";
                            if (VarListFetched.FunctionName == "OnDialogRequest" && ((string)VarListFetched.functionArgs[1]).ToLower().Contains("survey")) return "Received gazette, skipping it...";
                            if (VarListFetched.FunctionName == "OnSetPos" && Program.globalUserData.ignoreonsetpos && netID == worldMap.netID) return "Ignored position set by server, may corrupt doors but is used so it wont set back. (CAN BE BUGGY WITH SLOW CONNECTIONS)";
                            if (VarListFetched.FunctionName == "OnSpawn" && netID == -2)
                            {
                                if (Program.globalUserData.unlimitedZoom)
                                    return "Modified OnSpawn for unlimited zoom (mstate|1)"; // only doing unlimited zoom and not unlimited punch/place to be sure that no bans occur due to this. If you wish to use unlimited punching/placing as well, change the smstate in OperateVariant function instead.
                            }

                            break;
                        case NetTypes.PacketTypes.SET_CHARACTER_STATE:
                            {

                                /*TankPacket p = TankPacket.UnpackFromPacket(data);

                                return "Log of potentially wanted received GAME_PACKET Data:" +
                         "\nnetID: " + p.NetID +
                         "\nsecondnetid: " + p.SecondaryNetID +
                         "\ncharacterstate (prob 8): " + p.CharacterState +
                         "\nwaterspeed / offs 16: " + p.Padding +
                         "\nmainval: " + p.MainValue +
                         "\nX|Y: " + p.X + "|" + p.Y +
                         "\nXSpeed: " + p.XSpeed +
                         "\nYSpeed: " + p.YSpeed +
                         "\nSecondaryPadding: " + p.SecondaryPadding +
                         "\nPunchX|PunchY: " + p.PunchX + "|" + p.PunchY;*/
                                break;
                            }
                        case NetTypes.PacketTypes.PING_REQ:
                            SpoofedPingReply(TankPacket.UnpackFromPacket(data));
                            break;
                        case NetTypes.PacketTypes.LOAD_MAP:
                            if (Program.LogText.Length >= 32678) Program.LogText = string.Empty;

                            worldMap = worldMap.LoadMap(tankPacket);
                            worldMap.player.didCharacterStateLoad = false;
                            worldMap.player.didClothingLoad = false;

                            Program.realPeer.Timeout(1000, 2800, 3400);

                            break;
                        case NetTypes.PacketTypes.MODIFY_ITEM_OBJ:
                            {
                                TankPacket p = TankPacket.UnpackFromPacket(data);
                                if (p.NetID == -1)
                                {
                                    if (worldMap == null)
                                    {
                                        Program.LogText += ("[" + DateTime.UtcNow + "] (PROXY): (ERROR) World map was null." + "\n");
                                        break;
                                    }

                                    worldMap.dropped_ITEMUID++;

                                    DroppedObject dItem = new DroppedObject();
                                    dItem.id = p.MainValue;
                                    dItem.itemCount = data[16];
                                    dItem.x = p.X;
                                    dItem.y = p.Y;
                                    dItem.uid = worldMap.dropped_ITEMUID;
                                    worldMap.droppedItems.Add(dItem);
                                    if (Program.globalUserData.cheat_autocollect)
                                    {
                                        Utils utils = new Utils();
                                        TankPacket p2 = new TankPacket();
                                        Player playerObject = worldMap.player;
                                        float tilex = playerObject.X / 32;
                                        float tiley = playerObject.Y / 32;
                                        if (utils.isInside((int)p.X, (int)p.Y, Program.globalUserData.auto_range * 32, playerObject.X, playerObject.Y))
                                        {
                                            p2.PacketType = (int)NetTypes.PacketTypes.ITEM_ACTIVATE_OBJ;
                                            p2.NetID = p.NetID;
                                            p2.X = (int)p.X;
                                            p2.Y = (int)p.Y;
                                            p2.MainValue = dItem.uid;
                                            packetSender.SendPacketRaw((int)NetTypes.NetMessages.GAME_PACKET, p2.PackForSendingRaw(), Program.realPeer);
                                        }
                                    }
                                    if (Program.globalUserData.cheat_magplant)
                                    {
                                        Utils utils = new Utils();
                                        TankPacket p2 = new TankPacket();
                                        Player playerObject = worldMap.player;
                                        float tilex = playerObject.X / 32;
                                        float tiley = playerObject.Y / 32;
                                        p2.PacketType = (int)NetTypes.PacketTypes.ITEM_ACTIVATE_OBJ;
                                        p2.NetID = p.NetID;
                                        p2.X = (int)p.X;
                                        p2.Y = (int)p.Y;
                                        p2.MainValue = dItem.uid;
                                        packetSender.SendPacketRaw((int)NetTypes.NetMessages.GAME_PACKET, p2.PackForSendingRaw(), Program.realPeer);
                                    }
                                }
                            }
                            break;
                        default:
                            break;
                    }
                    break;
                case NetTypes.NetMessages.TRACK:
                    {
                        return "Track message:\n" + GetProperGenericText(data);
                        break;
                    }
                case NetTypes.NetMessages.LOG_REQ:
                case NetTypes.NetMessages.ERROR:
                    return "Blocked LOG_REQUEST/ERROR message from server";
                default:
                    //return "(SERVER): An unknown event occured. Message Type: " + msgType.ToString() + "\n";
                    break;

            }

            packetSender.SendData(data, Program.proxyPeer);
            if (msgType == NetTypes.NetMessages.GAME_PACKET && data[4] > 39) // customizable on which packets you wanna log, for speed im just gonna do this!
            {
                TankPacket p = TankPacket.UnpackFromPacket(data);
                uint extDataSize = BitConverter.ToUInt32(data, 56);
                byte[] actualData = data.Skip(4).Take(56).ToArray();
                byte[] extData = data.Skip(60).ToArray();

                string extDataStr = "";
                string extDataStrShort = "";
                string extDataString = Encoding.UTF8.GetString(extData);
                for (int i = 0; i < extDataSize; i++)
                {
                    //ushort pos = BitConverter.ToUInt16(extData, i);
                    extDataStr += extData[i].ToString() + "|";
                }

                return "Log of potentially wanted received GAME_PACKET Data:" +
                    "\npackettype: " + actualData[0].ToString() +
                    "\npadding byte 1|2|3: " + actualData[1].ToString() + "|" + actualData[2].ToString() + "|" + actualData[3].ToString() +
                    "\nnetID: " + p.NetID +
                    "\nsecondnetid: " + p.SecondaryNetID +
                    "\ncharacterstate (prob 8): " + p.CharacterState +
                    "\nwaterspeed / offs 16: " + p.Padding +
                    "\nmainval: " + p.MainValue +
                    "\nX|Y: " + p.X + "|" + p.Y +
                    "\nXSpeed: " + p.XSpeed +
                    "\nYSpeed: " + p.YSpeed +
                    "\nSecondaryPadding: " + p.SecondaryPadding +
                    "\nPunchX|PunchY: " + p.PunchX + "|" + p.PunchY +
                    "\nExtended Packet Data Length: " + extDataSize.ToString() +
                    "\nExtended Packet Data:\n" + extDataStr + "\n";
                return string.Empty;
            }
            else
            {
                return string.Empty;
            }
        }
    }
}
