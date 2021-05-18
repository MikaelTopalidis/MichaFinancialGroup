using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MichaFinancialGroup.ViewModels.User
{
    public class UserIndexViewModel
    {
        public string q { get; set; }
        public List<UserViewModel> Users { get; set; } = new List<UserViewModel>();
        public string SortTable { get; set; }
        public class UserViewModel
        {
            public string Id { get; set; }
            public string Username { get; set; }
        }
    }
}
