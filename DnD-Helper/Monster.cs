using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using IanUtility;
using System.IO;
using System.Runtime.Serialization;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Net;

namespace DnDMonsters
{
    [Flags]
    public enum Environments { None=0x0000, 
        Arctic=0x0001, Coastal=0x0002, Desert=0x0004, Forest=0x0008, 
        Grassland=0x0010, Hill=0x0020, Mountain=0x0040, Swamp=0x0080, 
        Underdark=0x0100, Underwater=0x0200, Urban=0x0400 }
    public enum MonsterSizes
    {
        Tiny, Small, Medium, Large, Huge, Gargantuan, NotSet
    }

    public class Monster
    {
        public string Name = "";
        public int AC = 12;
        public string HP = "";
        public string Speed = "60 ft";
        public int STR = 10;
        public int DEX = 10;
        public int CON = 10;
        public int INT = 10;
        public int WIS = 10;
        public int CHA = 10;
        public string Skills = "";
        public string Vuln = "";
        public string Resist = "";
        public string Immune = "";
        public int Darkvision = 0;
        public int Truesight = 0;
        public int Blindsight = 0;
        public int Tremorsense = 0;
        public int PassivePerception = 12;
        public string Saves = "";
        public int XP = 0;
        public string nAttacks = "";
        public List<Attack> Attacks = new List<Attack>();
        public string FeatsLong { 
            get {
                string ret = "";
                if (Feats.Count == 0) return "";
                foreach (string ft in Feats.Keys)
                {
                    ret += ft + ": ";
                    ret += Feats[ft];
                    if (!Feats[ft].EndsWith(". ")) ret += ". ";
                }
                return ret;
            } 
            set {
                if (value == "")
                {
                    Feats = new Dictionary<string, string>();
                    return;
                }
                ParseFeats(value);
            } 
        }
        public int SpellDC = 12;
        public int SpellRngAttack = 0;
        public Dictionary<int, Tuple<int, string>> Spells = new Dictionary<int, Tuple<int, string>>();
        public string Descr = "";
        public string Size
        {
            get { return ESize.ToString(); }
            set {
                string s = value.ToLower();
                if (s == "")
                {
                    ESize = MonsterSizes.NotSet;
                    return;
                }
                switch (s[0])
                {
                    case 't': ESize = MonsterSizes.Tiny; break;
                    case 'm': ESize = MonsterSizes.Medium; break;
                    case 'l': ESize = MonsterSizes.Large; break;
                    case 's': ESize = MonsterSizes.Small; break;
                    case 'h': ESize = MonsterSizes.Huge; break;
                    case 'g': ESize = MonsterSizes.Gargantuan; break;
                    case 'n': ESize = MonsterSizes.NotSet; break;
                    default:
                        throw new NotImplementedException("Cannot parse size "+s);
                }
            }
        }
        public MonsterSizes ESize = MonsterSizes.NotSet;
        public string Type = "";
        public string Alignment = "NN";
        public Environments Environ = Environments.None;
        public string Source = "";
        public string Lang = "";

        [NonSerialized, IgnoreDataMember]
        public Dictionary<string, string> Feats = new Dictionary<string, string>();

        #region Image settings
        public Uri SourceFile = null;
        public decimal DPI_cm
        {
            get
            {
                return DPI / 2.54m;
            }
        }

        internal static Dictionary<Uri, string> cachedUri = new Dictionary<Uri, string>();





        #region Static settings
        [NonSerialized, IgnoreDataMember]
        public static decimal BorderPX = 5; //pixel
        [NonSerialized, IgnoreDataMember]
        public static decimal FontSz = 11; //points
        [NonSerialized, IgnoreDataMember]
        public static decimal FooterCM = 1; //cm
        [NonSerialized, IgnoreDataMember]
        public static decimal MaxHeight = 13; //cm
        [NonSerialized, IgnoreDataMember]
        public static decimal MaxHeightMedMonster = 7; //cm
        [NonSerialized, IgnoreDataMember]
        public static decimal PaddingMM = 1; //mm
        [NonSerialized, IgnoreDataMember]
        public static decimal Width = 2.5m; //cm
        [NonSerialized, IgnoreDataMember]
        public static int DPI = 300;
        //TODO: Load/Save these Settings
        #endregion
        #endregion

        public int RollInitiative(Random r)
        {
            int roll = r.Next(1, 21);
            roll += GetModifier(DEX);
            return roll;
        }


        public Monster()
        {
            //ImageDetails = new MonsterImage(this);
        }

        private int GetModifier(int ability)
        {
            int i = ability / 2;
            i -= 5;
            return i;
        }
        public string Modifier(int ability)
        {
            int i = GetModifier(ability);
            string s = "";
            if (i > 0) s = "+";
            s += i.ToString();
            return s;
        }
        
        
        
