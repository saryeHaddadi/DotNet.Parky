using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace ParkyAPI;

public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
{
	public readonly IApiVersionDescriptionProvider _provider;

	public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider) => _provider = provider;
	public void Configure(SwaggerGenOptions options)
	{
		foreach (var desc in _provider.ApiVersionDescriptions)
		{
			options.SwaggerDoc(desc.GroupName, CreateVersionInfo(desc));
		}

		var xmlCommentFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
		var xmlCommentFileFullPath = Path.Combine(AppContext.BaseDirectory, xmlCommentFile);
		options.IncludeXmlComments(xmlCommentFileFullPath);
	}

    /// <summary>
    /// Create information about the version of the API
    /// </summary>
    /// <param name="description"></param>
    /// <returns>Information about the API</returns>
    private OpenApiInfo CreateVersionInfo(ApiVersionDescription desc)
    {
        var info = new OpenApiInfo()
        {
            Title = $"Parky API { desc.ApiVersion }",
            Version = desc.ApiVersion.ToString()
        };

        if (desc.IsDeprecated)
        {
            info.Description += " This API version has been deprecated. Please use one of the new APIs available from the explorer.";
        }
        return info;
    }
}
