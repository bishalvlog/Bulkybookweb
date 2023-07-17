namespace BulkyBook.DataAccess.Repository.IRepository
{
    public interface IUnitOfWork
    {
        ICategoryRepository Category{ get; }
        ICoverTypesRepository CoverType { get; }
        IProductRepository Product { get; }
        ICompanyRepository Company { get; }
        IApplicationUserRepositor ApplicationUser { get; }
        IShoppingCardRepository ShoppingCard { get; }



        void Save();

       
    }
}
