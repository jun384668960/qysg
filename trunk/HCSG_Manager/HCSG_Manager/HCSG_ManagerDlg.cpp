
// HCSG_ManagerDlg.cpp : 实现文件
//

#include "stdafx.h"
#include "HCSG_Manager.h"
#include "HCSG_ManagerDlg.h"
#include "afxdialogex.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#endif


// 用于应用程序“关于”菜单项的 CAboutDlg 对话框

class CAboutDlg : public CDialogEx
{
public:
	CAboutDlg();

// 对话框数据
	enum { IDD = IDD_ABOUTBOX };

	protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV 支持

// 实现
protected:
	DECLARE_MESSAGE_MAP()
};

CAboutDlg::CAboutDlg() : CDialogEx(CAboutDlg::IDD)
{
}

void CAboutDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialogEx::DoDataExchange(pDX);
}

BEGIN_MESSAGE_MAP(CAboutDlg, CDialogEx)
END_MESSAGE_MAP()


// CHCSG_ManagerDlg 对话框


CHCSG_ManagerDlg::CHCSG_ManagerDlg(CWnd* pParent /*=NULL*/)
	: CDialogEx(CHCSG_ManagerDlg::IDD, pParent)
{
	m_hIcon = AfxGetApp()->LoadIcon(IDI_ICON1);
}

void CHCSG_ManagerDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialogEx::DoDataExchange(pDX);
	DDX_Control(pDX, IDC_TAB_MAIN, m_TabMain);
	DDX_Control(pDX, IDC_EDIT_IP, m_CeditIP);
	DDX_Control(pDX, IDC_EDIT_ACC, m_CeditAcc);
	DDX_Control(pDX, IDC_EDIT_PW, m_CeditPw);
	DDX_Control(pDX, IDC_STATIC_STAT, m_CStatic_stat);
	DDX_Control(pDX, IDC_BUTTON_CONCT, m_CButtonConct);
	DDX_Control(pDX, IDC_EDIT_SERVER_PATH, m_EditServerPath);
	DDX_Control(pDX, IDC_BUTTON_ENABLE, m_CButtonEnable);
}

BEGIN_MESSAGE_MAP(CHCSG_ManagerDlg, CDialogEx)
	ON_WM_SYSCOMMAND()
	ON_WM_PAINT()
	ON_WM_QUERYDRAGICON()
	ON_MESSAGE(WM_SHOWTASK,onShowTask)
	ON_BN_CLICKED(IDC_BUTTON_CONCT, &CHCSG_ManagerDlg::OnBnClickedButtonConct)
	ON_BN_CLICKED(IDC_BUTTON_SACN, &CHCSG_ManagerDlg::OnBnClickedButtonSacn)
	ON_BN_CLICKED(IDC_BUTTON_ENABLE, &CHCSG_ManagerDlg::OnBnClickedButtonEnable)
	ON_WM_TIMER()
	ON_BN_CLICKED(IDC_BUTTON_RESET, &CHCSG_ManagerDlg::OnBnClickedButtonReset)
END_MESSAGE_MAP()

//ON_NOTIFY(TCN_SELCHANGE, IDC_TAB_MAIN, &CHCSG_ManagerDlg::OnTcnSelchangeTabMain)
// CHCSG_ManagerDlg 消息处理程序

