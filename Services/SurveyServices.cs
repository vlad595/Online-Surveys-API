using System;
using DTO;
using Models;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Data;
using Repositories;
using Exceptions;

namespace Services
{
    public interface ISurveyService
    {
        Task<PagedResultDto<SurveyReadDto>> GetPagedSurveysAsync(
            string? searchTerm,
            string? category,
            string? sortOrder,
            int pageNumber,
            int pageSize,
            bool onlyApprovedPublic = false);

        Task<SurveyReadDto> GetByIdAsync(int id);
        Task<SurveyUpdateDto> GetForUpdateAsync(int id);
        Task CreateAsync(SurveyCreateDto dto, string? authorId);
        Task UpdateAsync(SurveyUpdateDto dto);
        Task DeleteAsync(int id);
        Task ToggleApprovalAsync(int id);
        Task<byte[]> ExportSurveySummaryToCsvAsync(int id);
    }
    public class SurveyService : ISurveyService
    {
        private readonly ISurveyRepository _surveyRepository;

        public SurveyService(ISurveyRepository surveyRepository)
        {
            _surveyRepository = surveyRepository;
        }

        public async Task<PagedResultDto<SurveyReadDto>> GetPagedSurveysAsync(
            string? searchTerm,
            string? category,
            string? sortOrder,
            int pageNumber,
            int pageSize,
            bool onlyApprovedPublic = false)
        {
            var query = _surveyRepository.GetQueryableWithDetails();

            if (onlyApprovedPublic)
            {
                query = query.Where(s => s.IsPublic && s.IsApproved && s.IsActive);
            }

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.Trim().ToLower();
                query = query.Where(s => s.Title.ToLower().Contains(term) || s.Description.ToLower().Contains(term));
            }

            if (!string.IsNullOrWhiteSpace(category))
            {
                query = query.Where(s => s.Category == category);
            }
            query = sortOrder switch
            {
                "title_asc" => query.OrderBy(s => s.Title),
                "title_desc" => query.OrderByDescending(s => s.Title),
                "responses_desc" => query.OrderByDescending(s => s.Responses.Count),
                _ => query.OrderByDescending(s => s.CreatedAt)
            };

            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            pageNumber = Math.Max(1, pageNumber);

            var surveys = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var items = surveys.Select(MapToReadDto).ToList();

            return new PagedResultDto<SurveyReadDto>
            {
                Items = items,
                CurrentPage = pageNumber,
                TotalPages = totalPages == 0 ? 1 : totalPages,
                TotalCount = totalCount,
                SearchTerm = searchTerm,
                CategoryFilter = category,
                SortOrder = sortOrder
            };
        }

        public async Task<SurveyReadDto> GetByIdAsync(int id)
        {
            var survey = await _surveyRepository.GetByIdWithDetailsAsync(id);
            if (survey == null)
                throw new NotFoundException($"Опитування з ID {id} не знайдено.");

            return MapToReadDto(survey);
        }

        public async Task<SurveyUpdateDto> GetForUpdateAsync(int id)
        {
            var survey = await _surveyRepository.GetByIdAsync(id);
            if (survey == null)
                throw new NotFoundException($"Опитування з ID {id} не знайдено.");

            return new SurveyUpdateDto
            {
                Id = survey.Id,
                Title = survey.Title,
                Description = survey.Description,
                Category = survey.Category,
                IsPublic = survey.IsPublic,
                IsActive = survey.IsActive,
                IsApproved = survey.IsApproved
            };
        }

