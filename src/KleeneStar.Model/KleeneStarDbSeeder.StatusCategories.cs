using KleeneStar.Model.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using WebExpress.WebUI.WebIcon;

namespace KleeneStar.Model
{
    /// <summary>
    /// Provides methods for seeding the database with initial data required for application operation.
    /// </summary>
    public static partial class KleeneStarDbSeeder
    {
        /// <summary>
        /// Adds the status category to the database if it does not already exist.
        /// </summary>
        /// <param name="db">The database context used to add the status category. Cannot be null.</param>
        private static void SeedStatusCategories(KleeneStarDbContext db)
        {
            db.StatusCategories.Add
            (
                new StatusCategory
                {
                    Id = Guid.Parse("74C668D5-BAED-4DDD-8176-B54C396EA250"),
                    Name = "ToDo",
                    Description = "Items that are planned but not yet started.",
                    Icon = ImageIcon.FromString("/kleenestar/assets/icons/status-category-todo.svg"),
                    Color = "#FF5733",
                    // a state created on the designer canvas has not been through any workflow
                    // yet, so it starts out as something planned
                    IsDefault = true,
                    Created = DateTime.UtcNow,
                    Updated = DateTime.UtcNow
                }
            );

            db.StatusCategories.Add
            (
                new StatusCategory
                {
                    Id = Guid.Parse("66948427-5501-4E76-92BC-300929DA4A8F"),
                    Name = "InProgress",
                    Description = "Items that are currently being worked on.",
                    Icon = ImageIcon.FromString("/kleenestar/assets/icons/status-category-inprogress.svg"),
                    Color = "#33C1FF",
                    Created = DateTime.UtcNow,
                    Updated = DateTime.UtcNow
                }
            );

            db.StatusCategories.Add
            (
                new StatusCategory
                {
                    Id = Guid.Parse("922133B8-A250-495E-87F0-3485290A77CC"),
                    Name = "Waiting",
                    Description = "Items that are blocked or waiting for external input.",
                    Icon = ImageIcon.FromString("/kleenestar/assets/icons/status-category-waiting.svg"),
                    Color = "#FFC300",
                    Created = DateTime.UtcNow,
                    Updated = DateTime.UtcNow
                }
            );

            db.StatusCategories.Add
            (
                new StatusCategory
                {
                    Id = Guid.Parse("7407A6D6-205F-4BDD-8BF4-B3EBE4CBD0B9"),
                    Name = "Done",
                    Description = "Items that have been completed successfully.",
                    Icon = ImageIcon.FromString("/kleenestar/assets/icons/status-category-done.svg"),
                    Color = "#28A745",
                    Created = DateTime.UtcNow,
                    Updated = DateTime.UtcNow
                }
            );
        }
    }
}
