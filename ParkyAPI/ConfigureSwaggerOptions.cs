﻿using Microsoft.AspNetCore.Mvc.ApiExplorer;
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

        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Description =
               "JWT Authorization header using the Bearer scheme. \r\n\r\n " +
               "Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\n" +
               "Example: \"Bearer 12345abcdef\"",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.ApiKey,
            Scheme = "Bearer"
        });

        options.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header,

                        },
                        new List<string>()
                    }
                });

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
