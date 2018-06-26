// Win32Register.cpp : 定义应用程序的入口点。
//

#include "stdafx.h"
#include <stdlib.h>    
#include <string>  
#include <iostream>

#include "Win32Register.h"
#include "Common.h"
#include "Utils.h"
#include "tlhelp32.h"
#include "tinyxml2.h"

using namespace std;
using namespace tinyxml2;

//包含界面库
#include "xcgui.h"
#pragma comment(lib,"XCGUI.lib")

HWINDOW hWindow = NULL;
NOTIFYICONDATAW tnd;
HWINDOW hWindowRegit = NULL;
HWINDOW hWindowFz = NULL;

#define  WM_SHOWTASK (WM_USER+100)
#define MAX_LOADSTRING 100
#define MAX_CLNT_SIZE  7
static int g_ClntPidMap[MAX_CLNT_SIZE] = { 0, 0, 0, 0, 0, 0, 0 };
static int g_CurClntCount = 0;

//////////////////////////////////////////////////////////////////////////////
static CString ServerAddr;
static char pServerAddr[64] = {0};
static CString ServerPort;
static int iServerPort = 0;
static CString GameVersion;
static CString ClientCopyRight;
static CString ClientTitle;
static CString UpdateLogText;
//////////////////////////////////////////////////////////////////////////////
#include <WinSock2.h>
SOCKET sClient;
HELE hRichEditRegitAccount;
HELE hRichEditRegitPasswd;
HELE hRichEditRegitPasswd2;
HELE hRichEditRegitCode;
HXCGUI hPicCode;
CString m_bitmapCode;
HELE hRichEdit;
HELE hComboBoxKey;
CString ClntPid;

// 全局变量: 
HINSTANCE hInst;								// 当前实例
TCHAR szTitle[MAX_LOADSTRING];					// 标题栏文本
TCHAR szWindowClass[MAX_LOADSTRING];			// 主窗口类名

// 此代码模块中包含的函数的前向声明: 
ATOM				MyRegisterClass(HINSTANCE hInstance);
BOOL				InitInstance(HINSTANCE, int);
LRESULT CALLBACK	WndProc(HWND, UINT, WPARAM, LPARAM);
INT_PTR CALLBACK	About(HWND, UINT, WPARAM, LPARAM);
//-- 托盘处理
#if TRUE
#define WM_TRAY (WM_USER + 100)
#define WM_TASKBAR_CREATED RegisterWindowMessage(TEXT("TaskbarCreated"))

#define APP_NAME	TEXT("三国群英传OL")
#define APP_TIP		TEXT("Win32 API 实现系统托盘程序")

NOTIFYICONDATA nid;		//托盘属性
HMENU hMenu;			//托盘菜单

//实例化托盘
void InitTray(HINSTANCE hInstance, HWND hWnd)
{
	nid.cbSize = sizeof(NOTIFYICONDATA);
	nid.hWnd = hWnd;
	nid.uID = IDI_TRAY;
	nid.uFlags = NIF_ICON | NIF_MESSAGE | NIF_TIP | NIF_INFO;
	nid.uCallbackMessage = WM_TRAY;
	nid.hIcon = LoadIcon(hInstance, MAKEINTRESOURCE(IDI_TRAY));
	lstrcpy(nid.szTip, APP_NAME);

	//hMenu = CreatePopupMenu();//生成托盘菜单
	//为托盘菜单添加两个选项
	//AppendMenu(hMenu, MF_STRING, ID_SHOW, TEXT("提示"));
	//AppendMenu(hMenu, MF_STRING, ID_EXIT, TEXT("退出"));

	Shell_NotifyIcon(NIM_ADD, &nid);
}

//演示托盘气泡提醒
void ShowTrayMsg(CString msg)
{
	lstrcpy(nid.szInfoTitle, APP_NAME);
	lstrcpy(nid.szInfo, msg);
	nid.uTimeout = 1000;
	Shell_NotifyIcon(NIM_MODIFY, &nid);
}

LRESULT CALLBACK WndProc(HWND hWnd, UINT uMsg, WPARAM wParam, LPARAM lParam)
{
	switch (uMsg)
	{
	case WM_TRAY:
		switch (lParam)
		{
		case WM_RBUTTONDOWN:
		{
			//获取鼠标坐标
			POINT pt; GetCursorPos(&pt);

			//解决在菜单外单击左键菜单不消失的问题
			SetForegroundWindow(hWnd);

			//使菜单某项变灰
			//EnableMenuItem(hMenu, ID_SHOW, MF_GRAYED);	

			//显示并获取选中的菜单
			int cmd = TrackPopupMenu(hMenu, TPM_RETURNCMD, pt.x, pt.y, NULL, hWnd,
				NULL);
			/*if (cmd == ID_SHOW)
				MessageBox(hWnd, APP_TIP, APP_NAME, MB_OK);
			if (cmd == ID_EXIT)
				PostMessage(hWnd, WM_DESTROY, NULL, NULL);*/
		}
			break;
		case WM_LBUTTONDOWN:
			MessageBox(hWnd, APP_TIP, APP_NAME, MB_OK);
			break;
		case WM_LBUTTONDBLCLK:
			break;
		}
		break;
	case WM_DESTROY:
		//窗口销毁时删除托盘
		Shell_NotifyIcon(NIM_DELETE, &nid);
		PostQuitMessage(0);
		break;
	case WM_TIMER:
		ShowTrayMsg("");
		KillTimer(hWnd, wParam);
		break;
	}
	if (uMsg == WM_TASKBAR_CREATED)
	{
		//系统Explorer崩溃重启时，重新加载托盘
		Shell_NotifyIcon(NIM_ADD, &nid);
	}
	return DefWindowProc(hWnd, uMsg, wParam, lParam);
}

