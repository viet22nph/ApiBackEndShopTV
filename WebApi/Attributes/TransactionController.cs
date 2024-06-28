using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.DTOs.Order;
using Models.RequestModels;
using Services.Interfaces;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WebApi.Attributes
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionService _transactionService;
        public TransactionController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }
        [HttpPost("create-transaction")]
        public async Task<IActionResult> AddTransaction([FromBody] TransactionDto transactionDto)
        {
            var data = await _transactionService.CreateTransaction(transactionDto);
            return Ok(new
            {
                message = "Create transaction",
                data = data
            });
        }

        [HttpPut("update-status")]
        public async Task<IActionResult> UpdateStatus([FromBody] TransactionUpdateStatusRequest request)
        {
            var rs = await _transactionService.UpdateStatusTransaction(request.Id, request.Status);
            return Ok(new
            {
                message = "Update status transaction success",
                data = rs
            });
        }
    }
}
