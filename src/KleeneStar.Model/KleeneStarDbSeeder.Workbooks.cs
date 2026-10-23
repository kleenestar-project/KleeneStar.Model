using System;
using System.Collections.Generic;
using System.Linq;
using WebExpress.WebUI.WebSpreadsheet;

namespace KleeneStar.Model
{
    /// <summary>
    /// Provides methods for seeding the database with initial data required for application operation.
    /// </summary>
    public static partial class KleeneStarDbSeeder
    {
        /// <summary>
        /// The colour of the header rows of the seeded workbooks.
        /// </summary>
        private const string WorkbookHeaderFill = "#DCE6F1";

        /// <summary>
        /// Builds the workbook a seeded document of a spreadsheet class carries as its content.
        /// </summary>
        /// <remarks>
        /// The workbooks are written through the framework's own model rather than as JSON
        /// text, so they are exactly what the workbook dialog would have saved. Four layouts
        /// take turns - a budget plan, a cost comparison, a revenue forecast and a capacity plan
        /// - and every figure is derived from the document's index, so twenty documents differ
        /// and a reseed produces the same ones. Totals and derived columns are formulas, so the
        /// reading view and the editor calculate them rather than showing stored numbers.
        /// </remarks>
        /// <param name="title">The document's summary, written as the sheet's heading.</param>
        /// <param name="index">The index (0-19) of the document within its class.</param>
        /// <returns>The workbook as JSON.</returns>
        private static string SeedWorkbook(string title, int index)
        {
            var workbook = new SpreadsheetWorkbook();
            var seed = 1 + index;

            switch (index % 4)
            {
                case 0:
                    BudgetSheet(workbook.AddSheet("Budget", 40, 8), title, seed);
                    break;
                case 1:
                    ComparisonSheet(workbook.AddSheet("Comparison", 40, 8), title, seed);
                    break;
                case 2:
                    ForecastSheet(workbook.AddSheet("Forecast", 40, 8), title, seed);
                    break;
                default:
                    CapacitySheet(workbook.AddSheet("Capacity", 40, 8), title, seed);
                    break;
            }

            NotesSheet(workbook.AddSheet("Notes", 20, 4), title);
            workbook.ActiveSheet = workbook.Sheets[0].Name;

            return workbook.ToJson();
        }

        /// <summary>
        /// A quarterly budget: cost items by quarter, with row and column totals.
        /// </summary>
        /// <param name="sheet">The sheet.</param>
        /// <param name="title">The heading.</param>
        /// <param name="seed">The figure seed.</param>
        private static void BudgetSheet(SpreadsheetSheet sheet, string title, int seed)
        {
            string[] items = ["Personnel", "Licenses", "Hardware", "Travel", "Training", "Consulting"];

            var rows = new List<IEnumerable<object>>
            {
                new object[] { title },
                Array.Empty<object>(),
                new object[] { "Item", "Q1", "Q2", "Q3", "Q4", "Total" }
            };

            for (var i = 0; i < items.Length; i++)
            {
                var row = 4 + i;
                var basis = 1500 + (seed * 370 + i * 911) % 9000;

                rows.Add([items[i], basis, basis + 250 * (i % 3), basis - 120 * (i % 2), basis + 400, $"=SUM(B{row}:E{row})"]);
            }

            var last = 3 + items.Length;

            rows.Add(["Total", $"=SUM(B4:B{last})", $"=SUM(C4:C{last})", $"=SUM(D4:D{last})", $"=SUM(E4:E{last})", $"=SUM(F4:F{last})"]);

            sheet.SetData(rows);
            Heading(sheet, "A1");
            HeaderRow(sheet, "A3:F3");
            sheet.Format($"B4:F{last + 1}", s => s.NumberFormat = "#,##0");
            sheet.Format($"A{last + 1}:F{last + 1}", s => { s.Bold = true; s.BorderTop = "thin #000000"; });
            Columns(sheet, 160, 100, 100, 100, 100, 110);
            sheet.Freeze(3, 1);
        }

        /// <summary>
        /// A comparison of offers by total cost of ownership, with the cheapest one named.
        /// </summary>
        /// <param name="sheet">The sheet.</param>
        /// <param name="title">The heading.</param>
        /// <param name="seed">The figure seed.</param>
        private static void ComparisonSheet(SpreadsheetSheet sheet, string title, int seed)
        {
            string[] offers = ["Vendor A", "Vendor B", "Vendor C", "In-house"];

            var rows = new List<IEnumerable<object>>
            {
                new object[] { title },
                Array.Empty<object>(),
                new object[] { "Option", "One-off", "Monthly", "Months", "Total cost" }
            };

            for (var i = 0; i < offers.Length; i++)
            {
                var row = 4 + i;

                rows.Add([offers[i], 2000 + (seed * 530 + i * 1270) % 12000, 150 + (seed * 37 + i * 83) % 900, 36, $"=B{row}+C{row}*D{row}"]);
            }

            var last = 3 + offers.Length;

            rows.Add(Array.Empty<object>());
            rows.Add(["Lowest total", null, null, null, $"=MIN(E4:E{last})"]);
            rows.Add(["Average total", null, null, null, $"=AVERAGE(E4:E{last})"]);

            sheet.SetData(rows);
            Heading(sheet, "A1");
            HeaderRow(sheet, "A3:E3");
            sheet.Format($"B4:C{last}", s => s.NumberFormat = "#,##0");
            sheet.Format($"E4:E{last + 3}", s => s.NumberFormat = "#,##0");
            sheet.Format($"A{last + 2}:E{last + 3}", s => s.Bold = true);
            Columns(sheet, 150, 100, 100, 80, 120);
            sheet.Freeze(3);
        }

