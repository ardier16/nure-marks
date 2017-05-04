using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;


namespace NUREMarks.Models.MarksViewModels
{
    public class EditViewModel
    {
        [Required]
        [DataType(DataType.Text)]
        public int Value { get; set; }

        [Required]
        [DataType(DataType.Text)]
        public int MarkId { get; set; }
    }
}
