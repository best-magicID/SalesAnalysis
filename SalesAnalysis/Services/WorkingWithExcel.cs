using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.Win32;
using SalesAnalysis.Helpers;
using SalesAnalysis.Models;
using System.Collections.ObjectModel;

namespace SalesAnalysis.Services
{
    public class WorkingWithExcel : IWorkingWithExcel
    {

        /// <summary>
        /// Сохранение таблицы в Excel
        /// </summary>
        public void SaveToExcel(ObservableCollection<SalesModel> listSalesModels)
        {
            try
            {
                var nameFile = "Продажи " + DateTime.Now.ToShortDateString() + ".xlsx";

                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "Excel.xlsx|*.xlsx";
                saveFileDialog.RestoreDirectory = true;
                saveFileDialog.FileName = nameFile;
                bool? result = saveFileDialog.ShowDialog();

                if (result == null || result == false)
                {
                    return;
                }

                using (SpreadsheetDocument document = SpreadsheetDocument.Create(saveFileDialog.FileName, SpreadsheetDocumentType.Workbook))
                {
                    WorkbookPart workbookPart = document.AddWorkbookPart();
                    workbookPart.Workbook = new Workbook();

                    WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                    worksheetPart.Worksheet = new Worksheet(new SheetData());

                    WorkbookStylesPart workbookStylesPart = workbookPart.AddNewPart<WorkbookStylesPart>();

                    // Добавляем в документ набор стилей
                    workbookStylesPart.Stylesheet = CreateStyleForCell();
                    workbookStylesPart.Stylesheet.Save();

                    Sheets sheets = workbookPart.Workbook.AppendChild(new Sheets());
                    Sheet sheet = new Sheet()
                    {
                        Id = workbookPart.GetIdOfPart(worksheetPart),
                        SheetId = 1,
                        Name = "Лист1"
                    };
                    sheets.Append(sheet);

                    SheetData sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>() ?? new SheetData();

                    sheetData.AppendChild(CreateRowHeader(1));

                    int i = 2;
                    foreach (SalesModel salesModel in listSalesModels)
                    {
                        sheetData.AppendChild(CreateRow(i, salesModel));
                        i++;
                    }

                    workbookPart.Workbook.Save();

                    GeneralMethods.ShowNotification("Экспорт в Excel завершен.");
                }
            }
            catch (Exception ex)
            {
                GeneralMethods.ShowNotification(ex.Message);
            }
        }

        /// <summary>
        /// Создание ячейки
        /// </summary>
        /// <param name="numberColumn"></param>
        /// <param name="row"></param>
        /// <param name="valueCell"></param>
        /// <param name="typeCell"></param>
        /// <returns></returns>
        public Cell CreateCell(string numberColumn, Row row, string valueCell, CellValues typeCell)
        {
            Cell cell = new();

            var numberRow = int.TryParse(row.RowIndex, out int result) ? result : 0;
            cell.CellReference = numberColumn.ToString() + numberRow.ToString();

            if (typeCell == CellValues.Number)
            {
                cell.DataType = new EnumValue<CellValues>(CellValues.Number);
            }
            else
            {
                cell.DataType = new EnumValue<CellValues>(CellValues.String);
            }
            cell.CellValue = new CellValue(valueCell);
            cell.StyleIndex = 1;

            return cell;
        }

