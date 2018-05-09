
// KeyScriptDlg.cpp : 实现文件
//

#include "stdafx.h"
#include "KeyScript.h"
#include "KeyScriptDlg.h"
#include "afxdialogex.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#endif

HANDLE hSelProc;
HWND hSelWnd;
DWORD dwMineProcId;
HANDLE hMineProc;
HWND hMineWnd;
BOOL bStop;
int m_num;
HWND g_hWnd = NULL;
struct sWindowInfo
{
	HWND hHwnd;
	HANDLE hProc;
	DWORD dwProcId;
	CString csWindowName;
	CString csClassName;
};
struct sWindowInfo m_WindowInfo[32];

const int Keys[11] = { VK_TAB, VK_F1, VK_F2, VK_F3, VK_F4, VK_F5, VK_F6, VK_F7, VK_F8,
WM_LBUTTONDOWN, WM_RBUTTONDOWN };

struct stParam
{
	int nCulSel;
	int nIndex;
};
struct stParam m_stParam;

// CKeyScriptDlg 对话框



CKeyScriptDlg::CKeyScriptDlg(CWnd* pParent /*=NULL*/)
	: CDialogEx(CKeyScriptDlg::IDD, pParent)
{
	m_hIcon = AfxGetApp()->LoadIcon(IDR_MAINFRAME);
}

void CKeyScriptDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialogEx::DoDataExchange(pDX);
	DDX_Control(pDX, IDC_COMBO_PROCESS, m_CComboBoxProcess);
	DDX_Control(pDX, IDC_COMBO_KEYS, m_CComboBoxKey);
	DDX_Control(pDX, IDC_START, m_CButtonCrtl);
}

BEGIN_MESSAGE_MAP(CKeyScriptDlg, CDialogEx)
	ON_WM_PAINT()
	ON_WM_QUERYDRAGICON()
	ON_BN_CLICKED(IDC_CLOSE, &CKeyScriptDlg::OnBnClickedClose)
	ON_BN_CLICKED(IDC_START, &CKeyScriptDlg::OnBnClickedStart)
	ON_BN_CLICKED(IDC_HIDE, &CKeyScriptDlg::OnBnClickedHide)
	ON_WM_LBUTTONDOWN()
	ON_WM_MOUSEMOVE()
	ON_WM_LBUTTONUP()
	ON_CBN_SELCHANGE(IDC_COMBO_KEYS, &CKeyScriptDlg::OnCbnSelchangeComboKeys)
END_MESSAGE_MAP()


// CKeyScriptDlg 消息处理程序

BOOL CALLBACK MyEnumWindowsProc(HWND hwnd, LPARAM lParam)
{
	// 	if (::GetWindowLong(hwnd, GWL_STYLE)& WS_VISIBLE)
	// 	{
	// 		m_WindowInfo[m_num].hHwnd = hwnd;//record the HWND handle into array
	// 		::GetWindowThreadProcessId(hwnd, &m_WindowInfo[m_num].dwProcId); //获取窗口进程ID
	// 		CWnd* pWnd = CWnd::FromHandle(hwnd);
	// 		pWnd->GetWindowText(m_WindowInfo[m_num].csWindowName);// 获取窗口名称
	// 		//m_CComboBoxProcess.AddString(m_WindowInfo[m_num].csWindowName);
	// 		m_num++;//count start
	// 	}

	CKeyScriptDlg* pView = (CKeyScriptDlg*)lParam;
	/*	CListCtrl& lc = pView->GetListCtrl();*/

	if (hwnd != pView->GetParentFrame()->GetSafeHwnd()) //不是本程序
	{
		DWORD dwStyle = GetWindowLong(hwnd, GWL_STYLE);
		if ((dwStyle & WS_OVERLAPPEDWINDOW) && (dwStyle & WS_VISIBLE))
		{
			CString csWinName;
			CWnd* pWnd = CWnd::FromHandle(hwnd);
			// 窗口标题
			pWnd->GetWindowText(csWinName);
			//if (csWinName.Find("三国") >= 0 || csWinName.Find("online") >= 0 || csWinName.Find("loader") >= 0)
			if (csWinName.Find("三国群英传") >= 0)
			{
				m_WindowInfo[m_num].hHwnd = hwnd;//record the HWND handle into array

				//获取窗口进程ID
				::GetWindowThreadProcessId(hwnd, &m_WindowInfo[m_num].dwProcId);

				//保存窗口名
				CString csProcId;
				csProcId.Format(_T(" - PID:%d"), m_WindowInfo[m_num].dwProcId);
				m_WindowInfo[m_num].csWindowName = csWinName + csProcId;

				// 窗口类名
				::GetClassName(hwnd, m_WindowInfo[m_num].csClassName.GetBuffer(256), 256);
				m_num++;
			}

		}
	}

	return TRUE;
}

BOOL CKeyScriptDlg::OnInitDialog()
{
	CDialogEx::OnInitDialog();

	// 设置此对话框的图标。  当应用程序主窗口不是对话框时，框架将自动
	//  执行此操作
	SetIcon(m_hIcon, TRUE);			// 设置大图标
	SetIcon(m_hIcon, FALSE);		// 设置小图标

	// TODO:  在此添加额外的初始化代码
	m_num = 0;
	EnumWindows(MyEnumWindowsProc, (LPARAM)this);//遍历窗口程序

	CString strName;
	for (int i = 0; i < m_num; i++)
	{
		strName.Format("%d-%s", i + 1, m_WindowInfo[i].csWindowName);
		m_CComboBoxProcess.AddString(strName);
	}

	m_CComboBoxProcess.SetCurSel(0);
	m_CComboBoxKey.SetCurSel(0);

	bStop = TRUE;

	return TRUE;  // 除非将焦点设置到控件，否则返回 TRUE
}

