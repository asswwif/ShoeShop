using System;
using System.Data;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Word = Microsoft.Office.Interop.Word;

namespace WindowsFormsApp2
{
    public class WordReportService : IDisposable
    {
        private Word.Application wordApp;
        private Word.Document wordDoc;

        public WordReportService()
        {
            try
            {
                wordApp = new Word.Application();
                wordApp.Visible = false;
                wordApp.DisplayAlerts = Word.WdAlertLevel.wdAlertsNone;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не вдалося запустити Word. Переконайтеся, що Microsoft Word встановлений.\n\nПомилка: {ex.Message}",
                    "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw;
            }
        }

        public void Dispose()
        {
            try
            {
                if (wordDoc != null)
                {
                    try
                    {
                        wordDoc.Close(false);
                    }
                    catch { }

                    Marshal.ReleaseComObject(wordDoc);
                    wordDoc = null;
                }

                if (wordApp != null)
                {
                    try
                    {
                        wordApp.Quit();
                    }
                    catch { }

                    Marshal.ReleaseComObject(wordApp);
                    wordApp = null;
                }

                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
            }
            catch { }
        }

        public void CreateNewDocument()
        {
            try
            {
                wordDoc = wordApp.Documents.Add();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка створення документа: {ex.Message}", "Помилка");
                throw;
            }
        }

