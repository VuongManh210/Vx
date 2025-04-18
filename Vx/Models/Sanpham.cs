using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Vx.Models
{
    public class SanPham
    {
        [Key]
        public int ID { get; set; }
        public string TenSanPham { get; set; }
        public decimal Gia { get; set; }
        public string HinhAnh { get; set; }
    }
}