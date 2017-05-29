using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.Linq;
using NUREMarks.Models;
using PdfRpt.Core.Contracts;
using PdfRpt.FluentInterface;
using DocumentFormat.OpenXml.Spreadsheet;


namespace NUREMarks.Services
{
    public class DocSaver
    {
        public static void CreateWordDoc(string filepath, List<StudentData> data, string header, string info = "")
        {
            using (WordprocessingDocument document = WordprocessingDocument.Create(filepath, WordprocessingDocumentType.Document))
            {
                MainDocumentPart mainPart = document.AddMainDocumentPart();
                mainPart.Document = new Document();
                Body body = mainPart.Document.AppendChild(new Body());

                var doc = document.MainDocumentPart.Document;
                doc.Body.Append(GenerateParagraph(header, true, "000000"));
                doc.Body.Append(GenerateParagraph(info, false, "000000"));
                doc.Body.Append(GenerateParagraph("", false, "000000"));

                DocumentFormat.OpenXml.Wordprocessing.Table table = new DocumentFormat.OpenXml.Wordprocessing.Table();

                TableProperties props = new TableProperties(
                    new TableBorders(
                    new DocumentFormat.OpenXml.Wordprocessing.TopBorder
                    {
                        Val = new EnumValue<BorderValues>(BorderValues.Single),
                        Size = 8
                    },
                    new DocumentFormat.OpenXml.Wordprocessing.BottomBorder
                    {
                        Val = new EnumValue<BorderValues>(BorderValues.Single),
                        Size = 8
                    },
                    new DocumentFormat.OpenXml.Wordprocessing.LeftBorder
                    {
                        Val = new EnumValue<BorderValues>(BorderValues.Single),
                        Size = 8
                    },
                    new DocumentFormat.OpenXml.Wordprocessing.RightBorder
                    {
                        Val = new EnumValue<BorderValues>(BorderValues.Single),
                        Size = 8
                    },
                    new DocumentFormat.OpenXml.Wordprocessing.InsideHorizontalBorder
                    {
                        Val = new EnumValue<BorderValues>(BorderValues.Single),
                        Size = 8
                    },
                    new DocumentFormat.OpenXml.Wordprocessing.InsideVerticalBorder
                    {
                        Val = new EnumValue<BorderValues>(BorderValues.Single),
                        Size = 8
                    }));

                table.AppendChild<TableProperties>(props);
                Justification justification1 = new Justification() { Val = JustificationValues.Center };

                var th = new TableRow();

                var thc1 = new TableCell();

                thc1.Append(GenerateParagraph("№", true, "000000"));
                thc1.Append(new TableCellProperties(
                    new TableCellWidth { Type = TableWidthUnitValues.Pct, Width = "5" }));
                th.Append(thc1);

                var thc2 = new TableCell();
                thc2.Append(GenerateParagraph("П.I.Б. студента", true, "000000"));
                thc2.Append(new TableCellProperties(
                    new TableCellWidth { Type = TableWidthUnitValues.Pct, Width = "50" }));
                th.Append(thc2);

                var thc3 = new TableCell();
                thc3.Append(GenerateParagraph("Група", true, "000000"));
                thc3.Append(new TableCellProperties(
                    new TableCellWidth { Type = TableWidthUnitValues.Pct, Width = "15" }));
                th.Append(thc3);

                var thc4 = new TableCell();
                thc4.Append(GenerateParagraph("Рейтинг", true, "000000"));
                thc4.Append(new TableCellProperties(
                    new TableCellWidth { Type = TableWidthUnitValues.Pct, Width = "10" }));
                th.Append(thc4);

                var thc5 = new TableCell();
                thc5.Append(GenerateParagraph("Дод. iнформацiя", true, "000000"));
                thc5.Append(new TableCellProperties(
                    new TableCellWidth { Type = TableWidthUnitValues.Pct, Width = "20" }));
                th.Append(thc5);

                table.Append(th);

                for (var i = 0; i < data.Count; i++)
                {
                    var tr = new TableRow();
                    var tc1 = new TableCell();

                    tc1.Append(GenerateParagraph((i + 1).ToString(), false, "000000"));
                    tc1.Append(new TableCellProperties(
                        new TableCellWidth { Type = TableWidthUnitValues.Pct, Width = "5" },
                        new TableCellVerticalAlignment { Val = TableVerticalAlignmentValues.Center }));
                    tr.Append(tc1);

                    var tc2 = new TableCell();
                    tc2.Append(new Paragraph(new DocumentFormat.OpenXml.Wordprocessing.Run(new DocumentFormat.OpenXml.Wordprocessing.Text(data[i].Name))));
                    tc2.Append(new TableCellProperties(
                        new TableCellWidth { Type = TableWidthUnitValues.Pct, Width = "50" },
                        new TableCellVerticalAlignment { Val = TableVerticalAlignmentValues.Center }));
                    tr.Append(tc2);

                    var tc3 = new TableCell();
                    tc3.Append(GenerateParagraph(data[i].Group, false, "000000"));
                    tc3.Append(new TableCellProperties(
                        new TableCellWidth { Type = TableWidthUnitValues.Pct, Width = "15" },
                        new TableCellVerticalAlignment { Val = TableVerticalAlignmentValues.Center }));
                    tr.Append(tc3);

                    var tc4 = new TableCell();
                    tc4.Append(GenerateParagraph(data[i].Rating.ToString(), true, "FFFFFF"));
                    tc4.Append(new TableCellProperties(
                        new TableCellWidth { Type = TableWidthUnitValues.Pct, Width = "10" },
                        new Shading()
                        {
                            Fill = GetRatingColor(data[i].Rating)
                        },
                        new TableCellVerticalAlignment { Val = TableVerticalAlignmentValues.Center }));
                    tr.Append(tc4);

                    var tc5 = new TableCell();
                    tc5.Append(GenerateParagraph(data[i].Info, false, "000000"));
                    tc5.Append(new TableCellProperties(
                        new TableCellWidth { Type = TableWidthUnitValues.Pct, Width = "20" },
                        new TableCellVerticalAlignment { Val = TableVerticalAlignmentValues.Center }));
                    tr.Append(tc5);

                    table.Append(tr);
                }
                doc.Body.Append(table);
                doc.Save();
            }
        }

