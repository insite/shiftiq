using System;
using System.Collections.Generic;
using System.Text;

namespace InSite.Domain.Foundations
{
    [Serializable]
    public class PersonList : List<Person>
    {
        public PersonList() { }

        public PersonList(Person[] people)
        {
            Clear();
            foreach (var person in people)
                Add(person);
        }

        public string GetIdentifiersAsCsv()
        {
            var sb = new StringBuilder();
            foreach (var item in this)
            {
                if (sb.Length > 0)
                    sb.Append(",");
                sb.Append(item.User);
            }
            return sb.ToString();
        }
    }
}
