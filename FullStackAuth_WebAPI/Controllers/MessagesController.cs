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
    public class MessagesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public MessagesController(ApplicationDbContext context)
        {
            _context = context;
        }


        // GET: api/messages/myMessages
        [HttpGet("myMessages"), Authorize]
        public IActionResult GetUsersMessages()
        {

            try
            {
                // Retrieve the authenticated user's ID from the JWT token
                string userId = User.FindFirstValue("id");


                // Retrieve all followings that belong to the authenticated user, including the owner object
                var userMessages = _context.Messages
                    .Where(m => m.ReceiverId == userId || m.SenderId == userId)
                    .Select(m => new DirectMessagesDto()
                    {
                        Id = m.Id.ToString(),
                        UserName = m.Sender.UserName,
                        FirstName = m.Sender.FirstName,
                        LastName = m.Sender.LastName,
                        Text = m.Text,
                        Time = m.Time

                    })
                    .ToList();

                // Return the list of followings as a 200 OK response
                return StatusCode(200, userMessages);
            }
            catch (Exception ex)
            {
                // If an error occurs, return a 500 internal server error with the error message
                return StatusCode(500, ex.Message);
            }
        }

        //query to find all messages where logged in user is sender or receiver
        //Create a list of all userIds that appear
        //Take out current user's id, leaving list of other users that have DMed
        //Query separately to find all DMs with each individual user

        // POST api/messages/{id}
        [HttpPost("{recievingId}"), Authorize]
        public IActionResult Post([FromBody]Message message ,string recievingId)
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
                
                message.SenderId = userId;
                message.ReceiverId = recievingId;
                message.Time = DateTime.Now;
                _context.Add(message);
                _context.SaveChanges();

                return StatusCode(201, message);
            }
            catch (Exception ex)
            {
                // If an error occurs, return a 500 internal server error with the error message
                return StatusCode(500, ex.Message);
            }
        }

        
    }
}

