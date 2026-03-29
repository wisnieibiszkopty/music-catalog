using Artists.Service.Core.Dto;
using FluentValidation;

namespace Artists.Service.Core.Validators;

public class CreateArtistValidator : AbstractValidator<ArtistDto>
{
    public CreateArtistValidator()
    {
        RuleFor(x => x.Id)
            .Length(22).WithMessage("ID must be exactly 22 characters")
            .When(x => !string.IsNullOrEmpty(x.Id)); 
        
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Artist name is required")
            .MaximumLength(255).WithMessage("Maximal name length is 255");

        RuleFor(x => x.Description)
            .MaximumLength(2000).WithMessage("Maximal name length is 2000");
        
        RuleFor(x => x.FoundedYear)
            .InclusiveBetween(1000, DateTime.Now.Year)
            .When(x => x.FoundedYear.HasValue)
            .WithMessage("Invalid foundation year.");
        
        RuleFor(x => x.ImageUrl)
            .Must(uri => string.IsNullOrEmpty(uri) || Uri.TryCreate(uri, UriKind.Absolute, out _))
            .WithMessage("Invalid image URL.");
    }
}