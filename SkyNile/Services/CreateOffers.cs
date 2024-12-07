using SkyNile.Services.Interfaces;

namespace SkyNile.Services
{
    public class CreateOffers : ICreateOffers
    {
        public string GenerateRandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();

            return new string(Enumerable.Range(1, length)
                .Select(_ => chars[random.Next(chars.Length)]).ToArray());
        }

        public double GenerateRandomDouble()
        {
            var random = new Random();
            return Math.Round(random.NextDouble(), 2);
        }
    }
}