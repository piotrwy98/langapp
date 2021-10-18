using System;

namespace LangApp.Shared.Models
{
    public class Category
    {
        public Guid Id { get; set; }

        public Level Level { get; set; }

        public string Name { get; set; }

        public override string ToString()
        {
            if(Level != null)
                return Name + " (" + Level.Name + ")";

            return Name;
        }
    }
}
