﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Authentication.Entities
{
    public class User
    {
        public int Id { get; set; }

        public string Username { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        public string GivenName { get; set; }

        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        public string Email { get; set; }

        [NotMapped]
        [StringLength(100, ErrorMessage = "The password must be at least {2} characters in length.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [NotMapped]
        [StringLength(100, ErrorMessage = "The password must be at least {2} characters in length.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; }

        [NotMapped]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Password and confirm password do not match.")]
        public string ConfirmPassword { get; set; }

        public bool IsEnabled { get; set; }

        [NotMapped]
        public string Token { get; set; }

        [NotMapped]
        public ICollection<Role> Roles { get; set; }
    }
}
