using System;
using System.ComponentModel.DataAnnotations;

namespace FullStackAuth_WebAPI.Models
{
	public class Messages
	{
        [Key]
        public int Id { get; set; }
        public string Sender { get; set; }
        public string Receiver { get; set; }
        public string Text { get; set; }
        public DateTime Time { get; set; }

    }
}

