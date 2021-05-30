using FluentValidation;
using Microsoft.Extensions.Localization;
using P3AddNewFunctionalityDotNetCore.Models.ViewModels;

namespace P3AddNewFunctionalityDotNetCore.Models.Validation
{
    public class ProductViewModelValidator : AbstractValidator<ProductViewModel>
    {
        public ProductViewModelValidator(IStringLocalizer localizer)
        {
            RuleFor(p => p.Name)
               .NotEmpty()
               .WithMessage(localizer["MissingName"]);

            RuleFor(p => p.Price)
               .Cascade(CascadeMode.Stop)
               .NotEmpty().WithMessage(_ => localizer["MissingPrice"])
               .Must(value => int.TryParse(value, out _)).WithMessage(_ => localizer["PriceNotANumber"])
               .Must(value => int.Parse(value) > 0).WithMessage(_ => localizer["PriceNotGreaterThanZero"]);

            RuleFor(p => p.Stock)
               .Cascade(CascadeMode.Stop)
               .NotEmpty().WithMessage(_ => localizer["MissingQuantity"])
               .Must(value => int.TryParse(value, out _)).WithMessage(_ => localizer["StockNotAnInteger"])
               .Must(value => int.Parse(value) > 0).WithMessage(_ => localizer["StockNotGreaterThanZero"]);
        }
    }
}