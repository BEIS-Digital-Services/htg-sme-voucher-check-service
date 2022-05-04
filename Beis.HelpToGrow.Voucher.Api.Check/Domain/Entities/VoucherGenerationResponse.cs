using System;

namespace Beis.HelpToGrow.Voucher.Api.Check.Domain.Entities
{
    public class VoucherGenerationResponse
    {
        public string registration { get; set; }
        public string productSku { get; set; }
        public string voucherCode { get; set; }
        public Decimal voucherBalance { get; set; }
        public string status { get; set; }
        public string message { get; set; }
        public int errorCode { get; set; }
        
    }
}