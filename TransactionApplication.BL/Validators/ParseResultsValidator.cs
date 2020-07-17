using FluentValidation;
using TransactionApplication.BL.Models;

namespace TransactionApplication.BL.Validators
{
    public class ParseResultsValidator : AbstractValidator<ParseResults>
    {
        public ParseResultsValidator()
        {
            RuleForEach(x => x.Transactions)
                .SetValidator(new TransactionCreateModelValidator())
                .OverridePropertyName("TransactionItem");
        }
    }
}