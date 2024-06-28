using Application.DAL.Models;
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
        public ReportService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
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

    }
}
