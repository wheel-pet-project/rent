using Domain.RentAggregate;

namespace Application.Ports.Postgres.Repositories;

public interface IRentRepository
{
    Task<Rent?> GetById(Guid id);

    Task<Rent?> GetByBookingId(Guid bookingId);

    Task Add(Rent rent);

    public void Update(Rent rent);
}