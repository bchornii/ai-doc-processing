using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

// Application Insights telemetry for isolated worker
// DTS backend is configured in host.json extensions.durableTask.storageProvider
builder.Services
    .AddApplicationInsightsTelemetryWorkerService()
    .ConfigureFunctionsApplicationInsights();

/* TODO (T036): Register activity DI bindings
builder.Services.AddScoped<IGetDocumentFoldersActivity, PlaceholderGetDocumentFoldersActivity>();
builder.Services.AddScoped<IClassifyDocumentActivity,   PlaceholderClassifyDocumentActivity>();
builder.Services.AddScoped<IPersistResultActivity,      PlaceholderPersistResultActivity>();
builder.Services.AddScoped<IExtractInvoiceActivity,     PlaceholderExtractInvoiceActivity>();
builder.Services.AddScoped<IValidateInvoiceActivity,    PlaceholderValidateInvoiceActivity>();
builder.Services.AddScoped<IExtractContractActivity,    PlaceholderExtractContractActivity>();
builder.Services.AddScoped<IValidateContractActivity,   PlaceholderValidateContractActivity>();
builder.Services.AddScoped<IDetectBoundariesActivity,   PlaceholderDetectBoundariesActivity>();
builder.Services.AddScoped<IExtractGeneralDocumentActivity, PlaceholderExtractGeneralDocumentActivity>();
*/

builder.Build().Run();