#endif

//--具体实现：注册，修改密码，获取版本号
void CString2Char(CString str, char ch[])
{
	int i;
	char *tmpch;
	int wLen = WideCharToMultiByte(CP_ACP, 0, str, -1, NULL, 0, NULL, NULL);//得到Char的长度
	tmpch = new char[wLen + 1];                                             //分配变量的地址大小
	WideCharToMultiByte(CP_ACP, 0, str, -1, tmpch, wLen, NULL, NULL);       //将CString转换成char*

	for (i = 0; tmpch[i] != '\0'; i++) ch[i] = tmpch[i];
	ch[i] = '\0';
}
void DoRegister(CString name, CString pwd)
{
	WORD wVersionRequested;
	WSADATA wsaData;
	int err;

	char serverip[64] = { 0 };
	if (Utils::DemainToIp(pServerAddr, serverip) != 0)
	{
		MessageBox(XWnd_GetHWND(hWindow), L"无法连接到远程服务器", L"提示", MB_OK);
		return;
	}

	wVersionRequested = MAKEWORD(2, 2);
	err = WSAStartup(wVersionRequested, &wsaData);//加载套接字库
	if (err != 0)
	{
		return;
	}


	if (LOBYTE(wsaData.wVersion) != 2 ||
		HIBYTE(wsaData.wVersion) != 2)
	{
		WSACleanup();
		return;
	}

	SOCKADDR_IN addrSrv;
	addrSrv.sin_addr.S_un.S_addr = inet_addr(serverip);
	addrSrv.sin_family = AF_INET;
	addrSrv.sin_port = htons(iServerPort);

	sClient = socket(AF_INET, SOCK_STREAM, 0);
	if (sClient == INVALID_SOCKET)
	{
		MessageBox(XWnd_GetHWND(hWindowRegit), L"创建套接字失败", L"提示", MB_OK);
		closesocket(sClient);
		WSACleanup();
		return;

	}
	//向服务器发出连接请求（connect）。
	if (connect(sClient, (SOCKADDR*)&addrSrv, sizeof(SOCKADDR)) == SOCKET_ERROR)
	{
		MessageBox(XWnd_GetHWND(hWindowRegit), L"连接服务器失败", L"提示", MB_OK);
		closesocket(sClient);
		WSACleanup();
		return;
	}

	//发送注册消息
	//SG_MSG msg;
	//memcpy(msg.start, START_FLAG, START_FLAG_LEN);
	//msg.type = REGISTER;
	//srand(time(0));
	//int nRand = rand() % 10 + 1;
	//msg.key = nRand;

	//简单加密
	//name = Common::GetInstance()->Encode(msg.key, name);
	//pwd = Common::GetInstance()->Encode(msg.key, pwd);

	//SG_MSG_REG reg_msg;
	//memset(&reg_msg, 0x0, sizeof(SG_MSG_REG));
	//CString2Char(name, reg_msg.name);
	//CString2Char(pwd, reg_msg.passwd);
	//memcpy(msg.buf, &reg_msg, sizeof(reg_msg));

	Rg_Info rg_info;
	Tran_Head tran_head;
	CString2Char(name, rg_info.name);
	CString2Char(pwd, rg_info.passwd);
	srand(time(0));
	int nRand = rand() % 10 + 1;
	CString str = "";
	//str.Format(_T("%d"), nRand);
	str.Format(_T("%d"), 110);

	CString2Char(str, rg_info.key);

	BYTE msg[2048] = { 0 };
	tran_head.cmd = 1;
	tran_head.length = sizeof(rg_info);
	memcpy(msg, &tran_head, sizeof(tran_head));
	memcpy(msg + sizeof(tran_head), &rg_info, sizeof(rg_info));

	//if (send(sClient, (char*)&msg, sizeof(SG_MSG), 0) == SOCKET_ERROR)
	if (send(sClient, (char*)&msg, 2048, 0) == SOCKET_ERROR)
	{
		MessageBox(XWnd_GetHWND(hWindowRegit), L"发送失败", L"提示", MB_OK);
		closesocket(sClient);
		WSACleanup();
		return;
	}

	char szBuff[400];
	memset(szBuff, 0, sizeof(szBuff));
	int ret;
	//设置等到时间
	int nNetTimeout = 6000;//3秒
	setsockopt(sClient, SOL_SOCKET, SO_RCVTIMEO, (char *)&nNetTimeout, sizeof(nNetTimeout));
	ret = recv(sClient, szBuff, sizeof(szBuff), 0);
	if (ret > 0)
	{
		CString retError = "";
		retError.Format(_T("%s"), szBuff);
		MessageBox(XWnd_GetHWND(hWindowRegit), retError, L"提示", MB_OK);
	}
	else
	{
		MessageBox(XWnd_GetHWND(hWindowRegit), L"服务器响应超时，请重试！", L"提示", MB_OK);
	}
}
void DoAccMgr(CString name, CString pwd, CString pwd2)
{
	WORD wVersionRequested;
	WSADATA wsaData;
	int err;

	char serverip[64] = { 0 };
	if (Utils::DemainToIp(pServerAddr, serverip) != 0)
	{
		MessageBox(XWnd_GetHWND(hWindow), L"无法连接到远程服务器", L"提示", MB_OK);
		return;
	}

	wVersionRequested = MAKEWORD(2, 2);
	err = WSAStartup(wVersionRequested, &wsaData);//加载套接字库
	if (err != 0)
	{
		return;
	}


	if (LOBYTE(wsaData.wVersion) != 2 ||
		HIBYTE(wsaData.wVersion) != 2)
	{
		WSACleanup();
		return;
	}

	SOCKADDR_IN addrSrv;
	addrSrv.sin_addr.S_un.S_addr = inet_addr(serverip);
	addrSrv.sin_family = AF_INET;
	addrSrv.sin_port = htons(iServerPort);

	sClient = socket(AF_INET, SOCK_STREAM, 0);
	if (sClient == INVALID_SOCKET)
	{
		MessageBox(XWnd_GetHWND(hWindowRegit), L"创建套接字失败", L"提示", MB_OK);
		closesocket(sClient);
		WSACleanup();
		return;

	}
	//向服务器发出连接请求（connect）。
	if (connect(sClient, (SOCKADDR*)&addrSrv, sizeof(SOCKADDR)) == SOCKET_ERROR)
	{
		MessageBox(XWnd_GetHWND(hWindowRegit), L"连接服务器失败", L"提示", MB_OK);
		closesocket(sClient);
		WSACleanup();
		return;
	}

	Mdfy_Pwd_Info _info;
	Tran_Head tran_head;
	CString2Char(name, _info.name);
	CString2Char(pwd, _info.oldpasswd);
	CString2Char(pwd2, _info.newpasswd);
	srand(time(0));
	int nRand = rand() % 10 + 1;
	CString str = "";
	//str.Format(_T("%d"), nRand);
	str.Format(_T("%d"), 110);

	CString2Char(str, _info.key);

	BYTE msg[2048] = { 0 };
	tran_head.cmd = 3;
	tran_head.length = sizeof(_info);
	memcpy(msg, &tran_head, sizeof(tran_head));
	memcpy(msg + sizeof(tran_head), &_info, sizeof(_info));

	//if (send(sClient, (char*)&msg, sizeof(SG_MSG), 0) == SOCKET_ERROR)
	if (send(sClient, (char*)&msg, 2048, 0) == SOCKET_ERROR)
	{
		MessageBox(XWnd_GetHWND(hWindowRegit), L"发送失败", L"提示", MB_OK);
		closesocket(sClient);
		WSACleanup();
		return;
	}

	char szBuff[400];
	memset(szBuff, 0, sizeof(szBuff));
	int ret;
	//设置等到时间
	int nNetTimeout = 6000;//3秒
	setsockopt(sClient, SOL_SOCKET, SO_RCVTIMEO, (char *)&nNetTimeout, sizeof(nNetTimeout));
	ret = recv(sClient, szBuff, sizeof(szBuff), 0);
	if (ret > 0)
	{
		CString retError = "";
		retError.Format(_T("%s"), szBuff);
		MessageBox(XWnd_GetHWND(hWindowRegit), retError, L"提示", MB_OK);
	}
	else
	{
		MessageBox(XWnd_GetHWND(hWindowRegit), L"服务器响应超时，请重试！", L"提示", MB_OK);
	}
}

