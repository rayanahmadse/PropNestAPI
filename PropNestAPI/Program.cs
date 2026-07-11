using PropNestAPI;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Repositories
builder.Services.AddSingleton<TenantRepository>();
builder.Services.AddSingleton<PropertyUnitRepository>();
builder.Services.AddSingleton<RentalAgreementRepository>();
builder.Services.AddSingleton<RentPaymentRepository>();
builder.Services.AddSingleton<RentalAgreementRepository>();
builder.Services.AddSingleton<TenantRepository>();
builder.Services.AddSingleton<PropertyUnitRepository>();
builder.Services.AddSingleton<PropNestAPI.Services.PdfReceiptGenerator>();

builder.Services.AddSingleton<StaffRepository>();
builder.Services.AddSingleton<MaintenanceRequestRepository>();
builder.Services.AddSingleton<UserRepository>();
// Background worker for auto-closing old maintenance requests
builder.Services.AddHostedService<PropNestAPI.Workers.MaintenanceAutoCloseWorker>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
