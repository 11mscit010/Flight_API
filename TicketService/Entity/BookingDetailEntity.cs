using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BookingService.Entity
{
    [Table("BookingDetail")]
    public class BookingDetailEntity
    {
        [Key]
        public int Id { get; set; }
        public int BookingId { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public string Gender { get; set; }
        public string MealType { get; set; } //0: Veg, 1: Non-Veg
        public int SeatNo { get; set; }
    }
}
