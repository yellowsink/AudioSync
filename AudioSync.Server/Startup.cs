using System.Linq;
using AudioSync.Server.Hubs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AudioSync.Server
{
	public class Startup
	{
		public Startup(IConfiguration configuration) { Configuration = configuration; }

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddSingleton<IDataService, DataService>();

			services.AddSignalR();
			services.AddControllers();
			services.AddResponseCompression(opts =>
											{
												opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
												 new[] { "application/octet-stream" });
											});
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			app.UseResponseCompression();
			
			if (env.IsDevelopment()) app.UseDeveloperExceptionPage();

			app.UseHttpsRedirection();

			app.UseRouting();

			app.UseAuthorization();

			app.UseEndpoints(endpoints =>
							 {
								 endpoints.MapControllers();
								 endpoints.MapHub<SyncHub>("/synchub");
							 });
		}
	}
}