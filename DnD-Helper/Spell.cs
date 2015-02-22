using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Text.RegularExpressions;
using System.IO;
using IanUtility;

namespace DnDHelper
{
    /*
    Wall of Stone 
5th-level evocation 
Casting Time: 1 action 
Range: 120 feet 
Components: V, S, M (a small block of granite) 
Duration: Concentration, up to 10 minutes 
A nonmagical wall of solid stone springs into existence at a point you choose within range. The wall is 6 inches thick and is composed of ten 10-foot-by-10-foot panels. Each panel must be contiguous with at least one other panel. Alternatively, you can create 10-foot-by-20-foot panels that are only 3 inches thick. 
If the wall cuts through a creature’s space when it appears, the creature is pushed to one side of the wall (your choice). If a creature would be surrounded on all sides by the wall (or the wall and another solid surface), that creature can make a Dexterity saving throw. On a success, it can use its reaction to move up to its speed so that it is no longer enclosed by the wall. 
The wall can have any shape you desire, though it can’t occupy the same space as a creature or object. The wall doesn’t need to be vertical or rest on any firm foundation. It must, however, merge with and be solidly supported by existing stone. Thus, you can use this spell to bridge a chasm or create a ramp. 
If you create a span greater than 20 feet in length, you must halve the size of each panel to create supports. You can crudely shape the wall to create crenellations, battlements, and so on. 
The wall is an object made of stone that can be damaged and thus breached. Each panel has AC 15 and 30 hit points per inch of thickness. Reducing a panel to 0 hit points destroys it and might cause connected panels to collapse at the DM’s discretion. 
If you maintain your concentration on this spell for its whole duration, the wall becomes permanent and can’t be dispelled. Otherwise, the wall disappears when the spell ends. 
*/
    [Flags]
    public enum Classes { Bard=0x01, Cleric=0x02, Druid=0x04, Paladin=0x08, 
        Ranger=0x10, Sorcerer=0x20, Warlock=0x40, Wizard=0x80 }

    public partial class Spell
    {
        public string Name { get; set; }
        public string LevelSchool
        {
            get {
                if (Level == 0) return School + " Cantrip";
                if (Level == 1) return "1st-Level " + School;
                if (Level == 2) return "2nd-Level " + School;
                if (Level == 3) return "3rd-Level " + School;
                return Level.ToString()+"th-Level " + School;

            }
        }
        public int Level;
        public string School;
        public bool IsRitual;
        public string sCastingTime;
        public int CastingTime = -1;
        public string CastingTimeUnit;
        public string CastingTimeCondition;
        public string sRange;
        public int RangeSz = -1;
        public string RangeUnits;
        public string RangeShape;
        public bool RangeSight = false;
        public bool RangeSelf = false;
        public bool RangeTouch = false;
        public bool RangeSpecial = false;
        public bool RangeUnlimited = false;
        public string sComponents;
        public bool Verbal;
        public bool Somatic;
        public bool Material;
        public string MaterialNeeded;
        public string sDuration;
        public bool DurationConcentration = false;
        public bool DurationInstantaneous = false;
        public bool DurationSpecial = false;
        public bool DurationUntilDispelled = false;
        public bool DurationUntilTriggered = false;
        public int DurationMax = -1;
        public string DurationUnits;
        public string Description = "";
        public string rtfDescription = @"{\rtf1\ansi{\fonttbl{\f0\fswiss\fprq2\fcharset0 Microsoft Sans Serif;}{\f1\fnil\fcharset0 Courier New;}}\fs19 ";

        //Ritual	Page	Bard	Cleric	Druid	Paladin	Ranger	Sorcerer	Warlock	Wizard
        public Classes Classes;
        public string ClassesString
        {
            get
            {
                return System.Enum.Format(typeof(Classes), Classes, "f");
            }
        }
        public int PHBPage;

