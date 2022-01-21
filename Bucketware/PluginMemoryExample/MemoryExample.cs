using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bucketware.SDK_2;
// build plugin as x64
// build plugin as x64
// build plugin as x64
// build plugin as x64
namespace PluginMemoryExample
{
    public class MemoryExample : IPlugin
    {
        public string Title => "MemoryExample";
        public string Description => "Change GT Memory";
        public string Name => "MemoryExample";
        public string Version => "1.0";
        public string Creator => "Example";

        PHandler ph = new PHandler();
        public void LoadPlugin()
        {
            //Add here anything you want your plugin do when it loads
            Console.WriteLine("Loaded Example");
        }
        public void LoadUI()
        {
            ph.ShowGUI();
        }
        //When you set e.g button2 to false its not visible in ui
        //UI
        public bool button1 => false;
        public bool button2 => false;
        public bool button3 => false;
        public bool button4 => false;
        public bool button5 => false;
        public bool textbox1 => false;
        public bool textbox2 => false;
        public bool checkbox1 => true;
        public bool checkbox2 => true;
        public string button1text => "Dog";
        public string button2text => "Dog2";
        public string button3text => "Dog3";
        public string button4text => "Dog4";
        public string button5text => "Dog5";
        public string textbox1placehold => "Fakeban text";
        public string textbox2placehold => "dg";
        public string checkbox1text => "Example";
        public string checkbox2text => "dog2";

        public void button1Click()
        {
            //Do nothing we dont need this in this example
        }
        public void button2Click()
        {
            //Do nothing we dont need this in this example
        }
        public void button3Click()
        {
            //Do nothing we dont need this in this example
        }
        public void button4Click()
        {
            //Do nothing we dont need this in this example
        }
        public void button5Click()
        {
            //Do nothing we dont need this in this example
        }
        public bool check1 = false;
        public void checkbox1Click()
        {
            check1 = !check1;
            if (check1 is true)
            {
                ph.WriteMemory("Growtopia.exe+Address", "bytes", "90 90");
            }
            else
            {
                ph.WriteMemory("Growtopia.exe+Address", "bytes", "74 0F");
            }
        }
        public void checkbox2Click()
        {
            //Do nothing we dont need this in this example
        }
    }
}
