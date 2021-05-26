using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace MichaFinancialGroup.Services
{
    public interface IUserRepository
    {
        public IEnumerable<IdentityUser> GetUsers();
    }
}
