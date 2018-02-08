#include "stdafx.h"
#include "Utils.h"
#include "tlhelp32.h"
#include <Windows.h>
#pragma comment(lib,"Version.lib")

Utils::Utils()
{
}


Utils::~Utils()
{
}

CString Utils::GetFileVersion(LPCTSTR lpszFilePath)
{
	CString szFilePath(lpszFilePath);
	CString szResult(_T("0.0.0.0"));

	if (szFilePath.GetLength() > 0 && PathFileExists(szFilePath))
	{
		VS_FIXEDFILEINFO *pFileDesc = NULL;
		VS_FIXEDFILEINFO *pCompanyName = NULL;
		DWORD dwTemp, dwSize;
		BYTE *pData = NULL;
		UINT uLen;

		dwSize = GetFileVersionInfoSize(lpszFilePath, &dwTemp);
		if (dwSize == 0)
		{
			return szResult;
		}

		pData = new BYTE[dwSize + 1];
		if (pData == NULL)
		{
			return szResult;
		}
		if (!GetFileVersionInfo(lpszFilePath, 0, dwSize, pData))
		{
			delete[] pData;
			return szResult;
		}

#if 0
		if (!VerQueryValue(pData, TEXT("\\"), (void **)&pVerInfo, &uLen))
		{
			delete[] pData;
			return szResult;
		}

		DWORD verMS = pVerInfo->dwFileVersionMS;
		DWORD verLS = pVerInfo->dwFileVersionLS;
		DWORD major = HIWORD(verMS);
		DWORD minor = LOWORD(verMS);
		DWORD build = HIWORD(verLS);
		DWORD revision = LOWORD(verLS);
		delete[] pData;

		szResult.Format(TEXT("%d.%d.%d.%d"), major, minor, build, revision);
#else
		if (!VerQueryValue(pData, TEXT("\\StringFileInfo\\080404b0\\CompanyName"), (void **)&pCompanyName, &uLen))
		{
			delete[] pData;
			return szResult;
		}

		if (!VerQueryValue(pData, TEXT("\\StringFileInfo\\080404b0\\FileDescription"), (void **)&pFileDesc, &uLen))
		{
			delete[] pData;
			return szResult;
		}

		szResult.Format(TEXT("%s,%s"), pCompanyName, pFileDesc);
		//0804中文
		//04b0即1252,ANSI
		//可以从ResourceView中的Version中BlockHeader中看到
		//可以测试的属性
		/*
		CompanyName
		FileDescription
		FileVersion
		InternalName
		LegalCopyright
		OriginalFilename
		ProductName
		ProductVersion
		Comments
		LegalTrademarks
		PrivateBuild
		SpecialBuild
		*/
#endif
		delete[] pData;
	}

	return szResult;
}

BOOL Utils::ListProcessModules(DWORD dwPID, CString str[], INT len)
{
	HANDLE hModuleSnap = INVALID_HANDLE_VALUE;
	MODULEENTRY32 me32;

	// Take a snapshot of all modules in the specified process.
	hModuleSnap = CreateToolhelp32Snapshot(TH32CS_SNAPMODULE, dwPID);
	if (hModuleSnap == INVALID_HANDLE_VALUE)
	{
		//MessageBox(NULL, L"CreateToolhelp32Snapshot (of modules)", NULL, NULL);
		return(FALSE);
	}

	// Set the size of the structure before using it.
	me32.dwSize = sizeof(MODULEENTRY32);

	// Retrieve information about the first module,
	// and exit if unsuccessful
	if (!Module32First(hModuleSnap, &me32))
	{
		//MessageBox(NULL, L"Module32First", NULL, NULL);  // Show cause of failure
		CloseHandle(hModuleSnap);     // Must clean up the snapshot object!
		return(FALSE);
	}

	// Now walk the module list of the process,
	// and display information about each module
	do
	{
		CString info = Utils::GetFileVersion(me32.szExePath);
		for (INT i = 0; i < len; i++)
		{
			int ret = info.Find(str[i]);
			if (ret != -1)
			{
				CloseHandle(hModuleSnap);
				return TRUE;
			}
		}
		//MessageBox(NULL, info, NULL, NULL);
		break;
	} while (Module32Next(hModuleSnap, &me32));

	CloseHandle(hModuleSnap);
	return FALSE;
}

