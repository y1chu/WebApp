using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();


builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);  // Set the session timeout
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});


// Add GraphQL client services
builder.Services.AddSingleton(x =>
{
    var options = new GraphQLHttpClientOptions
    {
        EndPoint = new Uri("https://api.com/graphql"),
    };
    return new GraphQLHttpClient(options, new NewtonsoftJsonSerializer());
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();