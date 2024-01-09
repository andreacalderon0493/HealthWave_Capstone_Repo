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
                var messages = _context.Messages.Where(m => m.ReceiverId == userId)
                    .Select(m => new UserForDisplayDto()
                    {
                        //Id = m.Message.Id,
                        //UserName = m.Message.UserName,
                        //FirstName = m.Message.FirstName,
                        //LastName = m.Message.LastName
                    })
                    .ToList();

                // Return the list of followings as a 200 OK response
                return StatusCode(200, messages);
            }
            catch (Exception ex)
            {
                // If an error occurs, return a 500 internal server error with the error message
                return StatusCode(500, ex.Message);
            }
        }

        // POST api/messages/{id}
        [HttpPost("{acceptingId}"), Authorize]
        public IActionResult Post(string acceptingId)
        {
            Messages message = new Messages();
            try
            {
                // Retrieve the authenticated user's ID from the JWT token
                string userId = User.FindFirstValue("id");

                // If the user ID is null or empty, the user is not authenticated, so return a 401 unauthorized response
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }
                message.Text = "";
                message.SenderId = userId;
                message.ReceiverId = acceptingId;
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

