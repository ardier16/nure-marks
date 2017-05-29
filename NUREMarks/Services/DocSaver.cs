using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUREMarks.Models;

namespace NUREMarks.Services
{
    public class DocSaver
    {
        public static void CreateWordDoc(string filepath, List<StudentData> data)
        {
            using (WordprocessingDocument document = WordprocessingDocument.Create(filepath, WordprocessingDocumentType.Document))
            {
                MainDocumentPart mainPart = document.AddMainDocumentPart();
                mainPart.Document = new Document();
                Body body = mainPart.Document.AppendChild(new Body());

                var doc = document.MainDocumentPart.Document;

                Table table = new Table();

                TableProperties props = new TableProperties(
                    new TableBorders(
                    new TopBorder
                    {
                        Val = new EnumValue<BorderValues>(BorderValues.Single),
                        Size = 12
                    },
                    new BottomBorder
                    {
                        Val = new EnumValue<BorderValues>(BorderValues.Single),
                        Size = 12
                    },
                    new LeftBorder
                    {
                        Val = new EnumValue<BorderValues>(BorderValues.Single),
                        Size = 12
                    },
                    new RightBorder
                    {
                        Val = new EnumValue<BorderValues>(BorderValues.Single),
                        Size = 12
                    },
                    new InsideHorizontalBorder
                    {
                        Val = new EnumValue<BorderValues>(BorderValues.Single),
                        Size = 12
                    },
                    new InsideVerticalBorder
                    {
                        Val = new EnumValue<BorderValues>(BorderValues.Single),
                        Size = 12
                    }));

                table.AppendChild<TableProperties>(props);

                for (var i = 0; i < data.Count; i++)
                {
                    var tr = new TableRow();

                    var tc1 = new TableCell();
                    tc1.Append(new Paragraph(new Run(new Text(i.ToString()))));
                    tc1.Append(new TableCellProperties(
                        new TableCellWidth { Type = TableWidthUnitValues.Pct, Width = "10" }));
                    tr.Append(tc1);

                    var tc2 = new TableCell();
                    tc2.Append(new Paragraph(new Run(new Text(data[i].Name))));
                    tc2.Append(new TableCellProperties(
                        new TableCellWidth { Type = TableWidthUnitValues.Pct, Width = "35" }));
                    tr.Append(tc2);

                    var tc3 = new TableCell();
                    tc3.Append(new Paragraph(new Run(new Text(data[i].Group))));
                    tc3.Append(new TableCellProperties(
                        new TableCellWidth { Type = TableWidthUnitValues.Pct, Width = "20" }));
                    tr.Append(tc3);

                    var tc4 = new TableCell();
                    tc4.Append(new Paragraph(new Run(new Text(data[i].Rating.ToString()))));
                    tc4.Append(new TableCellProperties(
                        new TableCellWidth { Type = TableWidthUnitValues.Pct, Width = "15" }));
                    tr.Append(tc4);

                    var tc5 = new TableCell();
                    tc5.Append(new Paragraph(new Run(new Text(data[i].Info))));
                    tc5.Append(new TableCellProperties(
                        new TableCellWidth { Type = TableWidthUnitValues.Pct, Width = "20" }));
                    tr.Append(tc5);

                    table.Append(tr);
                }
                doc.Body.Append(table);
                doc.Save();
            }
        }
    }
}
