using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using Auditorius.PBPlatform.Analyzers.Common;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Auditorius.PBPlatform.Analyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class DomainDateTimeNewFormatAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "DomainDateTimeUtcNowAnalyzer";

        // You can change these strings in the Resources.resx file. If you do not want your analyzer to be localize-able, you can use regular strings for Title and MessageFormat.
        // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/Localizing%20Analyzers.md for more on localization
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.DomainDateTimeUtcNowAnalyzerTitle),
            Resources.ResourceManager,
            typeof (Resources));

        public static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.DomainDateTimeUtcNowAnalyzerMessageFormat),
            Resources.ResourceManager,
            typeof (Resources));

        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.DomainDateTimeUtcNowAnalyzerDescription),
            Resources.ResourceManager,
            typeof (Resources));

        private const string Category = "Format";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Error,
            isEnabledByDefault: true, description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(Rule); }
        }

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeSymbol, SyntaxKind.SimpleMemberAccessExpression);
        }

        private static void AnalyzeSymbol(SyntaxNodeAnalysisContext context)
        {
            var nodeExpression = (MemberAccessExpressionSyntax) context.Node;

            var property = context.SemanticModel.GetSymbolInfo(nodeExpression).Symbol as IPropertySymbol;

            if (property == null || !property.Type.Equals(context.SemanticModel.GetClrType(typeof (DateTime))))
            {
                return;
            }

            if (property.Name == "Now")
            {
                context.ReportDiagnostic(Diagnostic.Create(Rule, context.Node.GetLocation()));
            }
        }
    }
}