        void BreakoutCastingTime() {
            //(\d+) (bonus )?action(.*)
            //(\d+) minute(.*)
            //(\d+) hour(.*)
            //(\d+) reaction(, .*)?
            //XXX or XXX  : TODO

            if (sCastingTime == null || sCastingTime == "") return;
            Regex r = new Regex(@"(\d+) (bonus )?(action|minute|hour|reaction)(, (.*))?");
            Match m = r.Match(sCastingTime);
            
            if (!m.Success || !Int32.TryParse(m.Groups[1].Value, out CastingTime))
            {
                throw new NotImplementedException();
            }
            CastingTimeUnit = m.Groups[2].Value + m.Groups[3].Value;
            CastingTimeCondition = m.Groups[5].Value;
        }
        void BreakoutRange() {
            if (sRange == null || sRange == "") return;
            //Self
            //Self \((\d+)-foot radius)
            //Touch
            //(\d+) feet
            //(\d+) miles
            //Self \((\d+)-foot (cone|cube|cylinder|sphere|radius)
            Regex r1 = new Regex(@"Self(\s+)?(\((\d+)-(\w+)( |-)(cone|cube|cylinder|radius)(\s+)?(sphere)?\))?");
            Regex r2 = new Regex(@"(\d+) (\w+)");

            if (sRange == "Touch")
            {
                RangeTouch = true;
                return;
            }
            if (sRange == "Special")
            {
                RangeSpecial = true;
                return;
            }
            if (sRange == "Sight")
            {
                RangeSight = true;
                return;
            }
            if (sRange == "Unlimited")
            {
                RangeUnlimited = true;
                return;
            }
            Match m = r1.Match(sRange);
            if (m.Success)
            {
                RangeSelf = true;
                if (m.Groups[3].Value != "")
                {
                    RangeSz = Int32.Parse(m.Groups[3].Value);
                    RangeUnits = m.Groups[4].Value;
                    if (m.Groups[8].Value != "") RangeShape = m.Groups[8].Value;
                    else RangeShape = m.Groups[6].Value;
                }
                return;
            }

            m = r2.Match(sRange);
            if (m.Success)
            {
                RangeSz = Int32.Parse(m.Groups[1].Value);
                RangeUnits = m.Groups[2].Value;
                return;
            }

            throw new NotImplementedException();
        }
        void BreakoutComponents() {
            if (sComponents == null || sComponents == "") return;

            //V,S,M (components)
            Regex r = new Regex(@"(V|S|M)(,\s*)?(S)?(,\s*)?(M)?(\s*\((.*)\))?");
            Match m = r.Match(sComponents);
            if (m.Success)
            {
                switch (m.Groups[1].Value)
                {
                    case "V": Verbal = true; break;
                    case "S": Somatic = true; break;
                    case "M": Material = true;
                        break;
                    default:
                        throw new NotImplementedException();
                }
                if (m.Groups[3].Value == "S") Somatic = true;
                if (m.Groups[5].Value == "M") Material = true;
                if (m.Groups[7].Value != "") MaterialNeeded = m.Groups[7].Value;
                return;
            }
            else throw new NotImplementedException();
        }
        void BreakoutDuration() {
            sDuration = sDuration.Trim();
            if (sDuration == null || sDuration == "") return;
            //Concentration, up to (\d+) (minute|hour)(.*)
            //Instantaneous
            //(\d+) (hour|round)(.*)
            //Special
            if (sDuration == "Instantaneous")
            {
                DurationInstantaneous = true;
                return;
            }
            if (sDuration == "Special")
            {
                DurationSpecial = true;
                return;
            }
            if (sDuration == "Until dispelled")
            {
                DurationUntilDispelled = true;
                return;
            }
            if (sDuration == "Until dispelled or triggered")
            {
                DurationUntilDispelled = true;
                DurationUntilTriggered = true;
                return;
            } 
            Regex r1 = new Regex(@"Concentration, up to (\d+) (\w+)");
            Regex r2 = new Regex(@"(\d+) (\w+)");

            Match m = r1.Match(sDuration);
            if (m.Success)
            {
                DurationConcentration = true;
                DurationMax = Int32.Parse(m.Groups[1].Value);
                DurationUnits = m.Groups[2].Value;
                return;
            }
            m = r2.Match(sDuration);
            if (m.Success)
            {
                DurationMax = Int32.Parse(m.Groups[1].Value);
                DurationUnits = m.Groups[2].Value;
                return;
            }
            throw new NotImplementedException();
        }
        public void Breakout()
        {
            BreakoutCastingTime();
            BreakoutComponents();
            BreakoutDuration();
            BreakoutRange();
        }








        public bool Equals(string name)
        {
            char[] arr = name.ToLower().ToCharArray();
            arr = Array.FindAll<char>(arr, (c => (char.IsLetterOrDigit(c)
                                              || char.IsWhiteSpace(c)
                                              || c == '-')));
            //str = new string(arr);
            char[] arr2 = Name.ToLower().ToCharArray();
            arr2 = Array.FindAll<char>(arr2, (c => (char.IsLetterOrDigit(c)
                                              || char.IsWhiteSpace(c)
                                              || c == '-')));
            
            if (arr.Length != arr2.Length) return false;
            for (int i = 0; i < arr.Length; i++) if (arr[i] != arr2[i]) return false;
            return true;
        }

