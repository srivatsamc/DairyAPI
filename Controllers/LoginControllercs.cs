using Dairy.API.Models.Login;
using Dairy.Application.Interface;
using Dairy.Domain;
using DairyAPI.Models;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Utility;

namespace Dairy.API.Controllers;

[Route("api/login/[controller]")]
[ApiController]
//[Authorize]
public class LoginController : ControllerBase
{
    private readonly ILogger<LoginController> _logger;
    private readonly IConfiguration _configuration;
    private readonly IDairyServices _dairyServices;
    private readonly IValidator<LoginModel> _loginValidator;

    private string? key { get;set; }

    public LoginController(ILogger<LoginController> logger, IConfiguration configuration, IDairyServices dairyServices, IValidator<LoginModel> loginValidator)
    {
        _logger = logger;
        _configuration = configuration;
        _dairyServices = dairyServices;
        _loginValidator = loginValidator;
        key = _configuration.GetValue<string>("EncryptDecrypt:Key");
    }

    /// <summary>
    /// Method to verify the logged in user
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("ValidateUser")]
    public async Task<ActionResult<ActionResponse>> ValidateUser(Credentials request)
    {
        _logger.LogTrace($"Entered method {nameof(ValidateUser)}");

        try
        {
            if (request == null)
                return BadRequest("Request cannot be null or empty");

            LoginCredentials logins = new LoginCredentials
            {
                Email = request.Username
            };

            bool isValidUser = await _dairyServices.IsCustomerExists(logins, 1); // 1 for login verification

            if(isValidUser)
            {
                logins.Password = PasswordManager.HashPassword(request.Password);
                
                bool isValid = await _dairyServices.IsCustomerExists(logins, 2);

                if (isValid)
                    return Ok(new ActionResponse { Success = true });

                return Ok(new ActionResponse { Success = false, ErrorMessage = "Please enter the correct password" });
            }
            else
                return Ok(new ActionResponse { Success = false, ErrorMessage = "Username does not exists!" });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error in validating user : {ex.ToString}");
            return StatusCode(500);
        }
    }

    /// <summary>
    /// Method to insert new user
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("InsertCustomer")]
    public async Task<ActionResult<ActionResponse>> InsertCustomer(LoginModel request)
    {
        _logger.LogTrace($"Entered method {nameof(InsertCustomer)}");

        try
        {
            if (request == null)
                return BadRequest("Access denied");

            var validationResult = await _loginValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage);
                return BadRequest(errors);
            }

            string hashedPassword = PasswordManager.HashPassword(request.Password);

            Login login = new Login
            {
                Firstname = request?.Firstname,
                Lastname = request?.Lastname,
                Address = request?.Address,
                Email = request?.Email, 
                Password = hashedPassword,
                Contact = request?.Contact,
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
            return StatusCode(500);
        }
    }
}
