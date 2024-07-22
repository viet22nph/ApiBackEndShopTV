using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Models.Settings
{
    public static class Permissions
    {

        public static class Category
        {
            public const string Create = "Permissions.Category.Create";
            public const string Read = "Permissions.Category.Read";
            public const string Update = "Permissions.Category.Update";
            public const string Delete = "Permissions.Category.Delete";
        }

        public static class Supplier
        {
            public const string Create = "Permissions.Supplier.Create";
            public const string Read = "Permissions.Supplier.Read";
            public const string Update = "Permissions.Supplier.Update";
            public const string Delete = "Permissions.Supplier.Delete";
        }
        public static class Product
        {
            public const string Create = "Permissions.Product.Create";
            public const string Read = "Permissions.Product.Read";
            public const string Update = "Permissions.Product.Update";
           // public const string Delete = "Permissions.Product.Delete";
        }
        public static class UserManager
        {
            public const string Create = "Permissions.User.Create";
            public const string Read = "Permissions.User.Read";
            public const string Update = "Permissions.User.Update";
            public const string Delete = "Permissions.User.Delete";
        }
        public static class RoleManager
        {
            public const string Create = "Permissions.RoleManager.Create";
            public const string Read = "Permissions.RoleManager.Read";
            public const string Update = "Permissions.RoleManager.Update";
            public const string Delete = "Permissions.RoleManager.Delete";
        }
        public static class Discount
        {
            public const string Create = "Permissions.Discount.Create";
            public const string Read = "Permissions.Discount.Read";
            public const string Update = "Permissions.Discount.Update";
            public const string Delete = "Permissions.Discount.Delete";
        }
        public static class Order
        {

        }
        public static class Report
        {

        }
        public static class ContactUs
        {

        }
        public static class ManagerBlog
        {
            public const string Manager = "Permissions.ManagerBlog";
        }
        public static class ManagerBanner
        {

            public const string Manager = "Permissions.ManagerBanner";
        }
        public static IEnumerable<string> GetAllPermissions()
        {
            var permissionTypes = new[]
            {
                typeof(UserManager),
                typeof(Category),
                typeof(Supplier),
                typeof(Product),
                typeof(ManagerBlog),
                typeof(ManagerBanner),
                typeof(RoleManager)
            };

            var permissions = new List<string>();

            foreach (var type in permissionTypes)
            {
                var fields = type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
                foreach (var field in fields)
                {
                    if (field.FieldType == typeof(string))
                    {
                        permissions.Add((string)field.GetValue(null));
                    }
                }
            }

            return permissions;
        }
    }

        
}
