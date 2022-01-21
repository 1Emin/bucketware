using ENet.Managed;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using Timer = System.Timers.Timer;
using Bucketware.View;
using System.Net.NetworkInformation;

namespace GrowbrewProxy
{
    public class Program
    {
        static string Version = "HTTP/1.0";
        static string sahyui1337 = "Discord Sahyui#1337 | Bucketware";
        private static Program mf;
        private static HttpListener listener = new HttpListener();
        public static void HTTPHandler()
        {
            while (listener.IsListening)
            {
                try
                {
                    AppendLog("Starting HTTP Client to Auto-get port and IP...");

                    string server_metadata = string.Empty;
                    using (WebClient client = new WebClient())
                    {
                        server_metadata = client.DownloadString("http://www.growtopia2.com/growtopia/server_data.php");
                        client.Dispose();
                    }

                    if (server_metadata != "")
                    {
#if DEBUG
                        AppendLog("Got response, server metadata:\n" + server_metadata);
#endif
                        Console.WriteLine("Parsing server metadata...");

                        string[] tokens = server_metadata.Split('\n');
                        foreach (string s in tokens)
                        {
                            if (s.Length <= 0) continue;
                            if (s[0] == '#') continue;
                            if (s.StartsWith("RTENDMARKERBS1001")) continue;
                            string key = s.Substring(0, s.IndexOf('|')).Replace("\n", "");
                            string value = s.Substring(s.IndexOf('|') + 1);

                            switch (key)
                            {
                                case "server":
                                    {
                                        // server ip

                                       globalUserData.Growtopia_IP = value.Substring(0, value.Length);
                                        break;
                                    }
                                case "port":
                                    {
                                        ushort portval = ushort.Parse(value);
                                        globalUserData.Growtopia_Port = portval;
                                        break;
                                    }
                                default:
                                    break;
                            }
                        }
                        //dont change this shits!!! | if you change its not work!!!
                        globalUserData.Growtopia_IP = globalUserData.Growtopia_Master_IP;
                        globalUserData.Growtopia_Master_Port = globalUserData.Growtopia_Port;
                        Console.WriteLine("Parsing done, detected IP:Port -> " + globalUserData.Growtopia_IP + ":" + globalUserData.Growtopia_Master_Port);
                        Console.WriteLine("\n\nDiscord : Sahyui#1337\n\nAutoPort working well now!");


                        //Spams trade
                        // AppendLog(globalUserData.Growtopia_IP + ":" + globalUserData.Growtopia_Master_Port);
                    }

                    HttpListenerContext context = listener.GetContext();
                    HttpListenerRequest request = context.Request;
                    HttpListenerResponse response = context.Response;
#if DEBUG
                    AppendLog("New request from client:\n" + request.RawUrl + " " + request.HttpMethod + " " + request.UserAgent);
#endif

                    if (request.HttpMethod == "POST")
                    {

                        byte[] buffer = Encoding.UTF8.GetBytes(
                            "server|127.0.0.1\n" +
                            "port|2\n" +
                            "type|1\n" +
                            "beta_server|127.0.0.1\n" +
                            "beta_port|2\n" +
                            "meta|bucketware.com\n" +
                            "type2|1\n");

                        response.ContentLength64 = buffer.Length;
                        System.IO.Stream output = response.OutputStream;
                        output.Write(buffer, 0, buffer.Length);
                        output.Close();
                        response.Close();
                    }
                }
                catch (Exception ex)
                {

                    Console.WriteLine(ex.Message);
                    Thread.Sleep(1000);
                    // probably cuz we stopped it, no need to worry.
                }
            }
        }

        public static void StartHTTP(string[] prefixes)
        {
            Console.WriteLine("Setting up HTTP Server...");
            if (!HttpListener.IsSupported)
            {
                Console.WriteLine("Windows XP SP2 or Server 2003 is required to use the HttpListener class.");
                return;
            }
            if (prefixes == null || prefixes.Length == 0)
                throw new ArgumentException("prefixes");

            foreach (string s in prefixes)
            {
                listener.Prefixes.Add(s);
            }

            listener.Start();
            if (listener.IsListening) Console.WriteLine("Listening!");
            else Console.WriteLine("Could not listen to port 80, an error occured!");
            Thread t = new Thread(HTTPHandler);
            t.Start();
            Console.WriteLine("HTTP Server is running.");
        }
        public static void StopHTTP()
        {
            if (listener == null) return;
            if (listener.IsListening) listener.Stop();
        }

