#include "stdafx.h"
#include "Common.h"

Common* Common::instance = NULL;
Common* Common::GetInstance()
{
	if (instance == NULL)
	{
		instance = new Common();
	}
	return instance;
}

Common::Common()
{
	m_LogLevel = LOG_Debug;
	m_LogDirPath = ".";
}


Common::~Common()
{
}

void Common::Log(int Level, CString Msg)
{
	if (Level <= m_LogLevel)
	{
		//::SendMessage(Common::DlgLog, WM_LOGSERVER, 0, (LPARAM)(&Msg));

		DoSaveLogs(Msg);
	}
}

void Common::DoSaveLogs(CString Msg)
{
	CString cstemp;
	CString csdisp;
	CString cdtime;
	CString cstfile;
	CTime m_time;

	m_time = CTime::GetCurrentTime();
	cdtime = m_time.Format(_T("[%Y-%m-%d %H:%M:%S] "));
	cdtime += Msg;

	cstfile = m_time.Format(_T("%Y%m%d"));

	FILE *fplog = NULL;
	char buffline[256];

	memset(buffline, 0, 256);
	sprintf_s(buffline, "%s\\Logs\\%s.log", m_LogDirPath, (LPSTR)(LPCTSTR)cstfile);
	fopen_s(&fplog, buffline, "a+");
	if (fplog == NULL){
		//Common::Log(Error, _T("打开日志文件路径写错误!"));
	}
	else{
		fputs((LPSTR)(LPCTSTR)cdtime, fplog);
		fclose(fplog);
	}
}

CString Common::Encode(int key, CString str)
{
	int length = str.GetAllocLength();
	for (int i = 0; i < length; i++)
	{
		//str[i] = str[i] + key;
		//str.SetAt(i, str[i] + key);
	}
	return str;
}
CString Common::Decode(int key, CString str)
{
	for (int i = 0; i < str.GetAllocLength(); i++)
	{
		str.SetAt(i, str[i] - key);
	}
	return str;
}

BOOL  Common::FullNumAndWord(CString str)
{
	for (int i = 0; i < str.GetAllocLength(); i++)
	{
		char ch = str[i];
		if ((ch > 90 && ch < 97) || ch > 122 || (ch < 64 && ch > 57) || ch < 48)
		{
			return FALSE;
		}
	}
	return TRUE;
}
