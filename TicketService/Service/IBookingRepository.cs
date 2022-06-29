using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookingService.Entity;
using BookingService.Model;

namespace BookingService.Service
{
    public interface IBookingRepository
    {
        int AddBooking(BookingEntity input);
        void AddBookingDetails(List<BookingDetailEntity> input);
        Task<bool> CancelBooking(int BookingId);
        List<BookingEntity> GetTicketByPNR(string PNR, int userid);
        List<BookingEntity> GetTicketHistory(string email, int userid);
        BookingEntity GetBookingById(int bookingID);
        List<BookingDetailEntity> GetBookingDetailById(int bookingID);
        List<BookingEntity> GetBookingByUserId(int userID, bool fromHistory);
    }
}
