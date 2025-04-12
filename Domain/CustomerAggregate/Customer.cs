using Domain.SharedKernel.Exceptions.ArgumentException;

namespace Domain.CustomerAggregate;

public sealed class Customer
{
    private Customer()
    {
    }

    private Customer(Guid id) : this()
    {
        Id = id;
        Level = Level.Fickle;
        Rents = 0;
    }

    public Guid Id { get; private set; }
    public Level Level { get; private set; } = null!;
    public LoyaltyPoints Points => LoyaltyPoints.CreateFromRents(Rents);
    public int Rents { get; private set; }

    public void ChangeToOneLevel()
    {
        var newLevel = Level.GetNewLevelForChanging(Points);

        Level = newLevel;
    }

    public void AddRent()
    {
        Rents++;
    }

    public static Customer Create(Guid id)
    {
        if (id == Guid.Empty) throw new ValueIsRequiredException($"{nameof(id)} cannot be empty");

        return new Customer(id);
    }
}