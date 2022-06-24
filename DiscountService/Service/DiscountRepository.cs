using DiscountService.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiscountService.Service
{
    public class DiscountRepository : IDiscountRepository
    {
        private readonly AppDbContext context;
        public DiscountRepository(AppDbContext _context)
        {
            context = _context;
        }

        public bool AddDiscountCoupon(DiscountCouponEnity input)
        {
            var isExist = context.DiscountCouponRepository.Any(z => z.CouponName == input.CouponName);
            if (isExist)
                return false;

            context.DiscountCouponRepository.Add(input);
            return true;
        }

        public void DeleteDiscountCoupon(int id)
        {
            var airline = context.DiscountCouponRepository.Find(id);
            context.DiscountCouponRepository.Remove(airline);
        }

        public List<DiscountCouponEnity> GetAllCoupons()
        {
            return context.DiscountCouponRepository.ToList();
        }

        public DiscountCouponEnity GetCouponsbyCode(string code)
        {
            return context.DiscountCouponRepository.FirstOrDefault(z => z.CouponCode == code);
        }

        public DiscountCouponEnity GetCouponsbyId(int id)
        {
            return context.DiscountCouponRepository.FirstOrDefault(z => z.Id == id);
        }

        public List<DiscountCouponEnity> GetCouponsbyName(string name)
        {
            return context.DiscountCouponRepository.Where(z => z.CouponName.ToLower().Contains(name.ToLower())).ToList();
        }

        public void SaveChanges()
        {
            context.SaveChanges();
        }
    }
}
