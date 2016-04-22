using System;
using Microsoft.CodeAnalysis;

namespace Auditorius.PBPlatform.Analyzers.Common
{
    public static class ObjectExtensions
    {
        public static INamedTypeSymbol GetClrType(this SemanticModel model, Type type)
        {
            return model.Compilation.GetTypeByMetadataName(type.FullName);
        }
    }
}