using System.Threading.Tasks;
using Application.Dtos;
using Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using AutoMapper;
using Persistence;
using AutoMapper.QueryableExtensions;
using API.Dtos;
using Microsoft.AspNetCore.Http;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountsController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;
        private readonly DataContext _context;
        private readonly ICloudinaryServices _cloudinaryServices;
        public AccountsController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager,
        ITokenService tokenService, IMapper mapper, DataContext context, ICloudinaryServices cloudinaryServices)
        {
            _cloudinaryServices = cloudinaryServices;
            _context = context;
            _tokenService = tokenService;
            _signInManager = signInManager;
            _userManager = userManager;
            _mapper = mapper;
        }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> Register(RegisterDto registedDto)
    {
        var user = await _userManager.Users.SingleOrDefaultAsync(u => u.UserName == registedDto.Username || u.Email == registedDto.Email);
        if (user != null) return BadRequest("Username or Email already taken");
        var newUser = new AppUser
        {
            UserName = registedDto.Username,
            Email = registedDto.Email,
        };
        var result = await _userManager.CreateAsync(newUser, registedDto.Password);
        if (result.Succeeded == false) return BadRequest(result.Errors);
        return new UserDto
        {
            Token = _tokenService.CreateToken(newUser),
            Username = newUser.UserName,
            Email = newUser.Email,
            Bio = newUser.Bio
        };
    }


    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
    {
        var user = await _userManager.FindByEmailAsync(loginDto.Email);
        if (user == null) return BadRequest("Invalid Email Address");
        var signIn = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);
        if (signIn.Succeeded == false) return Unauthorized();
        return new UserDto
        {
            Username = user.UserName,
            Email = user.Email,
            Bio = user.Bio,
            Token = _tokenService.CreateToken(user)
        };
    }

    [HttpGet]
    public async Task<ActionResult<UserDto>> GetCurrentUser()
    {
        var user = await _userManager.FindByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));
        return _mapper.Map(user, new UserDto { Token = _tokenService.CreateToken(user) });
    }


    [HttpGet("{userId}")]
    public async Task<ActionResult<AppUser>> GetCertainUser(string userId)
    {
        var user = await _context.Users.ProjectTo<ProfileDto>(_mapper.ConfigurationProvider, new
        {
            currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)
        })
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null) return NotFound();
        return Ok(user);
    }

    [HttpPut("update_profile")]
    public async Task<ActionResult> UpdateUsersProfile(UpdateProfileDto updateProfileDto)
    {
        var user = await _context.Users.FindAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));

        user.UserName = updateProfileDto.Username;
        user.Bio = updateProfileDto.Bio;

        var result = await _context.SaveChangesAsync() > 0;
        if (result)
            return Ok();

        return BadRequest("Could not update profile");
    }

    [HttpPost("upload_profile_image")]
    public async Task<ActionResult> UploadProfileImage([FromForm] IFormFile file)
    {
        var user = await _context.Users
            .Include(u => u.UserProfileImage)
            .FirstOrDefaultAsync(u => u.Id == User.FindFirstValue(ClaimTypes.NameIdentifier));
        if(user == null)
            return Unauthorized();

        //if user has another profile image delete it 
        //and upload the new one
        if(user.UserProfileImage != null)
        {
            _context.UserProfileImages.Remove(user.UserProfileImage);
            await _cloudinaryServices.DeleteImage(user.UserProfileImage.PublicId);
            var uplaod_profile_image_result = await _cloudinaryServices.UploadPhoto(file);
            user.UserProfileImage = new UserProfileImage
            {
                PublicId= uplaod_profile_image_result.PublicId,
                Url= uplaod_profile_image_result.Url.ToString()
            };
        } else 
        {
            //if user hasnt previously uploaded a photo
            var uplaod_profile_image_result = await _cloudinaryServices.UploadPhoto(file);
            user.UserProfileImage = new UserProfileImage
            {
                PublicId= uplaod_profile_image_result.PublicId,
                Url= uplaod_profile_image_result.Url.ToString()
            };
        }
        var result = await _context.SaveChangesAsync() > 0;
        
        return result? Ok() : BadRequest("Could not upload profile Image");
    }
}
}