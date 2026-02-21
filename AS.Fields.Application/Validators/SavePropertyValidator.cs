using AS.Fields.Domain.DTO.Property;
using AS.Fields.Domain.Entities;
using FluentValidation;

namespace AS.Fields.Application.Validators
{
    public class SavePropertyValidator : AbstractValidator<SavePropertyDTO>
    {
        public SavePropertyValidator()
        {
            RuleFor(x => x.Description).NotEmpty();
            RuleFor(x => x.Name).NotEmpty();
        }
    }
}
