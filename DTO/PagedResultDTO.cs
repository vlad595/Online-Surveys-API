namespace DTO
{
    public class PagedResultDto<T>
    {
        public IEnumerable<T> Items { get; set; } = new List<T>();
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int TotalCount { get; set; }
        public string? SearchTerm { get; set; }
        public string? CategoryFilter { get; set; }
        public string? SortOrder { get; set; }
    }
}