using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DnDHelper
{
    public partial class BatchMonsterUpdate : Form
    {
        public bool SaveRequired = false;

        Dictionary<string, string[]> Settings = new Dictionary<string, string[]>();
        Dictionary<string,Monster> Monsters;
        public BatchMonsterUpdate(Dictionary<string,Monster> monsters)
        {
            InitializeComponent();

            Monsters = monsters;
            

            Settings.Add("Environment", GetValues_Env());
            comboSetting.DataSource = Settings.Keys.ToList();
            comboSetting.SelectedIndex = 0;

            clMonsters.Sorted = true;

            foreach (string s in Monsters.Keys)
            {
                clMonsters.Items.Add(s, false);
            }
        }

        private void butUpdate_Click(object sender, EventArgs e)
        {
            //if checked in any
            IEnumerable<string> ms = clMonsters.CheckedItems.Cast<string>();

            List<Monster> mons = new List<Monster>();
            foreach (string s in ms)
            {
                if (Monsters.ContainsKey(s)) mons.Add(Monsters[s]);
            }

            switch ((string)comboSetting.SelectedValue)
            {
                case "Environment":
                    UpdateMonsters_Env(mons, (string)comboValue.SelectedValue);
                    break;
                default:
                    break;      
            }
            SaveRequired = true;
        }

        private void butExit_Click(object sender, EventArgs e)
        {
            Close();
        }


        ///Environment settings
        string[] GetValues_Env()
        {
            string[] a = Enum.GetNames(typeof(Environments));
            List<string> b = new List<string>();
            b.AddRange(a);
            foreach (string aa in a)
            {
                b.Add("!" + aa);
            }
            return b.ToArray();
        }
        void UpdateMonsters_Env(List<Monster> monsters, string env)
        {
            bool inv = false;
            if (env[0] == '!')
            {
                inv = true;
                env = env.Substring(1);
            }
            Environments en = (Environments)Enum.Parse(typeof(Environments), env);
            foreach (Monster m in monsters)
            {
                if (!inv) m.Environ |= en;
                else m.Environ &= ~en;
            }
        }

        private void comboSetting_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboValue.DataSource = null;
            comboValue.Items.Clear();
            if (!Settings.ContainsKey((string)comboSetting.SelectedValue))
            {
                return;
            }
            comboValue.DataSource = Settings[(string)comboSetting.SelectedValue];
            comboValue.SelectedIndex = 0;
        }

        private void butUncheckAll_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < clMonsters.Items.Count; i++) clMonsters.SetItemChecked(i, false);
        }

        private void butCheckAll_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < clMonsters.Items.Count; i++) clMonsters.SetItemChecked(i, true);
        }

        private void butCheckNone_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < clMonsters.Items.Count; i++)
            {
                string s = clMonsters.Items[i] as string;
                if (Monsters.ContainsKey(s))
                {
                    if (Monsters[s].Environ == Environments.None)
                        clMonsters.SetItemChecked(i, true);
                    else
                        clMonsters.SetItemChecked(i, false);
                }
            }
            
        }



    }
}