        public static string textbox1 = "2";

        public class StateObject
        {
            // Client socket.  
            public Socket workSocket = null;
            // Size of receive buffer.  
            public const int BufferSize = 1024;
            // Receive buffer.  
            public byte[] buffer = new byte[BufferSize];
            // Received data string.  
            public StringBuilder sb = new StringBuilder();
        }

        public static byte GrowbrewHNetVersion = 1;
        static bool isHTTPRunning = false;

        public static bool multibottingEnabled = false;

        public static bool skipCache = false;
        public static bool logallpackettypes = false;

        public static TcpClient tClient = new TcpClient();
        public static StateObject stateObj = new StateObject();

        public static string LogText = string.Empty;

        private delegate void SafeCallDelegate(string text);
        private delegate void SafeCallDelegatePort(ushort port);
        public static ENetHost client;
        public static ENetHost m_Host;

        public static ENetPeer realPeer;
        public static ENetPeer proxyPeer;
#pragma warning disable CS0436 // Type conflicts with imported type
        // unnecessary as botting isnt made for open src anyway
#pragma warning restore CS0436 // Type conflicts with imported type

        public class UserData
        {
            public ulong connectIDReal = 0;
            public ulong connectID = 0;

            public bool didQuit = false;
            public bool mayContinue = false;
            public bool srvRunning = false;
            public int Growtopia_Port = 17210;
            public string Growtopia_IP = "213.179.209.168";
            public string Growtopia_Master_IP = "213.179.209.168";
            public int Growtopia_Master_Port = 17210;
            public bool clientRunning = false;
            /*  public int Growtopia_Port = 17196;
              public string Growtopia_IP = "213.179.209.168";
              public string Growtopia_Master_IP = "213.179.209.168";
              public int Growtopia_Master_Port = 17196;*/

            public bool isSwitchingServer = false;
            public bool blockEnterGame = false;
            public bool serializeWorldsAdvanced = true;
            public bool bypass10PlayerMax = true;
            public bool cheat_password = false;
            public bool actived = false;
            public int irok1 = 0;
            public int xrok1 = 1;
            public int irok2 = 1;
            public int xrok2 = 2;
            public int rok = 2;

            // internal variables =>
            public string tankIDName = "";
            public string tankIDPass = "";
            public string game_version = "4.20";
            public string country = "az";
            public string requestedName = "";
            public int token = 0;
            public bool resetStuffNextLogon = false;
            public int userID = -1;
            public int lmode = -1;
            public byte[] skinColor = new byte[4];
            public bool enableSilentReconnect = false;
            public bool hasLogonAlready = false;
            public bool hasUpdatedItemsAlready = false;
            public bool bypassAAP = false;
            public bool ghostSkin = false;
            public bool autojoin = false;
            public string autojoinworld = "sahyui1337";
            // CHEAT VARS/DEFS
            public int cheat_door_y;
            public int cheat_door_x;
            public bool cheat_selectdoor = false;
            public int auto_range = 3;
            public bool cheat_autocollect = false;
            public bool cheat_msgnew = false;
            public string cheat_msgnew_text = "`bdsc.gg/sahyui";
            public bool cheat_Banothers = false;
            public bool cheat_leavemod = false;
            public bool cheat_casinohack = false;
            public bool cheat_magplant = false;
            public bool cheat_rgbSkin = false;
            public bool cheat_autoworldban_mod = false;
            public bool cheat_speedy = false;
            public bool isAutofarming = false;
            public bool cheat_Autofarm_magplant_mode = false;
            public bool redDamageToBlock = false; // exploit discovered in servers at time of client being in version 3.36/3.37
                                                  // CHEAT VARS/DEFS
            public string macc = "02:15:01:20:30:05";
            public string doorid = "";
            public string rid = "", sid = "";

