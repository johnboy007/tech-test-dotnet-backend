using System.Linq;

namespace Moonpig.PostOffice.Data;

public interface IRepository
{
    Product GetProduct(int productId);
    Supplier GetSupplier(int supplierId);
}

public class Repository(IDbContext dbContext) : IRepository
{
    public Product GetProduct(int productId)
    {
        return dbContext.Products.Single(product => product.ProductId == productId);
    }

    public Supplier GetSupplier(int supplierId)
    {
        return dbContext.Suppliers.Single(supplier => supplier.SupplierId == supplierId);
    }
}