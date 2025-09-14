using Application;
using Domain;
using Infrastructure;

namespace Tests.Application;

[TestFixture]
public class CreateOrderTests
{
    [Test]
    public async Task CreatesOrder_And_Calculates_RequiredBinWidth()
    {
        var repo = new InMemoryOrderRepository();
        var calc = new BinWidthCalculator();
        var createOrder = new CreateOrder(repo, calc);

        var cmd = new CreateOrder.Command("Order-1", new[]
        {
            new CreateOrder.Item(ProductType.PhotoBook, 1),
            new CreateOrder.Item(ProductType.Calendar,  2),
            new CreateOrder.Item(ProductType.Mug,       2),
        }.ToList());

        var res = await createOrder.Handle(cmd, default);

        Assert.That(res.OrderId, Is.EqualTo("Order-1"));
        Assert.That(res.RequiredBinWidth, Is.EqualTo(133m));
        Assert.That(res.Items[ProductType.Calendar], Is.EqualTo(2));
    }

    [Test]
    public async Task Creates_With5Mugs_And_Computes_Width()
    {
        var repo = new InMemoryOrderRepository();
        var calc = new BinWidthCalculator();
        var createOrder = new CreateOrder(repo, calc);

        var cmd = new CreateOrder.Command("Order-1", new[]
        {
            new CreateOrder.Item(ProductType.PhotoBook, 1),
            new CreateOrder.Item(ProductType.Calendar,  2),
            new CreateOrder.Item(ProductType.Mug,       5),
        }.ToList());

        var res = await createOrder.Handle(cmd, default);

        Assert.That(res.OrderId, Is.EqualTo("Order-1"));
        Assert.That(res.RequiredBinWidth, Is.EqualTo(227m));
        Assert.That(res.Items[ProductType.Calendar], Is.EqualTo(2));
    }

    [Test]
    public void Duplicate_OrderId_Throws()
    {
        var repo = new InMemoryOrderRepository();
        var calc = new BinWidthCalculator();
        var createOrder = new CreateOrder(repo, calc);

        var first = new CreateOrder.Command("Order-1",
            new[] { new CreateOrder.Item(ProductType.Mug, 1) }.ToList());

        Assert.DoesNotThrowAsync(() => createOrder.Handle(first, default));

        var duplicate = new CreateOrder.Command("Order-1",
            new[] { new CreateOrder.Item(ProductType.Mug, 1) }.ToList());

        var ex = Assert.ThrowsAsync<InvalidOperationException>(() => createOrder.Handle(duplicate, default));
        Assert.That(ex!.Message, Does.Contain("already exists"));
    }
}
