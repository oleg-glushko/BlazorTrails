using BlazingTrails.Client;
using BlazingTrails.Client.Features.Auth;
using BlazingTrails.Client.State;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddMediatR(options =>
{
    options.RegisterServicesFromAssembly(typeof(Program).Assembly);
});

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddHttpClient("SecureAPIClient", client =>
    client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
        .AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
Console.WriteLine(builder.HostEnvironment.BaseAddress);


builder.Services.AddOidcAuthentication(options =>
{
    builder.Configuration.Bind("Authentication", options.ProviderOptions);

    options.UserOptions.RoleClaim = "role";
    options.UserOptions.NameClaim = "name";
}).AddAccountClaimsPrincipalFactory<CustomUserFactory<RemoteUserAccount>>();

builder.Services.AddScoped<AppState>();
builder.Services.AddBlazoredLocalStorage();

await builder.Build().RunAsync();
