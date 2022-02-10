using Newtonsoft.Json;
using ParkyWeb.Models;
using ParkyWeb.Repository.Interface;
using System.Net;
using System.Net.Mime;
using System.Text;

namespace ParkyWeb.Repository;

public class AccountRepository : Repository<User>, IAccountRepository
{
	private readonly IHttpClientFactory _clientFactory;
	public AccountRepository(IHttpClientFactory clientFactory) : base(clientFactory)
	{
		_clientFactory = clientFactory;
	}

	public async Task<User> LoginAsync(string url, User objToCreate)
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
			return new User();
		}

		var client = _clientFactory.CreateClient();
		var response = await client.SendAsync(request);
		if (response.StatusCode == HttpStatusCode.OK)
		{
			var jsonString = await response.Content.ReadAsStringAsync();
			return JsonConvert.DeserializeObject<User>(jsonString);
		}
		else
		{
			return new User();
		}
	}

	public async Task<bool> RegisterAsync(string url, User objToCreate)
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
		var response = await client.SendAsync(request);
		return (response.StatusCode == HttpStatusCode.OK);
	}
}