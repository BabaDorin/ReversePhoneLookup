using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ReversePhoneLookup.Abstract.Services;
using ReversePhoneLookup.Models.Requests;
using ReversePhoneLookup.Models.ViewModels;

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
        public async Task<IActionResult> Lookup([FromQuery] LookupRequest request, CancellationToken cancellationToken)
        {
            var result = await lookupService.LookupAsync(request, cancellationToken);
            return Ok(result);
        }

        [HttpPost("/phones")]
        public async Task<IActionResult> AddPhone(
            [FromBody] PhoneViewModelIn phone,
            CancellationToken cancellationToken)
        {
            var id = await phoneService.AddPhoneAsync(phone, cancellationToken);

            // RequestPath/id is not implemented yet, but still :) I want to return 201
            return Created(new Uri($"{Request.Path}/{id}", UriKind.Relative), id);
        }
    }
}
