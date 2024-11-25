using System.Text.RegularExpressions;

namespace QLKS_v1.Handle
{
    public class BillHelper
    {
        public static string UpdateString(string input, string keyword)
        {
            if (string.IsNullOrEmpty(input))
            {
                // Nếu chuỗi rỗng, gán giá trị mới
                return keyword;
            }

            // Biểu thức chính quy để tìm keyword có số đằng sau
            string pattern = $@"{keyword} \((\d+)\)";
            Match match = Regex.Match(input, pattern);

            if (match.Success)
            {
                // Nếu tìm thấy, tăng số đếm lên 1
                int currentNumber = int.Parse(match.Groups[1].Value);
                int newNumber = currentNumber + 1;
                return Regex.Replace(input, pattern, $"{keyword} ({newNumber})");
            }
            else
            {
                // Nếu không tìm thấy số, thêm "(1)" vào keyword
                return input + "," + keyword + " (1)";
            }
        }
    }
}
