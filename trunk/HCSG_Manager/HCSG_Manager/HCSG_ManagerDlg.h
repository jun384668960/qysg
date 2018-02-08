
// HCSG_ManagerDlg.h : 头文件
//

#pragma once
#include <afxdb.h>
#include "afxcmn.h"

#include "MyTabCtrl.h"
#include "XColorStatic.h"
#include "afxwin.h"
#include "resource.h"

// CHCSG_ManagerDlg 对话框
class CHCSG_ManagerDlg : public CDialogEx
{
// 构造
public:
	CHCSG_ManagerDlg(CWnd* pParent = NULL);	// 标准构造函数

// 对话框数据
	enum { IDD = IDD_HCSG_MANAGER_DIALOG };

	protected:
	virtual void DoDataExchange(CDataExchange* pDX);	// DDX/DDV 支持


// 实现
protected:
	HICON m_hIcon;

	// 生成的消息映射函数
	virtual BOOL OnInitDialog();
	afx_msg void OnSysCommand(UINT nID, LPARAM lParam);
	afx_msg void OnPaint();
	afx_msg HCURSOR OnQueryDragIcon();
	void InitCurConfig();
	void InitTabMain();
	void ToTray();
	afx_msg LRESULT onShowTask(WPARAM wParam,LPARAM lParam);
	DECLARE_MESSAGE_MAP()
public:
	CMyTabCtrl m_TabMain;
	CEdit m_CeditIP;
	CEdit m_CeditAcc;
	CEdit m_CeditPw;
	CXColorStatic m_CStatic_stat;
	CButton m_CButtonConct;
	afx_msg void OnBnClickedButtonConct();

	afx_msg void OnBnClickedButtonSacn();
	CEdit m_EditServerPath;
	CButton m_CButtonEnable;
	afx_msg void OnBnClickedButtonEnable();
	afx_msg void OnTimer(UINT_PTR nIDEvent);
	afx_msg void OnBnClickedButtonReset();
};
