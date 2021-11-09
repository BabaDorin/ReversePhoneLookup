using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ReversePhoneLookup.Abstract.Services;
using ReversePhoneLookup.Models.Requests;

namespace ReversePhoneLookup.Web.Controllers
{
    [ApiController]
    public class PhoneController : ControllerBase
    {
        private readonly ILookupService lookupService;
        private readonly IPhoneService phoneService;

        public PhoneController(
            ILookupService lookupService,
            IPhoneService phoneService)
        {
            this.lookupService = lookupService;
            this.phoneService = phoneService;
        }

        [HttpGet("/lookup")]
        public async Task<IActionResult> Lookup(
            [FromQuery] LookupRequest request, 
            CancellationToken cancellationToken)
        {
            var result = await lookupService.LookupAsync(request, cancellationToken);
            return Ok(result);
        }

        [HttpPut("/phones")]
        public async Task<IActionResult> AddPhone(
            [FromBody] UpsertPhoneRequest request,
            CancellationToken cancellationToken)
        {
            var response = await phoneService.AddOrUpdatePhoneAsync(request, cancellationToken);

            switch (response.StatusCode)
            {
                case Models.StatusCode.Conflict:
                    return Conflict();

                case Models.StatusCode.Created:
                    return Created(
                        new Uri($"{Request.Path}/{response.Data}", UriKind.Relative), 
                        response.Data);

                case Models.StatusCode.Updated:
                    return NoContent();

                default:
                    return Ok();
            }
        }
    }
}
