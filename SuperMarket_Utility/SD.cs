namespace SuperMarket_Utility
{
    public static class SD
    {
        //role
        public const string Role_User_Customer = "Customer";
        public const string Role_Admin = "Admin";

        //order status
        public const string StatusPending = "Pending";
        public const string StatusApproved = "Approved";
        public const string StatusInProcess = "Processing";
        public const string StatusRefunded = "Refunded";
        public const string StatusCancelRequest = "CancelRequest";
        public const string StatusCompleted = "Completed";

        //payment
        public const string PaymentStatusPending = "Pending";
        public const string PaymentStatusApproved = "Approved";
        public const string PaymentStatusRejected = "Rejected";

        //coupon
        public const string CouponExpired = "Expired";
        public const string CouponNotExists = "NotExists";
    }
}