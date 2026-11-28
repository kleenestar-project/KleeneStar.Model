using KleeneStar.Model.Transfer;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using WebExpress.WebUI.WebIcon;

namespace KleeneStar.Model
{
    internal static partial class ModelHub
    {
        /// <summary>
        /// The transfer schema per model (one model per provider and process).
        /// </summary>
        private static readonly ConditionalWeakTable<IModel, WorkspaceTransferSchema> _transferSchemas = [];

        /// <summary>
        /// Writes workspaces with everything that belongs to them into a zip archive
        /// (<see cref="WorkspaceTransferFormat"/>). Accounts, groups, tenants and other data of the
        /// installation are not written; only the id and name of those the data refers to.
        /// </summary>
        /// <param name="workspaceIds">The workspaces.</param>
        /// <param name="output">The stream the archive is written to.</param>
        /// <param name="progress">What is told of the progress (share, phase, detail), or <see langword="null"/>.</param>
        /// <param name="structureOnly">Whether objects and their dependent data are omitted.</param>
        /// <param name="exportIcon">The optional resolver embedding local icon data.</param>
        /// <returns>What was written.</returns>
        public static WorkspaceTransferReport ExportWorkspaces(IReadOnlyCollection<Guid> workspaceIds, Stream output, Action<double, string, string> progress = null, bool structureOnly = false, Func<ImageIcon, ImageIcon> exportIcon = null)
        {
            ArgumentNullException.ThrowIfNull(workspaceIds);
            ArgumentNullException.ThrowIfNull(output);

            using var context = CreateDbContext();

            return new WorkspaceExporter(context, TransferSchema(context.Model)) { Progress = progress, StructureOnly = structureOnly, ExportIcon = exportIcon }.Export(workspaceIds, output);
        }

        /// <summary>
        /// Reads a zip archive written by <see cref="ExportWorkspaces"/> - by this or by another
        /// version - into the database.
        /// </summary>
        /// <param name="input">The archive.</param>
        /// <param name="options">The choices of the import.</param>
        /// <param name="progress">What is told of the progress (share, phase, detail), or <see langword="null"/>.</param>
        /// <param name="importIcon">The optional resolver storing imported icon data.</param>
        /// <returns>What was imported, left out and left empty.</returns>
        public static WorkspaceTransferReport ImportWorkspaces(Stream input, WorkspaceImportOptions options = null, Action<double, string, string> progress = null, Func<ImageIcon, ImageIcon> importIcon = null)
        {
            ArgumentNullException.ThrowIfNull(input);

            using var context = CreateDbContext();

            return new WorkspaceImporter(context, TransferSchema(context.Model)) { Progress = progress, ImportIcon = importIcon }.Import(input, options);
        }

        /// <summary>
        /// Returns the entity types of the model that the transfer catalog does not classify.
        /// </summary>
        /// <returns>The entity names.</returns>
        public static IReadOnlyList<string> GetUnclassifiedTransferTables()
        {
            using var context = CreateDbContext();

            return TransferSchema(context.Model).Unclassified;
        }

        /// <summary>
        /// Returns the transfer schema of a model.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>The schema.</returns>
        internal static WorkspaceTransferSchema TransferSchema(IModel model)
        {
            return _transferSchemas.GetValue(model, x => new WorkspaceTransferSchema(x));
        }
    }
}
