using AdminService.Models;

namespace AdminService.Interfaces
{
    public interface IAdminService
    {
        Task<bool?> registeruser(Mst_Users userreq);
        Task<TokenresponseDto?> Login(string? username, string? password);
        Task<TokenresponseDto> refreshtokenasync(RefreshTokenReqDto req);
        Task<List<Mst_Menu>> GetMenus();


    }
}
