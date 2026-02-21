using System.Text.Json.Serialization;

namespace AS.Fields.Domain.ValueObjects
{
    public class Boundary
    {
        protected Boundary() { }

        [JsonConstructor]
        public Boundary(int latMax, int latMin, int longMax, int longMin)
        {
            LatMax = latMax;
            LatMin = latMin;
            LongMax = longMax;
            LongMin = longMin;
        }

        public int LatMax { get; init; }
        public int LatMin { get; init; }
        public int LongMax { get; init; }
        public int LongMin { get; init; }
    }
}
