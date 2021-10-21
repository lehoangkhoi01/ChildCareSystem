using ChildCareSystem.Areas.Identity.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ChildCareSystem.Models
{
    public class Reservation
    {
        public int Id { get; set; }
        public double Price { get; set; }
        public DateTime CheckInDate { get; set; }

        [ForeignKey("Id")]
        public int ServiceId { get; set; }
        public Service Service { get; set; }

        [ForeignKey("Id")]
        public int PatientId { get; set; }
        public Patient Patient { get; set; }

        

        public string StaffAssignedId { get; set; }
        [ForeignKey("StaffAssignedId")]
        public ChildCareSystemUser ChildCareSystemStaff { get; set; }

        public string CustomerId { get; set; }

        [ForeignKey("CustomerId")]
        public ChildCareSystemUser ChildCareSystemUser { get; set; }

        

    }
}
