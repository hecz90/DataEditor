using System.Collections;
using System.Linq;
using DataEditor.Attributes;

namespace Example.Data.Heroes
{
    public class HeroLevel
    {
        [EntryField, Key, LabelField("Уровень", 0)]
        public int Level;

        [EntryField, InputField("Нужно XP", 1, InputType.Int)]
        public int NeedExp;

        [EntryField, InputField("Атака", 1, InputType.Double)]
        public double Attack;

        [EntryField, InputField("Защита", 1, InputType.Double)]
        public double Defence;


        [Constructor]
        public HeroLevel(ICollection collection)
        {
            Level = collection.Count > 0 ? collection.Cast<HeroLevel>().Max(l => l.Level) + 1 : 1;
        }
    }
}