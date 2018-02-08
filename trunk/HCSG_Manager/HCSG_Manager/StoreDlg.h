#pragma once
#include "afxwin.h"

// CStoreDlg 对话框

class CStoreDlg : public CDialogEx
{
	DECLARE_DYNAMIC(CStoreDlg)

public:
	CStoreDlg(CWnd* pParent = NULL);   // 标准构造函数
	virtual ~CStoreDlg();

// 对话框数据
	enum { IDD = IDD_DIALOG_STORE };

protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV 支持

	DECLARE_MESSAGE_MAP()
public:
	virtual BOOL OnInitDialog();
	CListCtrl m_CListCtrlAcc;
	CEdit m_CEditFindAcc;
	afx_msg void OnEnChangeEditFindAcc();
	afx_msg void OnBnClickedButtonFindNextAcc();
	afx_msg void OnBnClickedButtonUpdate();
	CStatic m_CStaticAccTotal;
	void InitItemList();
	afx_msg void OnBnClickedButtonOutput();
	afx_msg void OnTimer(UINT_PTR nIDEvent);
};
