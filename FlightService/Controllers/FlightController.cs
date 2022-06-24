using FlightService.Entity;
using FlightService.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FlightController : ControllerBase
    {
        public IConfiguration Configuration { get; }
        private readonly IFlightRepository flightRepository;
        private readonly AppDbContext context;
        public FlightController(IConfiguration configuration, AppDbContext _context)
        {
            Configuration = configuration;
            context = _context;
            flightRepository = new FlightRepository(context);
        }

        [HttpPost("Add")]
        public ActionResult<string> Add(FlightEntity input)
        {
            var result = flightRepository.AddFlight(input);
            if (!result)
                return BadRequest("Add Flight Failed");

            flightRepository.SaveChanges();
            return Ok("Flight Added Successfully");
        }

        [HttpPut("Update")]
        public ActionResult<string> Update(FlightEntity input)
        {
            flightRepository.UpdateFlight(input);
            flightRepository.SaveChanges();
            return Ok("Flight Updated Successfully");
        }

        [HttpDelete("Delete")]
        public ActionResult<string> Delete(int id)
        {
            flightRepository.DeleteFlight(id);
            flightRepository.SaveChanges();
            return Ok("Flight Deleted Successfully");
        }

        [HttpGet("GetById")]
        public ActionResult<string> GetById(int id)
        {
            var flight = flightRepository.GetFlightById(id);
            var result = JsonConvert.SerializeObject(flight);
            return Ok(result);
        }

        [HttpGet("GetByLocation")]
        public ActionResult<string> GetByLocation(string source, string destination)
        {
            var flight = flightRepository.GetFlightbyLocation(source,destination);
            var result = JsonConvert.SerializeObject(flight);
            return Ok(result);
        }

        [HttpGet("AllowToCancelTicket")]
        public ActionResult<bool> AllowToCancelTicket(int flightId)
        {
            var result = flightRepository.AllowToCancelTicket(flightId);
            if (result)
                return Accepted();
            else
                return Ok();
        }

        [HttpGet("GetAll")]
        public ActionResult<string> GetAll()
        {
            var flight = flightRepository.GetAll();
            var result = JsonConvert.SerializeObject(flight);
            return Ok(result);
        }
    }
}
