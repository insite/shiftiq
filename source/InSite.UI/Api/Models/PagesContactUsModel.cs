using System.ComponentModel.DataAnnotations;

namespace InSite.Api.Models
{
    public class PagesContactUsModel
    {
        public string Name { get; set; }

        [Required(ErrorMessage = @"The Email field is required."), EmailAddress(ErrorMessage = @"Invalid email address.")]
        public string Email { get; set; }

        public string Phone { get; set; }

        public string Subject { get; set; }

        [Required(ErrorMessage = @"The Message field is required.")]
        public string Message { get; set; }

        public string RequestUrl { get; set; }
    }
}