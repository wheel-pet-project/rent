using System.Diagnostics.CodeAnalysis;

namespace Domain.SharedKernel.Exceptions.InternalExceptions.AlreadyHaveThisState;

[ExcludeFromCodeCoverage]
public class AlreadyHaveThisStateException(string message = "Resource already has this state")
    : InternalException(message);