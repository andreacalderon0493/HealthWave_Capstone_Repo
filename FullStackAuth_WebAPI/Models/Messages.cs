using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FullStackAuth_WebAPI.Models
{
	public class Messages
	{
        [Key]
        public int Id { get; set; }
        public string Text { get; set; }
        public DateTime Time { get; set; }

        [ForeignKey("Sender")]
        public string SenderId { get; set; }
        public User Sender { get; set; }

        [ForeignKey("Receiver")]
        public string ReceiverId { get; set; }
        public User Receiver { get; set; }


    }
}