BOOL CHCSG_ManagerDlg::OnInitDialog()
{
	CDialogEx::OnInitDialog();

	// 将“关于...”菜单项添加到系统菜单中。

	// IDM_ABOUTBOX 必须在系统命令范围内。
	ASSERT((IDM_ABOUTBOX & 0xFFF0) == IDM_ABOUTBOX);
	ASSERT(IDM_ABOUTBOX < 0xF000);

	CMenu* pSysMenu = GetSystemMenu(FALSE);
	if (pSysMenu != NULL)
	{
		BOOL bNameValid;
		CString strAboutMenu;
		bNameValid = strAboutMenu.LoadString(IDS_ABOUTBOX);
		ASSERT(bNameValid);
		if (!strAboutMenu.IsEmpty())
		{
			pSysMenu->AppendMenu(MF_SEPARATOR);
			pSysMenu->AppendMenu(MF_STRING, IDM_ABOUTBOX, strAboutMenu);
		}
	}

	// 设置此对话框的图标。当应用程序主窗口不是对话框时，框架将自动
	//  执行此操作
	SetIcon(m_hIcon, TRUE);			// 设置大图标
	SetIcon(m_hIcon, FALSE);		// 设置小图标

	// TODO: 在此添加额外的初始化代码
	::CoInitialize(NULL);

	Common::ReadConfig();
	InitCurConfig();
	Common::IsEnConn = TRUE;
	Common::SystemInit();

	InitTabMain();
	Common::Log(Info, "Server Start!!");

	if(Common::IsDbConct)
	{
		m_CButtonConct.SetWindowText("断开连接");
		m_CStatic_stat.SetWindowText("已连接数据库！");
		m_CStatic_stat.SetTextColor(RGB(0,0,205));
		Common::IsDbConct = TRUE;
	}
	else
	{
		m_CButtonConct.SetWindowText("点击连接");
		m_CStatic_stat.SetWindowText("未连接数据库！");
		m_CStatic_stat.SetTextColor(RGB(255,0,0));
		Common::m_pConnection = NULL;
		Common::IsDbConct = FALSE;
	}

	//SetTimer(TIMER_UPDATE, 60 * 1000, 0);  // 每分钟自动保存一下配置

	if (IDYES == AfxMessageBox("是否允许发放奖励？" , MB_YESNO))
	{
		OnBnClickedButtonEnable();
	}
 
	return TRUE;  // 除非将焦点设置到控件，否则返回 TRUE
}

void CHCSG_ManagerDlg::OnSysCommand(UINT nID, LPARAM lParam)
{
	if ((nID & 0xFFF0) == IDM_ABOUTBOX)
	{
		CAboutDlg dlgAbout;
		dlgAbout.DoModal();
	}
	else if (nID == SC_MINIMIZE)
	{  
		ToTray();
	}
	else
	{
		CDialogEx::OnSysCommand(nID, lParam);
	}
}

// 如果向对话框添加最小化按钮，则需要下面的代码
//  来绘制该图标。对于使用文档/视图模型的 MFC 应用程序，
//  这将由框架自动完成。

void CHCSG_ManagerDlg::OnPaint()
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

void CHCSG_ManagerDlg::InitCurConfig()
{
	m_EditServerPath.SetWindowText(Common::ServerPath);
	m_CeditIP.SetWindowText(Common::SQLServer);
	m_CeditAcc.SetWindowText(Common::SQLAccount);
	m_CeditPw.SetWindowText(Common::SQLPassWord);
}

