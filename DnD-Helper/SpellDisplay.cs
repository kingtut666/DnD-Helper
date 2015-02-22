using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Odbc;
using System.Data.OleDb;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using IanUtility;
using System.Runtime.Serialization;
using System.Xml;

namespace DnDHelper
{
    public partial class DnDHelper : Form
    {
        List<Spell> AllSpells = new List<Spell>();
        Spellbook curSpellbook = new Spellbook();
        SpellbookLookup Spellbooks = new SpellbookLookup();
        
        public event ListChangedEventHandler Spellbook_Changed;
        void RaiseSpellbookChanged(object sender, ListChangedEventArgs args)
        {
            if (Spellbook_Changed != null) Spellbook_Changed(sender, args);
        }


        void InitSpellPage()
        {
            lblFilenameSpellCombined.Text = Properties.Settings.Default.SpellsFile;
            
            //TODO: Save Spells in XML format
            if (Properties.Settings.Default.SpellsFile != "")
                LoadSpells(Properties.Settings.Default.SpellsFile);
            else
            {
                Properties.Settings.Default.SpellsFile = fileRoot + "DnDHelper-Spells.xml";
                Properties.Settings.Default.Save();
                if (!File.Exists(Properties.Settings.Default.SpellsFile))
                    SaveSpellsAsXlsx(Properties.Settings.Default.SpellsFile);
            }
            lblFilenameSpellCombined.Text = Properties.Settings.Default.SpellsFile;
         
            if (Properties.Settings.Default.SpellbooksFile != "")
                LoadSpellbooksAsXML(Properties.Settings.Default.SpellbooksFile);
            else
            {
                Properties.Settings.Default.SpellbooksFile = fileRoot + "DnDHelper-Spellbooks.xml";
                Properties.Settings.Default.Save();
                if (!File.Exists(Properties.Settings.Default.SpellbooksFile))
                    SaveSpellbooksAsXML(Properties.Settings.Default.SpellbooksFile);
            }
            lblSpellbookDB.Text = Properties.Settings.Default.SpellbooksFile;
            


            PopulateSpellTree();

            Spellbook_Changed += EncounterPlanner_Spellbook_Changed;
        }



        List<Spell> GetSortedSpellList()
        {
            List<Spell> ret = curSpellbook.Spells.ToList();
            ret.Sort(SortSpell);
            return ret;
        }
        int SortSpell(Spell a, Spell b){
            int retTrue = checkSpellOrderAsc.Checked ? 1 : -1;
            if (a == null && b == null) return 0;
            if (a == null) return retTrue * -1;
            if (b == null) return retTrue; 
            
            if (radioSpellOrderName.Checked)
            {
                return String.Compare(a.Name, b.Name, true) * retTrue;
            }
            else if (radioSpellOrderLFN.Checked)
            {
                if (a.Level == b.Level)
                {
                    if (String.Compare(a.School, b.School, true) == 0)
                    {
                        return String.Compare(a.Name, b.Name, true);
                    }
                    else return String.Compare(a.School, b.School, true);
                }
                if (a.Level < b.Level) return retTrue * -1;
                else return retTrue;
            }
            else if (radioSpellOrderFLN.Checked)
            {
                if (String.Compare(a.School, b.School, true) == 0)
                {
                    if (a.Level == b.Level)
                    {
                        return String.Compare(a.Name, b.Name, true);
                    }
                    if (a.Level < b.Level) return -1;
                    else return 1;
                }
                else return String.Compare(a.School, b.School, true) * retTrue;
            }
            return 0;
        }
        void EncounterPlanner_Spellbook_Changed(object sender, ListChangedEventArgs e)
        {
            //update display
            flowSpells.Controls.Clear();

            List<Spell> q = GetSortedSpellList() ;
            if (q == null) return;

            foreach (Spell s in q)
            {
                TableLayoutPanel p = DisplaySpell(s, flowSpells.Width-30);
                flowSpells.Controls.Add(p);
            }
        }

