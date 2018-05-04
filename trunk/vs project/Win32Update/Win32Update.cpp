// Win32Update.cpp : 定义应用程序的入口点。
//

#include "stdafx.h"
#include "Win32Update.h"
#include "Common.h"
#include "Utils.h"
#include "tlhelp32.h"
#include "ZipFunction.h"

using namespace ZipUtils;
//包含界面库
#include "xcgui.h"
#pragma comment(lib,"XCGUI.lib")

HWINDOW hWindow = NULL;
HWINDOW hWindowRegit = NULL;
HELE    m_hProgressBar = NULL;
HXCGUI  m_TxtUpdate;

#define MAX_LOADSTRING 100

#include <WinSock2.h>
SOCKET sClient;
HELE hRichEditRegitAccount;
HELE hRichEditRegitPasswd;
HELE hRichEditRegitPasswd2;
HELE hRichEditRegitCode;
HXCGUI hPicCode;
CString m_bitmapCode;
HELE hRichEdit;

// 全局变量: 

HINSTANCE hInst;								// 当前实例
TCHAR szTitle[MAX_LOADSTRING];					// 标题栏文本
TCHAR szWindowClass[MAX_LOADSTRING];			// 主窗口类名

CString StartFile;
CString ClntSize;

// 此代码模块中包含的函数的前向声明: 
ATOM				MyRegisterClass(HINSTANCE hInstance);
BOOL				InitInstance(HINSTANCE, int);
LRESULT CALLBACK	WndProc(HWND, UINT, WPARAM, LPARAM);
INT_PTR CALLBACK	About(HWND, UINT, WPARAM, LPARAM);

int CALLBACK OnProgressBarChange(BOOL *pbHandled, int pos)
{
	//*pbHandled = TRUE;

	XProgBar_SetPos(m_hProgressBar, pos);
	XEle_RedrawEle(m_hProgressBar);

	CString p = "";
	p.Format(TEXT("更新中...%d"), pos);

	USES_CONVERSION;
	wchar_t* pWchar = A2W(p);
	XShapeText_SetText(m_TxtUpdate, pWchar);

	return 0;
}

int AfterUpdateDone()
{
	try{
		//关闭当前客户端
		Utils::KillProcessByName(StartFile);
		Utils::KillProcessByName("Win32Protect");
		Utils::KillProcessByName("情义登录器");
		//删除上述文件
		if (PathFileExists(StartFile) && !DeleteFile(StartFile))
		{
			MessageBox(XWnd_GetHWND(hWindow), "游戏文件:qysg.dat 正在被使用，请关闭后重试！", "提示", MB_OK);
			return -1;
		}
		/*if (PathFileExists("Win32Protect.exe") && !DeleteFile("Win32Protect.exe"))
		{
		MessageBox(XWnd_GetHWND(hWindow), "游戏文件:Win32Protect 正在被使用，请关闭后重试！", "提示", MB_OK);
		return -1;
		}*/
		if (PathFileExists("情义登录器.exe") && !DeleteFile("情义登录器.exe"))
		{
			MessageBox(XWnd_GetHWND(hWindow), "游戏文件:情义登录器 正在被使用，请关闭后重试！", "提示", MB_OK);
			return -1;
		}

		//zip解压
		TCHAR szPath[MAX_PATH];
		GetCurrentDirectory(MAX_PATH, (LPTSTR)&szPath);
		CString Path;
		Path.Format("%s\\", szPath);
		CStringArray name;
		ZRESULT result = ZipUtils::ExtractZipToDir("Update.zip", name, Path);
		if (result != ZR_OK)
		{
			MessageBox(XWnd_GetHWND(hWindow), "升级包Update.zip解压失败，您后选择手动解压！", "提示", MB_OK);
			return -1;
		}
		Sleep(2000);
		//更新成功开启登录器
		STARTUPINFO si = { sizeof(si) };
		PROCESS_INFORMATION pi;
		if (!CreateProcess("情义登录器.exe", NULL, NULL, NULL, FALSE, 0, NULL, NULL, &si, &pi))
		{
			MessageBox(XWnd_GetHWND(hWindow), "启动登录器失败！", "提示", MB_OK);
			::SendMessage(XWnd_GetHWND(hWindow), WM_CLOSE, NULL, NULL);
			return -1;
		}
	}
	catch (CException& w)
	{
	}
	::SendMessage(XWnd_GetHWND(hWindow), WM_CLOSE, NULL, NULL);
	return 0;
}

int CALLBACK Link_EventBtnClick(BOOL *pbHandled)
{
	*pbHandled = TRUE;
	BOOL done = FALSE;
	if (PathFileExists(StartFile) && PathFileExists("情义登录器.exe"))
	{
		//Utils::OpenURL(" http://code.taobao.org/svn/Third-Part-Learning/trunk/Update.zip");
		done = Utils::FtpDownloadFile(hWindow, "Update.zip", "Update.zip");
	}
	else
	{
		//Utils::OpenURL(" http://code.taobao.org/svn/Third-Part-Learning/trunk/FullClient.zip");
		done = Utils::FtpDownloadFile(hWindow, "FullClient.zip", "Update.zip");
	}
	
	if (done)
		AfterUpdateDone();

	return 0;
}