void CHCSG_ManagerDlg::InitTabMain()
{
	CRect tabRect;   // 标签控件客户区的位置和大小
	m_TabMain.InsertItem(0, _T("日志"));
	m_TabMain.InsertItem(1, _T("服务管理"));
	m_TabMain.InsertItem(2, _T("国战英雄奖励"));
	m_TabMain.InsertItem(3, _T("国战军团奖励"));
	m_TabMain.InsertItem(4, _T("赤壁奖励"));
	m_TabMain.InsertItem(5, _T("虚宝发放"));
	m_TabMain.InsertItem(6, _T("人物属性"));
	m_TabMain.InsertItem(7, _T("充值 + VIP"));
	

	Common::DlgLog = m_TabMain.InsetPage(0, IDD_DIALOG_LOG, new CLogDlg);
	Common::DlgServer = m_TabMain.InsetPage(1, IDD_DIALOG_SERVER, new CLoginDlg);
	Common::DlgCwarPlayer = m_TabMain.InsetPage(2, IDD_DIALOG_CWAR_PLAYER, new CCwarPlayerDlg);
	Common::DlgCwarOrg = m_TabMain.InsetPage(3, IDD_DIALOG_CWAR_ORG, new CCwarOrgDlg);
	Common::DlgCB = m_TabMain.InsetPage(4, IDD_DIALOG_CB, new CCBDlg);
	Common::DlgXubao = m_TabMain.InsetPage(5, IDD_DIALOG_XUBAO, new CXubaoDlg);
	Common::DlgAttr = m_TabMain.InsetPage(6, IDD_DIALOG_ATTR, new CAttrDlg);
	Common::DlgVip = m_TabMain.InsetPage(7, IDD_DIALOG_VIP, new CVip);
	

	if (IDYES == AfxMessageBox("是否需要查看背包及仓库信息？" , MB_YESNO))
	{
		m_TabMain.InsertItem(8, _T("角色列表"));  // 帐号一多会卡死服务器  移交到本地工具中，由技术查验
		m_TabMain.InsertItem(9, _T("物品存档"));
		m_TabMain.InsertItem(10, _T("仓库存档"));
		Common::DlgAttrList = m_TabMain.InsetPage(8, IDD_DIALOG_PLAYERLIST, new CPlayerListDlg);
		Common::DlgItems = m_TabMain.InsetPage(9, IDD_DIALOG_ITEMS, new CItemDlg);
		Common::DlgStore = m_TabMain.InsetPage(10, IDD_DIALOG_STORE, new CStoreDlg);
		m_TabMain.SetNumberOfPages(11);
	}
	else
	{
		m_TabMain.SetNumberOfPages(8);		
	}

	m_TabMain.SetCurrentPage(0);
}

void CHCSG_ManagerDlg::ToTray()
{
	NOTIFYICONDATA nid;

	nid.cbSize = (DWORD)sizeof(NOTIFYICONDATA); 
	nid.hWnd = this->m_hWnd; 
	nid.uID = IDR_MAINFRAME; 
	nid.uFlags = NIF_ICON|NIF_MESSAGE|NIF_TIP ; 
	nid.uCallbackMessage = WM_SHOWTASK;//自定义的消息名称 
	nid.hIcon = LoadIcon(AfxGetInstanceHandle(),MAKEINTRESOURCE(IDR_MAINFRAME)); 
	strcpy_s(nid.szTip,_T("皇朝GM"));//信息提示条为"计划任务提醒" 
	Shell_NotifyIcon(NIM_ADD,&nid);//在托盘区添加图标 
	ShowWindow(SW_HIDE);//隐藏主窗口
}

//wParam接收的是图标的ID，而lParam接收的是鼠标的行为 
LRESULT CHCSG_ManagerDlg::onShowTask(WPARAM wParam,LPARAM lParam)
{ 
	//	if(wParam!=IDR_MAINFRAME) 
	//		return 1; 
	switch(lParam) 
	{ 
	case WM_RBUTTONUP:
		{ 
			LPPOINT lpoint = new tagPOINT; 
			::GetCursorPos(lpoint); 
			CMenu menu; 
			menu.CreatePopupMenu();
			menu.AppendMenu(MF_STRING,SW_SHOW,_T("显示窗口"));
			menu.AppendMenu(MF_STRING,WM_DESTROY,_T("退出"));
			//确定弹出式菜单的位置 
			menu.TrackPopupMenu(TPM_LEFTALIGN,lpoint->x,lpoint->y,this); 
			//资源回收 
			HMENU hmenu=menu.Detach(); 
			menu.DestroyMenu(); 
			delete lpoint; 
		} 
		break; 
	case WM_LBUTTONDOWN:
		{ 
			this->ShowWindow(SW_SHOW);
		} 
		break; 
	} 
	return 0; 
}

//当用户拖动最小化窗口时系统调用此函数取得光标
//显示。
HCURSOR CHCSG_ManagerDlg::OnQueryDragIcon()
{
	return static_cast<HCURSOR>(m_hIcon);
}


