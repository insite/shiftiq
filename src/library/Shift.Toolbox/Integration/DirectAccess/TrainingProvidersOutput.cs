using Newtonsoft.Json;

namespace Shift.Toolbox.Integration.DirectAccess
{
    public class TrainingProvidersOutput
    {
        public class Item
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public Staff[] Staff { get; set; }
            public int[] Trades { get; set; }
            public string PhoneNumber { get; set; }
            public string FaxNumber { get; set; }
            public string EmailAddress { get; set; }
            public TrainingProvidersOutputAddress Address { get; set; }
            public TrainingProvidersOutputLocation[] Locations { get; set; }
        }

        public Item[] Items { get; set; }

        public string Raw { get; set; }

        public static TrainingProvidersOutput Deserialize(string json)
        {
            if (string.IsNullOrEmpty(json))
                return null;

            var items = JsonConvert.DeserializeObject<Item[]>(json);

            return new TrainingProvidersOutput { Items = items, Raw = json };
        }
    }
}