using Application;
using Domain;
using Infrastructure;
using Infrastructure.Persistence;

namespace Tests.Integration;

[TestFixture]
public class CreateOrderEfTests
{
    private AppDbContext _db = default!;
    private EfOrderRepository _repo = default!;
    private BinWidthCalculator _calc = default!;
    private CreateOrder _usecase = default!;

    [SetUp]
    public void SetUp()
    {
        _db = TestDb.Create();
        TestDb.Clear(_db);

        _repo = new EfOrderRepository(_db);
        _calc = new BinWidthCalculator();
        _usecase = new CreateOrder(_repo, _calc);
    }

    [TearDown]
    public void TearDown()
    {
        _db.Dispose();
    }

    [Test]
    public async Task CreatesOrder_And_Calculates_RequiredBinWidth()
    {
        var cmd = new CreateOrder.Command("Order-1", new[]
        {
            new CreateOrder.Item(ProductType.PhotoBook, 1),
            new CreateOrder.Item(ProductType.Calendar,  2),
            new CreateOrder.Item(ProductType.Mug,       2),
        }.ToList());

        var res = await _usecase.Handle(cmd, default);

        Assert.That(res.OrderId, Is.EqualTo("Order-1"));
        Assert.That(res.RequiredBinWidth, Is.EqualTo(133m));
        Assert.That(res.Items[ProductType.Calendar], Is.EqualTo(2));

        var saved = await _repo.GetAsync("Order-1", default);
        Assert.That(saved!.OrderId, Is.EqualTo("Order-1"));
        Assert.That(saved.RequiredBinWidthMm, Is.EqualTo(133m));
        Assert.That(saved.Items.Single(i => i.Key == ProductType.Mug).Value, Is.EqualTo(2));
    }

    [Test]
    public async Task Creates_With5Mugs_And_Computes_Width()
    {
        var cmd = new CreateOrder.Command("Order-1", new[]
        {
            new CreateOrder.Item(ProductType.PhotoBook, 1),
            new CreateOrder.Item(ProductType.Calendar,  2),
            new CreateOrder.Item(ProductType.Mug,       5),
        }.ToList());

        var res = await _usecase.Handle(cmd, default);

        Assert.That(res.RequiredBinWidth, Is.EqualTo(227m));
        Assert.That(res.Items[ProductType.Calendar], Is.EqualTo(2));
    }

    [Test]
    public async Task Duplicate_OrderId_Throws()
    {
        var first = new CreateOrder.Command("Order-1",
            new[] { new CreateOrder.Item(ProductType.Mug, 1) }.ToList());
        await _usecase.Handle(first, default);

        var duplicate = new CreateOrder.Command("Order-1",
            new[] { new CreateOrder.Item(ProductType.Mug, 1) }.ToList());

        var ex = Assert.ThrowsAsync<InvalidOperationException>(() => _usecase.Handle(duplicate, default));
        Assert.That(ex!.Message, Does.Contain("already exists"));
    }
}
