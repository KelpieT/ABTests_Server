using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EvrikaTestServer.Pages.Test
{
	public class NewTestModel : PageModel
	{
		public ABTest abTest = new ABTest();
		public string errorMesage = string.Empty;
		public string successMesage = string.Empty;
		public void OnGet()
		{
			abTest.id = MyServer.GetNewIdForTest();
		}
		public void OnPost()
		{
			//try
			//{
			abTest.id = MyServer.GetNewIdForTest();
			abTest.name = Request.Form["name"];
			DateTime startDate = DateTime.MinValue;
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
			MyServer.CreateNewTest(abTest);
			successMesage = "New test added";
			abTest = new ABTest();
			abTest.id = MyServer.GetNewIdForTest();
			//}
			//catch (Exception ex)
			//{
			//	errorMesage = ex.Message;
			//}
		}
	}
}

