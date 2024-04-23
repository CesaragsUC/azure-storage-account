using Azure.Data.Tables;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Specialized;
using Azure.Storage.Queues;
using Microsoft.Extensions.Azure;
using Storage.Account.Demo.Services;
using Storage.Account.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddScoped<ITableStorageService, TableStorageService>();
builder.Services.AddScoped<IBobStorageService, BobStorageService>();
builder.Services.AddScoped<IQueueService, QueueService>();

#region AzureClients

// Adicionando os clientes do Azure
var connectionString = builder.Configuration["ConnectionStrings:StorageConnectionStrings"];

builder.Services.AddAzureClients(builder => {
    builder.AddBlobServiceClient(connectionString);

});

//desse modo quando eu chamar o QueueClient _queueClient no construtor da classe QueueService
//ele vai injetar a instancia do QueueClient com a string de conexao e suas configurações
//mesma logica para os outros clientes
builder.Services.AddAzureClients(b=> {

    b.AddClient<QueueClient, QueueClientOptions>((_) => { 
        return new QueueClient(connectionString, builder.Configuration["ConnectionStrings:QueueName"], 
        new QueueClientOptions
        {
            MessageEncoding = QueueMessageEncoding.Base64
        });
    });

    b.AddClient<TableClient, TableClientOptions>((_) => {
        return new TableClient(connectionString, builder.Configuration["ConnectionStrings:TableName"]);
    });

});
#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
