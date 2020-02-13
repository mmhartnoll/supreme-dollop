using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MindSculptor.Tools.CodeGeneration.Declarations
{
    [Flags]
    public enum MemberAccessModifiers
    {
        NotSpecified        = 0x0,
        Public              = 0x1,
        Private             = 0x2,
        Protected           = 0x4,
        Internal            = 0x8,
        ProtectedInternal   = Protected | Internal,
        PrivateProtected    = Private | Protected,
        Default             = NotSpecified
    }

    public enum TestEnumeration
    {
        ValueOne,
        ValueTwo
    }

    public static class MemberAccessModifierExtensions
    {
        public static IEnumerable<MemberAccessModifiers> Enumerate(this MemberAccessModifiers flags)
        {
            if (flags == MemberAccessModifiers.NotSpecified)
                yield break;
            foreach (var value in Enum.GetValues(typeof(MemberAccessModifiers)))
            {
                var flag = value as MemberAccessModifiers? ?? throw new Exception($"Object '{nameof(value)}' cannot be convertted to type '{nameof(MemberAccessModifiers)}'.");
                if (flag != MemberAccessModifiers.NotSpecified && flags.HasFlag(flag))
                {
                    flags &= ~flag;
                    yield return flag;
                }
            }
        }

        public static SyntaxToken[] ToSyntaxTokenArray(this IEnumerable<MemberAccessModifiers> source)
            => source.Select(value => SyntaxFactory.Token(
                value switch
                {
                    MemberAccessModifiers.Public => SyntaxKind.PublicKeyword,
                    MemberAccessModifiers.Private => SyntaxKind.PrivateKeyword,
                    MemberAccessModifiers.Protected => SyntaxKind.ProtectedKeyword,
                    MemberAccessModifiers.Internal => SyntaxKind.InternalKeyword,
                    _ => throw new NotSupportedException($"Cannot convert {nameof(MemberAccessModifiers)} value '{value}' to type of {nameof(SyntaxToken)}.")
                })).ToArray();
    }
}
