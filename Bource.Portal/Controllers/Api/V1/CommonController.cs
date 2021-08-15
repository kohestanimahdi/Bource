using AutoMapper;
using Bource.Data.Informations.UnitOfWorks;
using Bource.Portal.ViewModels.Dtos.Responses;
using Bource.WebConfiguration.Api;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Threading;
using System.Threading.Tasks;
namespace Bource.Portal.Controllers.Api.V1
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommonController : BaseApiController
    {
        private readonly ITsetmcUnitOfWork tsetmcUnitOfWork;

        public CommonController(IMapper mapper, IDistributedCache distributedCache, ITsetmcUnitOfWork tsetmcUnitOfWork)
            : base(mapper, distributedCache)
        {
            this.tsetmcUnitOfWork = tsetmcUnitOfWork ?? throw new ArgumentNullException(nameof(tsetmcUnitOfWork));
        }

        [HttpGet]
        [Route("Header")]
        [ProducesResponseType(typeof(ApiResult<HeaderInformationResponse>), 200)]
        public async Task<IActionResult> GetHeaderInformation(CancellationToken cancellationToken)
        {
            HeaderInformationResponse result = new();
            return Ok(result);
        }
    }
}
