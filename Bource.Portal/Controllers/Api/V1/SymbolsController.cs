using AutoMapper;
using AutoMapper.QueryableExtensions;
using Bource.Common.Utilities;
using Bource.Data.Informations.UnitOfWorks;
using Bource.Models.Data.Common;
using Bource.Portal.ViewModels.Dtos.SymbolDtos;
using Bource.WebConfiguration.Api;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Bource.Portal.Controllers.Api.V1
{
    public class SymbolsController : BaseApiController
    {
        private readonly ITsetmcUnitOfWork tsetmcUnitOfWork;

        public SymbolsController(IMapper mapper, IDistributedCache distributedCache, ITsetmcUnitOfWork tsetmcUnitOfWork)
            : base(mapper, distributedCache)
        {
            this.tsetmcUnitOfWork = tsetmcUnitOfWork ?? throw new ArgumentNullException(nameof(tsetmcUnitOfWork));
        }

        /// <summary>
        /// Get list of symbols with search in Name, Sign, Company Name or Code
        /// </summary>
        /// <param name="search"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(List<SymbolListResponse>))]
        public async Task<IActionResult> GetSymbolsListAsync([FromQuery] string search, CancellationToken cancellationToken)
        {
            // cache in redis
            List<Symbol> symbols = await distributedCache.GetValueAsync<List<Symbol>>("SymbolsList");
            if (symbols is null || !symbols.Any())
            {
                symbols = await tsetmcUnitOfWork.GetSymbolsAsync(cancellationToken);
                await distributedCache.SetValueAsync("SymbolsList", symbols, 5);
            }

            // search text in list
            if (!string.IsNullOrWhiteSpace(search))
                symbols = symbols.Where(i => i.Sign.Contains(search) || (i.CompanyName?.Contains(search) ?? false) || i.Name.Contains(search) || i.InsCodeValue == search).ToList();

            // map to response type
            var response = symbols.AsQueryable().ProjectTo<SymbolListResponse>(mapper.ConfigurationProvider);

            return Ok(response);
        }
    }
}