            public bool ignoreonsetpos = false;
            public bool unlimitedZoom = false;
            public bool isFacingSwapped = false;
            public bool blockCollecting = false;
            public short lastWrenchX = 0;
            public short lastWrenchY = 0;
            public bool awaitingReconnect = false;
            public bool enableAutoreconnect = false;
            public string autoEnterWorld = "";
            public bool dontSerializeInventory = false;
            public bool skipGazette = false;
        }
        public static ItemDatabase itemDB = new ItemDatabase();
        public static HandleMessages messageHandler = new HandleMessages();
        public static UserData globalUserData = new UserData();
        public static string GenerateRID()
        {
            string str = "0";
            Random random = new Random();
            const string chars = "ABCDEF0123456789";
            str += new string(Enumerable.Repeat(chars, 31)
               .Select(s => s[random.Next(s.Length)]).ToArray());
            return str;
        }

        private static Random random = new Random();
        private static System.Timers.Timer time2r;

        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static string GenerateUniqueWinKey()
        {
            string str = "7";
            Random random = new Random();
            const string chars = "ABCDEF0123456789";
            str += new string(Enumerable.Repeat(chars, 31)
               .Select(s => s[random.Next(s.Length)]).ToArray());
            return str;
        }

        public static string GenerateMACAddress()
        {
            var random = new Random();
            var buffer = new byte[6];
            random.NextBytes(buffer);
            var result = String.Concat(buffer.Select(x => string.Format("{0}:", x.ToString("X2"))).ToArray());
            return result.TrimEnd(':');
        }

        public static string CreateLogonPacket(string customGrowID = "", string customPass = "", int customUserID = -1, int customToken = -1, string doorID = "")
        {

            // this is kinda messy
            string gversion = globalUserData.game_version;
            string p = string.Empty;
            Random rand = new Random();
            bool requireAdditionalData = globalUserData.token > -1;

            if (customGrowID == "")
            {
                if (globalUserData.tankIDName != "")
                {
                    p += "tankIDName|" + (globalUserData.tankIDName + "\n");
                    p += "tankIDPass|" + (globalUserData.tankIDPass + "\n");
                }
            }
            else
            {
                //Console.WriteLine("CUSTOM GROWID IS : " + customGrowID);
                p += "tankIDName|" + (customGrowID + "\n");
                p += "tankIDPass|" + (customPass + "\n");
            }

            p += "requestedName|" + ("B431" + "\n"); //"Growbrew" + rand.Next(0, 255).ToString() + "\n"
            p += "f|1\n";
            p += "protocol|120\n";
            p += "game_version|" + (gversion + "\n");
            if (requireAdditionalData) p += "lmode|" + globalUserData.lmode + "\n";
            p += "cbits|128\n";
            p += "player_age|100\n";
            p += "GDPR|1\n";
            p += "hash2|" + rand.Next(-777777776, 777777776).ToString() + "\n";
            p += "meta|dsc.gg/sahyui\n"; // soon auto fetch meta etc.
            p += "fhash|-716928004\n";
            p += "platformID|0\n";
            p += "deviceVersion|0\n";
            p += "country|" + (globalUserData.country + "\n");
            p += "hash|" + rand.Next(-777777776, 777777776).ToString() + "\n";
            p += "mac|" + globalUserData.macc + "\n";
            p += ("rid|" + (globalUserData.rid == "" ? GenerateRID() : globalUserData.rid) + "\n");
            if (requireAdditionalData) p += "user|" + (globalUserData.userID.ToString() + "\n");
            if (requireAdditionalData) p += "token|" + (globalUserData.token.ToString() + "\n");
            if (customUserID > 0) p += "user|" + (customUserID.ToString() + "\n");
            if (customToken > 0) p += "token|" + (customToken.ToString() + "\n");
            if (globalUserData.doorid != "" && doorID == "") p += "doorID|" + globalUserData.doorid + "\n";
            else if (doorID != "") p += "doorID|" + doorID + "\n";
            p += ("wk|" + (globalUserData.sid == "" ? GenerateUniqueWinKey() : globalUserData.sid) + "\n");
            p += "fz|1331849031";
            Console.WriteLine(p);
            p += "zf|-1331849031";
            return p;
        }
        public static void AppendLog(string text)
        {
            Console.WriteLine(text);
        }
        public static void ConnectToServer(ref ENetPeer peer, UserData userData = null, bool FirstInitialUseOfBot = false)
        {
            Console.WriteLine("Internal proxy client is attempting a connection to server...");

            string ip = globalUserData.Growtopia_IP;
            int port = globalUserData.Growtopia_Port;

            if (peer == null)
            {
                peer = client.Connect(new IPEndPoint(IPAddress.Parse(ip), port), 2, 0);
            }
            else
            {
                if (peer.IsNull)
                {
                    peer = client.Connect(new IPEndPoint(IPAddress.Parse(ip), port), 2, 0);
                }
                else if (peer.State != ENetPeerState.Connected)
                {
                    peer = client.Connect(new IPEndPoint(IPAddress.Parse(ip), port), 2, 0);
                }
                else
                {

                    // peer = client.Connect(new IPEndPoint(IPAddress.Parse(ip), port), 2, 0);
                    globalUserData.awaitingReconnect = true;

                    //In this case, we will want the realPeer to be disconnected first 

                    // sub server switching, most likely.
                    peer = client.Connect(new IPEndPoint(IPAddress.Parse(ip), port), 2, 0);
                }
            }
        }

