using api.Interfaces;
using Microsoft.AspNetCore.Mvc;
using api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using api.Extensions;
namespace api.Controllers
{
    [Route("api/portfolio")]
    [ApiController]
    [Authorize]
    public class PortfolioController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IStockRepository _stockRepo;
        private readonly IPortfolioRepository _portfolioRepo;

        public PortfolioController(UserManager<AppUser> userManager,
            IStockRepository stockRepo, IPortfolioRepository portfolioRepo)
        {
            _userManager = userManager;
            _stockRepo = stockRepo;
            _portfolioRepo = portfolioRepo;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var username = User.GetUsername();

            if (string.IsNullOrEmpty(username))
            {
                return BadRequest("Username cannot be null or empty.");
            }

            var appUser = await _userManager.FindByNameAsync(username);

            if (appUser == null)
            {
                return NotFound("User not found.");
            }

            var userPortfolio = await _portfolioRepo.GetAsync(appUser);

            return Ok(userPortfolio);
        }

        [HttpPost]
        public async Task<IActionResult> Create(string symbol)
        {
            var username = User.GetUsername();

            if (string.IsNullOrEmpty(username))
            {
                return BadRequest("Username cannot be null or empty.");
            }

            var appUser = await _userManager.FindByNameAsync(username);

            if (appUser == null)
            {
                return NotFound("User not found.");
            }

            var stock = await _stockRepo.GetBySymbolAsync(symbol);

            if (stock == null)
            {
                return NotFound("Stock not found.");
            }

            var userPortfolio = await _portfolioRepo.GetAsync(appUser);

            if (userPortfolio.Any(e => e.Symbol.ToLower() == symbol.ToLower()))
            {
                return BadRequest("Cannot add same stock to portofolio");
            }

            var portfolioModel = new Portfolio
            {
                AppUserId = appUser.Id,
                StockId = stock.Id
            };

            await _portfolioRepo.CreateAsync(portfolioModel);

            if (portfolioModel == null)
            {
                return StatusCode(500, "Could not create");
            }
            else
            {
                return Created();
            }
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(string symbol)
        {
            var username = User.GetUsername();

            if (string.IsNullOrEmpty(username))
            {
                return BadRequest("Username cannot be null or empty.");
            }

            var appUser = await _userManager.FindByNameAsync(username);

            if (appUser == null)
            {
                return NotFound("User not found.");
            }

            var userPortfolio = await _portfolioRepo.GetAsync(appUser);

            var filteredStock = userPortfolio.Where(s => s.Symbol.ToLower() == symbol.ToLower()).ToList();

            if (filteredStock.Count() == 1)
            {
                await _portfolioRepo.DeleteAsync(appUser, symbol);
            }
            else
            {
                return BadRequest("Stock not in your portfolio");
            }

            return Ok();
        }
    }
}
