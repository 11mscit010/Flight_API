using AirlineService.Entity;
using AirlineService.Model;
using AirlineService.Service;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AirlineService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AirlineController : ControllerBase
    {
        public IConfiguration Configuration { get; }
        private readonly IAirlineRepository airlineRepository;
        private readonly AppDbContext context;
        private IWebHostEnvironment env;
        public AirlineController(IConfiguration configuration, AppDbContext _context, IWebHostEnvironment _environment)
        {
            Configuration = configuration;
            context = _context;
            airlineRepository = new AirlineRepository(context);
            env = _environment;
        }

        [HttpPost("Add")]
        public ActionResult<string> Add(AirlineModel input)
        {
            var filePathName = "";
            //Logo
            if (!string.IsNullOrEmpty(input.FileAsBase64))
            {
                var uploadPath = Path.Combine(env.ContentRootPath, "Images");

                filePathName = uploadPath + "\\" + Path.GetFileNameWithoutExtension(input.Logo) + "-" +
                        DateTime.Now.ToString().Replace("/", "").Replace(":", "").Replace(" ", "") +
                        Path.GetExtension(input.Logo);
                
                if (input.FileAsBase64.Contains(","))
                {
                    input.FileAsBase64 = input.FileAsBase64.Substring(input.FileAsBase64.IndexOf(",") + 1);
                }
                var FileAsByteArray = Convert.FromBase64String(input.FileAsBase64);
                using (var fs = new FileStream(filePathName, FileMode.CreateNew))
                {
                    fs.Write(FileAsByteArray, 0, FileAsByteArray.Length);
                }
            }

            var input1 = new AirlineEntity();
            input1.Name = input.Name;
            if (!string.IsNullOrEmpty(filePathName))
            {
                input1.Logo = filePathName;
            }
            input1.ContactNumber = input.ContactNumber;
            input1.Address = input.Address;
            input1.IsBlock = false;

            var result = airlineRepository.AddAirline(input1);
            if (!result)
                return BadRequest("Add Airline Failed");

            airlineRepository.SaveChanges();
            return Ok("Airline Added Successfully");
        }

        [HttpPut("Update")]
        public ActionResult<string> Update(AirlineModel input)
        {
            var filePathName = "";
            //Logo
            if (!string.IsNullOrEmpty(input.FileAsBase64))
            {
                string wwwPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

                filePathName = wwwPath + Path.GetFileNameWithoutExtension(input.Logo) + "-" +
                        DateTime.Now.ToString().Replace("/", "").Replace(":", "").Replace(" ", "") +
                        Path.GetExtension(input.Logo);
                if (input.FileAsBase64.Contains(","))
                {
                    input.FileAsBase64 = input.FileAsBase64.Substring(input.FileAsBase64.IndexOf(",") + 1);
                }
                var FileAsByteArray = Convert.FromBase64String(input.FileAsBase64);
                using (var fs = new FileStream(filePathName, FileMode.CreateNew))
                {
                    fs.Write(FileAsByteArray, 0, FileAsByteArray.Length);
                }
            }

            var input1 = new AirlineEntity();
            input1.Name = input.Name;
            if (!string.IsNullOrEmpty(filePathName))
            {
                input1.Logo = filePathName;
            }
            input1.ContactNumber = input.ContactNumber;
            input1.Address = input.Address;
            input1.IsBlock = false;

            airlineRepository.UpdateAirline(input1);
            airlineRepository.SaveChanges();
            return Ok("Airline Updated Successfully");
        }

        [HttpGet("BlockAction")]
        public ActionResult<string> BlockAction(int Id)
        {
            airlineRepository.BlockUnBlockAirline(Id);
            airlineRepository.SaveChanges();
            return Ok("Airline Updated Successfully");
        }

        [HttpGet("GetAll")]
        public ActionResult<string> GetAll()
        {
            var list = airlineRepository.GetAllAirline();
            var result = JsonConvert.SerializeObject(list);
            return Ok(result);
        }


        [HttpGet("GetById")]
        public ActionResult<string> GetById(int Id)
        {
            var list = airlineRepository.GetByID(Id);
            var result = JsonConvert.SerializeObject(list);
            return Ok(result);
        }

        [HttpGet("Find")]
        public ActionResult<string> Find(string name)
        {
            var list = airlineRepository.GetAirlineByName(name);
            var result = JsonConvert.SerializeObject(list);
            return Ok(result);
        }
    }
}
