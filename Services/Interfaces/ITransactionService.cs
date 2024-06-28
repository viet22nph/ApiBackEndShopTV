using Models.DTOs.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface ITransactionService
    {
        Task<TransactionDto> CreateTransaction(TransactionDto transaction); Task<TransactionDto> UpdateStatusTransaction(Guid id, string newStatus);
    }
}
