using AdminService.Interfaces;
using AdminService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AdminService.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;
        private readonly ILogger<AdminController> _logger;

        public AdminController(IAdminService adminService, ILogger<AdminController> logger)
        {
            _adminService = adminService;
            _logger = logger;
        }

        [HttpGet]
        //[Authorize]
        [Route("GetMenus")]
        public async Task<IActionResult> GetMenus()
        {
            var res = await _adminService.GetMenus();
            return res != null ? Ok(res) : NotFound();
        }

        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        [HttpPost]
        [Route("RegisterUser")]
        public async Task<IActionResult> RegisterUser([FromBody] Mst_Users user)
        {
            _logger.LogInformation("Register User Api called at", DateTime.Now);
            try
            {
               return Ok(await _adminService.registeruser(user));
            }
            catch(Exception ex)
            {
                _logger.LogError("Register Api error:-", ex);
                throw;
            }
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            _logger.LogInformation("Login Api called at", DateTime.Now);
            try
            {
                return Ok(await _adminService.Login(request.username, request.password));
            }
            catch(Exception ex)
            {
                _logger.LogError("Login Api error:-", ex);
                throw;
            }
        }

        [HttpPost]
        [Route("RefreshToken")]
        public async Task<IActionResult> refreshtoken(RefreshTokenReqDto req)
        {
            _logger.LogInformation("Refresh token Api called at", DateTime.Now);
            try
            {
                return Ok(await _adminService.refreshtokenasync(req));
            }
            catch (Exception ex)
            {
                _logger.LogError("Refresh Token Api error:-", ex);
                throw;
            }
        }

        // PUT api/<AdminController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<AdminController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
