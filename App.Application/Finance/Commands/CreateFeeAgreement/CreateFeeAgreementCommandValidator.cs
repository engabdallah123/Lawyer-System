using FluentValidation;

namespace App.Application.Finance.Commands.CreateFeeAgreement;

public class CreateFeeAgreementCommandValidator : AbstractValidator<CreateFeeAgreementCommand>
{
    public CreateFeeAgreementCommandValidator()
    {
        RuleFor(f => f.ClientId).NotEmpty().WithMessage("معرف الموكل مطلوب.");
        RuleFor(f => f.TotalAmount).GreaterThan(0).WithMessage("إجمالي مبلغ الأتعاب يجب أن يكون أكبر من صفر.");
    }
}
