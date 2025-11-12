using System;
using System.Data;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;

namespace WindowsFormsApp2
{
    public class ExcelReportService : IDisposable
    {
        private Excel.Application excelApp;
        private Excel.Workbooks excelWorkbooks;
        private Excel.Workbook excelWorkbook;
        private Excel.Sheets excelSheets;
        private Excel.Worksheet excelWorksheet;
        private Excel.Range excelCells;

        public ExcelReportService()
        {
            excelApp = new Excel.Application();
            excelApp.Visible = false;
            excelApp.DisplayAlerts = false;
        }

        public void Dispose()
        {
            try
            {
                if (excelCells != null) Marshal.ReleaseComObject(excelCells);
                if (excelWorksheet != null) Marshal.ReleaseComObject(excelWorksheet);
                if (excelSheets != null) Marshal.ReleaseComObject(excelSheets);
                if (excelWorkbook != null) Marshal.ReleaseComObject(excelWorkbook);
                if (excelWorkbooks != null) Marshal.ReleaseComObject(excelWorkbooks);
                if (excelApp != null)
                {
                    excelApp.Quit();
                    Marshal.ReleaseComObject(excelApp);
                }

                excelCells = null;
                excelWorksheet = null;
                excelSheets = null;
                excelWorkbook = null;
                excelWorkbooks = null;
                excelApp = null;

                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка при закритті Excel: {ex.Message}", "Помилка");
            }
        }

        public void CreateNewBook(string fullPathAndFilename)
        {
            try
            {
                excelApp.SheetsInNewWorkbook = 1;
                excelWorkbooks = excelApp.Workbooks;
                excelWorkbooks.Add(Type.Missing);
                excelWorkbook = excelWorkbooks[1];
                excelSheets = excelWorkbook.Worksheets;
                excelWorksheet = (Excel.Worksheet)excelSheets[1];
                excelWorksheet.Name = "Звіт ShoeShop";

                excelWorkbook.SaveAs(
                    fullPathAndFilename,
                    Excel.XlFileFormat.xlWorkbookDefault,
                    Type.Missing, Type.Missing, false, false,
                    Excel.XlSaveAsAccessMode.xlNoChange,
                    Type.Missing, Type.Missing, Type.Missing,
                    Type.Missing, Type.Missing);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка створення книги: {ex.Message}", "Помилка");
                throw;
            }
        }

