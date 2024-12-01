using SkyNile.DTO;
using SkyNile.HelperModel;
using System.Threading.Tasks;

namespace SkyNile.Services
{
    public interface IAuthService
    {
        Task<AuthModel> RegisterAsync(Register model);
        Task<AuthModel> GetTokenAsync(Login model);
        Task<string> AddRoleAsync(AddRoleModel model);

    }
}