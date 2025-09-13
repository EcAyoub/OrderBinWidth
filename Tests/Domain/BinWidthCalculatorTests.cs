using Domain;

namespace Tests.Domain;

[TestFixture]
public class BinWidthCalculatorTests
{
    private IBinWidthCalculator _calc = null!;

    [SetUp]
    public void SetUp() => _calc = new BinWidthCalculator();

    [Test]
    public void TwoOrFourMugs_SameWidth()
    {
        var two = _calc.Calculate(new Dictionary<ProductType, int> { [ProductType.Mug] = 2 });
        var four = _calc.Calculate(new Dictionary<ProductType, int> { [ProductType.Mug] = 4 });
        Assert.That(two, Is.EqualTo(94m));
        Assert.That(four, Is.EqualTo(94m));
    }

    [Test]
    public void FifthMug_Adds_New_Stack()
    {
        var five = _calc.Calculate(new Dictionary<ProductType, int> { [ProductType.Mug] = 5 });
        Assert.That(five, Is.EqualTo(188m));
    }

    [Test]
    public void MixedExample_Is_133()
    {
        var items = new Dictionary<ProductType, int>
        {
            [ProductType.PhotoBook] = 1, // 19
            [ProductType.Calendar] = 2, // 20
            [ProductType.Mug] = 2  // 94 (1 pour max 4 Mug)
        };
        Assert.That(_calc.Calculate(items), Is.EqualTo(133m));
    }

    [Test]
    public void NegativeQuantity_Throws()
    {
        Assert.Throws<ArgumentException>(() =>
            _calc.Calculate(new Dictionary<ProductType, int> { [ProductType.Canvas] = -1 }));
    }
}
