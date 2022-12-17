using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace yt_dlp_gui
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            load_settings();
        }

        private void save_settings()
        {
            Settings1.Default.downloads_path = textBox2.Text;
            Settings1.Default.yt_dl_path = textBox3.Text;
            Settings1.Default.download_mode = get_current_download_mode();
            Settings1.Default.Save();
        }

        private void load_settings()
        {
            if (Settings1.Default.downloads_path != "false") textBox2.Text = Settings1.Default.downloads_path;
            if (Settings1.Default.yt_dl_path != "false") textBox3.Text = Settings1.Default.yt_dl_path;
            if (Settings1.Default.download_mode != "false") set_current_download_mode(Settings1.Default.download_mode);
        }
        internal void p_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            ytdl_output_field.AppendText(e.Data+"\r\n");
        }

        internal void p_ProcessExited(object sender, System.EventArgs e)
        {
            ytdl_output_field.AppendText("Success" + "\r\n");
            process_next_entry();
        }

        private void process_download(string Url, string DlMode)
        {
            string mode_str = "";
            // determine flags for download modes
            switch (DlMode)
            {
                case "audio":
                   mode_str = "-f 140";
                break;
            }
            string output_tmpl = textBox2.Text+"\\%(title)s.%(ext)s";
            Process process;
            process = new Process();
            process.StartInfo.FileName = textBox3.Text;
            process.StartInfo.Arguments = mode_str + " -o \"" + output_tmpl + "\" " + Url;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.OutputDataReceived += p_OutputDataReceived;
            process.EnableRaisingEvents = true;
            process.Exited += new EventHandler(p_ProcessExited);
            process.Start();
            process.BeginOutputReadLine();
        }

        private void process_next_entry()
        {
            if (url_list_field.Lines.Length == 0) return;
            string url = url_list_field.Lines.First();
            var remain = string.Join("\r\n", url_list_field.Lines.Skip(1));
            url_list_field.Text = remain;
            process_download(url, get_current_download_mode());
        }

        private string get_current_download_mode()
        {
            if (radioButton2.Checked) return "audio";  // audio
            return "video+audio";                      // video+audio 
        }

        private void set_current_download_mode(string mode)
        {
            if (Settings1.Default.download_mode == "audio") radioButton2.Select();  // audio
            else radioButton1.Select();                                             // video+audio 
        }

        // Downloads Directory Picker
        private void textBox2_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.ShowDialog();
            if (folderBrowserDialog1.SelectedPath.Length < 1) return;
            textBox2.Text = folderBrowserDialog1.SelectedPath;
            save_settings();
        }

        // YT-DL path picker
        private void textBox3_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
            if (openFileDialog1.FileName.Length<1) return;
            textBox3.Text = openFileDialog1.FileName;
            save_settings();
        }

        // DL Mode changed to: Video + Audio
        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            save_settings();
        }

        // DL Mode changed to: Audio
        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            save_settings();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            process_next_entry();
        }
    }
}
