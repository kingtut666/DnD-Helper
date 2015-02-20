using IanUtility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Odbc;
using System.Data.OleDb;
using System.Deployment.Application;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;

namespace DnDMonsters
{
    public partial class DnDHelper : Form
    {

        #region Public
        public Dictionary<string, Monster> Monsters = new Dictionary<string, Monster>();
        public Encounter curEncounter = new Encounter();
        public static Random rand = new Random();
        string fileRoot = "";

        public DnDHelper()
        {
            InitializeComponent();
            if (ApplicationDeployment.IsNetworkDeployed)
            {
                lblSettingsMsg.Text = "Sample files and notes in: " + ApplicationDeployment.CurrentDeployment.DataDirectory;
                lblSettingsMsg.LinkClicked += lblSettingsMsg_LinkClicked;
                lblSettingsMsg.Links[0].LinkData = ApplicationDeployment.CurrentDeployment.DataDirectory;
            }
            else lblSettingsMsg.Text = "DEBUG BUILD";

            fileRoot = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\KingTut\\";
            if (!Directory.Exists(fileRoot)) Directory.CreateDirectory(fileRoot);
            //Properties.Settings.Default.Reset();

            if (Properties.Settings.Default.MonsterFile != "") 
                LoadMonsters(Properties.Settings.Default.MonsterFile, true);
            else
            {

                Properties.Settings.Default.MonsterFile = fileRoot+"DnDHelper-Monsters.xml";
                Properties.Settings.Default.Save();
                if (!File.Exists(Properties.Settings.Default.MonsterFile))
                    SaveMonstersToDefault();
            }
            lblFilenameEnc.Text = Properties.Settings.Default.MonsterFile;
                

            if (Properties.Settings.Default.EncountersFile != "")
            {
                LoadEncountersAsXml(Properties.Settings.Default.EncountersFile);
            }
            else
            {
                Properties.Settings.Default.EncountersFile = fileRoot + "DnDHelper-Encounters.xml";
                Properties.Settings.Default.Save();
                if(!File.Exists(Properties.Settings.Default.EncountersFile))
                    SaveEncountersAsXML(Properties.Settings.Default.EncountersFile);
            }
            lblEncounterDB.Text = Properties.Settings.Default.EncountersFile;
                

            ((IBindingList)curEncounter.Monsters).ApplySort(null, ListSortDirection.Descending);
            curEncounter.Monsters.ListChanged += activeMonsters_ListChanged;

            BindingSource bind = new BindingSource(SaveEncounterList, null);
            comboEncounters.DisplayMember = "Key";
            comboEncounters.DataSource = bind;
            comboEncounters.SelectedIndex = -1;


            InitSpellPage();
            InitMiniMaker();
        }

        void lblSettingsMsg_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(ApplicationDeployment.CurrentDeployment.DataDirectory);
        }

        void activeMonsters_ListChanged(object sender, ListChangedEventArgs e)
        {
            if (e.ListChangedType == ListChangedType.Reset)
            {
                flowMonsters.Controls.Clear();
                flowMonsters.Refresh();
                foreach (ActualMonster m in curEncounter.Monsters)
                {
                    TableLayoutPanel p = DisplayMonster(m, splitContainer1.Panel2.Width, true);
                    flowMonsters.Controls.Add(p);
                }
            }
            else if (e.ListChangedType == ListChangedType.ItemAdded)
            {
                TableLayoutPanel p = DisplayMonster(curEncounter.Monsters[e.NewIndex], splitContainer1.Panel2.Width, true);
                flowMonsters.Controls.Add(p);
            }
            else
            {
                throw new NotImplementedException();
            }
        }
        public void ClearMonsters()
        {
            curEncounter.Monsters.Clear();
            flowMonsters.Controls.Clear();
        }
        public bool ToggleSuspendLayout()
        {
            mSuspendLayout = !mSuspendLayout;
            if (mSuspendLayout) flowMonsters.SuspendLayout();
            else flowMonsters.ResumeLayout();
            return mSuspendLayout;
        }
        #endregion


        #region Events
        public event ListChangedEventHandler MonstersChanged;
        //public event ListChangedEventHandler ActiveMonstersChanged;

        void TriggerMonstersChanged()
        {
            if (MonstersChanged != null) MonstersChanged(this, null);
        }
        
