namespace SmartPOS.Shared.Enums;

public enum DiscountType
{
    Percentage = 0,
    Flat = 1
}

public enum SaleType
{
    Onsite = 0,
    Online = 1
}

public enum SaleStatus
{
    Completed = 0,
    Voided = 1,
    Refunded = 2
}

public enum POStatus
{
    Draft = 0,
    Sent = 1,
    Received = 2,
    Cancelled = 3
}

public enum PaymentMethod
{
    Cash = 0,
    Card = 1,
    Online = 2
}

public enum PaymentStatus
{
    Pending = 0,
    Completed = 1,
    Failed = 2,
    Refunded = 3
}
