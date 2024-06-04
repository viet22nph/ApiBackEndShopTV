using Application.DAL.Models;
using Data.Contexts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Models.Enums;
using Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.SeedData
{
    public class SeedData
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;
      
        private List<Category> Categories = new List<Category>();
        private List<Supplier> Suppliers = new List<Supplier>();
        private List<Color> Colors = new List<Color>();
        private List<Product> Products = new List<Product>();
        private List<ProductItem> ProductItems = new List<ProductItem>();
        private List<ProductImage> ProductImages = new List<ProductImage>();
        private List<ProductSpecification> ProductSpecifications = new List<ProductSpecification>();


      
        public SeedData(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager, 
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        #region Data
            #region DataCategory
            var idCategory = new List<string>{
                "18a8604b-4234-4a2f-93de-16270a79dfb9",
                "147f3a51-78eb-4e92-ab3d-883169472504",
                "0dfc0abc-d45f-4df1-8490-68921ae348a5",
                "667988d0-31cb-4706-9099-7ccd300a2226"
            };
            var dataCate = new List<Category>{
                new Category
                {
                    Name ="Ghế",
                    Id=Guid.Parse(idCategory[0]),
                    NomalizedName="GHẾ",
                    Description ="Mọi loại ghế",
                    CategoryChildren = null,
                    CategoryParent = null,
                },
                 new Category
                {
                    Name ="Bàn",
                    Id=Guid.Parse(idCategory[1]),
                    NomalizedName="GHẾ",
                    Description ="Mọi loại ghế",
                    CategoryChildren = null,
                    CategoryParent = null,
                },
                  new Category
                {
                    Name ="Sofa",
                    Id=Guid.Parse(idCategory[2]),
                    NomalizedName="SOFA",
                    Description ="Mọi loại ghế sofa",
                    CategoryChildren = null,
                    CategoryParent = Guid.Parse(idCategory[0]),
                },
                new Category
                {
                    Name ="Sofa - da",
                    Id=Guid.Parse(idCategory[3]),
                    NomalizedName="GHẾ",
                    Description ="Mọi loại ghế sofa - da",
                    CategoryChildren = null,
                    CategoryParent = Guid.Parse(idCategory[2]),
                },

               

            };
            Categories.AddRange(dataCate);
            #endregion


            #region DataSupplier
            var idSupplier = new List<string>
            {
                "3fff44ad-a67b-47e7-acb1-b35e11c8c5df",
                "28d27e78-9ad6-41a9-b6cf-c3ddb7436696"
            };
            List<Supplier> dataSupp = new List<Supplier>
                {
                new Supplier
                {
                    Id = Guid.Parse(idSupplier[0]),
                    ContactPerson ="Nguyen dinh viet",
                    ContactPhone = "0931232",
                    SupplierName ="NDV",
                    Address="Thuan phu binh phuoc",
                    Notes="Cung cap ban ghe chat luong cao"
                },
                  new Supplier
                {
                    Id = Guid.Parse(idSupplier[1]),
                    ContactPerson ="Nguyen khac the",
                    ContactPhone = "0931211132",
                    SupplierName ="NKT",
                    Address="Ha Tinh",
                    Notes="Cung cap ban ghe chat luong cao"
                }
            };
            Suppliers.AddRange(dataSupp);
            #endregion

            #region DataProduct
            var idProduct = new List<string>
            {
                "d230d218-72c7-4d7a-a9af-9691b2e26bcd",
                "bb74e4ee-879f-4dc9-95f1-32e3f5e1ddeb",
                "e4da7f4f-077f-4760-bdc9-dfacc23e55ee",
                "f5ee7ecb-fa60-4853-87fe-a1c36c56018f",
                "d1fdbf4d-b003-45ed-88da-f4c569cc9cfa"
            };
            List<Product> dataProd = new List<Product>
            {
                new Product
                {
                    Id=Guid.Parse(idProduct[0]),
                    Name = "Ghế văn phòng",
                    NormalizedName = "GHE_VAN_PHONG",
                    Description = "Ghế ngồi văn phòng cao cấp",
                    ProductQuantity = 100,
                    ProductBrand = "NDV",
                    Price = 1500000,
                    SupplierId = Guid.Parse(idSupplier[0]),
                    CategoryId = Guid.Parse(idCategory[0])
                },
                new Product
                {
                     Id=Guid.Parse(idProduct[1]),
                    Name = "Bàn làm việc",
                    NormalizedName = "BAN_LAM_VIEC",
                    Description = "Bàn làm việc hiện đại",
                    ProductQuantity = 50,
                    ProductBrand = "NKT",
                    Price = 2000000,
                    SupplierId = Guid.Parse(idSupplier[1]),
                    CategoryId = Guid.Parse(idCategory[1])
                },
                new Product
                {

                     Id=Guid.Parse(idProduct[2]),
                    Name = "Sofa da thật",
                    NormalizedName = "SOFA_DA_THAT",
                    Description = "Sofa da thật cao cấp",
                    ProductQuantity = 20,
                    ProductBrand = "NDV",
                    Price = 10000000,
                    SupplierId = Guid.Parse(idSupplier[0]),
                    CategoryId = Guid.Parse(idCategory[2])
                },
                new Product
                {
                    Id = Guid.Parse(idProduct[3]),
                    Name = "Sofa vải",
                    NormalizedName = "SOFA_VAI",
                    Description = "Sofa vải mềm mại",
                    ProductQuantity = 30,
                    ProductBrand = "NKT",
                    Price = 7000000,
                    SupplierId = Guid.Parse(idSupplier[1]),
                    CategoryId = Guid.Parse(idCategory[2])
                },
                new Product
                {
                    Id = Guid.Parse(idProduct[4]),
                    Name = "Sofa da cao cấp",
                    NormalizedName = "SOFA_DA_CAO_CAP",
                    Description = "Sofa da cao cấp từ Italy",
                    ProductQuantity = 10,
                    ProductBrand = "NDV",
                    Price = 15000000,
                    SupplierId = Guid.Parse(idSupplier[0]),
                    CategoryId = Guid.Parse(idCategory[3])
                }
            };

            Products.AddRange(dataProd);
            #endregion
            #region DataProductSpecification
            List<ProductSpecification> dataProdSpec = new List<ProductSpecification>
            {
                // Specifications for Product 1
                new ProductSpecification
                {
                    Id = Guid.NewGuid(),
                    ProductId = Guid.Parse(idProduct[0]),
                    SpecType = "Material",
                    SpecValue = "Leather"
                },
                new ProductSpecification
                {

                    Id = Guid.NewGuid(),
                    ProductId = Guid.Parse(idProduct[0]),
                    SpecType = "Size",
                    SpecValue = "50x50x120cm"
                },
                new ProductSpecification
                {

                    Id = Guid.NewGuid(),
                    ProductId = Guid.Parse(idProduct[0]),
                    SpecType = "Weight",
                    SpecValue = "15kg"
                },

                // Specifications for Product 2
                new ProductSpecification
                {

                    Id = Guid.NewGuid(),
                    ProductId = Guid.Parse(idProduct[1]),
                    SpecType = "Material",
                    SpecValue = "Wood"
                },
                new ProductSpecification
                {
                    Id = Guid.NewGuid(),
                    ProductId = Guid.Parse(idProduct[1]),
                    SpecType = "Size",
                    SpecValue = "120x60cm"
                },
                new ProductSpecification
                {
                    Id = Guid.NewGuid(),
                    ProductId = Guid.Parse(idProduct[1]),
                    SpecType = "Weight",
                    SpecValue = "20kg"
                },

                // Specifications for Product 3
                new ProductSpecification
                {
                    Id = Guid.NewGuid(),
                    ProductId = Guid.Parse(idProduct[2]),
                    SpecType = "Material",
                    SpecValue = "Genuine Leather"
                },
                new ProductSpecification
                {
                    Id = Guid.NewGuid(),
                    ProductId = Guid.Parse(idProduct[2]),
                    SpecType = "Size",
                    SpecValue = "200x90x85cm"
                },
                new ProductSpecification
                {
                    Id = Guid.NewGuid(),
                    ProductId = Guid.Parse(idProduct[2]),
                    SpecType = "Weight",
                    SpecValue = "50kg"
                },

                // Specifications for Product 4
                new ProductSpecification
                {Id = Guid.NewGuid(),
                    ProductId = Guid.Parse(idProduct[3]),
                    SpecType = "Material",
                    SpecValue = "Fabric"
                },
                new ProductSpecification
                {Id = Guid.NewGuid(),
                    ProductId = Guid.Parse(idProduct[3]),
                    SpecType = "Size",
                    SpecValue = "180x85x80cm"
                },
                new ProductSpecification
                {Id = Guid.NewGuid(),
                    ProductId = Guid.Parse(idProduct[3]),
                    SpecType = "Weight",
                    SpecValue = "45kg"
                },

                // Specifications for Product 5
                new ProductSpecification
                {Id = Guid.NewGuid(),
                    ProductId = Guid.Parse(idProduct[4]),
                    SpecType = "Material",
                    SpecValue = "Italian Leather"
                },
                new ProductSpecification
                {
                    Id = Guid.NewGuid(),
                    ProductId = Guid.Parse(idProduct[4]),
                    SpecType = "Size",
                    SpecValue = "210x95x90cm"
                },
                new ProductSpecification
                {
                    Id = Guid.NewGuid(),
                    ProductId = Guid.Parse(idProduct[4]),
                    SpecType = "Weight",
                    SpecValue = "55kg"
                }
            };

            ProductSpecifications.AddRange(dataProdSpec);
            #endregion
            #region DataColor
            var idColors = new List<string>
            {
                "95e5d345-6be7-481e-bb3a-814ff1fa2900",
                "92dec0e1-c78f-4f79-8835-836cab87be32",
                "4c9013c8-8564-4d9e-8ddb-b78b455af258",
                "705f326d-b87a-48a8-af65-f105a9d0e3d8",
                "a486627a-7545-46ce-8c07-49f6adf57cfc",
                "7dba2e93-eddf-43a4-ba2f-b11ce7aa885d",
                "99fe8bab-42ab-444c-a5b8-8195ddb2ba88",
                "1c1a2a09-d03e-40cd-b39c-088ce1333295",
                "8ea83fdd-ecf0-4c68-8df0-d9238e098400",
                "e8a5712b-32e7-4c1d-b52c-5edcd866cb61",
                "10525ddf-81ef-4adf-a116-0cd85700d4e7",
                "4186935b-7e90-4419-a1c3-10220db32a1c",
                "43785eb6-607f-4e35-a5e6-eddd57cdf161",
                "447211d5-2e95-4bb7-a070-5c1632f93532",
                "b6977284-84ac-463d-b222-69c7aa0f368a",
                "de3fcd41-a074-492f-a4f7-514ae2ef89cd",
                "c876b73a-9930-4fb0-914b-024c169cc8ed",
                "f6cfd5af-1873-4c2c-8c25-08dfd251449e",
                "1894c8da-4652-422d-8b35-30131a3478cb",
                "a97cdf10-4fc8-42b2-ae03-594a989abc69",
                "4d2ea9ff-6462-43a2-a6c8-b417ac001648",
                "04910ef9-2bbc-45d8-b0ed-16486824f569",
                "0305b505-08a0-43f9-85f2-44125f6e8fcf",
                "4ea2d0b9-ca97-41c1-b5db-d17cab681437",
                "f6c38cbe-8a1a-46b6-bd8f-50038558949c",
                "c0311754-5d10-4567-8cb3-00e8ff12996c",
                "30e7b689-2793-4a44-8304-daea21745483",
                "c96033bd-acb0-4bce-9839-17b07401e02b",
                "9a82dfa8-cc2d-487b-a48b-18721c9e34d4",
                "d222ca19-c485-46fd-beba-39c172651a74"
            };

            List<Color> dataColors = new List<Color>
            {
                new Color { Id = Guid.Parse(idColors[0]), ColorName = "Red", ColorCode = "#FF0000" },
                new Color { Id = Guid.Parse(idColors[1]), ColorName = "Green", ColorCode = "#00FF00" },
                new Color { Id = Guid.Parse(idColors[2]), ColorName = "Blue", ColorCode = "#0000FF" },
                new Color { Id = Guid.Parse(idColors[3]), ColorName = "Yellow", ColorCode = "#FFFF00" },
                new Color { Id = Guid.Parse(idColors[4]), ColorName = "Cyan", ColorCode = "#00FFFF" },
                new Color { Id = Guid.Parse(idColors[5]), ColorName = "Magenta", ColorCode = "#FF00FF" },
                new Color { Id = Guid.Parse(idColors[6]), ColorName = "Black", ColorCode = "#000000" },
                new Color { Id = Guid.Parse(idColors[7]), ColorName = "White", ColorCode = "#FFFFFF" },
                new Color { Id = Guid.Parse(idColors[8]), ColorName = "Gray", ColorCode = "#808080" },
                new Color { Id = Guid.Parse(idColors[9]), ColorName = "Maroon", ColorCode = "#800000" },
                new Color { Id = Guid.Parse(idColors[10]), ColorName = "Olive", ColorCode = "#808000" },
                new Color { Id = Guid.Parse(idColors[11]), ColorName = "Purple", ColorCode = "#800080" },
                new Color { Id = Guid.Parse(idColors[12]), ColorName = "Teal", ColorCode = "#008080" },
                new Color { Id = Guid.Parse(idColors[13]), ColorName = "Navy", ColorCode = "#000080" },
                new Color { Id = Guid.Parse(idColors[14]), ColorName = "Silver", ColorCode = "#C0C0C0" },
                new Color { Id = Guid.Parse(idColors[15]), ColorName = "Lime", ColorCode = "#00FF00" },
                new Color { Id = Guid.Parse(idColors[16]), ColorName = "Aqua", ColorCode = "#00FFFF" },
                new Color { Id = Guid.Parse(idColors[17]), ColorName = "Fuchsia", ColorCode = "#FF00FF" },
                new Color { Id = Guid.Parse(idColors[18]), ColorName = "Orange", ColorCode = "#FFA500" },
                new Color { Id = Guid.Parse(idColors[19]), ColorName = "Pink", ColorCode = "#FFC0CB" },
                new Color { Id = Guid.Parse(idColors[20]), ColorName = "Brown", ColorCode = "#A52A2A" },
                new Color { Id = Guid.Parse(idColors[21]), ColorName = "Lavender", ColorCode = "#E6E6FA" },
                new Color { Id = Guid.Parse(idColors[22]), ColorName = "Gold", ColorCode = "#FFD700" },
                new Color { Id = Guid.Parse(idColors[23]), ColorName = "Beige", ColorCode = "#F5F5DC" },
                new Color { Id = Guid.Parse(idColors[24]), ColorName = "Ivory", ColorCode = "#FFFFF0" },
                new Color { Id = Guid.Parse(idColors[25]), ColorName = "Coral", ColorCode = "#FF7F50" },
                new Color { Id = Guid.Parse(idColors[26]), ColorName = "Salmon", ColorCode = "#FA8072" },
                new Color { Id = Guid.Parse(idColors[27]), ColorName = "Khaki", ColorCode = "#F0E68C" },
                new Color { Id = Guid.Parse(idColors[28]), ColorName = "Crimson", ColorCode = "#DC143C" },
                new Color { Id = Guid.Parse(idColors[29]), ColorName = "Plum", ColorCode = "#DDA0DD" }
            };

            Colors.AddRange(dataColors);
            #endregion

            #region DataProductItem
            var random = new Random();
            List<ProductItem> dataProductItems = new List<ProductItem>();

            foreach (var productId in idProduct)
            {
                int numberOfItems = random.Next(1, 3); // Randomly decide between 1 or 2 items per product
                for (int i = 0; i < numberOfItems; i++)
                {
                    var colorId = Guid.Parse(idColors[random.Next(idColors.Count)]); // Randomly select a color
                    dataProductItems.Add(new ProductItem
                    {
                        ProductId = Guid.Parse(productId),
                        Quantity = random.Next(1, 101), // Randomly select a quantity between 1 and 100
                        ColorId = colorId
                    });
                }
            }

            ProductItems.AddRange(dataProductItems);
            #endregion
            #endregion
        }

        #region SeedData
        public async Task SeedDataAsync()
        {
            await SeedRoles();
            await SeendUser();
            await SeedCategry();
            await SeedSupplier();
            await SeedColor();
            await SeedProduct();
            await SeedProductSpec();
            await SeedProductItem();
        }

        #endregion
        #region SeedRoles
        async Task SeedRoles()
        {
            if (await _roleManager.Roles.AnyAsync()) return;

            foreach (var role in Enum.GetNames<RoleEnums>())
            {
                await _roleManager.CreateAsync(new IdentityRole { Name = role });
            }
        }
        #endregion
        #region SeedUser
        async Task SeendUser()
        {

            if (await _userManager.Users.AnyAsync()) return;
             var userArray = new[]
{
                    new { UserName = "Admin", Email = "admin@yam.cl", NumberPhone ="034211112", Role = Enum.GetName<RoleEnums>(RoleEnums.Admin)?? "User", Password = "Admin@123" },
                    new { UserName = "ndviet2020", Email = "viet22np@yam.cl", NumberPhone ="034211111",  Role =Enum.GetName<RoleEnums>(RoleEnums.Admin)?? "User", Password = "Viet@123" },
                    new {UserName = "nkthe1301", Email = "the@yam.cl", NumberPhone ="034211191", Role = Enum.GetName<RoleEnums>(RoleEnums.User)?? "User", Password = "The@123"}
            };

            foreach (var user in userArray)
            {
                var userdata = new ApplicationUser()
                {
                    UserName = user.UserName,
                    Email = user.Email,
                    PhoneNumber = user.NumberPhone,
                    EmailConfirmed = true
                };
                var result = await _userManager.CreateAsync(userdata, user.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(userdata, user.Role);

                }

            }

        }
        #endregion

        #region SeedCategory
        public async Task SeedCategry()
        {
            if (_context.Set<Category>().Any()) return;
            await _context.Set<Category>().AddRangeAsync(Categories);
            _context.SaveChanges();
        }
        #endregion

        #region SeedSupplier
        public async Task SeedSupplier()
        {
            if (_context.Set<Supplier>().Any()) return;
            await _context.Set<Supplier>().AddRangeAsync(Suppliers);
            _context.SaveChanges();
        }
        #endregion

        #region SeedColor
        public async Task SeedColor()
        {
            if (_context.Set<Color>().Any()) return;
            await _context.Set<Color> ().AddRangeAsync(Colors);
            _context.SaveChanges();
        }
        #endregion
        #region SeedProduct
        public async Task SeedProduct()
        {
            if (_context.Set<Product>().Any()) return;
            await _context.Set<Product>().AddRangeAsync(Products);
            _context.SaveChanges();
        }
        #endregion
        #region SeedProductSpec
        public async Task SeedProductSpec()
        {
            if (_context.Set<ProductSpecification>().Any()) return;
            await _context.Set<ProductSpecification>().AddRangeAsync(ProductSpecifications);
            _context.SaveChanges();
        }
        #endregion
        #region SeedProductItem
        public async Task SeedProductItem()
        {
            if (_context.Set<ProductItem>().Any()) return;
            await _context.Set<ProductItem>().AddRangeAsync(ProductItems);
            _context.SaveChanges();
        }
        #endregion
    }
}
