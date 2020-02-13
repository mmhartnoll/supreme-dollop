using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MindSculptor.Tools.CodeGeneration.Declarations
{
    public class TypeDeclaration
    {
        public static TypeDeclaration Var => new TypeDeclaration("var", false);

        public string Name { get; }
        public bool IsNullable { get; }

        protected TypeDeclaration(string name, bool isNullable)
        {
            Name = name;
            IsNullable = isNullable;
        }

        public static implicit operator TypeDeclaration(string typeName)
            => new TypeDeclaration(typeName, false);

        public static implicit operator TypeDeclaration(Type type)
            => new TypeDeclaration(FormatTypeName(type), false);

        public static implicit operator TypeSyntax(TypeDeclaration declaration)
        {
            var typeName = declaration.IsNullable ?
                $"{declaration.Name}?" :
                declaration.Name;
            return SyntaxFactory.ParseTypeName(typeName);
        }

        public static TypeDeclaration Create(Type type, bool isNullable = false)
            => new TypeDeclaration(FormatTypeName(type), isNullable);

        private static string FormatTypeName(Type type)
        {
            var nullableUnderlyingType = Nullable.GetUnderlyingType(type);
            if (nullableUnderlyingType != null)
                return $"{FormatTypeName(nullableUnderlyingType)}?";
            return GetFriendlyTypeName(type).Split('.').Last();
        }

        private static string GetFriendlyTypeName(Type type)
            => friendlyNameLookup.ContainsKey(type) ?
                friendlyNameLookup[type] :
                type.ToString();

        private static Dictionary<Type, string> friendlyNameLookup = new Dictionary<Type, string>
        {
            {typeof(bool),      "bool"},
            {typeof(byte),      "byte"},
            {typeof(sbyte),     "sbyte"},
            {typeof(char),      "char"},
            {typeof(decimal),   "decimal"},
            {typeof(double),    "double"},
            {typeof(float),     "float"},
            {typeof(int),       "int"},
            {typeof(uint),      "uint"},
            {typeof(long),      "long"},
            {typeof(ulong),     "ulong"},
            {typeof(object),    "object"},
            {typeof(short),     "short"},
            {typeof(ushort),    "ushort"},
            {typeof(string),    "string"}
        };
    }
}
