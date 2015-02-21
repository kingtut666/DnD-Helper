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
using PdfSharp.Pdf;
using PdfSharp.Drawing;

namespace DnDMonsters
{
    public partial class DnDHelper : Form
    {
        Monster curMini = null;
        bool ignoreNumMini = false;

        public class ImageSet
        {
            public Monster M;
            public string Name { get { return (M != null ? M.Name : ""); } }
            public double Width;
            public double Height;
            public int Count;
            public int From;
            public double TotalWidth { get { return Width * Count; } }
        }
        SortedDictionary<double, List<ImageSet>> MinisByHeight = new SortedDictionary<double, List<ImageSet>>();
        SortedDictionary<double, List<ImageSet>> MinisByWidth = new SortedDictionary<double, List<ImageSet>>();
        SortedDictionary<double, List<ImageSet>> MinisByTotWidth = new SortedDictionary<double, List<ImageSet>>();

        void InitMiniMaker()
        {
            //settings
            ignoreNumMini = true;
            numMiniBorder.Value = Monster.BorderPX;
            numMiniFont.Value = Monster.FontSz;
            numMiniFooter.Value = Monster.FooterCM;
            numMiniMax.Value = Monster.MaxHeight;
            numMiniMaxMed.Value = Monster.MaxHeightMedMonster;
            numMiniPadding.Value = Monster.PaddingMM;
            numMiniWidth.Value = Monster.Width;
            ignoreNumMini = false;

            MonstersChanged += MiniMaker_MonstersChanged;

            MiniMaker_MonstersChanged(null, null);

            //Fill encounter checklist

            chkListMiniEncounters.DataSource = SaveEncounterList;
            comboMiniPaper.SelectedIndex = 0;
        }

        void MiniMaker_MonstersChanged(object sender, ListChangedEventArgs e)
        {
            List<Monster> allM = Monsters.Values.ToList();
            allM.Sort((m1, m2) => String.Compare(m1.Name, m2.Name));
            comboMonsterMini.DataSource = allM;
            comboMonsterMini.DisplayMember = "Name";
            comboMonsterMini.SelectedIndex = (allM.Count>0?0:-1);
        }


        private void comboMonsterMini_SelectedIndexChanged(object sender, EventArgs e)
        {
            curMini = (Monster)comboMonsterMini.SelectedItem;
            if (curMini != null)
                pictMonsterMini.Image = curMini.GetFrontImage("");
            else 
                pictMonsterMini.Image = null;

        }