        void PopulateSpellTree()
        {
            clSpellsByClass.Items.Clear();
            clSpellsByLevel.Items.Clear();
            clSpellsByName.Items.Clear();
            clSpellsBySchool.Items.Clear();

            HashSet<string> levels = new HashSet<string>();
            HashSet<string> schools = new HashSet<string>();
            HashSet<string> classes = new HashSet<string>();
            
            //class
            foreach (Classes cls in Enum.GetValues(typeof(Classes)))
            {
                classes.Add(cls.ToString());
            }

            foreach (Spell s in AllSpells)
            {
                //school
                schools.Add(s.School); 
                
                //level
                string lvl = (s.Level == 0 ? "Cantrip" : "Level " + s.Level.ToString());
                levels.Add(lvl);
            }
            
            List<string> ss = schools.ToList();
            ss.Sort();
            foreach (string s in ss) clSpellsBySchool.Items.Add(s, true);
            ss = levels.ToList();
            ss.Sort();
            foreach (string s in ss) clSpellsByLevel.Items.Add(s, true);
            ss = classes.ToList();
            ss.Sort();
            foreach (string s in ss) clSpellsByClass.Items.Add(s, true);

            DisplayAllowedSpells();
        }

        TableLayoutPanel DisplaySpell(Spell sp, int maxWidth)
        {
            TableLayoutPanel p = new TableLayoutPanel();
            p.AutoSize = true;
            p.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            p.GrowStyle = TableLayoutPanelGrowStyle.AddRows;

            FlowLayoutPanel flp0 = new FlowLayoutPanel();
            flp0.AutoSize = true;
            flp0.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            flp0.Margin = new System.Windows.Forms.Padding(0);
            flp0.Controls.Add(NewLabel(sp.Name, LabelTypes.heading, maxWidth));
            Button butDel = new Button();
            butDel.Text = "Remove";
            butDel.Tag = sp;
            butDel.Click += butDel_Click;
            butDel.Anchor = AnchorStyles.Right;
            flp0.Controls.Add(butDel);
            p.Controls.Add(flp0);

            p.Controls.Add(NewLabel(sp.LevelSchool + "   (" + sp.ClassesString + ")", LabelTypes.italic, maxWidth));
            FlowLayoutPanel flp1 = new FlowLayoutPanel();
            flp1.AutoSize = true;
            flp1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            flp1.Margin = new System.Windows.Forms.Padding(0);
            flp1.Controls.Add(NewLabel("Casting Time: ", LabelTypes.bold, maxWidth));
            flp1.Controls.Add(NewLabel(sp.sCastingTime, LabelTypes.text, maxWidth));
            p.Controls.Add(flp1);
            FlowLayoutPanel flp2 = new FlowLayoutPanel();
            flp2.AutoSize = true;
            flp2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            flp2.Margin = new System.Windows.Forms.Padding(0); 
            flp2.Controls.Add(NewLabel("Range: ", LabelTypes.bold, maxWidth));
            flp2.Controls.Add(NewLabel(sp.sRange, LabelTypes.text, maxWidth));
            p.Controls.Add(flp2);
            FlowLayoutPanel flp3 = new FlowLayoutPanel();
            flp3.AutoSize = true;
            flp3.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            flp3.Margin = new System.Windows.Forms.Padding(0); 
            flp3.Controls.Add(NewLabel("Duration: ", LabelTypes.bold, maxWidth));
            flp3.Controls.Add(NewLabel(sp.sDuration, LabelTypes.text, maxWidth));
            p.Controls.Add(flp3);
            FlowLayoutPanel flp4 = new FlowLayoutPanel();
            flp4.AutoSize = true;
            flp4.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            flp4.Margin = new System.Windows.Forms.Padding(0); 
            flp4.Controls.Add(NewLabel("Components: ", LabelTypes.bold, maxWidth));
            flp4.Controls.Add(NewLabel(sp.sComponents, LabelTypes.text, maxWidth));
            p.Controls.Add(flp4);
          
            p.Controls.Add(NewLabel("See page " + sp.PHBPage.ToString() + " of the Players Handbook for more details", LabelTypes.text, maxWidth));
            
            //p.Controls.Add(NewLabel(sp.Description, LabelTypes.text, maxWidth));
            DrawableRichTextBox r = new DrawableRichTextBox();
            r.Width = maxWidth - 30;
            r.Height = 10;
            r.ContentsResized += r_ContentsResized;
            r.Rtf = sp.rtfDescription;
            r.BackColor = Color.White;
            r.BorderStyle = BorderStyle.None;

            r.ReadOnly = true;
            p.Controls.Add(r);

            return p;
        }

        void butDel_Click(object sender, EventArgs e)
        {
            Button b = sender as Button;
            Spell sp = b.Tag as Spell;
            curSpellbook.Spells.Remove(sp);
            RaiseSpellbookChanged(null, new ListChangedEventArgs(ListChangedType.Reset, -1));
        }

