using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Models
{
    public class Offer
    {
        public Guid Id { get; set; }

        [MaxLength(10)]
        public string Code { get; set; }
        public double Discount { get; set; }
        public virtual Ticket Ticket { get; set; }

    }
}
