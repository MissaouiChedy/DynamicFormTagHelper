using System;
using System.ComponentModel.DataAnnotations;

namespace DynamicFormTagHelper.Models
{
    public class PersonViewModel
    {
        [Required]
        [Display(Name = "Person's identification number")]
        public int? Id { get; set; }

        [Required]
        [Display(Name = "Person's full name")]
        public string Name { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Birth date", Prompt = "YYYY-MM-DD", Description = "YYYY-MM-DD")]
        public DateTime? BirthDate { get; set; }

        [Display(Name = "BasketBall Player?")]
        public bool IsBasketBallPlayer { get; set; } = false;

        public string IsBasketBallPlayerVM => IsBasketBallPlayer ? "Yes" : "No";

        public override string ToString()
        {
            return $@" Id: {Id}
Name: {Name}
BirthDate: {BirthDate.ToString()}
BasketBall Player?: {IsBasketBallPlayerVM}
";
        }
    }
}
