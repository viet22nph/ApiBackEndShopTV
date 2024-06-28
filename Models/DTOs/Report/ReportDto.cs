using Models.DTOs.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTOs.Report
{
    public class InvoiceReportDto
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public ICollection<OrderDto> Orders { get; set; }
        public decimal TotalAmount { get; set; }
        public List<ProductSummary> ProductsSummary { get; set; }
    }
    public class SalesOrderSummary
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int TotalOrder { get; set; }
        public int TotalProductSold { get; set; }
        public decimal TotalRevenue { get; set; }
        public ICollection<DailyOrderSummary>? dailyOrderSummaries { get; set; }
    }
    public class DailyOrderSummary
    {
        public DateTime Date { get; set; }
        public int TotalOrder { get; set; }
        public int TotalProductSold { get; set; }
        public decimal TotalRevenue { get; set; }
    }



    public class ProductSummary
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        public int TotalQuantitySold { get; set; }
    }
}
