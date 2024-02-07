using Microsoft.AspNetCore.Mvc;
using System.Xml.Linq;
using TechGearDatabase.Data;
using TechGearDatabase.Models;

namespace ProductAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddAuthorization();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            TechGearDB db = new TechGearDB("TechStoreComponentsDB");

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();
            //*******************************************************
            string productTable = DataCollection.Product.ToString();

            //Create
            app.MapPost("/product", async (Product data) =>
            {
                Product product = new Product (data.Name, data.Price);
                await db.AddData(productTable, product);

                return Results.Ok($"{product.Name} was added to database");
            });

            //Read
            app.MapGet("/searchProduct", async (string name) => //Search for data of matching string
            {
                var products = await db.GetAllData<Product>(productTable);
                var searchResults = products.Where(p=>p.Name.ToLower().Contains(name.ToLower())).ToList();

                if(searchResults.Count == 0) { 
                    return Results.NotFound($"No matching results on '{name}' ");
                }
                else
                {
                    return Results.Ok(searchResults);
                }
            });

            app.MapGet("/allProducts", async() => //Get all products
            {
                var products = await db.GetAllData<Product>(productTable);
                return Results.Ok(products);
            });

            //Update
            app.MapPut("/productName", async(string oldName, string newName) => //Hmmmmmm
            {
                var productToUpdate = await db.GetDataByName<Product>(productTable, oldName);
                
                if( productToUpdate == null)
                {
                    return Results.NotFound($"'{oldName}' doesn't exist");
                }
                else
                {
                    productToUpdate.Name = newName;
                    await db.UpdateData(productTable, productToUpdate);
                    return Results.Ok($"'{productToUpdate.Name}' has been updated");
                }
            });

            app.MapPut("/productPrice", async(string name, double newPrice) =>
            {
                var productToUpdate = await db.GetDataByName<Product>(productTable, name);

                if(productToUpdate == null)
                {
                    return Results.NotFound($"'{name}' doesn't exist");
                }
                else
                {
                    productToUpdate.Price = newPrice;
                    await db.UpdateData(productTable, productToUpdate);
                    return Results.Ok($"'{productToUpdate.Name}' has been updated");
                }
            });

            //Delete
            app.MapDelete("/product", async([FromBody]Product product) =>
            {
                //Might not need to be awaited
                await db.DeleteData(productTable, product);

                return Results.Ok($"{product.Name} was deleted");
            });
            

            //*******************************************************
            app.Run();
        }
    }
}
