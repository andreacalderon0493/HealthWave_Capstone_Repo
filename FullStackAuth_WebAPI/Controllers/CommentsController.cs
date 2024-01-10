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
    public class CommentsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CommentsController(ApplicationDbContext context)
        {
            _context = context;
        }


    

     

        // POST api/Conments/comments/{id}
        [HttpPost("comments/{postId}"), Authorize]
        public IActionResult CommentPost([FromBody] Comment comment, int postId)
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


                // Set the foreign key properties in the Comment model
                comment.PostId = postId;
                comment.UserId = userId;


                // Add the new comment to the database
                _context.Comments.Add(comment);
                _context.SaveChanges();

                return Ok(comment);
            }
            catch (Exception ex)
            {
                // If an error occurs, return a 500 internal server error with the error message
                return StatusCode(500, ex.Message);
            }
        }
        // GET api/Comments/posts/{postId}/comments
        [HttpGet("posts/{postId}/comments")]
        public IActionResult GetCommentsForPost(int postId)
        {
            var comments = _context.Comments.Where(c => c.PostId == postId).ToList();
            if (comments == null)
            {
                return NotFound();
            }

            return Ok(comments);
        }


        // PUT api/Comments/comments/{id}
        [HttpPut("comments/{id}")]
        public IActionResult UpdateComment(int id, [FromBody] Comment updatedComment)
        {
            var comment = _context.Comments.Find(id);
            if (comment == null)
            {
                return NotFound();
            }

            // Update the comment
            comment.Text = updatedComment.Text;
            // Update any other fields as necessary...

            _context.SaveChanges();

            return NoContent();
        }

        // DELETE api/Comments/comments/{id}
        [HttpDelete("comments/{id}")]
        public IActionResult DeleteComment(int id)
        {
            var comment = _context.Comments.Find(id);
            if (comment == null)
            {
                return NotFound();
            }

            _context.Comments.Remove(comment);
            _context.SaveChanges();

            return NoContent();
        }
    
    }
}