//--生成验证码
#if TRUE
BOOL  DoCreateBitmap(HDC hDC, HBITMAP hbitmap,
	PBITMAPFILEHEADER &outheadbuf, long *outheadsize,
	PBITMAPINFO &outinfobuf, long *outinfosize,
	LPBYTE &outdatabuf, long *outdatasize)	//生成单色位图					 
{
	BITMAP bmp;
	WORD cClrBits;
	DWORD my_biClrUsed = 0;
	outinfobuf = NULL;
	outdatabuf = NULL;
	outheadbuf = NULL;

	if (!GetObject(hbitmap, sizeof(BITMAP), (LPSTR)&bmp))
		goto errout;
	bmp.bmPlanes = 1;
	bmp.bmBitsPixel = 1;  //强制赋值转换出来的每像素BIT数
	cClrBits = 1;  //得到每像素多少位		
	*outinfosize = sizeof(BITMAPINFOHEADER)+sizeof(RGBQUAD)* (1 << cClrBits);
	outinfobuf = (PBITMAPINFO)GlobalAlloc(GPTR, *outinfosize);
	outinfobuf->bmiHeader.biSize = sizeof(BITMAPINFOHEADER); //信息头大小（不含调色板）	
	outinfobuf->bmiHeader.biWidth = bmp.bmWidth;
	outinfobuf->bmiHeader.biHeight = bmp.bmHeight;
	outinfobuf->bmiHeader.biPlanes = bmp.bmPlanes;
	outinfobuf->bmiHeader.biBitCount = bmp.bmBitsPixel;
	my_biClrUsed = (1 << cClrBits);
	outinfobuf->bmiHeader.biClrUsed = my_biClrUsed;
	outinfobuf->bmiHeader.biCompression = BI_RGB;
	outinfobuf->bmiHeader.biSizeImage = ((outinfobuf->bmiHeader.biWidth * cClrBits + 31) & ~31)\

		/ 8 * outinfobuf->bmiHeader.biHeight;
	//图像大小	
	outinfobuf->bmiHeader.biClrImportant = 0;
	/////////////////////////////////得到位图数据	
	// GlobalAlloc分配位图数据的内存	
	// GetDIBits 根据hDC 和HBITMAP得到位图数据、调色板数据	
	*outdatasize = outinfobuf->bmiHeader.biSizeImage;
	outdatabuf = (LPBYTE)GlobalAlloc(GPTR, *outdatasize);  //根据位图大小分配内存	
	if (!outdatabuf)
		goto errout;
	if (!GetDIBits(//根据DC和BITMAP得到位图数据		
		hDC,
		hbitmap,
		0,
		(WORD)outinfobuf->bmiHeader.biHeight,
		outdatabuf,     // outdatabuf中得到位图数据		
		outinfobuf,
		DIB_RGB_COLORS))

	{
		goto errout;
	}

	/////////////////////////////////得到文件头	
	*outheadsize = sizeof(BITMAPFILEHEADER);
	outheadbuf = (PBITMAPFILEHEADER)GlobalAlloc(GPTR, *outheadsize);
	//根据位图大小分配内存	
	if (!outheadbuf)
		goto errout;
	outheadbuf->bfType = 0x4d42;
	outheadbuf->bfSize = (DWORD)(sizeof(BITMAPFILEHEADER)+
		outinfobuf->bmiHeader.biSize +
		my_biClrUsed * sizeof(RGBQUAD)+
		outinfobuf->bmiHeader.biSizeImage);
	outheadbuf->bfReserved1 = 0;
	outheadbuf->bfReserved2 = 0;

	outheadbuf->bfOffBits = (DWORD) sizeof(BITMAPFILEHEADER)+
		outinfobuf->bmiHeader.biSize +
		my_biClrUsed * sizeof (RGBQUAD);
	return TRUE;

	//////////////////////错误处理	
errout:
	if (outinfobuf) GlobalFree(outinfobuf);
	if (outdatabuf) GlobalFree(outdatabuf);
	if (outheadbuf) GlobalFree(outheadbuf);
	outinfobuf = NULL;
	outdatabuf = NULL;
	outheadbuf = NULL;
	*outheadsize = 0;
	*outinfosize = 0;
	*outdatasize = 0;
	return FALSE;
}
BOOL DoCreateCode(CString str, CFile& file)//生成汉字验证码
{
	ASSERT(0 == str.GetLength() % 2);

	CWnd* pDesk = CWnd::GetDesktopWindow();
	CDC* pDC = pDesk->GetDC();

	//每个字符的位置随机偏移4
	CRect r(0, 0, 0, 0);
	pDC->DrawText(str, &r, DT_CALCRECT);
	const int w = r.Width() + 4;
	const int h = r.Height() + 4;
	const int iCharWidth = w * 2 / str.GetLength();

	//建立内存DC和位图并填充背景
	CBitmap bm;
	bm.CreateCompatibleBitmap(pDC, w, h);
	CDC memdc;
	memdc.CreateCompatibleDC(pDC);
	CBitmap*pOld = memdc.SelectObject(&bm);
	memdc.FillSolidRect(0, 0, w, h, RGB(255, 255, 255));

	::SetGraphicsMode(memdc.m_hDC, GM_ADVANCED);//为字体倾斜作准备

	for (int i = 0; i < str.GetLength() / 2; i++)
	{
		//设置字体
		CFont* pFont = memdc.GetCurrentFont();
		LOGFONT logFont;
		pFont->GetLogFont(&logFont);
		logFont.lfOutPrecision = OUT_TT_ONLY_PRECIS;
		logFont.lfOrientation = rand() % 90;
		CFont font;
		font.CreateFontIndirect(&logFont);
		memdc.SelectObject(&font);

		int x = iCharWidth*i + rand() % 5;
		int y = rand() % 5;
		memdc.TextOut(x, y, str.Mid(i * 2, 2));
	}

	//将内容存到文件(CFile或CMemFile)
	PBITMAPFILEHEADER outheadbuf;
	PBITMAPINFO outinfobuf;
	LPBYTE outdatabuf;
	long outheadsize, outinfosize, outdatasize;
	if (!DoCreateBitmap(memdc.m_hDC, bm,
		outheadbuf, &outheadsize,
		outinfobuf, &outinfosize,
		outdatabuf, &outdatasize))
		return FALSE;


	file.Write(outheadbuf, outheadsize);
	file.Write(outinfobuf, outinfosize);
	file.Write(outdatabuf, outdatasize);

	memdc.SelectObject(pOld);
	bm.DeleteObject();
	memdc.DeleteDC();

	return TRUE;
}
void OnCreateCode()
{
	int nRand;
	srand((unsigned)time(NULL));
	nRand = rand() % 1000 + 1000;

	m_bitmapCode.Format(_T("%d"), nRand);

	CString strFileName = ".\\out.bmp";
	CFile file;
	file.Open(strFileName, CFile::modeCreate | CFile::modeWrite);
	DoCreateCode(m_bitmapCode, file);
	file.Close();
	
	HBITMAP bitmap = (HBITMAP)LoadImage(NULL, strFileName, IMAGE_BITMAP, 0, 0, LR_LOADFROMFILE | LR_CREATEDIBSECTION);
	HIMAGE codeImg = XImage_LoadFileFromHBITMAP(bitmap);
	XShapePic_SetImage(hPicCode, codeImg);
	XWnd_RedrawWnd(hWindowRegit, TRUE);
}
#endif