        private void butAddMonsterMini_Click(object sender, EventArgs e)
        {
            if(curMini==null) return;
            AddMonsterMini(curMini, 1);
        }
        void AddMonsterMini(Monster m, int ct){
            int idx = dgMonsterMini.Rows.Add();
            dgMonsterMini.Rows[idx].Tag = m;
            Bitmap b = m.GetFrontImage(m.Name+" #01");
            dgMonsterMini.Rows[idx].Cells["colMonster"].Value = m.Name;
            dgMonsterMini.Rows[idx].Cells["colMonsterImage"].Value = b;
            //((DataGridViewImageCell)dgMonsterMini.Rows[idx].Cells["colMonsterImage"]).pi
            dgMonsterMini.Rows[idx].Cells["colMonsterSz"].Value = b.Width.ToString()+"x"+(b.Height*2).ToString();
            dgMonsterMini.Rows[idx].Cells["colNumber"].Value = ct;
            dgMonsterMini.Rows[idx].Cells["colCountFrom"].Value = "1";
        }
        void DecrementIS(ImageSet s)
        {
            double oldTotWidth = s.TotalWidth;
            s.Count -= 1;
            if (s.Count <= 0)
            {
                //update all
                if (MinisByHeight.ContainsKey(s.Height))
                {
                    if (MinisByHeight[s.Height].Count == 1)
                        MinisByHeight.Remove(s.Height);
                    else
                        MinisByHeight[s.Height].Remove(s);
                }
                if (MinisByWidth.ContainsKey(s.Width))
                {
                    if (MinisByWidth[s.Width].Count == 1)
                        MinisByWidth.Remove(s.Width);
                    else
                        MinisByWidth[s.Width].Remove(s);
                }
                if (MinisByTotWidth.ContainsKey(oldTotWidth))
                {
                    if (MinisByTotWidth[oldTotWidth].Count == 1)
                        MinisByTotWidth.Remove(oldTotWidth);
                    else
                    {
                        MinisByTotWidth[oldTotWidth].Remove(s);
                    }
                }
            }
            else
            {
                //update MinisByTotWidth
                if (MinisByTotWidth.ContainsKey(oldTotWidth))
                {
                    if (MinisByTotWidth[oldTotWidth].Count == 1)
                        MinisByTotWidth.Remove(oldTotWidth);
                    else
                    {
                        MinisByTotWidth[oldTotWidth].Remove(s);
                    }
                }
                if (!MinisByTotWidth.ContainsKey(s.TotalWidth)) MinisByTotWidth.Add(s.TotalWidth, new List<ImageSet>());
                MinisByTotWidth[s.TotalWidth].Add(s);
            }
        }
        void DrawImageAndDecrement(XGraphics gfx, double top, double left, out double bottom, out double right, bool horizontal, ImageSet i)
        {
            XImage img = XImage.FromGdiPlusImage(i.M.GetFullImage(i.M.Name + " #" + i.From.ToString()));
            gfx.DrawImage(img, left, top);
            bottom = top + img.PointHeight;
            right = left + img.PointWidth;

            i.From++;
            DecrementIS(i);
        }
        double GetClosestKey(SortedDictionary<double, List<ImageSet>> d, double maxKey, double maxWidth, double maxHeight)
        {
            double wTemp = 0;
            foreach (double w in d.Keys)
            {
                if (w > maxKey) break;
                bool yOkay = false;
                foreach (ImageSet s in d[w])
                {
                    if ((s.Height <= maxHeight || maxHeight==-1) && (s.Width <= maxWidth || maxWidth==-1))
                    {
                        yOkay = true;
                        break;
                    }
                }
                if (!yOkay) continue;
                wTemp = w;
            }
            return wTemp;
        }
        PdfPage AddPage(PdfDocument doc)
        {
            PdfPage p = doc.AddPage();
            if (comboMiniPaper.SelectedValue == null)
            {
                p.Size = PdfSharp.PageSize.A4;
            }
            else
            {
                switch (comboMiniPaper.SelectedValue.ToString())
                {
                    case "A4":
                    default:
                        p.Size = PdfSharp.PageSize.A4;
                        break;
                    case "US Letter":
                        p.Size = PdfSharp.PageSize.Letter;
                        break;
                }
            }

            double cm = XUnit.FromCentimeter((double)numMiniBorderSize.Value).Point;
            p.TrimBox = new PdfRectangle(new XPoint(cm, cm),
                new XPoint(p.Width.Point - cm, p.Height - cm));

            return p;
        }
        private void butSaveMini_Click(object sender, EventArgs e)
        {
            double convertResolution = Monster.DPI/72;
            //Note: Units are point (1/72 inch)
            MinisByHeight = new SortedDictionary<double, List<ImageSet>>();
            MinisByTotWidth = new SortedDictionary<double, List<ImageSet>>();
            MinisByWidth = new SortedDictionary<double, List<ImageSet>>();

            foreach (DataGridViewRow r in dgMonsterMini.Rows)
            {
                ImageSet s = new ImageSet();
                s.M = r.Tag as Monster;
                Bitmap b = r.Cells["colMonsterImage"].Value as Bitmap;
                if (b == null)
                {
                    MessageBox.Show("ERR: No image for monster "+s.Name);
                    return;
                }
                s.Height = (b.Height*2)/convertResolution;
                s.Width = b.Width/convertResolution;
                if (!Int32.TryParse(r.Cells["colNumber"].Value.ToString(), out s.Count))
                {
                    MessageBox.Show("ERR: Count must be a number, for monster " + s.Name);
                    return;
                }
                if (!Int32.TryParse(r.Cells["colCountFrom"].Value.ToString(), out s.From))
                {
                    MessageBox.Show("ERR: Count From must be a number, for monster " + s.Name);
                    return;
                }
                if(!MinisByHeight.ContainsKey(s.Height)) MinisByHeight.Add(s.Height, new List<ImageSet>());
                if(!MinisByTotWidth.ContainsKey(s.TotalWidth)) MinisByTotWidth.Add(s.TotalWidth, new List<ImageSet>());
                if(!MinisByWidth.ContainsKey(s.Width)) MinisByWidth.Add(s.Width, new List<ImageSet>());
                MinisByHeight[s.Height].Add(s);
                MinisByTotWidth[s.TotalWidth].Add(s);
                MinisByWidth[s.Width].Add(s);
            }

            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "PDF (*.pdf)|*.pdf";
            if(dlg.ShowDialog()!=DialogResult.OK) return;

            PdfDocument doc = new PdfDocument();
            PdfPage p = AddPage(doc);
            XGraphics gfx = XGraphics.FromPdfPage(p);
            
            double x = p.TrimBox.X1;
            double y = p.TrimBox.Y1;
            double maxWastage = 36; //in points

            bool strips = true;
            bool strips2 = true;
            while(MinisByHeight.Count>0){
                double spareY = p.TrimBox.Y2 - y;
                double spareX = p.TrimBox.X2 - x;
                //not enough space for the smallest height
                if (spareY < MinisByHeight.Keys.First())
                {
                    //not enough space for horizontal
                    if (spareY < MinisByWidth.Keys.First())
                    {
                        //So we'll add a page
                        p = AddPage(doc);
                        
                        gfx = XGraphics.FromPdfPage(p);
                        x = p.TrimBox.X1;
                        y = p.TrimBox.Y1;
                        //spares will be calculated at top of while
                        strips = true;
                        strips2 = true;
                        continue;
                    }
                    //too little X left
                    if (spareX < MinisByHeight.Keys.First())
                    {
                        y = p.TrimBox.Y2;
                        spareY = 0;
                        continue;
                    }
                    //rotate
                    while (MinisByHeight.Count > 0 && MinisByHeight.Keys.First() <= spareX)
                    {
                        double wTemp = GetClosestKey(MinisByWidth, spareY, -1, spareX);
                        int ct = (int)(spareY / wTemp);
                        double newRight = 0;
                        while (ct > 0)
                        {
                            if (!MinisByWidth.ContainsKey(wTemp))
                            {
                                //need to find some more
                                wTemp = GetClosestKey(MinisByWidth, p.TrimBox.Y2 - y, -1, spareX);
                                if (wTemp == 0)
                                {
                                    x = newRight;
                                    spareX = p.TrimBox.X2 - x;
                                    break;
                                }
                            }
                            ImageSet s = MinisByWidth[wTemp].First();
                            if (newRight == 0) newRight = x + s.Height;
                            double a, b; //throwaway
                            DrawImageAndDecrement(gfx, y, x, out a, out b, true, s);
                            ct--;
                        }
                    }
                    //Can't fit any more
                    y = p.TrimBox.Y2;
                    spareY = 0;
                    continue;
                }
                //look for a full strip across, vertical alignment
                bool stripsDone = false;
                while (strips && !stripsDone)
                {
                    x = p.TrimBox.X1;
                    double wTemp = 0;
                    spareY = p.TrimBox.Y2 - y;
                    wTemp = GetClosestKey(MinisByTotWidth, p.TrimBox.Width, -1, spareY);
                    if (wTemp>0 && p.TrimBox.Width - wTemp < maxWastage)
                    {
                        ImageSet iSet = MinisByTotWidth[wTemp][0];
                        double newBottom = 0;
                        while (iSet.Count>0)
                        {
                            DrawImageAndDecrement(gfx, y, x, out newBottom, out x, false, iSet);
                        }
                        if (newBottom != 0) y = newBottom;
                        x = p.TrimBox.X1;
                        spareX = p.TrimBox.X2 - x;
                        stripsDone = true; //possibly more strips, so need to see if new page needed, and repeat
                    }
                    else strips = false; //no possible full strips - don't repeat this while() again
                }
                if (stripsDone) continue; //want to restart the while() after doing the strips above
                //make up a strip of common height, vertical alignment
                while (strips2 && !stripsDone)
                {
                    x = p.TrimBox.X1;
                    spareX = p.TrimBox.X2 - x;
                    spareY = p.TrimBox.Y2 - y;
                    //1) For each MinisByHeight keys
                    List<Tuple<ImageSet, int>> commonHeight = new List<Tuple<ImageSet, int>>();
                    double commonHeightWidth = 0;
                    bool useCommonHeight = false;
                    foreach (double h in MinisByHeight.Keys.Reverse())
                    {
                        if (h > spareY) continue;
                        //  2) For each IS
                        foreach (ImageSet IS in MinisByHeight[h])
                        {
                            //     3) Get totalwidth, or as many as can fit
                            int maxCt = (int)(spareX / IS.Width);
                            if (IS.Count >= maxCt && maxCt>0)
                            {
                                //full row
                                commonHeight.Clear();
                                commonHeight.Add(new Tuple<ImageSet, int>(IS, maxCt));
                                useCommonHeight = true;
                                break;
                            }
                            //     4) If under maxWastage, draw them all
                            maxCt = (int)((spareX - commonHeightWidth) / IS.Width);
                            if (maxCt == 0)
                            {
                                useCommonHeight = true;
                                break;
                            }
                            else
                            {
                                if (IS.Count < maxCt) maxCt = IS.Count;
                                commonHeight.Add(new Tuple<ImageSet, int>(IS, maxCt));
                                commonHeightWidth += IS.Width * maxCt;
                                
                                if (spareX - commonHeightWidth <= maxWastage)
                                {
                                    useCommonHeight = true;
                                    break;
                                }
                            }
                        }
                        if (useCommonHeight) break;
                    }
                    if (useCommonHeight || commonHeight.Count>0)
                    {
                        double newBottom = 0;
                        foreach (Tuple<ImageSet, int> t in commonHeight)
                        {
                            //draw completeRow
                            ImageSet iSet = t.Item1;
                            for (int i = 0; i < t.Item2; i++)
                            {
                                double tempY = 0;
                                DrawImageAndDecrement(gfx, y, x, out tempY, out x, false, iSet);
                                if (tempY > newBottom) newBottom = tempY;
                            }
                        }
                        if (newBottom != 0)
                        {
                            y = newBottom;
                            spareY = p.TrimBox.Y2 - y;
                        }
                        x = p.TrimBox.X1;
                        spareX = p.TrimBox.X2 - x;
                        stripsDone = true; //Maybe more strips, but check for new page first
                    }
                    else strips2 = false; // no more strips, don't repeat this while()
                }
                if (stripsDone) continue; //want to restart the while() after doing the strips2 above
                //leftovers - big & small
                //TODO: Leftovers
                while (MinisByHeight.Count>0)
                {
                    //1) Start with max height, do as many as fit
                    List<ImageSet> kvp = MinisByHeight.Where(a => a.Key<=spareY).LastOrDefault().Value;
                    if(kvp == null){
                        //Not enough space, trigger a new page
                        y = p.TrimBox.Y2;
                        break;
                    }
                    ImageSet s = kvp.Last();
                    int maxCt = (int)(spareX / s.Width);
                    if (maxCt > s.Count) maxCt = s.Count;
                    double newBottom = 0;
                    for (int i = 0; i < maxCt; i++)
                    {
                        DrawImageAndDecrement(gfx, y, x, out newBottom, out x, false, s);
                    }
                    spareX = p.TrimBox.X2 - x;
                    if (spareX <= maxWastage)
                    {
                        if (newBottom != 0)
                        {
                            y = newBottom;
                            spareY = p.TrimBox.Y2 - y;
                        }
                        break;
                    }
                    //TODO: 2) check which wastes less, vertical or horizontal stack, and do. 
                    // a) See how many of closest Width can fit into spaceX
                    // b) See how many of closest Width can fit into height from (1), of max height =spaceX
                    // c) Do a=A or b=B
                    // A) stack horizontally (image vertical), from closest fit down
                    // B) stack vertically (image horizontal), from closest fit down, with height limitations
                    //Note: Not bothering with trying both H and V in the space to the right of a big one
                }
            }
            doc.Save(dlg.FileName);
            


        }

