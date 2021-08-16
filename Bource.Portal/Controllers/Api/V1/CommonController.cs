using AutoMapper;
using Bource.Data.Informations.UnitOfWorks;
using Bource.Portal.Services.CalculateServices;
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
        private readonly ISymbolCalculator symbolCalculator;

        public CommonController(IMapper mapper, IDistributedCache distributedCache, ITsetmcUnitOfWork tsetmcUnitOfWork, ISymbolCalculator symbolCalculator)
            : base(mapper, distributedCache)
        {
            this.tsetmcUnitOfWork = tsetmcUnitOfWork ?? throw new ArgumentNullException(nameof(tsetmcUnitOfWork));
            this.symbolCalculator = symbolCalculator ?? throw new ArgumentNullException(nameof(symbolCalculator));
        }

        [HttpGet]
        [Route("Header")]
        [ProducesResponseType(typeof(ApiResult<HeaderInformationResponse>), 200)]
        public async Task<IActionResult> GetHeaderInformation(CancellationToken cancellationToken)
        {
            await symbolCalculator.GetTurnoverAveragesAsync(2400322364771558, cancellationToken);
            HeaderInformationResponse result = new();
            return Ok(result);
        }
    }
}
