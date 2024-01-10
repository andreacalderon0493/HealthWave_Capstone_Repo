using System;
using FullStackAuth_WebAPI.Models;

namespace FullStackAuth_WebAPI.DataTransferObjects
{
	public class FavoritesForDisplayDto
	{
        public string Id { get; set; }
        public Post Post { get; set; }
        public UserForDisplayDto User { get; set; }
    }
}

