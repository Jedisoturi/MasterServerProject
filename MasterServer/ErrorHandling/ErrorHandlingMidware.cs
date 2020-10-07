using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace MasterServer
{
	public class ErrorHandlingMidware
	{
		private readonly RequestDelegate _next;
		public ErrorHandlingMidware(RequestDelegate next)
		{
			_next = next;
		}
		public async Task Invoke(HttpContext context)
		{
			try
			{
				await _next(context);
			}
			catch (IdNotFoundException e)
			{
				string ex = e.ToString();
				Console.WriteLine(ex);
				context.Response.StatusCode = (int)HttpStatusCode.OK;
			}
		}
		/*
		public class ImplExceptionFilterAttribute : ExceptionFilterAttribute
		{
			public override void OnException(ExceptionContext context)
			{
				if (context.Exception is TooLowLevelException e)
				{
					Console.WriteLine(e.Message);
					string ex = e.ToString();
					context.Result = new BadRequestObjectResult(ex);
				}
			}
		}*/
	}
}