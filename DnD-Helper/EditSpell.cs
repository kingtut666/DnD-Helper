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
    public partial class EditSpell : Form
    {
        List<Spell> Spells;
        List<Spell> AllSpells;
        Spell cur = null;
        public EditSpell(List<Spell> spells)
        {
            InitializeComponent();

            AllSpells = spells;
            UpdateSpellList();

            //List schools
            string[] schools = new string[] { "Abjuration", "Conjuration", "Divination", "Enchantment", "Evocation", "Illusion", "Necromancy", "Transmutation", "" };
            comboSchool.DataSource = schools;

        }
        void UpdateSpellList()
        {
            if (checkShowMissingOnly.Checked)
            {
                Spells = AllSpells.Where(s => s.Description == null || s.Description == "").ToList();
            }
            else Spells = AllSpells;

            if (checkMissingMat.Checked)
            {
                Spells = AllSpells.Where(s => s.Material && 
                    (s.MaterialNeeded == null || s.MaterialNeeded == "")).ToList();
            }
            


            //Load Spell List
            comboSpellList.DataSource = Spells;
            comboSpellList.DisplayMember = "Name";
            comboSpellList.ValueMember = null;
            comboSpellList.SelectedIndex = (Spells.Count>0?0:-1);
        }

        private void comboSpellList_SelectedValueChanged(object sender, EventArgs e)
        {
            cur = comboSpellList.SelectedItem as Spell;

            //update fields
            //TOP
            numericLevel.Value = cur.Level;
            comboSchool.Text = cur.School;
            checkRitual.Checked = cur.IsRitual;
            //CLASSES
            checkBard.Checked = cur.Classes.HasFlag(Classes.Bard);
            checkCleric.Checked = cur.Classes.HasFlag(Classes.Cleric);
            checkDruid.Checked = cur.Classes.HasFlag(Classes.Druid);
            checkPaladin.Checked = cur.Classes.HasFlag(Classes.Paladin);
            checkRanger.Checked = cur.Classes.HasFlag(Classes.Ranger);
            checkSorcerer.Checked = cur.Classes.HasFlag(Classes.Sorcerer);
            checkWarlock.Checked = cur.Classes.HasFlag(Classes.Warlock);
            checkWizard.Checked = cur.Classes.HasFlag(Classes.Wizard);
            //Duration,CastingTime,Range
            textCastingTime.Text = cur.sCastingTime;
            textDuration.Text = cur.sDuration;
            textRange.Text = cur.sRange;
            //Components
            checkSomatic.Checked = cur.Somatic;
            checkVerbal.Checked = cur.Verbal;
            checkMaterial.Checked = cur.Material;
            richTextMaterial.Text = cur.MaterialNeeded;
            //Description
            richDescr.Rtf = cur.rtfDescription;
        }

        private void butSave_Click(object sender, EventArgs e)
        {

            //update fields
            //TOP
            cur.Level = (int)numericLevel.Value;
            cur.School = comboSchool.Text;
            cur.IsRitual = checkRitual.Checked;
            //CLASSES
            Classes newClass = 0;
            if(checkBard.Checked) newClass |= Classes.Bard;
            if(checkCleric.Checked) newClass |= Classes.Cleric;
            if(checkDruid.Checked) newClass |= Classes.Druid;
            if(checkPaladin.Checked) newClass |= Classes.Paladin;
            if(checkRanger.Checked) newClass |= Classes.Ranger;
            if(checkSorcerer.Checked) newClass |= Classes.Sorcerer;
            if(checkWarlock.Checked) newClass |= Classes.Warlock;
            if (checkWizard.Checked) newClass |= Classes.Wizard;
            cur.Classes = newClass;
            //Duration,CastingTime,Range
            cur.sCastingTime = textCastingTime.Text;
            cur.sDuration = textDuration.Text;
            cur.sRange = textRange.Text;
            //Components
            cur.Somatic = checkSomatic.Checked;
            cur.Verbal = checkVerbal.Checked;
            cur.Material = checkMaterial.Checked;
            cur.MaterialNeeded = richTextMaterial.Text;
            //Description
            cur.rtfDescription = richDescr.Rtf;
            cur.Description = richDescr.Text;
        }

        private void butAddNew_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void butDelete_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void checkShowMissingOnly_CheckedChanged(object sender, EventArgs e)
        {
            UpdateSpellList();
        }

        private void checkMissingMat_CheckedChanged(object sender, EventArgs e)
        {
            UpdateSpellList();
        }
    }
}