        static Regex rSpellLevel = new Regex(@"(\d+)..-level (.*)");
        static Regex rRange = new Regex("Range: (.*)");
        static Regex rCastingTime = new Regex("Casting Time: (.*)");
        static Regex rComponents = new Regex("Components: (.*)");
        static Regex rDuration = new Regex("Duration: (.*)");
        static string GetRtf(Run r, Text t, RunProperties p)
        {
            bool close = false;
            string ret = "";
            if (p != null)
            {
                ret += "{";
                if (p.Bold != null) ret += "\\b";
                if (p.Italic != null) ret += "\\i ";
                close = true;
            }

            if (r!=null && r.HasChildren)
            {
                RunProperties pp = null;
                foreach (object o in r.ChildElements)
                {
                    RunProperties tryP = o as RunProperties;
                    if (tryP != null) pp = tryP;
                    else
                    {
                        Run tryR = o as Run;
                        Text tryText = o as Text;
                        
                        if (tryR != null || tryText != null) ret += GetRtf(tryR, tryText, pp);
                    }
                }
            }
            else if (t != null)
            {
                ret += t.Text;
            }
            else
            {
                ret += r.InnerText;
            }
            if (close) ret += "}";

            return ret;
        }

        public static List<Spell> FromXlsX_Combined(string fname)
        {
            List<Spell> ret = new List<Spell>();
            IanUtility.ExcelParser p;
            try
            {
                p = IanUtility.ExcelParser.FromFile(fname, null, true, true);
            }
            catch (Exception)
            {
                return ret;
            }
            IanUtility.ExcelParser.Sheet s = p.Sheets[0];
            //Level	Name	School	Ritual	Page	Bard	Cleric	Druid	Paladin	Ranger	Sorcerer	Warlock	Wizard
            foreach (IanUtility.ExcelParser.Row r in s.Rows)
            {
                Spell sp = new Spell();
                sp.Level = r["Level"].Int;
                sp.Name = r["Name"].Text.Trim();
                sp.School = r["School"].Text.Trim();
                sp.IsRitual = r["IsRitual"].Bool;
                sp.PHBPage = r["PHBPage"].Int;
                if (r["Bard"].Bool) sp.Classes |= Classes.Bard;
                if (r["Cleric"].Bool) sp.Classes |= Classes.Cleric;
                if (r["Druid"].Bool) sp.Classes |= Classes.Druid;
                if (r["Paladin"].Bool) sp.Classes |= Classes.Paladin;
                if (r["Ranger"].Bool) sp.Classes |= Classes.Ranger;
                if (r["Sorcerer"].Bool) sp.Classes |= Classes.Sorcerer;
                if (r["Warlock"].Bool) sp.Classes |= Classes.Warlock;
                if (r["Wizard"].Bool) sp.Classes |= Classes.Wizard;
                sp.sCastingTime = r["CastingTime"].Text;
                sp.sRange = r["Range"].Text;
                sp.Verbal = r["Verbal"].Bool;
                sp.Somatic = r["Somatic"].Bool;
                sp.Material = r["Material"].Bool;
                sp.MaterialNeeded = r["MaterialNeeded"].Text;
                sp.sDuration = r["Duration"].Text;
                sp.Description = r["Description"].Text;
                sp.rtfDescription = r["RtfDescription"].Text;

                sp.Breakout();
                ret.Add(sp);
            }


            return ret;
        }

