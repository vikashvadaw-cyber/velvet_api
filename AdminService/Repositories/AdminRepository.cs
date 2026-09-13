using AdminService.Interfaces;
using AdminService.Models;
using AdminService.Dbcontext;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;

namespace AdminService.Repositories
{
    public class AdminRepository : IAdminRepository
    {
        private readonly admindbcontext _dbcontext;
        private readonly ILogger<AdminRepository> _logger;
        private readonly IConfiguration _config;

        public AdminRepository(admindbcontext dbcontext, ILogger<AdminRepository> logger, IConfiguration config)
        {
            _dbcontext = dbcontext;
            _logger = logger;
            _config = config;
        }

        public async Task<List<Mst_Menu>> GetMenus()
        {
            try
            {
                var menus = await _dbcontext.Mst_Menu.ToListAsync();

                return menus;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting all users.");
                throw;
            }
        }

        public async Task<bool?> registeruser(Mst_Users userreq)
        {
            _logger.LogInformation("Register Api called at", DateTime.Now);
            try
            {
                if (await _dbcontext.Mst_Users.AnyAsync(u => u.UserName == userreq.UserName))
                {
                    return null;
                }

                var user = userreq;
                user.Password = new PasswordHasher<Mst_Users>().HashPassword(user, userreq.Password);
                user.CreatedOn = DateTime.Now;
                 _dbcontext.Mst_Users.Add(user);
                await _dbcontext.SaveChangesAsync();
                return true;

            }
            catch(Exception ex)
            {
                _logger.LogError("Error in registeruser at", ex);
                throw;
            }
        }

        public async Task<TokenresponseDto?> Login(string ? username, string ? password)
        {
            _logger.LogInformation("Login Api called at", DateTime.Now);
            try
            {
                var user = await _dbcontext.Mst_Users.FirstOrDefaultAsync(u => u.UserName == username);

                if(user is null)
                {
                    return null;
                }
                if(new PasswordHasher<Mst_Users>().VerifyHashedPassword
                (user, user.Password, password)==PasswordVerificationResult.Failed)
                {
                    return null;
                }

                var token = new TokenresponseDto
                {
                    userid = user.UserId,
                    username = user.UserName,
                    roleid = user.RoleId,
                    email = user.Email,
                    accesstoke = createtoken(user),
                    refreshtoken = await refreshtoken(user)

                };


                user.LastLogin = DateTime.Now;
                user.RefreshToken = token.refreshtoken;
                user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(1);

                await _dbcontext.SaveChangesAsync();

                return token;
            }
            catch(Exception ex)
            {
                _logger.LogError("Error in Login Api", ex);
                throw;
            }
        }

        private string createtoken(Mst_Users user)
        {
            var claim = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Role, user.RoleId.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetValue<string>("jwt:Token")!));

            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

            var tokendescriptor = new JwtSecurityToken
                (
                    issuer: _config.GetValue<string>("jwt:Issuer"),
                    audience: _config.GetValue<string>("jwt:Audience"),
                    claims: claim,
                    expires: DateTime.UtcNow.AddDays(1),
                    signingCredentials: cred
                );

            return new JwtSecurityTokenHandler().WriteToken(tokendescriptor);
        }

        private async Task<string> refreshtoken(Mst_Users user)
        {
            var randomnum = new byte[32];
            using var randomgenerator = RandomNumberGenerator.Create();
            randomgenerator.GetBytes(randomnum);
            var refreshtoken = Convert.ToBase64String(randomnum);

            user.RefreshToken = refreshtoken;
            user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(1);

            await _dbcontext.SaveChangesAsync();
            return refreshtoken;
        }

        public async Task<TokenresponseDto> refreshtokenasync(RefreshTokenReqDto req)
        {
            var user =await _dbcontext.Mst_Users.FindAsync(req.UserId);

            if (user is null || user.RefreshToken != req.refreshtoken || user.RefreshTokenExpiry < DateTime.UtcNow)
            {
                return null;
            }

            var token = new TokenresponseDto
            {
                accesstoke = createtoken(user),
                refreshtoken =await refreshtoken(user)
            };

            return token;

        }



    }
}
