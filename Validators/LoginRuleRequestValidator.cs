using Azure.Core;
using Dairy.API.Models.Login;
using Dairy.Application.Interface;
using Dairy.Domain;
using FluentValidation;
using Utility;

namespace Dairy.API.Validators;

public class LoginRuleRequestValidator : AbstractValidator<LoginModel>
{
    private readonly ILogger<LoginRuleRequestValidator> _logger;
    private readonly IDairyServices _dairyServices;

    public LoginRuleRequestValidator(ILogger<LoginRuleRequestValidator> logger, IDairyServices dairyServices)
    {
        _logger = logger;
        _dairyServices = dairyServices;

        RuleFor(l => l.Firstname)
            .NotEmpty().WithMessage("First name is required");

        RuleFor(l => l.Email)
            .NotEmpty().WithMessage("Email is required");

        RuleFor(l => l.RoleId)
            .NotEmpty().WithMessage("Role Id is required")
            .GreaterThan(0).WithMessage("Role Id cannot be zero")
            .LessThan(3).WithMessage("Only 1 and 2 are allowed");

        RuleFor(l => l.Contact)
            .NotEmpty().WithMessage("Contact is required")
            .MaximumLength(10).WithMessage("Maximum length of contact is 10");
        
        RuleFor(l => l.Password)
            .NotEmpty().WithMessage("Password is required")
            .MustAsync(async (model, password, token) =>
            {
                return await IsCustomerDetailsExists(model.Email);
            }).WithMessage("Username already exists");
    }

    /// <summary>
    /// Method to check the duplicate user exists or not
    /// </summary>
    /// <param name="email"></param>
    /// <param name="password"></param>
    /// <returns>true or false</returns>
    private async Task<bool> IsCustomerDetailsExists(string? email)
    {
        _logger.LogTrace($"Entered method {nameof(IsCustomerDetailsExists)}");

        bool isExists = false;
        try
        {
            if (!string.IsNullOrEmpty(email))
            {
                LoginCredentials credentials = new LoginCredentials
                {
                    Email = email
                };

                isExists = await _dairyServices.IsCustomerExists(credentials, 1);
            } 
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error in {nameof(IsCustomerDetailsExists)} : {ex.ToString}");
        }
        return !isExists;
    }
}
