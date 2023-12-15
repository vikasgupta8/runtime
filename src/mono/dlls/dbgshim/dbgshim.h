// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
//*****************************************************************************
// DbgShim.h
//
//*****************************************************************************

#include <mono/utils/mono-publib.h>

#if defined(TARGET_WINDOWS)
#include <windows.h>
#include <libloaderapi.h>
#endif

#include <unknwn.h>

typedef VOID (*PSTARTUP_CALLBACK)(IUnknown *pCordb, PVOID parameter, HRESULT hr);


MONO_API DLLEXPORT HRESULT
CreateProcessForLaunch(
    _In_ LPWSTR lpCommandLine,
    _In_ BOOL bSuspendProcess,
    _In_ LPVOID lpEnvironment,
    _In_ LPCWSTR lpCurrentDirectory,
    _Out_ PDWORD pProcessId,
    _Out_ HANDLE *pResumeHandle);

MONO_API DLLEXPORT HRESULT
ResumeProcess(
    _In_ HANDLE hResumeHandle);

MONO_API DLLEXPORT HRESULT
CloseResumeHandle(
    _In_ HANDLE hResumeHandle);

MONO_API DLLEXPORT HRESULT
RegisterForRuntimeStartup(
    _In_ DWORD dwProcessId,
    _In_ PSTARTUP_CALLBACK pfnCallback,
    _In_ PVOID parameter,
    _Out_ PVOID *ppUnregisterToken);

MONO_API DLLEXPORT HRESULT
RegisterForRuntimeStartupEx(
    _In_ DWORD dwProcessId,
    _In_ LPCWSTR szApplicationGroupId,
    _In_ PSTARTUP_CALLBACK pfnCallback,
    _In_ PVOID parameter,
    _Out_ PVOID *ppUnregisterToken);

MONO_API DLLEXPORT HRESULT
UnregisterForRuntimeStartup(
    _In_ PVOID pUnregisterToken);

MONO_API DLLEXPORT HRESULT
GetStartupNotificationEvent(
    _In_ DWORD debuggeePID,
    _Out_ HANDLE* phStartupEvent);

MONO_API DLLEXPORT HRESULT
EnumerateCLRs(DWORD debuggeePID,
    _Out_ HANDLE** ppHandleArrayOut,
    _Out_ LPWSTR** ppStringArrayOut,
    _Out_ DWORD* pdwArrayLengthOut);

MONO_API DLLEXPORT HRESULT
CloseCLREnumeration(
    _In_ HANDLE* pHandleArray,
    _In_ LPWSTR* pStringArray,
    _In_ DWORD dwArrayLength);

MONO_API DLLEXPORT HRESULT
CreateVersionStringFromModule(
    _In_ DWORD pidDebuggee,
    _In_ LPCWSTR szModuleName,
    _Out_writes_to_opt_(cchBuffer, *pdwLength) LPWSTR pBuffer,
    _In_ DWORD cchBuffer,
    _Out_ DWORD* pdwLength);

MONO_API DLLEXPORT HRESULT
CreateDebuggingInterfaceFromVersionEx(
    _In_ int iDebuggerVersion,
    _In_ LPCWSTR szDebuggeeVersion,
    _Out_ IUnknown ** ppCordb);

MONO_API DLLEXPORT
HRESULT
CreateDebuggingInterfaceFromVersion2(
    _In_ int iDebuggerVersion,
    _In_ LPCWSTR szDebuggeeVersion,
    _In_ LPCWSTR szApplicationGroupId,
    _Out_ IUnknown ** ppCordb);

MONO_API DLLEXPORT HRESULT
CreateDebuggingInterfaceFromVersion(
    _In_ LPCWSTR szDebuggeeVersion,
    _Out_ IUnknown ** ppCordb);
