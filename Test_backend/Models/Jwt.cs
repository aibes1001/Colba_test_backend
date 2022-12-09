using System.Security.Claims;

namespace Test_backend.Models
{
    public class Jwt
    {
        public string Key { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public string Subject { get; set; }

        public static dynamic ValidateToken(ClaimsIdentity identity)
        {
            try
            {
                if (identity.Claims.Count() == 0)
                {
                    return new
                    {
                        success = false,
                        msg = "Not valid token",
                        result = ""
                    };
                }
                return new
                {
                    success = true,
                    msg = "Validation token successfull",
                    result = new
                    {
                        UserId = identity.Claims.FirstOrDefault(x => x.Type == "Id").Value,
                        Username = identity.Claims.FirstOrDefault(x => x.Type == "Username").Value,
                        UserRole = identity.Claims.FirstOrDefault(x => x.Type == "Role").Value
                    }
                };
            }
            catch (Exception e)
            {
                return new
                {
                    success = false,
                    msg = "Error validation" + e.ToString(),
                    result = ""

                };
            }
        }
    }
}
