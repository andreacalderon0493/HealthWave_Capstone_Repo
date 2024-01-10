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

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FullStackAuth_WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SharedPostsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SharedPostsController(ApplicationDbContext context)
        {
            _context = context;
        }



        // POST api/sharedPosts/sharedPosts/{id}
        [HttpPost("sharedPosts/{postId}"), Authorize]
        public IActionResult CommentPost( int postId)
        {
            SharedPost sharedPost = new SharedPost();
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


                // Set the foreign key properties in the Post model
                sharedPost.PostId = postId;
                sharedPost.UserId = userId;


                // Add the new Post to the database
                _context.SharedPosts.Add(sharedPost);
                _context.SaveChanges();

                return Ok(sharedPost);
            }
            catch (Exception ex)
            {
                // If an error occurs, return a 500 internal server error with the error message
                return StatusCode(500, ex.Message);
            }
        }
    }


}
