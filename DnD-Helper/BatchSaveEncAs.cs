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
    public partial class BatchSaveEncAs : Form
    {
        //Dictionary<string, List<Tuple<string, int, bool, string>>> Saved;
        Dictionary<string, Encounter> Saved;
        Encounter currentEncounter;

        public BatchSaveEncAs(Dictionary<string, Encounter> saved, 
            Encounter cur)
        {
            InitializeComponent();
            Saved = saved;
            currentEncounter = cur;
        
            InitList();
        }

        void InitList(){
            clEncounters.Items.Clear();
            if(currentEncounter!=null && currentEncounter.Monsters.Count>0) clEncounters.Items.Add("Current", true);
            foreach (string s in Saved.Keys)
                clEncounters.Items.Add(s, false);

        }

        private void butCheckAll_Click(object sender, EventArgs e)
        {
            clEncounters.SuspendLayout();
            for(int i=0;i<clEncounters.Items.Count;i++)
            {
                clEncounters.SetItemChecked(i, true);
            }
            clEncounters.ResumeLayout();
        }

        private void butUnCheckAll_Click(object sender, EventArgs e)
        {
            clEncounters.SuspendLayout();
            for (int i = 0; i < clEncounters.Items.Count; i++)
            {
                clEncounters.SetItemChecked(i, false);
            }
            clEncounters.ResumeLayout();
        }

        private void butSave_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "DocX files (*.docx)|*.docx|All files (*.*)|*.*";
            sfd.FilterIndex = 0;
            sfd.RestoreDirectory = true;

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                using (IanUtility.DocXWriter wr = new IanUtility.DocXWriter(sfd.FileName))
                {
                    Random r = new Random();
                    Encounter.PopulateDocXStyles(wr);
                    foreach (string s in clEncounters.CheckedItems)
                    {
                        if (s == "Current")
                            currentEncounter.PopulateDocX(wr, r);
                        else
                            Saved[s].PopulateDocX(wr, r);

                        if (s != (string)clEncounters.CheckedItems[clEncounters.CheckedItems.Count - 1]) wr.HorizRule();

                    }


                    
                    wr.Save();
                }
            }
        }
    }
}
