using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EvrikaTestServer.Pages.Test
{
	public class EditTestModel : PageModel
	{
		public ABTest abTest;
		public string errorMesage = string.Empty;
		public string successMesage = string.Empty;
		private int id;
		public void OnGet()
		{
			id = int.Parse(Request.Query["id"]);
			abTest = MyServer.GetABTest(id);
		}
		public void OnPost()
		{
			//try
			//{
			id = int.Parse(Request.Form["id"]);
			abTest = MyServer.GetABTest(id);
			abTest.name = Request.Form["name"];
			DateTime startDate = DateTime.MaxValue;
			DateTime endDate = DateTime.MaxValue;
			if (DateTime.TryParse(Request.Form["startDate"], out startDate))
			{
				abTest.startDate = startDate;
			}
			if (DateTime.TryParse(Request.Form["endDate"], out endDate))
			{
				abTest.endDate = endDate;
			}
			abTest.startCondition = Request.Form["startCondition"];
			abTest.endCondition = Request.Form["endCondition"];
			MyServer.UpdateTest(abTest);
			successMesage = "New test added";
			Redirect("/Test/Index");
		}
	}
}
