using System;
using System.Linq;

namespace FullStackAuth_WebAPI.DataTransferObjects
{
	public class SharedPostForDisplayDto
	{
      
            public int Id { get; set; }
            public int PostId { get; set; }
            public PostForDisplayDto Post { get; set; }
            public string UserId { get; set; }
            public UserForDisplayDto User { get; set; }
        
    }
}

