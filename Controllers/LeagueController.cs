using CampeonatinhoApp.Interfaces;
using CampeonatinhoApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CampeonatinhoApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LeagueController : ControllerBase
    {
        private readonly ILogger<League> _logger;
        private readonly ILeagueRepository _leagueRepository;

        public LeagueController(ILogger<League> logger, ILeagueRepository leagueRepository)
        {
            _logger = logger;
            _leagueRepository = leagueRepository;
        }

        [HttpGet]
        public ActionResult<IList<League>> GetAllLeagues()
        {
            var leagues = _leagueRepository.GetAllAsync().GetAwaiter().GetResult().ToList();
            if (leagues == null || leagues.Count == 0)
            {
                return NotFound("No leagues found.");
            }
            return Ok(leagues);
        }

        [HttpGet("{id}")]
        public ActionResult<League> GetLeagueById(int id)
        {
            var league = _leagueRepository.GetByIdAsync(id).GetAwaiter().GetResult();
            if (league == null)
            {
                return NotFound();
            }
            return Ok(league);
        }

    }
}
