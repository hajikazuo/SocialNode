using Microsoft.AspNetCore.Identity;
using SocialNode.Mvc.Context;
using SocialNode.Mvc.Models;
using System;
using System.Threading.Tasks;

namespace SocialNode.Mvc.Services
{
    public class SeedService : ISeedService, IDisposable
    {
        private readonly UserManager<User> _userManager;
        private readonly INeo4jService _neo4jService;
        private readonly SocialNodeContext _context;

        public SeedService(UserManager<User> userManager, INeo4jService neo4jService, SocialNodeContext context)
        {
            _userManager = userManager;
            _neo4jService = neo4jService;
            _context = context;
        }

        public void Seed()
        {
            CreateUsers().GetAwaiter().GetResult();
        }

        private async Task CreateUsers()
        {
            string[] names = {
                "Alice Johnson", "Bob Smith", "Charlie Brown", "Diana Prince", "Eve Adams",
                "Frank Castle", "Grace Hopper", "Hank Pym", "Ivy Williams", "Jack Reacher"
            };

            foreach (var name in names)
            {
                string email = name.Replace(" ", ".").ToLower() + "@gmail.com";
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    user = new User
                    {
                        UserName = email,
                        Email = email,
                        CompleteName = name
                    };

                    var result = await _userManager.CreateAsync(user, "Test@123");
                    if (result.Succeeded)
                    {
                        await _neo4jService.CreateUserAsync(user.Id, user.CompleteName);
                    }
                }
            }

            var allUsers = _userManager.Users.ToList();
            var rand = new Random();

            for (int i = 0; i < allUsers.Count; i++)
            {
                for (int j = i + 1; j < allUsers.Count; j++)
                {
                    if (rand.NextDouble() < 0.3)
                    {
                        await _neo4jService.CreateFriendshipAsync(allUsers[i].Id, allUsers[j].Id);
                    }
                }
            }
        }
        public void Dispose()
        {
            GC.Collect();
        }
    }
}