        /// <summary>
        /// Создание строки заголовка для Excel
        /// </summary>
        /// <param name="indexRow"></param>
        /// <returns></returns>
        public Row CreateRowHeader(int indexRow)
        {
            Row row = new Row() { RowIndex = Convert.ToUInt32(indexRow) };

            row.AppendChild(CreateCell("A", row, "Порядковый номер", CellValues.String));
            row.AppendChild(CreateCell("B", row, "Id", CellValues.String));
            row.AppendChild(CreateCell("C", row, "Название", CellValues.String));

            row.AppendChild(CreateCell("D", row, "Январь (Кол-во)", CellValues.String));
            row.AppendChild(CreateCell("E", row, "Январь (общая стоимость)", CellValues.String));

            row.AppendChild(CreateCell("F", row, "Февраль (Кол-во)", CellValues.String));
            row.AppendChild(CreateCell("G", row, "Февраль (общая стоимость)", CellValues.String));

            row.AppendChild(CreateCell("H", row, "Март (Кол-во)", CellValues.String));
            row.AppendChild(CreateCell("I", row, "Март (общая стоимость)", CellValues.String));

            row.AppendChild(CreateCell("J", row, "Апрель (Кол-во)", CellValues.String));
            row.AppendChild(CreateCell("K", row, "Апрель (общая стоимость)", CellValues.String));

            row.AppendChild(CreateCell("L", row, "Май (Кол-во)", CellValues.String));
            row.AppendChild(CreateCell("M", row, "Май (общая стоимость)", CellValues.String));

            row.AppendChild(CreateCell("N", row, "Июнь (Кол-во)", CellValues.String));
            row.AppendChild(CreateCell("O", row, "Июнь (общая стоимость)", CellValues.String));

            row.AppendChild(CreateCell("P", row, "Июль (Кол-во)", CellValues.String));
            row.AppendChild(CreateCell("Q", row, "Июль (общая стоимость)", CellValues.String));

            row.AppendChild(CreateCell("R", row, "Август (Кол-во)", CellValues.String));
            row.AppendChild(CreateCell("S", row, "Август (общая стоимость)", CellValues.String));

            row.AppendChild(CreateCell("T", row, "Сентябрь (Кол-во)", CellValues.String));
            row.AppendChild(CreateCell("U", row, "Сентябрь (общая стоимость)", CellValues.String));

            row.AppendChild(CreateCell("V", row, "Октябрь (Кол-во)", CellValues.String));
            row.AppendChild(CreateCell("W", row, "Октябрь (общая стоимость)", CellValues.String));

            row.AppendChild(CreateCell("X", row, "Ноябрь (Кол-во)", CellValues.String));
            row.AppendChild(CreateCell("Y", row, "Ноябрь (общая стоимость)", CellValues.String));

            row.AppendChild(CreateCell("Z", row, "Декабрь (Кол-во)", CellValues.String));
            row.AppendChild(CreateCell("AA", row, "Декабрь (общая стоимость)", CellValues.String));

            row.AppendChild(CreateCell("AB", row, "Год (Кол-во)", CellValues.String));
            row.AppendChild(CreateCell("AC", row, "Год (общая стоимость)", CellValues.String));

            return row;
        }

        /// <summary>
        /// Создание строки для Excel
        /// </summary>
        /// <param name="indexRow"></param>
        /// <param name="salesModel"></param>
        /// <returns></returns>
        public Row CreateRow(int indexRow, SalesModel salesModel)
        {
            Row row = new Row() { RowIndex = Convert.ToUInt32(indexRow) };

            row.AppendChild(CreateCell("A", row, (indexRow - 1).ToString(), CellValues.Number));
            row.AppendChild(CreateCell("B", row, salesModel.IdModel.ToString(), CellValues.Number));
            row.AppendChild(CreateCell("C", row, salesModel.GetNameModel(), CellValues.String));

            row.AppendChild(CreateCell("D", row, salesModel.TotalAmountForJanuary.ToString(), CellValues.Number));
            row.AppendChild(CreateCell("E", row, salesModel.TotalCostForJanuary.ToString(), CellValues.Number));

            row.AppendChild(CreateCell("F", row, salesModel.TotalAmountForFebruary.ToString(), CellValues.Number));
            row.AppendChild(CreateCell("G", row, salesModel.TotalCostForFebruary.ToString(), CellValues.Number));

            row.AppendChild(CreateCell("H", row, salesModel.TotalAmountForMarch.ToString(), CellValues.Number));
            row.AppendChild(CreateCell("I", row, salesModel.TotalCostForMarch.ToString(), CellValues.Number));

            row.AppendChild(CreateCell("J", row, salesModel.TotalAmountForApril.ToString(), CellValues.Number));
            row.AppendChild(CreateCell("K", row, salesModel.TotalCostForApril.ToString(), CellValues.Number));

            row.AppendChild(CreateCell("L", row, salesModel.TotalAmountForMay.ToString(), CellValues.Number));
            row.AppendChild(CreateCell("M", row, salesModel.TotalCostForMay.ToString(), CellValues.Number));

            row.AppendChild(CreateCell("N", row, salesModel.TotalAmountForJune.ToString(), CellValues.Number));
            row.AppendChild(CreateCell("O", row, salesModel.TotalCostForJune.ToString(), CellValues.Number));

            row.AppendChild(CreateCell("P", row, salesModel.TotalAmountForJuly.ToString(), CellValues.Number));
            row.AppendChild(CreateCell("Q", row, salesModel.TotalCostForJuly.ToString(), CellValues.Number));

            row.AppendChild(CreateCell("R", row, salesModel.TotalAmountForAugust.ToString(), CellValues.Number));
            row.AppendChild(CreateCell("S", row, salesModel.TotalCostForAugust.ToString(), CellValues.Number));

            row.AppendChild(CreateCell("T", row, salesModel.TotalAmountForSeptember.ToString(), CellValues.Number));
            row.AppendChild(CreateCell("U", row, salesModel.TotalCostForSeptember.ToString(), CellValues.Number));

            row.AppendChild(CreateCell("V", row, salesModel.TotalAmountForOctober.ToString(), CellValues.Number));
            row.AppendChild(CreateCell("W", row, salesModel.TotalCostForOctober.ToString(), CellValues.Number));

            row.AppendChild(CreateCell("X", row, salesModel.TotalAmountForNovember.ToString(), CellValues.Number));
            row.AppendChild(CreateCell("Y", row, salesModel.TotalCostForNovember.ToString(), CellValues.Number));

            row.AppendChild(CreateCell("Z", row, salesModel.TotalAmountForDecember.ToString(), CellValues.Number));
            row.AppendChild(CreateCell("AA", row, salesModel.TotalCostForDecember.ToString(), CellValues.Number));

            row.AppendChild(CreateCell("AB", row, salesModel.TotalAmountForYear.ToString(), CellValues.Number));
            row.AppendChild(CreateCell("AC", row, salesModel.TotalCostForYear.ToString(), CellValues.Number));

            return row;
        }


