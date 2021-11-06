using Microsoft.AspNetCore.Mvc;
using ReversePhoneLookup.Abstract.Services;
using ReversePhoneLookup.Models.ViewModels;
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
            [FromBody] OperatorViewModelIn @operator,
            CancellationToken cancellationToken)
        {
            var id = await operatorService.AddOperatorAsync(@operator, cancellationToken);

            // RequestPath/id is not implemented yet, but still :) I want to return 201
            return Created(new Uri($"{Request.Path}/{id}", UriKind.Relative), id);
        }
    }
}