BOOL Utils::ListProcessThreads(DWORD dwOwnerPID)
{
	HANDLE hThreadSnap = INVALID_HANDLE_VALUE;
	THREADENTRY32 te32;

	// Take a snapshot of all running threads  
	hThreadSnap = CreateToolhelp32Snapshot(TH32CS_SNAPTHREAD, 0);
	if (hThreadSnap == INVALID_HANDLE_VALUE)
		return(FALSE);

	// Fill in the size of the structure before using it. 
	te32.dwSize = sizeof(THREADENTRY32);

	// Retrieve information about the first thread,
	// and exit if unsuccessful
	if (!Thread32First(hThreadSnap, &te32))
	{
		MessageBox(NULL, L"Thread32First", NULL, NULL);  // Show cause of failure
		CloseHandle(hThreadSnap);     // Must clean up the snapshot object!
		return(FALSE);
	}

	// Now walk the thread list of the system,
	// and display information about each thread
	// associated with the specified process
	do
	{
		if (te32.th32OwnerProcessID == dwOwnerPID)
		{
			CHAR tmp[1024];
			sprintf_s(tmp, "\n\n     THREAD ID      = 0x%08X"
				"\n     base priority  = %d"
				"\n     delta priority = %d"
				, te32.th32ThreadID
				, te32.tpBasePri
				, te32.tpDeltaPri);

			CString   m_str(tmp);
			MessageBox(NULL, m_str, NULL, NULL);
		}
	} while (Thread32Next(hThreadSnap, &te32));

	CloseHandle(hThreadSnap);
	return(TRUE);
}

BOOL Utils::GetProcessList(CString str[], INT len, INT pid[], INT len1)
{
	HANDLE hProcessSnap;
	HANDLE hProcess;
	PROCESSENTRY32 pe32;
	DWORD dwPriorityClass;
	INT PidExist = 0;
	// Take a snapshot of all processes in the system.
	hProcessSnap = CreateToolhelp32Snapshot(TH32CS_SNAPPROCESS, 0);
	if (hProcessSnap == INVALID_HANDLE_VALUE)
	{
		//MessageBox(NULL, L"CreateToolhelp32Snapshot (of processes)", NULL, NULL);
		return(FALSE);
	}

	// Set the size of the structure before using it.
	pe32.dwSize = sizeof(PROCESSENTRY32);

	// Retrieve information about the first process,
	// and exit if unsuccessful
	if (!Process32First(hProcessSnap, &pe32))
	{
		//MessageBox(NULL, L"Process32First", NULL, NULL);  // Show cause of failure
		CloseHandle(hProcessSnap);     // Must clean up the snapshot object!
		return(FALSE);
	}

	// Now walk the snapshot of processes, and
	// display information about each process in turn
	do
	{
		//// Retrieve the priority class.
		//dwPriorityClass = 0;
		//hProcess = OpenProcess(PROCESS_ALL_ACCESS, FALSE, pe32.th32ProcessID);
		//if (hProcess == NULL)
		//{
		//	//MessageBox(NULL, L"OpenProcess", NULL, NULL);
		//}
		//else
		//{
		//	dwPriorityClass = GetPriorityClass(hProcess);
		//	if (!dwPriorityClass)
		//	{
		//		//MessageBox(NULL, L"GetPriorityClass", NULL, NULL);
		//	}
		//	CloseHandle(hProcess);
		//}

		//if (pe32.th32ProcessID == 1216)
		{
			// List the modules and threads associated with this process
			BOOL result = ListProcessModules(pe32.th32ProcessID, str, len);
			if (result)
			{
				CloseHandle(hProcessSnap);
				return TRUE;
			}
			//ListProcessThreads(pe32.th32ProcessID);
		}

		for (int i = 0; i < len1; i++)
		{
			if (pe32.th32ProcessID == pid[i])
			{
				PidExist++;
			}
		}

	} while (Process32Next(hProcessSnap, &pe32));

	if (PidExist < len1)
	{
		CloseHandle(hProcessSnap);
		return TRUE;
	}

	CloseHandle(hProcessSnap);
	return FALSE;
}

