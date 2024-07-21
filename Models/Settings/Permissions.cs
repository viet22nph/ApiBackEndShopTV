using System;
using System.Collections.Generic;
using System.Linq;
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
        public static class Banner
        {
            public const string Create = "Permissions.Banner.Create";
            public const string Read = "Permissions.Banner.Read";
            public const string Update = "Permissions.Banner.Update";
            public const string Delete = "Permissions.Banner.Delete";

        }
        public static class GroupBanner
        {
            public const string Create = "Permissions.GroupBanner.Create";
            public const string Read = "Permissions.GroupBanner.Read";
            public const string Update = "Permissions.GroupBanner.Update";
            public const string Delete = "Permissions.GroupBanner.Delete";
        }
        public static class User
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
        public static class Order
        {

        }
        public static class Report
        {

        }
        public static class ContactUs
        {

        }
    }

        
}
