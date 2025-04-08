using Application.Ports.Postgres.Repositories;
using Domain.RentAggregate;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Adapters.Postgres.Repositories;

public class RentRepository(DataContext context) : IRentRepository
{
    public async Task<Rent?> GetById(Guid id)
    {
        return await context.Rents.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task Add(Rent rent)
    {
        await context.Rents.AddAsync(rent);
    }

    public void Update(Rent rent)
    {
        context.Update(rent);
    }
}