#pragma once

//定义日志等级
#define Fatal	1
#define Error	2
#define Warn	3
#define Info	4
#define Debug	5

class Common
{
public:
	Common();
	~Common();

	void Log(int Level, CString Msg);
	void DoSaveLogs(CString Msg);

	CString Encode(int key, CString str);
	CString Decode(int key, CString str);

	BOOL FullNumAndWord(CString str);

	static Common* GetInstance();
	static Common* instance;
private:
	int m_LogLevel;
	CString m_LogDirPath;
};

