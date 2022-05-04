using Beis.Htg.VendorSme.Database.Models;
using Microsoft.Extensions.Logging;
using Beis.HelpToGrow.Voucher.Api.Check.Common;
using System;
using System.Text.Json;
using System.Threading.Tasks;
using Beis.HelpToGrow.Voucher.Api.Check.Interfaces;
using Beis.HelpToGrow.Voucher.Api.Check.Domain.Entities;

namespace Beis.HelpToGrow.Voucher.Api.Check.Services
{
    public class VoucherCheckService : IVoucherCheckService
    {
        const string tokenExpiryFormat = "yyyy-MM-dd'T'HH:mm:ss";
        private readonly IEncryptionService _encryptionService;
        private readonly ITokenRepository _tokenRepository;
        private readonly IProductRepository _productRepository;
        private readonly IVendorCompanyRepository _vendorCompanyRepository;
        private readonly IEnterpriseRepository _enterpriseRepository;
        private ILogger<VoucherCheckService> _logger;
        private IVendorAPICallStatusServices _vendorApiCallStatusService;

        public VoucherCheckService(ILogger<VoucherCheckService> logger, IEncryptionService encryptionService, ITokenRepository tokenRepository, IProductRepository productRepository, IVendorCompanyRepository vendorCompanyRepository, IEnterpriseRepository enterpriseRepository, IVendorAPICallStatusServices vendorApiCallStatusService)
        {
            _encryptionService = encryptionService;
            _tokenRepository = tokenRepository;
            _productRepository = productRepository;
            _vendorCompanyRepository = vendorCompanyRepository;
            _enterpriseRepository = enterpriseRepository;
            _logger = logger;
            _vendorApiCallStatusService = vendorApiCallStatusService;
        }

        private void logAPiCallStatus(vendor_api_call_status vendor_api_call_status, VoucherResponse voucherResponse)
        {
            vendor_api_call_status.result = JsonSerializer.Serialize(voucherResponse);
            _vendorApiCallStatusService.LogRequestDetails(vendor_api_call_status);
        }

        private VoucherResponse getVoucherErrorResponse(VoucherRequest voucherRequest, int errorCode, string message)
        {
            _logger.LogError("There was an error checking the voucher ({errorCode}) : {Message}", errorCode, message);
            var vendorApiCallStatus = _vendorApiCallStatusService.CreateLogRequestDetails(voucherRequest);

            var voucherResponse = new VoucherResponse
            {
                status = "ERROR",
                errorCode = errorCode,
                message = message,
                voucherCode = voucherRequest.voucherCode
            };
            

            vendorApiCallStatus.error_code = "400";
            logAPiCallStatus(vendorApiCallStatus, voucherResponse);
            return voucherResponse;
        }
        public async Task<VoucherResponse> GetVoucherResponse(VoucherRequest voucherRequest)
        {
            _logger.LogInformation("VoucherCheckServiceRequest: {@VoucherRequest}", JsonSerializer.Serialize(voucherRequest));
         
            try
            {
                _logger.LogInformation("Getting vendor company for registration {registration}", voucherRequest.registration);
                var vendorCompany = _vendorCompanyRepository.GetVendorCompanyByRegistration(voucherRequest.registration);
                if (vendorCompany != null)
                {
                    _logger.LogInformation("Decrypting voucher");
                    var decryptedVoucherCode = DecryptVoucher(voucherRequest, vendorCompany);
                    _logger.LogInformation("Getting token for decrypted voucher code {decryptedVoucherCode}", decryptedVoucherCode);
                    var token = GetToken(decryptedVoucherCode);
                    _logger.LogInformation("Got Token for decrypted voucher code {code}", decryptedVoucherCode);
                    if (token != null)
                    {
                        // reject tokens with any cancellation status
                        if (token.cancellation_status_id.HasValue)
                        {
                            return getVoucherErrorResponse(voucherRequest, 50, "Cancelled token");
                        }
                        var productId = token.product;
                        _logger.LogInformation("Getting product for product Id {productId}", productId);
                        var product = await _productRepository.GetProductSingle(productId);

                        if (IsValidVendor(voucherRequest, vendorCompany))
                        {
                            return await GetVoucherCheckResponse(token, voucherRequest, vendorCompany, product);
                        }
                        return getVoucherErrorResponse(voucherRequest, 10, "Unknown token Invalid vendor details");
                    }
                    
                    return getVoucherErrorResponse(voucherRequest, 10, "Unknown token Unknown token");
                }
                return getVoucherErrorResponse(voucherRequest, 10, "Unknown token Invalid Vendor company");
                
            }

            catch (Exception e)
            {                
                _logger.LogError(e, "There was an unxpected error checking the voucher : {Message}", e.Message);
                return getVoucherErrorResponse(voucherRequest, 10, e.Message);
                
            }
        }

        private token GetToken(string decryptedVoucherCode)
        {
            var token = _tokenRepository.GetToken(decryptedVoucherCode);

            return token;
        }

        private async Task<VoucherResponse> GetVoucherCheckResponse(token token, VoucherRequest voucherRequest, vendor_company vendorCompanySingle, product product)
        {
            //Check balance > 0
            //check token status
            //check token expiry
            
            _logger.LogInformation("Getting voucher response");
            if (token.token_expiry.CompareTo(DateTime.Now) < 0)
            {
                return getVoucherErrorResponse(voucherRequest, 20, "Expired Token");
            }

            if (token.token_balance == 0)
            {
                return getVoucherErrorResponse(voucherRequest, 30, "No Balance");
            }

            if (token.authorisation_code == null)
            {
                return getVoucherErrorResponse(voucherRequest, 40, "Locked");
            }

            var voucherResponse = new VoucherResponse();
            voucherResponse.status = "OK";
            voucherResponse.message = "Successful check - proceed with Voucher";
            voucherResponse.errorCode = 0;
            voucherResponse.voucherCode = voucherRequest.voucherCode;
            voucherResponse.authorisationCode = token.authorisation_code;

            voucherResponse.vendor = vendorCompanySingle.vendor_company_name;
            voucherResponse.productSku = product.product_SKU;
            voucherResponse.productName = product.product_name;
            _logger.LogInformation("Getting enterprise for id {id}", token.enterprise_id);
            var enterprise = await _enterpriseRepository.GetEnterprise(token.enterprise_id);

            voucherResponse.licenceTo = enterprise.enterprise_name;
            voucherResponse.smeEmail = enterprise.applicant_email_address;
            voucherResponse.purchaserName = enterprise.applicant_name;

            voucherResponse.maxDiscountAllowed = token.token_balance;
            voucherResponse.currency = "GBP";
            voucherResponse.tokenExpiry = token.token_expiry.ToString(tokenExpiryFormat);


            var vendorApiCallStatus = _vendorApiCallStatusService.CreateLogRequestDetails(voucherRequest);
            vendorApiCallStatus.error_code = "200";
            logAPiCallStatus(vendorApiCallStatus, voucherResponse);

            return voucherResponse;
        }

        private string DecryptVoucher(VoucherRequest voucherRequest, vendor_company vendorCompany)
        {
            var voucherCode = voucherRequest.voucherCode + "==";
            return _encryptionService.Decrypt(voucherCode, vendorCompany.registration_id + vendorCompany.vendorid);
        }

        private bool IsValidVendor(VoucherRequest voucherRequest, vendor_company vendorCompany)
        {
            return voucherRequest.registration == vendorCompany.registration_id &&
                voucherRequest.accessCode == vendorCompany.access_secret;
        }
    }
}

