using Guna.UI2.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Bucketware.SDK_2;
using Bucketware.PluginSystem;

namespace Bucketware.View
{
    public partial class PluginWin : Form
    {
        public PluginWin(string title, bool button1, bool button2, bool button3, bool button4, bool button5, bool textbox1, bool textbox2, bool checkbox1, bool checkbox2, string button1text, string button2text, string button3text, string button4text, string button5text, string textbox1place, string textbox2place, string checkbox1text, string checkbox2text)
        {
            FormCollection fc = Application.OpenForms;
            InitializeComponent();
            /*if (Application.OpenForms.OfType<PluginWin>().Count() == 1)
                Application.OpenForms.OfType<PluginWin>().First().Close();*/
            /*int y = winSizeY;
            if (String.IsNullOrEmpty(y.ToString()))
            {
                y = 300;//Bucketware sdk2.0
            }
            this.Size = new Size(546, y);*/
            winTitle.Text = title;
            guna2Button1.Visible = button1;
            guna2Button2.Visible = button2;
            guna2Button3.Visible = button3;
            guna2Button4.Visible = button4;
            guna2Button5.Visible = button5;
            guna2TextBox1.Visible = textbox1;
            guna2TextBox2.Visible = textbox2;
            guna2CustomCheckBox1.Visible = checkbox1;
            label31.Visible = checkbox1;
            guna2CustomCheckBox2.Visible = checkbox2;
            label1.Visible = checkbox2;
            guna2Button1.Text = button1text;
            guna2Button2.Text = button2text;
            guna2Button3.Text = button3text;
            guna2Button4.Text = button4text;
            guna2Button5.Text = button5text;
            guna2TextBox1.PlaceholderText = textbox1place;
            guna2TextBox2.PlaceholderText = textbox2place;
            label31.Text = checkbox1text;
            label1.Text = checkbox2text;
        }
        public static void Show(string title, bool button1, bool button2, bool button3, bool button4, bool button5, bool textbox1, bool textbox2, bool checkbox1, bool checkbox2, string button1text, string button2text, string button3text, string button4text, string button5text, string textbox1place, string textbox2place, string checkbox1text, string checkbox2text)
        {
            new PluginWin(title, button1, button2, button3, button4, button5, textbox1, textbox2, checkbox1, checkbox2, button1text, button2text, button3text, button4text, button5text, textbox1place, textbox2place, checkbox1text, checkbox2text).Show();
        }

        private void label3_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void guna2Panel3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void guna2TextBox1_TextChanged(object sender, EventArgs e)
        {
            GrowbrewProxy.Program.textbox1 = guna2TextBox1.Text;
        }
        Configure cfg = new Configure();
        PluginHandler ph = new PluginHandler();

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            ph.button1Click();
        }

        private void guna2Button2_Click(object sender, EventArgs e)
        {
            ph.button2Click();
        }

        private void guna2Button3_Click(object sender, EventArgs e)
        {
            ph.button3Click();
        }

        private void guna2Button4_Click(object sender, EventArgs e)
        {
            ph.button4Click();
        }

        private void guna2Button5_Click(object sender, EventArgs e)
        {
            ph.button5Click();
        }

        private void guna2CustomCheckBox1_Click(object sender, EventArgs e)
        {
            ph.checkbox1Click();
        }

        private void guna2CustomCheckBox2_Click(object sender, EventArgs e)
        {
            ph.checkbox2Click();
        }
    }
}