        public static void CreateExcelDoc(string filepath, List<StudentData> data, string header)
        {
            using (SpreadsheetDocument document = SpreadsheetDocument.Create(filepath, SpreadsheetDocumentType.Workbook))
            {
                WorkbookPart workbookPart = document.AddWorkbookPart();
                workbookPart.Workbook = new Workbook();

                WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                worksheetPart.Worksheet = new Worksheet();

                // Adding style
                WorkbookStylesPart stylePart = workbookPart.AddNewPart<WorkbookStylesPart>();
                stylePart.Stylesheet = GenerateStylesheet();
                stylePart.Stylesheet.Save();

                // Setting up columns
                DocumentFormat.OpenXml.Spreadsheet.Columns columns = new DocumentFormat.OpenXml.Spreadsheet.Columns(
                        new DocumentFormat.OpenXml.Spreadsheet.Column
                        {
                            Min = 1,
                            Max = 1,
                            Width = 4,
                            CustomWidth = true
                        },
                        new DocumentFormat.OpenXml.Spreadsheet.Column
                        {
                            Min = 2,
                            Max = 2,
                            Width = 45,
                            CustomWidth = true
                        },
                        new DocumentFormat.OpenXml.Spreadsheet.Column
                        {
                            Min = 3,
                            Max = 3,
                            Width = 12,
                            CustomWidth = true
                        },
                        new DocumentFormat.OpenXml.Spreadsheet.Column
                        {
                            Min = 4,
                            Max = 4,
                            Width = 9,
                            CustomWidth = true
                        },
                        new DocumentFormat.OpenXml.Spreadsheet.Column
                        {
                            Min = 5,
                            Max = 5,
                            Width = 18,
                            CustomWidth = true
                        });

                worksheetPart.Worksheet.AppendChild(columns);

                Sheets sheets = workbookPart.Workbook.AppendChild(new Sheets());
                Sheet sheet = new Sheet() { Id = workbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = header };
                sheets.Append(sheet);

                workbookPart.Workbook.Save();

                SheetData sheetData = worksheetPart.Worksheet.AppendChild(new SheetData());

                Row row = new Row();
                row.Append(
                    ConstructCell("№", CellValues.String, 2),
                    ConstructCell("П.I.Б. студента", CellValues.String, 2),
                    ConstructCell("Група", CellValues.String, 2),
                    ConstructCell("Рейтинг", CellValues.String, 2),
                    ConstructCell("Дод. iнформацiя", CellValues.String, 2));

                sheetData.AppendChild(row);

                for (int i = 0; i < data.Count; i++)
                {
                    row = new Row();

                    row.Append(
                        ConstructCell((i + 1).ToString(), CellValues.Number, 1),
                        ConstructCell(data[i].Name, CellValues.String, 1),
                        ConstructCell(data[i].Group, CellValues.String, 1),
                        ConstructRatingCell(data[i].Rating),
                        ConstructCell(data[i].Info, CellValues.String, 1));

                    sheetData.AppendChild(row);
                }

                worksheetPart.Worksheet.Save();
            }
        }

