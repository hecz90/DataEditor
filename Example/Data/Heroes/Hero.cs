using System.Collections;
using System.Collections.Generic;
using DataEditor.Attributes;

namespace Example.Data.Heroes
{
    public class Hero : Unit
    {
        [CollectionField("Уровни", 8, typeof(HeroLevel), Options = ListOptions.ComplexEntry)]
        public Dictionary<int, HeroLevel> Levels = new Dictionary<int, HeroLevel>();

        [Constructor]
        public Hero(ICollection collection) : base(collection)
        {
            Levels = new Dictionary<int, HeroLevel>();
        }
    }
}