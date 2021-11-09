using Microsoft.AspNetCore.Mvc;
using ReversePhoneLookup.Abstract.Services;
using ReversePhoneLookup.Models.Requests;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ReversePhoneLookup.Web.Controllers
{
    [ApiController]
    public class OperatorController : ControllerBase
    {
        private readonly IOperatorService operatorService;

        public OperatorController(IOperatorService operatorService)
        {
            this.operatorService = operatorService;
        }

        [HttpPost("/operators")]
        public async Task<IActionResult> AddOperator(
            [FromBody] CreateOperatorRequest request,
            CancellationToken cancellationToken)
        {
            var response = await operatorService.AddOperatorAsync(request, cancellationToken);

            return Created(
                new Uri($"{Request.Path}/{response.Data}", UriKind.Relative), 
                response.Data);
        }
    }
}
