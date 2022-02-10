using Newtonsoft.Json;
using ParkyWeb.Repository.Interface;
using System.Net.Mime;
using System.Net;
using System.Text;
using System.Net.Http.Headers;

namespace ParkyWeb.Repository;

public class Repository<T> : IRepository<T> where T : class
{
	private readonly IHttpClientFactory _clientFactory;

	public Repository(IHttpClientFactory clientFactory)
	{
		_clientFactory = clientFactory;
	}

	public async Task<bool> CreateAsync(string url, T objToCreate, string token = "")
	{
		var request = new HttpRequestMessage(HttpMethod.Post, url);
		if (objToCreate is not null)
		{
			request.Content = new StringContent(
				JsonConvert.SerializeObject(objToCreate),
				Encoding.UTF8, MediaTypeNames.Application.Json);
		}
		else
		{
			return false;
		}

		var client = _clientFactory.CreateClient();
		if (token is not null && token.Length != 0)
		{
			client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
		}

		var response = await client.SendAsync(request);
		return (response.StatusCode == HttpStatusCode.Created);

	}

	public async Task<bool> DeleteAsync(string url, int id, string token = "")
	{
		var request = new HttpRequestMessage(HttpMethod.Delete, url+id);
		var client = _clientFactory.CreateClient();
		if (token is not null && token.Length != 0)
		{
			client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
		}
		var response = await client.SendAsync(request);
		return (response.StatusCode == HttpStatusCode.NoContent);
	}

	public async Task<IEnumerable<T>> GetAllAsync(string url, string token = "")
	{
		var request = new HttpRequestMessage(HttpMethod.Get, url);
		var client = _clientFactory.CreateClient();
		if (token is not null && token.Length != 0)
		{
			client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
		}
		var response = await client.SendAsync(request);
		if (response.StatusCode == HttpStatusCode.OK)
		{
			var jsonString = await response.Content.ReadAsStringAsync();
			return JsonConvert.DeserializeObject<IEnumerable<T>>(jsonString);
		}
		return null;
	}

	public async Task<T> GetAsync(string url, int id, string token = "")
	{
		var request = new HttpRequestMessage(HttpMethod.Get, url+ id);
		var client = _clientFactory.CreateClient();
		if (token is not null && token.Length != 0)
		{
			client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
		}
		var response = await client.SendAsync(request);
		if (response.StatusCode == HttpStatusCode.OK)
		{
			var jsonString = await response.Content.ReadAsStringAsync();
			return JsonConvert.DeserializeObject<T>(jsonString);
		}
		return null;
	}

	public async Task<bool> UpdateAsync(string url, T objToUpdate, string token = "")
	{
		var request = new HttpRequestMessage(HttpMethod.Patch, url);
		if (objToUpdate is not null)
		{
			request.Content = new StringContent(
				JsonConvert.SerializeObject(objToUpdate),
				Encoding.UTF8, MediaTypeNames.Application.Json);
		}
		else
		{
			return false;
		}

		var client = _clientFactory.CreateClient();
		if (token is not null && token.Length != 0)
		{
			client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
		}
		var response = await client.SendAsync(request);
		return (response.StatusCode == HttpStatusCode.NoContent);
	}
}
