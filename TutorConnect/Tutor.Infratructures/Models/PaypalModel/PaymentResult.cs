﻿namespace Tutor.Infratructures.Models.PaypalModel
{
    public class PaymentResult
    {
        public bool Success { get; set; }
        public string ApprovalUrl { get; set; }
        public string Message { get; set; }
    }
}
