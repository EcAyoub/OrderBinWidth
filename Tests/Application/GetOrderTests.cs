using Application;
using Domain;
using Infrastructure;

namespace Tests.Application;

[TestFixture]
public class GetOrderTests
{
    [Test]
    public async Task Get_Returns_Saved_Order()
    {
        var repo = new InMemoryOrderRepository();
        var calc = new BinWidthCalculator();
        var create = new CreateOrder(repo, calc);
        var getOrder = new GetOrder(repo);

        await create.Handle(new CreateOrder.Command("Order-2",
            new[] { new CreateOrder.Item(ProductType.Cards, 3) }.ToList()), default);

        var res = await getOrder.Handle("Order-2", default);

        Assert.That(res, Is.Not.Null);
        Assert.That(res!.OrderId, Is.EqualTo("Order-2"));
        Assert.That(res.RequiredBinWidth, Is.EqualTo(14.1m));
        Assert.That(res.Items[ProductType.Cards], Is.EqualTo(3));
    }

    [Test]
    public async Task Get_Unknown_Returns_Null()
    {
        var repo = new InMemoryOrderRepository();
        var getOrder = new GetOrder(repo);

        var res = await getOrder.Handle("ORDER-DOES-NOT-EXIST", default);
        Assert.That(res, Is.Null);
    }
}
