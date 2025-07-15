namespace SocialNode.Mvc.Services
{
    public interface INeo4jService
    {
        Task CreateUserAsync(Guid userId, string fullName);
        Task CreateFriendshipAsync(Guid userId1, Guid userId2);
        Task<List<Guid>> GetFriendsAsync(Guid userId);
        Task<List<Guid>> GetFriendSuggestionsAsync(Guid userId);
    }
}
