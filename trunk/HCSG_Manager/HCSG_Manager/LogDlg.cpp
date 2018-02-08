// LogDlg.cpp : 实现文件
//

#include "stdafx.h"
#include "HCSG_Manager.h"
#include "LogDlg.h"
#include "afxdialogex.h"


// LogDlg 对话框

IMPLEMENT_DYNAMIC(CLogDlg, CDialogEx)

CLogDlg::CLogDlg(CWnd* pParent /*=NULL*/)
	: CDialogEx(CLogDlg::IDD, pParent)
{
	
}

CLogDlg::~CLogDlg()
{
}

void CLogDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialogEx::DoDataExchange(pDX);
	DDX_Control(pDX, IDC_COMBO_LOG_LEVEL, m_CComboBoxLogLevel);
	DDX_Control(pDX, IDC_EDIT_LOG_PATH, m_CEditLogPath);
	DDX_Control(pDX, IDC_RICHEDIT2_LOG, m_CRichEditCtrlLog);
}


BEGIN_MESSAGE_MAP(CLogDlg, CDialogEx)
	ON_CBN_SELCHANGE(IDC_COMBO_LOG_LEVEL, &CLogDlg::OnCbnSelchangeComboLogLevel)
	ON_MESSAGE(WM_LOGSERVER, PrintMsg)
	ON_BN_CLICKED(ID_CLEAR, &CLogDlg::OnBnClickedClear)
END_MESSAGE_MAP()


// LogDlg 消息处理程序


BOOL CLogDlg::OnInitDialog()
{
	CDialogEx::OnInitDialog();

	// TODO:  在此添加额外的初始化
	CString CurPath="";
	::GetCurrentDirectory(1024,CurPath.GetBuffer(1024));
	CurPath.ReleaseBuffer();
	m_CEditLogPath.SetWindowText(CurPath);

	LogDirPath = CurPath;

	m_CComboBoxLogLevel.SetCurSel(Common::LogLevel - 1);

	return TRUE;  // return TRUE unless you set the focus to a control
}

LRESULT CLogDlg::PrintMsg(WPARAM wParam, LPARAM lParam)
{
	CString strValue  = *((CString*)lParam);

	DoPrintMsg(strValue + "\n");	
	return 0;
}

void CLogDlg::DoPrintMsg(CString csprint)
{
	CString cstemp;
	CString csdisp;
	CString cdtime;
	CString cstfile;
	CTime m_time;
	int nStart;

	m_CRichEditCtrlLog.GetWindowText(cstemp);
	if(cstemp.GetLength() > 10240){
		csdisp = cstemp.Right(1000000);	
		nStart = 1000000;
	}
	else{
		csdisp = cstemp;
		nStart = cstemp.GetLength();
	}
	m_time = CTime::GetCurrentTime();
	cdtime = m_time.Format(_T("[%Y-%m-%d %H:%M:%S] "));
	cdtime += csprint;
	csdisp += cdtime;

	m_CRichEditCtrlLog.SetWindowText(csdisp);
	m_CRichEditCtrlLog.PostMessage(WM_VSCROLL,SB_BOTTOM,0);

	cstfile = m_time.Format(_T("%Y%m%d"));
	SaveLogs(cstfile,cdtime);
}

////////////////////////////////////////// save log //////////////////////////////////////////////////////////
void CLogDlg::SaveLogs(CString ctime, CString csmsg)
{
	FILE *fplog = NULL;
	char buffline[256];

	memset(buffline,0,256);
	sprintf_s(buffline,"%s\\Logs\\hcsg_%s.log",LogDirPath,(LPSTR)(LPCTSTR)ctime);
	fopen_s(&fplog, buffline,"a+");
	if(fplog==NULL){
		Common::Log(Error, _T("打开日志文件路径写错误!"));
	}
	else{
		fputs((LPSTR)(LPCTSTR)csmsg,fplog);
		fclose(fplog);
	}
}


void CLogDlg::OnCbnSelchangeComboLogLevel()
{
	// TODO: 在此添加控件通知处理程序代码
	Common::LogLevel = m_CComboBoxLogLevel.GetCurSel() + 1;
	Common::SaveConfig();
}


void CLogDlg::OnBnClickedClear()
{
	// TODO: 在此添加控件通知处理程序代码
	m_CRichEditCtrlLog.SetWindowText("");
}
