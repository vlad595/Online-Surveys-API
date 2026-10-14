using Microsoft.EntityFrameworkCore;
using Data;
using Models;

namespace Repositories
{
    public class SurveyRepository : Repository<Survey>, ISurveyRepository
    {
        public SurveyRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<Survey?> GetByIdWithDetailsAsync(int id)
        {
            return await _context.Surveys
                .Include(s => s.Author)
                .Include(s => s.Questions.OrderBy(q => q.Order))
                    .ThenInclude(q => q.Options)
                .Include(s => s.Responses)
                    .ThenInclude(r => r.Answers)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public IQueryable<Survey> GetQueryableWithDetails()
        {
            return _context.Surveys
                .Include(s => s.Author)
                .Include(s => s.Questions)
                .Include(s => s.Responses)
                .AsQueryable();
        }
    }
}