using Bource.WebConfiguration.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Bource.WebConfiguration.Api
{
    [ApiController]
    //[AllowAnonymous]
    [ApiVersion("1.0")]
    [ApiResultFilter]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class BaseApiController : ControllerBase
    {
        public bool UserIsAutheticated => HttpContext.User.Identity.IsAuthenticated;
    }
}