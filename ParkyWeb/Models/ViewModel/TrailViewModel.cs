using Microsoft.AspNetCore.Mvc.Rendering;

namespace ParkyWeb.Models.ViewModel;

public class TrailViewModel
{
	public IEnumerable<SelectListItem> NationalParkList { get; set; }
	public Trail Trail { get; set; }
}
