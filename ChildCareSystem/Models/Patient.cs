using ChildCareSystem.Areas.Identity.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ChildCareSystem.Models
{
    public class Patient
    {
        public int Id { get; set; }
        public string PatientName { get; set; }
        public bool Gender { get; set; }
        public DateTime Birthdate { get; set; }

        public string CustomerId { get; set; }
        [ForeignKey("CustomerId")]
        public ChildCareSystemUser ChildCareSystemUser { get; set; }

        [ForeignKey("Id")]
        public int StatusId { get; set; }
        public Status Status { get; set; }
    }
}
