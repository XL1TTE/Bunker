using System.Security.Cryptography;

namespace Bunker.LobbyService.Domain;

public readonly record struct InviteCode(string Value);

public static class InviteCodeFactory
{
    private const int Length = 12;
    private const string Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
    
    extension (InviteCode)
    {
        public static InviteCode New()
        {
            var chars = new char[Length];
            for (int i = 0; i < Length; i++)
            {
                int idx = RandomNumberGenerator.GetInt32(Alphabet.Length);
                chars[i] = Alphabet[idx];
            }

            return new InviteCode(new string(chars));
        }

        public static InviteCode Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value) || value.Length != Length)
                throw new ArgumentException("Invite code must be 12 characters long");

            return new InviteCode(value.ToUpperInvariant());
        }
    }
}

