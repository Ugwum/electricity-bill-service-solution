using ElectricityBillService.Core.Interfaces;
using ElectricityBillService.Core.Models;
using ElectricityBillService.Core.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ElectricityBillService.API.Controllers
{
    
    [ApiController]
    [Route("wallets")]
    public class WalletController : ControllerBase
    {
        private readonly IWalletService _walletService;
        private readonly ILogger<WalletController> _logger;
        public WalletController(IWalletService walletService, ILogger<WalletController> logger)
        {
            _walletService = walletService;
            _logger = logger;   
        }

        [HttpPost("{walletId}/addFunds")]
        [ProducesResponseType(typeof(APIResult), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(APIResult), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(GenericErrorResponse), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> AddFunds(Guid walletId, [FromBody] decimal amount)
        {
            try
            { 
                var result = await _walletService.AddFundsAsyn(walletId, amount);
                if (!result.succeeded)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, result);

                }
                return StatusCode(StatusCodes.Status200OK, result);

            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred, See details {ex.Message}, {ex.StackTrace}");
                return StatusCode(StatusCodes.Status500InternalServerError, new GenericErrorResponse { code = "UNEXPECTED_ERROR", message = "An unexpected error occurred" });
            }             
        }
    }
}
