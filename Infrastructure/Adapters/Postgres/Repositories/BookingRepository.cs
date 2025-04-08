using Application.Ports.Postgres.Repositories;
using Domain.BookingAggregate;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Adapters.Postgres.Repositories;

public class BookingRepository(DataContext context) : IBookingRepository
{
    public async Task<Booking?> GetById(Guid id)
    {
        return await context.Bookings.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task Add(Booking booking)
    {
        await context.Bookings.AddAsync(booking);
    }

    public void Update(Booking booking)
    {
        context.Update(booking);
    }
}