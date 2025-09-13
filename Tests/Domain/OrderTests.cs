using Domain;

namespace Tests.Domain;

[TestFixture]
public class OrderTests
{
    private IBinWidthCalculator _calc = null!;

    [SetUp]
    public void SetUp() => _calc = new BinWidthCalculator();

    [Test]
    public void Order_Created_With_ValidData_ComputesWidth()
    {
        var items = new Dictionary<ProductType, int>
        {
            [ProductType.PhotoBook] = 1,
            [ProductType.Calendar] = 2,
            [ProductType.Mug] = 2
        };

        var order = Order.Create("Order-1", items, _calc);

        Assert.That(order.OrderId, Is.EqualTo("Order-1"));
        Assert.That(order.Items.Count, Is.EqualTo(3));
        Assert.That(order.RequiredBinWidthMm, Is.EqualTo(133m)); // 19 + 20 + 94
    }

    [Test]
    public void OrderId_Cannot_Be_Empty()
    {
        var items = new Dictionary<ProductType, int> { [ProductType.Mug] = 1 };
        Assert.Throws<ArgumentException>(() => Order.Create("", items, _calc));
        Assert.Throws<ArgumentException>(() => Order.Create("  ", items, _calc));
    }

    [Test]
    public void Order_Must_Have_AtLeast_One_Item()
    {
        var emptyItems = new Dictionary<ProductType, int>();
        Assert.Throws<ArgumentException>(() => Order.Create("Order-2", emptyItems, _calc));
    }

    [Test]
    public void Item_Quantity_Must_Be_Positive()
    {
        var items = new Dictionary<ProductType, int> { [ProductType.Canvas] = 0 };
        Assert.Throws<ArgumentException>(() => Order.Create("Order-3", items, _calc));
    }
}