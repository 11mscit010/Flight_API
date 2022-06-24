using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DiscountService.Entity
{
    [Table("DiscountCoupon")]
    public class DiscountCouponEnity
    {
        [Key]
        public int Id { get; set; }
        public string CouponName { get; set; }
        public int DiscountPrice { get; set; }
        public string CouponCode { get; set; }
    }
}
