// Win32Protect.cpp : 定义应用程序的入口点。
//

#include "stdafx.h"
#include "Win32Protect.h"
#include "Common.h"
#include "Utils.h"
#include "tlhelp32.h"

//包含界面库
#include "xcgui.h"
#pragma comment(lib,"XCGUI.lib")

HWINDOW hWindow = NULL;
HWINDOW hWindowRegit = NULL;

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
CString ClntPid;

// 此代码模块中包含的函数的前向声明: 
ATOM				MyRegisterClass(HINSTANCE hInstance);
BOOL				InitInstance(HINSTANCE, int);
LRESULT CALLBACK	WndProc(HWND, UINT, WPARAM, LPARAM);
INT_PTR CALLBACK	About(HWND, UINT, WPARAM, LPARAM);

int CALLBACK WinClose_EventBtnClick(BOOL *pbHandled)
{
	*pbHandled = TRUE;
	INT PID = Utils::KillProcessByName(StartFile);

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
	CString str[] = { L"创意嘉和软件", L"按键精灵", L"按键小精灵" };
	INT pid[] = { _ttoi(ClntPid) };
	while (1)
	{
		try
		{
			BOOL result = Utils::GetProcessList(str, 3, pid, 1);
			if (result)
			{
				//close client 
				BOOL flag;
				WinClose_EventBtnClick(&flag);
				//MessageBox(NULL, L"检测到非法程序运行，请遵守游戏规则！", L"提示", NULL);
			}

			/*if (FindWindow(NULL, L"QYProClient") == 0)
			{
				BOOL flag;
				WinClose_EventBtnClick(&flag);
			}*/
		}
		catch (CException& w)
		{
		}
		
		Sleep(5000);
	}
	return 0;
}

int InitializeComponent()
{
	hWindow = XWnd_Create(20, 20, 597, 333, L"情义登录器",NULL,xc_window_style_modal);//创建窗口
	if (hWindow)
	{
		XShapeText_Create(12, 6, 0, 0, L"情义三国", hWindow);

		HELE hBtnWinClose = XBtn_Create(573, 7, 15, 15, L"X", hWindow);
		XEle_RegEventC(hBtnWinClose, XE_BNCLICK, WinClose_EventBtnClick);

		HELE hBtnWinMin = XBtn_Create(553, 7, 15, 15, L"-", hWindow);
		XEle_RegEventC(hBtnWinMin, XE_BNCLICK, WinMin_EventBtnClick);

		//XWnd_ShowWindow(hWindow, SW_SHOW);
	}

	//start listen
	DWORD threadID;
	HANDLE hThread;
	hThread = CreateThread(NULL, 0, ThreadProc, NULL, 0, &threadID); // 创建线程
	if (hThread == NULL)
	{
		MessageBox(NULL, L"启动失败！", L"提示", NULL);
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
			ClntPid.Format(TEXT("%s"), token);
		}
		else
		{
			break;
		}
		token = _tcstok(NULL, seps);
	}

	InitializeComponent(); 
	//从lpCmdLine解析出要关闭的进程名和保护的进程ID
	//MessageBox(NULL, lpCmdLine, L"提示", MB_OK);

	//MessageBox(NULL, StartFile, L"提示StartFile", MB_OK);
	//MessageBox(NULL, ClntPid, L"提示ClntPid", MB_OK);

	XRunXCGUI();
	XExitXCGUI();
	return 0;
}
