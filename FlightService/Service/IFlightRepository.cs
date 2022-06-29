using FlightService.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlightService.Service
{
    interface IFlightRepository
    {
        bool AddFlight(FlightEntity input);
        void UpdateFlight(FlightEntity input);
        void DeleteFlight(int id);
        FlightEntity GetFlightById(int Id);
        List<FlightEntity> GetFlightbyLocation(string source, string destination);
        List<FlightEntity> GetAll();
        void SaveChanges();
        bool AllowToCancelTicket(int flightId);
        void BlockUnblockFlight(int airlineId, bool isBlock);
        List<FlightEntity> GetListByIds(List<string> Ids);
    }
}
