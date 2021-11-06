using Microsoft.AspNetCore.Mvc;
using ReversePhoneLookup.Abstract.Services;

namespace ReversePhoneLookup.Web.Controllers
{
    [ApiController]
    public class OperatorController : ControllerBase
    {
        private readonly IPhoneService phoneService;
    }
}
