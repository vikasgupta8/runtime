// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

// Generated by Fuzzlyn v1.5 on 2022-02-06 15:53:33
// Run on Arm64 Windows
// Seed: 7637938960038665944
// Reduced from 168.4 KiB to 0.2 KiB in 00:05:13
// Hits JIT assert in Release:
// Assertion failed '!"Write to unaliased local overlaps outstanding read"' in 'Program:Main(Fuzzlyn.ExecutionServer.IRuntime)' during 'Rationalize IR' (IL size 26)
// 
//     File: D:\a\_work\1\s\src\coreclr\jit\lir.cpp Line: 1397
// 
using System.IO;
using System.Reflection;
using System.Runtime.Loader;

public class Runtime_64883
{
    public static uint s_29;
    public static int Main()
    {
        // This needs an ALC because the "static access" helper is different in ALCs.
        CollectibleALC alc = new CollectibleALC();
        Assembly asm = alc.LoadFromAssemblyPath(Assembly.GetExecutingAssembly().Location);
        MethodInfo mi = asm.GetType(nameof(Runtime_64883)).GetMethod(nameof(MainT));
        mi.Invoke(null, new object[0]);
        return 100;
    }

    public static void MainT()
    {
        long vr7 = 4447329742151181917L;
        vr7 /= (vr7 ^ s_29);
        uint vr6 = s_29;
    }
    
    private class CollectibleALC : AssemblyLoadContext
    {
        public CollectibleALC() : base(true)
        {
        }
    }
}

