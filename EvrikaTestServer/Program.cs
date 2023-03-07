var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.MapGet("/GetNewUserID", MyServer.GetNewUserID);
app.MapGet("/GetAllTests", MyServer.GetAllTests);
app.MapGet("/SetTestsToUser", MyServer.SetTestsToUser);
app.MapGet("/GetServerParams", MyServer.GetServerParams);


app.Run();
