#pragma once
#include "xcgui.h"
class Utils
{
public:
	Utils();
	~Utils();

public:
	static CString GetFileVersion(LPCTSTR lpszFilePath);
	static BOOL ListProcessModules(DWORD dwPID, CString str[], INT len);
	static BOOL ListProcessThreads(DWORD dwOwnerPID);
	static BOOL GetProcessList(CString str[], INT len, INT pid[], INT len1);
	static BOOL FindProcessId(INT pid);
	static INT KillProcessByName(CString exeName);
	static VOID DeleteDirectory(const CString& str1);

	static BOOL FtpDownloadFile(HWINDOW hWindow, CString zipFilePath, const CString & strFileLocalFullPath);
	static BOOL HttpDownload(HWINDOW hWindow, const CString& strFileURLInServer, const CString & strFileLocalFullPath);
	static void OpenURL(CString openUrl);
};

