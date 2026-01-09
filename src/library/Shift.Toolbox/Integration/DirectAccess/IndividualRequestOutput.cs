using Newtonsoft.Json;

namespace Shift.Toolbox.Integration.DirectAccess
{
    public class IndividualRequestOutput
    {
        public Individual[] Individuals { get; set; }

        public string Raw { get; set; }

        public static IndividualRequestOutput Deserialize(string json)
        {
            return new IndividualRequestOutput
            {
                Individuals = JsonConvert.DeserializeObject<Individual[]>(json),
                Raw = json
            };
        }
    }
}