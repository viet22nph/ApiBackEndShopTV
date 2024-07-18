using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Status
{
    public class DiscountStatus
    {
        public const string PENDING = "PENDING";
        public const string ACTIVE = "ACTIVE";
        public const string EXPIRED = "EXPIRED";
        public const string CANCELLED = "CANCELLED";
        public const string PAUSE = "PAUSE";
        public const string USED_UP = "USED_UP";// Chuyển trạng thái từ active sang used_up.
    }
    public static class OrderStatus
    {
        public const string NEWORDER = "NEW-ORDER";
        public const string CANCELLED = "CANCELLED";
        public const string COMPLETED = "COMPLETED";
        public const string PROCESSING = "PROCESSING";
        public const string FAILED = "FAILED";
        public static bool IsValidStatus(string status)
        {
            var validStatuses = new HashSet<string>
            {
                NEWORDER, CANCELLED, COMPLETED, PROCESSING
            };

            return validStatuses.Contains(status);
        }
    }

    public static class TransactionStatus
    {
        public const string PENDING = "PENDING";
        public const string COMPLETED = "COMPLETED";
        public static bool IsValidStatus(string status)
        {
            var validStatuses = new HashSet<string>
            {
                COMPLETED, PENDING
            };

            return validStatuses.Contains(status);
        }
    }
}
