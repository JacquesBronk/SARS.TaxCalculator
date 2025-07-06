// This file provides the IsExternalInit type required for C# 9 init-only properties in .NET Standard 2.1
// This is a standard polyfill that allows us to use modern C# features in older target frameworks

namespace System.Runtime.CompilerServices
{
    internal static class IsExternalInit { }
}