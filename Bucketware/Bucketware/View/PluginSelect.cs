using Bucketware.PluginSystem;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Bucketware.View
{
    public partial class PluginSelect : UserControl
    {
        public PluginSelect()
        {
            InitializeComponent();
        }
        public int index { get; set; }
        public string PluginName
        {
            get
            {
                return this.label1.ToString();
            }
            set
            {
                this.label1.Text = value;
            }
        }
        public string Description
        {
            get
            {
                return this.label2.ToString();
            }
            set
            {
                this.label2.Text = value;
            }
        }
        public string CreatedBy
        {
            get
            {
                return this.label3.ToString();
            }
            set
            {
                this.label3.Text = value;
            }
        }
        private void PluginSelect_Load(object sender, EventArgs e)
        {

        }

        private void guna2Button15_Click(object sender, EventArgs e)
        {
            PluginHandler ph = new PluginHandler();
            ph.SePlugin(index);
            ph.PluginLoadUI(index);
        }
    }
}
