using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FullStackAuth_WebAPI.Models
{
	public class Followings
	{
        [Key]

        public int Id { get; set; }
        public string Status { get; set; }

        [ForeignKey("Follower")]

        public string FollowerId { get; set; }
        public User Follower { get; set; }

        [ForeignKey("Following")]

        public string FollowingId { get; set; }
        public User Following { get; set; }


    }
}

