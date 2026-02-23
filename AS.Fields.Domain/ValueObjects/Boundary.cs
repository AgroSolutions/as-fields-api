using System.Text.Json.Serialization;

namespace AS.Fields.Domain.ValueObjects
{
    public class Boundary
    {
        protected Boundary() { }

        [JsonConstructor]
        public Boundary(int latMin, int latMax, int longMin , int longMax)
        {
            LatMax = latMax;
            LatMin = latMin;
            LongMax = longMax;
            LongMin = longMin;
        }

        public int LatMin { get; init; }
        public int LatMax { get; init; }
        public int LongMin { get; init; }
        public int LongMax { get; init; }
    }
}
