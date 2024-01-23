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
    public class FollowingsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public FollowingsController(ApplicationDbContext context)
        {
            _context = context;
        }


        // GET: api/followings/myFollowings
        [HttpGet("myFollowings"), Authorize]
        public IActionResult GetUsersFollowings()
        { 
            try
            {
                // Retrieve the authenticated user's ID from the JWT token
                string userId = User.FindFirstValue("id"); //id of logged in user


                // Retrieve all followings that belong to the authenticated user, including the owner object
                var following = _context.Followings.Where(f => f.ReceivingFollowingId == userId)
                       .Include(f => f.DoingFollowing)
                    .Select(f => new UserForDisplayDto()
                    {
                        Id = f.Id.ToString(),
                        UserName = f.DoingFollowing.UserName,
                        FirstName = f.DoingFollowing.FirstName,
                        LastName = f.DoingFollowing.LastName
                    })
                    .ToList();

                // Return the list of followings as a 200 OK response
                return StatusCode(200, following);
            } 
            catch (Exception ex)
            {
                // If an error occurs, return a 500 internal server error with the error message
                return StatusCode(500, ex.Message);
            }
        }

        // GET api/followings/5
        [HttpGet("{id}")]
        public IActionResult Get(string id)
        {
            try
            {

                // Retrieve the following with the specified ID, including the owner object
                var user = _context.Users.FirstOrDefault(u => u.Id == id);
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

        // POST api/followings/{id}
        [HttpPost("{acceptingId}"), Authorize]
        public IActionResult Post(string acceptingId)
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
                var newFollow = new Following
                {
                    Status = "Following",
                    DoingFollowingId = userId,
                    ReceivingFollowingId = acceptingId
                };
                _context.Followings.Add(newFollow);
                _context.SaveChanges();
                
                return StatusCode(201, newFollow);
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
                Following followings = _context.Followings.FirstOrDefault(f => f.Id == id);
                if (followings == null)
                {
                    // Return a 404 Not Found error if the following with the specified ID does not exist
                    return NotFound();
                }

                // Check if the authenticated user is the owner of the following
                var userId = User.FindFirstValue("id");
                if (string.IsNullOrEmpty(userId) || followings.DoingFollowingId != userId)
                {
                    // Return a 401 Unauthorized error if the authenticated user is not the owner of the car
                    return Unauthorized();
                }

                // Remove the car from the database
                _context.Followings.Remove(followings);
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
