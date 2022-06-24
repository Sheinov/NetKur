using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DropClient
{
    public partial class FileName : Form
    {
        public FileName()
        {
            InitializeComponent();
        }
        public string NumUpDown
        {
            get
            {
                return fileN;
            }
            set
            {
                fileN = value;
            }
        }
        string fileN;
        private void button1_Click(object sender, EventArgs e)
        {
            fileN = textBox1.Text;
            Hide();
        }
    }
}
