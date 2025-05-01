using CSharpFunctionalExtensions;
using Domain.SharedKernel.Exceptions.PublicExceptions;

namespace Domain.SharedKernel.ValueObjects;

public sealed class Tariff : ValueObject
{
    private Tariff()
    {
    }

    private Tariff(decimal pricePerMinute, decimal pricePerHour, decimal pricePerDay) : this()
    {
        PricePerMinute = pricePerMinute;
        PricePerHour = pricePerHour;
        PricePerDay = pricePerDay;
    }

    public decimal PricePerMinute { get; private set; }
    public decimal PricePerHour { get; private set; }
    public decimal PricePerDay { get; private set; }


    public static Tariff With(Tariff tariff)
    {
        if (tariff == null) throw new ValueIsRequiredException($"{nameof(tariff)} cannot be null");

        return new Tariff(tariff.PricePerMinute, tariff.PricePerHour, tariff.PricePerDay);
    }

    public static Tariff Create(decimal pricePerMinute, decimal pricePerHour, decimal pricePerDay)
    {
        if (pricePerMinute <= 0)
            throw new ValueIsUnsupportedException($"{nameof(pricePerMinute)} must be greater than zero");
        if (pricePerHour <= 0)
            throw new ValueIsUnsupportedException($"{nameof(pricePerHour)} must be greater than zero");
        if (pricePerDay <= 0)
            throw new ValueIsUnsupportedException($"{nameof(pricePerDay)} must be greater than zero");

        return new Tariff(pricePerMinute, pricePerHour, pricePerDay);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return PricePerMinute;
        yield return PricePerHour;
        yield return PricePerDay;
    }
}