//--子窗口按钮事件处理
#if TRUE 
int CALLBACK ComboBoxKey_EventChange(BOOL *pbHandled)
{
	//*pbHandled = TRUE;
	wchar_t key[1024] = {};
	XRichEdit_GetText(hComboBoxKey, key, 1024);
	CString _Key = key;
	MessageBox(NULL, _Key, L"提示", MB_OK);
	return 0;
}

int CALLBACK DoCodeFlush_EventBtnClick(BOOL *pbHandled)
{
	OnCreateCode();

	*pbHandled = TRUE;
	return 0;
}
int CALLBACK DoRegist_EventBtnClick(BOOL *pbHandled)
{
	CString Account;
	CString Passwd;
	CString Passwd2;
	CString Code;
	wchar_t account[64];
	wchar_t passwd[64];
	wchar_t passwd2[64];
	wchar_t code[64];

	XRichEdit_GetText(hRichEditRegitAccount, account, 24);
	XRichEdit_GetText(hRichEditRegitPasswd, passwd, 24);
	XRichEdit_GetText(hRichEditRegitPasswd2, passwd2, 24);
	XRichEdit_GetText(hRichEditRegitCode, code, 24);
	Account = account;
	Passwd = passwd;
	Passwd2 = passwd2;
	Code = code;

	//检测
	if (m_bitmapCode != Code)
	{
		MessageBox(XWnd_GetHWND(hWindowRegit), L"验证码错误", L"提示", MB_OK);
		XRichEdit_SetText(hRichEditRegitCode, L"");
		return -1;
	}
	if (Passwd != Passwd2)
	{
		MessageBox(XWnd_GetHWND(hWindowRegit), L"两次输入密码不一致，请核对！", L"提示", MB_OK);
		XRichEdit_SetText(hRichEditRegitPasswd, L"");
		XRichEdit_SetText(hRichEditRegitPasswd2, L"");
		return -1;
	}
	//字符合法性
	if (Account.GetAllocLength() > 24 || Account.GetAllocLength() < 6)
	{
		MessageBox(XWnd_GetHWND(hWindowRegit), L"账号必须是6-24位长度！", L"提示", MB_OK);
		XRichEdit_SetText(hRichEditRegitAccount, L"");
		return -1;
	}

	if (Passwd.GetAllocLength() > 24 || Passwd.GetAllocLength() < 6)
	{
		MessageBox(XWnd_GetHWND(hWindowRegit), L"密码必须是6-24位长度！", L"提示", MB_OK);
		XRichEdit_SetText(hRichEditRegitPasswd, L"");
		XRichEdit_SetText(hRichEditRegitPasswd2, L"");
		return -1;
	}

	if (!Common::GetInstance()->FullNumAndWord(Account))
	{
		MessageBox(XWnd_GetHWND(hWindowRegit), L"账号必须仅由字母和数字组成", L"提示", MB_OK);
		XRichEdit_SetText(hRichEditRegitAccount, L"");
		return -1;
	}

	if (!Common::GetInstance()->FullNumAndWord(Passwd))
	{
		MessageBox(XWnd_GetHWND(hWindowRegit), L"密码必须仅由字母和数字组成", L"提示", MB_OK);
		XRichEdit_SetText(hRichEditRegitPasswd, L"");
		XRichEdit_SetText(hRichEditRegitPasswd2, L"");
		return -1;
	}

	DoRegister(Account, Passwd);
	OnCreateCode();
	//XWnd_RedrawWnd(hWindowRegit, TRUE);
	*pbHandled = TRUE;
	return 0;
}
int CALLBACK DoAccMgr_EventBtnClick(BOOL *pbHandled)
{
	CString Account;
	CString Passwd;
	CString Passwd2;
	CString Code;
	wchar_t account[64];
	wchar_t passwd[64];
	wchar_t passwd2[64];
	wchar_t code[64];

	XRichEdit_GetText(hRichEditRegitAccount, account, 24);
	XRichEdit_GetText(hRichEditRegitPasswd, passwd, 24);
	XRichEdit_GetText(hRichEditRegitPasswd2, passwd2, 24);
	XRichEdit_GetText(hRichEditRegitCode, code, 24);
	Account = account;
	Passwd = passwd;
	Passwd2 = passwd2;
	Code = code;

	//检测
	if (m_bitmapCode != Code)
	{
		MessageBox(XWnd_GetHWND(hWindowRegit), L"验证码错误", L"提示", MB_OK);
		XRichEdit_SetText(hRichEditRegitCode, L"");
		return -1;
	}
	if (Passwd == Passwd2)
	{
		MessageBox(XWnd_GetHWND(hWindowRegit), L"两次输入密码一致，请核对！", L"提示", MB_OK);
		return -1;
	}
	//字符合法性
	if (Account.GetAllocLength() > 24 || Account.GetAllocLength() < 6)
	{
		MessageBox(XWnd_GetHWND(hWindowRegit), L"账号必须是6-24位长度！", L"提示", MB_OK);
		XRichEdit_SetText(hRichEditRegitAccount, L"");
		return -1;
	}

	if (Passwd.GetAllocLength() > 24 || Passwd.GetAllocLength() < 6)
	{
		MessageBox(XWnd_GetHWND(hWindowRegit), L"密码必须是6-24位长度！", L"提示", MB_OK);
		XRichEdit_SetText(hRichEditRegitPasswd, L"");
		XRichEdit_SetText(hRichEditRegitPasswd2, L"");
		return -1;
	}

	if (!Common::GetInstance()->FullNumAndWord(Account))
	{
		MessageBox(XWnd_GetHWND(hWindowRegit), L"账号必须仅由字母和数字组成", L"提示", MB_OK);
		XRichEdit_SetText(hRichEditRegitAccount, L"");
		return -1;
	}

	if (!Common::GetInstance()->FullNumAndWord(Passwd))
	{
		MessageBox(XWnd_GetHWND(hWindowRegit), L"密码必须仅由字母和数字组成", L"提示", MB_OK);
		XRichEdit_SetText(hRichEditRegitPasswd, L"");
		XRichEdit_SetText(hRichEditRegitPasswd2, L"");
		return -1;
	}

	DoAccMgr(Account, Passwd, Passwd2);
	OnCreateCode();
	//XWnd_RedrawWnd(hWindowRegit, TRUE);
	*pbHandled = TRUE;
	return 0;
}
#endif

