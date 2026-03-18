using FluentValidation;
using MusicStore.Dto.Request;

namespace MusicStore.Dto.Validations;

public class RegisterDtoRequestValidator : AbstractValidator<RegisterDtoRequest>
{
    public RegisterDtoRequestValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("El nombre es requerido");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("El apellido es requerido");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("El correo es requerido")
            .EmailAddress().WithMessage("El formato del correo es inválido");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("La contraseña es requerida")
            .MinimumLength(5).WithMessage("La contraseña debe tener al menos 5 caracteres");

        RuleFor(x => x.ConfirmPassword)
            .Equal(x => x.Password).WithMessage("Las contraseñas no coinciden");

        RuleFor(x => x.Age)
            .GreaterThan((short)18).WithMessage("Debe ser mayor de edad para registrarse");

        RuleFor(x => x.DocumentNumber)
            .NotEmpty().WithMessage("El número de documento es requerido");
    }
}
