using Application.DAL.Models;
using AutoMapper;
using Core.Exceptions;
using Data.UnitOfWork;
using Models.DTOs.Order;
using Models.Status;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Services.Concrete
{
    public class TransactionService :ITransactionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public TransactionService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }


        public async Task<TransactionDto> UpdateStatusTransaction(Guid id, string newStatus)
        {
            var transaction = await _unitOfWork.Repository<Transaction>().GetById(id);
            if (transaction == null)
            {
                throw new ApiException($"Internal server error: Not found") { StatusCode = (int)HttpStatusCode.BadRequest };
            }
            if(transaction.Status  == TransactionStatus.COMPLETED)
            {
                throw new ApiException($"Internal server error: Not update transaction contains status completed") { StatusCode = (int)HttpStatusCode.BadRequest };

            }
            transaction.Status = newStatus;
            transaction.DateUpdate = DateTime.Now;
            var data =await _unitOfWork.Repository<Transaction>().Update(transaction);
            var res = _mapper.Map<TransactionDto>(data);
            return res;
        }

        public async Task<TransactionDto> CreateTransaction(TransactionDto transaction)
        {
            try
            {
                var data = _mapper.Map<Transaction>(transaction);
                data.DateCreate = DateTime.Now;
                data = await _unitOfWork.Repository<Transaction>().Insert(data);
                if (data == null)
                {
                    throw new ApiException($"Internal server error: Can not create transaction") { StatusCode = (int)HttpStatusCode.BadRequest };
                }
                var res = _mapper.Map<TransactionDto>(data);
                return res;
            }
            catch(Exception ex)
            {
                throw new ApiException($"Internal server error: {ex.Message}") { StatusCode = (int)HttpStatusCode.BadRequest };

            }
        
        }

    }
}
