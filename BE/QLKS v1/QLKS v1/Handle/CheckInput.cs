using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace QLKS_v1.Handle
{
    public class CheckInput
    {
        public static string IsValidUsername(string username)
        {
            // Kiểm tra nếu chuỗi rỗng hoặc null
            if (string.IsNullOrWhiteSpace(username))
            {
                return "Tên đăng nhập không được để trống.";        
            }
            // Kiểm tra độ dài của chuỗi (phải có từ 3 đến 50 ký tự)
            if (username.Length < 3 || username.Length > 50)
            {
                return "Tên đăng nhập phải có từ 3 đến 50 ký tự.";
            }
            // Kiểm tra định dạng ký tự (chỉ cho phép chữ cái và số)
            foreach (char c in username)
            {
                if (!char.IsLetterOrDigit(c))
                {
                   return "Tên đăng nhập chỉ được chứa các ký tự chữ và số.";                
                }
            }
            return username;
        }
        public static string IsPassWord(string password)
        {
            Regex regex = new Regex("[!@#$%^&*()_+{}\\[\\]:;<>,.?/~`]");
            if (password.Length < 8 || password.Length > 20 || !regex.IsMatch(password))
                return "Password phải từ 8-20 kí tự và chứa ít nhất 1 kí tự đặc biệt !";
            else
                return password;
        }
        public static bool IsValiEmail(string email)
        {

            var CheckEmail = new EmailAddressAttribute();
            return CheckEmail.
                IsValid(email);
        }

        public static string IsValidPhoneNumber(string phoneNumber)
        {        

            if(phoneNumber == null)
            {
                return "Số điện thoại không được để trống !";
            }
            // Kiểm tra nếu số điện thoại chỉ chứa các chữ số và có thể bắt đầu với dấu '+'
            if (!System.Text.RegularExpressions.Regex.IsMatch(phoneNumber, @"^\+?[0-9]{10,15}$"))
            {
               return "Số điện thoại không hợp lệ. Số điện thoại phải chứa từ 10 đến 15 chữ số và có thể bắt đầu bằng dấu '+'.";
                
            }

            // Nếu tất cả các kiểm tra đều thành công, trả về true
            return phoneNumber;
        }

    }
}
