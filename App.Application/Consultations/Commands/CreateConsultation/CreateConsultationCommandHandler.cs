using App.Domain;
using App.Domain.Clients.Errors;
using App.Domain.Consultations.Entities;
using Shared.Application.Messaging;
using Shared.Domain;

namespace App.Application.Consultations.Commands.CreateConsultation;

internal sealed class CreateConsultationCommandHandler : ICommandHandler<CreateConsultationCommand, Guid>
{
    private readonly IAppUnitOfWork _unitOfWork;

    public CreateConsultationCommandHandler(IAppUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreateConsultationCommand request, CancellationToken cancellationToken)
    {
        var clientExists = await _unitOfWork.Clients.AnyAsync(c => c.Id == request.ClientId, cancellationToken);
        if (!clientExists)
            return Result<Guid>.Failure(ClientErrors.NotFound(request.ClientId));

        var consultationResult = Consultation.Create(
            request.ClientId,
            request.ConsultationDate,
            request.Subject,
            request.Description,
            request.Fee,
            request.Notes);

        if (consultationResult.IsFailure)
            return Result<Guid>.Failure(consultationResult.Error);

        var consultation = consultationResult.Value!;
        consultation.SetCreated(request.CreatedBy);

        await _unitOfWork.Consultations.AddAsync(consultation, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(consultation.Id);
    }
}
