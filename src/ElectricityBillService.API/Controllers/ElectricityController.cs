using ElectricityBillService.Core.Interfaces;
using ElectricityBillService.Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace ElectricityBillService.API.Controllers
{
   
    [ApiController]
    [Route("electricity")]
    public class ElectricityController : ControllerBase
    {
        private readonly IBillService _billService;
        private readonly ILogger<ElectricityController> _logger;    
        public ElectricityController(IBillService billService, ILogger<ElectricityController> logger)
        {
            _billService = billService;
            _logger = logger;   
        }

        [HttpPost("verify")]
        [ProducesResponseType(typeof(APIResult), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(APIResult), (int)HttpStatusCode.BadRequest)]        
        [ProducesResponseType(typeof(GenericErrorResponse), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> VerifyBill([FromBody] decimal amount)
        {
            try
            {
                var result = await _billService.CreateBillAsync(amount);
                if (!result.succeeded)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, result);

                }
                return StatusCode(StatusCodes.Status201Created, result);

            }
            catch(Exception ex)
            {
                _logger.LogError($"An error occurred, See details {ex.Message}, {ex.StackTrace}");
                return StatusCode(StatusCodes.Status500InternalServerError, new GenericErrorResponse { code = "UNEXPECTED_ERROR", message = "An unexpected error occurred" });
            }
           
        }

        [HttpPost("vend/{validationRef}/pay")]
        [ProducesResponseType(typeof(APIResult), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(APIResult), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(GenericErrorResponse), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> PayBill(string validationRef, Guid walletId)
        {

            try
            {
                var result = await _billService.ProcessPaymentAsync(validationRef, walletId.ToString());
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
