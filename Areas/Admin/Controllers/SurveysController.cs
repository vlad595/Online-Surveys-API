using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using DTO;
using Exceptions;
using Services;

namespace SurveySystem.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class SurveysController : Controller
    {
        private readonly ISurveyService _surveyService;

        public SurveysController(ISurveyService surveyService)
        {
            _surveyService = surveyService;
        }

        public async Task<IActionResult> Index(string? searchTerm, string? category, string? sortOrder, int page = 1)
        {
            PopulateCategories(category);
            var pagedResult = await _surveyService.GetPagedSurveysAsync(searchTerm, category, sortOrder, page, pageSize: 5);
            return View(pagedResult);
        }

        public IActionResult Create()
        {
            PopulateCategories();
            return View(new SurveyCreateDto());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SurveyCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                PopulateCategories(dto.Category);
                return View(dto);
            }

            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                await _surveyService.CreateAsync(dto, userId);
                TempData["SuccessMessage"] = "Опитування успішно створено!";
                return RedirectToAction(nameof(Index));
            }
            catch (BusinessValidationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                PopulateCategories(dto.Category);
                return View(dto);
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var dto = await _surveyService.GetForUpdateAsync(id);
                PopulateCategories(dto.Category);
                return View(dto);
            }
            catch (NotFoundException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(SurveyUpdateDto dto)
        {
            if (!ModelState.IsValid)
            {
                PopulateCategories(dto.Category);
                return View(dto);
            }

            try
            {
                await _surveyService.UpdateAsync(dto);
                TempData["SuccessMessage"] = "Опитування успішно оновлено!";
                return RedirectToAction(nameof(Index));
            }
            catch (NotFoundException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var dto = await _surveyService.GetByIdAsync(id);
                return View(dto);
            }
            catch (NotFoundException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _surveyService.DeleteAsync(id);
                TempData["SuccessMessage"] = "Опитування видалено.";
            }
            catch (NotFoundException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleApprove(int id)
        {
            await _surveyService.ToggleApprovalAsync(id);
            TempData["SuccessMessage"] = "Статус модерації змінено.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> ExportCsv(int id)
        {
            var bytes = await _surveyService.ExportSurveySummaryToCsvAsync(id);
            return File(bytes, "text/csv", $"survey_{id}_analytics.csv");
        }

        private void PopulateCategories(string? selected = null)
        {
            var categories = new List<string> { "Освіта", "HR та Корпоративна культура", "Маркетинг", "ІТ та Технології", "Загальне" };
            ViewBag.Categories = new SelectList(categories, selected);
        }
    }
}