//--主窗口按钮事件处理
int LoadSrcToEdit(int SRCID)
{
	XRichEdit_SetText(hRichEdit, L"");
	HRSRC hRes = FindResource(NULL, MAKEINTRESOURCE(SRCID), _T("TXT"));
	DWORD len = SizeofResource(NULL, hRes);
	HGLOBAL hg = LoadResource(NULL, hRes);

	CString tmp = "";
	tmp = (CHAR*)LockResource(hg);
	XRichEdit_SetText(hRichEdit, tmp);
	FreeResource(hg);

	return 1;
}
int CALLBACK WinClose_EventBtnClick(BOOL *pbHandled)
{
	for (int i = 0; i < MAX_CLNT_SIZE; i++)
	{
		if (g_ClntPidMap[i] != 0)
		{
			HANDLE hProcess = OpenProcess(PROCESS_ALL_ACCESS, FALSE, g_ClntPidMap[i]);
			if (hProcess != NULL)
			{
				if (TerminateProcess(hProcess, 0))
				{
					g_ClntPidMap[i] = 0;
					g_CurClntCount--;
				}
			}
		}
	}

	if (hWindow != NULL)
	{
		Shell_NotifyIcon(NIM_DELETE, &nid);//向任务栏添加图标
		::SendMessage(XWnd_GetHWND(hWindow), WM_CLOSE, NULL, NULL);
	}
	*pbHandled = TRUE;
	return 0;
}
int CALLBACK WinMin_EventBtnClick(BOOL *pbHandled)
{
	ShowTrayMsg("登录器托盘在这里！");
	XWnd_ShowWindow(hWindow, SW_HIDE);//SW_MINIMIZE
	//ShowWindow(XWnd_GetHWND(hWindow), SW_HIDE);

	*pbHandled = TRUE;
	return 0;
}
int CALLBACK MainWinShow_EventBtnClick(WPARAM wParam, LPARAM IParam)//BOOL *pbHandled
{
	if ((IParam == WM_LBUTTONDOWN) || (IParam == WM_RBUTTONDOWN))
	{
		XWnd_ShowWindow(hWindow, SW_SHOW);
	}
	//*pbHandled = TRUE;
	return 0;
}
int CALLBACK Regist_EventBtnClick(BOOL *pbHandled)
{
	hWindowRegit = XModalWnd_Create(383, 230, L"用户注册", XWnd_GetHWND(hWindow));

	XShapeText_Create(12, 6, 0, 0, L"用户注册", hWindowRegit);
	XShapeText_Create(88, 54, 60, 20, L"注册账户:", hWindowRegit);
	XShapeText_Create(88, 82, 60, 20, L"注册密码:", hWindowRegit);
	XShapeText_Create(88, 110, 60, 20, L"确认密码:", hWindowRegit);
	XShapeText_Create(100, 139, 60, 20, L"验证码:", hWindowRegit);

	hRichEditRegitAccount = XRichEdit_Create(167, 54, 135, 20, hWindowRegit);
	hRichEditRegitPasswd = XRichEdit_Create(167, 82, 135, 20, hWindowRegit);
	hRichEditRegitPasswd2 = XRichEdit_Create(167, 110, 135, 20, hWindowRegit);
	XRichEdit_EnablePassword(hRichEditRegitPasswd, TRUE);
	XRichEdit_EnablePassword(hRichEditRegitPasswd2, TRUE);

	hRichEditRegitCode = XRichEdit_Create(167, 139, 51, 20, hWindowRegit);
	hPicCode = XShapePic_Create(226, 139, 76, 20, hWindowRegit);

	HELE hBtnDoRegist = XBtn_Create(160, 170, 72, 29, L"确认注册", hWindowRegit);
	XEle_RegEventC(hBtnDoRegist, XE_BNCLICK, DoRegist_EventBtnClick);

	HELE hBtnCodeFlush = XBtn_Create(306, 140, 20, 20, L"", hWindowRegit);
	HRSRC hRes = FindResource(NULL, MAKEINTRESOURCE(IDB_PNG1), _T("PNG"));
	DWORD len = SizeofResource(NULL, hRes);
	HGLOBAL hg = LoadResource(NULL, hRes);
	LPVOID lp = (LPSTR)LockResource(hg);
	XBtn_AddBkImage(hBtnCodeFlush, button_state_stay, XImage_LoadMemory(lp, len, TRUE));
	XBtn_AddBkImage(hBtnCodeFlush, button_state_leave, XImage_LoadMemory(lp, len, TRUE));
	XBtn_AddBkImage(hBtnCodeFlush, button_state_down, XImage_LoadMemory(lp, len, TRUE));
	XBtn_AddBkImage(hBtnCodeFlush, button_state_check, XImage_LoadMemory(lp, len, TRUE));
	XBtn_SetStyle(hBtnCodeFlush, button_style_scrollbar_slider);
	XEle_RegEventC(hBtnCodeFlush, XE_BNCLICK, DoCodeFlush_EventBtnClick);

	HELE hBtnWinClose = XBtn_Create(357, 7, 15, 15, L"X", hWindowRegit);
	OnCreateCode();

	XBtn_SetType(hBtnWinClose, button_type_close);
	int nResult = XModalWnd_DoModal(hWindowRegit);

	*pbHandled = TRUE;
	return 0;
}
int CALLBACK AccMgr_EventBtnClick(BOOL *pbHandled)
{
	hWindowRegit = XModalWnd_Create(383, 230, L"账户管理", XWnd_GetHWND(hWindow));

	XShapeText_Create(12, 6, 0, 0, L"账户管理", hWindowRegit);
	XShapeText_Create(88, 54, 60, 20, L"注册账户:", hWindowRegit);
	XShapeText_Create(88, 82, 60, 20, L"原始密码:", hWindowRegit);
	XShapeText_Create(88, 110, 60, 20, L"新 密码:", hWindowRegit);
	XShapeText_Create(100, 139, 60, 20, L"验证码:", hWindowRegit);

	hRichEditRegitAccount = XRichEdit_Create(167, 54, 135, 20, hWindowRegit);
	hRichEditRegitPasswd = XRichEdit_Create(167, 82, 135, 20, hWindowRegit);
	hRichEditRegitPasswd2 = XRichEdit_Create(167, 110, 135, 20, hWindowRegit);
	XRichEdit_EnablePassword(hRichEditRegitPasswd, TRUE);
	XRichEdit_EnablePassword(hRichEditRegitPasswd2, TRUE);

	hRichEditRegitCode = XRichEdit_Create(167, 139, 51, 20, hWindowRegit);
	hPicCode = XShapePic_Create(226, 139, 76, 20, hWindowRegit);

	HELE hBtnDoRegist = XBtn_Create(160, 170, 72, 29, L"确认修改", hWindowRegit);
	XEle_RegEventC(hBtnDoRegist, XE_BNCLICK, DoAccMgr_EventBtnClick);

	HELE hBtnCodeFlush = XBtn_Create(306, 140, 20, 20, L"", hWindowRegit);
	HRSRC hRes = FindResource(NULL, MAKEINTRESOURCE(IDB_PNG1), _T("PNG"));
	DWORD len = SizeofResource(NULL, hRes);
	HGLOBAL hg = LoadResource(NULL, hRes);
	LPVOID lp = (LPSTR)LockResource(hg);
	XBtn_AddBkImage(hBtnCodeFlush, button_state_stay, XImage_LoadMemory(lp, len, TRUE));
	XBtn_AddBkImage(hBtnCodeFlush, button_state_leave, XImage_LoadMemory(lp, len, TRUE));
	XBtn_AddBkImage(hBtnCodeFlush, button_state_down, XImage_LoadMemory(lp, len, TRUE));
	XBtn_AddBkImage(hBtnCodeFlush, button_state_check, XImage_LoadMemory(lp, len, TRUE));
	XBtn_SetStyle(hBtnCodeFlush, button_style_scrollbar_slider);
	XEle_RegEventC(hBtnCodeFlush, XE_BNCLICK, DoCodeFlush_EventBtnClick);

	HELE hBtnWinClose = XBtn_Create(357, 7, 15, 15, L"X", hWindowRegit);
	OnCreateCode();

	XBtn_SetType(hBtnWinClose, button_type_close);
	int nResult = XModalWnd_DoModal(hWindowRegit);

	*pbHandled = TRUE;
	return 0;
}

