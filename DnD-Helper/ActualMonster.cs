using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IanUtility;
using System.Runtime.Serialization;

namespace DnDMonsters
{
    [Serializable]
    public class ActualMonster : IComparer<ActualMonster>
    {
        [NonSerialized()]
        public Monster Monster;

        public string MonsterName;
        public int HP;
        public bool Summary;
        public string ActualName = "";
        public string Name
        {
            get { if (ActualName == "") return Monster.Name; return ActualName; }
            set { ActualName = value; }
        }

        [OnSerializing()]
        internal void OnSerializingMethod(StreamingContext context)
        {
            MonsterName = Monster.Name;
        }
        [OnDeserialized()]
        internal void OnDeserializedMethod(StreamingContext context)
        {
            if (context.Context == null) return;
            Dictionary<string, Monster> ms = (Dictionary<string, Monster>)context.Context;
            FixupMonster(ms);   
        }
        internal void FixupMonster(Dictionary<string, Monster> ms)
        {
            if (ms.ContainsKey(MonsterName))
            {
                Monster = ms[MonsterName];
            }
            else
                Monster = null;
        }



        int IComparer<ActualMonster>.Compare(ActualMonster a, ActualMonster b)
        {
            if (a == null && b == null) return 0;
            if (a == null)
            {
                return -1;
            }
            if (b == null) return 1;
            int i;
            //compare by name
            i = String.Compare(a.Monster.Name, b.Monster.Name, StringComparison.CurrentCultureIgnoreCase);
            if (i < 0)
            {
                return i;
            }
            else if (i > 0) return i;
            //compare by summary
            if (a.Summary != b.Summary)
            {
                if (a.Summary) return 1;
                else
                {
                    return -1; //b.Summary
                }
            }
            //compare by HP
            if (a.HP == b.HP) return 0;
            if (a.HP > b.HP)
            {
                return -1;
            }
            return 1;
        }


