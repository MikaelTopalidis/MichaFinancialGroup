using System.Collections.Generic;

namespace MichaFinancialGroup.ViewModels.User
{
    public class UserIndexViewModel
    {
        public List<UserViewModel> Users { get; set; } = new List<UserViewModel>();
        public class UserViewModel
        {
            public string Id { get; set; }
            public string Username { get; set; }
        }
    }
}