        public void SaveAndClose()
        {
            try
            {
                if (excelWorkbook != null)
                {
                    excelWorkbook.Save();
                    excelWorkbook.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка збереження: {ex.Message}", "Помилка");
            }
        }

        public void SetValue(string address, object value, bool isBold = false)
        {
            try
            {
                excelCells = excelWorksheet.Range[address];
                excelCells.Value2 = value;

                if (isBold)
                {
                    excelCells.Font.Bold = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка запису значення: {ex.Message}", "Помилка");
            }
        }

        public void SetCellStyle(string address, bool isBold = false, int fontSize = 11,
            string fontColor = "Black", string bgColor = null)
        {
            try
            {
                excelCells = excelWorksheet.Range[address];
                excelCells.Font.Bold = isBold;
                excelCells.Font.Size = fontSize;

                if (!string.IsNullOrEmpty(bgColor))
                {
                    excelCells.Interior.Color = System.Drawing.ColorTranslator.ToOle(
                        System.Drawing.Color.FromName(bgColor));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка стилю: {ex.Message}", "Помилка");
            }
        }

        public void MergeCells(string range)
        {
            try
            {
                excelCells = excelWorksheet.Range[range];
                excelCells.Merge();
                excelCells.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка об'єднання: {ex.Message}", "Помилка");
            }
        }

        public void SetBorders(string range)
        {
            try
            {
                excelCells = excelWorksheet.Range[range];
                excelCells.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                excelCells.Borders.Weight = Excel.XlBorderWeight.xlThin;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка границь: {ex.Message}", "Помилка");
            }
        }

        public void AutoFitColumns(string range)
        {
            try
            {
                excelCells = excelWorksheet.Range[range];
                excelCells.Columns.AutoFit();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка авторозміру: {ex.Message}", "Помилка");
            }
        }

        public void GenerateSalesReport(DataTable salesData, string fileName,
            DateTime startDate, DateTime endDate)
        {
            try
            {
                CreateNewBook(fileName);

                // Заголовок звіту
                SetValue("A1", "ЗВІТ ПО ПРОДАЖАМ", true);
                SetCellStyle("A1", true, 16, "Black", "LightPink");
                MergeCells("A1:I1");

                SetValue("A2", $"Період: {startDate:dd.MM.yyyy} - {endDate:dd.MM.yyyy}");
                MergeCells("A2:I2");

                SetValue("A3", $"Дата формування: {DateTime.Now:dd.MM.yyyy HH:mm}");
                MergeCells("A3:I3");

                // Заголовки таблиці
                int headerRow = 5;
                SetValue($"A{headerRow}", "Чек", true);
                SetValue($"B{headerRow}", "Дата та час", true);
                SetValue($"C{headerRow}", "Клієнт", true);
                SetValue($"D{headerRow}", "Товар", true);
                SetValue($"E{headerRow}", "Колір", true);
                SetValue($"F{headerRow}", "Розмір", true);
                SetValue($"G{headerRow}", "Кількість", true);
                SetValue($"H{headerRow}", "Ціна за од.", true);
                SetValue($"I{headerRow}", "Сума", true);

                SetCellStyle($"A{headerRow}:I{headerRow}", true, 11, "Black", "Pink");
                SetBorders($"A{headerRow}:I{headerRow}");

                // Дані
                int row = headerRow + 1;
                decimal totalSum = 0;
                int totalQuantity = 0;

                foreach (DataRow dataRow in salesData.Rows)
                {
                    SetValue($"A{row}", dataRow["sale_id"]);
                    SetValue($"B{row}", Convert.ToDateTime(dataRow["sale_datetime"]).ToString("dd.MM.yyyy HH:mm"));
                    SetValue($"C{row}", dataRow["customer_name"]);
                    SetValue($"D{row}", dataRow["product_name"]);
                    SetValue($"E{row}", dataRow["color_name"]);
                    SetValue($"F{row}", dataRow["size_value"]);

                    int quantity = Convert.ToInt32(dataRow["quantity"]);
                    decimal unitPrice = Convert.ToDecimal(dataRow["unit_price"]);
                    decimal lineSum = quantity * unitPrice;

                    SetValue($"G{row}", quantity);
                    SetValue($"H{row}", unitPrice);
                    SetValue($"I{row}", lineSum);

                    totalSum += lineSum;
                    totalQuantity += quantity;

                    SetBorders($"A{row}:I{row}");
                    row++;
                }

                // Підсумки
                row++;
                SetValue($"F{row}", "ВСЬОГО:", true);
                SetValue($"G{row}", totalQuantity, true);
                SetValue($"H{row}", "", true);
                SetValue($"I{row}", totalSum, true);
                SetCellStyle($"F{row}:I{row}", true, 12, "Black", "LightYellow");
                SetBorders($"F{row}:I{row}");

                // Додаткова інформація
                row += 2;
                SetValue($"A{row}", $"Кількість чеків: {salesData.Rows.Count}");
                row++;
                SetValue($"A{row}", $"Загальна кількість товарів: {totalQuantity}");
                row++;
                SetValue($"A{row}", $"Загальна сума продажів: {totalSum:F2} грн");

                // Автоширина колонок
                AutoFitColumns("A:I");
                try
                {
                    // A: Чек (короткий числовий ідентифікатор)
                    excelWorksheet.Columns["A"].ColumnWidth = 10;
                    // B: Дата/Час (зазвичай фіксована довжина)
                    excelWorksheet.Columns["B"].ColumnWidth = 18;
                    // G: Кількість
                    excelWorksheet.Columns["G"].ColumnWidth = 12;
                    // H: Ціна за од.
                    excelWorksheet.Columns["H"].ColumnWidth = 12;
                    // I: Сума
                    excelWorksheet.Columns["I"].ColumnWidth = 15;
                }
                catch (Exception ex)
                {
                    // Це дозволяє згенерувати звіт, навіть якщо не вдалося встановити ширину
                    Console.WriteLine($"Помилка ручного налаштування ширини: {ex.Message}");
                }

                SaveAndClose();

                MessageBox.Show($"Звіт успішно збережено:\n{fileName}", "Успіх",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка генерації звіту: {ex.Message}\n\nStack Trace: {ex.StackTrace}", "Помилка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void GenerateReturnsReport(DataTable returnsData, string fileName,
    DateTime startDate, DateTime endDate)
        {
            try
            {
                CreateNewBook(fileName);

                // Заголовок
                SetValue("A1", "ЗВІТ ПО ПОВЕРНЕННЯХ", true);
                SetCellStyle("A1", true, 16, "Black", "LightPink");
                MergeCells("A1:G1");  // Змінено з F1 на G1

                SetValue("A2", $"Період: {startDate:dd.MM.yyyy} - {endDate:dd.MM.yyyy}");
                MergeCells("A2:G2");  // Змінено з F2 на G2

                SetValue("A3", $"Дата формування: {DateTime.Now:dd.MM.yyyy HH:mm}");
                MergeCells("A3:G3");  // Змінено з F3 на G3

                // Заголовки
                int headerRow = 5;
                SetValue($"A{headerRow}", "ID", true);
                SetValue($"B{headerRow}", "Дата", true);
                SetValue($"C{headerRow}", "Товар", true);
                SetValue($"D{headerRow}", "Кількість", true);
                SetValue($"E{headerRow}", "Сума повернення", true);
                SetValue($"F{headerRow}", "Призначення", true);  // Нова колонка
                SetValue($"G{headerRow}", "Причина", true);      // Змінено з F на G

                SetCellStyle($"A{headerRow}:G{headerRow}", true, 11, "Black", "Pink");  // Змінено з F на G
                SetBorders($"A{headerRow}:G{headerRow}");  // Змінено з F на G

                // Дані
                int row = headerRow + 1;
                decimal totalRefund = 0;

                foreach (DataRow dataRow in returnsData.Rows)
                {
                    SetValue($"A{row}", dataRow["return_id"]);
                    SetValue($"B{row}", Convert.ToDateTime(dataRow["return_datetime"]).ToString("dd.MM.yyyy"));
                    SetValue($"C{row}", dataRow["product_name"]);
                    SetValue($"D{row}", dataRow["quantity"]);

                    decimal refund = Convert.ToDecimal(dataRow["refund_amount"]);
                    SetValue($"E{row}", refund);
                    SetValue($"F{row}", dataRow["return_destination"]);  // Нова колонка
                    SetValue($"G{row}", dataRow["reason"]);              // Змінено з F на G

                    totalRefund += refund;
                    SetBorders($"A{row}:G{row}");  // Змінено з F на G
                    row++;
                }

                // Підсумок
                row++;
                SetValue($"E{row}", "ВСЬОГО ПОВЕРНЕНЬ:", true);  // Змінено з D на E
                SetValue($"F{row}", totalRefund, true);          // Змінено з E на F
                SetCellStyle($"E{row}:F{row}", true, 12, "Black", "LightYellow");  // Змінено з D:E на E:F

                AutoFitColumns("A:G");  // Змінено з F на G
                SaveAndClose();

                MessageBox.Show($"Звіт успішно збережено:\n{fileName}", "Успіх",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка генерації звіту: {ex.Message}", "Помилка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}