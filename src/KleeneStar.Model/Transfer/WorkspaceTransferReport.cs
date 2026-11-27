using System;
using System.Collections.Generic;

namespace KleeneStar.Model.Transfer
{
    /// <summary>
    /// The format of the export file.
    /// </summary>
    /// <remarks>
    /// A zip archive with <c>manifest.json</c> (format, version, schema of the writing
    /// installation, the exported workspaces and the columns of every table),
    /// <c>tables/&lt;entity&gt;.json</c> (an array of rows, each an object keyed by property name)
    /// and <c>references/&lt;entity&gt;.json</c> (id and name of the accounts, groups and tenants
    /// the data refers to). Every reference names the id of the row it points to, never a
    /// database id, so the file means the same in every installation.
    /// </remarks>
    public static class WorkspaceTransferFormat
    {
        /// <summary>
        /// The name of the format written into the manifest.
        /// </summary>
        public const string Name = "kleenestar.workspaces";

        /// <summary>
        /// The version of the format this installation writes. A reader accepts every version:
        /// it takes what it knows and leaves the rest.
        /// </summary>
        public const int Version = 1;

        /// <summary>
        /// The name of the manifest entry.
        /// </summary>
        public const string Manifest = "manifest.json";

        /// <summary>
        /// The folder of the tables.
        /// </summary>
        public const string TablesFolder = "tables/";

        /// <summary>
        /// The folder of the references to data of the installation.
        /// </summary>
        public const string ReferencesFolder = "references/";
    }

    /// <summary>
    /// A workspace an export holds or an import found.
    /// </summary>
    public sealed class WorkspaceTransferWorkspace
    {
        /// <summary>
        /// Returns the id.
        /// </summary>
        public Guid Id { get; init; }

        /// <summary>
        /// Returns the key.
        /// </summary>
        public string Key { get; init; }

        /// <summary>
        /// Returns the name.
        /// </summary>
        public string Name { get; init; }

        /// <summary>
        /// Returns why the workspace was not imported, or <see langword="null"/> when it was.
        /// </summary>
        public string Skipped { get; init; }

        /// <summary>
        /// Returns whether the import overwrote a workspace that was present.
        /// </summary>
        public bool Replaced { get; init; }
    }

    /// <summary>
    /// What happened to one table.
    /// </summary>
    public sealed class WorkspaceTransferTableReport
    {
        /// <summary>
        /// Returns the entity name.
        /// </summary>
        public string Name { get; init; }

        /// <summary>
        /// Returns the number of rows in the file.
        /// </summary>
        public int Rows { get; set; }

        /// <summary>
        /// Returns the number of rows written.
        /// </summary>
        public int Written { get; set; }

        /// <summary>
        /// Returns the number of rows that overwrote a present row of the same id.
        /// </summary>
        public int Updated { get; set; }

        /// <summary>
        /// Returns the number of rows of an overwritten workspace the file no longer holds,
        /// which were deleted.
        /// </summary>
        public int Deleted { get; set; }

        /// <summary>
        /// Returns the number of rows that were already present and reused.
        /// </summary>
        public int Reused { get; set; }

        /// <summary>
        /// Returns the number of rows left out.
        /// </summary>
        public int Skipped { get; set; }
    }

    /// <summary>
    /// The result of an export or an import.
    /// </summary>
    public sealed class WorkspaceTransferReport
    {
        /// <summary>
        /// Returns whether the transfer took effect.
        /// </summary>
        public bool Succeeded { get; set; }

        /// <summary>
        /// Returns why it did not, or <see langword="null"/>.
        /// </summary>
        public string Error { get; set; }

        /// <summary>
        /// Returns the format version of the file.
        /// </summary>
        public int FormatVersion { get; set; }

        /// <summary>
        /// Returns the schema (last migration) of the installation that wrote the file.
        /// </summary>
        public string Schema { get; set; }

        /// <summary>
        /// Returns the workspaces of the file, imported or skipped.
        /// </summary>
        public List<WorkspaceTransferWorkspace> Workspaces { get; } = [];

        /// <summary>
        /// Returns what happened per table.
        /// </summary>
        public List<WorkspaceTransferTableReport> Tables { get; } = [];

        /// <summary>
        /// Returns the notes: what was left out or left empty, and why.
        /// </summary>
        public List<WorkspaceTransferNote> Notes { get; } = [];

        /// <summary>
        /// Adds a note, or counts it once more when the same note was made before.
        /// </summary>
        /// <param name="code">What happened (see <see cref="WorkspaceTransferNote.Code"/>).</param>
        /// <param name="table">The table, or <see langword="null"/>.</param>
        /// <param name="detail">The detail (a column, a referenced table), or <see langword="null"/>.</param>
        public void Note(string code, string table = null, string detail = null)
        {
            var note = Notes.Find(x => x.Code == code && x.Table == table && x.Detail == detail);

            if (note is null)
            {
                Notes.Add(new WorkspaceTransferNote { Code = code, Table = table, Detail = detail, Count = 1 });
            }
            else
            {
                note.Count++;
            }
        }
    }

    /// <summary>
    /// A note of a transfer: something the file held that was left out or left empty.
    /// </summary>
    public sealed class WorkspaceTransferNote
    {
        /// <summary>
        /// What happened: <c>format-newer</c>, <c>table-unknown</c>, <c>table-excluded</c>,
        /// <c>column-unknown</c>, <c>type-unknown</c>, <c>value-unreadable</c>,
        /// <c>reference-cleared</c>, <c>row-present</c>, <c>row-duplicate</c>,
        /// <c>row-conflict</c>, <c>row-reference</c>, <c>row-owner</c>,
        /// <c>reference-unmatched</c>, <c>file-unreadable</c>.
        /// </summary>
        public string Code { get; init; }

        /// <summary>
        /// Returns the table, or <see langword="null"/>.
        /// </summary>
        public string Table { get; init; }

        /// <summary>
        /// Returns the detail (a column, a referenced table), or <see langword="null"/>.
        /// </summary>
        public string Detail { get; init; }

        /// <summary>
        /// Returns how often it happened.
        /// </summary>
        public int Count { get; set; }
    }

    /// <summary>
    /// The choices of an import.
    /// </summary>
    public sealed class WorkspaceImportOptions
    {
        /// <summary>
        /// Returns the ids of the workspaces of the file to import, or <see langword="null"/>
        /// for all of them.
        /// </summary>
        public IReadOnlyCollection<Guid> WorkspaceIds { get; init; }

        /// <summary>
        /// Returns whether the file is only read and checked, without writing anything.
        /// </summary>
        public bool DryRun { get; init; }

        /// <summary>
        /// Returns whether a workspace of the file that is present (the same id) is overwritten:
        /// its rows take the values of the file, rows the file adds are created and rows the file
        /// no longer holds are deleted. Without it a present workspace is left as it is.
        /// </summary>
        public bool Overwrite { get; init; }
    }
}
