using Domain.SharedKernel.Exceptions.ArgumentException;
using Domain.SharedKernel.ValueObjects;

namespace Domain.VehicleModelAggregate;

public class VehicleModel
{
    private VehicleModel() { }

    private VehicleModel(Guid id, Tariff tariff) : this()
    {
        Id = id;
        Tariff = tariff;
    }
    
    public Guid Id { get; private set; }
    public Tariff Tariff { get; private set; } = null!;

    public void ChangeTariff(Tariff potentialTariff)
    {
        if (potentialTariff == null) throw new ValueIsRequiredException($"{nameof(potentialTariff)} cannot be null");
        
        Tariff = potentialTariff;
    }
    public static VehicleModel Create(Guid id, Tariff tariff)
    {
        if (id == Guid.Empty) throw new ValueIsRequiredException($"{nameof(id)} cannot be empty");
        if (tariff == null) throw new ValueIsRequiredException($"{nameof(tariff)} cannot be null");
        
        return new VehicleModel(id, tariff);
    }
}