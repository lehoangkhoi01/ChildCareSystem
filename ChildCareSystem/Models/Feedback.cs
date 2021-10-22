using ChildCareSystem.Areas.Identity.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ChildCareSystem.Models
{
    public class Feedback
    {
        public int Id { get; set; }
        public int Rate { get; set; }
        public string Comment { get; set; }

        [ForeignKey("Id")]
        public int ServiceId { get; set; }
        public Service Service { get; set; }

        [ForeignKey("Id")]
        public int ReservationId { get; set; }
        public Reservation Reservation { get; set; }

        public string CustomerId { get; set; }

        [ForeignKey("CustomerId")]
        public ChildCareSystemUser ChildCareSystemUser { get; set; }
    }
}