void LoadXmlInfos()
{
	try{
		tinyxml2::XMLDocument doc;
		XMLError ret = doc.LoadFile("main.xml");

		if (ret != XML_SUCCESS)
		{
			MessageBox(NULL, L"配置文件错误，请查实", L"提示", NULL);
			exit(1);
		}
		XMLElement* element;
		XMLElement* document = doc.RootElement();
		element = document->FirstChildElement("ServerAddr");
		ServerAddr = W(element->GetText());
		element = document->FirstChildElement("ServerPort");
		ServerPort = W(element->GetText());
		element = document->FirstChildElement("GameVersion");
		GameVersion = W(element->GetText());
		element = document->FirstChildElement("ClientCopyRight");
		ClientCopyRight = W(element->GetText());
		element = document->FirstChildElement("ClientTitle");
		ClientTitle = W(element->GetText());
		element = document->FirstChildElement("UpdateLogText");
		UpdateLogText = W(element->GetText());

		wchar_t *pWChar = ServerAddr.GetBuffer(); //获取str的宽字符用数组保存  
		ServerAddr.ReleaseBuffer();
		int nLen = ServerAddr.GetLength(); //获取str的字符数  
		char *addr = new char[nLen * 2 + 1];
		memset(addr, 0, nLen * 2 + 1);
		wcstombs(addr, pWChar, nLen * 2 + 1); //宽字符转换为多字节字符 
		strcpy(pServerAddr, addr);
		delete[] addr;

		pWChar = ServerPort.GetBuffer(); //获取str的宽字符用数组保存  
		ServerPort.ReleaseBuffer();
		nLen = ServerPort.GetLength(); //获取str的字符数  
		char *port = new char[nLen * 2 + 1];
		memset(port, 0, nLen * 2 + 1);
		wcstombs(port, pWChar, nLen * 2 + 1); //宽字符转换为多字节字符 
		iServerPort = atoi(port);
		delete[] port;
	}
	catch (exception ex){
		MessageBox(NULL, L"配置文件错误，请查实", L"提示", NULL);
		exit(1);
	}
}