int CALLBACK WinClose_EventBtnClick(BOOL *pbHandled)
{
	*pbHandled = TRUE;
	//INT PID = Utils::KillProcessByName(StartFile);

	::SendMessage(XWnd_GetHWND(hWindow), WM_CLOSE, NULL, NULL);
	return 0;
}

int CALLBACK WinMin_EventBtnClick(BOOL *pbHandled)
{
	XWnd_ShowWindow(hWindow, SW_MINIMIZE);
	*pbHandled = TRUE;
	return 0;
}

BOOL g_bLimitKeyPress = true;

DWORD WINAPI ThreadProc(LPVOID lpParam)
{
	return -1;
	try
	{
		//ftp下载
		bool ret = false;
		//LONGLONG size = atoll(ClntSize.GetBuffer());
		//Utils::FtpDownloadFile(hWindow, "Update.zip", /*1024 * 1024 * 103*/size);
		//http下载
		bool download = false;
		for (int i = 0; i < 5; i++)
		{
			if (PathFileExists(StartFile) && PathFileExists("情义登录器.exe"))
			{
				//ret = Utils::HttpDownload(hWindow, "http://code.taobao.org/svn/Third-Part-Learning/trunk/Update.zip", "Update.zip");
				ret = Utils::FtpDownloadFile(hWindow, "Update.zip", "Update.zip");
			}
			else
			{
				//ret = Utils::HttpDownload(hWindow, "http://code.taobao.org/svn/Third-Part-Learning/trunk/FullClient.zip", "Update.zip");
				ret = Utils::FtpDownloadFile(hWindow, "FullClient.zip", "Update.zip");
			}
			if (!ret)
			{
				Sleep(1000);
				download = false;
			}
			else
			{
				download = true;
				break;
			}
		}
		if (!download)
		{
			MessageBox(XWnd_GetHWND(hWindow), "下载更新失败，请重试或者手动下载！", "提示", MB_OK);
			return -1;
		}
		
		AfterUpdateDone();
	}
	catch (CException& w)
	{
	}
	::SendMessage(XWnd_GetHWND(hWindow), WM_CLOSE, NULL, NULL);
	return 0;
}

int InitializeComponent()
{
	hWindow = XWnd_Create(20, 20, 320, 100, L"情义登录器", NULL, xc_window_style_modal);//创建窗口
	if (hWindow)
	{
		XShapeText_Create(12, 6, 0, 0, L"情义三国", hWindow);

		HELE hBtnWinClose = XBtn_Create(300, 7, 15, 15, L"X", hWindow);
		XEle_RegEventC(hBtnWinClose, XE_BNCLICK, WinClose_EventBtnClick);

		HELE hBtnWinMin = XBtn_Create(270, 7, 15, 15, L"-", hWindow);
		XEle_RegEventC(hBtnWinMin, XE_BNCLICK, WinMin_EventBtnClick);

		m_hProgressBar = XProgBar_Create(20, 40, 260, 20, hWindow);
		XProgBar_SetRange(m_hProgressBar, 100);
		XProgBar_SetPos(m_hProgressBar, 0);
		XProgBar_SetSpaceTwo(m_hProgressBar, 0, 0);

		XWnd_RegEventC(hWindow, WM_RGSMSG, OnProgressBarChange);

		m_TxtUpdate = XShapeText_Create(220, 70, 0, 0, L"更新中...", hWindow);

		HELE   hBtnLink = XBtn_Create(30, 65, 100, 20, L"点击手动下载", hWindow);
		XBtn_SetStyle(hBtnLink, button_style_scrollbar_slider);
		XEle_RegEventC(hBtnLink, XE_BNCLICK, Link_EventBtnClick);

		XWnd_ShowWindow(hWindow, SW_SHOW);
	}

	//start FTP download
	DWORD threadID;
	HANDLE hThread;
	hThread = CreateThread(NULL, 0, ThreadProc, NULL, 0, &threadID); // 创建线程
	if (hThread == NULL)
	{
		MessageBox(NULL, "更新失败！", "提示", NULL);
		BOOL flag;
		WinClose_EventBtnClick(&flag);
		return -1;
	}

	return 0;
}

int APIENTRY _tWinMain(_In_ HINSTANCE hInstance,
                     _In_opt_ HINSTANCE hPrevInstance,
                     _In_ LPTSTR    lpCmdLine,
                     _In_ int       nCmdShow)
{
	XInitXCGUI();//初始化
	XC_EnableDebugFile(FALSE);

	//截取到长度
	CString CmdLine = lpCmdLine;

	TCHAR seps[] = _T(" ");
	TCHAR* token = _tcstok((LPTSTR)(LPCTSTR)CmdLine, seps);

	INT COUNT = 0;
	while (token != NULL)
	{
		COUNT++;
		if (COUNT == 1)
		{
			StartFile.Format(TEXT("%s"), token);
		}
		else if (COUNT == 2)
		{
			ClntSize.Format(TEXT("%s"), token);
		}
		else
		{
			break;
		}
		token = _tcstok(NULL, seps);
	}
	if (StartFile == "" || ClntSize == "")
	{
		StartFile = "qysg.dat";
		ClntSize = "9010899";
	}

	InitializeComponent(); 

	XRunXCGUI();
	XExitXCGUI();
	return 0;
}
