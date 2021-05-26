using MFGApi.ViewModels;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SharedLibrary.data;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace MFGApi.Controllers
{
    [ApiController]
    [Route("api/")]
    public class MeController : ControllerBase
    {

        private readonly BankAppDataContext _context;
        private readonly IConfiguration _config;
        public MeController(BankAppDataContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }


        [Route("me")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet]
        public ActionResult<MeApiViewModel> Me()
        {
            var id = User.Claims.First(i => i.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier").Value;

            var query = _context.Customers.Include(d => d.Dispositions).ThenInclude(a => a.Account).FirstOrDefault(c => c.CustomerId == int.Parse(id));
            var viewModel = new MeApiViewModel
            {
                CustomerId = query.CustomerId,
                NationalId = query.NationalId,
                Birthday = (DateTime)query.Birthday,
                Country = query.Country,
                Telephonecountrycode = query.Telephonecountrycode,
                Telephonenumber = query.Telephonenumber,
                Surname = query.Surname,
                Emailaddress = query.Emailaddress,
                Gender = query.Gender,
                Zipcode = query.Zipcode,
                CountryCode = query.CountryCode,
                Streetaddress = query.Streetaddress,
                Givenname = query.Givenname,
                City = query.City,
                Dispositions = query.Dispositions.Select(a => new MeApiViewModel.DispositionsViewModel
                {
                    Account = new MeApiViewModel.AccountsViewModel
                    {
                        AccountId = a.AccountId,
                        Type = a.Type,
                        Balance = a.Account.Balance,
                        Created = a.Account.Created,
                        Frequency = a.Account.Frequency
                    }

                }).ToList()

            };

            viewModel.TotalBalance = query.Dispositions.Select(c => c.Account.Balance).Sum();

            return Ok(viewModel);
        }

        [HttpPost]
        [Route("GenerateJWTToken")]
        public ActionResult<string> Get([FromBody] CustomerApiViewModel login)
        {
            var tokenString = GenerateJSONWebToken(login);
            return Ok(new { token = tokenString });
        }

        private string GenerateJSONWebToken(CustomerApiViewModel customerViewInfo)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"])); 
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.NameId, customerViewInfo.Id.ToString()), 
                new Claim(JwtRegisteredClaimNames.Email, customerViewInfo.Email), 
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            var token = new JwtSecurityToken(_config["Jwt:Issuer"], _config["Jwt:Issuer"],
                claims,
                expires: DateTime.Now.AddMinutes(120),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