int InitializeComponent()
{
	LoadXmlInfos();

	CString str = ClientTitle;
	LPCWSTR title = (LPCWSTR)str;

	hWindow = XWnd_Create(20, 20, 200, 333, title, NULL, xc_window_style_modal);//创建窗口
	if (hWindow)
	{
		CString curVer = ClientCopyRight + "三国 V" + GameVersion;
		XShapeText_Create(12, 6, 0, 0, curVer, hWindow);

		HELE hBtnWinClose = XBtn_Create(170, 7, 15, 15, L"X", hWindow);
		XEle_RegEventC(hBtnWinClose, XE_BNCLICK, WinClose_EventBtnClick);

		HELE hBtnWinMin = XBtn_Create(150, 7, 15, 15, L"-", hWindow);
		XEle_RegEventC(hBtnWinMin, XE_BNCLICK, WinMin_EventBtnClick);

		HELE hBtnRegist = XBtn_Create(30, 285, 60, 25, L"用户注册", hWindow);
		XEle_RegEventC(hBtnRegist, XE_BNCLICK, Regist_EventBtnClick);

		HELE hBtnAccMgr = XBtn_Create(110, 285, 60, 25, L"账户管理", hWindow);
		XEle_RegEventC(hBtnAccMgr, XE_BNCLICK, AccMgr_EventBtnClick);

		hRichEdit = XRichEdit_Create(20, 45, 160, 223, hWindow);
		XRichEdit_EnableAutoWrap(hRichEdit, TRUE);
		XRichEdit_EnableReadOnly(hRichEdit, TRUE);

		XRichEdit_SetText(hRichEdit, UpdateLogText);

		InitTray(NULL, XWnd_GetHWND(hWindow));

		XWnd_RegEventC(hWindow, WM_SHOWTASK, MainWinShow_EventBtnClick);

		XWnd_ShowWindow(hWindow, SW_SHOW);
	}

	return 0;
}

