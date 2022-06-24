using AirlineService.Entity;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirlineService.Service
{
    public class AirlineRepository : IAirlineRepository
    {
        private readonly AppDbContext context;
        public AirlineRepository(AppDbContext _context)
        {
            context = _context;
        }

        public bool AddAirline(AirlineEntity input)
        {
            var isExist = context.AirlineRepository.Any(z => z.Name == input.Name);
            if (isExist)
                return false;
            
            context.AirlineRepository.Add(input);
            return true;
        }

        public void UpdateAirline(AirlineEntity input)
        {
            context.AirlineRepository.Update(input);
        }

        public void DeleteAirline(int id)
        {
            var airline = context.AirlineRepository.Find(id);
            context.AirlineRepository.Remove(airline);
        }

        public List<AirlineEntity> GetAllAirline()
        {
            return context.AirlineRepository.ToList();
        }

        public AirlineEntity GetByID(int Id)
        {
            return context.AirlineRepository.FirstOrDefault(z=>z.Id == Id);
        }

        public void BlockUnBlockAirline(int id)
        {
            var airline = context.AirlineRepository.Find(id);
            airline.IsBlock = !airline.IsBlock;
            context.AirlineRepository.Update(airline);
            //Initiate rabbitMQ queue to notify related flights --START
            var data = new List<KeyValuePair<string, string>>();
            data.Add(new KeyValuePair<string, string>("AirlineId", airline.Id.ToString()));
            data.Add(new KeyValuePair<string, string>("IsBlock", airline.IsBlock.ToString()));
            var output = JsonConvert.SerializeObject(data);
            AirlineProducer.QueueProducer("Airline-Q1", output);
            //--END
        }

        public void SaveChanges()
        {
            context.SaveChanges();
        }

        public List<AirlineEntity> GetAirlineByName(string name)
        {
            var list = context.AirlineRepository.Where(z => z.Name.ToLower().Contains(name.ToLower())).ToList();
            return list;
        }
    }
}