        public static void Host_OnConnect(ENetPeer peer)
        {

            proxyPeer = peer;
            AppendLog("Connecting to gt servers at " + globalUserData.Growtopia_IP + ":" + globalUserData.Growtopia_Port + "...");
            globalUserData.connectID++;
            ConnectToServer(ref Program.realPeer);

        }

        public static void Peer_OnDisconnect(object sender, uint e)
        {
            ENetPeer peer = (ENetPeer)sender;
            if (globalUserData.isSwitchingServer)
            {
                globalUserData.isSwitchingServer = false;
                GamePacketProton variantPacket = new GamePacketProton();
                variantPacket = new GamePacketProton();
                variantPacket.delay = 0; //Avoid too quick connection and give headroom for enetcommand to prevent random/rare freezing (fix by Toxic Vampor)
                variantPacket.NetID = -1;
                variantPacket.AppendString("OnSendToServer");
                variantPacket.AppendInt(2);
                variantPacket.AppendInt(globalUserData.token);
                variantPacket.AppendInt(globalUserData.userID);
                variantPacket.AppendString("127.0.0.1|" + globalUserData.doorid);
                variantPacket.AppendInt(globalUserData.lmode);

                messageHandler.packetSender.SendData(variantPacket.GetBytes(), Program.proxyPeer);
                return;
            }

            if (globalUserData.enableSilentReconnect)
            {
                unsafe
                {
                    if (((ENetPeer)sender).GetNativePointer()->ConnectID != realPeer.GetNativePointer()->ConnectID) return;
                }

                try
                {
                    realPeer.Send(0, new byte[60], ENetPacketFlags.Reliable);
                }
                catch
                {

                    if (proxyPeer != null)
                    {
                        if (proxyPeer.State == ENetPeerState.Connected)
                        {
                            GamePacketProton variantPacket = new GamePacketProton();
                            variantPacket.AppendString("OnConsoleMessage");
                            variantPacket.AppendString("`e[BUCKETWARE SILENT RECONNECT]: `wBUCKETWARE detected an unexpected disconnection, silently reconnecting...``");
                            messageHandler.packetSender.SendData(variantPacket.GetBytes(), Program.proxyPeer);
                        }
                    }
                }

                // ConnectToServer(useRealPeer ? ref realPeer : ref peer);

                ConnectToServer(ref realPeer);
            }
            else if (globalUserData.enableAutoreconnect)
            {
                unsafe
                {
                    if (((ENetPeer)sender).GetNativePointer()->ConnectID != realPeer.GetNativePointer()->ConnectID) return;
                }

                try
                {
                    realPeer.Send(0, new byte[60], ENetPacketFlags.Reliable);
                }
                catch
                {

                    if (proxyPeer != null)
                    {
                        if (proxyPeer.State == ENetPeerState.Connected)
                        {
                            GamePacketProton variantPacket2 = new GamePacketProton();
                            variantPacket2.AppendString("OnReconnect");
                            messageHandler.packetSender.SendData(variantPacket2.GetBytes(), Program.proxyPeer);
                        }
                    }
                }
            }
            messageHandler.enteredGame = false;

            AppendLog("An internal disconnection was triggered in the proxy, you may want to reconnect your GT Client if you are not being disconnected by default (maybe because of sub-server switching?)");
            proxyPeer.DisconnectNow(0);
        }

