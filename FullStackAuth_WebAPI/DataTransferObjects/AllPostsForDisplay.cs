using System;
namespace FullStackAuth_WebAPI.DataTransferObjects
{
    public class AllPostsForDisplayDto
    {
        public List<PostForDisplayDto> Posts { get; set; }
        public List<SharedPostForDisplayDto> SharedPosts { get; set; }
    }
}

