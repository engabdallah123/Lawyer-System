using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace App.Application.Documents.Commands.CreateDocument
{
    public class CreateDocumentCommandValidator : AbstractValidator<CreateDocumentCommand>
    {
        public CreateDocumentCommandValidator()
        {
            RuleFor(d => d.Name).NotEmpty().WithMessage("اسم المستند مطلوب.");
            RuleFor(d => d.FilePath).NotEmpty().WithMessage("مسار الملف مطلوب.");
            RuleFor(d => d.FileName).NotEmpty().WithMessage("اسم الملف مطلوب.");
        }
    }

}
