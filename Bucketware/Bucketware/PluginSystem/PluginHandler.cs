using Bucketware.View;
using Bucketware.SDK_2;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace Bucketware.PluginSystem
{
    public class PluginInformation
    {
        public string pluginName { get; set; }
        public string description { get; set; }
        public string createdBy { get; set; }

        public PluginInformation(string pluginName, string description, string createdBy)
        {
            this.pluginName = pluginName;
            this.description = description;
            this.createdBy = createdBy;
        }
    }
    public class PluginHandler
    {
        public static int bundleIdentifier = 4;
        public static int index = 0;
        public static bool isSelectPlugin = false;
        /*public static List<PluginInformation> pluginInformations = new List<PluginInformation>
        {
            //new PluginInformation("CustomFakeBan 1.0", "Make your own ban", "Created By Fyrax"),    
            //new PluginInformation("WarpPlugin", "Warp worlds without supporter", "Created By Zenix"),
            //new PluginInformation("OpenRender", "Opens world in browser", "Created By Zenix")
        };*/

        public void SePlugin(int serverIndex)
        {
            index = serverIndex;
            isSelectPlugin = true;
        }
        public void UILOAD()
        {
            _plugins = ReadExtensions();
            var plugin = _plugins[index];
            PluginWin.Show(plugin.Title, plugin.button1, plugin.button2, plugin.button3, plugin.button4, plugin.button5, plugin.textbox1, plugin.textbox2, plugin.checkbox1, plugin.checkbox2, plugin.button1text, plugin.button2text, plugin.button3text, plugin.button4text, plugin.button5text, plugin.textbox1placehold, plugin.textbox2placehold, plugin.checkbox1text, plugin.checkbox2text);
        }
        public void PluginLoadUI(int index)
        {
            _plugins = ReadExtensions();
            var plug = _plugins[index];
            plug.LoadUI();
        }

        public void button1Click()
        {
            _plugins = ReadExtensions();
            var plugin = _plugins[index];
            plugin.button1Click();
        }
        public void button2Click()
        {
            _plugins = ReadExtensions();
            var plugin = _plugins[index];
            plugin.button2Click();
        }
        public void button3Click()
        {
            _plugins = ReadExtensions();
            var plugin = _plugins[index];
            plugin.button3Click();
        }
        public void button4Click()
        {
            _plugins = ReadExtensions();
            var plugin = _plugins[index];
            plugin.button4Click();
        }
        public void button5Click()
        {
            _plugins = ReadExtensions();
            var plugin = _plugins[index];
            plugin.button5Click();
        }
        public void checkbox1Click()
        {
            _plugins = ReadExtensions();
            var plugin = _plugins[index];
            plugin.checkbox1Click();
        }
        public void checkbox2Click()
        {
            _plugins = ReadExtensions();
            var plugin = _plugins[index];
            plugin.checkbox2Click();
        }
        public static List<PluginInformation> pluginInformations = new List<PluginInformation>();
        public void LoadPlugins()
        {
            _plugins = ReadExtensions();
            Console.WriteLine($"{_plugins.Count} plugin(s) found");
            for (int i = 0; i < _plugins.Count; i++)
            {
                var plug2 = _plugins[i];
                pluginInformations.Add(new PluginInformation(plug2.Name, plug2.Description, plug2.Creator));
            }
            Console.WriteLine(pluginInformations.Count);
            // Print 
            /*for (int i = 0; i < _plugins.Count; i++)
                Console.WriteLine(_plugins[0]);
            var plug = _plugins[0];*/
            //Console.WriteLine($"{plug.Title} | {plug.Description}");
            /*foreach (var plugin in _plugins)
            {
                //Console.WriteLine($"{plugin.Title} | {plugin.Description}");
                //List<string> TrustedPlugins = new List<string>(new string[] { "CustomFakeBan" });
                //bool isTrusted = TrustedPlugins.Any(s => plugin.Title.Contains(s));
            }*/
            //Console.WriteLine("-----------------------");

            foreach (var plugin in _plugins)
            {
                plugin.LoadPlugin();
            }
        }
        public void RunPlugin()
        {

        }
        public List<IPlugin> _plugins = null;

        public List<IPlugin> ReadExtensions()
        {
            var pluginsLists = new List<IPlugin>();
            var files = Directory.GetFiles(Application.StartupPath + @"\extensions", "*.dll");

            foreach (var file in files)
            {
                var assembly = Assembly.LoadFile(Path.Combine(Directory.GetCurrentDirectory(), file));

                var pluginTypes = assembly.GetTypes().Where(t => typeof(IPlugin).IsAssignableFrom(t) && !t.IsInterface).ToArray();

                foreach (var pluginType in pluginTypes)
                {
                    var pluginInstance = Activator.CreateInstance(pluginType) as IPlugin;
                    pluginsLists.Add(pluginInstance);
                }
            }

            return pluginsLists;
        }
    }
}