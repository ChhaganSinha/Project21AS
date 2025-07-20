using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Project21AS.Dto.Auth
{
    public class RegisterParameters
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        [Compare(nameof(Password), ErrorMessage = "Passwords do not match")]
        public string PasswordConfirm { get; set; }

        public int AllowedBatches { get; set; } = 4;
        public int MaxStudentPerBatch { get; set; } = 35;
        public bool IsActive { get; set; } = true;
    }

}
