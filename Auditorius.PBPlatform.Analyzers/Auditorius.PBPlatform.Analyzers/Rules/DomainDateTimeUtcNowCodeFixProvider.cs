using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Rename;
using Microsoft.CodeAnalysis.Text;

namespace Auditorius.PBPlatform.Analyzers
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(DomainDateTimeNewFormatCodeFixProvider)), Shared]
    public class DomainDateTimeNewFormatCodeFixProvider : CodeFixProvider
    {
        private const string title = "Transform DateTime.Now to DateTime.UtcNow";

        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(DomainDateTimeNewFormatAnalyzer.DiagnosticId); }
        }

        public sealed override FixAllProvider GetFixAllProvider()
        {
            // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/FixAllProvider.md for more information on Fix All Providers
            return WellKnownFixAllProviders.BatchFixer;
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;


            var datetimeMember =
                root.FindToken(diagnosticSpan.Start).Parent
                    .AncestorsAndSelf().OfType<MemberAccessExpressionSyntax>()
                    .First();

            // Регистрируем действие, которое выполнит нужное преобразование
            var action = CodeAction.Create(title, ct => ChangeNowToUtc(context.Document, datetimeMember, ct), title);

            context.RegisterCodeFix(action, diagnostic);
        }

        private async Task<Document> ChangeNowToUtc(Document document, MemberAccessExpressionSyntax expression, CancellationToken ct)
        {
            //Меняем Now to Utc
            var updatedNewExpression = expression.WithName(SyntaxFactory.IdentifierName("UtcNow"));
            var root = await document.GetSyntaxRootAsync(ct);

            return document.WithSyntaxRoot(root.ReplaceNode(expression, updatedNewExpression));
        }
    }
}