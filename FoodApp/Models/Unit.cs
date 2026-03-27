using System.ComponentModel.DataAnnotations;

namespace FoodApp.Models
{
    public class Unit
    {
        public int id { get; set; }
        public string name { get; set; } = string.Empty;
        public decimal volumeEquivalent { get; set; }
        public string desc { get; set; } = string.Empty;

        public override string ToString()
        {
            return $"{name} (id: {id}, volumeEquivalent: {volumeEquivalent})";
        }
    }

    public class UnitConvertResp
    {
        public Unit unit { get; set; }
        public float newUnitAmount { get; set; }
    }

    public class UnitCreateRequest
    {
        public string name { get; set; } = string.Empty;
        public decimal volumeEquivalent { get; set; }
        public string desc { get; set; } = string.Empty;
    }
    public class UnitUpdateRequest
    {
        public string? name { get; set; }
        public decimal? volumeEquivalent { get; set; }
        public string? desc { get; set; }
    }
}
