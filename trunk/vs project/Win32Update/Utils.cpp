#include "stdafx.h"
#include "Utils.h"
#include "tlhelp32.h"
#include <Windows.h>
#include "WinInet.h"  
#include "unzip.h" 
#include "afxinet.h" 
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
		MessageBox(NULL, "Thread32First", NULL, NULL);  // Show cause of failure
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
	//HANDLE hProcess;
	PROCESSENTRY32 pe32;
	//DWORD dwPriorityClass;
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
	//HANDLE hProcess;
	PROCESSENTRY32 pe32;
	//DWORD dwPriorityClass;

	// Take a snapshot of all processes in the system.
	hProcessSnap = CreateToolhelp32Snapshot(TH32CS_SNAPPROCESS, 0);
	if (hProcessSnap == INVALID_HANDLE_VALUE)
	{
		MessageBox(NULL, "FindProcessId CreateToolhelp32Snapshot (of processes)", NULL, NULL);
		return(FALSE);
	}

	// Set the size of the structure before using it.
	pe32.dwSize = sizeof(PROCESSENTRY32);

	if (!Process32First(hProcessSnap, &pe32))
	{
		MessageBox(NULL, "FindProcessId Process32First", NULL, NULL);  // Show cause of failure
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
	//HANDLE hProcess;
	PROCESSENTRY32 pe32;
	//DWORD dwPriorityClass;

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
		if ((tmp.Find(exeName) != -1))
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

BOOL Utils::FtpDownloadFile(HWINDOW hWindow, CString zipFilePath, LONGLONG UpdateLen)
{
	float percent = 0;
	try   {
		CInternetSession *pSession = NULL;
		pSession = new CInternetSession("111", 1, PRE_CONFIG_INTERNET_ACCESS);
		CFtpConnection *pFtpCon = NULL;
		pFtpCon = pSession->GetFtpConnection("192.168.1.100", "FtpUser", "jun384668960", 21);
		//pFtpCon = pSession->GetFtpConnection("120.77.147.184", "FtpUser", "jun384668960", 21217);
		//pFtpCon = pSession->GetFtpConnection("120.77.147.184", "FtpUser", "Donyj_ljj@163.com", 21217);
		//DELETE zipFilePath

		CFile file;
		//CString pDirName;
		if (!file.Open(zipFilePath, CFile::modeCreate | CFile::modeWrite))
		{
			return false;
		}
		//pFtpCon->GetFile(zipFilePath, zipFilePath);
		//CFtpFileFind fileFind(pFtpCon);
		//BOOL bSucc = fileFind.FindFile(zipFilePath);
		//if (bSucc)
		//{
		//	//printf_s("文件大小=%d\n", fileFind.GetLength());
		//	//UpdateLen = fileFind.GetLength();
		//}

		CInternetFile* pFile = pFtpCon->OpenFile(zipFilePath, GENERIC_READ);//GENERIC_WRITE
		CHAR buff[2048] = { 0 };
		INT readCount;
		LONGLONG total = 0;
		int pos = 0;
		readCount = pFile->Read(buff, 2048);
		while (readCount > 0)
		{
			file.Write(buff, readCount);
			total += readCount;
			int _pos = (total * 100) / UpdateLen;
			//发送信号 更新进度条
			//::SendMessage(XWnd_GetHWND(hWindow), WM_RGSMSG, pos, NULL);
			if (_pos != pos)
			{
				pos = _pos;
				::SendMessage(XWnd_GetHWND(hWindow), WM_RGSMSG, NULL, pos);
				//::PostMessage(XWnd_GetHWND(hWindow), WM_RGSMSG, NULL, pos);
			}

			readCount = pFile->Read(buff, 2048);
		}
		pFile->Close();
		file.Close();

		return true;
	}
	catch (CException&)
	{
		return false;
	}
}

BOOL Utils::HttpDownload(HWINDOW hWindow, const CString& strFileURLInServer, const CString & strFileLocalFullPath)
{
	ASSERT(strFileURLInServer != "");
	ASSERT(strFileLocalFullPath != "");
	CInternetSession session("111", 1, PRE_CONFIG_INTERNET_ACCESS);
/*	CInternetSession *pSession = NULL;
	pSession = new CInternetSession("111", 1, PRE_CONFIG_INTERNET_ACCESS);*/
	CHttpConnection* pHttpConnection = NULL;
	CHttpFile* pHttpFile = NULL;
	CString strServer, strObject;
	INTERNET_PORT wPort;
	DWORD dwType;
	const int nTimeOut = 2000;
	session.SetOption(INTERNET_OPTION_CONNECT_TIMEOUT, nTimeOut); //重试之间的等待延时
	session.SetOption(INTERNET_OPTION_CONNECT_RETRIES, 1);   //重试次数
	char* pszBuffer = NULL;
	try
	{
		AfxParseURL(strFileURLInServer, dwType, strServer, strObject, wPort);
		pHttpConnection = session.GetHttpConnection(strServer, wPort,"jun384668960","5a588a");
		//pHttpConnection = session.GetHttpConnection(strServer, wPort);
		pHttpFile = pHttpConnection->OpenRequest(CHttpConnection::HTTP_VERB_GET, strObject);
		if (pHttpFile->SendRequest() == FALSE)
			return false;
		DWORD dwStateCode;
		pHttpFile->QueryInfoStatusCode(dwStateCode);
		if (dwStateCode == HTTP_STATUS_OK)
		{
			HANDLE hFile = CreateFile(strFileLocalFullPath, GENERIC_WRITE,
				FILE_SHARE_WRITE, NULL, CREATE_ALWAYS, FILE_ATTRIBUTE_NORMAL,
				NULL);  //创建本地文件
			if (hFile == INVALID_HANDLE_VALUE)
			{
				pHttpFile->Close();
				pHttpConnection->Close();
				session.Close();
				return false;
			}

			char szInfoBuffer[1000];  //返回消息
			DWORD dwFileSize = 0;   //文件长度
			DWORD dwInfoBufferSize = sizeof(szInfoBuffer);
			BOOL bResult = FALSE;
			bResult = pHttpFile->QueryInfo(HTTP_QUERY_CONTENT_LENGTH,
				(void*)szInfoBuffer, &dwInfoBufferSize, NULL);
			dwFileSize = atoi(szInfoBuffer);
			const int BUFFER_LENGTH = 1024 * 10;
			pszBuffer = new char[BUFFER_LENGTH];  //读取文件的缓冲
			DWORD dwWrite, dwTotalWrite;
			dwWrite = dwTotalWrite = 0;
			UINT nRead = pHttpFile->Read(pszBuffer, BUFFER_LENGTH); //读取服务器上数据
			int pos = 0;
			while (nRead > 0)
			{
				WriteFile(hFile, pszBuffer, nRead, &dwWrite, NULL);  //写到本地文件
				dwTotalWrite += dwWrite;
				int _pos = (dwTotalWrite * 100) / dwFileSize;
				//发送信号 更新进度条
				if (_pos != pos)
				{
					pos = _pos;
					//::SendMessage(XWnd_GetHWND(hWindow), WM_RGSMSG, NULL, pos);
					::PostMessage(XWnd_GetHWND(hWindow), WM_RGSMSG, NULL, pos);
				}
				nRead = pHttpFile->Read(pszBuffer, BUFFER_LENGTH);
			}
			delete[]pszBuffer;
			pszBuffer = NULL;
			CloseHandle(hFile);
		}
		else
		{
			delete[]pszBuffer;
			pszBuffer = NULL;
			if (pHttpFile != NULL)
			{
				pHttpFile->Close();
				delete pHttpFile;
				pHttpFile = NULL;
			}
			if (pHttpConnection != NULL)
			{
				pHttpConnection->Close();
				delete pHttpConnection;
				pHttpConnection = NULL;
			}
			session.Close();
			return false;
		}
	}
	catch (...)
	{
		delete[]pszBuffer;
		pszBuffer = NULL;
		if (pHttpFile != NULL)
		{
			pHttpFile->Close();
			delete pHttpFile;
			pHttpFile = NULL;
		}
		if (pHttpConnection != NULL)
		{
			pHttpConnection->Close();
			delete pHttpConnection;
			pHttpConnection = NULL;
		}
		session.Close();
		return false;
	}
	if (pHttpFile != NULL)
		pHttpFile->Close();
	if (pHttpConnection != NULL)
		pHttpConnection->Close();
	session.Close();
	return true;
}


void Utils::OpenURL(CString openUrl)
{
	if (openUrl == "")
		return;
	HKEY  hkRoot, hSubKey;   //注册表根关键字及子关键字  
	CString ValueName;
	unsigned char DataValue[MAX_PATH];
	unsigned long cbValueNAME = MAX_PATH;
	unsigned long cbDataNAME = MAX_PATH;
	CString ShellChar;//定义命令行  
	DWORD  dwType;
	//打开注册表根关键字  
	if (RegOpenKey(HKEY_CLASSES_ROOT, NULL, &hkRoot) == ERROR_SUCCESS)
	{
		//打开子关键字  
		if (RegOpenKeyEx(hkRoot, "htmlfile\\Shell\\open\\command",
			0, KEY_ALL_ACCESS, &hSubKey) == ERROR_SUCCESS)
		{
			RegEnumValue(hSubKey, 0, (LPSTR)ValueName.GetBuffer(MAX_PATH), &cbValueNAME, NULL,
				&dwType, DataValue, &cbDataNAME);
			ValueName.ReleaseBuffer();
			//调用参数赋值  
			ShellChar = (char *)DataValue;
			if (ShellChar == "\"")
			{
				ShellExecute(NULL, "open", (LPSTR)openUrl.GetBuffer(MAX_PATH), NULL, NULL, SW_SHOWNORMAL);
				openUrl.ReleaseBuffer();
			}
			else
			{
				ShellChar = ShellChar + openUrl;
				WinExec((LPCSTR)ShellChar.GetBuffer(MAX_PATH), SW_SHOW);
				ShellChar.ReleaseBuffer();
			}
		}
		else
		{
			RegCloseKey(hSubKey);
			RegCloseKey(hkRoot);
			ShellExecute(NULL, "open", (LPSTR)openUrl.GetBuffer(MAX_PATH), NULL, NULL, SW_SHOWNORMAL);
			openUrl.ReleaseBuffer();
		}
	}
}
