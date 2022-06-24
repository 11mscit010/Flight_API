using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AirlineService.Entity
{
    [Table("Airline")]
    public class AirlineEntity
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Logo { get; set; }
        public int ContactNumber { get; set; }
        public string Address { get; set; }
        public bool IsBlock { get; set; }
    }
}
