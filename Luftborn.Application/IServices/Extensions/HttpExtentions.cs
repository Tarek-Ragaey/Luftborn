using Microsoft.AspNetCore.Http;
using Luftborn.Application.IServices.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Luftborn.Application.IServices.Extensions
{
    public static class HttpExtensions
    {
        public static void AddPaginationHeader(this HttpResponse response, PaginationHeader header)
        {
            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            response.Headers.Add("X-Pagination", JsonSerializer.Serialize(header, options));
            response.Headers.Add("Access-Control-Expose-Headers", "X-Pagination");
        }
    }
}
