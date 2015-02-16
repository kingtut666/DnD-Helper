using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DnDMonsters
{
    public class Attack
    {
        public enum AttackType { Melee, Ranged };

        public string Name = "";
        public int AttackMod = 0;
        public string Damage = "";
        public string Special = "";
        public int MaxRange = 0;
        public int MaxRangeDisadv = 0;
        public AttackType Type = AttackType.Melee;
        public string ToHit()
        {
            return Attack.ToHit(AttackMod);
        }

        public static Attack Melee(string name, int attack, string damage, string special)
        {
            if (name == "" || (attack == 0 && special == "")) return null;
            Attack a = new Attack();
            a.Name = name;
            a.AttackMod = attack;
            a.Damage = damage;
            a.Special = special;
            a.Type = AttackType.Melee;
            return a;
        }
        public static Attack Ranged(string name, int attack, string damage, int norm, int max, string special)
        {
            if (name == "" || (attack == 0 && special == "")) return null;
            Attack a = new Attack();
            a.Name = name;
            a.AttackMod = attack;
            a.Damage = damage;
            a.Special = special;
            a.MaxRange = norm;
            a.MaxRangeDisadv = max;
            a.Type = AttackType.Ranged;
            return a;
        }
        static Regex otherRegex = new Regex(@"(\w+)\s*\((.*)\)");
        public static List<Attack> FromOther(string other)
        {
            if (other == "") return null;
            string[] others = other.Split(new string[] { ":::" }, StringSplitOptions.RemoveEmptyEntries);

            List<Attack> ret = new List<Attack>();

            foreach(string s in others){
                Match m = otherRegex.Match(s);
                if (!m.Success)
                    continue;
                string t = m.Groups[1].Value.Trim();
                if (String.Equals(t, "Melee", StringComparison.CurrentCultureIgnoreCase)) {
                    string[] s2 = m.Groups[2].Value.Split(new char[] { ',' }, 4);
                    if (s2.Length < 3) continue;
                    Attack a = new Attack();
                    a.Name = s2[0].Trim();
                    Int32.TryParse(s2[1].Trim(), out a.AttackMod);
                    a.Damage = s2[2].Trim();
                    if (s2.Length == 4) a.Special = s2[3].Trim();
                    ret.Add(a);
                }
                else if (String.Equals(t, "Ranged", StringComparison.CurrentCultureIgnoreCase)) {
                    string[] s2 = m.Groups[2].Value.Split(new char[] { ',' }, 6);
                    if (s2.Length < 5) continue;
                    Attack a = new Attack();
                    a.Name = s2[0].Trim();
                    Int32.TryParse(s2[1].Trim(), out a.AttackMod);
                    a.Damage = s2[2].Trim();
                    Int32.TryParse(s2[3].Trim(), out a.MaxRange);
                    Int32.TryParse(s2[4].Trim(), out a.MaxRangeDisadv);
                    if (s2.Length == 6) a.Special = s2[5].Trim();
                    ret.Add(a);
                }
                else continue;

            }

            return ret;
        }
        public static string ToHit(int i)
        {
            string s = "";
            if (i > 0) s = "+";
            s += i.ToString();
            return s;
        }

    }
}
