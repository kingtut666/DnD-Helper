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
    public partial class EncounterGenerator : Form
    {
        public EncounterGenerator()
        {
            InitializeComponent();
        }

        DnDHelper parent;
        TreeNode tnMonsterByName;
        TreeNode tnMonsterByXP;
        TreeNode tnMonsterBySize;
        TreeNode tnMonsterByType;
        TreeNode tnMonsterByAlignment;
        Dictionary<int, TreeNode> nodesByXP = new Dictionary<int, TreeNode>();
        Dictionary<string, TreeNode> nodesBySize = new Dictionary<string, TreeNode>();
        Dictionary<string, TreeNode> nodesByType = new Dictionary<string, TreeNode>();
        Dictionary<string, TreeNode> nodesByAlignment = new Dictionary<string, TreeNode>();


        #region Public methods
        public void SetParent(DnDHelper frm)
        {
            parent = frm;
            PopulateMonsterTree();

            parent.MonstersChanged += parent_MonstersChanged;
            parent.curEncounter.Monsters.ListChanged += parent_ActiveMonstersChanged;
            
            numNPlayers.Value = Properties.Settings.Default.NumPlayers;
            numLevel.Value = Properties.Settings.Default.PlayersLevel;
            comboDifficulty.SelectedIndex = Properties.Settings.Default.Difficulty;
            CalcXP();
            
            DisableMonsterTreeByXP();
        }
        #endregion

        #region Event Handlers
        void parent_ActiveMonstersChanged(object sender, ListChangedEventArgs e)
        {
            CalcXP();
        }

        void parent_MonstersChanged(object sender, ListChangedEventArgs e)
        {
            PopulateMonsterTree();
            DisableMonsterTreeByXP();
        }
        #endregion

        
        #region UI
        private void butRollMonsters_Click(object sender, EventArgs e)
        {
            checkListMonsters.Items.Clear();
            availMonsters = new Dictionary<int,List<Monster>>();
            minMonsterXP = 0;
            foreach (TreeNode n in nodesByXP.Values)
            {
                foreach (TreeNode n2 in n.Nodes)
                {
                    if (!n2.Checked) continue;
                    Monster m = n2.Tag as Monster;
                    if (m == null)
                        continue;
                    if (minMonsterXP == 0 || minMonsterXP >= m.XP) minMonsterXP = m.XP;
                    if (!availMonsters.ContainsKey(m.XP)) availMonsters.Add(m.XP, new List<Monster>());
                    availMonsters[m.XP].Add(m);
                }
            }

            decimal XP = numTargetXP.Value / EncounterMultiplier((int)numMonsters.Value);

            List<Monster> mons = TryRoll((int)XP, (int)numMonsters.Value, false);
            if (mons != null)
            {
                foreach (Monster m in mons)
                {
                    checkListMonsters.Items.Add(m, CheckState.Checked);
                }
            }
        }

        private void butAddSelected_Click(object sender, EventArgs e)
        {
            parent.ToggleSuspendLayout();
            foreach (object o in checkListMonsters.CheckedItems)
            {
                Monster m = o as Monster;
                bool Summary = false;
                foreach (ActualMonster a in parent.curEncounter.Monsters)
                {
                    if (a.Monster == m && !a.Summary)
                    {
                        Summary = true;
                        break;
                    }
                }
                parent.curEncounter.AddMonster(m, Summary);
            }
            parent.ToggleSuspendLayout();
        }

        private void butClear_Click(object sender, EventArgs e)
        {
            parent.ClearMonsters();
        }


        private void numNPlayers_ValueChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.NumPlayers = (int)numNPlayers.Value;
            Properties.Settings.Default.Save();
            CalcXP();
        }

        private void numLevel_ValueChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.PlayersLevel = (int)numLevel.Value;
            Properties.Settings.Default.Save();
            CalcXP();
        }

        private void comboDifficulty_SelectedIndexChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.Difficulty = comboDifficulty.SelectedIndex;
            Properties.Settings.Default.Save();
            CalcXP();
        }

        private void treeMonsters_AfterCheck(object sender, TreeViewEventArgs e)
        {
            Monster m = e.Node.Tag as Monster;

            //if (e.Action == TreeViewAction.Unknown) return;
            if (m == null)
            {
                //Not a monster - ripple
                foreach (TreeNode t in e.Node.Nodes)
                {
                    t.Checked = e.Node.Checked;
                    t.ForeColor = e.Node.ForeColor;
                }
            }
            else
            {
                //Monster

                //By XP
                IanUtility.TreeViewHelper.UpdateSetting<int>(nodesByXP, e.Node.Checked, m.XP, m);
                //By Alignment
                IanUtility.TreeViewHelper.UpdateSetting<string>(nodesByAlignment, e.Node.Checked, m.Alignment, m);
                //By Size
                IanUtility.TreeViewHelper.UpdateSetting<string>(nodesBySize, e.Node.Checked, m.Size, m);
                //By Type
                IanUtility.TreeViewHelper.UpdateSetting<string>(nodesByType, e.Node.Checked, m.Type, m);
                //name
                IanUtility.TreeViewHelper.UpdateSetting(tnMonsterByName, e.Node.Checked, m);

                //Supertrees
                IanUtility.TreeViewHelper.UpdateSupertree(tnMonsterByName);
                IanUtility.TreeViewHelper.UpdateSupertree<int>(nodesByXP, tnMonsterByXP);
                IanUtility.TreeViewHelper.UpdateSupertree<string>(nodesByAlignment, tnMonsterByAlignment);
                IanUtility.TreeViewHelper.UpdateSupertree<string>(nodesBySize, tnMonsterBySize);
                IanUtility.TreeViewHelper.UpdateSupertree<string>(nodesByType, tnMonsterByType);



            }
        }

        private void numTargetXP_ValueChanged(object sender, EventArgs e)
        {
            DisableMonsterTreeByXP();
        }
        #endregion



        void CalcXP()
        {
            if (comboDifficulty.SelectedIndex < 0)
            {
                lblThresholdXP.Text = " - ";
                lblCurrentXP.Text = " - ";
                lblRemainingXP.Text = " - ";
                return;
            }

            int partyXP = (int)(numNPlayers.Value);
            partyXP *= diff[(int)numLevel.Value, comboDifficulty.SelectedIndex];

            lblThresholdXP.Text = partyXP.ToString();

            int monsterXP = 0;
            foreach (ActualMonster m in parent.curEncounter.Monsters)
            {
                monsterXP += m.Monster.XP;
            }

            monsterXP = (int)((decimal)monsterXP * EncounterMultiplier(parent.curEncounter.Monsters.Count));

            lblCurrentXP.Text = monsterXP.ToString();
            lblRemainingXP.Text = (partyXP - monsterXP).ToString();
            if (partyXP < monsterXP) numTargetXP.Value = 0;
            else numTargetXP.Value = partyXP - monsterXP;

        }
        
        void PopulateMonsterTree()
        {
            try
            {
                treeMonsters.Nodes.Clear();
            }
            catch (Exception) { }

            //Roots
            tnMonsterByXP = IanUtility.TreeViewHelper.AddNewTreeNode(treeMonsters, "By XP", true);
            tnMonsterByAlignment = IanUtility.TreeViewHelper.AddNewTreeNode(treeMonsters, "By Alignment", true);
            tnMonsterBySize = IanUtility.TreeViewHelper.AddNewTreeNode(treeMonsters, "By Size", true);
            tnMonsterByType = IanUtility.TreeViewHelper.AddNewTreeNode(treeMonsters, "By Type", true);
            tnMonsterByName = IanUtility.TreeViewHelper.AddNewTreeNode(treeMonsters, "By Name", true);
            nodesByAlignment = new Dictionary<string, TreeNode>();
            nodesBySize = new Dictionary<string, TreeNode>();
            nodesByType = new Dictionary<string, TreeNode>();
            nodesByXP = new Dictionary<int, TreeNode>();

            foreach (Monster m in parent.Monsters.Values)
            {
                //By XP
                IanUtility.TreeViewHelper.AddSubTreeNode<int>(tnMonsterByXP, nodesByXP, m.Name, true, m, m.XP);
                //By Alignment
                IanUtility.TreeViewHelper.AddSubTreeNode<string>(tnMonsterByAlignment, nodesByAlignment, m.Name, true, m, m.Alignment);
                //By Size
                IanUtility.TreeViewHelper.AddSubTreeNode<string>(tnMonsterBySize, nodesBySize, m.Name, true, m, m.Size);
                //By Type
                IanUtility.TreeViewHelper.AddSubTreeNode<string>(tnMonsterByType, nodesByType, m.Name, true, m, m.Type);
                //name
                IanUtility.TreeViewHelper.AddSubTreeNode<object>(tnMonsterByName, null, m.Name, true, m, null);

            }

            //order nodes
            try
            {
                treeMonsters.TreeViewNodeSorter = new TreeSorter();
            
            treeMonsters.Sort();
            }
            catch (Exception) { }

        }
        public class TreeSorter : System.Collections.IComparer
        {
            public int Compare(object a, object b)
            {
                TreeNode x = a as TreeNode;
                TreeNode y = b as TreeNode;

                if (x == null && y == null) return 0;
                if (x == null) return -1;
                if (y == null) return 1;
                if (x.Tag == null)
                {
                    int i, j;
                    if (!Int32.TryParse(x.Text, out i) || !Int32.TryParse(y.Text, out j))
                    {
                        //one or both are strings, do a string compare
                        return String.Compare(x.Text, y.Text);
                    }
                    if (i == j) return 0;
                    if (i < j) return -1;
                    return 1;
                }
                else
                {
                    return String.Compare(x.Text, y.Text);
                }
            }
        }
        void UpdateDisableMonster(TreeNode n)
        {
            Monster m = n.Tag as Monster;
            //update byName
            IanUtility.TreeViewHelper.UpdateSetting(tnMonsterByName, n.ForeColor == Color.Gray, n.Tag, IanUtility.TreeViewHelper.UpdateType.Disabled);
            //update bySize
            IanUtility.TreeViewHelper.UpdateSetting<string>(nodesBySize, n.ForeColor == Color.Gray, m.Size, m, IanUtility.TreeViewHelper.UpdateType.Disabled);
            //update byAlignment
            IanUtility.TreeViewHelper.UpdateSetting<string>(nodesByAlignment, n.ForeColor == Color.Gray, m.Alignment, m, IanUtility.TreeViewHelper.UpdateType.Disabled);
            //update byType
            IanUtility.TreeViewHelper.UpdateSetting<string>(nodesByType, n.ForeColor == Color.Gray, m.Alignment, m, IanUtility.TreeViewHelper.UpdateType.Disabled);

        }
        void DisableMonsterTreeByXP()
        {
            //Disable the byXP
            foreach (TreeNode n in tnMonsterByXP.Nodes)
            {
                int XP = Int32.Parse(n.Text);
                if (XP > numTargetXP.Value)
                {
                    n.ForeColor = Color.Gray;
                    n.Checked = false;
                    foreach (TreeNode n2 in n.Nodes)
                    {
                        n2.ForeColor = Color.Gray;
                        n2.Checked = false;
                        UpdateDisableMonster(n2);
                    }
                }
                else
                {
                    if (n.ForeColor == Color.Gray && checkResetTree.Checked)
                    {
                        n.ForeColor = Color.Black;
                        n.Checked = true;
                        foreach (TreeNode n2 in n.Nodes)
                        {
                            n.ForeColor = Color.Black;
                            n2.Checked = true;
                            UpdateDisableMonster(n2);
                        }
                    }
                }


            }

            //ripple updates by parent tree



        }



        int[,] diff = new[,] { {1,1,1,1}, 
            { 25, 50, 75, 100}, //Level1
            {50,100,150,200},
            {75,150,225,400},
            {125,250,375,500},
            {250,500,750,1100}, //Level5
            {300,600,900,1400},
            {350,750,1100,1700},
            {450,900,1400,2100},
            {550,1100,1600,2400},
            {600,1200,1900,2800}, //Level10
            {800,1600,2400,3600},
            {1000,2000,3000,4000},
            {1100,2200,3400,5100},
            {1250,2500,3800,5700},
            {1400,2800,4300,6400}, //Level15
            {1600,3200,4800,7200},
            {2000,4200,6300,9500},
            {2100,4200,6300,9500},
            {2400,4900,7300,10900},
            {2800,5700,8500,12700} // Level20        
        };
        int minMonsterXP = 0;
        Dictionary<int, List<Monster>> availMonsters = null;
        
        List<Monster> ListAvailMonsters(int minXP, int maxXP)
        {
            List<Monster> ret = new List<Monster>();
            foreach (int xp in availMonsters.Keys)
            {
                if (minXP != -1 && minXP > xp) continue;
                if (maxXP != -1 && maxXP < xp) continue;
                ret.AddRange(availMonsters[xp]);
            }
            return ret;
        }
        List<Monster> GetMonsterGroup(int XP, int nMonsters)
        {
            List<Monster> ret = new List<Monster>();
            if (nMonsters == 0) return ret;
            if (XP / nMonsters < minMonsterXP) return ret;
            int targXP = XP / nMonsters;
            int highestSelected = 0;
            foreach (int xp in availMonsters.Keys)
                if (highestSelected < xp && xp < targXP && availMonsters[xp].Count > 0) highestSelected = xp;
            if (highestSelected == 0) return null;
            int i = DnDHelper.rand.Next(availMonsters[highestSelected].Count);
            Monster m = availMonsters[highestSelected][i];
            for (int j = 0; j < nMonsters; j++) ret.Add(m);
            return ret;
        }
        Monster FindBoss(int maxXP)
        {
            int xp = 0;
            foreach (int k in availMonsters.Keys)
                if (xp < k && k < maxXP) xp = k;
            if (xp == 0) return null;
            int i = DnDHelper.rand.Next(availMonsters[xp].Count);
            return availMonsters[xp][i];
        }
        List<Monster> TryRoll(int XP, int nMonsters, bool specialPicked)
        {
            List<Monster> ret = new List<Monster>();
            if (XP < minMonsterXP || nMonsters == 0) return null;
            if (radioRandom.Checked)
            {
                //pick a monster, with XP
                List<Monster> possible = ListAvailMonsters(-1, XP);
                if (possible == null || possible.Count == 0) return null;
                int i = 0;
                Monster m = null;
                i = DnDHelper.rand.Next(possible.Count);
                m = possible[i];
                ret = TryRoll(XP - m.XP, nMonsters - 1, true);
                if (ret == null) ret = new List<Monster>();
                ret.Add(m);
                return ret;
            }
            else if (radioMaxGroup.Checked)
            {
                return GetMonsterGroup(XP, nMonsters);
            }
            else if (radioMaxSingle.Checked)
            {
                if (!specialPicked)
                {
                    //Pick largest
                    List<int> xps = availMonsters.Keys.ToList();
                    xps.Sort((a, b) => b.CompareTo(a));
                    Monster m = null;
                    foreach (int i in xps)
                    {
                        if (i >= XP) continue;
                        //Pick a monster at this level
                        int r = DnDHelper.rand.Next(availMonsters[i].Count);
                        m = availMonsters[i][r];
                        ret = GetMonsterGroup(XP - m.XP, nMonsters - 1);
                        if (ret != null)
                        {
                            ret.Add(m);
                            return ret;
                        }
                    }
                    return null;
                }
                else
                {
                    //Pick remainder
                    return GetMonsterGroup(XP, nMonsters);
                }
            }
            else if (radioBoss25.Checked)
            {
                Monster m = FindBoss((XP * 3) / 4);
                if (m == null) return null;
                ret = GetMonsterGroup(XP - m.XP, nMonsters - 1);
                ret.Add(m);
                return ret;
            }
            else if (radioBoss50.Checked)
            {
                Monster m = FindBoss(XP / 2);
                if (m == null) return null;
                ret = GetMonsterGroup(XP - m.XP, nMonsters - 1);
                ret.Add(m);
                return ret;
            }
            else if (radioBoss75.Checked)
            {
                Monster m = FindBoss(XP / 4);
                if (m == null) return null;
                ret = GetMonsterGroup(XP - m.XP, nMonsters - 1);
                ret.Add(m);
                return ret;
            }

            return null;
        }
        decimal EncounterMultiplier(int nMonsters)
        {
            if (nMonsters == 1) return 1;
            else if (nMonsters == 2) return 1.5m;
            else if (nMonsters >= 3 && nMonsters <= 6) return 2;
            else if (nMonsters >= 7 && nMonsters <= 10) return 2.5m;
            else if (nMonsters >= 11 && nMonsters <= 14) return 3;
            else return 4;
        }

        
    }
}