        /*public static List<Spell> FromXlsX(string fname)
        {
            List<Spell> ret = new List<Spell>();

            IanUtility.ExcelParser p = IanUtility.ExcelParser.FromFile(fname, null, true, true);

            IanUtility.ExcelParser.Sheet s = p.Sheets[0];
            //Level	Name	School	Ritual	Page	Bard	Cleric	Druid	Paladin	Ranger	Sorcerer	Warlock	Wizard
            foreach (IanUtility.ExcelParser.Row r in s.Rows)
            {
                Spell sp = new Spell();
                sp.Level = r["Level"].Int;
                sp.Name = r["Name"].Text.Trim();
                sp.School = r["School"].Text.Trim();
                sp.IsRitual = r["Ritual"].Text.Equals("Yes", StringComparison.CurrentCultureIgnoreCase);
                sp.PHBPage = r["Page"].Int;
                if (r["Bard"].Text.Equals("Yes", StringComparison.CurrentCultureIgnoreCase)) sp.Classes |= Classes.Bard;
                if (r["Cleric"].Text.Equals("Yes", StringComparison.CurrentCultureIgnoreCase)) sp.Classes |= Classes.Cleric;
                if (r["Druid"].Text.Equals("Yes", StringComparison.CurrentCultureIgnoreCase)) sp.Classes |= Classes.Druid;
                if (r["Paladin"].Text.Equals("Yes", StringComparison.CurrentCultureIgnoreCase)) sp.Classes |= Classes.Paladin;
                if (r["Ranger"].Text.Equals("Yes", StringComparison.CurrentCultureIgnoreCase)) sp.Classes |= Classes.Ranger;
                if (r["Sorcerer"].Text.Equals("Yes", StringComparison.CurrentCultureIgnoreCase)) sp.Classes |= Classes.Sorcerer;
                if (r["Warlock"].Text.Equals("Yes", StringComparison.CurrentCultureIgnoreCase)) sp.Classes |= Classes.Warlock;
                if (r["Wizard"].Text.Equals("Yes", StringComparison.CurrentCultureIgnoreCase)) sp.Classes |= Classes.Wizard;

                sp.Breakout();
                ret.Add(sp);
            }


            return ret;
        }
        */
        public static bool SaveAsXlsX(string fname, List<Spell> spells)
        {
            IanUtility.ExcelParser ep = new IanUtility.ExcelParser();
            IanUtility.ExcelParser.Sheet s = ep.NewSheet("Spells");

            //Column headers
            s.CreateNewColumn("Name");
            s.CreateNewColumn("Level");
            s.CreateNewColumn("School");
            s.CreateNewColumn("IsRitual");
            s.CreateNewColumn("CastingTime");
            s.CreateNewColumn("Range");
            s.CreateNewColumn("Verbal");
            s.CreateNewColumn("Somatic");
            s.CreateNewColumn("Material");
            s.CreateNewColumn("MaterialNeeded");
            s.CreateNewColumn("Duration");
            s.CreateNewColumn("PHBPage");
            s.CreateNewColumn("Bard");
            s.CreateNewColumn("Cleric");
            s.CreateNewColumn("Druid");
            s.CreateNewColumn("Paladin");
            s.CreateNewColumn("Ranger");
            s.CreateNewColumn("Sorcerer");
            s.CreateNewColumn("Warlock");
            s.CreateNewColumn("Wizard");
            s.CreateNewColumn("Description");
            s.CreateNewColumn("RtfDescription");

            foreach (Spell sp in spells)
            {
                IanUtility.ExcelParser.Row r = s.CreateNewRow();
                r["Name"].Text = sp.Name;
                r["Level"].Int = sp.Level;
                r["School"].Text = sp.School;
                r["IsRitual"].Bool = sp.IsRitual;
                r["CastingTime"].Text = sp.sCastingTime;
                r["Range"].Text = sp.sRange;
                r["Verbal"].Bool = sp.Verbal;
                r["Somatic"].Bool = sp.Somatic;
                r["Material"].Bool = sp.Material;
                r["MaterialNeeded"].Text = sp.MaterialNeeded;
                r["Duration"].Text = sp.sDuration;
                r["PHBPage"].Int = sp.PHBPage;
                r["Bard"].Bool = sp.Classes.HasFlag(Classes.Bard);
                r["Cleric"].Bool = sp.Classes.HasFlag(Classes.Cleric);
                r["Druid"].Bool = sp.Classes.HasFlag(Classes.Druid);
                r["Paladin"].Bool = sp.Classes.HasFlag(Classes.Paladin);
                r["Ranger"].Bool = sp.Classes.HasFlag(Classes.Ranger);
                r["Sorcerer"].Bool = sp.Classes.HasFlag(Classes.Sorcerer);
                r["Warlock"].Bool = sp.Classes.HasFlag(Classes.Warlock);
                r["Wizard"].Bool = sp.Classes.HasFlag(Classes.Wizard);
                r["Description"].Text = sp.Description;
                r["RtfDescription"].Text = sp.rtfDescription;
            }

            return ep.SaveAs(fname);
        }

