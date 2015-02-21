using IanUtility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Odbc;
using System.Data.OleDb;
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


        #region Monster save/load
        string GetConnectionString(string file)
        {
            Dictionary<string, string> props = new Dictionary<string, string>();

            // XLSX - Excel 2007, 2010, 2012, 2013
            props["Provider"] = "Microsoft.ACE.OLEDB.12.0";
            props["Extended Properties"] = "Excel 12.0 XML; HDR=YES; IMEX=1";
            props["Data Source"] = file;

            // XLS - Excel 2003 and Older
            //props["Provider"] = "Microsoft.Jet.OLEDB.4.0";
            //props["Extended Properties"] = "Excel 8.0";
            //props["Data Source"] = "C:\\MyExcel.xls";

            StringBuilder sb = new StringBuilder();

            foreach (KeyValuePair<string, string> prop in props)
            {
                sb.Append(prop.Key);
                sb.Append("=\"");
                sb.Append(prop.Value);
                sb.Append("\";");
            }

            return sb.ToString();
        }
        string GetOdbcConnectionString(string folder, string fname)
        {
            OleDbConnectionStringBuilder bld = new OleDbConnectionStringBuilder();
            //bld.Provider = "Microsoft.ACE.OLEDB.12.0";
            bld.Provider = "Microsoft.Jet.OLEDB.4.0";
            bld.DataSource = folder + "\\" + fname;
            bld.Add("Extended Properties", "Excel 8.0 Xml;HDR=YES");

            //string ret = "Driver={Microsoft Text Driver (*.txt; *.csv)};Dbq=";
            //ret += folder + ";ReadOnly=0;Extended Properties=\"Text;HDR=No;FMT=Delimited\"";
            //string ret = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source="+folder+"\\"+fname;
            //ret += ";Extended Properties=\"Excel 12.0 Xml;HDR=YES\"";
            //string ret = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source="+folder+"\\"+fname;
            //ret += ";Extended Properties=\"Excel 12.0;HDR=YES;IMEX=1\"";
            //string ret = "Driver={Microsoft Excel Driver (*.xls, *.xlsx, *.xlsm, *.xlsb)};DBQ=" + folder + "\\" + fname + ";";
            //ret += ";Extended Properties=\"Excel 12.0 Xml;HDR=YES\"";
            return bld.ConnectionString;
        }
        void LoadMonsters(string file, bool clear)
        {
            Dictionary<string, Monster> tempMonsters = new Dictionary<string, Monster>();
            if (!File.Exists(file)) return;

            string ext = Path.GetExtension(file).ToLower();

            try
            {
                if (ext == ".xml")
                {
                    Type[] extraTypes = new Type[] {typeof(Monster),
                        typeof(Attack),
                        typeof(Dictionary<int, Tuple<int, string>>), typeof(Tuple<int, string>),
                        typeof(List<Attack>)
                            };

                    DataContractSerializer ser2 = new DataContractSerializer(typeof(Dictionary<string, Monster>));
                    FileStream sin = new FileStream(file, FileMode.Open);
                    tempMonsters = (Dictionary<string, Monster>)ser2.ReadObject(sin);
                    sin.Close();
                }
                else
                {
                    IanUtility.ExcelParser p = IanUtility.ExcelParser.FromFile(file, "", true, true);


                    /*DataTable data = new DataTable();
                    
                    int i = file.LastIndexOf('\\');
                    string fname = file.Substring(i + 1);
                    string folder = file.Substring(0, i);
                    using (OdbcConnection conn = new OdbcConnection(GetOdbcConnectionString(folder, fname)))
                    {
                        conn.Open();
                        //OdbcDataAdapter adapter = new OdbcDataAdapter("SELECT * FROM [" + fname + "]", conn);
                        OdbcDataAdapter adapter = new OdbcDataAdapter("SELECT * FROM [Sheet1$]", conn);
                        adapter.Fill(data);
                    }*/
                    IanUtility.ExcelParser.Sheet s = p.Sheets.First();
                    foreach (IanUtility.ExcelParser.Row r in s.Rows)
                    {
                        Monster m = Monster.FromDataRow(s, r);
                        if (m != null) tempMonsters.Add(m.Name, m);
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Couldn't load monsters", "Error");
            }

            if (clear)
            {
                Monsters = tempMonsters;
            }
            else
            {
                //Merge
                List<string> existing = new List<string>();
                int import = 0;
                foreach (string m in tempMonsters.Keys)
                {
                    if (Monsters.ContainsKey(m))
                    {
                        existing.Add(m);
                    }
                    else
                    {
                        import++;
                        Monsters.Add(m, tempMonsters[m]);
                    }
                }

                if (existing.Count > 0)
                {
                    if (existing.Count < 20)
                        MessageBox.Show("Note: Import didn't import the following existing monsters:-\n   " +
                            String.Join(", ", existing) + "\n Imported " + import.ToString() + " monsters");
                    else
                        MessageBox.Show("Note: Import didn't import " + existing.Count.ToString() + " monsters.\n" +
                            " Imported " + import.ToString() + " monsters");
                }
            }

            //Update combo
            comboMonsters.DataSource = null;
            comboMonsters.Items.Clear();
            if (Monsters.Count > 0)
            {
                List<string> names = Monsters.Keys.ToList();
                names.Sort();
                //BindingSource bind = new BindingSource(Monsters, null);
                comboMonsters.DataSource = names;
                //comboMonsters.DisplayMember = "Key";
                //comboMonsters.ValueMember = "Value";
                if (Monsters.Count > 0) comboMonsters.SelectedIndex = 0;
                else comboMonsters.SelectedIndex = -1;
            }
            TriggerMonstersChanged();
        }
        void SaveMonstersToDefault()
        {
            SaveMonstersAsXML(Properties.Settings.Default.MonsterFile);
        }
        void SaveMonsters(string file)
        {
            string ext = Path.GetExtension(file).ToLower();

            if (!Directory.Exists(Path.GetDirectoryName(file))) Directory.CreateDirectory(Path.GetDirectoryName(file));
            
            switch (ext)
            {
                case ".xml":
                    SaveMonstersAsXML(file);
                    break;
                case ".xlsx":
                    Monster.ToXlsX(file, Monsters);
                    break;
                default:
                    MessageBox.Show("ERROR: Cannot save files with extension " + ext + "\n Only XML, and XLSX supported.");
                    break;
            }
        }
        void SaveMonstersAsXML(string file)
        {
            if (!Directory.Exists(Path.GetDirectoryName(file))) Directory.CreateDirectory(Path.GetDirectoryName(file));
            
            Type[] extraTypes = new Type[] {typeof(Monster),
                typeof(Attack),
                typeof(Dictionary<int, Tuple<int, string>>), typeof(Tuple<int, string>),
                typeof(List<Attack>)
            };

            DataContractSerializer ser2 = new DataContractSerializer(typeof(Dictionary<string, Monster>));
            FileStream s = new FileStream(file, FileMode.Create);
            XmlDictionaryWriter xdw = XmlDictionaryWriter.CreateTextWriter(s);
            ser2.WriteObject(xdw, Monsters);
            xdw.Close();
            s.Close();
        }
        #endregion

        #region Encounter save/load
        private void butPickEncounterDB_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "XML files (*.xml)|*.xml";
            sfd.FilterIndex = 0;
            sfd.RestoreDirectory = true;

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                Properties.Settings.Default.EncountersFile = sfd.FileName;
                Properties.Settings.Default.Save();
                lblEncounterDB.Text = sfd.FileName;
                SaveEncountersAsXML(sfd.FileName);
            }
        }
        IanUtility.SortedBindingList<string> SaveEncounterList = new IanUtility.SortedBindingList<string>();
        Dictionary<string, Encounter> SavedEnc = new Dictionary<string, Encounter>();
        void SaveEncountersDefault()
        {
            SaveEncountersAsXML(Properties.Settings.Default.EncountersFile);
        } 
        void LoadEncountersAsBinary(string base64Blob)
        {
            string s = base64Blob;
            if (s == "" || s == null) return;

            using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(s)))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Context = new System.Runtime.Serialization.StreamingContext(bf.Context.State, Monsters);
                SavedEnc = (Dictionary<string, Encounter>)bf.Deserialize(ms);
            }

            SaveEncounterList.RaiseListChangedEvents = false;
            SaveEncounterList.Clear();
            foreach (string ss in SavedEnc.Keys) SaveEncounterList.Add(ss);
            SaveEncounterList.RaiseListChangedEvents = true;

            ((IBindingList)SaveEncounterList).ApplySort(null, ListSortDirection.Ascending);
        }
        void LoadEncountersAsXml(string fname)
        {
            //Type[] extraTypes = new Type[] { typeof(ActualMonster),
            //    typeof(SortedBindingList<ActualMonster>)
            //};
            if (!File.Exists(fname)) return;

            DataContractSerializer ser2 = new DataContractSerializer(typeof(Dictionary<string, Encounter>));
            FileStream sin = new FileStream(fname, FileMode.Open);
            SavedEnc = (Dictionary<string, Encounter>)ser2.ReadObject(sin);
            sin.Close();

            if (SavedEnc == null) return;
            foreach (Encounter en in SavedEnc.Values) en.FixupMonster(Monsters);

            SaveEncounterList.RaiseListChangedEvents = false;
            SaveEncounterList.Clear();
            foreach (string ss in SavedEnc.Keys) SaveEncounterList.Add(ss);
            SaveEncounterList.RaiseListChangedEvents = true;

            ((IBindingList)SaveEncounterList).ApplySort(null, ListSortDirection.Ascending);
        }
        string SaveEncountersAsBinary()
        {
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(ms, SavedEnc);
                //bf.Serialize(ms, newSaved);
                ms.Position = 0;
                byte[] buffer = new byte[(int)ms.Length];
                ms.Read(buffer, 0, buffer.Length);
                return Convert.ToBase64String(buffer);
            }
        }
        void SaveEncountersAsXML(string name)
        {
            //Type[] extraTypes = new Type[] { typeof(ActualMonster),
            //    typeof(SortedBindingList<ActualMonster>)
            //};
            if (!Directory.Exists(Path.GetDirectoryName(name))) Directory.CreateDirectory(Path.GetDirectoryName(name));
            
            DataContractSerializer ser2 = new DataContractSerializer(typeof(Dictionary<string, Encounter>));
            FileStream s = new FileStream(name, FileMode.Create);
            XmlDictionaryWriter xdw = XmlDictionaryWriter.CreateTextWriter(s);
            ser2.WriteObject(xdw, SavedEnc);
            xdw.Close();
            s.Close();
        }
        void SaveEncounter(string name)
        {
            if (SavedEnc.ContainsKey(name))
            {
                DialogResult dlg = MessageBox.Show("An encounter with name " + name + " already exists. Overwrite?", "Warning", MessageBoxButtons.YesNo);
                if (dlg == System.Windows.Forms.DialogResult.No) return;
                curEncounter.Name = name;
                SavedEnc[name] = new Encounter(curEncounter);
            }
            else
            {
                curEncounter.Name = name;
                SavedEnc.Add(name, new Encounter(curEncounter));
            }

            SaveEncountersDefault();
        }
        void LoadEncounter(string name)
        {
            curEncounter.Monsters.ListChanged -= activeMonsters_ListChanged;
            ClearMonsters();
            if (!SavedEnc.ContainsKey(name)) return;

            curEncounter.Name = name;
            curEncounter.Monsters = SavedEnc[name].Monsters;

            ((IBindingList)curEncounter.Monsters).ApplySort(null, ListSortDirection.Descending);
            curEncounter.Monsters.ListChanged += activeMonsters_ListChanged;
            curEncounter.Monsters.ResetBindings();
        }
        #endregion

        #region Spellbooks
        private void butPickSpellbookDB_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "XML files (*.xml)|*.xml";
            sfd.FilterIndex = 0;
            sfd.RestoreDirectory = true;

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                Properties.Settings.Default.SpellbooksFile = sfd.FileName;
                Properties.Settings.Default.Save();
                SaveSpellbooksAsXML(sfd.FileName);
            }
        }
        void SaveSpellbooksDefault()
        {
            SaveSpellbooksAsXML(Properties.Settings.Default.SpellbooksFile);
        }
        void LoadSpellbooksAsXML(string fname)
        {
            if (!File.Exists(fname)) return;
            try
            {
                DataContractSerializer ser2 = new DataContractSerializer(typeof(Dictionary<string, Spellbook>));
                FileStream sin = new FileStream(fname, FileMode.Open);
                Spellbooks = (Dictionary<string, Spellbook>)ser2.ReadObject(sin);
                sin.Close();

                if (Spellbooks != null)
                {
                    foreach (Spellbook sp in Spellbooks.Values) sp.FixupSpells(AllSpells);
                }

                UpdateSpellbookList();
            }
            catch (Exception)
            {
                MessageBox.Show("Couldn't load Spellbook: " + fname);
            }
        }
        void SaveSpellbooksAsXML(string fname)
        {
            if (fname == "") return;
            if (!Directory.Exists(Path.GetDirectoryName(fname))) Directory.CreateDirectory(Path.GetDirectoryName(fname));
            
            DataContractSerializer ser2 = new DataContractSerializer(typeof(Dictionary<string, Spellbook>));
            FileStream s = new FileStream(fname, FileMode.Create);
            XmlDictionaryWriter xdw = XmlDictionaryWriter.CreateTextWriter(s);
            ser2.WriteObject(xdw, Spellbooks);
            xdw.Close();
            s.Close();
        }
        /*void LoadSpellbooksAsBlob(string b64Blob)
        {
            string s = b64Blob;
            if (s == "" || s == null) return;

            using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(s)))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Context = new StreamingContext(bf.Context.State, (object)AllSpells);
                //StreamingContext c = bf.Context;
                Spellbooks = (Dictionary<string, Spellbook>)bf.Deserialize(ms);
            }

            //updateComboList
            UpdateSpellbookList();

        }
        string SaveSpellbooksAsBlob()
        {
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(ms, Spellbooks);
                ms.Position = 0;
                byte[] buffer = new byte[(int)ms.Length];
                ms.Read(buffer, 0, buffer.Length);
                return Convert.ToBase64String(buffer);
            }
        }
        */
        void SaveSpellbook(string name)
        {
            if (Spellbooks.ContainsKey(name))
            {
                DialogResult dlg = MessageBox.Show("A spellbook with name " + name + " already exists. Overwrite?", "Warning", MessageBoxButtons.YesNo);
                if (dlg == System.Windows.Forms.DialogResult.No) return;
                Spellbooks[name] = curSpellbook;
            }
            else Spellbooks.Add(name, curSpellbook);

            UpdateSpellbookList();

            SaveSpellbooksDefault();
        }
        void LoadSpellbook(string name)
        {
            if (!Spellbooks.ContainsKey(name))
            {
                curSpellbook = new Spellbook();
            }
            else
            {
                curSpellbook = Spellbooks[name];
            }
            cmbSpellbook.SelectedText = curSpellbook.Name;
            RaiseSpellbookChanged(this, new ListChangedEventArgs(ListChangedType.Reset, -1));
        }
        #endregion

        #region Spells
        private void butSpellSave_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "XML Files (*.xml)|*.xml|Excel *.xlsx|*.xlsx";
            sfd.FilterIndex = 0;
            sfd.RestoreDirectory = true;

            if (sfd.ShowDialog() == DialogResult.OK)
            {
				SaveSpells(sfd.FileName);
            }
        }
        private void butSpellImport_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "XML Files (*.xml)|*.xml|Excel *.xlsx|*.xlsx";
            dlg.FilterIndex = 0;
            dlg.Multiselect = false;
            dlg.ReadOnlyChecked = true;
            dlg.ValidateNames = true;
            dlg.CheckFileExists = true;
            dlg.FileName = Properties.Settings.Default.SpellsFile;
            DialogResult r = dlg.ShowDialog();
            if (r == DialogResult.OK)
            {
                LoadSpells(dlg.FileName, true);
                SaveSpells(Properties.Settings.Default.SpellsFile);
            }
        }
        private void butSpellsLoadCombined_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "XML Files (*.xml)|*.xml|Excel *.xlsx|*.xlsx";
            dlg.FilterIndex = 0;
            dlg.Multiselect = false;
            dlg.ReadOnlyChecked = true;
            dlg.ValidateNames = true;
            dlg.CheckFileExists = true;
            dlg.FileName = Properties.Settings.Default.SpellsFile;
            DialogResult r = dlg.ShowDialog();
            if (r == DialogResult.OK)
            {
                Properties.Settings.Default.SpellsFile = dlg.FileName;
                Properties.Settings.Default.Save();
                lblFilenameSpellCombined.Text = dlg.FileName;
                LoadSpells(dlg.FileName);
            }
        }
        private void butSpellLoadTextFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Multiselect = false;
            dlg.ReadOnlyChecked = true;
            dlg.ValidateNames = true;
            dlg.CheckFileExists = true;
            dlg.FileName = Properties.Settings.Default.SpellsFile;
            DialogResult r = dlg.ShowDialog();
            if (r == DialogResult.OK)
            {
                int updated = 0;
                List<Spell> spells = Spell.FromText(dlg.FileName);
                List<string> failed = new List<string>();
                //Merge cells
                foreach (Spell sp in spells)
                {
                    bool updatedB = false;
                    //search for hit
                    Spell currSp = AllSpells.Where(s => s.Equals(sp.Name)).FirstOrDefault();
                    if (currSp == null)
                    {
                        failed.Add(sp.Name);
                        continue;
                    }

                    //descr, rtfdescr, range, casting time, duration, components
                    if (currSp.Description == "")
                    {
                        currSp.Description = sp.Description;
                        currSp.rtfDescription = sp.rtfDescription;
                        updatedB = true;
                    }
                    else
                    {
                        ; //skip if we've already got a description
                    }
                    if (currSp.sRange == null || currSp.sRange == "") { currSp.sRange = sp.sRange; updatedB = true; }
                    if (currSp.sCastingTime == null || currSp.sCastingTime == "") { currSp.sCastingTime = sp.sCastingTime; updatedB = true; }
                    if (currSp.sDuration == null || currSp.sDuration == "") { currSp.sDuration = sp.sDuration; updatedB = true; }
                    if ((currSp.sComponents == null || currSp.sComponents == "") && (!currSp.Verbal && !currSp.Somatic && !currSp.Material)) { currSp.sComponents = sp.sComponents; updatedB = true; }
                    currSp.Breakout();
                    if (updatedB) updated++;
                }
                string msg = "Updated " + updated.ToString() + "/" + spells.Count.ToString() + " entries.";
                if (failed.Count > 0) msg += "Failed entries are:-\n" + String.Join("\n", failed);
                SaveSpells(Properties.Settings.Default.SpellsFile);
                MessageBox.Show(msg);

            }

        }
        void SaveSpells(string fname)
        {
            if (fname == "") return;
			string ext = Path.GetExtension(fname).ToLower();
            if (!Directory.Exists(Path.GetDirectoryName(fname))) Directory.CreateDirectory(Path.GetDirectoryName(fname));
            
            if (ext == ".xml") SaveSpellsAsXML(fname);
			else SaveSpellsAsXlsx(fname);
        }
        void LoadSpells(string fname, bool mergeWithExisting=false)
        {
            if (fname == "") return;
            string ext = Path.GetExtension(fname).ToLower();
            if (!File.Exists(fname)) return;

		
            List<Spell> l = null;
            try
            {
                if (ext == ".xml") l = LoadSpellsAsXML(fname);
                else l = LoadSpellsAsXlsx(fname);
            }
            catch (Exception)
            {
                MessageBox.Show("Couldn't load Spells from " + fname);
                return;
            }

            if (l == null)
            {
                MessageBox.Show("Couldn't load Spells from " + fname);
                return;
            }

            if (!mergeWithExisting) AllSpells = l;
            else
            {
                int added = 0;
                List<Spell> dup = new List<Spell>();
                foreach (Spell sp in l)
                {
                    if (AllSpells.Where(a => String.Compare(a.Name, sp.Name) == 0).Count() == 0)
                    {
                        AllSpells.Add(sp);
                        added++;
                    }
                    else {
                        dup.Add(sp);
                    }
                }
                string msg = "Added " + added.ToString() + " spells.\n";
                if (dup.Count > 10)
                    msg += " but " + dup.Count().ToString() + " duplicates skipped.";
                else if (dup.Count > 0)
                    msg += " Duplicates: " + String.Join(", ", dup);
                MessageBox.Show(msg);
             
            }

            PopulateSpellTree();
        }
        List<Spell> LoadSpellsAsXlsx(string fname)
        {
            if (!File.Exists(fname)) return null;

            return Spell.FromXlsX_Combined(fname);
            
        }
        void SaveSpellsAsXlsx(string fname)
        {
            if (!Directory.Exists(Path.GetDirectoryName(fname))) Directory.CreateDirectory(Path.GetDirectoryName(fname));
            
            Spell.SaveAsXlsX(fname, AllSpells);
        }
        List<Spell> LoadSpellsAsXML(string fname)
        {
            if (!File.Exists(fname)) return null;

            DataContractSerializer ser2 = new DataContractSerializer(typeof(List<Spell>));
            FileStream sin = new FileStream(fname, FileMode.Open);
            List<Spell> ret = (List<Spell>)ser2.ReadObject(sin);
            sin.Close();
            return ret;
        }
		void SaveSpellsAsXML(string fname)
        {
            if (!Directory.Exists(Path.GetDirectoryName(fname))) Directory.CreateDirectory(Path.GetDirectoryName(fname));
            
            DataContractSerializer ser2 = new DataContractSerializer(typeof(List<Spell>));
            FileStream s = new FileStream(fname, FileMode.Create);
            XmlDictionaryWriter xdw = XmlDictionaryWriter.CreateTextWriter(s);
            ser2.WriteObject(xdw, AllSpells);
            xdw.Close();
            s.Close();
        }
        #endregion

    }
}
