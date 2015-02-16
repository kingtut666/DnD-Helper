using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DnDMonsters
{
    public partial class HoverPopup : Form
    {
        public HoverPopup()
        {
            InitializeComponent();
        }

        private void HoverPopup_MouseMove(object sender, MouseEventArgs e)
        {
            Close();
        }
    }
}
