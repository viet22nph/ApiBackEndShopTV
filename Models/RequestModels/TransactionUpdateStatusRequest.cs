using Models.Status;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.RequestModels
{
    public class TransactionUpdateStatusRequest
    {
        public Guid Id { get; set; }
        public String Status { get; set; }
    }
}
