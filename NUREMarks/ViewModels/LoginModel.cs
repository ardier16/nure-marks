using System.ComponentModel.DataAnnotations;

namespace NUREMarks.ViewModels
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Не вказано E-Mail")]
        public string EMail { get; set; }

        [Required(ErrorMessage = "Не вказано пароль")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
