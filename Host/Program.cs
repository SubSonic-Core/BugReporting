using SubSonic.Configuration;
using SubSonic.Notification;

var builder = WebApplication.CreateBuilder(args);

if(!builder.Environment.IsTestIntegration())
{
    var section = builder.Configuration.GetRequiredSection("Settings");

    builder.Services.AddEnvironment(opt =>
    {
        opt.Environment = section.GetRequiredValue<string>("Environment");
    });

    builder.Services
        .AddHttpContextAccessor()
        .AddControllers();

    builder.Services.AddNotification(notify =>
    {
        notify.Configure(options =>
        { 
            options.Enabled = true;
        });

        notify.AddWebNotification(web =>
        {
            
        });
    });

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
}

// program is primarily hosted in a test framework for integration testing of packages
var app = builder.Build();

if (!app.Environment.IsTestIntegration())
{

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseNotificationServices();

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();
}

app.Run();
