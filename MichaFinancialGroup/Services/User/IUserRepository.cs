using MichaFinancialGroup.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MichaFinancialGroup.Services
{
    public interface IUserRepository
    {
        public IEnumerable<IdentityUser> GetUsers();
    }
}
