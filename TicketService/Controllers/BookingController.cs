using BookingService.Entity;
using BookingService.Model;
using BookingService.Service;
using BookingService.Utility;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BookingService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        public IConfiguration Configuration { get; }
        private readonly IBookingRepository bookingRepository;
        private readonly AppDbContext context;

        public BookingController(IConfiguration configuration, AppDbContext _context)
        {
            Configuration = configuration;
            context = _context;
            bookingRepository = new BookingRepository(context);
        }

        [HttpPost("BookTicket")]
        public ActionResult<string> BookTicket(BookingModel input)
        {
            var booking = new BookingEntity()
            {
                Id = input.Id,
                Email = input.Email,
                NoOfSeat = input.NoOfSeat,
                PNR = GeneratePNR(),
                FlightId = input.FlightId,
                BookingDate = input.BookingDate,
                UserId = input.UserId,
                TicketPrice = input.TicketPrice
            };
            var result = bookingRepository.AddBooking(booking);
            if (result <= 0)
                return BadRequest("Flight Booking Failed");

            var bookingDetails = input.BookingDetails.Select(z =>
            {
                z.BookingId = result;
                return z;
            }).ToList();
            bookingRepository.AddBookingDetails(bookingDetails);

            var book = JsonConvert.SerializeObject(booking);
            return Ok(book);
        }

        [HttpDelete("CancelTicket")]
        public ActionResult<string> CancelTicket(int id)
        {
            var result = bookingRepository.CancelBooking(id);
            if (result.Result)
                return Ok(true);
            else
                return Ok(false);
        }

        [HttpGet("GetTicketByPNR")]
        public ActionResult<string> GetTicketByPNR(string PNR, int userid)
        {
            var booking = bookingRepository.GetTicketByPNR(PNR,userid);
            var result = JsonConvert.SerializeObject(booking);
            return Ok(result);
        }

        [HttpGet("GetBookingHistory")]
        public ActionResult<string> GetBookingHistory(string email, int userid)
        {
            var booking = bookingRepository.GetTicketHistory(email,userid);
            var result = JsonConvert.SerializeObject(booking);
            return Ok(result);
        }

        [HttpGet("GetBookingById")]
        public ActionResult<string> GetBookingById(int id)
        {
            var booking = bookingRepository.GetBookingById(id);
            var result = JsonConvert.SerializeObject(booking);
            return Ok(result);
        }

        [HttpGet("GetBookingDetailByBookingId")]
        public ActionResult<string> GetBookingDetailByBookingId(int bookingId)
        {
            var bookingDetails = bookingRepository.GetBookingDetailById(bookingId);
            var result = JsonConvert.SerializeObject(bookingDetails);
            return Ok(result);
        }

        [HttpGet("test")]
        public ActionResult<string> test()
        {
            return Ok(GeneratePNR());
        }

        [HttpGet("GetBookingByUserId")]
        public ActionResult<string> GetBookingByUserId(int userId)
        {
            var bookingDetails = bookingRepository.GetBookingByUserId(userId);
            var result = JsonConvert.SerializeObject(bookingDetails);
            return Ok(result);
        }

        private string GeneratePNR()
        {
            var random = new Random();
            const string characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string numbers = "0123456789";
            var charPnr = new string(Enumerable.Repeat(characters, 3)
                .Select(z => z[random.Next(z.Length)]).ToArray());
            var numPnr = new string(Enumerable.Repeat(numbers, 5)
                .Select(z => z[random.Next(z.Length)]).ToArray());
            return charPnr + numPnr;

        }

        [HttpPost("CreatePDFTicket")]
        public async Task<IActionResult> CreatePDFTicket(int bookingId)
        {
            var booking = bookingRepository.GetBookingById(bookingId);
            var bookingDetails = bookingRepository.GetBookingDetailById(bookingId);

            var model = new BookingModel()
            {
                BookingDate = booking.BookingDate,
                Email = booking.Email,
                FlightId = booking.FlightId,
                NoOfSeat = booking.NoOfSeat,
                PNR = booking.PNR,
                BookingDetails = bookingDetails
            };

            //Generate PDF:
            var file = PDFGenerator.GenerateTicket(model);
            return File(file, "application/pdf");

            //var bytes = await System.IO.File.ReadAllBytesAsync(file);
            //return File(bytes, "application/pdf", Path.GetFileName(file));

            //return File(@"D:\Harshal\H&T Project\FlightBooking_API\TicketService\Downloads\Sample_Test.pdf", "application/pdf");
            //var filenamePath = Path.Combine(Directory.GetCurrentDirectory(), "Downloads", booking.PNR+DateTime.Today.Minute+"_Ticket.pdf");

            //var globalSettings = new GlobalSettings
            //{
            //    ColorMode = ColorMode.Color,
            //    Orientation = Orientation.Portrait,
            //    PaperSize = PaperKind.A4,
            //    Margins = new MarginSettings { Top = 10 },
            //    DocumentTitle = "PDF Report",
            //    Out = @"D:\PDFCreator\Employee_Report.pdf"
            //};
            //var objectSettings = new ObjectSettings
            //{
            //    PagesCount = true,
            //    HtmlContent = PDFGenerator.GetPDFTicketHTML(model),
            //    WebSettings = { DefaultEncoding = "utf-8", UserStyleSheet = Path.Combine(Directory.GetCurrentDirectory(), "Utility", "PDFTicket.css") },
            //    HeaderSettings = { FontName = "Arial", FontSize = 9, Right = "Page [page] of [toPage]", Line = true },
            //    FooterSettings = { FontName = "Arial", FontSize = 9, Line = true, Center = "Report Footer" }
            //};
            //var pdf = new HtmlToPdfDocument()
            //{
            //    GlobalSettings = globalSettings,
            //    Objects = { objectSettings }
            //};
            //var file = converter.Convert(pdf);
            //return File(file, "application/pdf");

        }
    }
}
