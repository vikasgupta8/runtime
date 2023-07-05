// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class Version
    {
        [LibraryImport(Libraries.Version, EntryPoint = "VerQueryValueW", StringMarshalling = StringMarshalling.Utf16)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static unsafe partial bool VerQueryValue(void* pBlock, string lpSubBlock, out void* lplpBuffer, out uint puLen);
    }
}
