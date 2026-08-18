using App.Domain.Clients.Enums;
using FluentValidation;

namespace App.Application.Clients.Commands.CreateClient;

public class CreateClientCommandValidator : AbstractValidator<CreateClientCommand>
{
    public CreateClientCommandValidator()
    {
        RuleFor(c => c.Phone)
            .NotEmpty().WithMessage("رقم الهاتف الأساسي مطلوب.")
            .MaximumLength(20).WithMessage("رقم الهاتف لا يجب أن يتجاوز 20 حرفًا.");

        When(c => c.ClientType == ClientType.Individual, () =>
        {
            RuleFor(c => c.FullName)
                .NotEmpty().WithMessage("اسم الموكل مطلوب للشخص الطبيعي.")
                .MaximumLength(200).WithMessage("اسم الموكل لا يجب أن يتجاوز 200 حرف.");
        });

        When(c => c.ClientType == ClientType.Company, () =>
        {
            RuleFor(c => c.CompanyName)
                .NotEmpty().WithMessage("اسم الشركة مطلوب.")
                .MaximumLength(300).WithMessage("اسم الشركة لا يجب أن يتجاوز 300 حرف.");
        });

        RuleFor(c => c.Email)
            .EmailAddress().WithMessage("البريد الإلكتروني غير صحيح.")
            .When(c => !string.IsNullOrWhiteSpace(c.Email));
    }
}