//定义函数：  
inline void EnableMemLeakCheck()
{
	_CrtSetDbgFlag(_CrtSetDbgFlag(_CRTDBG_REPORT_FLAG) | _CRTDBG_LEAK_CHECK_DF);
}

#define SCANSETTINGS_CLASSNAME      _T("ScanSettingsWindowClass")  
#define APPMUTEX                    _T("Global\\ScanSettings")  
HANDLE m_hMutex;

BOOL RestrictOneInstance(LPCTSTR className, LPCTSTR winName)
{
	SECURITY_DESCRIPTOR secutityDese;
	::InitializeSecurityDescriptor(&secutityDese, SECURITY_DESCRIPTOR_REVISION);
	::SetSecurityDescriptorDacl(&secutityDese, TRUE, NULL, FALSE);

	SECURITY_ATTRIBUTES securityAttr;
	securityAttr.nLength = sizeof SECURITY_ATTRIBUTES;
	securityAttr.bInheritHandle = FALSE;
	securityAttr.lpSecurityDescriptor = &secutityDese;

	m_hMutex = ::CreateMutex(&securityAttr, FALSE, APPMUTEX);
	BOOL bLaunched = (m_hMutex != NULL && ERROR_ALREADY_EXISTS == GetLastError());

	CWnd *pWndPrev = NULL;
	CWnd *pWndChild = NULL;

	if (pWndPrev == NULL)
	{
		if (className == NULL && winName == NULL)
		{
			return TRUE;
		}
		pWndPrev = CWnd::FindWindow(className, winName);
	}

	if (pWndPrev != NULL)
	{
		pWndPrev->ShowWindow(SW_SHOW);
		// If so, does it have any popups?  
		pWndChild = pWndPrev->GetLastActivePopup();

		// If iconic, restore the main window  
		if (pWndPrev->IsIconic())
		{
			pWndPrev->ShowWindow(SW_RESTORE);
		}

		// Bring the main window or its popup to  
		// the foreground  
		pWndChild->SetForegroundWindow();

		return FALSE;
	}

	return TRUE;
}

int APIENTRY _tWinMain(_In_ HINSTANCE hInstance,
                     _In_opt_ HINSTANCE hPrevInstance,
                     _In_ LPTSTR    lpCmdLine,
                     _In_ int       nCmdShow)
{
	// 创建互斥量
	CString str = ClientCopyRight + "注册器";
	LPCWSTR title = (LPCWSTR)str;
	HANDLE hMutex = CreateMutex(NULL, FALSE, title);
	// 检查错误代码
	if (GetLastError() == ERROR_ALREADY_EXISTS)
	{
		// 如果已有互斥量存在则释放句柄并复位互斥量
		MessageBox(NULL, L"程序已经开启,请注意是否托盘！", L"提示", NULL);
		return FALSE;
	}

	XInitXCGUI();//初始化
	XC_EnableDebugFile(FALSE);

	InitializeComponent();

	XRunXCGUI();
	XExitXCGUI();

	return 0;
}
