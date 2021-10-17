using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ChildCareSystem.ViewModels
{
    public class BlogViewModel
    {
        [Display(Name = "Blog ID")]
        public int Id { get; set; }

        [Display(Name = "Title")]
        [Required]
        public string Title { get; set; }

        [Display(Name = "Content")]
        [Required]
        public string Content { get; set; }

        [Display(Name = "Image")]
        [Required]
        public string imageLink { get; set; }

        public string imageName { get; set; }
    }
}