        private static Cell ConstructCell(string value, CellValues dataType, uint styleIndex = 0)
        {
            return new Cell()
            {
                CellValue = new CellValue(value),
                DataType = new EnumValue<CellValues>(dataType),
                StyleIndex = styleIndex
            };
        }

        private static Cell ConstructRatingCell(double rating)
        {
            uint idx = 9;

            if (rating >= 92)
                idx = 3;
            else if (rating >= 85)
                idx = 4;
            else if (rating >= 80)
                idx = 5;
            else if (rating >= 75)
                idx = 6;
            else if (rating >= 70)
                idx = 7;
            else if (rating >= 60)
                idx = 8;

            return new Cell()
            {
                CellValue = new CellValue(GetRatingValue(rating)),
                DataType = new EnumValue<CellValues>(CellValues.Number),
                StyleIndex = idx
            };
        }

        private static Stylesheet GenerateStylesheet()
        {
            Stylesheet styleSheet = null;

            DocumentFormat.OpenXml.Spreadsheet.Fonts fonts = new DocumentFormat.OpenXml.Spreadsheet.Fonts(
                new DocumentFormat.OpenXml.Spreadsheet.Font( // Index 0 - default
                    new DocumentFormat.OpenXml.Spreadsheet.FontSize() { Val = 14 }

                ),
                new DocumentFormat.OpenXml.Spreadsheet.Font( // Index 1 - header
                    new DocumentFormat.OpenXml.Spreadsheet.FontSize() { Val = 14 },
                    new DocumentFormat.OpenXml.Spreadsheet.Bold(),
                    new DocumentFormat.OpenXml.Spreadsheet.Color() { Rgb = "FFFFFF" }

                ));

            Fills fills = new Fills(
                    new Fill(new PatternFill() { PatternType = PatternValues.None }),
                    new Fill(new PatternFill() { PatternType = PatternValues.Gray125 }),
                    new Fill(new PatternFill(new ForegroundColor { Rgb = new HexBinaryValue() { Value = "66666666" } })
                    { PatternType = PatternValues.Solid }),
                    new Fill(new PatternFill(new ForegroundColor { Rgb = new HexBinaryValue() { Value = "00056F33" } })
                    { PatternType = PatternValues.Solid }),
                    new Fill(new PatternFill(new ForegroundColor { Rgb = new HexBinaryValue() { Value = "0017982E" } })
                    { PatternType = PatternValues.Solid }),
                    new Fill(new PatternFill(new ForegroundColor { Rgb = new HexBinaryValue() { Value = "008DC153" } })
                    { PatternType = PatternValues.Solid }),
                    new Fill(new PatternFill(new ForegroundColor { Rgb = new HexBinaryValue() { Value = "00F6BB43" } })
                    { PatternType = PatternValues.Solid }),
                    new Fill(new PatternFill(new ForegroundColor { Rgb = new HexBinaryValue() { Value = "00E77E23" } })
                    { PatternType = PatternValues.Solid }),
                    new Fill(new PatternFill(new ForegroundColor { Rgb = new HexBinaryValue() { Value = "00E9573E" } })
                    { PatternType = PatternValues.Solid }),
                    new Fill(new PatternFill(new ForegroundColor { Rgb = new HexBinaryValue() { Value = "00838383" } })
                    { PatternType = PatternValues.Solid })
                );

            Borders borders = new Borders(
                    new DocumentFormat.OpenXml.Spreadsheet.Border(), // index 0 default
                    new DocumentFormat.OpenXml.Spreadsheet.Border( // index 1 black border
                        new DocumentFormat.OpenXml.Spreadsheet.LeftBorder(new DocumentFormat.OpenXml.Spreadsheet.Color() { Auto = true }) { Style = BorderStyleValues.Thin },
                        new DocumentFormat.OpenXml.Spreadsheet.RightBorder(new DocumentFormat.OpenXml.Spreadsheet.Color() { Auto = true }) { Style = BorderStyleValues.Thin },
                        new DocumentFormat.OpenXml.Spreadsheet.TopBorder(new DocumentFormat.OpenXml.Spreadsheet.Color() { Auto = true }) { Style = BorderStyleValues.Thin },
                        new DocumentFormat.OpenXml.Spreadsheet.BottomBorder(new DocumentFormat.OpenXml.Spreadsheet.Color() { Auto = true }) { Style = BorderStyleValues.Thin },
                        new DiagonalBorder())
                );

            CellFormats cellFormats = new CellFormats(
                    new CellFormat(), // default
                    new CellFormat { FontId = 0, FillId = 0, BorderId = 1, ApplyBorder = true }, // body
                    new CellFormat { FontId = 1, FillId = 2, BorderId = 1, ApplyFill = true },
                    new CellFormat { FontId = 1, FillId = 3, BorderId = 1, ApplyFill = true },
                    new CellFormat { FontId = 1, FillId = 4, BorderId = 1, ApplyFill = true },
                    new CellFormat { FontId = 1, FillId = 5, BorderId = 1, ApplyFill = true },
                    new CellFormat { FontId = 1, FillId = 6, BorderId = 1, ApplyFill = true },
                    new CellFormat { FontId = 1, FillId = 7, BorderId = 1, ApplyFill = true },
                    new CellFormat { FontId = 1, FillId = 8, BorderId = 1, ApplyFill = true },
                    new CellFormat { FontId = 1, FillId = 9, BorderId = 1, ApplyFill = true }
                );

            styleSheet = new Stylesheet(fonts, fills, borders, cellFormats);

            return styleSheet;
        }

