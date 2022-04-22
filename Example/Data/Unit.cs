using System.Collections;
using System.Linq;
using DataEditor.Attributes;

namespace Example.Data
{
    public class Unit
    {
        [Key, EntryField, LabelField("Id", 0)]
        public int Id { get; set; }

        [EntryField, InputField("Имя", 1, InputType.String)]
        public string Name { get; set; }
        
        protected Unit(ICollection collection)
        {
            Id = collection.Count > 0 ? collection.Cast<Unit>().Max(l => l.Id) + 1 : 1;
        }
    }
}