using System.Diagnostics.CodeAnalysis;

namespace Domain.SharedKernel.Exceptions.PublicExceptions;

[ExcludeFromCodeCoverage]
public class ValueIsUnsupportedException(string publicMessage = "Value is unsupported")
    : PublicException(publicMessage);