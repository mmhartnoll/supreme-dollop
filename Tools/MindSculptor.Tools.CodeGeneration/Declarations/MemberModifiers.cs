using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MindSculptor.Tools.CodeGeneration.Declarations
{
    [Flags]
    public enum MemberModifiers
    {
        None        = 0x0,
        Abstract    = 0x1,
        Async       = 0x2,
        Const       = 0x4,
        Event       = 0x8,
        Extern      = 0x10,
        In          = 0x20,
        New         = 0x40,
        Out         = 0x80,
        Override    = 0x100,
        ReadOnly    = 0x200,
        Sealed      = 0x400,
        Static      = 0x800,
        Unsafe      = 0x1000,
        Virtual     = 0x2000,
        Volatile    = 0x4000,
        Default     = None
    }

    public static class MemberModifiersExtensions
    {
        public static IEnumerable<MemberModifiers> Enumerate(this MemberModifiers flags)
        {
            if (flags == MemberModifiers.None)
                yield break;
            foreach (var value in Enum.GetValues(typeof(MemberModifiers)))
            {
                var flag = value as MemberModifiers? ?? throw new Exception($"Object '{nameof(value)}' cannot be convertted to type '{nameof(MemberModifiers)}'.");
                if (flag != MemberModifiers.None && flags.HasFlag(flag))
                {
                    flags &= ~flag;
                    yield return flag;
                }
            }
        }

        public static SyntaxToken[] ToSyntaxTokenArray(this IEnumerable<MemberModifiers> source)
            => source.Select(value => SyntaxFactory.Token(
                value switch
                {
                    MemberModifiers.Abstract  => SyntaxKind.AbstractKeyword,
                    MemberModifiers.Async     => SyntaxKind.AsyncKeyword,
                    MemberModifiers.Const     => SyntaxKind.ConstKeyword,
                    MemberModifiers.Event     => SyntaxKind.EventKeyword,
                    MemberModifiers.Extern    => SyntaxKind.ExternKeyword,
                    MemberModifiers.In        => SyntaxKind.InKeyword,
                    MemberModifiers.New       => SyntaxKind.NewKeyword,
                    MemberModifiers.Out       => SyntaxKind.OutKeyword,
                    MemberModifiers.Override  => SyntaxKind.OverrideKeyword,
                    MemberModifiers.ReadOnly  => SyntaxKind.ReadOnlyKeyword,
                    MemberModifiers.Sealed    => SyntaxKind.SealedKeyword,
                    MemberModifiers.Static    => SyntaxKind.StaticKeyword,
                    MemberModifiers.Unsafe    => SyntaxKind.UnsafeKeyword,
                    MemberModifiers.Virtual   => SyntaxKind.VirtualKeyword,
                    MemberModifiers.Volatile  => SyntaxKind.VolatileKeyword,
                    _ => throw new NotSupportedException($"Cannot convert {nameof(MemberModifiers)} value '{value}' to type of {nameof(SyntaxToken)}.")
                })).ToArray();
    }
}
