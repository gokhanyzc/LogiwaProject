#region 

using System;
using System.Text.RegularExpressions;

#endregion


namespace Logiwa
{
    public static class DtoExtensions
    {
        
        public static string TrimProperty(this string property)
        {
            if (!property.IsNullOrEmpty())
            {
                property= property.Trim();
                property = Regex.Replace(property, @"\s+", " ");
            }
            return property;
        }
    }
}