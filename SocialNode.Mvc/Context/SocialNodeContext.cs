using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SocialNode.Mvc.Models;
using System;

namespace SocialNode.Mvc.Context
{
    public class SocialNodeContext : IdentityDbContext<User, Role, Guid>
    {
        public SocialNodeContext(DbContextOptions<SocialNodeContext> options)
            : base(options)
        {
        }
    }
}
