using AutoMapper;
using Bource.WebConfiguration.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using System;

namespace Bource.WebConfiguration.Api
{
    [ApiController]
    //[AllowAnonymous]
    [ApiVersion("1.0")]
    [ApiResultFilter]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class BaseApiController : ControllerBase
    {
        protected readonly IMapper mapper;
        protected readonly IDistributedCache distributedCache;

        public BaseApiController(IMapper mapper, IDistributedCache distributedCache)
        {
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this.distributedCache = distributedCache ?? throw new ArgumentNullException(nameof(distributedCache));
        }

        public bool UserIsAutheticated => HttpContext.User.Identity.IsAuthenticated;
    }
}