// 如果向对话框添加最小化按钮，则需要下面的代码
//  来绘制该图标。  对于使用文档/视图模型的 MFC 应用程序，
//  这将由框架自动完成。

void CKeyScriptDlg::OnPaint()
{
	if (IsIconic())
	{
		CPaintDC dc(this); // 用于绘制的设备上下文

		SendMessage(WM_ICONERASEBKGND, reinterpret_cast<WPARAM>(dc.GetSafeHdc()), 0);

		// 使图标在工作区矩形中居中
		int cxIcon = GetSystemMetrics(SM_CXICON);
		int cyIcon = GetSystemMetrics(SM_CYICON);
		CRect rect;
		GetClientRect(&rect);
		int x = (rect.Width() - cxIcon + 1) / 2;
		int y = (rect.Height() - cyIcon + 1) / 2;

		// 绘制图标
		dc.DrawIcon(x, y, m_hIcon);
	}
	else
	{
		CDialogEx::OnPaint();
	}
}

//当用户拖动最小化窗口时系统调用此函数取得光标
//显示。
HCURSOR CKeyScriptDlg::OnQueryDragIcon()
{
	return static_cast<HCURSOR>(m_hIcon);
}



void CKeyScriptDlg::OnBnClickedClose()
{
	// TODO:  在此添加控件通知处理程序代码

	exit(0);
}

LPARAM VKBParam(int VirtKey, int flag)
{
	if (flag == 1) // 按下
	{
		return (0x1 | (MapVirtualKey(VirtKey, MAPVK_VK_TO_VSC) << 16));
	}
	else if (flag == 0)
	{
		return (0x1 | ((KF_REPEAT | KF_UP | MapVirtualKey(VirtKey, MAPVK_VK_TO_VSC)) << 16));
	}
}

static DWORD WINAPI  DoAutoScript(void *pArg)
{
	//这里写上创建线程做什么的函数

	while (1)
	{
		if (bStop)
		{
			return 0;
		}

		::SendMessage(m_WindowInfo[m_stParam.nCulSel].hHwnd, WM_KEYDOWN, Keys[m_stParam.nIndex], VKBParam(Keys[m_stParam.nIndex], 1));
		Sleep(500);
		::SendMessage(m_WindowInfo[m_stParam.nCulSel].hHwnd, WM_KEYUP, Keys[m_stParam.nIndex], VKBParam(Keys[m_stParam.nIndex], 0));
		Sleep(500);
	}
}

void CKeyScriptDlg::OnBnClickedStart()
{
	// TODO:  在此添加控件通知处理程序代码
	if (bStop == TRUE)
	{
		m_CButtonCrtl.SetWindowText("暂停");
		bStop = FALSE;
	}
	else if (bStop == FALSE)
	{
		m_CButtonCrtl.SetWindowText("开始");
		bStop = TRUE;
		return;
	}

	int nCurSel = m_CComboBoxProcess.GetCurSel();
	hMineProc = ::OpenProcess(PROCESS_ALL_ACCESS, FALSE, m_WindowInfo[nCurSel].dwProcId); //打开进程句柄
	g_hWnd = m_WindowInfo[nCurSel].hHwnd;

	CRect rtGameWnd, rtMyWnd;
	CWnd *pWnd = CWnd::FromHandle(g_hWnd);
	pWnd->GetWindowRect(&rtGameWnd);  // 获取游戏窗口尺寸
	GetWindowRect(&rtMyWnd);		// 获取本dll的窗口尺寸


	int nIndex = m_CComboBoxKey.GetCurSel();
	if (nIndex < 0)
		return ;

	m_stParam.nCulSel = nCurSel;
	m_stParam.nIndex = nIndex;

	DWORD dwThreadID = 0;
	HANDLE hThread = CreateThread(NULL, 0, DoAutoScript, 0, NULL, &dwThreadID);	
}


void CKeyScriptDlg::OnBnClickedHide()
{
	// TODO:  在此添加控件通知处理程序代码
	this->ShowWindow(SW_HIDE);
}


void CKeyScriptDlg::OnLButtonDown(UINT nFlags, CPoint point)
{
	// TODO:  在此添加消息处理程序代码和/或调用默认值
	isMouseDown = true;
	startPoint = point;
	this->GetWindowRect(startRect);

	CDialogEx::OnLButtonDown(nFlags, point);
}


void CKeyScriptDlg::OnMouseMove(UINT nFlags, CPoint point)
{
	// TODO:  在此添加消息处理程序代码和/或调用默认值
// 	if (isMouseDown == true)
// 	{
// 		int Dx = point.x - startPoint.x;
// 		int Dy = point.y - startPoint.y;
// 		startRect.left += Dx;
// 		startRect.right += Dx;
// 		startRect.top += Dy;
// 		startRect.bottom += Dy;             //获取新的位置
// 		this->MoveWindow(&startRect);     //将窗口移到新的位置
// 	}

	CDialogEx::OnMouseMove(nFlags, point);
}


void CKeyScriptDlg::OnLButtonUp(UINT nFlags, CPoint point)
{
	// TODO:  在此添加消息处理程序代码和/或调用默认值
	isMouseDown = false;

	//CDialogEx::OnLButtonUp(nFlags, point);
}


void CKeyScriptDlg::OnCbnSelchangeComboKeys()
{
	// TODO:  在此添加控件通知处理程序代码
	int nIndex = m_CComboBoxKey.GetCurSel();
	if (nIndex < 0)
		return;

	m_stParam.nIndex = nIndex;
}
