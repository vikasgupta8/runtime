// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
using System;


namespace System.Reflection.Metadata.ApplyUpdate.Test
{
    public class GenericAddInstanceField<T>
    {
        public GenericAddInstanceField (T p) {
        }

        public T GetIt()
        {
            return default(T);
        }
    }
}
