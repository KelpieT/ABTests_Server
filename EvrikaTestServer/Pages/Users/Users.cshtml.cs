using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EvrikaTestServer.Pages.Users
{
	public class UsersModel : PageModel
	{
		public List<User> Users { get; set; }
		public void OnGet()
		{

			var tempUsers = MyServer.GetUsers();
			if (tempUsers != null)
			{
				Users = tempUsers;
			}
			else
			{
				Users = new List<User>();
			}
		}
	}
}
