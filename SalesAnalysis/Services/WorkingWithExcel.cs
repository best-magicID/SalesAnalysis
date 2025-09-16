using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
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
        public async Task SaveToExcel(ObservableCollection<SalesByYear> listSalesModels, string pathFile)
        {
            try
            {
                await Task.Run(() =>
                {
                    // Создание копии на момент экспорта
                    var tempListSalesModels = new ObservableCollection<SalesByYear>(listSalesModels);

                    using (SpreadsheetDocument document = SpreadsheetDocument.Create(pathFile, SpreadsheetDocumentType.Workbook))
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
                        foreach (SalesByYear salesModel in tempListSalesModels)
                        {
                            sheetData.AppendChild(CreateRow(i, salesModel));
                            i++;
                        }

                        workbookPart.Workbook.Save();

                        GeneralMethods.ShowNotification("Экспорт в Excel завершен.");
                    }
                } );
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
            Row row = new Row() { RowIndex = (uint)indexRow };

            row.AppendChild(CreateCell("A", row, "Порядковый номер", CellValues.String));
            row.AppendChild(CreateCell("B", row, "Id", CellValues.String));
            row.AppendChild(CreateCell("C", row, "Название", CellValues.String));

            string[] months =
            {
                "Январь","Февраль","Март","Апрель","Май","Июнь",
                "Июль","Август","Сентябрь","Октябрь","Ноябрь","Декабрь", "Год"
            };

            int columnIndex = 4; // Начинаем с D

            foreach (var month in months)
            {
                string colCount = GetExcelColumnName(columnIndex++);
                string colCost = GetExcelColumnName(columnIndex++);

                row.AppendChild(CreateCell(colCount, row, $"{month} (Кол-во)", CellValues.String));
                row.AppendChild(CreateCell(colCost, row, $"{month} (Общая стоимость)", CellValues.String));
            }

            return row;
        }

        /// <summary>
        /// Создание строки для Excel
        /// </summary>
        /// <param name="indexRow"></param>
        /// <param name="salesModel"></param>
        /// <returns></returns>
        public Row CreateRow(int indexRow, SalesByYear salesModel)
        {
            Row row = new Row() { RowIndex = (uint)indexRow };

            row.AppendChild(CreateCell("A", row, (indexRow - 1).ToString(), CellValues.Number));
            row.AppendChild(CreateCell("B", row, salesModel.IdModel.ToString(), CellValues.Number));
            row.AppendChild(CreateCell("C", row, salesModel.NameModel, CellValues.String));

            int colIndex = 4;

            for (int i = 0; i < 12; i++)
            {
                string colCount = GetExcelColumnName(colIndex++);
                string colCost = GetExcelColumnName(colIndex++);

                row.AppendChild(CreateCell(colCount, row, salesModel.ArrAllTotalAmounts[i].ToString(), CellValues.Number));
                row.AppendChild(CreateCell(colCost, row, salesModel.ArrAllTotalCosts[i].ToString(), CellValues.Number));
            }

            string totalCountCol = GetExcelColumnName(colIndex++);
            string totalCostCol = GetExcelColumnName(colIndex++);

            row.AppendChild(CreateCell(totalCountCol, row, salesModel.TotalAmountForYear.ToString(), CellValues.Number));
            row.AppendChild(CreateCell(totalCostCol, row, salesModel.TotalCostForYear.ToString(), CellValues.Number));

            return row;
        }

        /// <summary>
        /// Получение номера столбца в Excel по его индексу
        /// </summary>
        /// <param name="columnNumber"></param>
        /// <returns></returns>
        private string GetExcelColumnName(int columnNumber)
        {
            string columnName = string.Empty;
            while (columnNumber > 0)
            {
                int modulo = (columnNumber - 1) % 26;
                columnName = Convert.ToChar('A' + modulo) + columnName;
                columnNumber = (columnNumber - modulo) / 26;
            }
            return columnName;
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
