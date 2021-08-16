using Bource.WebConfiguration.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.OpenApi.Models;
using SignalRSwaggerGen.Attributes;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bource.Portal.Controllers.SignalR.V1
{
    [ApiResultFilter]
    [ApiVersion("1.0")]
    [SignalRHub(path: "/hubs/SymbolsHub")]
    public class SymbolsHub : Hub
    {
        private static object symbolDataUsersObject = new();
        public static ConcurrentDictionary<long, List<string>> SymbolDataUsers = new();
        public SymbolsHub()
        {

        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            lock (symbolDataUsersObject)
            {
                foreach (var key in SymbolDataUsers.Keys)
                {
                    if (SymbolDataUsers[key].Any(i => i == Context.ConnectionId))
                    {
                        SymbolDataUsers[key].Remove(Context.ConnectionId);
                        if (!SymbolDataUsers[key].Any())
                            SymbolDataUsers.TryRemove(key, out _);

                        break;
                    }
                }
            }

            return base.OnDisconnectedAsync(exception);
        }

        private void addToSymbolDataUser(long insCode)
        {
            lock (symbolDataUsersObject)
            {
                if (!SymbolDataUsers.ContainsKey(insCode))
                    SymbolDataUsers[insCode] = new List<string>();

                if (!SymbolDataUsers[insCode].Any(i => i == Context.ConnectionId))
                    SymbolDataUsers[insCode].Add(Context.ConnectionId);
            }
        }


        [SignalRMethod(name: "GetSymbolData", operationType: OperationType.Get)]
        public async Task<object> GetSymbolData([SignalRArg] long insCode)
        {
            addToSymbolDataUser(insCode);

            await Groups.AddToGroupAsync(Context.ConnectionId, $"SymbolData-{insCode}");

            return new { Name = "Mahdi", InsCode = insCode };
        }


    }
}
