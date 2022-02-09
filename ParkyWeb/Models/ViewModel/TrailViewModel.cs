﻿using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ParkyWeb.Models.ViewModel;

public class TrailViewModel
{
	[ValidateNever]
	public IEnumerable<SelectListItem> NationalParkList { get; set; }
	public Trail Trail { get; set; }
}
