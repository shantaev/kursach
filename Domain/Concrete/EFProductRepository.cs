using System.Linq;
using Domain.Abstract;
using Domain.Entities;

namespace Domain.Concrete
{
    public class EFProductRepository: IProductRepository
    {
        private EFDbContext context = new EFDbContext();
        public IQueryable<Product> Products 
        {
            get { return context.Products; } 
        }

        public void SaveProduct(Product product)
        {
            if(product.ProductID == 0)
            {
                context.Products.Add(product);
            }
            else
            {
                context.Entry(product).State = System.Data.EntityState.Modified;
            }

            int result = context.SaveChanges();
        }

        public void DeleteProduct(Product product)
        {
            context.Products.Remove(product);

            context.SaveChanges();
        }
    }
}
