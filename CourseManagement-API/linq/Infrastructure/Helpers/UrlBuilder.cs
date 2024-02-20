namespace linq.Infrastructure.Helpers
{
    public class UrlBuilder
    {
        private const string ApplicationUrl = "https://localhost:7013";

        public static string EmailConfirmationLink(string userId, string code)
        {
            return $"{ApplicationUrl}/Account/ConfirmEmail?userId={userId}&code={code}";
        }

        public static string ResetPasswordCallbackLink(string code)
        {
            return $"{ApplicationUrl}/Account/ResetPassword?code={code}";
        }
    }
}