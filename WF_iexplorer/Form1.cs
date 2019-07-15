using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace WF_iexplorer
{
    public partial class Form1 : Form
    {
        public delegate void ChangeLabel_safe(string label_text);
        public System.Windows.Forms.Label Txt2Write {
            get {
                return this.label1;
            }
        }
        public Process p = null;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void Label1_Click(object sender, EventArgs e)
        {

        }

        private void FolderBrowserDialog1_HelpRequest(object sender, EventArgs e)
        {

        }

        private void Button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "Executable (EXE)|*.exe";
            var result = openFileDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                button1.Text = openFileDialog1.FileName;
                var str = openFileDialog1.FileName.Split('\\');
                var file = str[str.Length - 1];
                if (file == "YoutubeControlWebServer.exe")
                {
                    p = new Process();
                    var pinfo = new ProcessStartInfo(openFileDialog1.FileName);
                    pinfo.WindowStyle = ProcessWindowStyle.Minimized;
                    pinfo.WorkingDirectory = openFileDialog1.FileName.Replace("\\YoutubeControlWebServer.exe","");
                    p.StartInfo = pinfo;
                    p.Start();
                    button1.Text = "Process STARTED";
                }

            }
        }

        private void Label2_Click(object sender, EventArgs e)
        {

        }

        private void PathResult_Click(object sender, EventArgs e)
        {

        }
    }
}
