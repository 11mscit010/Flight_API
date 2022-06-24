using DiscountService.Entity;
using DiscountService.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiscountService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DiscountController : ControllerBase
    {
        private readonly IDiscountRepository discountRepository;
        private readonly AppDbContext context;
        public DiscountController(AppDbContext _context)
        {
            context = _context;
            discountRepository = new DiscountRepository(context);
        }

        [HttpPost("Add")]
        public ActionResult<string> Add(DiscountCouponEnity input)
        {
            var pre = input.CouponName.Substring(0, 2).ToUpper();
            input.CouponCode = pre + input.DiscountPrice;
            var result = discountRepository.AddDiscountCoupon(input);
            if (!result)
                return BadRequest("Add Discount Coupon Failed");

            discountRepository.SaveChanges();
            return Ok("Discount Coupon Added Successfully");
        }

        [HttpDelete("Delete")]
        public ActionResult<string> Delete(int Id)
        {
            discountRepository.DeleteDiscountCoupon(Id);
            discountRepository.SaveChanges();
            return Ok("Discount Coupon Deleted Successfully");
        }

        [HttpGet("GetAll")]
        public ActionResult<string> GetAll()
        {
            var list = discountRepository.GetAllCoupons();
            var result = JsonConvert.SerializeObject(list);
            return Ok(result);
        }

        [HttpGet("GetByCode")]
        public ActionResult<string> GetByCode(string code)
        {
            var list = discountRepository.GetCouponsbyCode(code);
            var result = JsonConvert.SerializeObject(list);
            return Ok(result);
        }

        [HttpGet("GetById")]
        public ActionResult<string> GetById(int id)
        {
            var list = discountRepository.GetCouponsbyId(id);
            var result = JsonConvert.SerializeObject(list);
            return Ok(result);
        }

        [HttpGet("Find")]
        public ActionResult<string> Find(string name)
        {
            var list = discountRepository.GetCouponsbyName(name);
            var result = JsonConvert.SerializeObject(list);
            return Ok(result);
        }
    }
}
