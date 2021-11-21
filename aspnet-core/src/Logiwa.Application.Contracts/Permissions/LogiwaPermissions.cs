namespace Logiwa.Permissions
{
    public static class LogiwaPermissions
    {
        public const string GroupName = "Logiwa";

        public static class Dashboard
        {
            public const string DashboardGroup = GroupName + ".Dashboard";
            public const string Host = DashboardGroup + ".Host";
            public const string Tenant = DashboardGroup + ".Tenant";
        }
        
        public class Products
        {
            public const string Default = GroupName + ".Products";
            public const string Edit = Default + ".Edit";
            public const string Create = Default + ".Create";
            public const string Delete = Default + ".Delete";
            public const string HardDelete = Default + ".HardDelete";
            public const string Read = Default + ".Read";
            
        }
        public class Categories
        {
            public const string Default = GroupName + ".Categories";
            public const string Edit = Default + ".Edit";
            public const string Create = Default + ".Create";
            public const string Delete = Default + ".Delete";
            public const string HardDelete = Default + ".HardDelete";
            public const string Read = Default + ".Read";
            
        }


        //Add your own permission names. Example:
        //public const string MyPermission1 = GroupName + ".MyPermission1";
    }
}
