using CampeonatinhoApp.Interfaces;
using CampeonatinhoApp.Models;
using CampeonatinhoApp.Models.DTOs;
using CampeonatinhoApp.Repositories;
using CampeonatinhoApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Sprache;
using System.Text.Encodings.Web;
using System.Web;

namespace CampeonatinhoApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TournamentController : ControllerBase
    {
        private readonly ILogger<UserProfileDTO> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly EmailSenderService _emailSenderService;
        private readonly ITokenRepository _tokenRepository;

        public TournamentController(ILogger<UserProfileDTO> logger, UserManager<ApplicationUser> userManager, EmailSenderService emailSenderService, ITokenRepository tokenRepository)
        {
            _logger = logger;
            _userManager = userManager;
            _emailSenderService = emailSenderService;
            _tokenRepository = tokenRepository;
        }

        [HttpGet("all")]
        [Authorize(Roles = "Admin")]
        public ActionResult<IList<TournamentDTO>> GetAllTournaments()
        {
            var users = _userManager.Users.ToList();
            return users == null ? BadRequest("Invalid Request.") : Ok(users);
        }



    }
}
