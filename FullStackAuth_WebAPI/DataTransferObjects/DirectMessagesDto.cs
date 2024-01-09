using System;
namespace FullStackAuth_WebAPI.DataTransferObjects
{
	public class DirectMessagesDto
	{
        public string Id { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Text { get; set; }
        public DateTime Time { get; set; }

    }
}

