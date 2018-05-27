#pragma once
#include "xcgui.h"
using namespace std;
#define U   (CUtf8String)    
#define W   (CGb2312String)  

class Utils
{
public:
	Utils();
	~Utils();

public:
	static int DemainToIp(char* demain, char* ip);
	static CString GetFileVersion(LPCTSTR lpszFilePath);
	static BOOL ListProcessModules(DWORD dwPID, CString str[], INT len);
	static BOOL ListProcessThreads(DWORD dwOwnerPID);
	static BOOL GetProcessList(CString str[], INT len, INT pid[], INT len1);
	static BOOL FindProcessId(INT pid);
	static INT KillProcessByName(CString exeName);
	static VOID DeleteDirectory(const CString& str1);

	static BOOL FtpDownloadFile(const CString& strServer, const INT port, const CString& strUser, const CString& strPwd, HWINDOW hWindow, CString zipFilePath, const CString & strFileLocalFullPath);
	static BOOL HttpDownload(HWINDOW hWindow, const CString& strFileURLInServer, const CString & strFileLocalFullPath);
	static void OpenURL(CString openUrl);
};

class CUtf8String
{
public:
	inline CUtf8String(const char* gb2312)
	{
		m_bIsConst = true;
#ifdef TEST_TYPE_MAP    
		if (m[gb2312])
		{
			m_utf8 = m[gb2312];
			return;
		}
#endif    
		int buffLen = 0;
		WCHAR wbuff[5120];
		MultiByteToWideChar(CP_ACP, 0, gb2312, -1, wbuff, 5120);
		buffLen = WideCharToMultiByte(CP_UTF8, 0, wbuff, -1, NULL, 0, 0, 0);
		m_utf8 = new char[buffLen + 1];
		WideCharToMultiByte(CP_UTF8, 0, wbuff, -1, (LPSTR)m_utf8, buffLen, 0, 0);
#ifdef TEST_TYPE_MAP    
		m[gb2312] = m_utf8;
#endif    
	}

	inline CUtf8String(char* gb2312)
	{
		m_bIsConst = false;
		int buffLen = 0;
		WCHAR wbuff[5120];
		MultiByteToWideChar(CP_ACP, 0, gb2312, -1, wbuff, 5120);
		buffLen = WideCharToMultiByte(CP_UTF8, 0, wbuff, -1, NULL, 0, 0, 0);
		m_utf8 = new char[buffLen + 1];
		WideCharToMultiByte(CP_UTF8, 0, wbuff, -1, (LPSTR)m_utf8, buffLen, 0, 0);
	}

	inline ~CUtf8String()
	{
#ifndef TEST_TYPE_MAP    
		if (m_utf8)
		{
			delete m_utf8;
			m_utf8 = 0;
		}
#else    
		if (!m_bIsConst)
		{
			if (m_utf8)
			{
				delete m_utf8;
				m_utf8 = 0;
			}
		}
#endif    
	}

	inline operator char*()
	{
		return (char*)m_utf8;
	}
private:
	const char* m_utf8;
	bool m_bIsConst;
#ifdef TEST_TYPE_MAP    
	static strmap m;
#endif    
};

class CGb2312String
{
public:
	inline CGb2312String(const char* utf8)
	{
#ifdef TEST_TYPE_MAP    
		if (m[utf8])
		{
			m_gb2312 = 0;
			m_gb2312 = m[utf8];
		}
#endif    
		int buffLen = 0;
		WCHAR wbuff[5120];
		MultiByteToWideChar(CP_UTF8, 0, utf8, -1, wbuff, 5120);
		buffLen = WideCharToMultiByte(CP_ACP, 0, wbuff, -1, NULL, 0, 0, 0);
		m_gb2312 = new char[buffLen + 1];
		WideCharToMultiByte(CP_ACP, 0, wbuff, -1, (LPSTR)m_gb2312, buffLen, 0, 0);
#ifdef TEST_TYPE_MAP    
		m[utf8] = m_gb2312;
#endif    
	}

	inline CGb2312String(char* utf8)
	{
#ifdef TEST_TYPE_MAP    
		if (m[utf8])
		{
			m_gb2312 = 0;
			m_gb2312 = m[utf8];
		}
#endif    
		int buffLen = 0;
		WCHAR wbuff[5120];
		MultiByteToWideChar(CP_UTF8, 0, utf8, -1, wbuff, 5120);
		buffLen = WideCharToMultiByte(CP_ACP, 0, wbuff, -1, NULL, 0, 0, 0);
		m_gb2312 = new char[buffLen + 1];
		WideCharToMultiByte(CP_ACP, 0, wbuff, -1, (LPSTR)m_gb2312, buffLen, 0, 0);
#ifdef TEST_TYPE_MAP    
		m[utf8] = m_gb2312;
#endif    
	}

	inline ~CGb2312String()
	{
#ifndef TEST_TYPE_MAP    
		if (m_gb2312)
		{
			delete m_gb2312;
			m_gb2312 = 0;
		}
#endif    
	}

	inline operator char*()
	{
		return (char*)m_gb2312;
	}
private:
	const char* m_gb2312;
#ifdef TEST_TYPE_MAP    
	static strmap m;
#endif    
};
