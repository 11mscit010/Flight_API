using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AirlineService.Model
{
    public class AirlineModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Logo { get; set; }
        public int ContactNumber { get; set; }
        public string Address { get; set; }
        public bool IsBlock { get; set; }
        public string FileAsBase64 { get; set; }
    }
}
