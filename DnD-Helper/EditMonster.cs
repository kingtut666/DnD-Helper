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
    public partial class EditMonster : Form
    {
        string[] Alignments = new string[] { "LG", "LN", "LE", "NG", "NN", "NE", "CG", "CN", "CE", "Unaligned", "Any" };
        string[] Types = new string[] { "", "Aberration", "Beast", "Dragon", "Giant", "Humanoid", "Monstrosity",
            "Ooze", "Plant", "Undead" };
        HashSet<int> XPs = new HashSet<int>();
        List<int> XPsSorted;

        public bool SaveRequired = false;

        Dictionary<string,Monster> Monsters = null;
        public EditMonster(Dictionary<string,Monster> monsters)
        {
            InitializeComponent();
            Monsters = monsters;
            Initialize();
        }
        void Initialize()
        {
            XPs.Clear();

            foreach (Monster m in Monsters.Values) XPs.Add(m.XP);
            XPsSorted = XPs.ToList();
            XPsSorted.Sort();

            //Size, Type, Alignment, XP
            comboAlignment.DataSource = Alignments;
            string[] Sizes = Enum.GetNames(typeof(MonsterSizes));
            comboSize.DataSource = Sizes;
            comboType.DataSource = Types;
            comboXP.DataSource = XPsSorted;

            //Filters
            //XP
            List<string> filterXPs = new List<string>();
            filterXPs.Add("Any");
            foreach (int i in XPsSorted) filterXPs.Add(i.ToString());
            cmbSelXP.DataSource = filterXPs;
            cmbSelXP.SelectedIndex = 0;
            //Type
            List<string> filterTypes = new List<string>();
            filterTypes.Add("Any");
            foreach (string s in Types) filterTypes.Add(s);
            cmbSelType.DataSource = filterTypes;
            cmbSelType.SelectedIndex = 0;
            //Env
            List<string> filterEnv = new List<string>();
            filterEnv.Add("Any");
            foreach (string s in Enum.GetNames(typeof(Environments))) filterEnv.Add(s);
            cmbSelEnviron.DataSource = filterEnv;
            cmbSelEnviron.SelectedIndex = 0;

            cmbSelPic.SelectedIndex = 0;

            dontDisplay = false;

            dgStats.Rows.Add();
            for (int i = 0; i < 6; i++)
                dgStats.Rows[0].Cells[i].Value = "10";

            DisplayMonsters();
        }

        bool dontDisplay = true;
        void DisplayMonsters()
        {
            if (dontDisplay) return;
            List<Monster> mm = Monsters.Values.ToList();
            string fXP = cmbSelXP.SelectedItem.ToString();
            string fType = cmbSelType.SelectedItem.ToString();
            string fEnv = cmbSelEnviron.SelectedItem.ToString();
            string fPic = cmbSelPic.SelectedItem.ToString();

            //filter by XP
            if (fXP != "Any" && fXP!="")
            {
                int xp = Int32.Parse(fXP);
                mm = mm.Where(a => a.XP == xp).ToList();
            }
            //filter by environ
            if (fEnv != "Any" && fEnv != "")
            {
                Environments env = (Environments)Enum.Parse(typeof(Environments), fEnv);
                if (env == Environments.None) mm = mm.Where(a => a.Environ == Environments.None).ToList();
                else mm = mm.Where(a => (a.Environ & env) == env).ToList();
            }
            //filter by type
            if (fType != "Any" && fType != "")
            {
                mm = mm.Where(a => a.Type == fType).ToList();
            }
            //filter by pic
            if (fPic != "Any" && fPic != "")
            {
                if (fPic == "Yes") mm = mm.Where(a => a.SourceFile != null).ToList();
                else mm = mm.Where(a => a.SourceFile == null).ToList();
            }

            comboMonsterName.DataSource = null;
            comboMonsterName.Items.Clear();

            mm.Sort((a, b) => String.Compare(a.Name, b.Name));
            comboMonsterName.DataSource = mm;
            comboMonsterName.DisplayMember = "Name";
            comboMonsterName.SelectedIndex = (mm.Count > 0 ? 0 : -1);
        }


        private void butAdd_Click(object sender, EventArgs e)
        {
            if (Save(true)) Initialize();
        }

        private void butSave_Click(object sender, EventArgs e)
        {
            Save(false);
        }

        private void butDel_Click(object sender, EventArgs e)
        {
            if (cur == null) return;
            if (!Monsters.ContainsValue(cur)) return; //shouldn't happen
            if (!Monsters.ContainsKey(cur.Name)) return; //shouldn't happen
            Monsters.Remove(cur.Name);
            Initialize();
            SaveRequired = true;
        }

        private void butBatchUpdate_Click(object sender, EventArgs e)
        {
            BatchMonsterUpdate b = new BatchMonsterUpdate(Monsters);
            b.ShowDialog();
            Initialize();
        }

        Monster cur = null;
        private void comboMonsterName_SelectedValueChanged(object sender, EventArgs e)
        {
            cur = comboMonsterName.SelectedValue as Monster;
            if (cur == null) return;
            UpdateAll();
        }

        void UpdateAll()
        {
            if (cur == null) return;
            //XP, Size, Type, Alignment
            comboXP.Text = cur.XP.ToString();
            comboSize.Text = cur.Size;
            comboType.Text = cur.Type;
            comboAlignment.Text = cur.Alignment;
            //AC, HP, Speed
            numAC.Value = cur.AC;
            textHP.Text = cur.HP;
            textSpeed.Text = cur.Speed;
            //Stats
            dgStats.Rows[0].Cells["STR"].Value = cur.STR.ToString();
            dgStats.Rows[0].Cells["DEX"].Value = cur.DEX.ToString();
            dgStats.Rows[0].Cells["INT"].Value = cur.INT.ToString();
            dgStats.Rows[0].Cells["WIS"].Value = cur.WIS.ToString();
            dgStats.Rows[0].Cells["CON"].Value = cur.CON.ToString();
            dgStats.Rows[0].Cells["CHA"].Value = cur.CHA.ToString();
            //Environments
            if (cur.Environ.HasFlag(Environments.Arctic)) checkArctic.Checked = true; else checkArctic.Checked = false;
            if (cur.Environ.HasFlag(Environments.Coastal)) checkCoastal.Checked = true; else checkCoastal.Checked = false;
            if (cur.Environ.HasFlag(Environments.Desert)) checkDesert.Checked = true; else checkDesert.Checked = false;
            if (cur.Environ.HasFlag(Environments.Forest)) checkForest.Checked = true; else checkForest.Checked = false;
            if (cur.Environ.HasFlag(Environments.Grassland)) checkGrassland.Checked = true; else checkGrassland.Checked = false;
            if (cur.Environ.HasFlag(Environments.Hill)) checkHill.Checked = true; else checkHill.Checked = false;
            if (cur.Environ.HasFlag(Environments.Mountain)) checkMountain.Checked = true; else checkMountain.Checked = false;
            if (cur.Environ.HasFlag(Environments.Swamp)) checkSwamp.Checked = true; else checkSwamp.Checked = false;
            if (cur.Environ.HasFlag(Environments.Underdark)) checkUnderdark.Checked = true; else checkUnderdark.Checked = false;
            if (cur.Environ.HasFlag(Environments.Underwater)) checkUnderwater.Checked = true; else checkUnderwater.Checked = false;
            if (cur.Environ.HasFlag(Environments.Urban)) checkUrban.Checked = true; else checkUrban.Checked = false;
            //Senses
            numBlindsight.Value = cur.Blindsight;
            numDarksight.Value = cur.Darkvision;
            numTruesight.Value = cur.Truesight;
            numTremorsense.Value = cur.Tremorsense;
            numPassPercep.Value = cur.PassivePerception;
            //Vuln, Immune, Saves, Skills
            textVuln.Text = cur.Vuln;
            textImmune.Text = cur.Immune;
            textResistant.Text = cur.Resist;
            textSave.Text = cur.Saves;
            textSkill.Text = cur.Skills;
            dgFeats.Rows.Clear();
            foreach(string ftName in cur.Feats.Keys){
                int idx = dgFeats.Rows.Add();
                dgFeats.Rows[idx].Cells["colFeatName"].Value = ftName;
                dgFeats.Rows[idx].Cells["colFeatDescr"].Value = cur.Feats[ftName];
            }
            //Attacks
            textNAttacks.Text = cur.nAttacks;
            dgAttacks.Rows.Clear();
            foreach(Attack a in cur.Attacks){
                int idx = dgAttacks.Rows.Add();
                dgAttacks.Rows[idx].Cells["colName"].Value = a.Name;
                dgAttacks.Rows[idx].Cells["colType"].Value = (a.Type == Attack.AttackType.Melee ? "Melee" : "Ranged");
                dgAttacks.Rows[idx].Cells["colHitMod"].Value = a.ToHit();
                if(a.Type == Attack.AttackType.Melee){
                    dgAttacks.Rows[idx].Cells["colRange"].Value = "";
                    dgAttacks.Rows[idx].Cells["colMaxRange"].Value = "";
                }
                else {
                    dgAttacks.Rows[idx].Cells["colRange"].Value = a.MaxRange.ToString();
                    dgAttacks.Rows[idx].Cells["colMaxRange"].Value = a.MaxRangeDisadv.ToString();
                }
                dgAttacks.Rows[idx].Cells["colDmg"].Value = a.Damage;
                //dgAttacks.Rows[idx].Cells["colDmgType"].Value; //TODO: Break out damage type and display
                dgAttacks.Rows[idx].Cells["colOther"].Value = a.Special;
            }
            //Descr
            rtfDescr.Text = cur.Descr;
            txtSource.Text = cur.Source;
            txtLang.Text = cur.Lang;
            //DC, Attack, Spells
            dgSpells.Rows.Clear();
            numSpDC.Value = cur.SpellDC;
            numSpAtk.Value = cur.SpellRngAttack;
            if (cur.SpellDC > 0)
            {
                List<int> spl = cur.Spells.Keys.ToList();
                spl.Sort();
                foreach (int lvl in spl)
                {
                    int idx = dgSpells.Rows.Add();
                    if ((lvl != 0 && cur.Spells[lvl].Item1 == 0) || cur.Spells[lvl].Item2 == "") continue;
                    if (lvl == 0)
                        dgSpells.Rows[idx].Cells["colSlots"].Value = "";
                    else
                        dgSpells.Rows[idx].Cells["colSlots"].Value = cur.Spells[lvl].Item1.ToString();
                    dgSpells.Rows[idx].Cells["colLevel"].Value = lvl.ToString();
                    dgSpells.Rows[idx].Cells["colSpells"].Value = cur.Spells[lvl].Item2;
                }
            }
            //picture
            txtPicture.Text = GetImageDetailsText();
            if (txtPicture.Text != "")
            {
                picMonster.Image = cur.GetFrontImage(cur.Name);
            }
            else
            {
                picMonster.Image = null;
            }
        }

        string GetImageDetailsText()
        {
            if (cur == null || cur.SourceFile==null) return "";
            return (cur.SourceFile.IsFile ? cur.SourceFile.LocalPath :
                cur.SourceFile.ToString());        
        }

        bool Save(bool toAdd)
        {
            if (cur == null && !toAdd)
            {
                MessageBox.Show("Cannot add new monsters unless you click Add");
                return false;
            }
            if (toAdd)
            {
                cur = new Monster();
                cur.Name = comboMonsterName.Text;
            }
            //XP, Size, Type, Alignment
            cur.XP  = Int32.Parse(comboXP.Text);
            cur.Size = comboSize.Text;
            cur.Type = comboType.Text;
            cur.Alignment = comboAlignment.Text;
            //AC, HP, Speed
            cur.AC = (int)numAC.Value;
            cur.HP = textHP.Text;
            cur.Speed = textSpeed.Text;
            //STATS
            if (!Int32.TryParse(dgStats.Rows[0].Cells["STR"].EditedFormattedValue.ToString(), out cur.STR))
            {
                MessageBox.Show("ERR: Cannot parse STR - must be a number");
            }
            if (!Int32.TryParse(dgStats.Rows[0].Cells["DEX"].EditedFormattedValue.ToString(), out cur.DEX))
            {
                MessageBox.Show("ERR: Cannot parse DEX - must be a number");
            }
            if (!Int32.TryParse(dgStats.Rows[0].Cells["INT"].EditedFormattedValue.ToString(), out cur.INT))
            {
                MessageBox.Show("ERR: Cannot parse INT - must be a number");
            }
            if (!Int32.TryParse(dgStats.Rows[0].Cells["WIS"].EditedFormattedValue.ToString(), out cur.WIS))
            {
                MessageBox.Show("ERR: Cannot parse WIS - must be a number");
            }
            if (!Int32.TryParse(dgStats.Rows[0].Cells["CON"].EditedFormattedValue.ToString(), out cur.CON))
            {
                MessageBox.Show("ERR: Cannot parse CON - must be a number");
            }
            if (!Int32.TryParse(dgStats.Rows[0].Cells["CHA"].EditedFormattedValue.ToString(), out cur.CHA))
            {
                MessageBox.Show("ERR: Cannot parse CHA - must be a number");
            }
            //Environments
            cur.Environ = 0;
            if (checkArctic.Checked) cur.Environ |= Environments.Arctic;
            if (checkCoastal.Checked) cur.Environ |= Environments.Coastal;
            if (checkDesert.Checked) cur.Environ |= Environments.Desert;
            if (checkForest.Checked) cur.Environ |= Environments.Forest;
            if (checkGrassland.Checked) cur.Environ |= Environments.Grassland;
            if (checkHill.Checked) cur.Environ |= Environments.Hill;
            if (checkMountain.Checked) cur.Environ |= Environments.Mountain;
            if (checkSwamp.Checked) cur.Environ |= Environments.Swamp;
            if (checkUnderdark.Checked) cur.Environ |= Environments.Underdark;
            if (checkUnderwater.Checked) cur.Environ |= Environments.Underwater;
            if (checkUrban.Checked) cur.Environ |= Environments.Urban;
            //Senses
            cur.Blindsight = (int)numBlindsight.Value;
            cur.Darkvision = (int)numDarksight.Value;
            cur.Truesight = (int)numTruesight.Value;
            cur.Tremorsense = (int)numTremorsense.Value;
            cur.PassivePerception = (int)numPassPercep.Value;
            //Vuln, Immune, Saves, Skills
            cur.Vuln = textVuln.Text;
            cur.Immune = textImmune.Text;
            cur.Resist = textResistant.Text;
            cur.Saves = textSave.Text;
            cur.Skills = textSkill.Text;
            cur.Feats.Clear();
            foreach (DataGridViewRow r in dgFeats.Rows)
            {
                if (r.Cells["colFeatName"].Value==null) continue;
                cur.Feats.Add(r.Cells["colFeatName"].Value.ToString(), r.Cells["colFeatDescr"].EditedFormattedValue.ToString());
            }
            //Attacks
            cur.nAttacks = textNAttacks.Text;
            cur.Attacks.Clear();
            foreach (DataGridViewRow r in dgAttacks.Rows)
            {
                if (r.Cells["colName"].Value == null) continue;
                Attack a = new Attack();
                a.Name = r.Cells["colName"].Value.ToString();
                if (r.Cells["colType"].Value.ToString() == "Melee") a.Type = Attack.AttackType.Melee;
                else a.Type = Attack.AttackType.Ranged;
                if(!Int32.TryParse(r.Cells["colHitMod"].Value.ToString(), out a.AttackMod)){
                    MessageBox.Show("Hit modifiers need to be a number, rather than: "+r.Cells["colHitMod"].Value.ToString());
                    return false;
                }
                if (a.Type == Attack.AttackType.Ranged)
                {
                    
                    if (r.Cells["colRange"].Value == null || !Int32.TryParse(r.Cells["colRange"].Value.ToString(), out a.MaxRange))
                    {
                        MessageBox.Show("Range need to be a number, rather than: " + (r.Cells["colRange"].Value==null?"blank": r.Cells["colRange"].Value.ToString()));
                        return false;
                    }
                    if (r.Cells["colMaxRange"].Value == null || !Int32.TryParse(r.Cells["colMaxRange"].Value.ToString(), out a.MaxRangeDisadv))
                    {
                        MessageBox.Show("Max Range modifiers need to be a number, rather than: " + (r.Cells["colMaxRange"].Value==null?"blank":r.Cells["colMaxRange"].Value.ToString()));
                        return false;
                    }
                }
                a.Damage = r.Cells["colDmg"].Value.ToString();
                //dgAttacks.Rows[idx].Cells["colDmgType"].Value; //TODO: Break out damage type and display
                a.Special = r.Cells["colOther"].EditedFormattedValue.ToString();
                cur.Attacks.Add(a);
            }
            //Descr
            cur.Descr = rtfDescr.Text;
            cur.Source = txtSource.Text;
            cur.Lang = txtLang.Text;
            //DC, Attack, Spells
            cur.SpellDC = (int)numSpDC.Value;
            cur.SpellRngAttack = (int)numSpAtk.Value;
            cur.Spells.Clear();
            if (numSpDC.Value > 0)
            {
                foreach (DataGridViewRow r in dgSpells.Rows)
                {
                    if (r.Cells["colLevel"].Value == null) continue;
                    int lvl = 0;
                    int slots = 0;
                    if (!Int32.TryParse(r.Cells["colLevel"].Value.ToString(), out lvl))
                    {
                        MessageBox.Show("Spell level must be a number, rather than: " + r.Cells["colLevel"].Value.ToString());
                        return false;
                    }
                    if (lvl == 0) { slots = 0; }
                    else
                    {
                        if (!Int32.TryParse(r.Cells["colSlots"].Value.ToString(), out slots))
                        {
                            MessageBox.Show("Spell slots must be a number, rather than: " + r.Cells["colSlots"].Value.ToString());
                            return false;
                        }
                    }
                    if(!cur.Spells.ContainsKey(lvl)) cur.Spells.Add(lvl, new Tuple<int, string>(slots, r.Cells["colSpells"].Value.ToString()));

                }
            }

            if (txtPicture.Text != GetImageDetailsText())
            {
                if (txtPicture.Text == "") cur.SourceFile = null;
                else cur.SourceFile = new Uri(txtPicture.Text);
            }

            if (toAdd) Monsters.Add(cur.Name, cur);

            SaveRequired = true;
            return true;
        }

        private void butPickPic_Click(object sender, EventArgs e)
        {
            //TODO: Allow picking of pictures via either local file or URL
            OpenFileDialog dlg = new OpenFileDialog();
            if (dlg.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;

            txtPicture.Text = dlg.FileName;
            CreateTempPic();
        }

        private void dgFeats_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void txtPicture_Leave(object sender, EventArgs e)
        {
            if (txtPicture.Text == GetImageDetailsText()) return;
            CreateTempPic();
        }
        void CreateTempPic(){
            //it has changed, let's try to refresh image
            Uri temp = new Uri(txtPicture.Text);
            picMonster.Image = cur.GetFrontImage("TMP: " + cur.Name, temp);
        }

        private void Selections_SelectedIndexChanged(object sender, EventArgs e)
        {
            DisplayMonsters();
        }

        private void EditMonster_Load(object sender, EventArgs e)
        {

        }

    }
}
