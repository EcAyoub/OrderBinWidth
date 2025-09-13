namespace Domain;

public class BinWidthCalculator : IBinWidthCalculator
{
    public decimal Calculate(Dictionary<ProductType, int> items)
    {
        if (items is null || items.Count == 0) return 0m;

        decimal totalWidth = 0m;

        foreach (var (productType, quantity) in items)
        {
            if (quantity <= 0)
                throw new ArgumentException($"Quantity for {productType} must be > 0.");

            totalWidth += productType switch
            {
                ProductType.Mug => ((quantity + 3) / 4) * 94m,  // empilement par 4
                ProductType.PhotoBook => quantity * 19m,
                ProductType.Calendar => quantity * 10m,
                ProductType.Canvas => quantity * 16m,
                ProductType.Cards => quantity * 4.7m,
                _ => throw new ArgumentOutOfRangeException(nameof(productType), productType, "Unknown product type")
            };
        }

        return totalWidth;
    }
}