void CHCSG_ManagerDlg::OnBnClickedButtonConct()
{
	// TODO: 在此添加控件通知处理程序代码
	if(!Common::IsDbConct)
	{
		m_CeditIP.GetWindowText(Common::SQLServer);
		m_CeditAcc.GetWindowText(Common::SQLAccount);
		m_CeditPw.GetWindowText(Common::SQLPassWord);
		Common::IsEnConn = TRUE;
		Common::ConnToSQLServer();
		if(!Common::IsDbConct)
		{
			AfxMessageBox("连接数据库失败！");
			Common::IsEnConn = FALSE;
			return;
		}
		m_CButtonConct.SetWindowText("断开连接");
		m_CStatic_stat.SetWindowText("已连接数据库！");
		m_CStatic_stat.SetTextColor(RGB(0,0,205));
		Common::IsEnConn = TRUE;
		Common::SaveConfig();
		
	}
	else
	{
		Common::ConnToSQLServer();
		m_CButtonConct.SetWindowText("点击连接");
		m_CStatic_stat.SetWindowText("未连接数据库！");
		m_CStatic_stat.SetTextColor(RGB(255,0,0));
		Common::IsEnConn = FALSE;
	}
}


void CHCSG_ManagerDlg::OnBnClickedButtonSacn()
{
	// TODO: 在此添加控件通知处理程序代码
	char szPath[MAX_PATH];     //存放选择的目录路径 
	CString str;

	ZeroMemory(szPath, sizeof(szPath));   

	BROWSEINFO bi;   
	bi.hwndOwner = m_hWnd;   
	bi.pidlRoot = NULL;   
	bi.pszDisplayName = szPath;   
	//bi.lpszTitle = INFO_CHOICE_DIR;  
	bi.lpszTitle = "";
	bi.ulFlags = BIF_NEWDIALOGSTYLE | BIF_EDITBOX;  
	bi.lpfn = NULL;   
	bi.lParam = 0;   
	bi.iImage = 0;   
	bi.hwndOwner = GetSafeHwnd();//获取窗口句柄 
	//bi.pszDisplayName = (LPTSTR)buffer;//此参数如果为空，则不能显示对话框 
	//弹出选择目录对话框
	LPITEMIDLIST lp = SHBrowseForFolder(&bi);   

	if(lp && SHGetPathFromIDList(lp, szPath))   
	{
		strcat_s(szPath, "\\");
		m_EditServerPath.SetWindowText(szPath);
		Common::ServerPath = szPath;
		Common::SaveConfig();
	}
}


void CHCSG_ManagerDlg::OnBnClickedButtonEnable()
{
	// TODO: 在此添加控件通知处理程序代码
	if(Common::SanGuoServerIsRuning)
	{
		m_CButtonEnable.SetWindowText("允许奖励");
		Common::SanGuoServerIsRuning = FALSE;
	}
	else
	{
		m_CButtonEnable.SetWindowText("禁止奖励");
		Common::SanGuoServerIsRuning = TRUE;
		Common::SaveConfig();
	}
}


void CHCSG_ManagerDlg::OnTimer(UINT_PTR nIDEvent)
{
	// TODO: 在此添加消息处理程序代码和/或调用默认值
	switch(nIDEvent){
	case TIMER_UPDATE:
		{
			KillTimer(TIMER_UPDATE);
			SetTimer(TIMER_UPDATE, 60 * 1000, 0);  // 每分钟自动保存一下配置 
			//Common::SaveConfig();
			break;
		}
	default:
		break;
	}
	CDialogEx::OnTimer(nIDEvent);
}


void CHCSG_ManagerDlg::OnBnClickedButtonReset()
{
	// TODO: 在此添加控件通知处理程序代码

	Common::Log(Info, "重读服务器战场、赤壁时间配置！");
	Common::GetServerConfig();

	::SendMessage(Common::DlgCwarOrg, WM_CWARORGRESET, 0, NULL);
	::SendMessage(Common::DlgCwarPlayer, WM_CWARPLAYERRESET, 0, NULL);
	::SendMessage(Common::DlgCB, WM_CBAWARDSRESET, 0, NULL);
}
