using Application.DAL.Models;
using AutoMapper;
using Caching;
using Data.UnitOfWork;
using Models.DTOs.Product;
using Models.DTOs.Report;
using Services.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Concrete
{
    public class ReportService : IReportService
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly ICacheManager _cacheManager;
        private readonly IMapper _mapper;
        public ReportService(IUnitOfWork unitOfWork, ICacheManager cacheManager, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _cacheManager = cacheManager;
            _mapper = mapper;
        }
        public async Task<SalesOrderSummary> SalesReportSummary(DateTime startDate, DateTime endDate)
        {
            var data = await _unitOfWork.OrderRepository.GetOrderSummary(startDate, endDate);
            if(data == null)
            {
                return new SalesOrderSummary
                {
                    StartDate = startDate,
                    EndDate = endDate,
                    TotalOrder = 0,
                    TotalRevenue = 0,
                    TotalProductSold = 0,
                    dailyOrderSummaries = null

                };
            }

            return new SalesOrderSummary()
            {
                TotalOrder = data.Sum(x => x.TotalOrder),
                TotalProductSold = data.Sum(x => x.TotalProductSold),
                TotalRevenue = data.Sum(x => x.TotalRevenue),
                StartDate = startDate.Date,
                EndDate = endDate.Date,
                dailyOrderSummaries = data
            }; 
        }

        public async Task<ICollection<ReportVisited>> ReportVisited(DateTime startDate, DateTime endDate)
        {
            var reportVisits = new List<ReportVisited>();
            for (var date = startDate.Date; date <= endDate.Date; date = date.AddDays(1))
            {
                var count = await _cacheManager.GetVisitCountAsync(date);
                reportVisits.Add(new ReportVisited
                {
                    Date = date.ToString("dd-MM-yyyy"),
                    Count = (int)count
                });
            }
            
            return reportVisits;
        }
        public async Task<ICollection<ReportProductView>> ReportProductView(DateTime startDate, DateTime endDate, int top)
        {
            var topViewedProducts = await _cacheManager.GetTopViewedProductsAsync(startDate, endDate);
            var reportProductView = new List<ReportProductView>();
            int i = 1;
            foreach (var product in topViewedProducts)
            {
                if(i <= top)
                {
                    var data = _mapper.Map<ProductResponse>(await _unitOfWork.ProductRepository.GetProduct(product.Key));
                    reportProductView.Add(new ReportProductView
                    {
                        Products = data,
                        QuantityView = (int)product.Value
                    });
                    i++;
                }
                else
                {
                    break;
                }
            }
            return reportProductView;

        }
    }
}
