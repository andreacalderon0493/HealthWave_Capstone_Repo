using FullStackAuth_WebAPI.Data;
using FullStackAuth_WebAPI.DataTransferObjects;
using FullStackAuth_WebAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace FullStackAuth_WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LikesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public LikesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // POST api/Likes/Likes/{id}
        [HttpPost("likes/{postId}"), Authorize]
        public IActionResult FavoritePost(int postId)
        {

            try
            {
                // Retrieve the authenticated user's ID from the JWT token
                string userId = User.FindFirstValue("id");

                // If the user ID is null or empty, the user is not authenticated, so return a 401 unauthorized response
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }
                // Check if the post exists
                var postExists = _context.Posts.Any(p => p.Id == postId);
                if (!postExists)
                {
                    return NotFound();
                }

                // Check if the user has already liked this post
                var alreadyLiked = _context.Likes.Any(f => f.UserId == userId && f.PostId == postId);
                if (alreadyLiked)
                {
                    return BadRequest("This post is already liked.");
                }

                // Create a new favorite
                var like = new Like
                {
                    UserId = userId,
                    PostId = postId
                };

                // Add the new favorite to the database
                _context.Likes.Add(like);
                _context.SaveChanges();

                return Ok("Post Liked successfully!");
            }
            catch (Exception ex)
            {
                // If an error occurs, return a 500 internal server error with the error message
                return StatusCode(500, ex.Message);
            }
        }
    }
}

