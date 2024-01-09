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
    public class FavoritesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public FavoritesController(ApplicationDbContext context)
        {
            _context = context;
        }


        // GET: api/followings/myFollowings
        [HttpGet("myFavorites"), Authorize]
        public IActionResult GetUsersFavorites()
        {

            try
            {
                // Retrieve the authenticated user's ID from the JWT token
                string userId = User.FindFirstValue("id");


                // Retrieve all followings that belong to the authenticated user, including the owner object
                var favorites = _context.Favorites.Where(f => f.UserId == userId)
                    .Select(f => new UserForDisplayDto()
                    {
                        Id = f.Id.ToString(),
                        UserName = f.User.UserName,
                        FirstName = f.User.FirstName,
                        LastName = f.User.LastName
                    })
                    .ToList();

                // Return the list of followings as a 200 OK response
                return StatusCode(200, favorites);
            }
            catch (Exception ex)
            {
                // If an error occurs, return a 500 internal server error with the error message
                return StatusCode(500, ex.Message);
            }
        }

        // GET api/favorites/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            try
            {

                // Retrieve the following with the specified ID, including the owner object
                var user = _context.Favorites.FirstOrDefault(f => f.Id == id);
                //var following = _context.Followings.Include(f => f.FollowerId).FirstOrDefault(c => c.Id == id);

                // If the following does not exist, return a 404 not found response
                if (user == null)
                {
                    return NotFound();
                }

                // Return the following as a 200 OK response
                return StatusCode(200, user);
            }
            catch (Exception ex)
            {
                // If an error occurs, return a 500 internal server error with the error message
                return StatusCode(500, ex.Message);
            }
        }

        // POST api/Favorite/{id}
        [HttpPost("favorite/{postId}"), Authorize]
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

                // Check if the user has already favorited this post
                var alreadyFavorited = _context.Favorites.Any(f => f.UserId == userId && f.PostId == postId);
                if (alreadyFavorited)
                {
                    return BadRequest("This post is already favorited.");
                }

                // Create a new favorite
                var favorite = new Favorite
                {
                    UserId = userId,
                    PostId = postId
                };

                // Add the new favorite to the database
                _context.Favorites.Add(favorite);
                _context.SaveChanges();

                return Ok("Post favorited successfully!");
            }
            catch (Exception ex)
            {
                // If an error occurs, return a 500 internal server error with the error message
                return StatusCode(500, ex.Message);
            }
        }

        //DELETE api/followings/5
        [HttpDelete("{id}"), Authorize]
        public IActionResult Delete(int id)
        {
            try
            {
                // Find the car to be deleted
                var favorites = _context.Favorites.FirstOrDefault(f => f.Id == id);
                if (favorites == null)
                {
                    // Return a 404 Not Found error if the following with the specified ID does not exist
                    return NotFound();
                }

                // Check if the authenticated user is the owner of the following
                var userId = User.FindFirstValue("id");
                if (string.IsNullOrEmpty(userId) || favorites.UserId != userId)
                {
                    // Return a 401 Unauthorized error if the authenticated user is not the owner of the car
                    return Unauthorized();
                }

                // Remove the favorite from the database
                _context.Favorites.Remove(favorites);
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


