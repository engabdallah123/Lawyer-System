using App.Domain;
using App.Domain.Consultations.Errors;
using Shared.Application.Messaging;
using Shared.Domain;

namespace App.Application.Consultations.Commands.UpdateConsultation;

internal sealed class UpdateConsultationCommandHandler : ICommandHandler<UpdateConsultationCommand>
{
    private readonly IAppUnitOfWork _unitOfWork;

    public UpdateConsultationCommandHandler(IAppUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdateConsultationCommand request, CancellationToken cancellationToken)
    {
        var consultation = await _unitOfWork.Consultations.GetByIdAsync(request.Id, cancellationToken);
        if (consultation is null)
            return Result.Failure(ConsultationErrors.NotFound(request.Id));

        var updateResult = consultation.Update(
            request.ConsultationDate,
            request.Subject,
            request.Description,
            request.Fee,
            request.Notes);

        if (updateResult.IsFailure)
            return updateResult;

        consultation.SetUpdated(request.UpdatedBy);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