        public async Task CreateAsync(SurveyCreateDto dto, string? authorId)
        {
            // Бізнес-перевірка (Крок 2.3)
            var existing = await _surveyRepository.GetQueryableWithDetails()
                .AnyAsync(s => s.Title.ToLower() == dto.Title.Trim().ToLower());

            if (existing)
                throw new BusinessValidationException("Опитування з такою назвою вже існує в системі.");

            var survey = new Survey
            {
                Title = dto.Title.Trim(),
                Description = dto.Description.Trim(),
                Category = dto.Category.Trim(),
                IsPublic = dto.IsPublic,
                IsActive = dto.IsActive,
                IsApproved = dto.IsApproved,
                AuthorId = authorId,
                CreatedAt = DateTime.UtcNow
            };

            if (!string.IsNullOrWhiteSpace(dto.InitialQuestionText))
            {
                var question = new Question
                {
                    Text = dto.InitialQuestionText.Trim(),
                    Type = QuestionType.SingleChoice,
                    Order = 1
                };

                if (!string.IsNullOrWhiteSpace(dto.InitialOptionsCsv))
                {
                    var opts = dto.InitialOptionsCsv.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                    foreach (var opt in opts)
                    {
                        question.Options.Add(new Option { Text = opt });
                    }
                }

                survey.Questions.Add(question);
            }

            await _surveyRepository.AddAsync(survey);
            await _surveyRepository.SaveAsync();
        }

        public async Task UpdateAsync(SurveyUpdateDto dto)
        {
            var survey = await _surveyRepository.GetByIdAsync(dto.Id);
            if (survey == null)
                throw new NotFoundException($"Опитування з ID {dto.Id} не знайдено.");

            survey.Title = dto.Title.Trim();
            survey.Description = dto.Description.Trim();
            survey.Category = dto.Category.Trim();
            survey.IsPublic = dto.IsPublic;
            survey.IsActive = dto.IsActive;
            survey.IsApproved = dto.IsApproved;

            _surveyRepository.Update(survey);
            await _surveyRepository.SaveAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var survey = await _surveyRepository.GetByIdAsync(id);
            if (survey == null)
                throw new NotFoundException($"Опитування з ID {id} не знайдено.");

            _surveyRepository.Delete(survey);
            await _surveyRepository.SaveAsync();
        }

        public async Task ToggleApprovalAsync(int id)
        {
            var survey = await _surveyRepository.GetByIdAsync(id);
            if (survey == null)
                throw new NotFoundException($"Опитування з ID {id} не знайдено.");

            survey.IsApproved = !survey.IsApproved;
            _surveyRepository.Update(survey);
            await _surveyRepository.SaveAsync();
        }

        public async Task<byte[]> ExportSurveySummaryToCsvAsync(int id)
        {
            var survey = await _surveyRepository.GetByIdWithDetailsAsync(id);
            if (survey == null)
                throw new NotFoundException($"Опитування з ID {id} не знайдено.");

            var sb = new StringBuilder();
            sb.AppendLine("ID Опитування;Назва;Категорія;Кількість питань;Кількість відповідей;Статус модерації");
            sb.AppendLine($"{survey.Id};\"{survey.Title}\";\"{survey.Category}\";{survey.Questions.Count};{survey.Responses.Count};{(survey.IsApproved ? "Схвалено" : "На модерації")}");
            sb.AppendLine();
            sb.AppendLine("Порядок;Питання;Тип;Варіанти відповідей");

            foreach (var q in survey.Questions)
            {
                var optionsStr = string.Join(", ", q.Options.Select(o => o.Text));
                sb.AppendLine($"{q.Order};\"{q.Text}\";{q.Type};\"{optionsStr}\"");
            }

            return Encoding.UTF8.GetPreamble().Concat(Encoding.UTF8.GetBytes(sb.ToString())).ToArray();
        }

        private static SurveyReadDto MapToReadDto(Survey s)
        {
            return new SurveyReadDto
            {
                Id = s.Id,
                Title = s.Title,
                Description = s.Description,
                Category = s.Category,
                CreatedAt = s.CreatedAt,
                IsPublic = s.IsPublic,
                IsApproved = s.IsApproved,
                IsActive = s.IsActive,
                AuthorName = s.Author?.FullName ?? s.Author?.Email ?? "Система",
                QuestionsCount = s.Questions?.Count ?? 0,
                ResponsesCount = s.Responses?.Count ?? 0,
                Questions = s.Questions?.Select(q => new QuestionReadDto
                {
                    Id = q.Id,
                    Text = q.Text,
                    Type = q.Type.ToString(),
                    Options = q.Options.Select(o => o.Text).ToList()
                }).ToList() ?? new List<QuestionReadDto>()
            };
        }
    }
}