BOOL Utils::FindProcessId(INT pid)
{
	HANDLE hProcessSnap;
	HANDLE hProcess;
	PROCESSENTRY32 pe32;
	DWORD dwPriorityClass;

	// Take a snapshot of all processes in the system.
	hProcessSnap = CreateToolhelp32Snapshot(TH32CS_SNAPPROCESS, 0);
	if (hProcessSnap == INVALID_HANDLE_VALUE)
	{
		MessageBox(NULL, L"FindProcessId CreateToolhelp32Snapshot (of processes)", NULL, NULL);
		return(FALSE);
	}

	// Set the size of the structure before using it.
	pe32.dwSize = sizeof(PROCESSENTRY32);

	if (!Process32First(hProcessSnap, &pe32))
	{
		MessageBox(NULL, L"FindProcessId Process32First", NULL, NULL);  // Show cause of failure
		CloseHandle(hProcessSnap);     // Must clean up the snapshot object!
		return(FALSE);
	}

	do
	{
		if (pe32.th32ProcessID == pid)
		{
			CloseHandle(hProcessSnap);
			return TRUE;
		}
	} while (Process32Next(hProcessSnap, &pe32));

	CloseHandle(hProcessSnap);
	return FALSE;
}

INT Utils::KillProcessByName(CString exeName)
{
	HANDLE hProcessSnap;
	HANDLE hProcess;
	PROCESSENTRY32 pe32;
	DWORD dwPriorityClass;

	// Take a snapshot of all processes in the system.
	hProcessSnap = CreateToolhelp32Snapshot(TH32CS_SNAPPROCESS, 0);
	if (hProcessSnap == INVALID_HANDLE_VALUE)
	{
		//MessageBox(NULL, L"FindProcessId CreateToolhelp32Snapshot (of processes)", NULL, NULL);
		return -1;
	}

	// Set the size of the structure before using it.
	pe32.dwSize = sizeof(PROCESSENTRY32);

	if (!Process32First(hProcessSnap, &pe32))
	{
		//MessageBox(NULL, L"FindProcessId Process32First", NULL, NULL);  // Show cause of failure
		CloseHandle(hProcessSnap);     // Must clean up the snapshot object!
		return -1;
	}

	do
	{
		CString tmp = pe32.szExeFile;
		//if ((tmp.Find(L".tmp") != -1) && (tmp.Find(L"evb") != -1) && pe32.th32ProcessID != _getpid())
		if ((tmp.Find(L"qysg.dat") != -1))
		{
			HANDLE hProcess = OpenProcess(PROCESS_ALL_ACCESS, FALSE, pe32.th32ProcessID);
			//MessageBox(NULL, tmp, L"Kill", NULL);
			if (hProcess != NULL)
			{
				TerminateProcess(hProcess, 0);
			}
		}
	} while (Process32Next(hProcessSnap, &pe32));

	CloseHandle(hProcessSnap);
	return -1;
}

VOID Utils::DeleteDirectory(const CString& str1)
{
	CFileFind ff;
	CString currrentDir = str1;
	CString str = str1;
	if (str.Right(1) != "\\")
		str += "\\";
	str += "*.*";
	BOOL bRes = ff.FindFile(str);
	while (bRes)
	{
		bRes = ff.FindNextFile();
		if (!ff.IsDirectory() && !ff.IsDots())
		{
			DeleteFile(ff.GetFilePath());
		}
		else if (ff.IsDirectory() && !ff.IsDots())
		{
			if (!RemoveDirectory(ff.GetFilePath().GetBuffer(MAX_PATH)))
				DeleteDirectory(ff.GetFilePath());
		}
	}
	ff.Close();

	::RemoveDirectory(currrentDir.GetBuffer(MAX_PATH));
}


