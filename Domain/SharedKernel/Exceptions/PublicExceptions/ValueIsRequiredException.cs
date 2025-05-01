using System.Diagnostics.CodeAnalysis;

namespace Domain.SharedKernel.Exceptions.PublicExceptions;

[ExcludeFromCodeCoverage]
public class ValueIsRequiredException(string publicMessage = "Parameter is required")
    : PublicException(publicMessage);