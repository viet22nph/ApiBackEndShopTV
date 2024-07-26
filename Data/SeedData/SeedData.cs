using Application.DAL.Models;
using Data.Contexts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Models.Enums;
using Models.Models;
using Models.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
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
        private List<GroupBanner> Groups = new List<GroupBanner>();


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
                    Name ="Phòng làm việc",
                    NomalizedName ="PHÒNG LÀM VIỆC",
                    Id=Guid.Parse(idCategory[0]),
                    Description ="Tại danh mục phòng làm việc, các sản phẩm nội thất thường được" +
                    " thiết kế để tối ưu hóa không gian, tăng cường sự thoải mái và nâng cao hiệu quả công việc",
                    CategoryChildren = null,
                    CategoryParent = null,
                },
                 new Category
                {
                    Name ="Kệ sách",
                    NomalizedName ="KỆ SÁCH",
                    Id=Guid.Parse(idCategory[1]),
                    Description ="Kệ sách là một phần quan trọng trong không gian phòng làm việc, giúp tổ chức sách vở," +
                    " tài liệu và các vật dụng trang trí một cách gọn gàng và thẩm mỹ."
,
                    CategoryChildren = null,
                    CategoryParent = Guid.Parse(idCategory[0]),
                },
                  new Category
                {
                    Name ="Bàn làm việc",
                    NomalizedName = "BÀN LÀM VIỆC",
                    Id=Guid.Parse(idCategory[2]),
                    Description ="Bàn làm việc là một phần quan trọng trong không gian văn phòng, được thiết kế để hỗ trợ" +
                    " tốt nhất cho các hoạt động công việc hàng ngày.",
                    CategoryChildren = null,
                    CategoryParent = Guid.Parse(idCategory[0]),
                },
                new Category
                {
                    Name ="Ghế làm việc",
                    NomalizedName ="GHẾ LÀM VIỆC",
                    Id=Guid.Parse(idCategory[3]),
                    Description ="Ghế làm việc là một yếu tố quan trọng trong bất kỳ không gian văn phòng nào," +
                    " ảnh hưởng trực tiếp đến sự thoải mái và hiệu suất làm việc của người sử dụng. "
,
                    CategoryChildren = null,
                    CategoryParent = Guid.Parse(idCategory[0]),
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

            List<Product> dataProd = new List<Product>
            {
                new Product
                {
                    Name = "Ghế làm việc Monica",
                    NormalizedName = "GHẾ LÀM VIỆC MONICA",
                    Description = "<p>Với thiết kế hiện đại, kiểu dáng sang trọng và chất lượng bền bỉ, " +
                    "ghế Lounge Monica nhập khẩu cao cấp của thương hiệu The Center sẽ mang đến luồng gió mới cho bất gì không gian nội thất nào" +
                    ".</p><p>Ghế Lounge Monica cấu tạo đơn giản không quá cầu kì, phức tạp nên dễ dàng tháo lắp," +
                    " kích thước nhỏ gọn và trọng lượng tương đối nhẹ nên thuận tiện hơn cho việc di chuyển khi cần thiết.</p><p>Sản phẩm" +
                    " thiết kế kiểu đáng đặc biệt, tựa lưng thấp kết cấu liền với phần tay vịn và đệm ngồi tạo thành một khối thống nhất mang đến" +
                    " sự chắc chắn, an toàn cao.</p><p>Cấu trúc khung chân đặc biệt, được gia công 100% từ hợp kim nhôm cao cấp nên độ bền chất lượng cao," +
                    " chống hoen gỉ, khả năng chống chịu tải trọng cao tốt và giữ thăng bằng ổn định khi ngồi.</p><p>Phần đệm tựa chế tạo từ chất liệu ván" +
                    " ép định dạng cứng cáp + mouse xốp đúc dày có tỉ trọng cao nên không bị xẹp lún và bề mặt ngoài bọc lớp vải chất lượng cho cảm" +
                    " giác ngồi êm ái, dễ chịu nhất.</p><p>Màu sắc ghế đa dạng với các tone màu trung tính thời thượng hợp thời trang nên quý khách hàng" +
                    " có thể phong phú hóa việc lựa chọn theo sở thích cá nhân.</p><p>Ghế Lounge Monica tuy không quá đặc biệt về chất liệu nhưng lại được " +
                    "nhiều chuyên gia đánh giá cao về sự tỉ mỉ trong từng chi tiết. Sản phẩm hiện được ưa chuộng sử dụng cho các không gian phòng khách," +
                    " quán cafe, nhà hàng, khách sạn…</p>",
                    ProductQuantity = 50,
                    ProductBrand = "Nội Thất NDV",
                    Price = 13000000,
                    SupplierId = Guid.Parse(idSupplier[0]),
                    CategoryId = Guid.Parse(idCategory[3]),
                    ProductSpecifications = new  List<ProductSpecification>()
                    {
                        new ProductSpecification()
                        {
                            SpecType= "Vật liệu",
                            SpecValue ="\"Khung gỗ sồi Nga tự nhiên bề mặt sơn căng mịn sử dụng chủng loại sơn Inchem, phủ verne Óc Chó, mút D40, bọc da Microfiber\",\r\n      "
                        },
                        new ProductSpecification()
                        {
                            SpecType ="Kích thước",
                            SpecValue ="D830 - R760 - C790 mm"
                        }

                    },
                    ProductItems = new List<ProductItem>
                    {
                        new ProductItem
                        {
                            Quantity = 50,
                            ColorId = Guid.Parse("4D2EA9FF-6462-43A2-A6C8-B417AC001648"),
                            ProductImages = new List<ProductImage>
                            {
                                new ProductImage
                                {
                                    Url ="http://res.cloudinary.com/dpnqk5rk8/image/upload/v1718906803/NDV_Images/lkdr9w2vbk1ceqhn36d0.jpg",

                                },
                                 new ProductImage
                                {
                                    Url ="http://res.cloudinary.com/dpnqk5rk8/image/upload/v1718906804/NDV_Images/doxh0ip61xkbmy42liai.jpg",

                                },
                                  new ProductImage
                                {
                                    Url ="http://res.cloudinary.com/dpnqk5rk8/image/upload/v1718906805/NDV_Images/apo3cojhskwyetxn85eu.jpg",

                                }, new ProductImage
                                {
                                    Url ="http://res.cloudinary.com/dpnqk5rk8/image/upload/v1718906806/NDV_Images/s1hvm0rxuyn1cejyc4br.jpg",

                                }, new ProductImage
                                {
                                    Url ="http://res.cloudinary.com/dpnqk5rk8/image/upload/v1718906807/NDV_Images/xwgs3yzvyniwgmv0efam.jpg",

                                }, new ProductImage
                                {
                                    Url ="http://res.cloudinary.com/dpnqk5rk8/image/upload/v1718906808/NDV_Images/apmrz4xvxqc763yqxqbi.jpg",

                                },
                                   new ProductImage
                                {
                                    Url ="http://res.cloudinary.com/dpnqk5rk8/image/upload/v1718906809/NDV_Images/ss7gkg4rel9tyl0se8zf.jpg",

                                }, new ProductImage
                                {
                                    Url ="http://res.cloudinary.com/dpnqk5rk8/image/upload/v1718906810/NDV_Images/jav9hpoai7fn3h2lutel.jpg",

                                }
                            }
                        }
                    }


                },
               new Product
{
    Name = "Ghế làm việc check out 83959K",
    NormalizedName = "GHẾ LÀM VIỆC CHECK OUT 83959K",
    Description = "<p>Với thiết kế hiện đại, kiểu dáng sang trọng và chất lượng bền bỉ, ghế làm việc check out 83959K là lựa chọn hoàn hảo cho không gian làm việc của bạn. Chân ghế kim loại có bánh xe xoay giúp dễ dàng di chuyển, lưng MDF veneer - bọc da công nghiệp mang đến sự thoải mái và độ bền cao.</p><p>Kích thước ghế D750 - R750 - C1180 mm phù hợp với nhiều không gian văn phòng khác nhau.</p>",
    ProductQuantity = 50,
    ProductBrand = "Nội Thất NDV",
    Price = 24000000, SupplierId = Guid.Parse(idSupplier[0]),
                    CategoryId = Guid.Parse(idCategory[3]), // Thay bằng id thực tế
    ProductSpecifications = new List<ProductSpecification>
    {
        new ProductSpecification
        {
            SpecType = "Vật liệu",
            SpecValue = "Chân kim loại có bánh xe xoay, lưng mdf veneer - bọc da công nghiệp"
        },
        new ProductSpecification
        {
            SpecType = "Kích thước",
            SpecValue = "D750 - R750 - C1180 mm"
        }
    },
    ProductItems = new List<ProductItem>
    {
        new ProductItem
        {
            Quantity = 50,
            ColorId = Guid.Parse("4D2EA9FF-6462-43A2-A6C8-B417AC001648"),
            ProductImages = new List<ProductImage>
            {
                new ProductImage
                {
                    Url = "http://res.cloudinary.com/dpnqk5rk8/image/upload/v1718907232/NDV_Images/x8cowzojvsqbqug7uknf.jpg"
                },
                new ProductImage
                {
                    Url = "http://res.cloudinary.com/dpnqk5rk8/image/upload/v1718907233/NDV_Images/qs6xabgg2lhx3vfohztv.jpg"
                },
                new ProductImage
                {
                    Url = "http://res.cloudinary.com/dpnqk5rk8/image/upload/v1718907234/NDV_Images/wubvmhjpdijgk2dfzjkk.jpg"
                },
                new ProductImage
                {
                    Url = "http://res.cloudinary.com/dpnqk5rk8/image/upload/v1718907235/NDV_Images/apjdug9gztzmvypw6dwb.jpg"
                }
            }
        }
    }
}
,
            new Product
{
    Name = "Ghế làm việc xoay Marla",
    NormalizedName ="GHẾ LÀM VIỆC XOAY MARLA",
    Description = "<p>Ghế làm việc xoay Marla được thiết kế với chân mạ Chrome xoay 360 độ, nệm ghế da công nghiệp mang đến sự thoải mái và phong cách hiện đại.</p><p>Kích thước ghế D490 - R590 - C900 mm phù hợp với nhiều không gian văn phòng khác nhau.</p>",
    ProductQuantity = 50,
    ProductBrand = "Nội Thất NDV",
    Price = 12500000,
                SupplierId = Guid.Parse(idSupplier[0]),
                    CategoryId = Guid.Parse(idCategory[3]),  // Thay bằng id thực tế
    ProductSpecifications = new List<ProductSpecification>
    {
        new ProductSpecification
        {
            SpecType = "Vật liệu",
            SpecValue = "Chân mạ Chrome xoay 360 độ, nệm ghế da công nghiệp"
        },
        new ProductSpecification
        {
            SpecType = "Kích thước",
            SpecValue = "D490 - R590 - C900 mm"
        }
    },
    ProductItems = new List<ProductItem>
    {
        new ProductItem
        {
            Quantity = 50,
            ColorId = Guid.Parse("8EA83FDD-ECF0-4C68-8DF0-D9238E098400"),
            ProductImages = new List<ProductImage>
            {
                new ProductImage
                {
                    Url = "http://res.cloudinary.com/dpnqk5rk8/image/upload/v1718907491/NDV_Images/vyqh076tl68mwnefo4sx.jpg"
                },
                new ProductImage
                {
                    Url = "http://res.cloudinary.com/dpnqk5rk8/image/upload/v1718907492/NDV_Images/vqhlfc3nbq2spjccy7nx.jpg"
                },
                new ProductImage
                {
                    Url = "http://res.cloudinary.com/dpnqk5rk8/image/upload/v1718907493/NDV_Images/chkrtljpwrvuhgq3kivu.jpg"
                },
                new ProductImage
                {
                    Url = "http://res.cloudinary.com/dpnqk5rk8/image/upload/v1718907495/NDV_Images/wun8d4xfmnligf0kzbef.jpg"
                },
                new ProductImage
                {
                    Url = "http://res.cloudinary.com/dpnqk5rk8/image/upload/v1718907496/NDV_Images/telvqp4ujtva9kyjkhr3.jpg"
                }
            }
        }
    } 
}, new Product
{
    Name = "Bàn làm việc Fence",
    ProductQuantity = 50,
    ProductBrand = "Shop NDV",
    Price = 30000000,
    CategoryId = Guid.Parse("0dfc0abc-d45f-4df1-8490-68921ae348a5"),
    ProductSpecifications = new List<ProductSpecification>
    {
        new ProductSpecification
        {
            SpecType = "Vật liệu",
            SpecValue = "Chân kim loại - mặt kính"
        },
        new ProductSpecification
        {
            SpecType = "Kích thước",
            SpecValue = "D1280 - R295 - C700 mm"
        }
    },
    ProductItems = new List<ProductItem>
    {
        new ProductItem
        {
            Quantity = 50,
            ColorId = Guid.Parse("99FE8BAB-42AB-444C-A5B8-8195DDB2BA88"),
            ProductImages = new List<ProductImage>
            {
                new ProductImage { Url = "http://res.cloudinary.com/dpnqk5rk8/image/upload/v1719833267/NDV_Images/xhk9zavkdyngzsfncz3i.jpg" },
                new ProductImage { Url = "http://res.cloudinary.com/dpnqk5rk8/image/upload/v1719833270/NDV_Images/vuhxefucvt5t0m1bgj0j.jpg" },
                new ProductImage { Url = "http://res.cloudinary.com/dpnqk5rk8/image/upload/v1719833271/NDV_Images/xxy21boqj7yx12gohy9q.jpg" },
                new ProductImage { Url = "http://res.cloudinary.com/dpnqk5rk8/image/upload/v1719833272/NDV_Images/jxrostkpidruuaboyn3d.jpg" },
                new ProductImage { Url = "http://res.cloudinary.com/dpnqk5rk8/image/upload/v1719833273/NDV_Images/ntx5czsozafte1jligis.jpg" },
                new ProductImage { Url = "http://res.cloudinary.com/dpnqk5rk8/image/upload/v1719833274/NDV_Images/fl2dx5adszlv2p4gtlp4.jpg" },
                new ProductImage { Url = "http://res.cloudinary.com/dpnqk5rk8/image/upload/v1719833275/NDV_Images/gllk1nwve3rrlsa9jtxf.jpg" }
            }
        }
    }
}, new Product
{
    Name = "Bàn làm việc Finn 260011",
    ProductQuantity = 50,
    ProductBrand = "Shop NDV",
    Price = 25500000,
    CategoryId = Guid.Parse("0dfc0abc-d45f-4df1-8490-68921ae348a5"),
    ProductSpecifications = new List<ProductSpecification>
    {
        new ProductSpecification
        {
            SpecType = "Vật liệu",
            SpecValue = "Gỗ nâu"
        },
        new ProductSpecification
        {
            SpecType = "Kích thước",
            SpecValue = "D1100 - R565 - C1020 mm"
        }
    },
    ProductItems = new List<ProductItem>
    {
        new ProductItem
        {
            Quantity = 50,
            ColorId = Guid.Parse("4D2EA9FF-6462-43A2-A6C8-B417AC001648"),
            ProductImages = new List<ProductImage>
            {
                new ProductImage { Url = "http://res.cloudinary.com/dpnqk5rk8/image/upload/v1719833653/NDV_Images/etbbcem6t5uk7cuyz1og.jpg" },
                new ProductImage { Url = "http://res.cloudinary.com/dpnqk5rk8/image/upload/v1719833655/NDV_Images/fbkfftv5ptdyx05g5fnc.jpg" },
                new ProductImage { Url = "http://res.cloudinary.com/dpnqk5rk8/image/upload/v1719833662/NDV_Images/yvritfdhfu1xkqarblkf.jpg" }
            }
        }
    }
}, new Product
{
    Name = "Kệ Sách Artigo",
    ProductQuantity = 50,
    ProductBrand = "Shop NDV",
    Price = 28800000,
    CategoryId = Guid.Parse("147f3a51-78eb-4e92-ab3d-883169472504"),
    ProductSpecifications = new List<ProductSpecification>
    {
        new ProductSpecification
        {
            SpecType = "Collection",
            SpecValue = "Coci"
        },
        new ProductSpecification
        {
            SpecType = "Vật liệu",
            SpecValue = "Gỗ Sồi kết hợp MDF veneer, chân thép sơn tĩnh điện"
        },
        new ProductSpecification
        {
            SpecType = "Kích thước",
            SpecValue = "D850 - R380 - C1980 mm"
        }
    },
    ProductItems = new List<ProductItem>
    {
        new ProductItem
        {
            Quantity = 50,
            ColorId = Guid.Parse("4D2EA9FF-6462-43A2-A6C8-B417AC001648"),
            ProductImages = new List<ProductImage>
            {
                new ProductImage { Url = "http://res.cloudinary.com/dpnqk5rk8/image/upload/v1719834193/NDV_Images/xqpsyxga81pj72uomndg.jpg" },
                new ProductImage { Url = "http://res.cloudinary.com/dpnqk5rk8/image/upload/v1719834195/NDV_Images/bneb4vrlrkyoqpzroq9k.jpg" }
            }
        }
    }
}


            };

            Products.AddRange(dataProd);
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


            #endregion

            #region banner groups
            Groups.AddRange(
                new List<GroupBanner> {
            new GroupBanner
            {
                GroupName = "HOME PAGE"
            },
             new GroupBanner
             {
                 GroupName = "PRODUCT PAGE"
             },
             new GroupBanner
             {
                 GroupName = "CART PAGE"
             },
             new GroupBanner
             {
                 GroupName = "DETAIL PAGE"
             }
});


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
            await SeedGroup();
        }

        #endregion
        #region SeedRoles
        public enum RoleEnums
        {
            Admin,
            Manager,
            User
        }
        async Task SeedRoles()
        {
            if (await _roleManager.Roles.AnyAsync()) return;
            foreach (var role in Enum.GetNames(typeof(RoleEnums)))
            {
                var identityRole = new IdentityRole { Name = role };
                await _roleManager.CreateAsync(identityRole);

                var claims = Permissions.GetAllPermissions();
                foreach (var claim in claims)
                {
                    if(role == nameof(RoleEnums.User))
                    {
                        await _roleManager.AddClaimAsync(identityRole, new Claim(claim, "false"));
                    }
                    else
                    {

                        await _roleManager.AddClaimAsync(identityRole, new Claim(claim, "true"));
                    }
                }
            }

        }
        #endregion
        #region SeedUser
        async Task SeendUser()
        {

            if (await _userManager.Users.AnyAsync()) return;
            var userArray = new[]
{
                    new { UserName = "Admin", Email = "admin@yam.cl", NumberPhone ="034211112", Role =  nameof(RoleEnums.Admin)?? "User", Password = "Admin@123" },
                    new { UserName = "ndviet2020", Email = "viet22np@yam.cl", NumberPhone ="034211111", Role=  nameof(RoleEnums.User)?? "User", Password = "Viet@123" },
                    new {UserName = "nkthe1301", Email = "the@yam.cl", NumberPhone ="034211191", Role = nameof(RoleEnums.User)?? "User", Password = "The@123"}
            };

            foreach (var user in userArray)
            {
                var userdata = new ApplicationUser()
                {
                    UserName = user.UserName,
                    Email = user.Email,
                    PhoneNumber = user.NumberPhone,
                    EmailConfirmed = true,
                    DislayName = user.UserName
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
            await _context.Set<Color>().AddRangeAsync(Colors);
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
       
        public async Task SeedGroup()
        {
            if (_context.Set<GroupBanner>().Any()) return;
            await _context.Set<GroupBanner>().AddRangeAsync(Groups);
            _context.SaveChanges();
        }
    }
}