        public static List<Spell> FromText(string fname)
        {
            List<Spell> ret = new List<Spell>();
            using (TextReader r = new StreamReader(fname)) { 
                string line = "";
                Spell sp = null;
                bool inDescr = false;
                bool hasLevel = false;
                bool rtfOpened = false;
                while ((line = r.ReadLine())!=null)
                {
                    //line = line.Trim();
                    if (line == "") continue; //skip blank lines
                    //if ALL CAPS and 3+ chars then that's a new spell
                    if (inDescr && line.ToUpper().Equals(line) && line.Trim().Length>2)
                    {
                        int i = 0;
                        for (i = 0; i < line.Length; i++)
                        {
                            if (Char.IsLetter(line[i])) break;
                        }
                        if (i < line.Length)
                        {
                            if (rtfOpened) sp.rtfDescription += "\\par}";
                            rtfOpened = false;
                            sp.Breakout();
                            ret.Add(sp);
                            sp = null;
                        }
                    }
                    if (sp == null)
                    {
                        //Spell name
                        sp = new Spell();
                        sp.Name = line.Trim();
                        inDescr = false;
                        hasLevel = false;
                        rtfOpened = false;
                        continue;
                    }
                    if (!hasLevel)
                    {
                        //type (italic)
                        Match m = rSpellLevel.Match(line);
                        if (m.Success)
                        {
                            if (!Int32.TryParse(m.Groups[1].Value, out sp.Level)) continue;
                            string[] spellType = m.Groups[2].Value.Split(new char[] { ' ' });
                            sp.School = spellType[0];
                            if (spellType.Length > 1 && spellType[1].Equals("(ritual)", StringComparison.CurrentCultureIgnoreCase)) sp.IsRitual = true;
                        }
                        else
                        {
                            string[] spellType = line.Trim().Split(new char[] { ' ' });
                            if (spellType == null || spellType.Length != 2)
                                continue;
                            sp.Level = 0;
                            sp.School = spellType[0];
                        }
                        sp.School = Char.ToUpper(sp.School[0]) + sp.School.Substring(1);
                        sp.School = sp.School.Trim();
                        hasLevel = true;
                        continue;
                    }

                    if (!inDescr)
                    {
                        //properties (bold, then not)
                        Match mm = rRange.Match(line);
                        if (mm.Success)
                        {
                            sp.sRange = mm.Groups[1].Value.Trim();
                            continue;
                        }

                        mm = rCastingTime.Match(line);
                        if (mm.Success)
                        {
                            sp.sCastingTime = mm.Groups[1].Value.Trim();
                            continue;
                        }

                        mm = rComponents.Match(line);
                        if (mm.Success)
                        {
                            sp.sComponents = mm.Groups[1].Value.Trim();
                            continue;
                        }

                        mm = rDuration.Match(line);
                        if (mm.Success)
                        {
                            sp.sDuration = mm.Groups[1].Value.Trim();
                            continue;
                        }
                    }
                    //description (may contain bold for higher levels)
                    inDescr = true;
                    sp.Description += line;// +"\n"; //TODO: Handle Rich Text, indentation, etc.
                    bool highLevels = false;
                    if (!rtfOpened)
                    {
                        sp.rtfDescription += "{     ";
                        rtfOpened = true;
                        if(line.StartsWith("At Higher Levels.", StringComparison.CurrentCultureIgnoreCase)){
                            highLevels = true;
                            sp.rtfDescription += " {\\b At Higher Levels.}";
                        }
                    }
                    if(highLevels) {
                        sp.rtfDescription += line.Substring("At Higher Levels.".Length);
                        highLevels = false;
                    }
                    else sp.rtfDescription += line;
                    if (line.EndsWith(". "))
                    {
                        sp.Description += "\n";
                        sp.rtfDescription += "\\par}";
                        rtfOpened = false;
                    }

                }
                if (rtfOpened) sp.rtfDescription += "\\par}";
                rtfOpened = false;
                sp.Breakout();
                ret.Add(sp);
                sp = null;
                 
            }
            return ret;
        }

