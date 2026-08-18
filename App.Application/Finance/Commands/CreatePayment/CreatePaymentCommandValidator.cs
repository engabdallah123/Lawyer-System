using FluentValidation;

namespace App.Application.Finance.Commands.CreatePayment;

public class CreatePaymentCommandValidator : AbstractValidator<CreatePaymentCommand>
{
    public CreatePaymentCommandValidator()
    {
        RuleFor(p => p.ClientId).NotEmpty().WithMessage("معرف الموكل مطلوب.");
        RuleFor(p => p.Amount).GreaterThan(0).WithMessage("مبلغ الدفعة يجب أن يكون أكبر من صفر.");
        RuleFor(p => p.PaymentMethod).NotEmpty().WithMessage("طريقة الدفع مطلوبة.");
    }
}
