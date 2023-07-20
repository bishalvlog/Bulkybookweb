using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBooks.Utilitys
{
    public static class SD
    {
        public const string Role_User_Indi = "Individual";
        public const string Role_User_Comp = "Company";
        public const string Role_Admin = "Admin";
        public const string Role_Employee = "Employee";

        public const string StatusPending = "Pending";
        public const string StatusApprove = "Approve";
        public const string StatusInProcess = "Proccessing";
        public const string StatusShipped = "Shipped";
        public const string StatusCancelled = "Cancelled";
        public const string StatusRefunded = "Refunded";

        public const string PaymentStatusPending = "Pending";
        public const string PaymentStatusApprove = "Approved";
        public const string paymentStatusDelayedPayment = "ApproveForDelayedPayment";
        public const string PaymentStatusRejected = "Rejected";

    }
}