        /*
        static Style CreateStyle(string name, string id, bool isDefault, StyleValues type, string font, int fontSize, 
            bool bold=false, bool italic=false, string linkedTo="", int beforeSpace=2, int firstLineIndent=360)
        {
            Style ret = new Style() { Type = type, StyleId = id, CustomStyle = true, Default = isDefault };
            ret.Append(new StyleName() { Val = name });
            if(linkedTo!="") ret.Append(new LinkedStyle() { Val = linkedTo });
            
            StyleRunProperties srp = new StyleRunProperties();
            if(font!=null) srp.Append(new RunFonts() { Ascii = font });
            if(fontSize>0) srp.Append(new FontSize() { Val = fontSize.ToString() });
            if(bold) srp.Append(new Bold());
            if (italic) srp.Append(new Italic());
            if(srp.ChildElements.Count>0) ret.Append(srp);

            if (type == StyleValues.Paragraph)
            {
                StyleParagraphProperties spp = new StyleParagraphProperties();
                spp.SpacingBetweenLines = new SpacingBetweenLines() { After = "0", Before = (beforeSpace*20).ToString() };
                if (firstLineIndent > 0) spp.Indentation = new Indentation() { FirstLine = firstLineIndent.ToString() };
                ret.Append(spp);
            }
            return ret;
        }

        static List<Style> CreateStyles()
        {
            //name, text, descr, smalltext      Char styles: bold, bolditalic - both based on text
            List<Style> ret = new List<Style>();
            ret.Add(CreateStyle("Name", "Name", false, StyleValues.Paragraph, "Arial", 48, beforeSpace:18, firstLineIndent: 0));
            ret.Add(CreateStyle("text", "text", true, StyleValues.Paragraph, "Arial", 22, firstLineIndent: 0));
            ret.Add(CreateStyle("descr", "descr", false, StyleValues.Paragraph, "Arial", 20));
            ret.Add(CreateStyle("descFixed", "descFixed", false, StyleValues.Paragraph, "Courier New", 20, firstLineIndent:0));
            ret.Add(CreateStyle("smalltext", "smalltext", false, StyleValues.Paragraph, "Arial", 16, firstLineIndent:0));
            ret.Add(CreateStyle("School", "School", false, StyleValues.Paragraph, "Arial", 22, false, true, firstLineIndent:0));
            ret.Add(CreateStyle("bold", "bold", false, StyleValues.Character, null, -1, true, false, "text"));
            ret.Add(CreateStyle("bolditalic", "bolditalic", false, StyleValues.Character, null, -1, true, true, "text"));
            return ret;
        }
        static bool ApplyStyle(Paragraph p, string styleName)
        {
            // If the paragraph has no ParagraphProperties object, create one.
            if (p.Elements<ParagraphProperties>().Count() == 0)
            {
                p.PrependChild<ParagraphProperties>(new ParagraphProperties());
            }

            // Get a reference to the ParagraphProperties object.
            ParagraphProperties pPr = p.ParagraphProperties;

            // If a ParagraphStyleId object does not exist, create one.
            if (pPr.ParagraphStyleId == null)
                pPr.ParagraphStyleId = new ParagraphStyleId();

            // Set the style of the paragraph.
            pPr.ParagraphStyleId.Val = styleName;
            return true;
        }
        static bool ApplyStyle(Run r, string styleName)
        {
            // If the Run has no RunProperties object, create one.
            if (r.Elements<RunProperties>().Count() == 0)
            {
                r.PrependChild<RunProperties>(new RunProperties());
            }

            // Get a reference to the RunProperties.
            RunProperties rPr = r.RunProperties;

            // Set the character style of the run.
            if (rPr.RunStyle == null)
                rPr.RunStyle = new RunStyle();
            rPr.RunStyle.Val = styleName;
            return true;
        }
        static bool ApplyCharFormat(Run r, bool bold, bool italic)
        {
            if (r.RunProperties == null) r.RunProperties = new RunProperties();
            if (bold)
            {
                Bold b = new Bold();
                b.Val = DocumentFormat.OpenXml.OnOffValue.FromBoolean(bold);
                r.RunProperties.Append(b);
            }
            if (italic)
            {
                Italic b = new Italic();
                b.Val = DocumentFormat.OpenXml.OnOffValue.FromBoolean(italic);
                r.RunProperties.Append(b);
            }
            return true;
        }
        */
        public static bool SaveAsDocX(string fname, List<Spell> spells)
        {
            using (IanUtility.DocXWriter wr = new IanUtility.DocXWriter(fname))
            {
                wr.CreateDefaultStyles();
                foreach (Spell s in spells)
                {
                    s.SaveAsDocX();
                }
                wr.Save();
            }
            return true;
        }
        public void SaveAsDocX()
        {
            //Name
            DocXWriter.CreatePara(Name, "Name");
            //Level, family, classes
            DocXWriter.CreatePara(LevelSchool + "  (" + ClassesString + ")" + (IsRitual ? "  Ritual" : ""), "School");
            //Casting time, Range, Components, Duration
            DocXWriter.CreatePara2Run("text", "Casting time:\t", "bold", sCastingTime, "text");
            DocXWriter.CreatePara2Run("text", "Range:\t", "bold", sRange, "text");
            if (sComponents == null || sComponents == "")
            {
                sComponents = "";
                if (Verbal) sComponents += "Verbal";
                if (Somatic) sComponents += (sComponents == "" ? "" : ", ") + "Somatic";
                if (Material) sComponents += (sComponents == "" ? "" : ", ") + "Material";
                if (Material && MaterialNeeded != null && MaterialNeeded != "") sComponents += " (" + MaterialNeeded + ")";
            }
            DocXWriter.CreatePara2Run("text", "Components:\t", "bold", sComponents, "text");
            DocXWriter.CreatePara2Run("text", "Duration:\t", "bold", sDuration, "text");
            //PHBPage
            DocXWriter.CreatePara("See page " + PHBPage.ToString() + " of Players Hand Book.", "smalltext");
            //Description
            RtfToDocX(rtfDescription);
        }

