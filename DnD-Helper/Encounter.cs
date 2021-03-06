﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IanUtility;
using System.Runtime.Serialization;

namespace DnDHelper
{
      [CollectionDataContract
    (Name = "ArrayOfKeyValueOfstringEncounterD32yJ8R3",
    ItemName = "KeyValueOfstringEncounterD32yJ8R3")]
    //KeyName = "countryorregion",
    //ValueName = "capital")]
    public class EncounterLookup : Dictionary<string, Encounter> { }
    


    [Serializable]
    public class Encounter
    {
        public string Name = "";
        public SortedBindingList<ActualMonster> Monsters = new SortedBindingList<ActualMonster>();

        public int XP
        {
            get
            {
                int ret = 0;
                foreach (ActualMonster m in Monsters)
                    ret += m.Monster.XP;
                return ret;
            }
        }

        public Encounter() { }
        public Encounter(Encounter enc)
        {
            Name = enc.Name;
            foreach (ActualMonster m in enc.Monsters)
                Monsters.Add(m);
        }

        public void FixupMonster(MonsterLookup ms)
        {
            foreach (ActualMonster m in Monsters) m.FixupMonster(ms);
        }

        public ActualMonster AddMonster(Monster m, bool summary, int hp = -1, string actualName = "")
        {
            if (m == null) return null;

            ActualMonster mm = new ActualMonster();
            mm.Monster = m;
            if (hp == -1) mm.HP = m.GetHP();
            else mm.HP = hp;
            mm.Summary = summary;
            mm.ActualName = actualName;
            Monsters.Add(mm);

            return mm;
        }

        public static void PopulateDocXStyles(DocXWriter wr)
        {
            ActualMonster.PopulateDocXStyles(wr);
            wr.CreateStyle("EncTitle", "EncTitle", false, DocumentFormat.OpenXml.Wordprocessing.StyleValues.Paragraph,
                "Arial", 48, bold: true, beforeSpace: 0, firstLineIndent: 720);
            
        }
        public void PopulateDocX(DocXWriter wr, Random r=null)
        {
            wr.PushStyle("EncTitle");
            wr.PushCharFormat(false, false);
            wr.NewParagraph();
            wr.AppendText(Name + " (XP: "+XP+")");
            wr.PopCharFormat();
            wr.PopStyle();

            if (r == null) r = new Random();

            Dictionary<Monster, int> init = new Dictionary<Monster, int>();
            Dictionary<Monster, int> idx = new Dictionary<Monster, int>();

            foreach (ActualMonster m in Monsters)
            {
                if (!init.ContainsKey(m.Monster))
                {
                    init.Add(m.Monster, m.Monster.RollInitiative(r));
                    idx.Add(m.Monster, 1);
                }

                m.PopulateDocX(wr, idx[m.Monster]++, init[m.Monster]);
            }
        }

    }
}