        private void miniMakerSettingsChange_ValueChanged(object sender, EventArgs e)
        {
            if (ignoreNumMini) return;
            Monster.BorderPX = numMiniBorder.Value;
            Monster.FontSz = numMiniFont.Value;
            Monster.FooterCM = numMiniFooter.Value;
            Monster.MaxHeight = numMiniMax.Value;
            Monster.MaxHeightMedMonster = numMiniMaxMed.Value;
            Monster.PaddingMM = numMiniPadding.Value;
            Monster.Width = numMiniWidth.Value;

            foreach (DataGridViewRow r in dgMonsterMini.Rows)
            {
                Monster m = r.Tag as Monster;
                if (m == null) continue;
                Bitmap b = m.GetFrontImage(m.Name + " #01");
                r.Cells["colMonsterImage"].Value = b;
                r.Cells["colMonsterSz"].Value = b.Width.ToString() + "x" + b.Height.ToString();
            }
        }


        private void butMiniAddCheckedEncounters_Click(object sender, EventArgs e)
        {
            Dictionary<Monster, int> monsterCount = new Dictionary<Monster, int>();
            foreach (string encName in chkListMiniEncounters.CheckedItems.Cast<string>())
            {
                if (SavedEnc.ContainsKey(encName))
                {
                    //get counts for this encounter
                    Dictionary<Monster, int> encMonsterCount = new Dictionary<Monster, int>();
                    foreach (ActualMonster m in SavedEnc[encName].Monsters)
                    {
                        if (!encMonsterCount.ContainsKey(m.Monster)) encMonsterCount.Add(m.Monster, 0);
                        encMonsterCount[m.Monster]++;
                    }
                    //merge with monsterCount, use whichever is higher
                    foreach (Monster m in encMonsterCount.Keys)
                    {
                        if (monsterCount.ContainsKey(m))
                        {
                            if (monsterCount[m] < encMonsterCount[m]) monsterCount[m] = encMonsterCount[m];
                        }
                        else
                        {
                            monsterCount.Add(m, encMonsterCount[m]);
                        }
                    }
                }
            }

            //merge with what's already there.
            foreach (DataGridViewRow r in dgMonsterMini.Rows)
            {
                Monster m = r.Tag as Monster;
                if (m == null) continue;
                if (monsterCount.ContainsKey(m))
                {
                    int curVal = 0;
                    if(!Int32.TryParse(r.Cells["colNumber"].Value.ToString(), out curVal)){
                        MessageBox.Show("Count of monster "+m.Name+" isn't valid, resetting it");
                        curVal = 0;
                    }
                    if(curVal<monsterCount[m]){
                        r.Cells["colNumber"].Value = curVal.ToString();
                    }
                    monsterCount.Remove(m);
                }
            }
            foreach (Monster m in monsterCount.Keys) AddMonsterMini(m, monsterCount[m]);
        }

        


    }
}
