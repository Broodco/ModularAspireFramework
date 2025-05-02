using Microsoft.AspNetCore.Routing;

namespace ModularAspire.Common.Presentation.Endpoints;

public interface IEndpoint
{
    void MapEndpoint(IEndpointRouteBuilder app);
}