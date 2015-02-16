using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DnDMonsters
{
    [Serializable]
    public class Spellbook
    {
        public string Name;
        [NonSerialized]
        public HashSet<Spell> Spells = new HashSet<Spell>();
        public List<string> SpellNames = new List<string>();

        [OnSerializing()]
        internal void OnSerializingMethod(StreamingContext context)
        {
            SpellNames = new List<string>();
            if (Spells != null)
            {
                foreach (Spell s in Spells)
                {
                    SpellNames.Add(s.Name);
                }
            }
        }
        [OnDeserialized()]
        internal void OnDeserializedMethod(StreamingContext context)
        {
            List<Spell> allSpells = (List<Spell>)context.Context;
            if (allSpells == null) return;
            Spells = new HashSet<Spell>();
            FixupSpells(allSpells);
        }

        internal void FixupSpells(List<Spell> allSpells){
            //fixup spells
            if (Spells == null) Spells = new HashSet<Spell>();
            if (SpellNames != null && allSpells != null)
            {
                foreach (string s in SpellNames)
                {
                    foreach (Spell sp in allSpells)
                    {
                        if (sp.Name == s)
                        {
                            Spells.Add(sp);
                            break;
                        }
                    }
                }
            }
        }


    }
}
