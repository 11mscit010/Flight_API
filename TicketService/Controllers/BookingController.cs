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
using System.Net.Http;
using System.Net.Http.Headers;
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
                TicketPrice = input.TicketPrice,
                ReturnFlightId = input.ReturnFlightId,
                ReturnDate = input.ReturnDate
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
        public ActionResult<string> GetBookingByUserId(int userId, bool fromHistory)
        {
            var bookingDetails = bookingRepository.GetBookingByUserId(userId,fromHistory);
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

        [HttpGet("CreatePDFTicket")]
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
            //return File(file, "application/pdf");

            //var memory = new MemoryStream();

            //using (var stream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            //{
            //    await stream.CopyToAsync(memory);
            //}
            //memory.Position = 0;

            //return File(memory, "application/pdf", Path.GetFileName(file));
            return Ok(file);


        }

        [HttpGet("GetDetailByBookingId")]
        public async Task<ActionResult<string>> GetDetailByBookingId(int id)
        {
            try
            {
                var bookingMaster = bookingRepository.GetBookingById(id);
                var bookDetails = bookingRepository.GetBookingDetailById(id);
                dynamic booking = "";
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://localhost:9200/");
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    //GET Method
                    var ids = string.Join(',', bookingMaster.FlightId, bookingMaster.ReturnFlightId);
                    HttpResponseMessage response = await client.GetAsync("api/Flight/GetListByIds?ids=" + ids);
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();

                        var flightDetail = JsonConvert.DeserializeObject<List<FlightModel>>(JsonConvert.DeserializeObject<object>(apiResponse).ToString());
                        var flight1 = flightDetail.FirstOrDefault(z => z.Id == bookingMaster.FlightId);
                        var flight2 = flightDetail.FirstOrDefault(z => z.Id == bookingMaster.ReturnFlightId);

                        booking = new
                        {
                            Id = bookingMaster.Id,
                            BookingDate = bookingMaster.BookingDate.ToString("dd/MM/yyyy HH:mm"),
                            Email = bookingMaster.Email,
                            FlightId = bookingMaster.FlightId,
                            NoOfSeat = bookingMaster.NoOfSeat,
                            PNR = bookingMaster.PNR,
                            TicketPrice = bookingMaster.TicketPrice,
                            UserId = bookingMaster.UserId,
                            BookingDetails = bookDetails,
                            ReturnDate = bookingMaster.ReturnDate.ToString("dd/MM/yyyy HH:mm"),
                            ReturnFlightId = bookingMaster.ReturnFlightId,
                            FlightRoute = flight1 != null ? flight1.FromPlace + " To " + flight1.ToPlace : "",
                            ReturnFlightRoute = flight2 != null ? flight2.FromPlace + " To " + flight2.ToPlace : ""
                        };
                    }
                }

                //booking = new
                //{
                //    Id = bookingMaster.Id,
                //    BookingDate = bookingMaster.BookingDate,
                //    Email = bookingMaster.Email,
                //    FlightId = bookingMaster.FlightId,
                //    NoOfSeat = bookingMaster.NoOfSeat,
                //    PNR = bookingMaster.PNR,
                //    TicketPrice = bookingMaster.TicketPrice,
                //    UserId = bookingMaster.UserId,
                //    BookingDetails = bookDetails
                //};
                var result = JsonConvert.SerializeObject(booking);
                return Ok(result);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