        #endregion


        #region UI
        private void butReset_Click(object sender, EventArgs e)
        {
            ClearMonsters();
        }
        private void butAdd_Click(object sender, EventArgs e)
        {
            Monster m = comboMonsters.SelectedValue as Monster;
            bool summ = checkSummaryOnly.Checked;

            if (!summ && chkAutoSummary.Checked)
            {
                //if this monster already exists, just add a summary
                if (curEncounter.Monsters.Where(a => a.Monster == m).FirstOrDefault() != null)
                    summ = true;
            }

            curEncounter.AddMonster(m, summ);
        }
        private void butReloadMons_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Multiselect = false;
            dlg.ReadOnlyChecked = true;
            dlg.ValidateNames = true;
            dlg.CheckFileExists = true;
            dlg.FileName = Properties.Settings.Default.MonsterFile;
            DialogResult r = dlg.ShowDialog();
            if (r == DialogResult.OK)
            {
                Properties.Settings.Default.MonsterFile = dlg.FileName;
                Properties.Settings.Default.Save();
                lblFilenameEnc.Text = dlg.FileName;
                LoadMonsters(dlg.FileName, true);
            }

        }
        private void Form1_ResizeEnd(object sender, EventArgs e)
        {
            flowMonsters.Controls.Clear();
            foreach (ActualMonster m in curEncounter.Monsters)
            {
                TableLayoutPanel p = DisplayMonster(m, splitContainer1.Panel2.Width, true);
                flowMonsters.Controls.Add(p);
            }
        }
        private void butEncounter_Click(object sender, EventArgs e)
        {
            EncounterGenerator en = new EncounterGenerator();
            en.SetParent(this);
            en.Show();
        }
        private void butSaveEncounter_Click(object sender, EventArgs e)
        {
            SaveEncounter(comboEncounters.Text);
            SaveEncounterList.Add(comboEncounters.Text);
        }
        private void butLoadEncounter_Click(object sender, EventArgs e)
        {
            LoadEncounter(comboEncounters.Text);
        }
        private void butDelEncounter_Click(object sender, EventArgs e)
        {
            string enc = comboEncounters.Text;
            if (SavedEnc.ContainsKey(enc)) { 
                SavedEnc.Remove(comboEncounters.Text);
                SaveEncounterList.Remove(enc);
            }
            SaveEncountersAsBinary();
        }
        #endregion

        #region Private
        enum LabelTypes { heading, subHeading, text, tiny, bold, italic };
        Label lastAdded = null;
        Label NewLabel(string text, LabelTypes t, int maxWidth)
        {
            Label l = new Label();
            l.Text = text;
            switch (t)
            {
                case LabelTypes.heading:
                    l.Font = new Font(FontFamily.GenericSansSerif, (float)16.0, FontStyle.Bold);
                    break;
                case LabelTypes.subHeading:
                    l.Font = new Font(FontFamily.GenericSansSerif, (float)10.0, FontStyle.Bold);
                    break;
                case LabelTypes.bold:
                    l.Font = new Font(FontFamily.GenericSansSerif, (float)10.0, FontStyle.Bold);
                    break;
                case LabelTypes.italic:
                    l.Font = new Font(FontFamily.GenericSansSerif, (float)10.0, FontStyle.Italic);
                    break;
                case LabelTypes.tiny:
                    l.Font = new Font(FontFamily.GenericSansSerif, (float)9.0, FontStyle.Italic);
                    break;
                case LabelTypes.text:
                default:
                    l.Font = new Font(FontFamily.GenericSerif, (float)10.0, FontStyle.Regular);
                    break;
            }
            l.Anchor = AnchorStyles.Left;
            int width = maxWidth - 10;
            l.MaximumSize = new Size(width, 0);
            l.AutoSize = true;
            lastAdded = l;
            return l;
        }

        bool mSuspendLayout = false;
        
        TableLayoutPanel DisplayMonster(ActualMonster mm, int maxWidth, bool Editable=false){
            Monster m = mm.Monster;
            TableLayoutPanel p = new TableLayoutPanel();

            //Get some typical widths
            Graphics gg = flowMonsters.CreateGraphics();
            int width6 = gg.MeasureString("WWWWWW", NewLabel("", LabelTypes.text, 100).Font).ToSize().Width;

            //p.AutoScroll = true;
            p.AutoSize = true;
            p.Dock = DockStyle.Fill;
            p.BackColor = Color.White;
            p.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            p.MaximumSize = new System.Drawing.Size(maxWidth, 0);
            p.Refresh();
            p.GrowStyle = TableLayoutPanelGrowStyle.AddRows;
            p.Tag = mm;
            if (mm.Summary)
            {
                TableLayoutPanel summ_panel = new TableLayoutPanel();
                summ_panel.RowCount = 1;
                summ_panel.AutoSize = true;
                summ_panel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
                summ_panel.GrowStyle = TableLayoutPanelGrowStyle.AddColumns;
                //FlowLayoutPanel name_panel = new FlowLayoutPanel();
                //name_panel.AutoSize = true;
                //name_panel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
                //name_panel.FlowDirection = FlowDirection.LeftToRight;
                //name_panel.WrapContents = false;
                summ_panel.Controls.Add(NewLabel(m.Name, LabelTypes.heading, maxWidth - 20));
                if (Editable)
                {
                    TextBox name_txt = new TextBox();
                    name_txt.Text = mm.ActualName;
                    name_txt.Tag = mm;
                    name_txt.TextChanged += name_txt_TextChanged;
                    summ_panel.Controls.Add(name_txt);
                    summ_panel.Controls.Add(NewLabel("HP:", LabelTypes.subHeading, maxWidth - summ_panel.Controls[1].Right - 20));
                    TextBox textBoxHP = new TextBox();
                    textBoxHP.Text = mm.HP.ToString();
                    textBoxHP.Anchor = AnchorStyles.Left;
                    textBoxHP.TextChanged += textBoxHP_TextChanged;
                    summ_panel.Controls.Add(textBoxHP);
                }
                else
                {
                    summ_panel.Controls.Add(NewLabel(mm.ActualName != "" ? " (" + mm.ActualName + ")" : "", LabelTypes.heading, maxWidth - summ_panel.Controls[0].Right - 40));
                    summ_panel.Controls.Add(NewLabel("HP:", LabelTypes.subHeading, maxWidth - summ_panel.Controls[1].Right - 20));
                    summ_panel.Controls.Add(NewLabel(mm.HP.ToString(), LabelTypes.subHeading, summ_panel.Controls[2].Right - 20));
                }
                p.Controls.Add(summ_panel);
            }
            else
            {


                //p.GrowStyle = TableLayoutPanelGrowStyle.AddColumns | TableLayoutPanelGrowStyle.AddRows;
                //Name
                FlowLayoutPanel name_panel = new FlowLayoutPanel();
                name_panel.AutoSize = true;
                name_panel.WrapContents = false;
                name_panel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink; 
                name_panel.FlowDirection = FlowDirection.LeftToRight;
                name_panel.Controls.Add(NewLabel(m.Name, LabelTypes.heading, maxWidth-20));
                if (Editable)
                {
                    TextBox name_txt = new TextBox();
                    name_txt.Text = mm.ActualName;
                    name_txt.Tag = mm;
                    name_txt.TextChanged += name_txt_TextChanged;
                    name_panel.Controls.Add(name_txt);
                }
                else
                    name_panel.Controls.Add(NewLabel(mm.ActualName!=""?" (" + mm.ActualName + ")":"", LabelTypes.heading, maxWidth - -name_panel.Controls[0].Right - 40));
                
                p.Controls.Add(name_panel);
                //type, size
                p.Controls.Add(NewLabel(m.Size + " " + m.Type + ", " + m.Alignment, LabelTypes.tiny, maxWidth-20));
                //AC, Speed, HP
                TableLayoutPanel ac_line = new TableLayoutPanel();
                ac_line.AutoSize = true;
                ac_line.RowCount = 1;
                ac_line.GrowStyle = TableLayoutPanelGrowStyle.AddColumns;
                ac_line.Controls.Add(NewLabel("AC:", LabelTypes.subHeading, maxWidth));
                ac_line.Controls.Add(NewLabel(m.AC.ToString(), LabelTypes.text, width6));
                ac_line.Controls.Add(NewLabel("Speed:", LabelTypes.subHeading, maxWidth));
                ac_line.Controls.Add(NewLabel(m.Speed, LabelTypes.text, width6));
                ac_line.Controls.Add(NewLabel("MaxHP:", LabelTypes.subHeading, maxWidth));
                ac_line.Controls.Add(NewLabel(m.HP, LabelTypes.text, width6));
                ac_line.Controls.Add(NewLabel("XP:", LabelTypes.subHeading, maxWidth));
                ac_line.Controls.Add(NewLabel(m.XP.ToString(), LabelTypes.text, width6));
                ac_line.Controls.Add(NewLabel("HP:", LabelTypes.subHeading, maxWidth));
                if (Editable)
                {
                    TextBox textBoxHP = new TextBox();
                    textBoxHP.Text = mm.HP.ToString();
                    textBoxHP.TextChanged += textBoxHP_TextChanged;
                    ac_line.Controls.Add(textBoxHP);
                }
                else
                    ac_line.Controls.Add(NewLabel(mm.HP.ToString(), LabelTypes.subHeading, maxWidth));

                p.Controls.Add(ac_line);
                //Stats1
                TableLayoutPanel stats1_panel = new TableLayoutPanel();
                stats1_panel.MaximumSize = new System.Drawing.Size(maxWidth-ac_line.Left-10,0);
                stats1_panel.AutoSize = true;
                stats1_panel.GrowStyle = TableLayoutPanelGrowStyle.AddColumns;
                stats1_panel.RowCount = 2;
                
                stats1_panel.Controls.Add(NewLabel("STR", LabelTypes.subHeading, maxWidth), 0, 0);
                stats1_panel.Controls.Add(NewLabel(m.STR.ToString()+" ("+m.Modifier(m.STR)+")", LabelTypes.text, maxWidth), 0, 1);
                stats1_panel.Controls.Add(NewLabel("DEX", LabelTypes.subHeading, maxWidth), 1, 0);
                stats1_panel.Controls.Add(NewLabel(m.DEX.ToString() + " (" + m.Modifier(m.DEX) + ")", LabelTypes.text, maxWidth), 1, 1);
                stats1_panel.Controls.Add(NewLabel("INT", LabelTypes.subHeading, maxWidth), 2, 0);
                stats1_panel.Controls.Add(NewLabel(m.INT.ToString() + " (" + m.Modifier(m.INT) + ")", LabelTypes.text, maxWidth), 2, 1);
                stats1_panel.Controls.Add(NewLabel("WIS", LabelTypes.subHeading, maxWidth), 3, 0);
                stats1_panel.Controls.Add(NewLabel(m.WIS.ToString() + " (" + m.Modifier(m.WIS) + ")", LabelTypes.text, maxWidth), 3, 1);
                stats1_panel.Controls.Add(NewLabel("CON", LabelTypes.subHeading, maxWidth), 4, 0);
                stats1_panel.Controls.Add(NewLabel(m.CON.ToString() + " (" + m.Modifier(m.CON) + ")", LabelTypes.text, maxWidth), 4, 1);
                stats1_panel.Controls.Add(NewLabel("CHA", LabelTypes.subHeading, maxWidth), 5, 0);
                stats1_panel.Controls.Add(NewLabel(m.CHA.ToString() + " (" + m.Modifier(m.CHA) + ")", LabelTypes.text, maxWidth), 5, 1);
                
                
                p.Controls.Add(stats1_panel);
                //Attacks
                p.Controls.Add(NewLabel("Attacks", LabelTypes.subHeading, maxWidth));
                if (m.nAttacks != "" && m.nAttacks != "1")
                    p.Controls.Add(NewLabel("Available attacks: " + m.nAttacks, LabelTypes.text, maxWidth - width6*3-10));
                TableLayoutPanel attack_panel = new TableLayoutPanel();
                attack_panel.AutoSize = true;
                attack_panel.ColumnCount = 6;
                attack_panel.GrowStyle = TableLayoutPanelGrowStyle.AddRows;
                int row = 0;
                foreach (Attack a in m.Attacks)
                {
                    attack_panel.RowCount += 2;
                    attack_panel.Controls.Add(NewLabel("   " + a.Name, LabelTypes.bold, maxWidth - width6*6), 0, row);
                    Label la = NewLabel(a.Type.ToString(), LabelTypes.italic, width6*2);
                    attack_panel.Controls.Add(la, 1, row);
                    attack_panel.Controls.Add(NewLabel(a.ToHit(), LabelTypes.text, width6), 2, row);
                    attack_panel.Controls.Add(NewLabel(a.Damage, LabelTypes.text, width6*2), 3, row);
                    if (a.Type == Attack.AttackType.Ranged)
                    {
                        if (a.MaxRangeDisadv == 0)
                            attack_panel.Controls.Add(NewLabel("Range: " + a.MaxRange.ToString(), LabelTypes.text, width6*2), 4, row);
                        else
                            attack_panel.Controls.Add(NewLabel("Range: " + a.MaxRange.ToString() +
                                " / " + a.MaxRangeDisadv.ToString(), LabelTypes.text, width6*3), 4, row);
                    }
                    if (a.Special != "")
                    {
                        attack_panel.Controls.Add(NewLabel(a.Special, LabelTypes.text, maxWidth - la.Right - 10), 1, row + 1);
                        attack_panel.SetColumnSpan(lastAdded, 4);
                    }

                    row += 2;
                }
                p.Controls.Add(attack_panel);
                //Spells
                if (m.SpellDC > 0)
                {
                    p.Controls.Add(NewLabel("Spells", LabelTypes.subHeading, maxWidth-50));
                    p.Controls.Add(NewLabel("    DC=" + m.SpellDC.ToString() + " Attack=" + Attack.ToHit(m.SpellRngAttack), LabelTypes.text, maxWidth-50));
                    TableLayoutPanel spell_panel = new TableLayoutPanel();
                    spell_panel.AutoSize = true;
                    spell_panel.RowCount = 1 + m.Spells.Count;
                    spell_panel.ColumnCount = 2;
                    if (m.Spells.ContainsKey(0))
                    {
                        spell_panel.Controls.Add(NewLabel("Cantrips:", LabelTypes.bold, maxWidth), 0, 0);
                        spell_panel.Controls.Add(NewLabel(m.Spells[0].Item2, LabelTypes.italic, maxWidth - lastAdded.Right - 30), 1, 0);
                    }
                    for (int lvl = 1; lvl < m.Spells.Count; lvl++)
                    {
                        if (m.Spells[lvl].Item1 <= 0) continue;
                        spell_panel.Controls.Add(NewLabel("Level " + lvl.ToString() + " (" + m.Spells[lvl].Item1.ToString() + " slots):",
                            LabelTypes.bold, maxWidth), 0, lvl);
                        spell_panel.Controls.Add(NewLabel(m.Spells[lvl].Item2, LabelTypes.italic, maxWidth - lastAdded.Right - 30), 1, lvl);
                    }
                    p.Controls.Add(spell_panel);
                }
                //Senses, Vulns, 
                TableLayoutPanel data_panel = new TableLayoutPanel();
                data_panel.AutoSize = true;
                int data_panel_row = 0;
                if (m.Saves != "")
                {
                    data_panel.Controls.Add(NewLabel("Saves:", LabelTypes.bold, maxWidth), 0, data_panel_row);
                    data_panel.Controls.Add(NewLabel(m.Saves, LabelTypes.text, maxWidth-lastAdded.Right-20), 1, data_panel_row++);
                }
                if (m.Skills != "")
                {
                    data_panel.Controls.Add(NewLabel("Skills:", LabelTypes.bold, maxWidth), 0, data_panel_row);
                    data_panel.Controls.Add(NewLabel(m.Skills, LabelTypes.text, maxWidth - lastAdded.Right - 20), 1, data_panel_row++);
                }
                if (m.Vuln != "")
                {
                    data_panel.Controls.Add(NewLabel("Vulnerable:", LabelTypes.bold, maxWidth), 0, data_panel_row);
                    data_panel.Controls.Add(NewLabel(m.Vuln, LabelTypes.text, maxWidth - lastAdded.Right - 20), 1, data_panel_row++);
                }
                if (m.Resist != "")
                {
                    data_panel.Controls.Add(NewLabel("Resistant:", LabelTypes.bold, maxWidth), 0, data_panel_row);
                    data_panel.Controls.Add(NewLabel(m.Resist, LabelTypes.text, maxWidth - lastAdded.Right - 20), 1, data_panel_row++);
                }
                if (m.Immune != "")
                {
                    data_panel.Controls.Add(NewLabel("Immunities:", LabelTypes.bold, maxWidth), 0, data_panel_row);
                    data_panel.Controls.Add(NewLabel(m.Immune, LabelTypes.text, maxWidth - lastAdded.Right - 20), 1, data_panel_row++);
                }

                data_panel.Controls.Add(NewLabel("Senses:", LabelTypes.bold, maxWidth), 0, data_panel_row);
                string sense = "";
                if (m.Darkvision > 0) sense += "Darkvision " + m.Darkvision.ToString();
                if (m.Truesight > 0) sense += (sense != "" ? ", " : "") + "Truevision " + m.Truesight.ToString();
                if (m.Blindsight > 0) sense += (sense != "" ? ", " : "") + "Blindsight " + m.Blindsight.ToString();
                if (m.PassivePerception > 0) sense += (sense != "" ? ", " : "") + "Passive Perception " + m.PassivePerception.ToString();
                data_panel.Controls.Add(NewLabel(sense, LabelTypes.text, maxWidth - lastAdded.Right - 20), 1, data_panel_row++);
                p.Controls.Add(data_panel);
                //Feats
                if (m.Feats.Count > 0)
                {
                    p.Controls.Add(NewLabel("Feats", LabelTypes.subHeading, maxWidth));
                    TableLayoutPanel feat_panel = new TableLayoutPanel();
                    feat_panel.MaximumSize = new System.Drawing.Size(maxWidth - 10, 0);
                    feat_panel.AutoSize = true;
                    feat_panel.ColumnCount = 2;
                    feat_panel.GrowStyle = TableLayoutPanelGrowStyle.AddRows;
                    int feat_row = 0;
                    int max_width = 0;
                    Graphics g = this.CreateGraphics();
                    foreach (string f in m.Feats.Keys)
                    {
                        Label l = NewLabel("  " + f, LabelTypes.bold, maxWidth);
                        int w = g.MeasureString(l.Text, l.Font).ToSize().Width;
                        if (max_width < w) max_width = w;
                        feat_panel.Controls.Add(l, 0, feat_row++);
                    }
                    feat_row = 0;
                    max_width = maxWidth - 40 - max_width;
                    foreach (string f in m.Feats.Keys)
                    {
                        Label l = NewLabel(m.Feats[f], LabelTypes.text, maxWidth);
                        l.MaximumSize = new System.Drawing.Size(max_width, 0);
                        feat_panel.Controls.Add(l, 1, feat_row++);
                    }
                    p.Controls.Add(feat_panel);
                }
                //Descr
                if (m.Descr != "")
                {
                    p.Controls.Add(NewLabel("Description", LabelTypes.subHeading, maxWidth));
                    p.Controls.Add(NewLabel("   " + m.Descr, LabelTypes.text, maxWidth-lastAdded.Right-20));
                }
            }
            return p;
        }

        void name_txt_TextChanged(object sender, EventArgs e)
        {
            TextBox t = sender as TextBox;
            ActualMonster m = t.Tag as ActualMonster;
            m.ActualName = t.Text;
        }

        void textBoxHP_TextChanged(object sender, EventArgs e)
        {
            TextBox t = sender as TextBox;
            ActualMonster mm = t.Parent.Parent.Tag as ActualMonster;

            //if not int, reset
            int newVal = 0;
            if (Int32.TryParse(t.Text, out newVal))
            {
                mm.HP = newVal;
                return;
            }

            t.Text = mm.HP.ToString();
        }

        #region Monster Printing
        private void butPrint_Click(object sender, EventArgs e)
        {
            PrintDialog pd = new PrintDialog();
            pd.AllowPrintToFile = true;
            pd.AllowSomePages = true;
            pd.AllowSelection = true;
            pd.AllowCurrentPage = false;
            pd.Document = new System.Drawing.Printing.PrintDocument();
            pd.Document.PrintPage += Document_PrintPage;
            pd.Document.BeginPrint += Document_BeginPrint;

            DialogResult r = pd.ShowDialog();
            if (r == System.Windows.Forms.DialogResult.OK)
            {
                pd.Document.Print();
            }
        }
        private void butEncPrintPreview_Click(object sender, EventArgs e)
        {
            PrintPreviewDialog ppd = new PrintPreviewDialog();
            ppd.Document = new System.Drawing.Printing.PrintDocument();
            ppd.Document.PrintPage += Document_PrintPage;
            ppd.Document.BeginPrint += Document_BeginPrint;
            ppd.UseAntiAlias = true;
            ppd.ShowDialog();
        }      
        void Document_BeginPrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            printIdx = 0;
            
        }
        static int printIdx = 0;
        void Document_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            int bottom = 0;
            if (printIdx == 0)
            {
                Label hdr = new Label();
                hdr.AutoSize = true;
                hdr.Font = new Font(FontFamily.GenericSansSerif, 24);
                hdr.BackColor = Color.White;
                hdr.Text = comboEncounters.Text;
                Size ac = hdr.GetPreferredSize(new Size(0,0));
                ac.Width = e.MarginBounds.Right - e.MarginBounds.Left - 50;
                ac.Height = 2 * ac.Height;
                hdr.Width = ac.Width;
                hdr.Height = ac.Height;
                using (Bitmap b = new Bitmap(ac.Width, ac.Height))
                {
                    hdr.DrawToBitmap(b, new Rectangle(0, 0, ac.Width, ac.Height));
                    e.Graphics.DrawImage(b, new Point(e.MarginBounds.Left, e.MarginBounds.Top));
                    bottom += ac.Height + 20;
                } 


            }
            for (int i = printIdx; i < curEncounter.Monsters.Count; i++)
            {
                TableLayoutPanel p = DisplayMonster(curEncounter.Monsters[i], e.MarginBounds.Width);
                p.PerformLayout();
                Size sz = p.GetPreferredSize(new Size(100, 100));
                p.SetBounds(0, 0, sz.Width, sz.Height);
                if (bottom == 0 || bottom + sz.Height < e.MarginBounds.Height)
                {
                    using (Bitmap b = new Bitmap(sz.Width, sz.Height))
                    {
                        p.DrawToBitmap(b, new Rectangle(0, 0, sz.Width, sz.Height));
                        e.Graphics.DrawImage(b, new Point(e.MarginBounds.Left, bottom+e.MarginBounds.Top));
                        bottom += sz.Height + 20;
                    }
                }
                else
                {
                    printIdx = i;
                    e.HasMorePages = true;
                    return;
                }
            }
            e.HasMorePages = false;
            return;
        }
        #endregion

        

        #endregion

        private void butEditSpell_Click(object sender, EventArgs e)
        {
            EditSpell es = new EditSpell(AllSpells);
            es.ShowDialog();
        }

        private void butEditMonsters_Click(object sender, EventArgs e)
        {
            EditMonster m = new EditMonster(Monsters);
            m.ShowDialog();
            if (m.SaveRequired)
            {
                SaveMonstersToDefault();
                TriggerMonstersChanged();
            }
        }

        private void butEncSaveAs_Click(object sender, EventArgs e)
        {
            BatchSaveEncAs b = new BatchSaveEncAs(SavedEnc, curEncounter);
            b.ShowDialog();
        }

        private void butEncSaveSingleAs_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "DocX files (*.docx)|*.docx|All files (*.*)|*.*";
            sfd.FilterIndex = 0;
            sfd.RestoreDirectory = true;

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                using (IanUtility.DocXWriter wr = new IanUtility.DocXWriter(sfd.FileName))
                {
                    Encounter.PopulateDocXStyles(wr);
                    curEncounter.PopulateDocX(wr);
                }
            }
        }

        private void butBatchUpdate_Click(object sender, EventArgs e)
        {
            BatchMonsterUpdate b = new BatchMonsterUpdate(Monsters);
            b.ShowDialog();

            if (b.SaveRequired)
            {
                SaveMonstersToDefault();
                TriggerMonstersChanged();
            }
        }

        private void butSaveMonsters_Click(object sender, EventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            if (dlg.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;

            SaveMonstersAsXML(dlg.FileName);
        }

        private void butImport_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "XML Files (*.xml)|*.xml|Excel *.xlsx|*.xlsx";
            dlg.RestoreDirectory = true;
            if (dlg.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;

            LoadMonsters(dlg.FileName, false);
            SaveMonstersToDefault();
        }




    }
}
