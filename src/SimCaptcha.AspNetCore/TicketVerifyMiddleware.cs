﻿using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using SimCaptcha.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SimCaptcha.AspNetCore
{
    /// <summary>
    /// ticket效验 - 给业务后台验证使用
    /// </summary>
    public class TicketVerifyMiddleware : SimCaptchaMiddleware
    {
        public TicketVerifyMiddleware(RequestDelegate next, IOptions<SimCaptchaOptions> optionsAccessor, IMemoryCache memoryCache, IHttpContextAccessor accessor) : base(next, optionsAccessor, memoryCache, accessor)
        { }

        public override async Task InvokeAsync(HttpContext context)
        {
            string inputBody;
            using (var reader = new System.IO.StreamReader(
                context.Request.Body, Encoding.UTF8))
            {
                inputBody = await reader.ReadToEndAsync();
            }
            TicketVerifyModel ticketVerify = _jsonHelper.Deserialize<TicketVerifyModel>(inputBody);

            // ticket 效验
            TicketVerifyResponseModel responseModel = _service.TicketVerify(ticketVerify.AppId, ticketVerify.AppSecret, ticketVerify.Ticket, ticketVerify.UserId, ticketVerify.UserIp);
            string responseJsonStr = _jsonHelper.Serialize(responseModel);

            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(responseJsonStr, Encoding.UTF8);

            // Response.Write 开始, 不要再 Call next
            // Call the next delegate/middleware in the pipeline
            //await _next(context);
        }
    }
}
