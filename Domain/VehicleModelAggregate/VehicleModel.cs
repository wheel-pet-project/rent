using Domain.SharedKernel.Exceptions.ArgumentException;
using Domain.SharedKernel.ValueObjects;

namespace Domain.VehicleModelAggregate;

public class VehicleModel
{
    private VehicleModel() { }

    private VehicleModel(Guid id, Category category, Tariff tariff) : this()
    {
        Id = id;
        Category = category;
        Tariff = tariff;
    }
    
    public Guid Id { get; private set; }
    public Category Category { get; private set; } = null!;
    public Tariff Tariff { get; private set; } = null!;

    public void ChangeTariff(Tariff potentialTariff)
    {
        if (potentialTariff == null) throw new ValueIsRequiredException($"{nameof(potentialTariff)} cannot be null");
        
        Tariff = potentialTariff;
    }
    public static VehicleModel Create(Guid id, Category category, Tariff tariff)
    {
        if (id == Guid.Empty) throw new ValueIsRequiredException($"{nameof(id)} cannot be empty");
        if (category == null) throw new ValueIsRequiredException($"{nameof(category)} cannot be null");
        if (tariff == null) throw new ValueIsRequiredException($"{nameof(tariff)} cannot be null");
        
        return new VehicleModel(id, category, tariff);
    }
}