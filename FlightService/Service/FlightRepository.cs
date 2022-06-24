using FlightService.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlightService.Service
{
    public class FlightRepository : IFlightRepository
    {
        private readonly AppDbContext context;
        public FlightRepository(AppDbContext _context)
        {
            context = _context;
        }

        public bool AddFlight(FlightEntity input)
        {
            var isExist = context.FlightRepository.Any(z => z.FromPlace == input.ToPlace && z.StartTime == input.StartTime && z.EndTime == z.EndTime);
            if (isExist)
                return false;

            context.FlightRepository.Add(input);
            return true;
        }

        public void UpdateFlight(FlightEntity input)
        {
            context.FlightRepository.Update(input);
        }

        public void DeleteFlight(int id)
        {
            var airline = context.FlightRepository.Find(id);
            context.FlightRepository.Remove(airline);
        }

        public FlightEntity GetFlightById(int Id)
        {
            return context.FlightRepository.Find(Id);
        }

        public List<FlightEntity> GetFlightbyLocation(string source, string destination)
        {
            return context.FlightRepository.Where(z => !z.IsBlock && (z.FromPlace.ToLower().Contains(source) || z.ToPlace.ToLower().Contains(destination))).ToList();
        }

        public List<FlightEntity> GetAll()
        {
            return context.FlightRepository.ToList();
        }

        public void SaveChanges()
        {
            context.SaveChanges();
        }

        public bool AllowToCancelTicket(int flightId)
        {
            var flight = context.FlightRepository.FirstOrDefault(z => z.Id == flightId);
            if(flight != null)
            {
                var startTime = Convert.ToDateTime(flight.StartTime).AddHours(-24);
                if (DateTime.Now <= startTime)
                {
                    return true;
                }
            }
            return false;
        }

        public void BlockUnblockFlight(int airlineId, bool isBlock)
        {
            var flightList = context.FlightRepository.Where(z => z.AirlineId == airlineId).ToList();
            flightList = flightList.Select(z =>
            {
                z.IsBlock = isBlock;
                return z;
            }).ToList();

            context.FlightRepository.UpdateRange(flightList);
            context.SaveChanges();
        }
    }
}
