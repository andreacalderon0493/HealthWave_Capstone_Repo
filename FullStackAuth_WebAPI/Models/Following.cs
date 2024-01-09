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

        [ForeignKey("Follower")]

        public string FollowerId { get; set; }
        public User UserIsFollower { get; set; }

        [ForeignKey("Following")]

        public string FollowingId { get; set; }
        public User UserIsFollowing { get; set; }


    }
}