        /// <summary>
        /// Создание стилей для ячеек
        /// </summary>
        /// <returns></returns>
        private static Stylesheet CreateStyleForCell()
        {
            return new Stylesheet(
                new Fonts(
                    new Font(
                        new FontSize() { Val = 11 },
                        new Color() { Rgb = new HexBinaryValue() { Value = "000000" } },
                        new FontName() { Val = "Times New Roman" })
                ),

                new Fills(
                    // Заполнение ячейки по умолчанию
                    new Fill(
                        new PatternFill() { PatternType = PatternValues.None }),

                    // Заполнение ячейки серым цветом
                    new Fill(
                        new PatternFill(
                            new ForegroundColor() { Rgb = new HexBinaryValue() { Value = "FFAAAAAA" } }
                            )
                        { PatternType = PatternValues.Solid }),

                    // Заполнение ячейки красным
                    new Fill(
                        new PatternFill(
                            new ForegroundColor() { Rgb = new HexBinaryValue() { Value = "FFFFAAAA" } }
                        )
                        { PatternType = PatternValues.Solid })
                )
                ,
                new Borders(
                    // 0
                    new Border(
                        new LeftBorder(),
                        new RightBorder(),
                        new TopBorder(),
                        new BottomBorder(),
                        new DiagonalBorder()),

                    // 1
                    new Border(
                        new LeftBorder(
                            new Color() { Auto = true }
                        )
                        { Style = BorderStyleValues.Medium },
                        new RightBorder(
                            new Color() { Indexed = (UInt32Value)64U }
                        )
                        { Style = BorderStyleValues.Medium },
                        new TopBorder(
                            new Color() { Auto = true }
                        )
                        { Style = BorderStyleValues.Medium },
                        new BottomBorder(
                            new Color() { Indexed = (UInt32Value)64U }
                        )
                        { Style = BorderStyleValues.Medium },
                        new DiagonalBorder()),

                    // 2 - Грани
                    new Border(
                        new LeftBorder(
                            new Color() { Auto = true }
                        )
                        { Style = BorderStyleValues.Thin },
                        new RightBorder(
                            new Color() { Indexed = (UInt32Value)64U }
                        )
                        { Style = BorderStyleValues.Thin },
                        new TopBorder(
                            new Color() { Auto = true }
                        )
                        { Style = BorderStyleValues.Thin },
                        new BottomBorder(
                            new Color() { Indexed = (UInt32Value)64U }
                        )
                        { Style = BorderStyleValues.Thin },
                        new DiagonalBorder())
                ),

                new CellFormats(
                    // По умолчанию
                    new CellFormat() { FontId = 0, FillId = 0, BorderId = 1 },

                    // Times New Roman - грани
                    new CellFormat(new Alignment()
                    {
                        Horizontal = HorizontalAlignmentValues.Center,
                        Vertical = VerticalAlignmentValues.Center,
                        WrapText = true
                    })
                    {
                        FontId = 0,
                        FillId = 0,
                        BorderId = 2,
                        ApplyFont = true
                    }
                )
            );
        }

    }
}
