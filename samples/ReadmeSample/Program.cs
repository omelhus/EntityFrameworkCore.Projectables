﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ReadmeSample.Entities;
using ReadmeSample.Extensions;

namespace ReadmeSample
{
    class Program
    {
        static void Main(string[] args)
        {
            using var dbContext = new ApplicationDbContext();

            // recreate database
            dbContext.Database.EnsureDeleted();
            dbContext.Database.EnsureCreated();

            // Populate with seed data
            var sampleUser = new User { UserName = "Jon", EmailAddress = "jon@doe.com" };
            var sampleProduct = new Product { Name = "Blue Pen", ListPrice = 1.5m };
            var sampleOrder = new Order {
                User = sampleUser,
                TaxRate = .19m,
                CreatedDate = DateTime.UtcNow,
                Items = new List<OrderItem> {
                    new OrderItem { Product = sampleProduct, Quantity = 5  }
                }
            };

            dbContext.AddRange(sampleUser, sampleProduct, sampleOrder);
            dbContext.SaveChanges();

            // Lets try a query!
            var query = dbContext.Users
                .Where(x => x.UserName == "Jon")
                .Select(x => new {
                    x.GetMostRecentOrderForUser(DateTime.UtcNow.AddDays(-30)).GrandTotal
                });

            var result = query.First();

            Console.WriteLine($"Jons latest order had a grant total of {result.GrandTotal}");
            Console.WriteLine($"The following query was used to fetch the results:");
            Console.WriteLine(query.ToQueryString());
        }
    }
}