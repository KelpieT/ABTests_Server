using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EvrikaTestServer.Pages.Test.Groups
{
    public class EditGroupsModel : PageModel
    {
        public ABTest abTest { get; set; }
        public void OnGet()
        {
            int id = int.Parse(Request.Query["id"]);
            abTest = MyServer.GetABTest(id);
        }
        public void OnPost()
        {
            int id = int.Parse(Request.Form["id"]);
            abTest = MyServer.GetABTest(id);
            for (int i = 0; i < abTest.groups.Count; i++)
            {
                abTest.groups[i].value = float.Parse(Request.Form[("value_" + i)]);
                abTest.groups[i].serverParams.freeMoney = int.Parse(Request.Form[("freeMoney_" + i)]);
                abTest.groups[i].serverParams.expMyltyplier = float.Parse(Request.Form[("expMyltyplier_" + i)]);
                var enableAds = Request.Form[("enableADS_" + i)];
                abTest.groups[i].serverParams.enableADS = !string.IsNullOrEmpty(enableAds) ? 1 : 0;

            }
            MyServer.UpdateTest(abTest);

        }
    }
}