        public static Monster FromDataRow(IanUtility.ExcelParser.Sheet s, IanUtility.ExcelParser.Row r)
        {
            Monster m = new Monster();
            m.Name = GetString(s, r, "Name");
            m.AC = GetInt(s, r, "AC");
            m.HP = GetString(s, r, "HP");
            m.Speed = GetString(s, r, "Speed");
            if (m.Speed == "") m.Speed = "30ft";
            m.STR = GetInt(s, r, "STR");
            m.DEX = GetInt(s, r, "DEX");
            m.INT = GetInt(s, r, "INT");
            m.WIS = GetInt(s, r, "WIS");
            m.CON = GetInt(s, r, "CON");
            m.CHA = GetInt(s, r, "CHA");
            m.Skills = GetString(s, r, "Skills");
            m.Vuln = GetString(s, r, "Vulnerable");
            m.Resist = GetString(s, r, "Resist");
            m.Immune = GetString(s, r, "Immune");
            m.Darkvision = GetInt(s, r, "Darkvision");
            m.Truesight = GetInt(s, r, "Truesight");
            m.Tremorsense = GetInt(s, r, "Tremorsense");
            m.Blindsight = GetInt(s, r, "Blindsight");
            m.PassivePerception = GetInt(s, r, "Passive Perception");
            m.Saves = GetString(s, r, "Saves");
            m.XP = GetInt(s, r, "XP");
            m.nAttacks = GetString(s, r, "#Attack");
            Attack a = Attack.Melee(GetString(s, r, "Melee"), GetInt(s, r, "MeleeAtk"), GetString(s, r, "MeleeDmg"), GetString(s, r, "MeleeSpecial"));
            if (a != null) m.Attacks.Add(a);
            a = Attack.Ranged(GetString(s, r, "Rng"), GetInt(s, r, "RngAtk"), GetString(s, r, "RngDmg"), GetInt(s, r, "RngShort"),
                GetInt(s, r, "RngFull"), GetString(s, r, "RngSpecial"));
            if (a != null) m.Attacks.Add(a);
            List<Attack> aa = Attack.FromOther(GetString(s, r, "Other"));
            if (aa != null) m.Attacks.AddRange(aa);
            m.FeatsLong = GetString(s, r, "Feats");
            m.SpellDC = GetInt(s, r, "Spell DC");
            m.SpellRngAttack = GetInt(s, r, "Spell Rng Atk");
            m.Spells.Add(0, new Tuple<int, string>(0, GetString(s, r, "Cantrips")));
            m.Spells.Add(1, new Tuple<int, string>(GetInt(s, r, "#1st Level"), GetString(s, r, "1st Level")));
            m.Spells.Add(2, new Tuple<int, string>(GetInt(s, r, "#2nd Level"), GetString(s, r, "2nd Level")));
            m.Spells.Add(3, new Tuple<int, string>(GetInt(s, r, "#3rd Level"), GetString(s, r, "3rd Level")));
            m.Spells.Add(4, new Tuple<int, string>(GetInt(s, r, "#4th Level"), GetString(s, r, "4th Level")));
            m.Spells.Add(5, new Tuple<int, string>(GetInt(s, r, "#5th Level"), GetString(s, r, "5th Level")));
            m.Descr = GetString(s, r, "Descr");
            m.Source = GetString(s, r, "Source");
            m.Size = GetString(s, r, "Size");
            m.Type = GetString(s, r, "Type");
            m.Alignment = GetString(s, r, "Alignment");
            m.Lang = GetString(s, r, "Lang");

            if (GetInt(s, r, "Arctic") != 0) m.Environ |= Environments.Arctic;
            if (GetInt(s, r, "Coastal") != 0) m.Environ |= Environments.Coastal;
            if (GetInt(s, r, "Desert") != 0) m.Environ |= Environments.Desert;
            if (GetInt(s, r, "Forest") != 0) m.Environ |= Environments.Forest;
            if (GetInt(s, r, "Grassland") != 0) m.Environ |= Environments.Grassland;
            if (GetInt(s, r, "Hill") != 0) m.Environ |= Environments.Hill;
            if (GetInt(s, r, "Mountain") != 0) m.Environ |= Environments.Mountain;
            if (GetInt(s, r, "Swamp") != 0) m.Environ |= Environments.Swamp;
            if (GetInt(s, r, "Underdark") != 0) m.Environ |= Environments.Underdark;
            if (GetInt(s, r, "Underwater") != 0) m.Environ |= Environments.Underwater;
            if (GetInt(s, r, "Urban") != 0) m.Environ |= Environments.Urban;

            return m;
        }

