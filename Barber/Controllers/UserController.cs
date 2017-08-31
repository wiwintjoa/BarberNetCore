using AutoMapper;
using Barber.API.DataAccess;
using Barber.API.DomainModel;
using Barber.API.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Barber.API.Controllers
{
    [Authorize]
    [Produces("application/json")]
    [Route("api/User")]
    public class UserController : Controller
    {
        private readonly BarberContext _context;
        private ILogger<AuthController> _logger;        
        private readonly IMapper _mapper;
        private readonly UserManager<BarberUser> _userManager;

        public UserController(ILogger<AuthController> logger, BarberContext context, IMapper mapper, UserManager<BarberUser> userManager)
        {
            _logger = logger;
            _context = context;
            _mapper = mapper;
            _userManager = userManager;
        }

        [HttpGet]
        [Route("GetAll")]
        public IEnumerable<UserViewModel> GetAll()
        {
            var users = new List<BarberUser>();

            users = _context.Users.ToList();
            
            return _mapper.Map<IEnumerable<BarberUser>, IEnumerable<UserViewModel>>(users);
        }

        [HttpGet("{id}")]        
        public UserViewModel GetById(string id)
        {
            var user = _context.Users.FirstOrDefault(e => e.Id == id);

            return _mapper.Map<BarberUser, UserViewModel>(user);
        }

        [HttpPut]
        [Route("Modify")]
        public async Task<IActionResult> Modify([FromBody] ModifyUserViewModel model)
        {
            if (!ModelState.IsValid) return BadRequest();

            var user = new BarberUser()
            {
                UserName = string.Concat(model.FirstName, model.LastName),
                FirstName = model.FirstName,
                LastName = model.LastName,
                ModifiedDateTime = DateTime.UtcNow,
                ModifiedBy = model.User,
                Email = model.Email,
                Address = model.Address,
                DOB = model.DOB,
                PasswordHash = HashPassword(model.Password)
            };

           
            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded) return Ok(result);
           
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("error", error.Description);
            }

            return Ok(new
            {
                user = _mapper.Map<BarberUser, UserViewModel>(user)
            });            
        }

        private static string HashPassword(string password)
        {
            byte[] salt;
            byte[] buffer2;
            if (password == null)
            {
                throw new ArgumentNullException("password");
            }
            using (Rfc2898DeriveBytes bytes = new Rfc2898DeriveBytes(password, 0x10, 0x3e8))
            {
                salt = bytes.Salt;
                buffer2 = bytes.GetBytes(0x20);
            }
            byte[] dst = new byte[0x31];
            Buffer.BlockCopy(salt, 0, dst, 1, 0x10);
            Buffer.BlockCopy(buffer2, 0, dst, 0x11, 0x20);
            return Convert.ToBase64String(dst);
        }

        private static bool VerifyHashedPassword(string hashedPassword, string password)
        {
            byte[] buffer4;
            if (hashedPassword == null)
            {
                return false;
            }
            if (password == null)
            {
                throw new ArgumentNullException("password");
            }
            byte[] src = Convert.FromBase64String(hashedPassword);
            if ((src.Length != 0x31) || (src[0] != 0))
            {
                return false;
            }
            byte[] dst = new byte[0x10];
            Buffer.BlockCopy(src, 1, dst, 0, 0x10);
            byte[] buffer3 = new byte[0x20];
            Buffer.BlockCopy(src, 0x11, buffer3, 0, 0x20);
            using (Rfc2898DeriveBytes bytes = new Rfc2898DeriveBytes(password, dst, 0x3e8))
            {
                buffer4 = bytes.GetBytes(0x20);
            }
            return ByteArraysEqual(buffer3, buffer4);
        }

        private static bool ByteArraysEqual(byte[] b1, byte[] b2)
        {
            if (b1 == b2) return true;
            if (b1 == null || b2 == null) return false;
            if (b1.Length != b2.Length) return false;
            for (int i = 0; i < b1.Length; i++)
            {
                if (b1[i] != b2[i]) return false;
            }
            return true;
        }
    }
}