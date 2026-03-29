using Catalog.Service.Core.Models;
using FluentValidation;

namespace Catalog.Service.Core.Validators;

public class TrackValidator : AbstractValidator<Track>
{
    public TrackValidator()
    {
        RuleFor(t => t.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(t => t.DurationMs)
            .GreaterThan(0);

        RuleFor(t => t.TrackNumber)
            .GreaterThan(0);

        RuleFor(t => t.AlbumId)
            .NotEmpty();
    }
}