using ClosedXML.Excel;
using DocumentFormat.OpenXml.Wordprocessing;

namespace Bookify.web.Extentions
{
    public static class ExcelSheetExtensions
    {
        public static void AddHeaders(this IXLWorksheet sheet, string[] headers)
        {
            for (int i =0; i< headers.Length;i++)
            {
                sheet.Cell(1, i + 1).SetValue(headers[i]);
            }

            var Header = sheet.Range(1,1,1,(int) headers.Length);
              
            Header.Style.Fill.BackgroundColor = XLColor.Black;
            Header.Style.Font.FontColor = XLColor.White;
            Header.Style.Font.SetBold();

        }
        public static void formatsheet(this IXLWorksheet sheet)
        {
            sheet.ColumnsUsed().AdjustToContents();
            sheet.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            sheet.CellsUsed().Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            sheet.CellsUsed().Style.Border.OutsideBorderColor = XLColor.Black;

        }
    }
}