        public override string ToString()
        {
            return Name;
        }
        Regex hpReg = new Regex(@"(\d+)d(\d+)(\+(\d+))?");
        public int GetHP()
        {
            Match m = hpReg.Match(HP);
            if (!m.Success)
            {
                int i;
                if (Int32.TryParse(HP, out i)) return i;
                return -1;
            }
            //1,2,4
            try
            {
                int ret = 0;
                if(m.Groups[4].Value!="") Int32.Parse(m.Groups[4].Value);
                int nDice = Int32.Parse(m.Groups[1].Value);
                int szDice = Int32.Parse(m.Groups[2].Value);
                for (int i = 0; i < nDice;i++)
                {
                    ret += DnDHelper.rand.Next(szDice) + 1;
                }
                return ret;
            }
            catch (Exception)
            {
                return -1;
            }
        }


        static int GetInt(IanUtility.ExcelParser.Sheet s, IanUtility.ExcelParser.Row r, string ColumnName)
        {
            try
            {
                if (!s.ColumnByName.ContainsKey(ColumnName)) return 0;
                return r[ColumnName].Int;
                //string s = r[ColumnName].ToString();
                //if (s == "") return 0;
                //decimal i = 0;
                //if (decimal.TryParse(s, out i)) return (int)i;
                //return 0;
            }
            catch (Exception)
            {
                return 0;
            }
        }
        static string GetString(IanUtility.ExcelParser.Sheet s, IanUtility.ExcelParser.Row r, string ColumnName)
        {
            try
            {
                if (!s.ColumnByName.ContainsKey(ColumnName)) return "";
                return r[ColumnName].Text;
                //string s = r[ColumnName].ToString();
                //return s;
            }
            catch (Exception)
            {
                return "";
            }
        }
        void ParseFeats(string ft)
        {
            try
            {
                Feats = new Dictionary<string, string>();
                string[] s = ft.Split(new string[] { ". " }, StringSplitOptions.RemoveEmptyEntries);
                int idx = 0;
                string feat = "";
                while (idx < s.Length)
                {
                    bool hasColon = false;
                    if(s[idx].IndexOf(':') != -1) hasColon = true;
                    if (feat == "" || !hasColon)
                        feat += s[idx] + ". ";
                    
                    if ((hasColon || idx == s.Length - 1) && feat != "")
                    {
                        int fColon = feat.IndexOf(':');
                        if (fColon != -1)
                            Feats.Add(feat.Substring(0, fColon), feat.Substring(fColon + 1));
                        else Feats.Add(feat, "");
                        feat = "";
                    }
                    if(hasColon)
                        feat += s[idx] + ". ";
                    idx++;
                }
            }
            catch (Exception) {
                ;
            }
        }

        public static bool ToXlsX(string fname, Dictionary<string, Monster> ms)
        {
            FileStream fs = new FileStream(fname, FileMode.Create);
            ExcelFormatter f = new ExcelFormatter();
            
            f.Serialize(fs, ms.Values.ToList());
            fs.Close();

            return true;
        }



