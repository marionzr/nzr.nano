using Microsoft.OpenApi.Models;
using Nzr.Nano;
using Nzr.Nano.Demo.Dtos;
using Nzr.Nano.Demo.Entities;
using Nzr.Nano.Extensions;

var category = new ProductCategory() { Id = 1, Name = "Devices & Electronics" };

List<IProduct> products =
[
    new Product { Id = 1, Name = "Laptop", Category = category },
    new Product { Id = 2, Name = "Phone" , Category = category},
    new VirtualProduct { Id = 3, Name = "Tablet", Category = category }
];

var builder = WebApplication.CreateBuilder(args);

#region Initialize Nano
var configuration = builder.Configuration;
var nanoOptions = configuration.GetSection("NanoOptions").Get<NanoOptions>()!;
Nano.Initialize(nanoOptions);
#endregion

builder.Services.AddControllers();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Nzr.Nano.Demo" });
});

var app = builder.Build();

GetProducts(products, app);
GetProductById(products, app);
DeleteProductById(products, app);
GetProductCategoryById(products, app);

app.MapControllers();
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Nzr.Nano.Demo");
    c.RoutePrefix = string.Empty;
});

await app.RunAsync();


#region Controllers

static void GetProducts(List<IProduct> products, WebApplication app)
{
    app.MapGet("/api/products/", () =>
    {
        var output = new List<ProductOutput>();

        foreach (var product in products)
        {
            output.Add(new ProductOutput(product));
        }

        return Results.Ok(output);
    });
}

static void GetProductById(List<IProduct> products, WebApplication app)
{
    app.MapGet("/api/products/{id}", (string id) =>
    {
        var internalId = id.Deobfuscate<IProduct>();
        var product = products.SingleOrDefault(p => p.Id == internalId);

        if (product is null)
        {
            return Results.NotFound();
        }

        var productOutput = new ProductOutput(product);

        return Results.Ok(productOutput);
    });
}

static void GetProductCategoryById(List<IProduct> products, WebApplication app)
{
    app.MapGet("/api/product-categories/{id}", (string id) =>
    {
        var internalId = id.Deobfuscate<ProductCategory>();
        var productCategory = products.FirstOrDefault(p => p.Category?.Id == internalId)?.Category;

        if (productCategory is null)
        {
            return Results.NotFound();
        }

        var productCategoryOutput = new ProductCategoryOutput(productCategory);

        return Results.Ok(productCategoryOutput);
    });
}

static void DeleteProductById(List<IProduct> products, WebApplication app)
{
    app.MapPost("/api/products/delete/{id}", (string id) =>
    {
        var internalId = id.Deobfuscate<IProduct>();
        var product = products.SingleOrDefault(p => p.Id == internalId);

        if (product is null)
        {
            return Results.NotFound();
        }

        products.Remove(product);

        return Results.NoContent();
    });
}

#endregion
