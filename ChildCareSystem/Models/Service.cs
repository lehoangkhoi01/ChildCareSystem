using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ChildCareSystem.Models
{
    public class Service
    {
        public int Id { get; set; }
        public string ServiceName { get; set; }
        public string ThumbnailLink { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }

        [ForeignKey("Id")]
        public int SpecialtyId { get; set; }
        public Specialty Specialty { get; set; }
    }
}
