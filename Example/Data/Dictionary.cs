using System.Collections.Generic;
using DataEditor.Attributes;
using Example.Data.Heroes;

namespace Example.Data
{
    public class Dictionary
    {
        [Tab, CollectionField("Герои", 4, typeof(Hero), Options = ListOptions.ComplexEntry, AutoApply = true)]
        public Dictionary<int, Hero> Heroes = new Dictionary<int, Hero>();
        
        #region Editing
        [Action("Сохнанить на диск")]
        public static void Save()
        {
            // Save
        }
        
        [Action("Загрузить с диска")]
        public static void Reload()
        {
            //reload
        }
        #endregion

    }
}