namespace ModularAspire.Modules.Identity.Application.Abstractions.Mapping;

public interface IMapper<in TInput, out TOutput>
{
    TOutput Map(TInput input);
}