        static List<string> SplitIntoBlocks(string s)
        {
            List<string> ret = new List<string>();
            int depth = 0;
            int blockStart = 0;
            for (int i = 0; i < s.Length; i++)
            {
                switch (s[i])
                {
                    case '{':
                        if (depth == 0) blockStart = i;
                        depth++;
                        break;
                    case '}':
                        depth--;
                        if (depth < 0) depth = 0;
                        if (depth == 0)
                        {
                            string block = s.Substring(blockStart+1, i - (blockStart+1));
                            ret.Add(block);
                            blockStart = i;
                        }
                        break;
                    default:
                        break;
                }

            }
            if (blockStart!=s.Length-1)
            {
                string block = s.Substring(blockStart + (s[blockStart] == '{' ? 1 : 0), s.Length - 1 - (blockStart + (s[blockStart] == '{' ? 1 : 0)));
                ret.Add(block);
            }
            return ret;
        }
        static void Update(ref string cmd, Stack<bool> bold, Stack<bool> italic, Stack<int> fontIdx, ref string run)
        {
            if (cmd[0] == 'f' && Char.IsDigit(cmd[1]))
            {
                int i = Int32.Parse(cmd.Substring(1));
                fontIdx.Pop();
                fontIdx.Push(i);

                if (i > 0)
                    DocXWriter.curXWriter.PushStyle("descFixed");
                else
                    DocXWriter.curXWriter.PushStyle("descr");

            }
            else {
                switch (cmd)
                {
                    case "b":
                        bold.Pop();
                        bold.Push(true);
                        break;
                    case "i":
                        italic.Pop();
                        italic.Push(true);
                        break;
                    case "line":
                        DocXWriter.curXWriter.LineBreak();
                        break;
                    default:
                        break;
                }  
            }
            cmd = "";
        }
        /*static List<Tuple<string, bool, bool, int>> SplitIntoRuns(string s)
        {
            List<Tuple<string, bool, bool, int>> ret = new List<Tuple<string, bool, bool, int>>();
            string run = "";
            string cmd = "";
            bool inCmd = false;
            Stack<bool> bold = new Stack<bool>();
            bold.Push(false);
            Stack<bool> italic = new Stack<bool>();
            italic.Push(false);
            Stack<int> fontIdx = new Stack<int>();
            fontIdx.Push(0);
            for (int i = 0; i < s.Length; i++)
            {
                switch (s[i])
                {
                    case '{':
                        if (inCmd)
                        {
                            Update(ref cmd, bold, italic, fontIdx, ref run);
                            inCmd = false;
                        }
                        ret.Add(new Tuple<string, bool, bool, int>(run, bold.Peek(), italic.Peek(), fontIdx.Peek()));
                        run = "";
                        bold.Push(bold.Peek());
                        italic.Push(italic.Peek());
                        fontIdx.Push(fontIdx.Peek());
                        break;
                    case '}':
                        if (inCmd)
                        {
                            inCmd = false;
                            Update(ref cmd, bold, italic, fontIdx, ref run);
                        }
                        ret.Add(new Tuple<string, bool, bool, int>(run, bold.Pop(), italic.Pop(), fontIdx.Pop()));
                        run = "";
                        break;
                    case '\\':
                        if (inCmd)
                        {
                            if (cmd == "")
                                run += "\\";
                            else
                                Update(ref cmd, bold, italic, fontIdx, ref run);
                        }
                        else
                            inCmd = true;
                        break;
                    default:
                        if (!inCmd) run += s[i];
                        else
                        {
                            if (s[i] == ' ')
                            {
                                inCmd = false;
                                Update(ref cmd, bold, italic, fontIdx, ref run);
                            }
                            else
                                cmd += s[i];
                        }
                        break;
                }
            }
            if (run != "")
                ret.Add(new Tuple<string, bool, bool, int>(run, bold.Peek(), italic.Peek(), fontIdx.Peek()));

            return ret;
        }*/
        static void DoRuns(string s)
        {
            List<Tuple<string, bool, bool, int>> ret = new List<Tuple<string, bool, bool, int>>();
            string run = "";
            string cmd = "";
            bool inCmd = false;
            Stack<bool> bold = new Stack<bool>();
            bold.Push(false);
            Stack<bool> italic = new Stack<bool>();
            italic.Push(false);
            Stack<int> fontIdx = new Stack<int>();
            fontIdx.Push(0);
            for (int i = 0; i < s.Length; i++)
            {
                switch (s[i])
                {
                    case '{':
                        DocXWriter.curXWriter.AppendText(run);
                        run = "";
                        
                        bold.Push(bold.Peek());
                        italic.Push(italic.Peek());
                        fontIdx.Push(fontIdx.Peek());
                        break;
                    case '}':
                        if (inCmd)
                        {
                            inCmd = false;
                            Update(ref cmd, bold, italic, fontIdx, ref run);
                        }
                        DocXWriter.curXWriter.AppendText(run);
                        run = "";
                        bold.Pop();
                        italic.Pop();
                        fontIdx.Pop();
                        break;
                    case '\\':
                        if (inCmd)
                        {
                            if (cmd == "")
                                run += "\\";
                            else
                                Update(ref cmd, bold, italic, fontIdx, ref run);
                        }
                        else
                            inCmd = true;
                        break;
                    default:
                        if (!inCmd) run += s[i];
                        else
                        {
                            if (s[i] == ' ')
                            {
                                inCmd = false;
                                Update(ref cmd, bold, italic, fontIdx, ref run);
                            }
                            else
                                cmd += s[i];
                        }
                        break;
                }
            }
            if (run != "")
            {
                DocXWriter.curXWriter.AppendText(run);
                run = "";
            }
        }
        static void RtfToDocX(string rtf)
        {
            string temp = rtf;
            char[] openBrace = new char[] { '{' };
            char[] closedBrace = new char[] { '{' };
            char[] brace = new char[] { '{', '}' };
            string[] paraSep = new string[]{"\\par"};
            

            //skip over font descr, and remove trailing brace (to match first brace)
            //{\rtf1\ansi{\fonttbl{\f0\fswiss\fprq2\fcharset0 Microsoft Sans Serif;}{\f1\fnil\fcharset0 Courier New;}}
            int i = rtf.IndexOf(";}}");
            temp = temp.Substring(i + 3, rtf.Length - 1 - (i + 3));

            //skip initial \viewkind4\uc1\pard\lang2057\f0\fs19
            int skip = 0;
            temp = temp.TrimStart();
            if (temp[0] == '\\')
            {
                for (skip = 0; skip < temp.Length; skip++)
                    if (Char.IsWhiteSpace(temp[skip])) break;
                if (skip < temp.Length - 1)
                    temp = temp.Substring(skip);
            }

            List<string> paras = SplitIntoBlocks(temp);

            foreach (string para in paras)
            {
                string [] actualParas = para.Split(paraSep, StringSplitOptions.RemoveEmptyEntries);
                foreach(string actualPara in actualParas){
                    DocXWriter.curXWriter.PushStyle("descr");
                    DocXWriter.curXWriter.NewParagraph();
                    //DocXWriter.ApplyStyle(p, "descr");
                    //Trim spaces from the start of each para
                    DoRuns(actualPara.TrimStart());
                    /*ApplyStyle(p, "descr");
                    List<Tuple<string,bool,bool,int>> sirs = SplitIntoRuns(actualPara);
                    bool bold = false;
                    bool italic = false;
                    foreach(Tuple<string,bool,bool,int> sir in sirs){
                        Run r = new Run();
                        if (sir.Item2 == true || sir.Item3 == true) ApplyCharFormat(r, sir.Item2, sir.Item2, sir.Item3, sir.Item3);
                        if (sir.Item4 > 0) ApplyStyle(r, "descFixed");
                        r.Append(new Text(sir.Item1));
                        p.Append(r);
                    }
                    */
                    DocXWriter.curXWriter.PopStyle();
                }
            }
        }


    }
}
