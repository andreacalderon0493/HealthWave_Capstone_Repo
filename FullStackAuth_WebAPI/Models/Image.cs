using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FullStackAuth_WebAPI.Models
{
	public class Image
	{
            [Key]
            public int Id { get; set; }
            public string Title { get; set; }
            
            
            [NotMapped]
            public IFormFile ImageFile { get; set; }
            [NotMapped]
            public string ImageSrc { get; set; }
        
    }
}

