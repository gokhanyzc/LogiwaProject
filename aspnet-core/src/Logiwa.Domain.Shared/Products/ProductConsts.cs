namespace Logiwa.Products
{
    public static class ProductConsts
    {
        private const string DefaultSorting = "{0}Title asc";
        public const string CodePrefix = "PRO";

        public static string GetDefaultSorting(bool withEntityName)
        {
            return string.Format(DefaultSorting, withEntityName ? "Product." : string.Empty);
        }
    }
}