        private static Paragraph GenerateParagraph(string text, bool bold, string textColor)
        {
            Paragraph paragraph1 = new Paragraph() { RsidParagraphAddition = "004F7104", RsidParagraphProperties = "008F2986", RsidRunAdditionDefault = "004F7104" };
            ParagraphProperties paragraphProperties1 = new ParagraphProperties();
            Justification justification1 = new Justification() { Val = JustificationValues.Center };
            Shading shading = new Shading()
            {
                Color = textColor,
                Fill = textColor == "000000" ? "FFFFFF" : GetRatingColor(Double.Parse(text))
            };

            paragraphProperties1.Append(justification1);
            paragraphProperties1.Append(shading);

            DocumentFormat.OpenXml.Wordprocessing.Run run1 = new DocumentFormat.OpenXml.Wordprocessing.Run();
            DocumentFormat.OpenXml.Wordprocessing.RunProperties runProperties1 = 
                new DocumentFormat.OpenXml.Wordprocessing.RunProperties(new DocumentFormat.OpenXml.Wordprocessing.Color()
                {
                    Val = textColor
                });

            if (bold)
                runProperties1.Append(new DocumentFormat.OpenXml.Wordprocessing.Bold());

            DocumentFormat.OpenXml.Wordprocessing.Text text1 = new DocumentFormat.OpenXml.Wordprocessing.Text();
            text1.Text = text;

            run1.Append(runProperties1);
            run1.Append(text1);

            paragraph1.Append(paragraphProperties1);
            paragraph1.Append(run1);
            return paragraph1;
        }

        private static string GetRatingColor(double rating)
        {
            if (rating >= 92)
                return "056F33";
            if (rating >= 85)
                return "17982E";
            if (rating >= 80)
                return "8DC153";
            if (rating >= 75)
                return "F6BB43";
            if (rating >= 70)
                return "E77E23";
            if (rating >= 60)
                return "E9573E";

            return "838383";
        }

