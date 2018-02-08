#pragma once
#include "afxwin.h"


// CLoginDlg 对话框

class CLoginDlg : public CDialogEx
{
	DECLARE_DYNAMIC(CLoginDlg)

public:
	CLoginDlg(CWnd* pParent = NULL);   // 标准构造函数
	virtual ~CLoginDlg();

// 对话框数据
	enum { IDD = IDD_DIALOG_SERVER };

protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV 支持

	DECLARE_MESSAGE_MAP()
public:
	CEdit m_EditServerPath;
	CEdit m_EditAcc;
	CEdit m_EditDB;
	CEdit m_EditLogin;
	CEdit m_EditVT;
	CEdit m_EditMap1;
	CEdit m_EditMap2;
	CEdit m_EditMap3;
	CEdit m_EditMap4;
	CEdit m_EditBackup1;
	CEdit m_EditBackup2;
	CEdit m_EditGate;
	CString setEditVlaue();
	void InitCurConfig();
	void UpdataAcc();
	virtual BOOL OnInitDialog();
	afx_msg void OnBnClickedStart();
	afx_msg void OnBnClickedStop();
	afx_msg void OnBnClickedExit();
	afx_msg void OnBnClickedButtonAcc();
	afx_msg void OnBnClickedButtonDb();
	afx_msg void OnBnClickedButtonLogin();
	afx_msg void OnBnClickedButtonVt();
	afx_msg void OnBnClickedButtonMap1();
	afx_msg void OnBnClickedButtonMap2();
	afx_msg void OnBnClickedButtonMap3();
	afx_msg void OnBnClickedButtonMap4();
	afx_msg void OnBnClickedButtonBak1();
	afx_msg void OnBnClickedButtonBak2();
	afx_msg void OnBnClickedButtonGate();
	afx_msg void OnNMClickListAccount(NMHDR *pNMHDR, LRESULT *pResult);
	afx_msg void OnBnClickedButtonUpdate();
	afx_msg void OnEnChangeEditFindAcc();
	afx_msg void OnBnClickedButtonFindNextAcc();

	CListCtrl m_CListCtrlAccount;
	CEdit m_CEditAcc;
	CEdit m_CEditPoints;
	CComboBox m_CComboBoxState;
	CEdit m_CEditLoginIP;
	CEdit m_CEditFindAcc;
	CEdit m_CStaticAccTotal;
	CEdit m_CEditAccPw;
	CEdit m_CEditAccPw2;
	afx_msg void OnBnClickedAddAccount();
	afx_msg void OnBnClickedAddModifyPw();
	afx_msg void OnBnClickedFreeze();
	afx_msg void OnBnClickedUnfreeze();
};
