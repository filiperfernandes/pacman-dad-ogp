using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace pacman
{
    public partial class MainWindow : Form
    {
        public MainWindow()
        {
            InitializeComponent();
            output_box.Text = "";
            output_box.ReadOnly = true;
            file_box.ReadOnly = true;
        }

        
        
        private void MainWindow_Load_1(object sender, EventArgs e)
        {
           // AllocConsole();
        }

        private void btn_crash_Click(object sender, EventArgs e)
        {
            PuppetMaster.cmdCrash(input_box.Text,0);
        }

        private void btn_freeze_Click(object sender, EventArgs e)
        {
            PuppetMaster.cmdFreeze(input_box.Text, 0);
        }
        
        private void btn_wait_Click(object sender, EventArgs e)
        {
            PuppetMaster.cmdWait(Int32.Parse(input_box.Text),0);
        }

       

        private void btn_load_Click(object sender, EventArgs e)
        {
            i = 0;
            string[] lines = System.IO.File.ReadAllLines(input_box.Text);

            foreach (string line in lines)
            {
                file_box.Text += line + "\r\n";
                
            }

        }

       
        private void btn_start_server_Click(object sender, EventArgs e)
        {
            String[] words = PuppetMaster.splitInputBox(input_box.Text);


            PuppetMaster.cmdStartServer(words[0], words[1], words[2], Int32.Parse(words[3]), Int32.Parse(words[4]),0);
        }

        private void btn_start_client_Click(object sender, EventArgs e)
        {
            string input = input_box.Text;
            char[] delimiterChars = { ' ', '\t' };


            string[] words = input.Split(delimiterChars);

            string path = "";
            //printPM(words.Length.ToString(), 0);
            if (words.Length > 6) { path = words[6]; }
            PuppetMaster.cmdStartClient(words[0], words[1], words[2], Int32.Parse(words[3]), Int32.Parse(words[4]), path, 0);
        }
        
        private void btn_unfreeze_Click(object sender, EventArgs e)
        {
            PuppetMaster.cmdUnfreeze(input_box.Text, 0);
        }

        private void btn_run_command_Click(object sender, EventArgs e)
        {
            string commands = file_box.Text;

            string[] delimiterChars = new string[] { "\r\n" };
            string[] lines = commands.Split(delimiterChars, StringSplitOptions.None);

            foreach (String line in lines)
            {

                PuppetMaster.readConsole(line,0);
                System.Threading.Thread.Sleep(1000);
            }

        }
        static int i = 0;
        private void btn_next_command_Click(object sender, EventArgs e)
        {
            string commands = file_box.Text;

            string[] delimiterChars = new string[] { "\r\n" };
            string[] lines = commands.Split(delimiterChars, StringSplitOptions.None);

            
            PuppetMaster.readConsole(lines[i],0);
            i++;

        }

        private void btn_delay_Click(object sender, EventArgs e)
        {

        }

        private void btn_localstate_Click(object sender, EventArgs e)
        {

        }
        

        private void btn_globalstate_Click(object sender, EventArgs e)
        {
            PuppetMaster.cmdGlobalStatus(0);
        }

        private void input_box_TextChanged(object sender, EventArgs e)
        {

        }

        private void output_box_TextChanged(object sender, EventArgs e)
        {

        }

        private void file_box_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
