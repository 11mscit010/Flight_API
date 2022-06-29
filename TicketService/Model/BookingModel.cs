using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookingService.Entity;

namespace BookingService.Model
{
    public class BookingModel
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public int NoOfSeat { get; set; }
        public string PNR { get; set; }
        public int FlightId { get; set; }
        public DateTime BookingDate { get; set; }
        public int UserId { get; set; }
        public int TicketPrice { get; set; }
        public int ReturnFlightId { get; set; }
        public DateTime ReturnDate { get; set; }

        public List<BookingDetailEntity> BookingDetails { get; set; }
    }
}
