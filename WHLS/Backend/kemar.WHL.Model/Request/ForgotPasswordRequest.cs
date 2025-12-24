namespace Kemar.WHL.Model.Request
{
    public class ForgotPasswordRequest
    {
        public string Email { get; set; } = "";
        public string NewPassword { get; set; } = "";
        public string ConfirmPassword { get; set; } = "";
    }
}