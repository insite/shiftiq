using System;
using System.Collections.Generic;

using Newtonsoft.Json;

using Shift.Common;
using Shift.Constant;

using ShiftHumanizer = Shift.Common.Humanizer;

namespace InSite.Domain.Records
{
    public class Expiration
    {
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        public ExpirationType Type { get; set; }

        public DateTimeOffset? Date { get; set; }
        public Lifetime Lifetime { get; set; }

        public Expiration()
        {
            Lifetime = new Lifetime();
        }

        public Expiration(string type, DateTimeOffset? date, int? quantity, string unit)
        {
            Type = type.ToEnum(ExpirationType.None);
            Date = date;
            Lifetime = new Lifetime(quantity ?? 0, unit);
        }

        public Expiration(ExpirationType type, DateTimeOffset? date, int? quantity, string unit)
        {
            Type = type;
            Date = date;
            Lifetime = new Lifetime(quantity ?? 0, unit);
        }

        public bool ShouldSerializeLifetime()
            => Lifetime != null && Lifetime.Quantity > 0;

        public override bool Equals(Object o)
        {
            // Check for null and compare run-time types.
            if ((o == null) || !GetType().Equals(o.GetType()))
                return false;

            var p = (Expiration)o;
            if (p.Type != Type)
                return false;

            if ((p.Date == null && Date != null) || (p.Date != null && Date == null))
                return false;

            if (p.Date != null && Date != null && p.Date != Date)
                return false;

            if ((p.ShouldSerializeLifetime() && !ShouldSerializeLifetime()) || (!p.ShouldSerializeLifetime() && ShouldSerializeLifetime()))
                return false;

            if (p.Lifetime != null && Lifetime != null && (p.Lifetime.Quantity != Lifetime.Quantity || p.Lifetime.Unit != Lifetime.Unit))
                return false;

            return true;
        }

        public override int GetHashCode()
        {
            int hashCode = -1861070036;
            hashCode = hashCode * -1521134295 + Type.GetHashCode();
            hashCode = hashCode * -1521134295 + Date.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<Lifetime>.Default.GetHashCode(Lifetime);
            return hashCode;
        }

        public override string ToString()
            => ToString(TimeZones.Pacific);

        public string ToString(TimeZoneInfo tz)
        {
            var s = Type.ToString();

            if (Type == ExpirationType.Fixed && Date.HasValue)
                s += " " + TimeZones.FormatDateOnly(Date.Value, tz);

            else if (Type == ExpirationType.Relative && Lifetime != null && Lifetime.Quantity > 0 && Lifetime.Unit != null)
                s += " " + ShiftHumanizer.ToQuantity(Lifetime.Quantity, Lifetime.Unit);

            return s;
        }
    }
}
