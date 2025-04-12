using Api.Adapters.Grpc.Contract;
using Application.UseCases.Commands.Rent.CompleteRent;
using Application.UseCases.Commands.Rent.StartRent;
using Application.UseCases.Queries.Rent.GetAllCurrentRents;
using Application.UseCases.Queries.Rent.GetCurrentAmountRent;
using Application.UseCases.Queries.Rent.GetRentById;
using Domain.SharedKernel.Errors;
using Domain.SharedKernel.Exceptions.ArgumentException;
using FluentResults;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using MediatR;

namespace Api.Adapters.Grpc;

public class RentV1(IMediator mediator, EnumMapper enumMapper) : Rent.RentBase
{
    public override async Task<StartRentResponse> StartRent(StartRentRequest request, ServerCallContext context)
    {
        var response = await mediator.Send(new StartRentCommand(
            ParseGuidOrThrow(request.CustomerId),
            ParseGuidOrThrow(request.BookingId), 
            ParseGuidOrThrow(request.VehicleId)));

        return response.IsSuccess
            ? new StartRentResponse { RentId = response.Value.RentId.ToString() }
            : ParseErrorToRpcException<StartRentResponse>(response.Errors);
    }

    public override async Task<CompleteRentResponse> CompleteRent(CompleteRentRequest request, ServerCallContext context)
    {
        var response = await mediator.Send(new CompleteRentCommand(ParseGuidOrThrow(request.RentId)));
        
        return response.IsSuccess
            ? new CompleteRentResponse { ActualAmount = ParseToDoubleAndRound(response.Value.ActualAmount) }
            : ParseErrorToRpcException<CompleteRentResponse>(response.Errors);
    }

    public override async Task<GetAllCurrentRentsResponse> GetAllCurrentRents(GetAllCurrentRentsRequest request, ServerCallContext context)
    {
        var response = await mediator.Send(new GetAllCurrentRentsQuery(request.Page, request.PageSize));
        
        if (response.IsFailed) return ParseErrorToRpcException<GetAllCurrentRentsResponse>(response.Errors);
        
        var currentRents = new GetAllCurrentRentsResponse();
        currentRents.Rents.AddRange(response.Value.Rents.Select(x => new GetAllCurrentRentsResponse.Types.CurrentRent
        {
            RentId = x.RentId.ToString(), 
            CustomerId = x.CustomerId.ToString(), 
            VehicleId = x.VehicleId.ToString(),
            Start = x.Start.ToTimestamp()
        }));
        
        return currentRents;
    }

    public override async Task<GetRentByIdResponse> GetRentById(GetRentByIdRequest request, ServerCallContext context)
    {
        var response = await mediator.Send(new GetRentByIdQuery(ParseGuidOrThrow(request.RentId)));

        return response.IsSuccess
            ? new GetRentByIdResponse
            {
                RentId = response.Value.RentId.ToString(),
                Status = enumMapper.FromDomain(response.Value.Status),
                BookingId = response.Value.BookingId.ToString(),
                VehicleId = response.Value.VehicleId.ToString(),
                Start = response.Value.Start.ToTimestamp(),
                End = response.Value.End?.ToTimestamp(),
                ActualAmount = ParseToDoubleAndRound(response.Value.ActualAmount.GetValueOrDefault())
            }
            : ParseErrorToRpcException<GetRentByIdResponse>(response.Errors);
    }

    public override async Task<GetCurrentAmountRentResponse> GetCurrentAmountRent(GetCurrentAmountRentRequest request, ServerCallContext context)
    {
        var response = await mediator.Send(new GetCurrentAmountRentQuery(ParseGuidOrThrow(request.RentId)));

        return response.IsSuccess
            ? new GetCurrentAmountRentResponse
            {
                RentId = response.Value.RentId.ToString(),
                CurrentAmount = ParseToDoubleAndRound(response.Value.CurrentAmount)
            }
            : ParseErrorToRpcException<GetCurrentAmountRentResponse>(response.Errors);
    }

    private double ParseToDoubleAndRound(decimal value)
    {
        return Math.Round((double)value, 2);
    }
    
    private T ParseErrorToRpcException<T>(List<IError> errors)
    {
        if (errors.Exists(x => x is NotFound))
            throw new RpcException(new Status(StatusCode.NotFound, string.Join(' ', errors.Select(x => x.Message))));

        if (errors.Exists(x => x is CommitFail))
            throw new RpcException(new Status(StatusCode.Unavailable, string.Join(' ', errors.Select(x => x.Message))));

        throw new RpcException(new Status(StatusCode.InvalidArgument, string.Join(' ', errors.Select(x => x.Message))));
    }

    

    private Guid ParseGuidOrThrow(string potentialId)
    {
        return Guid.TryParse(potentialId, out var id)
            ? id
            : throw new ValueOutOfRangeException($"{nameof(potentialId)} is invalid uuid");
    }
}