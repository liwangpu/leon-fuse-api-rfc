using OfficeOpenXml;
using System.Text;

namespace ApiServer.Libraries
{
    public static class EpplusHelper
    {
        public static string ContentToString(this ExcelWorksheet sheet)
        {
            var strBuilder = new StringBuilder();
            var endColumn = sheet.Dimension.End.Column;
            var endRow = sheet.Dimension.End.Row;
            for (int r = 1; r <= endRow; r++)
            {
                for (int c = 1; c <= endColumn; c++)
                {
                    var cell = sheet.Cells[r, c];
                    strBuilder.Append(cell.Value == null ? string.Empty : cell.Value.ToString().Trim());
                    if (c != endColumn)
                        strBuilder.Append(',');
                }
                if (r != endRow)
                    strBuilder.AppendLine();
            }

            return strBuilder.ToString();
        }
    }
}
