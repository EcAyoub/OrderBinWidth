namespace Domain;
public interface IBinWidthCalculator
{
    decimal Calculate(Dictionary<ProductType, int> items);
}