        #region Image settings
        public Bitmap GetFrontImage(string text, Uri altURI = null)
        {
            Uri uri = altURI;
            if (uri == null) uri = SourceFile;

            //get max sizes
            decimal widthCM = Width;
            decimal maxHeightCM = MaxHeightMedMonster;
            switch (ESize)
            {
                case MonsterSizes.Large:
                    widthCM = 2 * widthCM;
                    maxHeightCM = MaxHeight;
                    break;
                case MonsterSizes.Huge:
                    widthCM = 3 * widthCM;
                    maxHeightCM = MaxHeight;
                    break;
                case MonsterSizes.Gargantuan:
                    widthCM = 4 * widthCM;
                    maxHeightCM = MaxHeight;
                    break;
                default:
                    //Use default size
                    break;
            }

            int maxImageHeightPX = (int)((maxHeightCM - FooterCM - (PaddingMM / 10) * 2) * DPI_cm - BorderPX);
            int maxImageWidthPX = (int)((widthCM - (PaddingMM / 10) * 2) * DPI_cm - 2 * BorderPX);


            //get front bitmap
            Bitmap b = new Bitmap((int)(DPI_cm * widthCM), (int)(DPI_cm * maxHeightCM));
            b.SetResolution(DPI, DPI);
            Graphics gB = Graphics.FromImage(b);
            //gB.CompositingMode = CompositingMode.SourceCopy;
            gB.CompositingQuality = CompositingQuality.HighQuality;
            gB.InterpolationMode = InterpolationMode.HighQualityBicubic;
            gB.SmoothingMode = SmoothingMode.HighQuality;
            gB.PixelOffsetMode = PixelOffsetMode.HighQuality;
            SizeF textSz = new SizeF(0, 0);

            //fill with white
            Brush whiteBrush = new SolidBrush(Color.White);
            gB.FillRectangle(whiteBrush, 0, 0, b.Width, b.Height);

            //measure text
            Font f = new Font(FontFamily.GenericSansSerif, (float)FontSz, FontStyle.Regular);
            if (text != "")
            {
                textSz = gB.MeasureString(text, f, maxImageWidthPX);
                maxImageHeightPX -= (int)textSz.Height; //assuming Height is in pixels
            }

            Image pic = null;

            string fname = "";
            if (uri != null)
            {
                if (uri.IsFile) fname = uri.LocalPath;
                else
                {
                    if (cachedUri.ContainsKey(uri) && File.Exists(cachedUri[uri]))
                        fname = cachedUri[uri];
                    else
                        fname = CacheUri(uri);
                }
                try
                {
                    if(fname!="")
                        pic = Image.FromFile(fname);
                }
                catch (Exception e)
                {
                    MessageBox.Show("Error: Couldn't load image from: " + fname + "\n" + e.Message);
                }
            }

            int picHeight = 0;
            if (pic != null)
            {
                //TODO: Crop all whitespace
                Rectangle croppedSize = new Rectangle(0, 0, pic.Width, pic.Height);


                //calculate new pic size
                decimal aspectRatio = (decimal)(croppedSize.Width) / croppedSize.Height;
                int newWidth = maxImageWidthPX;
                int newHeight = (int)(maxImageWidthPX / aspectRatio);
                if (newHeight > maxImageHeightPX)
                {
                    //Height too large, so need to shrink
                    newHeight = maxImageHeightPX;
                    newWidth = (int)(newHeight * aspectRatio);
                }
                picHeight = newHeight;

                //find the top,left
                int top = (int)((PaddingMM / 10) * DPI_cm + BorderPX);
                int left = (int)((b.Width - newWidth) / 2);
                Rectangle destRect = new Rectangle(top, left, newWidth, newHeight);
                Rectangle srcRect = new Rectangle(croppedSize.X, croppedSize.Y,
                    croppedSize.Width, croppedSize.Height);

                //draw pic into b
                ImageAttributes wrap = new ImageAttributes();
                wrap.SetWrapMode(WrapMode.TileFlipXY);
                gB.DrawImage(pic, destRect, srcRect.X, srcRect.Y, srcRect.Width, srcRect.Height, GraphicsUnit.Pixel, wrap);
            }

            Brush brush = new SolidBrush(Color.Black);
            if (text != "")
            {
                float top = (float)(picHeight + (2 * PaddingMM / 10) * DPI_cm + BorderPX);
                float left = (float)((b.Width - textSz.Width) / 2);
                gB.DrawString(text, f, brush, new RectangleF(left, top, textSz.Width, textSz.Height));
            }

            //resize
            int finalHeight = (int)(BorderPX * 2 + (((2 * (PaddingMM / 20)) + FooterCM) * DPI_cm));
            if (pic != null) finalHeight += picHeight;
            if (text != null) finalHeight += (int)textSz.Height;

            b = b.Clone(new Rectangle(0, 0, b.Width, finalHeight), b.PixelFormat);
            gB = Graphics.FromImage(b);
            gB.CompositingQuality = CompositingQuality.HighQuality;
            gB.InterpolationMode = InterpolationMode.HighQualityBicubic;
            gB.SmoothingMode = SmoothingMode.HighQuality;
            gB.PixelOffsetMode = PixelOffsetMode.HighQuality;

            //draw border
            Pen pen = new Pen(brush);
            pen.Width = (float)BorderPX;
            gB.DrawRectangle(pen, 0, 0, b.Width - 1, b.Height - 1);

            return b;
        }
        public Bitmap GetFullImage(string text)
        {
            Bitmap front = GetFrontImage(text);

            Bitmap full = new Bitmap(front.Width, front.Height*2);
            full.SetResolution(front.HorizontalResolution, front.VerticalResolution);
            Graphics g = Graphics.FromImage(full);
            g.CompositingMode = CompositingMode.SourceCopy;
            g.CompositingQuality = CompositingQuality.HighQuality;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;

            g.DrawImageUnscaled(front, 0, front.Height);
            Point[] destPts = new Point[]{
                new Point(front.Width-1, front.Height-1),//top left
                new Point(0, front.Height-1),//top right
                new Point(front.Width-1, 0)//low left
            };
            g.DrawImage(front, destPts);
            
            return full;
        }


        string CacheUri(Uri uri)
        {
            string fname = Path.GetTempFileName();
            try
            {
                WebClient web = new WebClient();
                web.DownloadFile(uri, fname);
            }
            catch (Exception)
            {
                MessageBox.Show("Error: Couldn't download image file: " + SourceFile.ToString());
                return "";
            }
            if (cachedUri.ContainsKey(uri))
                cachedUri[uri] = fname;
            else 
                cachedUri.Add(uri, fname);

            return fname;
        }


        #endregion



    }
}
