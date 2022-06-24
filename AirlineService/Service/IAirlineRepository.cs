using AirlineService.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AirlineService.Service
{
    interface IAirlineRepository
    {
        bool AddAirline(AirlineEntity input);
        void UpdateAirline(AirlineEntity input);
        void DeleteAirline(int id);
        List<AirlineEntity> GetAllAirline();
        AirlineEntity GetByID(int Id);
        void BlockUnBlockAirline(int id);
        void SaveChanges();
        List<AirlineEntity> GetAirlineByName(string name);
    }
}