        void r_ContentsResized(object sender, ContentsResizedEventArgs e)
        {
            DrawableRichTextBox r = sender as DrawableRichTextBox;
            r.Height = e.NewRectangle.Height + 5;
        }


     
        private void butAddChecked_Click(object sender, EventArgs e)
        {
            bool added = false;
            foreach (string sName in clSpellsByName.CheckedItems.Cast<string>())
            {
                Spell s = null;
                foreach (Spell s2 in AllSpells)
                {
                    if (s2.Name == sName)
                    {
                        s = s2;
                        break;
                    }
                }
                if (s == null) 
                    continue; //Shouldn't happen

                if (curSpellbook.Spells.Add(s)) added = true;
            }
            if (added)
                RaiseSpellbookChanged(this, new ListChangedEventArgs(ListChangedType.ItemAdded, -1));
        }

        private void DisplayAllowedSpells()
        {
            //Get list of allowed classes
            Classes allowedClass = 0;
            foreach (object o in clSpellsByClass.CheckedItems)
            {
                string s = o as string;
                allowedClass |= (Classes)Enum.Parse(typeof(Classes), s);
            }

            //Get list of allowed schools
            List<string> allowedSchools = new List<string>();
            foreach (object o in clSpellsBySchool.CheckedItems)
            {
                string s = o as string;
                allowedSchools.Add(s);
            }

            //Get list of allowed levels
            List<int> allowedLevels = new List<int>();
            foreach (object o in clSpellsByLevel.CheckedItems)
            {
                string s = o as string;
                if (s == "Cantrip"){
                    allowedLevels.Add(0);
                    continue;
                }
                s = s.Substring("Level ".Length);
                allowedLevels.Add(Int32.Parse(s));
            }

            //Iterate over spells
            IEnumerable<string> wereChecked = clSpellsByName.CheckedItems.Cast<string>();
            clSpellsByName.Items.Clear();
            List<string> sp = new List<string>();
            foreach (Spell s in AllSpells)
            {
                //class
                if ((s.Classes & allowedClass) == 0) continue;
                //school
                if (!allowedSchools.Contains(s.School)) continue;
                //level
                if (!allowedLevels.Contains(s.Level)) continue;

                sp.Add(s.Name);
            }
            sp.Sort();
            foreach (string s in sp)
            {
                clSpellsByName.Items.Add(s, wereChecked.Contains(s));
            }
            
            
        }


        #region Spell Printing
        private void butSpellPrintPreview_Click(object sender, EventArgs e)
        {
            PrintPreviewDialog ppd = new PrintPreviewDialog();
            ppd.Document = new System.Drawing.Printing.PrintDocument();
            ppd.Document.PrintPage += Document_PrintPageSpell;
            ppd.Document.BeginPrint += Document_BeginPrintSpell;
            ppd.UseAntiAlias = false;
            ppd.ShowDialog();
        }
        private void butPrintSpells_Click(object sender, EventArgs e)
        {
            PrintDialog pd = new PrintDialog();
            pd.AllowPrintToFile = true;
            pd.AllowSomePages = true;
            pd.AllowSelection = true;
            pd.AllowCurrentPage = false;
            pd.Document = new System.Drawing.Printing.PrintDocument();
            pd.Document.PrintPage += Document_PrintPageSpell;
            pd.Document.BeginPrint += Document_BeginPrintSpell;

            DialogResult r = pd.ShowDialog();
            if (r == System.Windows.Forms.DialogResult.OK)
            {
                pd.Document.Print();
            }
        }
        void Document_BeginPrintSpell(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            printSpellIdx = 0;
            toPrint = GetSortedSpellList().ToList();
        }
        static int printSpellIdx = 0;
        static List<Spell> toPrint = null;
        void Document_PrintPageSpell(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            if (toPrint == null) return;
            int bottom = 0;
            for (int i = printSpellIdx; i < toPrint.Count; i++)
            {
                TableLayoutPanel p = DisplaySpell(toPrint[i], e.MarginBounds.Width);
                p.BackColor = Color.White;
                p.PerformLayout();
                Size sz = p.GetPreferredSize(new Size(e.MarginBounds.Width, 0));
                p.SetBounds(0, 0, sz.Width, sz.Height);

                //replace the richtextbox with a bitmap
                DrawableRichTextBox drt = null;
                foreach (Control c in p.Controls)
                {
                    if (c is DrawableRichTextBox)
                    {
                        
                        drt = c as DrawableRichTextBox;
                        int height = (int)(((decimal)drt.Height) * 1.1m);
                        
                        Bitmap bmp = new Bitmap(drt.Width, height);
                        drt.DrawToBitmap_Actual(bmp, new Rectangle(0, 0, drt.Width, height));

                        PictureBox pic = new PictureBox();
                        pic.SizeMode = PictureBoxSizeMode.Normal;
                        pic.ClientSize = new System.Drawing.Size(drt.Width, height);
                        pic.Image = bmp;
                        p.Controls.Remove(c);
                        p.Controls.Add(pic);

                    }
                }
                //p.PerformLayout();


                sz = p.GetPreferredSize(new Size(0, 0));
                p.SetBounds(0, 0, sz.Width, sz.Height);
                if (bottom == 0 || bottom + sz.Height < e.MarginBounds.Height)
                {
                    using (Bitmap b = new Bitmap(sz.Width, sz.Height))
                    {
                        p.DrawToBitmap(b, new Rectangle(0, 0, sz.Width, sz.Height));
                        e.Graphics.DrawImage(b, new Point(e.MarginBounds.Left, bottom + e.MarginBounds.Top));
                        bottom += sz.Height + 30;
                    }
                }
                else
                {
                    printSpellIdx = i;
                    e.HasMorePages = true;
                    return;
                }
            }
            e.HasMorePages = false;
            return;
        }
        #endregion

