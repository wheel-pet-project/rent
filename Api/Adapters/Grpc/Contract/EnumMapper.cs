using Domain.RentAggregate;
using Domain.SharedKernel.Exceptions.PublicExceptions;

namespace Api.Adapters.Grpc.Contract;

public class EnumMapper
{
    public RentStatus FromDomain(Status domainCategory)
    {
        return domainCategory switch
        {
            _ when domainCategory == Status.InProgress => RentStatus.InProgressUnspecified,
            _ when domainCategory == Status.Completed => RentStatus.Completed,
            _ => throw new ValueIsUnsupportedException($"{nameof(domainCategory)} is unknown category")
        };
    }

    public Status FromProto(RentStatus protoCategory)
    {
        return protoCategory switch
        {
            RentStatus.InProgressUnspecified => Status.InProgress,
            RentStatus.Completed => Status.Completed,
            _ => throw new ValueIsUnsupportedException($"{nameof(protoCategory)} is unknown category")
        };
    }
}