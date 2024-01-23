using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FullStackAuth_WebAPI.Models
{
	public class Following
	{
        [Key]

        public int Id { get; set; }
        public string Status { get; set; }

        [ForeignKey("DoingFollowing")]
        public string DoingFollowingId { get; set; }
        public User DoingFollowing { get; set; }

        [ForeignKey("ReceivingFollowing")]
        public string ReceivingFollowingId { get; set; }
        public User ReceivingFollowing { get; set; }
        
    }
}