        public static void Peer_OnReceive(object sender, ENetPacket e)
        {
            ENetPeer peer = (ENetPeer)sender;
            if (peer.IsNull) AppendLog("Attention peer is null!! (Peer_OnReceive)");
            string str = messageHandler.HandlePacketFromClient(ref peer, e);
            if (str != "_none_" && str != "") AppendLog(str);
        }

        public static void Peer_OnReceive_Client(object sender, ENetPacket e)
        {

            ENetPeer peer = (ENetPeer)sender;
            if (peer.IsNull) AppendLog("Attention peer is null!! (Peer_OnReceive_Client)");
            string str = messageHandler.HandlePacketFromServer(ref peer, e);
            if (str != "_none_" && str != "") AppendLog(str);
        }
        public static void Client_OnConnect(ENetPeer peer)
        {
            AppendLog("The growtopia client just connected successfully.");
            peer.Timeout(1000, 4000, 6000);
            realPeer = peer;
            globalUserData.connectIDReal++;
        }

        public static void doServerService(int delay = 0)
        {
            doClientService(0);
            var Event = m_Host.Service(TimeSpan.FromMilliseconds(delay));

            switch (Event.Type)
            {
                case ENetEventType.None:

                    break;
                case ENetEventType.Connect:
                    Host_OnConnect(Event.Peer);
                    break;
                case ENetEventType.Disconnect:

                    break;
                case ENetEventType.Receive:

                    Peer_OnReceive(Event.Peer, Event.Packet);

                    Event.Packet.Destroy();
                    break;
                default:
                    throw new NotImplementedException();
            }
            doClientService(0);
        }