        public void SaveAndClose(string fullPathAndFilename)
        {
            try
            {
                if (wordDoc != null)
                {
                    string extension = System.IO.Path.GetExtension(fullPathAndFilename).ToLower();
                    object missing = Type.Missing;

                    if (extension == ".pdf")
                    {
                        // Зберігаємо як PDF
                        wordDoc.SaveAs2(
                            FileName: fullPathAndFilename,
                            FileFormat: Word.WdSaveFormat.wdFormatPDF
                        );
                    }
                    else
                    {
                        // Зберігаємо як Word документ
                        wordDoc.SaveAs2(
                            FileName: fullPathAndFilename,
                            FileFormat: Word.WdSaveFormat.wdFormatXMLDocument
                        );
                    }

                    wordDoc.Close(SaveChanges: false);
                    Marshal.ReleaseComObject(wordDoc);
                    wordDoc = null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка збереження: {ex.Message}\n\nШлях: {fullPathAndFilename}", "Помилка");
                throw;
            }
        }

        private void AddParagraph(string text, int fontSize = 14, bool isBold = false,
            Word.WdParagraphAlignment alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft)
        {
            try
            {
                Word.Paragraph para = wordDoc.Content.Paragraphs.Add();
                para.Range.Text = text;
                para.Range.Font.Size = fontSize;
                para.Range.Font.Bold = isBold ? 1 : 0;
                para.Range.Font.Name = "Times New Roman";
                para.Alignment = alignment;
                para.Range.InsertParagraphAfter();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка додавання параграфу: {ex.Message}", "Помилка");
            }
        }

        private Word.Table CreateTable(int rows, int cols)
        {
            try
            {
                Word.Range range = wordDoc.Content;
                range.Collapse(Word.WdCollapseDirection.wdCollapseEnd);
                Word.Table table = wordDoc.Tables.Add(range, rows, cols);
                table.Borders.Enable = 1;
                return table;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка створення таблиці: {ex.Message}", "Помилка");
                return null;
            }
        }

        private void FormatTableHeader(Word.Table table)
        {
            try
            {
                for (int col = 1; col <= table.Columns.Count; col++)
                {
                    Word.Cell cell = table.Cell(1, col);
                    cell.Range.Font.Bold = 1;
                    cell.Range.Font.Size = 12;
                    cell.Range.Font.Name = "Times New Roman";
                    cell.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка форматування заголовку: {ex.Message}", "Помилка");
            }
        }

        public void GenerateSalesReport(DataTable salesData, string fileName,
            DateTime startDate, DateTime endDate)
        {
            try
            {
                CreateNewDocument();

                // Заголовок
                AddParagraph("ЗВІТ ПО ПРОДАЖАМ", 14, true,
                    Word.WdParagraphAlignment.wdAlignParagraphCenter);
                AddParagraph($"Період: {startDate:dd.MM.yyyy} - {endDate:dd.MM.yyyy}", 14, false,
                    Word.WdParagraphAlignment.wdAlignParagraphCenter);
                AddParagraph($"Дата формування: {DateTime.Now:dd.MM.yyyy HH:mm}", 14, false,
                    Word.WdParagraphAlignment.wdAlignParagraphCenter);
                AddParagraph("", 14);

                // Створення таблиці
                int rowCount = salesData.Rows.Count + 1;
                Word.Table table = CreateTable(rowCount, 9);

                if (table == null) return;

                // Заголовки таблиці
                table.Cell(1, 1).Range.Text = "Чек";
                table.Cell(1, 2).Range.Text = "Дата";
                table.Cell(1, 3).Range.Text = "Клієнт";
                table.Cell(1, 4).Range.Text = "Товар";
                table.Cell(1, 5).Range.Text = "Колір";
                table.Cell(1, 6).Range.Text = "Розмір";
                table.Cell(1, 7).Range.Text = "Кількість";
                table.Cell(1, 8).Range.Text = "Ціна за од.";
                table.Cell(1, 9).Range.Text = "Сума";

                FormatTableHeader(table);

                // Дані
                int row = 2;
                decimal totalSum = 0;
                int totalQuantity = 0;

                foreach (DataRow dataRow in salesData.Rows)
                {
                    table.Cell(row, 1).Range.Text = dataRow["sale_id"].ToString();
                    table.Cell(row, 2).Range.Text = Convert.ToDateTime(dataRow["sale_datetime"]).ToString("dd.MM.yyyy");
                    table.Cell(row, 3).Range.Text = dataRow["customer_name"].ToString();
                    table.Cell(row, 4).Range.Text = dataRow["product_name"].ToString();
                    table.Cell(row, 5).Range.Text = dataRow["color_name"].ToString();
                    table.Cell(row, 6).Range.Text = dataRow["size_value"].ToString();

                    int quantity = Convert.ToInt32(dataRow["quantity"]);
                    decimal unitPrice = Convert.ToDecimal(dataRow["unit_price"]);
                    decimal lineSum = quantity * unitPrice;

                    table.Cell(row, 7).Range.Text = quantity.ToString();
                    table.Cell(row, 8).Range.Text = unitPrice.ToString("F2");
                    table.Cell(row, 9).Range.Text = lineSum.ToString("F2");

                    // Форматування рядка даних
                    for (int col = 1; col <= 9; col++)
                    {
                        table.Cell(row, col).Range.Font.Size = 12;
                        table.Cell(row, col).Range.Font.Name = "Times New Roman";
                    }

                    totalSum += lineSum;
                    totalQuantity += quantity;

                    row++;
                }

                // Додаткова інформація
                AddParagraph("", 14);
                AddParagraph($"Кількість чеків: {salesData.Rows.Count}", 14, true);
                AddParagraph($"Загальна кількість товарів: {totalQuantity}", 14, true);
                AddParagraph($"Загальна сума продажів: {totalSum:F2} грн", 14, true);

                SaveAndClose(fileName);

                MessageBox.Show($"Звіт успішно збережено:\n{fileName}", "Успіх",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка генерації звіту: {ex.Message}\n\nStack Trace: {ex.StackTrace}",
                    "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
   
        public void GenerateReturnsReport(DataTable returnsData, string fileName,
            DateTime startDate, DateTime endDate)
        {
            try
            {
                CreateNewDocument();

                // Заголовок
                AddParagraph("ЗВІТ ПО ПОВЕРНЕННЯХ", 14, true,
                    Word.WdParagraphAlignment.wdAlignParagraphCenter);
                AddParagraph($"Період: {startDate:dd.MM.yyyy} - {endDate:dd.MM.yyyy}", 14, false,
                    Word.WdParagraphAlignment.wdAlignParagraphCenter);
                AddParagraph($"Дата формування: {DateTime.Now:dd.MM.yyyy HH:mm}", 14, false,
                    Word.WdParagraphAlignment.wdAlignParagraphCenter);
                AddParagraph("", 14);

                // Створення таблиці
                int rowCount = returnsData.Rows.Count + 1;
                Word.Table table = CreateTable(rowCount, 7);

                if (table == null) return;

                // Заголовки
                table.Cell(1, 1).Range.Text = "ID";
                table.Cell(1, 2).Range.Text = "Дата";
                table.Cell(1, 3).Range.Text = "Товар";
                table.Cell(1, 4).Range.Text = "Кількість";
                table.Cell(1, 5).Range.Text = "Сума повернення";
                table.Cell(1, 6).Range.Text = "Призначення";
                table.Cell(1, 7).Range.Text = "Причина";

                FormatTableHeader(table);

                // Дані
                int row = 2;
                decimal totalRefund = 0;

                foreach (DataRow dataRow in returnsData.Rows)
                {
                    table.Cell(row, 1).Range.Text = dataRow["return_id"].ToString();
                    table.Cell(row, 2).Range.Text = Convert.ToDateTime(dataRow["return_datetime"]).ToString("dd.MM.yyyy");
                    table.Cell(row, 3).Range.Text = dataRow["product_name"].ToString();
                    table.Cell(row, 4).Range.Text = dataRow["quantity"].ToString();

                    decimal refund = Convert.ToDecimal(dataRow["refund_amount"]);
                    table.Cell(row, 5).Range.Text = refund.ToString("F2");
                    table.Cell(row, 6).Range.Text = dataRow["return_destination"].ToString();
                    table.Cell(row, 7).Range.Text = dataRow["reason"].ToString();

                    // Форматування рядка даних
                    for (int col = 1; col <= 7; col++)
                    {
                        table.Cell(row, col).Range.Font.Size = 12;
                        table.Cell(row, col).Range.Font.Name = "Times New Roman";
                    }

                    totalRefund += refund;
                    row++;
                }

                // Додаткова інформація
                AddParagraph("", 14);
                AddParagraph($"Загальна сума повернень: {totalRefund:F2} грн", 14, true);

                SaveAndClose(fileName);

                MessageBox.Show($"Звіт успішно збережено:\n{fileName}", "Успіх",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка генерації звіту: {ex.Message}",
                    "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}