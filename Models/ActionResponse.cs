namespace DairyAPI.Models;

public class ActionResponse
{
    public bool Success { get; set; }
    public string ErrorMessage { get; set; } = default!;
}