        public static void PopulateDocXStyles(DocXWriter wr)
        {
            wr.CreateStyle("AMname", "AMname", false, DocumentFormat.OpenXml.Wordprocessing.StyleValues.Paragraph,
                "Arial", 32, bold:true, beforeSpace:18, firstLineIndent: 0);
            wr.CreateStyle("AMsmallitalic", "AMsmallitalic", false, DocumentFormat.OpenXml.Wordprocessing.StyleValues.Paragraph,
                "Arial", 18, italic: true, firstLineIndent: 0);
            wr.CreateStyle("AMnormal", "AMnormal", true, DocumentFormat.OpenXml.Wordprocessing.StyleValues.Paragraph,
                "Arial", 22, firstLineIndent: 0);

        }
        public void PopulateDocX(DocXWriter wr, int idx=-1, int initiative=-1)
        {
            wr.PushCharFormat(false, false);
            wr.PushStyle("AMname");
            wr.NewParagraph();
            string name = "";
            name += Monster.Name;
            if (idx != -1)
                name += " #"+idx.ToString();
            if (ActualName != "") name += " (" + ActualName + ")";
            if (initiative != -1)
                name += " \tInit: " + initiative.ToString();
            wr.AppendText(name);

                //wr.PushCharFormat(true, false);
                //wr.PushStyle("AMnormal");
                wr.AppendText(" \tHP:  " + HP.ToString());
                //wr.PopStyle();
                //wr.PopCharFormat();
                wr.EndParagraph();
            
            if(!Summary)
            {
                wr.PushStyle("AMsmallitalic");
                wr.NewParagraph();
                wr.AppendText(Monster.Size + " " + Monster.Type + ", " + Monster.Alignment);
                wr.PopStyle();
                wr.PushStyle("AMnormal");

                wr.NewParagraph();
                wr.PushCharFormat(true, false);
                wr.AppendText("AC: ");
                wr.PopCharFormat();
                wr.AppendText(Monster.AC.ToString()+"   ");

                wr.PushCharFormat(true, false);
                wr.AppendText("Speed: ");
                wr.PopCharFormat();
                wr.AppendText(Monster.Speed+"   ");

                wr.PushCharFormat(true, false);
                wr.AppendText("MaxHP: ");
                wr.PopCharFormat();
                wr.AppendText(Monster.HP+"   ");

                wr.PushCharFormat(true, false);
                wr.AppendText("XP: ");
                wr.PopCharFormat();
                wr.AppendText(Monster.XP.ToString()+"   ");

                //wr.PushCharFormat(true, false);
                //wr.AppendText("HP: ");
                //wr.PopCharFormat();
                //wr.AppendText(HP.ToString());

                wr.TableStart();
                wr.PushCharFormat(true, false);
                wr.TableNewRow();
                wr.TableNewCell();
                wr.AppendText("STR");
                wr.TableNewCell();
                wr.AppendText("DEX");
                wr.TableNewCell();
                wr.AppendText("INT");
                wr.TableNewCell();
                wr.AppendText("WIS");
                wr.TableNewCell();
                wr.AppendText("CON");
                wr.TableNewCell();
                wr.AppendText("CHA");
                wr.PopCharFormat();
                wr.TableNewRow();
                wr.TableNewCell();
                wr.AppendText(" "+Monster.STR.ToString() + " (" + Monster.Modifier(Monster.STR) + ") ");
                wr.TableNewCell();
                wr.AppendText(" " + Monster.DEX.ToString() + " (" + Monster.Modifier(Monster.DEX) + ") ");
                wr.TableNewCell();
                wr.AppendText(" " + Monster.INT.ToString() + " (" + Monster.Modifier(Monster.INT) + ") ");
                wr.TableNewCell();
                wr.AppendText(" " + Monster.WIS.ToString() + " (" + Monster.Modifier(Monster.WIS) + ") ");
                wr.TableNewCell();
                wr.AppendText(" " + Monster.CON.ToString() + " (" + Monster.Modifier(Monster.CON) + ") ");
                wr.TableNewCell();
                wr.AppendText(" " + Monster.CHA.ToString() + " (" + Monster.Modifier(Monster.CHA) + ") ");
                wr.TableEnd();

                wr.NewParagraph();
                wr.PushCharFormat(true, false);
                wr.AppendText("Attacks: " + Monster.nAttacks);
                wr.PopCharFormat();

                foreach (Attack a in Monster.Attacks)
                {
                    wr.NewParagraph();
                    wr.PushCharFormat(true, false);
                    wr.AppendText("   " + a.Name+" \t");
                    wr.PopCharFormat();
                    wr.PushCharFormat(false, true);
                    wr.AppendText(a.Type.ToString() + "\t");
                    wr.PopCharFormat();
                    wr.AppendText(a.AttackMod + "\t");
                    wr.AppendText(a.Damage + "\t");
                    if (a.Type == Attack.AttackType.Ranged)
                    {
                        wr.AppendText("Range: ");
                        wr.AppendText(a.MaxRange.ToString() + "/" + a.MaxRangeDisadv.ToString() + "\t");
                    }
                    wr.AppendText(a.Special);
                }

                if (Monster.SpellDC>0)
                {
                    wr.NewParagraph();
                    wr.PushCharFormat(true, false);
                    wr.AppendText("Spellcasting (DC=" + Monster.SpellDC.ToString() + ", Rng=" + Monster.SpellRngAttack.ToString() + ")");
                    wr.PopCharFormat();
                    if (Monster.Spells.ContainsKey(0))
                    {
                        wr.NewParagraph();
                        wr.AppendText("   Cantrips: " + Monster.Spells[0].Item2);
                    }
                    List<int> lvls = Monster.Spells.Keys.ToList();
                    lvls.Sort();
                    foreach (int i in lvls)
                    {
                        wr.NewParagraph();
                        wr.AppendText("   Level " + i.ToString() + " (" + Monster.Spells[i].Item1.ToString() + " slots): " + Monster.Spells[i].Item2);
                    }
                    
                }

                if (Monster.Skills != "")
                {
                    wr.NewParagraph();
                    wr.PushCharFormat(true, false);
                    wr.AppendText("Skills:  ");
                    wr.PopCharFormat();
                    wr.AppendText(Monster.Skills);
                }
                if (Monster.Saves != "")
                {
                    wr.NewParagraph();
                    wr.PushCharFormat(true, false);
                    wr.AppendText("Saves:  ");
                    wr.PopCharFormat();
                    wr.AppendText(Monster.Saves);
                }
                if (Monster.Vuln != "")
                {
                    wr.NewParagraph();
                    wr.PushCharFormat(true, false);
                    wr.AppendText("Vulns:  ");
                    wr.PopCharFormat();
                    wr.AppendText(Monster.Vuln);
                }
                if (Monster.Immune != "")
                {
                    wr.NewParagraph();
                    wr.PushCharFormat(true, false);
                    wr.AppendText("Immun:  ");
                    wr.PopCharFormat();
                    wr.AppendText(Monster.Immune);
                }

                wr.NewParagraph();
                wr.PushCharFormat(true, false);
                wr.AppendText("Senses: ");
                wr.PopCharFormat();
                string senses = "";
                if (Monster.Blindsight > 0) 
                    senses = "Blindsight " + Monster.Blindsight.ToString();
                if (Monster.Darkvision > 0)
                    senses = senses + (senses == "" ? "" : ", ") + "Darkvision " + Monster.Darkvision.ToString();
                if (Monster.Tremorsense > 0)
                    senses = senses + (senses == "" ? "" : ", ") + "Tremorsense " + Monster.Tremorsense.ToString();
                if (Monster.Truesight > 0) 
                    senses = senses + (senses == "" ? "" : ", ") + "Truesight " + Monster.Truesight.ToString();
                if (Monster.PassivePerception > 0) 
                    senses = senses + (senses == "" ? "" : ", ") + "Passive Perception " + Monster.PassivePerception.ToString();
                wr.AppendText(senses);

                if (Monster.Feats != null && Monster.Feats.Count > 0)
                {
                    wr.NewParagraph();
                    wr.PushCharFormat(true, false);
                    wr.AppendText("Feats");
                    wr.PopCharFormat();
                    foreach (string ft in Monster.Feats.Keys)
                    {
                        wr.NewParagraph();
                        wr.PushCharFormat(true, false);
                        wr.AppendText("   " + ft + " \t");
                        wr.PopCharFormat();
                        wr.AppendText(Monster.Feats[ft]);
                    }
                }
                if (Monster.Descr != "")
                {
                    wr.NewParagraph();
                    wr.PushCharFormat(true, false);
                    wr.AppendText("Description");
                    wr.LineBreak();
                    wr.PopCharFormat();
                    wr.AppendText(Monster.Descr);
                }
                
                wr.EndParagraph();
                wr.PopStyle();

            }

            wr.PopStyle();
            wr.PopCharFormat();
            
        }

    }
}
