using Phronesis.Domain.Common;

namespace Phronesis.Domain.Commerce;

public enum BillingInterval
{
    Monthly,
    Annual
}

public class SubscriptionPlan : BaseEntity
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public string PlanCode { get; private set; }
    public decimal Price { get; private set; }
    public string Currency { get; private set; }
    public BillingInterval Interval { get; private set; }
    public bool IsActive { get; private set; }

    // Storing features as a serialized JSON array for flexibility in MVP
    public string FeaturesJson { get; private set; }

    private SubscriptionPlan() { }

    public SubscriptionPlan(
        string name, 
        string description, 
        string planCode, 
        decimal price, 
        string currency, 
        BillingInterval interval,
        string featuresJson)
    {
        Name = name;
        Description = description;
        PlanCode = planCode;
        Price = price;
        Currency = currency;
        Interval = interval;
        FeaturesJson = featuresJson;
        IsActive = true;
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public void UpdateDetails(string name, string description, decimal price, string featuresJson)
    {
        Name = name;
        Description = description;
        Price = price;
        FeaturesJson = featuresJson;
    }
}
