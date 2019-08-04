using Microsoft.AspNetCore.Identity;

namespace ChatLe.Repository.Identity
{
    public class LookupNormalizer : ILookupNormalizer
    {
        public string Normalize(string key)
        {
            return key;
        }

        public string NormalizeEmail(string email) => Normalize(email);

        public string NormalizeName(string name) => Normalize(name);
    }
}
