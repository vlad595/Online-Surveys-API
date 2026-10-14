using System.ComponentModel.DataAnnotations;

namespace DTO
{
    public class SurveyCreateDto
    {
        [Required(ErrorMessage = "Назва опитування є обов'язковою.")]
        [StringLength(150, MinimumLength = 5, ErrorMessage = "Назва повинна містити від 5 до 150 символів.")]
        [Display(Name = "Назва опитування")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Опис опитування є обов'язковим.")]
        [StringLength(1000, MinimumLength = 10, ErrorMessage = "Опис повинен містити від 10 до 1000 символів.")]
        [Display(Name = "Опис анкети")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Оберіть або введіть категорію.")]
        [StringLength(50, ErrorMessage = "Назва категорії не може перевищувати 50 символів.")]
        [Display(Name = "Категорія")]
        public string Category { get; set; } = "Освіта";

        [Display(Name = "Публічне опитування")]
        public bool IsPublic { get; set; } = true;

        [Display(Name = "Активне (доступне для проходження)")]
        public bool IsActive { get; set; } = true;

        [Display(Name = "Схвалено модератором")]
        public bool IsApproved { get; set; } = false;

        // Початкове запитання при створенні анкети
        [StringLength(300, ErrorMessage = "Текст питання не може перевищувати 300 символів.")]
        [Display(Name = "Перше питання (необов'язково)")]
        public string? InitialQuestionText { get; set; }

        [Display(Name = "Варіанти відповідей (через кому)")]
        public string? InitialOptionsCsv { get; set; }
    }
}