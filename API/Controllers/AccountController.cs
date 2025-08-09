using System.Security.Claims;
using API.DTOs;
using API.Extensions;
using Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class AccountController(SignInManager<AppUser> signInManager) : BaseApiController
{
    [HttpPost("register")] // force to login after register
    public async Task<ActionResult> Register(RegisterDto registerDto)
    {
        var user = new AppUser
        {
            FirstName = registerDto.FirstName,
            LastName = registerDto.LastName,
            Email = registerDto.Email,
            UserName = registerDto.Email,
        };

        var result = await signInManager.UserManager.CreateAsync(user, registerDto.Password);

        // password validation errors (weak password)
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                // model state dictionary
                ModelState.AddModelError(error.Code, error.Description);
            }
            return ValidationProblem();
        }

        return Ok();
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<ActionResult> Logout()
    {
        await signInManager.SignOutAsync(); // includes removing cookie from browser

        return NoContent();
    }

    [HttpGet("user-info")]
    public async Task<ActionResult> GetUserInfo() 
    {
        if (User.Identity?.IsAuthenticated == false) return NoContent(); // so that in dev tools no errors were shown to users

        var user = await signInManager.UserManager.GetUserByEmailWithAddress(User); // extension method from ClaimsPrincipleExtensions

        // return anonymous object => no type
        return Ok(new
        {
            user.FirstName,
            user.LastName,
            user.Email,
            Address = user.Address?.ToDto(),
            // the role contained inside the cookie, which cannot access from client side
            Roles = User.FindFirstValue(ClaimTypes.Role) // returns roles user belongs to
        });
    }

    [HttpGet("auth-status")] // not async as not using anything from db
    public ActionResult GetAuthState()
    {
        return Ok(new { IsAuthenticated = User.Identity?.IsAuthenticated ?? false });
    }

    [Authorize]
    [HttpPost("address")]
    public async Task<ActionResult<Address>> CreateOrUpdateAddress(AddressDto addressDto)
    {
        // get a user + address
        var user = await signInManager.UserManager.GetUserByEmailWithAddress(User);

        if (user.Address == null)
        {
            user.Address = addressDto.ToEntity();
        }
        else
        {
            user.Address.UpdateFromDto(addressDto);
        }

        // update user with new/updated address
        var result = await signInManager.UserManager.UpdateAsync(user);

        if (!result.Succeeded) return BadRequest("Problem updating user address");

        return Ok(user.Address.ToDto());
    }
}
