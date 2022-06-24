using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Automation;
using System.Windows.Forms;

namespace DropClient
{
    public partial class Aurh : Form
    {
        public Aurh()
        {
            InitializeComponent();
        }

        private void Aurh_Load(object sender, EventArgs e)
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
            Close();
        }

        private void xuiButton5_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        private void xuiSuperButton1_Click(object sender, EventArgs e)
        {
            string token;
            var psi = new ProcessStartInfo
            {
                FileName = "https://www.dropbox.com/1/oauth2/authorize?client_id=5k95vx41knifgy8&response_type=token&redirect_uri=http://localhost:5000/Home/Auth&state=ddeos67u0exo8yc",
                UseShellExecute = true
            };
            Process.Start(psi);
            HttpListener listener = new HttpListener();
            listener.Prefixes.Add("http://localhost:5000/Home/Auth/");
            listener.Start();
            HttpListenerContext context = listener.GetContext();
            if (context != null)
            {
                listener.Stop();
                Process[] procsChrome = Process.GetProcessesByName("chrome");
                foreach (Process chrome in procsChrome)
                {
                    if (chrome.MainWindowHandle == IntPtr.Zero)
                        continue;

                    AutomationElement element = AutomationElement.FromHandle(chrome.MainWindowHandle);
                    if (element == null)
                        return;
                    Condition conditions = new AndCondition(
                        new PropertyCondition(AutomationElement.ProcessIdProperty, chrome.Id),
                        new PropertyCondition(AutomationElement.IsControlElementProperty, true),
                        new PropertyCondition(AutomationElement.IsContentElementProperty, true),
                        new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Edit));

                    AutomationElement elementx = element.FindFirst(TreeScope.Descendants, conditions);
                    
                    token = ((ValuePattern)elementx.GetCurrentPattern(ValuePattern.Pattern)).Current.Value as string;
                    token = token.Substring(token.IndexOf('=')+1, token.IndexOf('&') - token.IndexOf('=')-1);
                    Hide();
                    Form1 form2 = new Form1(token);
                    form2.ShowDialog();
                }
            }
        }
    }
}
