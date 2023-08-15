using ExampleGra.DAO;
using ExampleGra;
using ExampleGra.Datos;
using Microsoft.EntityFrameworkCore;
using ExampleGra.Repository;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<ExampleDBContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("cnexample"));
});


builder.Services.AddGraphQLServer()
    .AddQueryType<Query>()
    .AddMutationType<Mutation>()
    .AddSubscriptionType<Subscription>()
    .AddInMemorySubscriptions();

builder.Services.AddScoped<EmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<DepartmentRepository, DepartmentRepository>();
builder.Services.AddScoped<DbContextExtension, DbContextExtension>();

builder.Services.AddCors(option => {
    option.AddPolicy("allowedOrigin",
        builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()
        );
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();

app.MapGraphQL();

app.Run();