        private void butResetSpells_Click(object sender, EventArgs e)
        {
            curSpellbook = new Spellbook();
            RaiseSpellbookChanged(this, new ListChangedEventArgs(ListChangedType.Reset, -1));
        }

        private void radioSpellOrderName_CheckedChanged(object sender, EventArgs e)
        {
            EncounterPlanner_Spellbook_Changed(sender, new ListChangedEventArgs(ListChangedType.Reset, -1));
        }

        private void radioSpellOrderLFN_CheckedChanged(object sender, EventArgs e)
        {
            EncounterPlanner_Spellbook_Changed(sender, new ListChangedEventArgs(ListChangedType.Reset, -1));
        }

        private void radioSpellOrderFLN_CheckedChanged(object sender, EventArgs e)
        {
            EncounterPlanner_Spellbook_Changed(sender, new ListChangedEventArgs(ListChangedType.Reset, -1));
        }

        private void checkSpellOrderAsc_CheckedChanged(object sender, EventArgs e)
        {
            EncounterPlanner_Spellbook_Changed(sender, new ListChangedEventArgs(ListChangedType.Reset, -1));
        }

        private void butSaveAsDocX_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "DocX files (*.docx)|*.docx|All files (*.*)|*.*";
            sfd.FilterIndex = 0;
            sfd.RestoreDirectory = true;
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                Spell.SaveAsDocX(sfd.FileName, curSpellbook.Spells.ToList());
            }
        }

        #region Spellbooks
        private void butSpellbookDel_Click(object sender, EventArgs e)
        {
            string spb = cmbSpellbook.Text;
            if (Spellbooks.ContainsKey(spb))
            {
                Spellbooks.Remove(cmbSpellbook.Text);
                UpdateSpellbookList();
            }
            cmbSpellbook.SelectedIndex = -1;
            cmbSpellbook.SelectedText = "";

            SaveSpellbooksDefault();
        }
        private void butSpellbookLoad_Click(object sender, EventArgs e)
        {
            LoadSpellbook(cmbSpellbook.Text);
        }
        private void butSpellbookSave_Click(object sender, EventArgs e)
        {
            SaveSpellbook(cmbSpellbook.Text);
        }
        void UpdateSpellbookList()
        {
            List<string> s = Spellbooks.Keys.ToList();
            s.Sort();
            cmbSpellbook.DataSource = s;
        }
        #endregion

        

        private void checkAllSpells_CheckedChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < clSpellsByName.Items.Count; i++)
                clSpellsByName.SetItemChecked(i, true);    
        }

        private void checkNoSpells_CheckedChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < clSpellsByName.Items.Count; i++)
                clSpellsByName.SetItemChecked(i, false);
        }

        private void Spells_SelectionCheckChanged(object sender, ItemCheckEventArgs e)
        {
            //Horrible hack as this event fires _before_ the check changes
            CheckedListBox clb = (CheckedListBox)sender;
            // Switch off event handler
            clb.ItemCheck -= Spells_SelectionCheckChanged;
            clb.SetItemCheckState(e.Index, e.NewValue);
            // Switch on event handler
            clb.ItemCheck += Spells_SelectionCheckChanged;

            //Now do the actual work
            DisplayAllowedSpells();
        }



    }
}
