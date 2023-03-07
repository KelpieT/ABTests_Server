using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EvrikaTestServer.Pages.Users
{
	public class DeleteModel : PageModel
	{
		public void OnGet()
		{
			int id = 0;
			if (int.TryParse(Request.Query["id"], out id))
			{
				MyServer.DeleteUser(id);
			}
			else
			{
				MyServer.DeleteAllUsers();
			}
			Response.Redirect("/Users/Users");
		}
	}
}
