using Microsoft.IdentityModel.Tokens;

namespace OceanBattle.Rsa.Abstractions
{
    public interface IRsaKeyFactory
    {
        IEnumerable<JsonWebKey> GetSecretKeys();
        IEnumerable<JsonWebKey> GetPublicKeys();
    }
}