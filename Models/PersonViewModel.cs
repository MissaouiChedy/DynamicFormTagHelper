using DynamicFormTagHelper.TagHelpers;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DynamicFormTagHelper.Models
{
    public enum Sex
    {
        MALE,
        FEMALE,
        OTHER
    }

    public class PersonViewModel
    {
        [Required]
        [Display(Name = "Person's identification number")]
        public int? Id { get; set; }

        [Required]
        [Display(Name = "Person's full name")]

        [AutoComplete(SuggestionsProperty="SuggestedNames")]
        public string Name { get; set; }

        public List<string> SuggestedNames => new List<string>() { "John", "Johnson", "Chirmoi", "Bay Sargo", "Mahmoud"};

        [Required]
        [Display(Name = "Person's Gender")]
        [ItemsSource(ItemsEnum = typeof(Sex))]
        public Sex Gender { get; set; }
        
        [DataType(DataType.Date)]
        [Display(Name = "Birth date")]
        public DateTime? BirthDate { get; set; }

        [Display(Name = "BasketBall Player?")]
        public bool IsBasketBallPlayer { get; set; } = false;

        [Required]
        [Display(Name = "Item")]
        [ItemsSource(ItemsProperty = nameof(Items))]
        public string SelectedItem { get; set; }

        [Required]
        [Display(Name = "Multiple Items")]
        [ItemsSource(ItemsProperty = nameof(Items))]
        public List<string> SelectedItems { get; set; }

        [Required]
        [Display(Name = "Single Item")]
        [ItemsSource(ItemsProperty = nameof(Items), ChoicesType = ChoicesTypes.RADIO)]
        public string SingleItem { get; set; }
        
        

        [Display(Name = "Person's Dog")]
        public DogViewModel OwnedDog { get; set; } = new DogViewModel();

        public string IsBasketBallPlayerVM => IsBasketBallPlayer ? "Yes" : "No";

        public List<SelectListItem> Items => new List<SelectListItem>()
        {
            new SelectListItem {Value = "0", Text="Glove", Selected = true},
            new SelectListItem {Value = "1", Text="Umbrella"},
            new SelectListItem {Value = "2", Text="Laptop"},
            new SelectListItem {Value = "3", Text="Gamepad"},
        };
    }

    public class DogViewModel
    {
        [Display(Name="Dog's Id")]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Dog's Name")]
        public string Name { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Birth date")]
        public DateTime BirthDate { get; set; }

        public Leash DogLeash { get; set; } = new Leash();
    }

    public class Leash
    {
        public string Name { get; set; }

        [Required]
        [Display(Name="Type of the leash")]
        [ItemsSource(ItemsProperty = nameof(KindsOfLeashes))]
        public int LeashKind { get; set; }

        public List<SelectListItem> KindsOfLeashes => new List<SelectListItem>
        {
            new SelectListItem(){Value = "0", Text = "High end"},
            new SelectListItem(){Value = "1", Text = "Mid tier"},
            new SelectListItem(){Value = "2", Text = "Low end"},
        };
    }
}
