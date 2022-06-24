using Dropbox.Api;
using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Dropbox.Api.Files;
using System.Diagnostics;
using System.Windows.Automation;
using System.Net;

namespace DropClient
{
    public partial class Form1 : Form
    {
        static string _accessKey = "sl.BKD7Jrq2kNk0Lb1Bys0G2ZiFlDvcV0vD6gbUVCade782G8PygVJGt_9KkW_82QT3cUcDLP3EMnAnC-fHrgPjBuJ6jj9wWYO8IJGT40lvpZmYmbHINluJKN1MP_jfDFcNZstEHC4";
        static string curPuth = "";
        static string Puth = "";
        public Form1(string key)
        {
            InitializeComponent();
            listView1.SmallImageList = imageList1;
            listView1.ContextMenuStrip = metroContextMenu1;
            _accessKey=key;
        }
        
        private async void Form1_Load(object sender, EventArgs e)
        {
            await TreeTask(string.Empty, treeView1.Nodes[0].Nodes);
            await ListTask(listView1, string.Empty);
        }
        //Загрузка иерархии папок
        static async Task TreeTask(string path,TreeNodeCollection collection)
        {
            using (var dbx = new DropboxClient(_accessKey))
            {
                TreeNode node = null;
                string curPath = path;
                var list = await dbx.Files.ListFolderAsync(path);
                // show folders then files
                foreach (var item in list.Entries.Where(i => i.IsFolder))
                {
                    node=collection.Add(item.Name);
                    curPath = item.AsFolder.PathDisplay;
                    await TreeTask(curPath, collection[node.Index].Nodes);
                    curPath = path;
                }
            }
        }
        //Скачивание файла
        static async Task DownloadTask(string path,string filename,string type)
        {
            if(type=="file")
            {
                using (var response = await new DropboxClient(_accessKey).Files.DownloadAsync(path))
                {
                    var s = response.GetContentAsByteArrayAsync();
                    s.Wait();
                    var d = s.Result;
                    File.WriteAllBytes(filename, d);
                }
            }
            else
            {
                using (var response = await new DropboxClient(_accessKey).Files.DownloadZipAsync(path))
                {
                    var s = response.GetContentAsByteArrayAsync();
                    s.Wait();
                    var d = s.Result;
                    File.WriteAllBytes(filename+".zip", d);
                }
            }
            
        }
        //Создание папки
        static async Task CreateFolderTask(string path)
        {
            using (var dbx = new DropboxClient(_accessKey))
            {
                await dbx.Files.CreateFolderV2Async(path);
            }
        }
        //Загрузка файла
        static async Task UploadTask(string folder, string file, string content)
        {
            using (var dbx = new DropboxClient(_accessKey))
            {
                using (var mem = new MemoryStream(Encoding.UTF8.GetBytes(content)))
                {
                    var updated = await dbx.Files.UploadAsync(
                    folder + "/" + file,
                    WriteMode.Overwrite.Instance,
                    body: mem);
                }
            }
        }
        //Удаление файла/папки
        static async Task DeleteTask(string path)
        {
            using (var dbx = new DropboxClient(_accessKey))
            {
                await dbx.Files.DeleteV2Async(path);
            }
        }
        //Загрузка листа папок и файлов
        static async Task ListTask(ListView listView, string path,string partialpath=null)
        {
                Puth = path;
                using (var dbx = new DropboxClient(_accessKey))
                {
                    var list = await dbx.Files.ListFolderAsync(path);
                    // show folders then files
                    foreach (var item in list.Entries.Where(i => i.IsFolder))
                    {
                        if (partialpath != null)
                        {
                            if (item.Name == partialpath)
                            {
                                await ListTask(listView, item.PathDisplay);
                                curPuth = item.PathDisplay;
                            }
                            else
                            {
                                await ListTask(listView, item.PathDisplay, partialpath);
                            }
                        }
                        else
                        {
                            ListViewItem item1 = new ListViewItem(item.Name, 1, listView.Groups[1]);
                            item1.SubItems.Add(item.PathLower);
                            curPuth = item.PathDisplay;
                            listView.Items.Add(item1);
                        }
                        
                    }

                    foreach (var item in list.Entries.Where(i => i.IsFile))
                    {
                        if (partialpath != null)
                        {
                            //await ListTask(listView, item.PathDisplay);

                        }
                        else
                        {
                            ListViewItem item1 = new ListViewItem(item.Name, 0, listView.Groups[0]);
                            item1.SubItems.Add(item.PathLower);
                            listView.Items.Add(item1);
                        }
                            
                    }
                }
        }
        private async void button1_Click(object sender, EventArgs e)
        {
            
            
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void dirListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private async void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            //MessageBox.Show(treeView1.SelectedNode.Text);
            TreeNode treeNode = treeView1.SelectedNode;
            string path=String.Empty;
            while (treeNode.Parent != null)
            {
                path = "/"+treeNode.Text+path;
                treeNode = treeNode.Parent;
            }
            listView1.Items.Clear();
            await ListTask(listView1, path);
        }
        
