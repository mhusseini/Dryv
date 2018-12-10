using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Dryv;
using DryvDemo.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace DryvDemo.ViewModels
{
    public class HomeViewModel
    {
        public static readonly DryvRules Rules = DryvRules.For<HomeViewModel>()
            .Rule(m => m.TaxId,
                m => !string.IsNullOrWhiteSpace(m.Company) && string.IsNullOrWhiteSpace(m.TaxId)
                    ? $"The tax ID for {m.Company} must be specified."
                    : DryvResult.Success);

        [Required]
        public string Name { get; set; }

        public string Company { get; set; }

        [DryvRules]
        [Remote(controller:"Validation", action:nameof(ValidationController.Validate), AdditionalFields = "Name,Company")]
        public string TaxId { get; set; }
    }
}