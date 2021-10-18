using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ChildCareSystem.ViewModels
{
    public class ServiceViewModel
    {
        [Display(Name = "Service ID")]
        public int Id { get; set; }

        [Display(Name = "Service Name")]
        [Required]
        public string ServiceName { get; set; }

        [Display(Name = "Description")]
        [Required]
        public string Description { get; set; }

        [Display(Name = "Specialty")]
        public int SpecialtyId { get; set; }

        [Required]
        [Range(0, Double.MaxValue, ErrorMessage = "Price must not be less than 0")]
        public double Price { get; set; }

        [Display(Name = "Image")]
        [Required]
        public string ImageLink { get; set; }

        public string ImageName { get; set; }
    }
}
