using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using OIDC.BlazorWebApp.Components;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpContextAccessor();

builder.Services.AddAuthentication(options =>
{
	options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
	options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
})
	.AddCookie()
	.AddOpenIdConnect(options =>
	{
		options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
		options.Authority = builder.Configuration["Okta:Domain"] + "/oauth2/default";
		options.RequireHttpsMetadata = true;
		options.ClientId = builder.Configuration["Okta:ClientId"];
		options.ClientSecret = builder.Configuration["Okta:ClientSecret"];
		options.ResponseMode = "query";
        options.ResponseType = OpenIdConnectResponseType.Code;
		options.GetClaimsFromUserInfoEndpoint = true;
		options.Scope.Add("openid");
		options.Scope.Add("profile");
		options.SaveTokens = true;
		options.SkipUnrecognizedRequests = true;
		options.TokenValidationParameters = new TokenValidationParameters
		{
			NameClaimType = "name",
			RoleClaimType = "groups",
			ValidateIssuer = true,
		};
	});


// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

app.UseCookiePolicy(new CookiePolicyOptions { MinimumSameSitePolicy = SameSiteMode.Lax });

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseWebSockets();

app.UseAuthentication();
app.UseAuthorization();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
