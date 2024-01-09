﻿using FullStackAuth_WebAPI.Data;
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
    public class PostsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PostsController(ApplicationDbContext context)
        {
            _context = context;
        }


        // GET: api/posts/myPosts
        [HttpGet("myPosts"), Authorize]
        public IActionResult GetUsersPosts()
        {

            try
            {
                // Retrieve the authenticated user's ID from the JWT token
                string userId = User.FindFirstValue("id");


                // Retrieve all posts that belong to the authenticated user, including the owner object
                var posts = _context.Posts.Where(p => p.UserId == userId)
                    .Select(p => new PostForDisplayDto()
                    {
                        Id = p.Id.ToString(),
                        Text = p.Text,
                        User = new UserForDisplayDto()
                        {
                            Id = p.User.Id.ToString(),
                            UserName = p.User.UserName,
                            FirstName = p.User.FirstName,
                            LastName = p.User.LastName,

                        }
                        
                    })
                    .ToList();

                // Return the list of followings as a 200 OK response
                return StatusCode(200, posts);
            }
            catch (Exception ex)
            {
                // If an error occurs, return a 500 internal server error with the error message
                return StatusCode(500, ex.Message);
            }
        }

        // GET api/posts/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            try
            {

                // Retrieve the following with the specified ID, including the owner object
                var user = _context.Posts.FirstOrDefault(f => f.Id == id);
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

        // POST api/Posts/posts
        [HttpPost("posts"), Authorize]
        public IActionResult Post([FromBody] Post post)
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

                // Set the UserId property of the post to the authenticated user's ID
                post.UserId = userId;


                _context.Posts.Add(post);
                _context.SaveChanges();


                return Ok(post);
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
