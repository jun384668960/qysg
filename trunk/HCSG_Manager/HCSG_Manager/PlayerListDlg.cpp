// PlayerListDlg.cpp : 实现文件
//

#include "stdafx.h"
#include "HCSG_Manager.h"
#include "PlayerListDlg.h"
#include "afxdialogex.h"


// CPlayerListDlg 对话框

IMPLEMENT_DYNAMIC(CPlayerListDlg, CDialogEx)

CPlayerListDlg::CPlayerListDlg(CWnd* pParent /*=NULL*/)
	: CDialogEx(CPlayerListDlg::IDD, pParent)
{

}

CPlayerListDlg::~CPlayerListDlg()
{
}

void CPlayerListDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialogEx::DoDataExchange(pDX);
	DDX_Control(pDX, IDC_LIST_ACC, m_CListCtrlAcc);
	DDX_Control(pDX, IDC_EDIT_FIND_ACC, m_CEditFindAcc);
	DDX_Control(pDX, IDC_STATIC_ACC_TOTAL, m_CStaticAccTotal);
}


BEGIN_MESSAGE_MAP(CPlayerListDlg, CDialogEx)
	ON_EN_CHANGE(IDC_EDIT_FIND_ACC, &CPlayerListDlg::OnEnChangeEditFindAcc)
	ON_BN_CLICKED(IDC_BUTTON_FIND_NEXT_ACC, &CPlayerListDlg::OnBnClickedButtonFindNextAcc)
	ON_BN_CLICKED(IDC_BUTTON_UPDATE, &CPlayerListDlg::OnBnClickedButtonUpdate)
END_MESSAGE_MAP()


// CPlayerListDlg 消息处理程序



BOOL CPlayerListDlg::OnInitDialog()
{
	CDialogEx::OnInitDialog();

	// TODO:  在此添加额外的初始化

	return TRUE;  // return TRUE unless you set the focus to a control
}

void CPlayerListDlg::OnEnChangeEditFindAcc()
{
	// TODO:  如果该控件是 RICHEDIT 控件，它将不
	// 发送此通知，除非重写 CDialogEx::OnInitDialog()
	// 函数并调用 CRichEditCtrl().SetEventMask()，
	// 同时将 ENM_CHANGE 标志“或”运算到掩码中。

	// TODO:  在此添加控件通知处理程序代码
}


void CPlayerListDlg::OnBnClickedButtonFindNextAcc()
{
	// TODO: 在此添加控件通知处理程序代码
}


void CPlayerListDlg::OnBnClickedButtonUpdate()
{
	// TODO: 在此添加控件通知处理程序代码
}
