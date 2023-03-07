using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EvrikaTestServer.Pages.Test
{
	public class IndexModel : PageModel
	{
		public List<ABTest> abTests = new List<ABTest>();
		public void OnGet()
		{
			abTests = MyServer.GetABTests();

		}		
	}
}
