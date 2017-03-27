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
        [Display(Name = "Birth date")]
        public DateTime? BirthDate { get; set; }

        [Display(Name = "BasketBall Player?")]
        public bool IsBasketBallPlayer { get; set; } = false;

        [Display(Name="Person's Dog")]
        public Dog OwnedDog { get; set; } 

        public string IsBasketBallPlayerVM => IsBasketBallPlayer ? "Yes" : "No";
    }

    public class Dog
    {
        [Display(Name="Dog's Id")]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Dog's Name")]
        public string Name { get; set; }
    }
}
