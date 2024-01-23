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
    public class PostsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PostsController(ApplicationDbContext context)
        {
            _context = context;
        }




        // GET: api/posts
        [HttpGet]
        public IActionResult GetAllPosts()
        {
            try
            {
                // Retrieve all posts from the database, using Dtos
                var posts = _context.Posts
                    .GroupJoin(_context.Likes,
                              post => post.Id,
                              like => like.PostId,
                              (post, likes) => new
                              {
                                  Post = post,
                                  Likes = likes
                              })
                    .Select(result => new PostForDisplayDto
                    {
                        Id = result.Post.Id.ToString(),
                        Text = result.Post.Text,
                        Title = result.Post.Title,
                        User = new UserForDisplayDto
                        {
                            Id = result.Post.User.Id,
                            FirstName = result.Post.User.FirstName,
                            LastName = result.Post.User.LastName,
                            UserName = result.Post.User.UserName,
                        },
                        Like = result.Likes.Count() // Count Likes
                    }).ToList();

                // Return the list of posts as a 200 OK response
                return StatusCode(200, posts);
            }
            catch (Exception ex)
            {
                // If an error occurs, return a 500 internal server error with the error message
                return StatusCode(500, ex.Message);
            }
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
                            .GroupJoin(_context.Likes,
                              post => post.Id,
                              like => like.PostId,
                              (post, likes) => new
                              {
                                  Post = post,
                                  Likes = likes
                              })
                    .Select(result => new PostForDisplayDto
                    {
                        Id = result.Post.Id.ToString(),
                        Text = result.Post.Text,
                        Title= result.Post.Title,
                        User = new UserForDisplayDto
                        {
                            Id = result.Post.User.Id,
                            FirstName = result.Post.User.FirstName,
                            LastName = result.Post.User.LastName,
                            UserName = result.Post.User.UserName,
                        },
                        Like = result.Likes.Count() // Count Likes
                    }).ToList();

                


                // Return the list of posts  as a 200 OK response
                return StatusCode(200, posts);
            }
            catch (Exception ex)
            {
                // If an error occurs, return a 500 internal server error with the error message
                return StatusCode(500, ex.Message);
            }
        }

        // GET: api/posts/AllPosts
        [HttpGet("AllPosts"), Authorize]
        public IActionResult GetUsersandSharedPosts()
        {

            try
            {
                // Retrieve the authenticated user's ID from the JWT token
                string userId = User.FindFirstValue("id");


                // Retrieve all posts that belong to the authenticated user, including the owner object
                var posts = _context.Posts.Where(p => p.UserId == userId)
                              .Include(p => p.User)
                              .Select(p => new PostForDisplayDto()
                              {
                                  Id = p.Id.ToString(),
                                  Text = p.Text,
                                  Title= p.Title,
                                  User = new UserForDisplayDto()
                                  {
                                      Id = p.User.Id.ToString(),
                                      UserName = p.User.UserName,
                                      FirstName = p.User.FirstName,
                                      LastName = p.User.LastName,

                                  }

                              })
                            .ToList();

                var sharedPosts = _context.SharedPosts.Where(sp => sp.UserId == userId)
                            .Include(sp => sp.User)
                            .Select(sp => new SharedPostForDisplayDto()
                            {
                                Id = sp.Id,
                                Post = new PostForDisplayDto()
                                {
                                    Id = sp.Post.Id.ToString(),
                                    Text = sp.Post.Text,
                                    Title= sp.Post.Title,
                                    User = new UserForDisplayDto()
                                    {
                                        Id = sp.Post.User.Id.ToString(),
                                        UserName = sp.Post.User.UserName,
                                        FirstName = sp.Post.User.FirstName,
                                        LastName = sp.Post.User.LastName,
                                    }
                                }
                            })
                                .ToList();

                var allPostsForDisplay = new AllPostsForDisplayDto
                {
                    Posts = posts,
                    SharedPosts = sharedPosts
                };

                // Combine the lists of posts and shared posts
               

                // Return the list of posts and shared posts as a 200 OK response
                return StatusCode(200, allPostsForDisplay);
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
                var user = _context.Posts.FirstOrDefault(p => p.Id == id);
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
                var post = _context.Posts.FirstOrDefault(f => f.Id == id);
                if (post == null)
                {
                    // Return a 404 Not Found error if the following with the specified ID does not exist
                    return NotFound();
                }

                // Check if the authenticated user is the owner of the following
                var userId = User.FindFirstValue("id");
                if (string.IsNullOrEmpty(userId) || post.UserId != userId)
                {
                    // Return a 401 Unauthorized error if the authenticated user is not the owner of the car
                    return Unauthorized();
                }

                // Remove the favorite from the database
                _context.Posts.Remove(post);
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

