using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace App.Application.Tasks.Commands.CreateLegalTask
{
    public class CreateLegalTaskCommandValidator : AbstractValidator<CreateLegalTaskCommand>
    {
        public CreateLegalTaskCommandValidator()
        {
            RuleFor(t => t.Title).NotEmpty().WithMessage("عنوان المهمة مطلوب.");
            RuleFor(t => t.AssignedToUserId).NotEmpty().WithMessage("يجب تحديد المستخدم المسند إليه المهمة.");
        }
    }
}
