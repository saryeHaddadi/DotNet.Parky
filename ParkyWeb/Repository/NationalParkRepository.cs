using ParkyWeb.Models;
using ParkyWeb.Repository.Interface;

namespace ParkyWeb.Repository;

public class NationalParkRepository : Repository<NationalPark>, INationalParkRepository
{
	private readonly IHttpClientFactory _clientFactory;
	public NationalParkRepository(IHttpClientFactory clientFactory) : base(clientFactory)
	{
		_clientFactory = clientFactory;
	}

}
