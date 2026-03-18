using FluentValidation;
using MusicStore.Dto.Request;

namespace MusicStore.Dto.Validations;

public class GenreDtoRequestValidator : AbstractValidator<GenreDtoRequest>
{
    public GenreDtoRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("El nombre del género es requerido")
            .MinimumLength(3).WithMessage("El nombre debe tener al menos 3 caracteres")
            .MaximumLength(50).WithMessage("El nombre no puede exceder 50 caracteres");
    }
}
