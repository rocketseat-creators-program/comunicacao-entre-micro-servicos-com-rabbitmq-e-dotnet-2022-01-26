using ExpertStore.SeedWork.RabbitProducer;
using PaymentsProcessor;
using PaymentsProcessor.UseCases;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IProcessPayment, ProcessPayment>();
builder.Services.AddRabbitMessageBus();
builder.Services.AddPaymentProcessorSubscriber();

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.Run();