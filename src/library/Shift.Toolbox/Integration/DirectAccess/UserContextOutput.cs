using Newtonsoft.Json;

namespace Shift.Toolbox.Integration.DirectAccess
{
    public class UserContextOutput
    {
        public class Item
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public Staff[] Staff { get; set; }
            public int[] Trades { get; set; }
        }

        public Item[] Items { get; set; }

        public string Raw { get; set; }

        public static UserContextOutput Deserialize(string json)
        {
            if (string.IsNullOrEmpty(json))
                return null;

            var items = JsonConvert.DeserializeObject<Item[]>(json);

            return new UserContextOutput { Items = items, Raw = json };
        }
    }
}