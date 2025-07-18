namespace SocialNode.Mvc.ViewModels
{
    public class UserViewModel
    {
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public bool IsFriend { get; set; } = false;
    }

}
