using ParkyWeb.Models;
using ParkyWeb.Repository.Interface;

namespace ParkyWeb.Repository;

public class TrailRepository : Repository<Trail>, ITrailRepository
{
	private readonly IHttpClientFactory _clientFactory;
	public TrailRepository(IHttpClientFactory clientFactory) : base(clientFactory)
	{
		_clientFactory = clientFactory;
	}

}
