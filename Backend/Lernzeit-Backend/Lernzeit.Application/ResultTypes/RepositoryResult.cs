using FunicularSwitch.Generators;

namespace Lernzeit.Application.ResultTypes;

[ResultType(ErrorType = typeof(RepositoryError))]
public abstract partial class RepositoryResult<T>;

[UnionType(CaseOrder = CaseOrder.AsDeclared)]
public abstract partial record RepositoryError
{
    public sealed record NotFound_(string Message) : RepositoryError;
    public sealed record BadRequest_(string Message) : RepositoryError;
    
}
