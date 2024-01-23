using System;
using System.Linq;

namespace FullStackAuth_WebAPI.DataTransferObjects
{
	public class PostForDisplayDto
	{
		public string Id { get; set; }
		public string Text { get; set; }
        public int Like { get; set; }
        public string Title { get; set; }
        public UserForDisplayDto User { get; set; }
        

        //public string ImageSrc { get; set; }
    }
}

