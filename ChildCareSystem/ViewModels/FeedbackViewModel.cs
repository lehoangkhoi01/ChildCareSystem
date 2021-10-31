using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ChildCareSystem.ViewModels
{
    public class FeedbackViewModel
    {
        public int Id { get; set; }
        public int Rate { get; set; }

        [Required (ErrorMessage = "You should leave a comment about our service.")]
        public string Comment { get; set; }
        public int ServiceId { get; set; }
        public int ReservationId { get; set; }

    }
}
