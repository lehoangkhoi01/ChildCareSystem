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
        public string imageLink { get; set; }
        
        [ForeignKey("CategoryId")]
        public BlogCategory BlogCategory { get; set; }

        [ForeignKey("AuthorId")]
        public ChildCareSystemUser ChildCareSystemUser { get; set; }
    }
}
