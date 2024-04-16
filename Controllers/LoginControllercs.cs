using Dairy.API.Models.Login;
using Dairy.Application.Interface;
using Dairy.Domain;
using DairyAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Utility;

namespace Dairy.API.Controllers;

[Route("api/login/[controller]")]
[ApiController]
//[Authorize]
public class LoginControllercs : ControllerBase
{
    private readonly ILogger<LoginControllercs> _logger;
    private readonly IConfiguration _configuration;
    private readonly IDairyServices _dairyServices;

    private string? key { get;set; }

    public LoginControllercs(ILogger<LoginControllercs> logger, IConfiguration configuration, IDairyServices dairyServices)
    {
        _logger = logger;
        _configuration = configuration;
        _dairyServices = dairyServices;
        key = _configuration.GetValue<string>("EncryptDecrypt:Key");
    }

    [HttpPost("InsertCustomer")]
    public async Task<ActionResult<ActionResponse>> InsertCustomer(LoginModel request)
    {
        _logger.LogTrace($"Entered method {InsertCustomer}");

        try
        {
            if (request != null)
                return BadRequest("Access denied");


            Login login = new Login
            {
                Firstname = request?.Firstname,
                Lastname = request?.Lastname,
                Address = request?.Address,
                Email = request?.Email, 
                Password = new PasswordManager().EncryptPassword(request?.Password, key),
                Contact = request?.Contact ?? 0,
                RoleId = request?.RoleId ?? 2
            };

            var inserted = await _dairyServices.InsertNewCustomer(login);

            if (inserted > 0)
                return Ok(new ActionResponse { Success = true });

            return Ok(new ActionResponse { Success = false, ErrorMessage = "Error occured while adding new customer" });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error in inserting new customer : {ex.ToString}");
            return BadRequest(500);
        }
    }
}
