using DiscountService.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiscountService.Service
{
    interface IDiscountRepository
    {
        bool AddDiscountCoupon(DiscountCouponEnity input);
        void DeleteDiscountCoupon(int id);
        List<DiscountCouponEnity> GetAllCoupons();
        DiscountCouponEnity GetCouponsbyCode(string code);
        DiscountCouponEnity GetCouponsbyId(int id);
        List<DiscountCouponEnity> GetCouponsbyName(string name);
        void SaveChanges();
    }
}
