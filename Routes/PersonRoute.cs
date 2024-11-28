using Microsoft.AspNetCore.Mvc.RazorPages;
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
                return Results.Ok(new { Message = "Person created successfully.", Person = person });
            });

            route.MapGet("", async (PersonContext context, string? searchTerm = null, int page = 1, int pageSize = 10) =>
            {
                var query = context.People.AsQueryable();

                // Se searchTerm for fornecido, aplicamos o filtro
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    query = query.Where(person => EF.Functions.Like(person.Name, $"%{searchTerm}%"));
                }

                var people = await query.Where(person => person.Name != "Desativado").Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
                return Results.Ok(people);
            });

            route.MapPut("{id:guid}", async (Guid id, PersonRequest req, PersonContext context) =>
            {
                var person = await context.People.FirstOrDefaultAsync(x => x.Id == id);
                if (person == null)
                {
                    return Results.NotFound(new { Message = "Person not found.", Id = id });
                }
                person.ChangeName(req.name);
                await context.SaveChangesAsync();

                return Results.Ok(new { Message = "Person updated successfully.", Person = person });
            });

            route.MapDelete("{id:guid}", async (Guid id, PersonContext context) =>
            {
                var person = await context.People.FirstOrDefaultAsync(x => x.Id == id);
                
                if (person == null)
                {
                    return Results.NotFound(new { Message = "Person not found.", Id = id });
                }

                person.SetInactive();
                await context.SaveChangesAsync();
                return Results.Ok(new { Message = "Person deleted successfully.", Person = person });
            });
        }
    }
}
