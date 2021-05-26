using MichaFinancialGroup.Models;
using MichaFinancialGroup.Services;
using MichaFinancialGroup.ViewModels.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SharedLibrary.data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MichaFinancialGroup.Controllers
{
    public class UserController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IUserRepository _userRepository;
        private readonly BankAppDataContext _dbcontext;

        public UserController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, IUserRepository userRepository, BankAppDataContext dbcontext)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _userRepository = userRepository;
            _dbcontext = dbcontext;
        }
        [Authorize(Roles = "Admin, Cashier")]
        public IActionResult Index(string sorting)
        {
            var viewModel = new UserIndexViewModel();

            viewModel.Users = _userRepository.GetUsers()
                .Select(dbcat => new UserIndexViewModel.UserViewModel
                {
                    Id = dbcat.Id,
                    Username = dbcat.UserName
                }).ToList();

            return View(viewModel);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult New()
        {
            var viewModel = new UserNewViewModel();
            viewModel.AllRoles = GetRolesListItems();
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> New(UserNewViewModel user)
        {

            if (ModelState.IsValid)
            {

                IdentityUser identityUser = new IdentityUser
                {
                    Email = user.Username,
                    UserName = user.Username,
                    EmailConfirmed = true

                };

                var role = await _roleManager.FindByIdAsync(user.SelectedRoleId);
                IdentityResult result = await _userManager.CreateAsync(identityUser, user.NewPassword);
                if (user.SelectedRoleId != "0")
                {
                    await _userManager.AddToRoleAsync(identityUser, role.Name);
                }
                if (result.Succeeded)
                    return RedirectToAction("Index");
                else
                {
                    foreach (IdentityError error in result.Errors)
                        ModelState.AddModelError("", error.Description);
                }
            }
            user.AllRoles = GetRolesListItems();
            return View(user);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Edit(string Id)
        {
            var viewModel = new UserEditViewModel();

            var dbUser = _userManager.Users.First(r => r.Id == Id);

            viewModel.Id = dbUser.Id;
            viewModel.Username = dbUser.UserName;
            viewModel.AllRoles = GetRolesListItems();
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(string Id, UserEditViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var iduser = await _userManager.FindByIdAsync(Id);
                var role = await _roleManager.FindByIdAsync(viewModel.SelectedRoleId);
                var currentRole = await _userManager.GetRolesAsync(iduser);

                var dbUser = _userRepository.GetUsers().FirstOrDefault(i => i.Id == Id);

                dbUser.UserName = viewModel.Username;
                dbUser.Email = viewModel.Username;
                if (viewModel.NewPassword == null)
                {
                    viewModel.ConfirmPassword = null;
                }
                if (viewModel.ConfirmPassword == null)
                {
                    viewModel.NewPassword = null;
                }
                if (viewModel.SelectedRoleId != "0")
                {
                    if (currentRole.Count != 0)
                    {
                        await _userManager.RemoveFromRoleAsync(iduser, currentRole.First());
                    }
                    await _userManager.AddToRoleAsync(iduser, role.Name);
                }
                if (viewModel.NewPassword != null && viewModel.ConfirmPassword != null)
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(iduser);
                    var result = await _userManager.ResetPasswordAsync(iduser, token, viewModel.NewPassword);
                    if (!result.Succeeded)
                    {
                        ModelState.AddModelError("NewPassword", "Password must contain atleast 1 of each; Uppercase, number and a symbol");
                        viewModel.AllRoles = GetRolesListItems();
                        return View(viewModel);
                    }
                }
                _dbcontext.SaveChanges();
                await _userManager.UpdateAsync(iduser);
                return RedirectToAction("Index");
            }
            viewModel.AllRoles = GetRolesListItems();
            return View(viewModel);

        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(string id)
        {
            IdentityUser user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                IdentityResult result = await _userManager.DeleteAsync(user);
                if (result.Succeeded)
                    return RedirectToAction("Index");
                else
                    return RedirectToAction("Delete");

            }
            else
                ModelState.AddModelError("", "User Not Found");
            return View("Index", _userManager.Users);
        }

        private List<SelectListItem> GetRolesListItems()
        {
            var list = new List<SelectListItem>();
            list.Add(new SelectListItem { Value = "0", Text = "..please select..." });

            list.AddRange(_roleManager.Roles.Select(r => new SelectListItem
            {
                Text = r.Name,
                Value = r.Id.ToString()
            }));
            return list;
        }
    }

}

