using System.Linq;
using FluentValidation;
using TransactionApplication.BL.Constants;
using TransactionApplication.BL.Helpers;
using TransactionApplication.BL.Models;

namespace TransactionApplication.BL.Validators
{
    public class TransactionCreateModelValidator : AbstractValidator<TransactionCreateModel>
    {
        public TransactionCreateModelValidator()
        {
            RuleFor(x => x.Amount).NotNull().WithMessage(ValidationMessages.DecimalTypeError)
                .OverridePropertyName("Amount");
            RuleFor(x => x.Code).NotEmpty().WithMessage(ValidationMessages.EmptyItemError).DependentRules(() =>
            {
                RuleFor(x => x.Code).Must(BeIsoFormat).WithMessage(ValidationMessages.CodeFormatError);
            });
            RuleFor(x => x.Date).NotEmpty().WithMessage(ValidationMessages.DateFormatError);
            RuleFor(x => x.PublicId).NotEmpty().WithMessage(ValidationMessages.EmptyItemError)
                .OverridePropertyName("Id").MaximumLength(50).WithMessage(ValidationMessages.MaxLengthError);
            RuleFor(x => x.Status).NotEmpty().WithMessage(ValidationMessages.EmptyItemError).DependentRules(() =>
            {
                RuleFor(x => x.Status).Must(BeCorrectStatus)
                    .WithMessage(ValidationMessages.InappropriateStatusError);
            });
        }

        private bool BeIsoFormat(string code)
        {
            return code.Length.Equals(3) && IsAllUpper(code);
        }

        private bool IsAllUpper(string input)
        {
            return input.All(t => !char.IsLetter(t) || char.IsUpper(t));
        }

        private bool BeCorrectStatus(string status)
        {
            return StatusHelper.IsAppropriateStatus(status);
        }
    }
}