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

        //DELETE api/comments/5
        [HttpDelete("{id}"), Authorize]
        public IActionResult Delete(int id)
        {
            try
            {
                // Find the comment to be deleted
                var comments = _context.Comments.FirstOrDefault(c => c.Id == id);
                if (comments == null)
                {
                    // Return a 404 Not Found error if the following with the specified ID does not exist
                    return NotFound();
                }

                // Check if the authenticated user is the owner of the following
                var userId = User.FindFirstValue("id");
                if (string.IsNullOrEmpty(userId) || comments.UserId != userId)
                {
                    // Return a 401 Unauthorized error if the authenticated user is not the owner of the car
                    return Unauthorized();
                }

                // Remove the comment from the database
                _context.Comments.Remove(comments);
                _context.SaveChanges();

                // Return a 204 No Content status code
                return StatusCode(204);
            }
            catch (Exception ex)
            {
                // Return a 500 Internal Server Error with the error message if an exception occurs
                return StatusCode(500, ex.Message);
            }
        }
    }
}