        private async void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (listView1.SelectedItems.Count > 0)
                {
                    string path = listView1.SelectedItems[0].Text;
                    string group = listView1.SelectedItems[0].Group.Header;
                    if (group == "P")
                    {
                        listView1.Items.Clear();
                        await ListTask(listView1, string.Empty, path);
                    }
                }
            }
            catch (Exception)
            {
                
            }
            
            
            
        }

        private void treeView1_Leave(object sender, EventArgs e)
        {
            treeView1.SelectedNode = null;
        }

        private void xuiButton1_Click(object sender, EventArgs e)
        {
            
            
        }
        //Кнопка контекстного меню - Скачать
        private async void toolStripMenuItem4_Click(object sender, EventArgs e)
        {
            if(listView1.SelectedItems.Count>0)
                await DownloadTask(listView1.SelectedItems[0].SubItems[1].Text, listView1.SelectedItems[0].SubItems[0].Text,"file");
            else
            {
                int ind1=listView1.Items[0].SubItems[1].Text.LastIndexOf('/');
                string fullpath = listView1.Items[0].SubItems[1].Text.Substring(0, ind1);
                string filename= fullpath.Substring(fullpath.LastIndexOf('/')+1);
                await DownloadTask(fullpath, filename,"folder");
            }
        }
        //Кнопка контекстного меню - Создать папку
        private async void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            int ind1 = listView1.Items[0].SubItems[1].Text.LastIndexOf('/');
            string fullpath = listView1.Items[0].SubItems[1].Text.Substring(0, ind1);
            string filename = fullpath.Substring(fullpath.LastIndexOf('/') + 1);
            FileName file = new FileName();
            file.ShowDialog();
            string name;
            name = file.NumUpDown;
            await CreateFolderTask(fullpath + $"/{name}");
            listView1.Items.Clear();
            await ListTask(listView1, fullpath);
                
        }
        //Кнопка контекстного меню - Удалить
        private async void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                string fullpath = listView1.SelectedItems[0].SubItems[1].Text;
                await DeleteTask(fullpath);
                listView1.Items.Clear();
                await ListTask(listView1, fullpath.Substring(0, fullpath.LastIndexOf('/')));
            }  
            else
            {
                await DeleteTask(curPuth);
                listView1.Items.Clear();
                curPuth=curPuth.Substring(0, curPuth.LastIndexOf('/'));
                await ListTask(listView1, curPuth);
            }
            
            
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            
        }

        private void notifyIcon1_Click(object sender, EventArgs e)
        {
            
                
           
            
        }
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();
        private void metroPanel1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void xuiButton3_Click(object sender, EventArgs e)
        {
            this.Visible = false;
            notifyIcon1.Icon = new Icon("Dropbox-Icon.ico");
            notifyIcon1.Visible = true;
        }

        private void закрытьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void открытьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Visible = true;
            notifyIcon1.Visible = false;
        }

        private void xuiButton4_Click(object sender, EventArgs e)
        {
            Size= new Size(1366,768);
        }

        private void xuiButton5_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        private async void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.Cancel)
                return;
            string filename = openFileDialog1.FileName;
            string fileText = System.IO.File.ReadAllText(filename);
            filename=filename.Substring(filename.LastIndexOf('\\')+1);
            MessageBox.Show("Файл открыт");
            await UploadTask(Puth, filename, fileText);
            listView1.Items.Clear();
            await ListTask(listView1, Puth);
        }

        private void metroPanel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
