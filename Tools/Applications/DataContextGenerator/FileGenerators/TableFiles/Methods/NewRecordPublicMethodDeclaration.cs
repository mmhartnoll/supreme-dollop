﻿using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MindSculptor.DataAccess.Modelled.Records;
using MindSculptor.DataAccess.Modelled.Records.Fields;
using MindSculptor.Tools.Applications.DataContextGenerator.Extensions;
using MindSculptor.Tools.CodeGeneration.Declarations;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MindSculptor.Tools.Applications.DataContextGenerator.FileGenerators.TableFiles.Methods
{
    internal class NewRecordPublicMethodDeclaration : MethodDeclaration
    {
        private readonly RecordDefinition recordDefinition;

        private NewRecordPublicMethodDeclaration(RecordDefinition recordDefinition) 
            : base(recordDefinition.RecordName, "NewRecord", MemberAccessModifiers.Public)
        {
            this.recordDefinition = recordDefinition;

            foreach (var fieldDefinition in recordDefinition.Fields)
            {
                if (!(fieldDefinition is IdField idField) || !idField.IsAutoGenerated)
                {
                    var parameterName = fieldDefinition.Name.FormatAsVariableName();
                    AddParameter(TypeDeclaration.Create(fieldDefinition.MappedDalType, fieldDefinition.IsNullable), parameterName);
                }
            }
        }

        public static NewRecordPublicMethodDeclaration Create(RecordDefinition recordDefinition)
            => new NewRecordPublicMethodDeclaration(recordDefinition);

        protected override IEnumerable<StatementSyntax> GetMethodStatementSyntaxes()
        {
            var argumentList = new List<ArgumentSyntax>();
            argumentList.Add(SyntaxFactory.Argument(SyntaxFactory.IdentifierName("command")));
            foreach (var fieldDefinition in recordDefinition.Fields)
            {
                if (fieldDefinition is IdField idField && idField.IsAutoGenerated)
                {
                    var guidInvocationExpression = SyntaxFactory.InvocationExpression(SyntaxFactory.IdentifierName("Guid.NewGuid"));
                    argumentList.Add(SyntaxFactory.Argument(guidInvocationExpression));
                }
                else
                {
                    var argumentName = fieldDefinition.Name.FormatAsVariableName();
                    argumentList.Add(SyntaxFactory.Argument(SyntaxFactory.IdentifierName(argumentName)));
                }
            }

            var newRecordInvocationExpression = SyntaxFactory.InvocationExpression(SyntaxFactory.IdentifierName("NewRecord"))
                .AddArgumentListArguments(argumentList.ToArray());

            var lambdaExpression = SyntaxFactory.SimpleLambdaExpression(
                SyntaxFactory.Parameter(SyntaxFactory.Identifier("command")), 
                newRecordInvocationExpression);

            var executeInvocationExpression = SyntaxFactory.InvocationExpression(SyntaxFactory.IdentifierName("DatabaseContext.Execute"))
                .AddArgumentListArguments(SyntaxFactory.Argument(lambdaExpression));

            yield return SyntaxFactory.ReturnStatement(executeInvocationExpression);
        }
    }
}
