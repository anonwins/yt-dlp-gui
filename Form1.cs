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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace yt_dlp_gui
{
    public partial class Form1 : Form
    {

        private string SettingsLoaded = "false";

        public Form1()
        {
            InitializeComponent();
            init_dropdown();
            load_settings();
        }

        private void init_dropdown()
        {
            dropdown1.DisplayMember = "Text";
            dropdown1.ValueMember = "Value";
            var items = new[] {
                 new { Text = "Best", Value = "-f best" },
                 new { Text = "Audio Only", Value = "-f 140" },
                 new { Text = "MP3", Value = "-f 140 --extract-audio --audio-format mp3" }
            };
            dropdown1.DataSource = items;
        }

        private void save_settings()
        {
            Settings1.Default.downloads_path = textBox2.Text;
            Settings1.Default.yt_dl_path = textBox3.Text;
            Settings1.Default.dl_format = selected_format();
            Settings1.Default.Save();
        }

        private void load_settings()
        {
            if (Settings1.Default.downloads_path != "false") textBox2.Text = Settings1.Default.downloads_path;
            if (Settings1.Default.yt_dl_path != "false") textBox3.Text = Settings1.Default.yt_dl_path;
            if (Settings1.Default.dl_format != "false") select_format(Settings1.Default.dl_format);
            SettingsLoaded = "true";
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
            if (textBox2.Text=="") return;
            string output_tmpl = textBox2.Text+"\\%(title)s.%(ext)s";
            Process process;
            process = new Process();
            process.StartInfo.FileName = textBox3.Text;
            process.StartInfo.Arguments = DlMode + " -o \"" + output_tmpl + "\" " + Url;
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
            process_download(url, selected_format());
        }

        private string selected_format()
        {
            return dropdown1.SelectedValue.ToString();
        }

        private void select_format(string format)
        {
            dropdown1.SelectedValue = format;
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

        // Download button click
        private void button1_Click(object sender, EventArgs e)
        {
            process_next_entry();
        }

        // Format changed
        private void dropdown1_SelectedValueChanged(object sender, EventArgs e)
        {
            if (SettingsLoaded=="false") return;
            if (Settings1.Default.dl_format == selected_format()) return;
            save_settings();
        }
    }
}
