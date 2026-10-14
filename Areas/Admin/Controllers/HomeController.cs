using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models;
using Services;

namespace Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class HomeController : Controller
    {
        private readonly ISurveyService _surveyService;
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(ISurveyService surveyService, UserManager<ApplicationUser> userManager)
        {
            _surveyService = surveyService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var surveys = await _surveyService.GetPagedSurveysAsync(null, null, null, 1, 100);
            ViewBag.TotalSurveys = surveys.TotalCount;
            ViewBag.PendingModeration = surveys.Items.Count(s => !s.IsApproved);
            ViewBag.TotalUsers = _userManager.Users.Count();
            return View();
        }
    }
}