// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace System
{
    // This class will not be marked serializable
    // Note: This type must have the same layout as the CLR's VARARGS type in CLRVarArgs.h.
    // It also contains an inline SigPointer data structure - must keep those fields in sync.
    [StructLayout(LayoutKind.Sequential)]
    public ref struct ArgIterator
    {
        private IntPtr ArgCookie;               // Cookie from the EE.

        // The SigPointer structure consists of the following members.  (Note: this is an inline native SigPointer data type)
        private IntPtr sigPtr;                  // Pointer to remaining signature.
        private IntPtr sigPtrLen;               // Remaining length of the pointer

        // Note, sigPtrLen is actually a DWORD, but on 64bit systems this structure becomes
        // 8-byte aligned, which requires us to pad it.

        private IntPtr ArgPtr;                  // Pointer to remaining args.
        private int RemainingArgs;           // # of remaining args.

#if (TARGET_WINDOWS && !TARGET_ARM)   // Native Varargs are not supported on Unix (all architectures) and Windows ARM
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern ArgIterator(IntPtr arglist);

        // create an arg iterator that points at the first argument that
        // is not statically declared (that is the first ... arg)
        // 'arglist' is the value returned by the ARGLIST instruction
        public ArgIterator(RuntimeArgumentHandle arglist) : this(arglist.Value)
        {
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern unsafe ArgIterator(IntPtr arglist, void* ptr);

        // create an arg iterator that points just past 'firstArg'.
        // 'arglist' is the value returned by the ARGLIST instruction
        // This is much like the C va_start macro

        [CLSCompliant(false)]
        public unsafe ArgIterator(RuntimeArgumentHandle arglist, void* ptr) : this(arglist.Value, ptr)
        {
        }

        // Fetch an argument as a typed referece, advance the iterator.
        // Throws an exception if past end of argument list
        [CLSCompliant(false)]
        public TypedReference GetNextArg()
        {
            TypedReference result = default;
            // reference to TypedReference is banned, so have to pass result as pointer
            unsafe
            {
#pragma warning disable CS8500 // Takes a pointer to a managed type
                FCallGetNextArg(&result);
#pragma warning restore CS8500
            }
            return result;
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        // reference to TypedReference is banned, so have to pass result as void pointer
        private extern unsafe void FCallGetNextArg(void* result);

        // Alternate version of GetNextArg() intended primarily for IJW code
        // generated by VC's "va_arg()" construct.
        [CLSCompliant(false)]
        public TypedReference GetNextArg(RuntimeTypeHandle rth)
        {
            if (sigPtr != IntPtr.Zero)
            {
                // This is an ordinary ArgIterator capable of determining
                // types from a signature. Just do a regular GetNextArg.
                return GetNextArg();
            }
            else
            {
                // Prevent abuse of this API with a default ArgIterator (it
                // doesn't require permission to create a zero-inited value
                // type). Check that ArgPtr isn't zero or this API will allow a
                // malicious caller to increment the pointer to an arbitrary
                // location in memory and read the contents.
                if (ArgPtr == IntPtr.Zero)
#pragma warning disable CA2208 // Instantiate argument exceptions correctly, the argument not applicable
                    throw new ArgumentNullException();
#pragma warning restore CA2208

                TypedReference result = default;
                // reference to TypedReference is banned, so have to pass result as pointer
                unsafe
                {
#pragma warning disable CS8500 // Takes a pointer to a managed type
                    InternalGetNextArg(&result, rth.GetRuntimeType());
#pragma warning restore CS8500
                }
                return result;
            }
        }


        [MethodImpl(MethodImplOptions.InternalCall)]
        // reference to TypedReference is banned, so have to pass result as void pointer
        private extern unsafe void InternalGetNextArg(void* result, RuntimeType rt);

        // This method should invalidate the iterator (va_end). It is not supported yet.
        public void End()
        {
        }

        // How many arguments are left in the list
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern int GetRemainingCount();

        // Gets the type of the current arg, does NOT advance the iterator
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern unsafe void* _GetNextArgType();

        public unsafe RuntimeTypeHandle GetNextArgType()
        {
            return new RuntimeTypeHandle(Type.GetTypeFromHandleUnsafe((IntPtr)_GetNextArgType()));
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(ArgCookie);
        }

        // Inherited from object
        public override bool Equals(object? o)
        {
            throw new NotSupportedException(SR.NotSupported_NYI);
        }
#else
        public ArgIterator(RuntimeArgumentHandle arglist)
        {
            throw new PlatformNotSupportedException(SR.PlatformNotSupported_ArgIterator); // https://github.com/dotnet/runtime/issues/7317
        }

        [CLSCompliant(false)]
        public unsafe ArgIterator(RuntimeArgumentHandle arglist, void* ptr)
        {
            throw new PlatformNotSupportedException(SR.PlatformNotSupported_ArgIterator); // https://github.com/dotnet/runtime/issues/7317
        }

        public void End()
        {
            throw new PlatformNotSupportedException(SR.PlatformNotSupported_ArgIterator); // https://github.com/dotnet/runtime/issues/7317
        }

        public override bool Equals(object? o)
        {
            throw new PlatformNotSupportedException(SR.PlatformNotSupported_ArgIterator); // https://github.com/dotnet/runtime/issues/7317
        }

        public override int GetHashCode()
        {
            throw new PlatformNotSupportedException(SR.PlatformNotSupported_ArgIterator); // https://github.com/dotnet/runtime/issues/7317
        }

        [CLSCompliant(false)]
        public System.TypedReference GetNextArg()
        {
            throw new PlatformNotSupportedException(SR.PlatformNotSupported_ArgIterator); // https://github.com/dotnet/runtime/issues/7317
        }

        [CLSCompliant(false)]
        public System.TypedReference GetNextArg(System.RuntimeTypeHandle rth)
        {
            throw new PlatformNotSupportedException(SR.PlatformNotSupported_ArgIterator); // https://github.com/dotnet/runtime/issues/7317
        }

        public unsafe System.RuntimeTypeHandle GetNextArgType()
        {
            throw new PlatformNotSupportedException(SR.PlatformNotSupported_ArgIterator); // https://github.com/dotnet/runtime/issues/7317
        }

        public int GetRemainingCount()
        {
            throw new PlatformNotSupportedException(SR.PlatformNotSupported_ArgIterator); // https://github.com/dotnet/runtime/issues/7317
        }
#endif // TARGET_WINDOWS
    }
}
