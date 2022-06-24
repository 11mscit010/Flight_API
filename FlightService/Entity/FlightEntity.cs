using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace FlightService.Entity
{
    [Table("Flight")]
    public class FlightEntity
    {
        [Key]
        public int Id { get; set; }
        public string FromPlace { get; set; }
        public string ToPlace { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string ScheduleDays { get; set; }
        public int BusinessSeat { get; set; }
        public int NonBusinessSeat { get; set; }
        public int TotalCost { get; set; }
        public int MealType { get; set; } //0:Veg, 1:Non-Veg
        public int AirlineId { get; set; }
        [DefaultValue(false)]
        public bool IsBlock { get; set; }
    }
}
