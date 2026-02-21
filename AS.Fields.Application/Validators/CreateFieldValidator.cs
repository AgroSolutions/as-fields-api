using AS.Fields.Domain.DTO.Field;
using AS.Fields.Domain.ValueObjects;
using FluentValidation;

namespace AS.Fields.Application.Validators
{
    public class CreateFieldValidator : AbstractValidator<CreateFieldDTO>
    {
        public CreateFieldValidator()
        {
            RuleFor(x => x.Boundary)
                .NotNull()
                .SetValidator(new BoundaryValidator());

            RuleFor(x => x.Description)
                .NotEmpty()
                .WithMessage("A descrição do talhão é obrigatória.");

            RuleFor(x => x.Observations)
                .MaximumLength(500)
                .WithMessage("As observações não podem exceder 500 caracteres.");

            RuleFor(x => x.Crop)
                .IsInEnum()
                .WithMessage("O tipo de cultura é inválido.");
        }
    }

    public class PartialUpdateFieldValidator : AbstractValidator<PartialUpdateFieldDTO>
    {
        public PartialUpdateFieldValidator()
        {
            RuleFor(x => x.Boundary)
                 .SetValidator(new BoundaryValidator())
                 .When(x => x.Boundary != null);

            RuleFor(x => x.Description)
                .NotEmpty()
                .WithMessage("A descrição do talhão é obrigatória.")
                .When(x => x.Description != null);

            RuleFor(x => x.Observations)
                .MaximumLength(500)
                .WithMessage("As observações não podem exceder 500 caracteres.")
                .When(x => x.Observations != null);

            RuleFor(x => x.Crop)
                .IsInEnum()
                .WithMessage("O tipo de cultura é inválido.")
                .When(x => x.Crop != null);
        }
    }

    class BoundaryValidator : AbstractValidator<Boundary?>
    {
        public BoundaryValidator()
        {
            RuleFor(x => x.LatMin).LessThan(x => x.LatMax)
                .WithMessage("LatMin não pode ser maior ou igual a LatMax.");

            RuleFor(x => x.LongMin).LessThan(x => x.LongMax)
                .WithMessage("LongMin não pode ser maior ou igual a LongMax.");

        }
    }
}
