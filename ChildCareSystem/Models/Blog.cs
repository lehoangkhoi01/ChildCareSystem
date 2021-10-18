using ChildCareSystem.Areas.Identity.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ChildCareSystem.Models
{
    public class Blog
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string Image { get; set; }
        
        [ForeignKey("Id")]
        public int BlogCategoryId { get; set; }
        public BlogCategory BlogCategory { get; set; }
      
        public string AuthorId { get; set; }
        [ForeignKey("AuthorId")]
        public ChildCareSystemUser ChildCareSystemUser { get; set; }
    }
}
