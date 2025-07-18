using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SocialNode.Mvc.Models;
using SocialNode.Mvc.Services;
using SocialNode.Mvc.ViewModels;

namespace SocialNode.Mvc.Controllers
{
    public class FriendsController : Controller
    {
        private readonly INeo4jService _neo4jService;
        private readonly UserManager<User> _userManager;

        public FriendsController(INeo4jService neo4jService, UserManager<User> userManager)
        {
            _neo4jService = neo4jService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Search([FromQuery] string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return View(new List<UserViewModel>());
            }

            var users = await _userManager.Users
                .Where(u => u.CompleteName.Contains(query))
                .ToListAsync();

            var currentUser = await _userManager.GetUserAsync(User);
            var friendIds = await _neo4jService.GetFriendsAsync(currentUser.Id);

            var model = users.Select(u => new UserViewModel
            {
                Id = u.Id,
                FullName = u.CompleteName,
                IsFriend = friendIds.Contains(u.Id)
            }).ToList();

            return View(model);
        }

        public async Task<IActionResult> Suggestions()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var suggestions = await _neo4jService.GetFriendSuggestionsAsync(currentUser.Id);

            var users = await _userManager.Users
                .Where(u => suggestions.Contains(u.Id))
                .Select(u => new UserViewModel
                {
                    Id = u.Id,
                    FullName = u.CompleteName
                }).ToListAsync();

            return View(users);
        }

        public async Task<IActionResult> FriendList(Guid id)
        {
            var friendIds = await _neo4jService.GetFriendsAsync(id);

            var friends = new List<UserViewModel>();

            foreach (var friendId in friendIds)
            {
                var user = await _userManager.FindByIdAsync(friendId.ToString());
                if (user != null)
                {
                    friends.Add(new UserViewModel
                    {
                        Id = user.Id,
                        FullName = user.CompleteName
                    });
                }
            }

            return View(friends);
        }

        public async Task<IActionResult> MyFriends()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var friendIds = await _neo4jService.GetFriendsAsync(currentUser.Id);

            var friends = new List<UserViewModel>();

            foreach (var friendId in friendIds)
            {
                var user = await _userManager.FindByIdAsync(friendId.ToString());
                if (user != null)
                {
                    friends.Add(new UserViewModel
                    {
                        Id = user.Id,
                        FullName = user.CompleteName
                    });
                }
            }

            return View(friends);
        }

        public async Task<IActionResult> Add(Guid id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            await _neo4jService.CreateFriendshipAsync(currentUser.Id, id);
            return RedirectToAction("Suggestions");
        }

        public async Task<IActionResult> Remove(Guid id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            await _neo4jService.RemoveFriendshipAsync(currentUser.Id, id);
            return RedirectToAction("MyFriends");
        }

    }
}
