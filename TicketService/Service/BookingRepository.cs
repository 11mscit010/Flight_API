using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using BookingService.Entity;
using BookingService.Model;

namespace BookingService.Service
{
    public class BookingRepository : IBookingRepository
    {
        private readonly AppDbContext context;
        public BookingRepository(AppDbContext _context)
        {
            context = _context;
        }

        public int AddBooking(BookingEntity input)
        {
            var booking = context.BookingRepository.Add(input);
            context.SaveChanges();
            if (booking.Entity != null)
                return booking.Entity.Id;

            return -1;
        }

        public void AddBookingDetails(List<BookingDetailEntity> input)
        {
            context.BookingDetailRepository.AddRange(input);
            context.SaveChanges();
        }

        public async Task<bool> CancelBooking(int BookingId)
        {
            var booking = context.BookingRepository.FirstOrDefault(z => z.Id == BookingId);

            //Check flight starttime is >= 24 hours.
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:9000/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                //GET Method
                HttpResponseMessage response = await client.GetAsync("Flight/AllowToCancelTicket?flightId=" + booking.FlightId);
                if (response.StatusCode == System.Net.HttpStatusCode.Accepted || response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var bookingDetail = context.BookingDetailRepository.Where(z => z.BookingId == BookingId).ToList();
                    context.BookingDetailRepository.RemoveRange(bookingDetail);
                    context.BookingRepository.Remove(booking);

                    context.SaveChanges();
                }
                else
                {
                    return false;
                }
            }
            return true;
        }

        public List<BookingEntity> GetTicketByPNR(string PNR, int userid)
        {
            var booking = context.BookingRepository.Where(z => z.PNR == PNR && z.UserId ==userid).ToList();
            return booking;
        }

        public List<BookingEntity> GetTicketHistory(string email, int userid)
        {
            var booking = context.BookingRepository.Where(z => z.UserId == userid &&
                z.Email.ToLower().Contains(email.ToString())).ToList();
            return booking;
        }

        public BookingEntity GetBookingById(int bookingID)
        {
            var booking = context.BookingRepository.FirstOrDefault(z => z.Id== bookingID);
            return booking;
        }

        public List<BookingDetailEntity> GetBookingDetailById(int bookingID)
        {
            var bookingDetail = context.BookingDetailRepository.Where(z => z.BookingId == bookingID).ToList();
            return bookingDetail;
        }
        public List<BookingEntity> GetBookingByUserId(int userID, bool fromHistory)
        {
            if (fromHistory)
            {
                return context.BookingRepository.Where(z => z.UserId == userID).ToList();
            }
            else
            {
                return context.BookingRepository.Where(z => z.UserId == userID && z.BookingDate > DateTime.Now).ToList();
            }
        }
    }
}
