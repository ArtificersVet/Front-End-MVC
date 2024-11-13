using System.ComponentModel.DataAnnotations;

namespace MVC_NPANTS.Models
{
    public class UniqueTallaIdAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var viewModel = (EstiloEditViewModel)validationContext.ObjectInstance;
            var duplicateTallas = viewModel.Tallas
                .GroupBy(t => t.TallaId)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key);
            if (duplicateTallas.Any())
            {
                var errorMessage = $"Las tallas con los IDs {string.Join(", ", duplicateTallas)} ya están asignadas a este estilo.";
                return new ValidationResult(errorMessage);
            }
            return ValidationResult.Success;
        }
    }
}

