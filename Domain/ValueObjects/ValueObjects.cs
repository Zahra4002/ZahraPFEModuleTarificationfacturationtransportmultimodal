namespace Domain.ValueObjects;

/// <summary>
/// Value Object représentant une somme d'argent avec devise
/// </summary>
public record Money
{
    public decimal Amount { get; init; }
    public string CurrencyCode { get; init; } = "EUR";

    public Money(decimal amount, string currencyCode = "EUR")
    {
        if (amount < 0)
            throw new ArgumentException("Le montant ne peut pas être négatif", nameof(amount));
        
        Amount = amount;
        CurrencyCode = currencyCode?.ToUpper() ?? "EUR";
    }

    public static Money Zero(string currencyCode = "EUR") => new(0, currencyCode);

    public Money Add(Money other)
    {
        if (CurrencyCode != other.CurrencyCode)
            throw new InvalidOperationException("Impossible d'additionner des montants de devises différentes");
        
        return new Money(Amount + other.Amount, CurrencyCode);
    }

    public Money Subtract(Money other)
    {
        if (CurrencyCode != other.CurrencyCode)
            throw new InvalidOperationException("Impossible de soustraire des montants de devises différentes");
        
        return new Money(Amount - other.Amount, CurrencyCode);
    }

    public Money Multiply(decimal factor) => new(Amount * factor, CurrencyCode);

    public override string ToString() => $"{Amount:N2} {CurrencyCode}";
}

/// <summary>
/// Value Object représentant un poids
/// </summary>
public record Weight
{
    public decimal Value { get; init; }
    public WeightUnit Unit { get; init; }

    public Weight(decimal value, WeightUnit unit = WeightUnit.Kg)
    {
        if (value < 0)
            throw new ArgumentException("Le poids ne peut pas être négatif", nameof(value));
        
        Value = value;
        Unit = unit;
    }

    public decimal ToKg() => Unit switch
    {
        WeightUnit.Kg => Value,
        WeightUnit.Ton => Value * 1000,
        WeightUnit.Lb => Value * 0.453592m,
        _ => Value
    };

    public decimal ToTon() => ToKg() / 1000;

    public override string ToString() => $"{Value:N2} {Unit}";
}

public enum WeightUnit
{
    Kg,
    Ton,
    Lb
}

/// <summary>
/// Value Object représentant un volume
/// </summary>
public record Volume
{
    public decimal Value { get; init; }
    public VolumeUnit Unit { get; init; }

    public Volume(decimal value, VolumeUnit unit = VolumeUnit.M3)
    {
        if (value < 0)
            throw new ArgumentException("Le volume ne peut pas être négatif", nameof(value));
        
        Value = value;
        Unit = unit;
    }

    public decimal ToM3() => Unit switch
    {
        VolumeUnit.M3 => Value,
        VolumeUnit.Litre => Value / 1000,
        VolumeUnit.CBM => Value,
        _ => Value
    };

    public override string ToString() => $"{Value:N2} {Unit}";
}

public enum VolumeUnit
{
    M3,
    Litre,
    CBM
}

/// <summary>
/// Value Object représentant une adresse
/// </summary>
public record Address
{
    public string Street { get; init; } = string.Empty;
    public string City { get; init; } = string.Empty;
    public string PostalCode { get; init; } = string.Empty;
    public string Country { get; init; } = string.Empty;
    public string? State { get; init; } = string.Empty;
    public double? Latitude { get; init; } 
    public double? Longitude { get; init; }

    public Address() { }

    public Address(string street, string city, string postalCode, string country)
    {
        Street = street;
        City = city;
        PostalCode = postalCode;
        Country = country;
    }

    public string FullAddress => $"{Street}, {PostalCode} {City}, {Country}";

    public override string ToString() => FullAddress;
}

/// <summary>
/// Value Object représentant une période de dates
/// </summary>
public record DateRange
{
    public DateTime StartDate { get; init; }
    public DateTime EndDate { get; init; }

    public DateRange(DateTime startDate, DateTime endDate)
    {
        if (endDate < startDate)
            throw new ArgumentException("La date de fin doit être postérieure à la date de début");
        
        StartDate = startDate;
        EndDate = endDate;
    }

    public bool Contains(DateTime date) => date >= StartDate && date <= EndDate;
    
    public bool Overlaps(DateRange other) => 
        StartDate <= other.EndDate && EndDate >= other.StartDate;

    public int TotalDays => (EndDate - StartDate).Days;
}