        public static void doClientService(int delay = 0)
        {
            if (client == null) return;
            if (client.Disposed) return;
            var Event = client.Service(TimeSpan.FromMilliseconds(delay));

            unsafe
            {
                switch (Event.Type)
                {
                    case ENetEventType.None:

                        break;
                    case ENetEventType.Connect:
                        Client_OnConnect(Event.Peer);
                        break;
                    case ENetEventType.Disconnect:
                        Peer_OnDisconnect(Event.Peer, 0);
                        Event.Peer.UnsetUserData();
                        break;
                    case ENetEventType.Receive:

                        Peer_OnReceive_Client(Event.Peer, Event.Packet);
                        Event.Packet.Destroy();
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
        }

        public static void doClientServerServiceLoop()
        {
            while (true)
            {
                doClientService(1);
                doServerService(3);
            }
        }

        public static void doProxy()
        {
            if (client == null || m_Host == null)
            {
                return;
            }

            Task.Run(() => doClientServerServiceLoop());
        }

        public static void LaunchProxy()
        {
            if (!globalUserData.srvRunning)
            {
                globalUserData.srvRunning = true;
                globalUserData.clientRunning = true;

                // Setting up ENet-Server ->

                m_Host = new ENetHost(new IPEndPoint(IPAddress.Any, 2), 32, 10, 0, 0);
                m_Host.ChecksumWithCRC32();
                m_Host.CompressWithRangeCoder();
                m_Host.EnableNewProtocol(2);

                // Setting up ENet-Client ->
                client = new ENetHost(null, 64, 10); // for multibotting, coming soon.
                client.ChecksumWithCRC32();
                client.CompressWithRangeCoder();
                client.EnableNewProtocol(1);

                // realPeer = client.Connect(new IPEndPoint(IPAddress.Parse(globalUserData.Growtopia_Master_IP), globalUserData.Growtopia_Master_Port), 2, 0);
                //realPeer = client.Connect(new IPEndPoint(IPAddress.Parse(globalUserData.Growtopia_Master_IP), globalUserData.Growtopia_Master_Port), 2, 0);
                doProxy();
            }
        }
        public static uint HashBytes(byte[] b) // Thanks to iProgramInCpp !
        {
            byte[] n = b;
            uint acc = 0x55555555;

            for (int i = 0; i < b.Length; i++)
            {
                acc = (acc >> 27) + (acc << 5) + n[i];
            }
            return acc;
        }
        public static void SetCustomGS(int speed, int gravity)
        {
            globalUserData.cheat_speedy = true;

            TankPacket p = new TankPacket();
            p.PacketType = (int)NetTypes.PacketTypes.SET_CHARACTER_STATE;
            p.X = 1000;
            p.Y = 300;
            p.YSpeed = gravity;
            p.NetID = messageHandler.worldMap.netID;
            p.XSpeed = speed;
            byte[] data = p.PackForSendingRaw();
            Buffer.BlockCopy(BitConverter.GetBytes(8487168), 0, data, 1, 3);
            messageHandler.packetSender.SendPacketRaw((int)NetTypes.NetMessages.GAME_PACKET, data, proxyPeer);
        }
        public static void doDropAllInventory()
        {
            Inventory inventory = messageHandler.worldMap.player.inventory;

            if (inventory.items == null)
            {
                Console.WriteLine("inventory.items was null!");
                return;
            }

            int ctr = 0;
            bool swap = false;

            foreach (InventoryItem item in inventory.items)
            {
                ctr++;

                TankPacket tp = new TankPacket();
                tp.XSpeed = 32;

                if ((ctr % 24) == 0)
                {
                    swap = !swap;
                    Thread.Sleep(400);
                    if (swap)
                    {
                        if (globalUserData.isFacingSwapped) messageHandler.worldMap.player.X -= 32;
                        else messageHandler.worldMap.player.X += 32;
                    }
                    else
                    {
                        if (globalUserData.isFacingSwapped) messageHandler.worldMap.player.X += 32;
                        else messageHandler.worldMap.player.X -= 32;
                    }
                }

                tp.X = messageHandler.worldMap.player.X;
                tp.Y = messageHandler.worldMap.player.Y;

                messageHandler.packetSender.SendPacketRaw(4, tp.PackForSendingRaw(), realPeer);

                messageHandler.packetSender.SendPacket(2, "action|drop\nitemID|" + item.itemID.ToString() + "|\n", realPeer);
                // Console.WriteLine($"Dropping item with ID: {item.itemID} with amount: {item.amount}");
                string str = "action|dialog_return\n" +
                    "dialog_name|drop_item\n" +
                    "itemID|" + item.itemID.ToString() + "|\n" +
                    "count|" + item.amount.ToString() + "\n";

                messageHandler.packetSender.SendPacket(2, str, realPeer);
            }
        }
        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        public const int SW_HIDE = 0;
        public const int SW_SHOW = 5;
        public static Timer newTimer = new Timer();
        public static Timer newTimer2 = new Timer();
        public static Timer newTimer3 = new Timer();
        public static Timer newTimer4 = new Timer();
        public static Timer newTimer5 = new Timer();
        public static void LoadProxy()
        {
            var startupOptions = new ENetStartupOptions()
            {
                ModulePath = Application.StartupPath + "\\enet.dll"
            };
            ManagedENet.Startup(startupOptions);
            ManagedENet.Shutdown(delete: false);
            newTimer.Elapsed += new ElapsedEventHandler(DisplayTimeEvent);
            newTimer.Interval = 4;
            newTimer.Start();
            newTimer2.Elapsed += new ElapsedEventHandler(DisplayTimeEvent2);
            newTimer2.Interval = 1000;
            newTimer2.Start();
            newTimer3.Elapsed += new ElapsedEventHandler(DisplayTimeEvent3);
            newTimer3.Interval = 500;
            newTimer4.Elapsed += new ElapsedEventHandler(DisplayTimeEvent4);
            newTimer4.Interval = 1500;
            newTimer4.Start();
            newTimer5.Elapsed += new ElapsedEventHandler(DisplayTimeEvent5);
            newTimer5.Interval = 400;
            newTimer5.Start();
            globalUserData.macc = GenerateMACAddress();
            if (!Directory.Exists("stored"))
                Directory.CreateDirectory("stored");
            itemDB.SetupItemDefs();
        }
        public static void Start()
        {
            LaunchProxy();
            isHTTPRunning = !isHTTPRunning;
            if (isHTTPRunning)
            {
                string[] arr = new string[1];
                arr[0] = "http://*:80/";
                StartHTTP(arr);
            }
        }

        public static void doAutofarm(int itemID, bool remote_mode = false, bool selfblockstart = false)
        {
            bool oneblockmode = Bucket.oneblock;
            bool isBg = ItemDatabase.isBackground(itemID);
            while (globalUserData.isAutofarming)
            {
                Thread.Sleep(10);

                if (realPeer != null)
                {
                    if (realPeer.State != ENetPeerState.Connected)
                    {
                        Thread.Sleep(100);
                        continue;
                    }

                    int c = globalUserData.rok - (oneblockmode ? 1 : 0);
                    //c = 3;

                    for (int i = 0; i < c; i++)
                    {
                        int x, y;
                        x = messageHandler.worldMap.player.X / 32;
                        y = messageHandler.worldMap.player.Y / 32;

                        int e1 = globalUserData.irok1;
                        int o1 = globalUserData.xrok1;
                        int e2 = globalUserData.irok2;
                        int o2 = globalUserData.xrok2;

                        if (globalUserData.isFacingSwapped)
                        {
                            if (i == e1) x -= o1;
                            if (i == e2) x -= o2;
                            if (selfblockstart) x++;
                        }
                        else
                        {
                            if (i == e1) x += o1;
                            if (i == e2) x += o2;
                            if (selfblockstart) x--;
                        }

                        Thread.Sleep(166);

                        if (messageHandler.worldMap == null)
                        {
                            Thread.Sleep(100);
                            continue;
                        }

                        TankPacket tkPt = new TankPacket();
                        tkPt.PunchX = x;
                        tkPt.PunchY = y;

                        ushort fg = messageHandler.worldMap.tiles[x + (y * messageHandler.worldMap.width)].fg;
                        ushort bg = messageHandler.worldMap.tiles[x + (y * messageHandler.worldMap.width)].bg;

                        if (isBg)
                        {
                            if (bg != 0)
                            {
                                tkPt.MainValue = 18;
                            }
                            else
                            {
                                tkPt.MainValue = itemID;
                            }
                        }
                        else
                        {
                            if (fg == itemID)
                            {
                                tkPt.MainValue = 18;
                            }
                            else
                            {
                                tkPt.MainValue = itemID;
                            }
                        }

                        if (remote_mode && tkPt.MainValue != 18) tkPt.MainValue = 5640;

                        tkPt.X = messageHandler.worldMap.player.X;
                        tkPt.Y = messageHandler.worldMap.player.Y;
                        tkPt.ExtDataMask &= ~0x04;
                        tkPt.ExtDataMask &= ~0x40;
                        tkPt.ExtDataMask &= ~0x10000;
                        tkPt.NetID = -1;

                        // TODO THREAD SAFETY
                        //messageHandler.packetSender.SendPacketRaw(4, tkPt.PackForSendingRaw(), realPeer); no need for this
                        tkPt.NetID = -1;
                        tkPt.PacketType = 3;
                        tkPt.ExtDataMask = 0;
                        messageHandler.packetSender.SendPacketRaw(4, tkPt.PackForSendingRaw(), realPeer);

                    }
                }
            }
        }
        public static void Restart()
        {
            Stop();
            Thread.Sleep(500);
            Start();
        }
        public static void Stop()
        {
            try
            {
                if (realPeer != null)
                {
                    if (realPeer.State == ENetPeerState.Connected)
                        realPeer.Reset();
                }
            }
            catch
            {

            }
        }
        public static int GetTileX()
        {
            Player playerObject = messageHandler.worldMap.player;
            return playerObject.X / 32;
        }
        public static int GetTileY()
        {
            Player playerObject = messageHandler.worldMap.player;
            return playerObject.Y / 32;
        }
        public static void DisplayTimeEvent(object source, ElapsedEventArgs e)
        {
            //messageHandler.packetSender.SendPacket(3, "action|log\n|msg|`1Timer", Program.proxyPeer);
            int x;
            int y;
            if (messageHandler.worldMap != null) // checking if we have it setup
            {
                Player playerObject = messageHandler.worldMap.player;
                x = playerObject.X;
                y = playerObject.Y;
                GamePacketProton variantPacket = new GamePacketProton();
                variantPacket.AppendString("OnNameChanged");
                variantPacket.AppendString("`b" + globalUserData.tankIDName + " X:" + x / 32 + " Y:" + y / 32 + "``");
                variantPacket.NetID = messageHandler.worldMap.netID;
                messageHandler.packetSender.SendData(variantPacket.GetBytes(), proxyPeer);
            }

        }
        public static bool Spam = false;
        public static void Spammer(string text)
        {
            while (Spam is true)
            {
                int inter = Bucket.TimerInter;
                messageHandler.packetSender.SendPacket(2, "action|input\n|text|" + text, Program.realPeer);
                Thread.Sleep(inter);
            }
        }
        public static void CheckSpam()
        {
            while (messageHandler.isSpamDect is true)
            {
                if (Bucket.isSpam is true)
                {
                    Thread.Sleep(10000);
                    messageHandler.packetSender.SendPacket(3, "action|log\n|msg|`1Spam continues", Program.proxyPeer);
                    Spam = true;
                    messageHandler.isSpamDect = false;
                    Task.Run(() => Spammer(Bucket.SpamText));
                };
            }
        }
        public static bool Emote = false;
        public static void EmoteSpammer()
        {
            while (Emote is true)
            {
                int inter = Bucket.TimerInter;
                messageHandler.packetSender.SendPacket(2, "action|input\n|text|/troll", Program.realPeer);
                Thread.Sleep(inter);
                messageHandler.packetSender.SendPacket(2, "action|input\n|text|/omg", Program.realPeer);
                Thread.Sleep(inter);
                messageHandler.packetSender.SendPacket(2, "action|input\n|text|/yes", Program.realPeer);
                Thread.Sleep(inter);
                messageHandler.packetSender.SendPacket(2, "action|input\n|text|/cheer", Program.realPeer);
                Thread.Sleep(inter);
                messageHandler.packetSender.SendPacket(2, "action|input\n|text|/no", Program.realPeer);
                Thread.Sleep(inter);
                messageHandler.packetSender.SendPacket(2, "action|input\n|text|/troll", Program.realPeer);
                Thread.Sleep(inter);
                messageHandler.packetSender.SendPacket(2, "action|input\n|text|/furious", Program.realPeer);
                Thread.Sleep(inter);
                messageHandler.packetSender.SendPacket(2, "action|input\n|text|/kiss", Program.realPeer);
                Thread.Sleep(inter);
                messageHandler.packetSender.SendPacket(2, "action|input\n|text|/wave", Program.realPeer);
                Thread.Sleep(inter);
            }
        }
        public void SendMessage(string message)
        {
            messageHandler.packetSender.SendPacket(2, "action|input\n|text|" + message, Program.realPeer);
        }
        public static void DisplayTimeEvent2(object source, ElapsedEventArgs e)
        {
            if (HandleMessages.SkinChanger is true)
            {
                messageHandler.packetSender.SendPacket(2, "action|setSkin\n|color|1348237567", Program.realPeer);
                newTimer3.Start();
                newTimer2.Stop();
            }
        }
        public static void DisplayTimeEvent3(object source, ElapsedEventArgs e)
        {
            if (HandleMessages.SkinChanger is true)
            {
                messageHandler.packetSender.SendPacket(2, "action|setSkin\n|color|1345519520", Program.realPeer);
                newTimer2.Start();
                newTimer3.Stop();
            }
        }
        public static bool TradeAll = false;
        public static void DisplayTimeEvent4(object source, ElapsedEventArgs e)
        {
            if (TradeAll is true)
            {
                foreach (Player p in messageHandler.worldMap.players)
                {
                    string pName = p.name.Substring(2);
                    pName = pName.Substring(0, pName.Length - 2);
                    messageHandler.packetSender.SendPacket((int)NetTypes.NetMessages.GENERIC_TEXT, "action|input\n|text|/trade " + pName, Program.realPeer); //Spams trade
                }
            }
        }
        public static bool hs = false;
        public static void DisplayTimeEvent5(object source, ElapsedEventArgs e)
        {
            if (hs is true)
            {
                messageHandler.packetSender.SendPacket(2, "action|input\n|text|/hidestatus", Program.realPeer);//makes weird bounce to name if have super supporter
            }
        }
    }
}
