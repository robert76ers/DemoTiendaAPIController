using DemoTienda.Application.DTOs.Request;
using FluentValidation;

namespace DemoTienda.Application.Validators
{
    public class UpdateCategoriaRequestValidator : AbstractValidator<UpdateCategoriaRequestDTO>
    {
        public UpdateCategoriaRequestValidator()
        {
            RuleFor(x => x.Nombre)
                .NotEmpty().WithMessage("El nombre de la categoria es obligatorio")
                .MaximumLength(100).WithMessage("El nombre de la categoria no puede exceder los 100 caracteres");
        }
    }
}