        private static string GetRatingValue(double rating)
        {
            string rate = rating.ToString().Replace(",", ".");

            if (!rate.Contains('.'))
                rate += ".0";

            return rate;
        }

        public static IPdfReportData CreatePdfReport(string filepath, List<StudentData> data, string head, string info = "")
        {
            return new PdfReport().DocumentPreferences(doc =>
            {
                doc.RunDirection(PdfRunDirection.LeftToRight);
                doc.Orientation(PageOrientation.Portrait);
                doc.PageSize(PdfPageSize.A4);
                doc.DocumentMetadata(new DocumentMetadata
                {
                    Author = "NURE Marks",
                    Application = "NURE Marks",
                    Keywords = "NURE Marks, Ratings",
                    Subject = "Ratings",
                    Title = head
                });
            })
            .DefaultFonts(fonts =>
            {
                fonts.Path(Environment.GetEnvironmentVariable("SystemRoot") + "\\fonts\\arial.ttf",
                                  Environment.GetEnvironmentVariable("SystemRoot") + "\\fonts\\verdana.ttf");
            })
            .PagesFooter(footer =>
            {
                footer.DefaultFooter("NURE Marks " + DateTime.Now.ToString("MM/dd/yyyy"));
            })
            .PagesHeader(header =>
            {
                header.DefaultHeader(defaultHeader =>
                {
                    defaultHeader.RunDirection(PdfRunDirection.LeftToRight);
                    defaultHeader.Message(head + "\n\n" + info + "\n\n");
                });
                header.PdfFont.Size = 12;
            })
            .MainTableTemplate(template =>
            {
                template.BasicTemplate(BasicTemplate.ClassicTemplate);
            })
            .MainTablePreferences(table =>
            {
                table.ColumnsWidthsType(TableColumnWidthType.Relative);
                table.NumberOfDataRowsPerPage(0);
            })
            .MainTableDataSource(dataSource =>
            {
                dataSource.StronglyTypedList(data);
            })
            .MainTableColumns(columns =>
            {
                columns.AddColumn(column =>
                {
                    column.PropertyName("rowNo");
                    column.IsRowNumber(true);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(0);
                    column.Width(0.5f);
                    column.HeaderCell("№");
                    column.Font(font =>
                    {
                        font.Size(12);
                    });
                    column.PaddingTop(10);
                    column.PaddingBottom(10);
                });

                columns.AddColumn(column =>
                {
                    column.PropertyName<StudentData>(x => x.Name);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(1);
                    column.Width(4);
                    column.HeaderCell("П.I.Б. студента");
                    column.Font(font =>
                    {
                        font.Size(12);
                    });
                    column.PaddingTop(10);
                    column.PaddingBottom(10);
                });

                columns.AddColumn(column =>
                {
                    column.PropertyName<StudentData>(x => x.Group);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(2);
                    column.Width(1.5f);
                    column.HeaderCell("Група");
                    column.Font(font =>
                    {
                        font.Size(12);
                    });
                    column.PaddingTop(10);
                    column.PaddingBottom(10);
                });

                columns.AddColumn(column =>
                {
                    column.PropertyName<StudentData>(x => x.Rating);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(3);
                    column.Width(1);
                    column.HeaderCell("Рейтинг");
                    column.Font(font =>
                    {
                        font.Style(DocumentFontStyle.Bold);
                        font.Size(12);
                    });
                    column.PaddingTop(10);
                    column.PaddingBottom(10);

                });

                columns.AddColumn(column =>
                {
                    column.PropertyName<StudentData>(x => x.Info);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(4);
                    column.Width(2);
                    column.HeaderCell("Дод. iнформацiя");
                    column.Font(font =>
                    {
                        font.Size(12);
                    });
                    column.PaddingTop(10);
                    column.PaddingBottom(10);
                });

            })
            .MainTableEvents(events =>
            {
                events.DataSourceIsEmpty(message: "There is no data available to display.");
            })
            .Export(export =>
            {
                export.ToExcel();
                export.ToCsv();
                export.ToXml();
            })
            .Generate(dat => dat.AsPdfFile(filepath));
        }
    }
}