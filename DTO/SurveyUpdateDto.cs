using System.ComponentModel.DataAnnotations;

namespace DTO
{
    public class SurveyUpdateDto
    {
        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage = "Назва опитування є обов'язковою.")]
        [StringLength(150, MinimumLength = 5, ErrorMessage = "Назва повинна містити від 5 до 150 символів.")]
        [Display(Name = "Назва опитування")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Опис опитування є обов'язковим.")]
        [StringLength(1000, MinimumLength = 10, ErrorMessage = "Опис повинен містити від 10 до 1000 символів.")]
        [Display(Name = "Опис анкети")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Категорія є обов'язковою.")]
        [StringLength(50, ErrorMessage = "Категорія не може перевищувати 50 символів.")]
        [Display(Name = "Категорія")]
        public string Category { get; set; } = string.Empty;

        [Display(Name = "Публічне опитування")]
        public bool IsPublic { get; set; }

        [Display(Name = "Активне")]
        public bool IsActive { get; set; }

        [Display(Name = "Схвалено адміністратором (Модерація)")]
        public bool IsApproved { get; set; }
    }
}