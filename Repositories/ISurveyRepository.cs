using Models;

namespace Repositories
{
    public interface ISurveyRepository : IRepository<Survey>
    {
        Task<Survey?> GetByIdWithDetailsAsync(int id);
        IQueryable<Survey> GetQueryableWithDetails();
    }
}