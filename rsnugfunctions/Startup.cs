using System.Reflection;
using AzureFunctions.Extensions.Swashbuckle;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;
using Startup;

[assembly: WebJobsStartup(typeof(SwashBuckleStartup))]
namespace Startup
{
	internal class SwashBuckleStartup : IWebJobsStartup
	{
		public void Configure(IWebJobsBuilder builder)
		{
			//Register the extension
			builder.AddSwashBuckle(Assembly.GetExecutingAssembly());

		}
	}
}