using AdminService.Interfaces;
using AdminService.Models;

namespace AdminService.Services
{
    public class AdminServices : IAdminService
    {
        private readonly IAdminRepository _repository;
        private readonly ILogger<AdminServices> _logger;

        public AdminServices(IAdminRepository repository, ILogger<AdminServices> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<bool?> registeruser(Mst_Users userreq)
        {
            return await _repository.registeruser(userreq);
        }
        public async Task<TokenresponseDto?> Login(string? username, string? password)
        {
            return await _repository.Login(username, password);
        }

        public async Task<TokenresponseDto> refreshtokenasync(RefreshTokenReqDto req)
        {
            return await _repository.refreshtokenasync(req);
        }

        public async Task<List<Mst_Menu>> GetMenus()
        {
           
                return await _repository.GetMenus();
            
        }

        public async Task<List<Mst_Movies>> GetMovies()
        {
            
                return await _repository.GetMovies();
        }

    }
}
