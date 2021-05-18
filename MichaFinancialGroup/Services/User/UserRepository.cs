using MichaFinancialGroup.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MichaFinancialGroup.Services
{
    public class UserRepository : IUserRepository
    {
        private readonly BankAppDataContext _dbContext;
        public UserRepository(BankAppDataContext dbContext)
        {
            _dbContext = dbContext;
        }
        public IEnumerable<IdentityUser> GetUsers()
        {
            return _dbContext.Users;
        }


    }
}
