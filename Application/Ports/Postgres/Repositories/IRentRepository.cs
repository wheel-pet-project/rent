using Domain.RentAggregate;

namespace Application.Ports.Postgres.Repositories;

public interface IRentRepository
{
    Task<Rent?> GetById(Guid id);

    Task Add(Rent rent);

    public void Update(Rent rent);
}