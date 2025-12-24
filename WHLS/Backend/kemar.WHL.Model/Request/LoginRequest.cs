using kemar.WHL.Model.Common;

public class LoginRequest : CommonEntity
{
    public string Email { get; set; } = "";
    public string Password { get; set; } = "";
}