﻿using Bucket.Core;
using Bucket.Exceptions;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;

namespace Bucket.AspNetCore.Filters
{
    public class WebApiActionFilterAttribute : ActionFilterAttribute
    {
        private readonly IJsonHelper jsonHelper;
        /// <summary>
        /// Action 过滤器
        /// </summary>
        public WebApiActionFilterAttribute(IJsonHelper jsonHelper) {
            this.jsonHelper = jsonHelper;
        }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            #region 自定义模型验证
            if (!context.ModelState.IsValid)
            {
                var message = string.Empty;
                //获取第一个错误提示
                foreach (var key in context.ModelState.Keys)
                {
                    var state = context.ModelState[key];
                    if (state.Errors.Any())
                    {
                        message = state.Errors.First().ErrorMessage;
                        break;
                    }
                }
                if (!string.IsNullOrWhiteSpace(message))
                {
                    var errorInfo = jsonHelper.DeserializeObject<ErrorResult>(message);
                    if (errorInfo != null)
                        throw new BucketException(errorInfo.ErrorCode, errorInfo.Message);
                    else
                        throw new BucketException("-1", message);
                }
            }
            #endregion
        }
    }
}
