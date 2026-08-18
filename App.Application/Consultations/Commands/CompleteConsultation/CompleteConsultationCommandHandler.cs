using App.Domain;
using App.Domain.Consultations.Errors;
using Shared.Application.Messaging;
using Shared.Domain;

namespace App.Application.Consultations.Commands.CompleteConsultation;

internal sealed class CompleteConsultationCommandHandler : ICommandHandler<CompleteConsultationCommand>
{
    private readonly IAppUnitOfWork _unitOfWork;

    public CompleteConsultationCommandHandler(IAppUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(CompleteConsultationCommand request, CancellationToken cancellationToken)
    {
        var consultation = await _unitOfWork.Consultations.GetByIdAsync(request.Id, cancellationToken);
        if (consultation is null)
            return Result.Failure(ConsultationErrors.NotFound(request.Id));

        consultation.Complete();
        consultation.SetUpdated(request.UpdatedBy);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
