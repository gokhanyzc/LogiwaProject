namespace Logiwa.Categories
{
    public static class CategoryConsts
    {
        private const string DefaultSorting = "{0}CategoryName asc";
        public const string CodePrefix = "CAT";

        public static string GetDefaultSorting(bool withEntityName)
        {
            return string.Format(DefaultSorting, withEntityName ? "Category." : string.Empty);
        }
    }
}