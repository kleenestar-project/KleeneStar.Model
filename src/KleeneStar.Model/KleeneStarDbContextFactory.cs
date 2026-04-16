using KleeneStar.Model.Config;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Reflection;

namespace KleeneStar.Model
{
    /// <summary>
    /// Provides factory methods for creating instances of KleeneStarDbContext using 
    /// specified database configuration or provider information.
    /// </summary>
    public static class KleeneStarDbContextFactory
    {
        /// <summary>
        /// Creates a new instance of the KleeneStarDbContext using the specified 
        /// database configuration.
        /// </summary>
        /// <param name="config">
        /// The database configuration containing the provider and connection 
        /// string to use for the context. Cannot be null.
        /// </param>
        /// <returns>
        /// A new KleeneStarDbContext instance configured with the specified database 
        /// provider and connection string.
        /// </returns>
        public static KleeneStarDbContext Create(DbConfig config)
        {
            return Create(config.Provider, config.ConnectionString, config.Assembly);
        }

        /// <summary>
        /// Creates a new instance of the KleeneStarDbContext configured to use the specified 
        /// database provider and connection string.
        /// </summary>
        /// <param name="provider">
        /// The name of the database provider to use. Supported values are "SqlServer", 
        /// "PostgreSQL", and "SQLite". The comparison is case-sensitive.
        /// </param>
        /// <param name="connectionString">
        /// The connection string to use for connecting to the database. Must be valid for the 
        /// specified provider.
        /// </param>
        /// <param name="assemblyName">The name of the assembly associated with the provider.</param>
        /// <returns>
        /// A new KleeneStarDbContext instance configured with the specified provider and 
        /// connection string.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// Thrown if the specified provider is not supported.
        /// </exception>
        public static KleeneStarDbContext Create(string provider, string connectionString, string assemblyName = null)
        {
            // load the Create method from the provider-specific assembly
            var createMethod = LoadFactoryMethod(assemblyName ?? $"KleeneStar.Model.{provider}")
                ?? throw new ArgumentException($"Unknown provider: {provider}");

            // invoke the factory to create the DbContext
            return (KleeneStarDbContext)createMethod.Invoke
            (
                null,
                [connectionString]
            );
        }

        /// <summary>
        /// Locates and returns the public static 'Create' method from a specified type within a loaded assembly.
        /// </summary>
        /// <remarks>
        /// If typeName is not provided, the method searches for the first type in the assembly
        /// that defines a public static method named 'Create'. This method is intended for scenarios where factory
        /// methods are conventionally named 'Create'.
        /// </remarks>
        /// <param name="assemblyName">
        /// The name of the assembly to load. Must not be null or empty.
        /// </param>
        /// <param name="typeName">
        /// The name of the type to search for within the assembly. If null, the method automatically 
        /// discovers the first type containing a public static 'Create' method.
        /// </param>
        /// <returns>
        /// A MethodInfo object representing the public static 'Create' method of the specified or 
        /// discovered type.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown if the assembly cannot be loaded, the specified type is not found, no suitable 
        /// factory type with a static 'Create' method exists, or the 'Create' method is not found 
        /// on the type.
        /// </exception>
        private static MethodInfo LoadFactoryMethod(string assemblyName, string typeName = null)
        {
            // load the specified assembly
            var assembly = Assembly.Load(assemblyName)
                ?? throw new InvalidOperationException($"Assembly '{assemblyName}' could not be loaded.");

            // retrieve all types from the assembly
            var types = assembly.GetTypes();

            Type factoryType;

            if (typeName is not null)
            {
                // look for the explicitly specified type
                factoryType = types.FirstOrDefault(t => t.Name == typeName);
                if (factoryType is null)
                {
                    throw new InvalidOperationException
                    (
                        $"Type '{typeName}' was not found in assembly '{assemblyName}'."
                    );
                }
            }
            else
            {
                // automatic discovery: find a type that contains a static Create method
                factoryType = types.FirstOrDefault(t => t.GetMethods(BindingFlags.Public | BindingFlags.Static)
                     .Any(m => m.Name == "Create"));

                if (factoryType is null)
                {
                    throw new InvalidOperationException
                    (
                        $"No factory type with a static Create method was found in assembly '{assemblyName}'."
                    );
                }
            }

            // retrieve the Create method
            var method = factoryType.GetMethod("Create", BindingFlags.Public | BindingFlags.Static);

            return method is null
                ? throw new InvalidOperationException(
                    $"The method 'Create' was not found in type '{factoryType.FullName}'.")
                : method;
        }
    }
}
