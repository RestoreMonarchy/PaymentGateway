using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation;
using Microsoft.Extensions.FileProviders;
using Quartz;
using RestoreMonarchy.PaymentGateway.API.Abstractions;
using RestoreMonarchy.PaymentGateway.API.Services;
using RestoreMonarchy.PaymentGateway.Providers.Mock;
using RestoreMonarchy.PaymentGateway.Providers.Nano;
using RestoreMonarchy.PaymentGateway.Providers.PayPal;
using RestoreMonarchy.PaymentGateway.Web.Blazor.Services;
using RestoreMonarchy.PaymentGateway.Web.Repositories;
using RestoreMonarchy.PaymentGateway.Web.Services;
using RestoreMonarchy.PaymentGateway.Web.Services.Jobs;
using System.Data.SqlClient;
using System.Reflection;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<BlazorUser>();

builder.Services.AddRazorPages((o) => 
{
    o.RootDirectory = "/Blazor/Pages";
});
builder.Services.AddServerSideBlazor();
builder.Services.AddHttpContextAccessor();

// Add SqlConnection and data repositories
builder.Services.AddTransient(x => new SqlConnection(builder.Configuration.GetConnectionString("Default")));
builder.Services.AddTransient<StoresRepository>();
builder.Services.AddTransient<PaymentsRepository>();
builder.Services.AddTransient<UsersRepository>();

// Add always necessary services
builder.Services.AddTransient<PaymentInternalService>();
builder.Services.AddTransient<NotifyService>();
builder.Services.AddTransient<UserService>();

Assembly[] assemblies = new Assembly[]
{
    typeof(MockPaymentProvider).Assembly,
    typeof(PayPalPaymentProvider).Assembly
};

List<IPaymentProviderPlugin> plugins = new List<IPaymentProviderPlugin>();

foreach (Assembly assembly in assemblies)
{
    Type[] types = assembly.GetTypes();

    Type pluginType = types.FirstOrDefault(x => x.IsAssignableTo(typeof(PaymentProviderPlugin)));

    if (pluginType != null)
    {
        IPaymentProviderPlugin plugin = (IPaymentProviderPlugin)Activator.CreateInstance(pluginType);
        if (plugin != null)
        {
            plugins.Add(plugin);
        }
    }
}

builder.Services.AddTransient<IPaymentProviders, PaymentProviders>(
    (provider) => new PaymentProviders(provider.GetServices<IPaymentProvider>()));

// Add API services
builder.Services.AddTransient<IBaseUrl, BaseUrlService>();
builder.Services.AddTransient(typeof(IPaymentService), typeof(PaymentService));


// Add Quartz services
builder.Services.AddTransient<PaymentNotifyJob>();

builder.Services.AddQuartz(q => 
{
    q.SchedulerId = "JobScheduler";
    q.SchedulerName = "Job Scheduler";
    
    q.UseMicrosoftDependencyInjectionJobFactory();
    q.UsePersistentStore(x =>
    {
        x.UseSqlServer(builder.Configuration.GetConnectionString("Default"));
        x.UseJsonSerializer();
    });
});
builder.Services.AddQuartzServer(options => 
{ 
    options.WaitForJobsToComplete = true;
});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
    });

builder.Services.AddAuthorization(options => 
{
    AuthorizationPolicyBuilder authBuilder = new(CookieAuthenticationDefaults.AuthenticationScheme);
    authBuilder.RequireAuthenticatedUser();
    options.DefaultPolicy = authBuilder.Build();
});

builder.Services.AddHttpClient();
builder.Services.AddControllersWithViews();

foreach (IPaymentProviderPlugin plugin in plugins)
{
    plugin.ConfigureServices(builder.Services, builder.Configuration);
}

WebApplication app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

foreach (IPaymentProviderPlugin plugin in plugins)
{
    plugin.Configure(app, builder.Configuration);
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();
app.UseAuthentication();

app.MapControllers();
app.MapFallbackToPage("/_Host");

app.UseEndpoints(endpoints =>
{
    endpoints.MapBlazorHub();
    endpoints.MapDefaultControllerRoute();
});

app.Run();