        /// <summary>
        /// A revenue forecast: the first month is given, every further month grows by the rate
        /// in the assumptions, and the year is summed.
        /// </summary>
        /// <param name="sheet">The sheet.</param>
        /// <param name="title">The heading.</param>
        /// <param name="seed">The figure seed.</param>
        private static void ForecastSheet(SpreadsheetSheet sheet, string title, int seed)
        {
            string[] months = ["January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"];

            var rows = new List<IEnumerable<object>>
            {
                new object[] { title },
                new object[] { "Monthly growth", (seed % 5 + 1) / 100.0 },
                new object[] { "Month", "Revenue", "Cumulative" },
                new object[] { months[0], 20000 + seed * 1750, "=B4" }
            };

            for (var i = 1; i < months.Length; i++)
            {
                var row = 4 + i;

                rows.Add([months[i], $"=ROUND(B{row - 1}*(1+$B$2),0)", $"=C{row - 1}+B{row}"]);
            }

            rows.Add(["Year", "=SUM(B4:B15)"]);

            sheet.SetData(rows);
            Heading(sheet, "A1");
            sheet.Format("B2", s => s.NumberFormat = "0%");
            HeaderRow(sheet, "A3:C3");
            sheet.Format("B4:C16", s => s.NumberFormat = "#,##0");
            sheet.Format("A16:C16", s => { s.Bold = true; s.BorderTop = "thin #000000"; });
            Columns(sheet, 140, 110, 120);
            sheet.Freeze(3);
        }

        /// <summary>
        /// A capacity plan: teams with their people, hours and weeks, and the hours they can
        /// give in total.
        /// </summary>
        /// <param name="sheet">The sheet.</param>
        /// <param name="title">The heading.</param>
        /// <param name="seed">The figure seed.</param>
        private static void CapacitySheet(SpreadsheetSheet sheet, string title, int seed)
        {
            string[] teams = ["Service Desk", "Operations", "Development", "Finance", "Procurement"];

            var rows = new List<IEnumerable<object>>
            {
                new object[] { title },
                Array.Empty<object>(),
                new object[] { "Team", "People", "Hours per week", "Weeks", "Capacity (h)", "Share" }
            };

            for (var i = 0; i < teams.Length; i++)
            {
                var row = 4 + i;

                rows.Add([teams[i], 2 + (seed + i * 3) % 9, 40 - (i % 2) * 8, 12, $"=B{row}*C{row}*D{row}", $"=E{row}/$E${4 + teams.Length}"]);
            }

            var last = 3 + teams.Length;

            rows.Add(["Total", $"=SUM(B4:B{last})", null, null, $"=SUM(E4:E{last})", $"=SUM(F4:F{last})"]);

            sheet.SetData(rows);
            Heading(sheet, "A1");
            HeaderRow(sheet, "A3:F3");
            sheet.Format($"E4:E{last + 1}", s => s.NumberFormat = "#,##0");
            sheet.Format($"F4:F{last + 1}", s => s.NumberFormat = "0.0%");
            sheet.Format($"A{last + 1}:F{last + 1}", s => { s.Bold = true; s.BorderTop = "thin #000000"; });
            Columns(sheet, 150, 80, 120, 80, 120, 80);
            sheet.Freeze(3, 1);
        }

        /// <summary>
        /// A second sheet that says what the workbook is, so the seed shows the sheet tabs.
        /// </summary>
        /// <param name="sheet">The sheet.</param>
        /// <param name="title">The heading.</param>
        private static void NotesSheet(SpreadsheetSheet sheet, string title)
        {
            sheet.SetData(
            [
                [title],
                [],
                ["Prepared by", "Finance and Controlling"],
                ["Status", "Draft for review"],
                ["Note", "Figures are planning values; the totals are calculated."]
            ]);
            Heading(sheet, "A1");
            sheet.Format("A3:A5", s => s.Bold = true);
            Columns(sheet, 140, 360);
        }

        /// <summary>
        /// Formats the heading cell of a sheet.
        /// </summary>
        /// <param name="sheet">The sheet.</param>
        /// <param name="reference">The cell.</param>
        private static void Heading(SpreadsheetSheet sheet, string reference)
        {
            sheet.Format(reference, s => { s.Bold = true; s.Size = 14; });
        }

        /// <summary>
        /// Formats a header row.
        /// </summary>
        /// <param name="sheet">The sheet.</param>
        /// <param name="range">The row's range.</param>
        private static void HeaderRow(SpreadsheetSheet sheet, string range)
        {
            sheet.Format(range, s => { s.Bold = true; s.Fill = WorkbookHeaderFill; s.BorderBottom = "thin #000000"; });
        }

        /// <summary>
        /// Sets the widths of the first columns of a sheet.
        /// </summary>
        /// <param name="sheet">The sheet.</param>
        /// <param name="widths">The widths in pixels, from column A.</param>
        private static void Columns(SpreadsheetSheet sheet, params int[] widths)
        {
            foreach (var (width, column) in widths.Select((w, i) => (w, i)))
            {
                sheet.ColumnWidths[column] = width;
            }
        }
    }
}
