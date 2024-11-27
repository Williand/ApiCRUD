﻿using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Person.Data;
using Person.Models;

namespace Person.Routes
{
    public static class PersonRoute
    {
        public static void PersonRoutes(this WebApplication app)
        {
            var route = app.MapGroup("person");

            route.MapPost("", async (PersonRequest req, PersonContext context) =>
            {
                var person = new PersonModel(req.name);
                await context.AddAsync(person);
                await context.SaveChangesAsync();
            });

            route.MapGet("", async (PersonContext context, int page = 1, int pageSize = 10) =>
            {
                var people = await context.People.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
                return Results.Ok(people);
            });

            route.MapPut("{id:guid}", async (Guid id, PersonRequest req, PersonContext context) =>
            {
                var person = await context.People.FirstOrDefaultAsync(x => x.Id == id);
                if (person == null)
                {
                    return Results.NotFound();
                }
                person.ChangeName(req.name);
                await context.SaveChangesAsync();

                return Results.Ok(person);
            });

            route.MapDelete("{id:guid}", async (Guid id, PersonContext context) =>
            {
                var person = await context.People.FirstOrDefaultAsync(x => x.Id == id);
                
                if (person == null)
                {
                    return Results.NotFound();
                }

                person.SetInactive();
                await context.SaveChangesAsync();
                return Results.Ok();
            });
        }
    }
}
