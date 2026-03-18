using FluentValidation;
using MusicStore.Dto.Request;

namespace MusicStore.Dto.Validations;

public class SaleDtoRequestValidator : AbstractValidator<SaleDtoRequest>
{
    public SaleDtoRequestValidator()
    {
        RuleFor(x => x.ConcertId)
            .GreaterThan(0).WithMessage("El Id del concierto debe ser válido");

        RuleFor(x => x.TicketQuantity)
            .GreaterThan((short)0).WithMessage("La cantidad a comprar debe ser mayor a cero")
            .LessThan((short)100).WithMessage("La cantidad excede el límite permitido por venta");
    }
}
