using Catalog.Service.Core.Models;
using FluentValidation;

namespace Catalog.Service.Core.Validators;

public class AlbumValidator : AbstractValidator<Album>
{
    public AlbumValidator()
    {
        RuleFor(a => a.Id)
            .Length(22).WithMessage("ID must be exactly 22 characters")
            .When(x => !string.IsNullOrEmpty(x.Id)); 
        
        RuleFor(a => a.ArtistId)
            .NotEmpty()
            .Length(22).WithMessage("ID must be exactly 22 characters");

        RuleFor(a => a.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(a => a.ReleaseDate)
            .NotEmpty()
            .Must(BeAValidDate).WithMessage("ReleaseDate must be a valid date");

        RuleFor(a => a.TotalTracks)
            .GreaterThan(0);

        RuleFor(a => a)
            .Must(a => a.TotalTracks == a.Tracks.Count)
            .WithMessage(a => $"TotalTracks ({a.TotalTracks}) does not match number of tracks ({a.Tracks.Count})");

        RuleForEach(a => a.Tracks).SetValidator(new TrackValidator());
    }
    
    private bool BeAValidDate(string date)
    {
        return DateTime.TryParse(date, out _);
    }
}