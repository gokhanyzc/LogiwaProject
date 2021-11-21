using Logiwa.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;
using Volo.Abp.MultiTenancy;

namespace Logiwa.Permissions
{
    public class LogiwaPermissionDefinitionProvider : PermissionDefinitionProvider
    {
        public override void Define(IPermissionDefinitionContext context)
        {
            var myGroup = context.AddGroup(LogiwaPermissions.GroupName);

            myGroup.AddPermission(LogiwaPermissions.Dashboard.Host, L("Permission:Dashboard"), MultiTenancySides.Host);
            myGroup.AddPermission(LogiwaPermissions.Dashboard.Tenant, L("Permission:Dashboard"), MultiTenancySides.Tenant);

            //Define your own permissions here. Example:
            //myGroup.AddPermission(LogiwaPermissions.MyPermission1, L("Permission:MyPermission1"));
            
            
            var productPermission = myGroup.AddPermission(LogiwaPermissions.Products.Default, L("Permission:Products"));
            productPermission.AddChild(LogiwaPermissions.Products.Create, L("Permission:Create"));
            productPermission.AddChild(LogiwaPermissions.Products.Edit, L("Permission:Edit"));
            productPermission.AddChild(LogiwaPermissions.Products.Delete, L("Permission:Delete"));
            productPermission.AddChild(LogiwaPermissions.Products.Read, L("Permission:Read"));
            productPermission.AddChild(LogiwaPermissions.Products.HardDelete, L("Permission:HardDelete"));
            
            var categoryPermission = myGroup.AddPermission(LogiwaPermissions.Categories.Default, L("Permission:Categories"));
            categoryPermission.AddChild(LogiwaPermissions.Categories.Create, L("Permission:Create"));
            categoryPermission.AddChild(LogiwaPermissions.Categories.Edit, L("Permission:Edit"));
            categoryPermission.AddChild(LogiwaPermissions.Categories.Delete, L("Permission:Delete"));
            categoryPermission.AddChild(LogiwaPermissions.Categories.Read, L("Permission:Read"));
            categoryPermission.AddChild(LogiwaPermissions.Categories.HardDelete, L("Permission:HardDelete"));
            
        }
        
        private static LocalizableString L(string name)
        {
            return LocalizableString.Create<LogiwaResource>(name);
        }
    }
}