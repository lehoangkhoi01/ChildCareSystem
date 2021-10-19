using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ChildCareSystem.ViewModels
{
    public class PatientViewModel
    {
        [Required]
        public string PatientName { get; set; }
        [Required]
        public bool Gender { get; set; }
        public int Birthday { get; set; }
        public int Birthmonth { get; set; }
        public int Birthyear { get; set; }

    }
}
