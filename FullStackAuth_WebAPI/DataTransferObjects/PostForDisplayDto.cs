using System;
using System.Linq;

namespace FullStackAuth_WebAPI.DataTransferObjects
{
	public class PostForDisplayDto
	{
		public string Id { get; set; }
		public string Text { get; set; }
		public UserForDisplayDto User { get; set; }
	}
}

