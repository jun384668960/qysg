#pragma once
#include "afxwin.h"
#include "XColorStatic.h"


// CCwarOrgDlg 对话框

class CCwarOrgDlg : public CDialogEx
{
	DECLARE_DYNAMIC(CCwarOrgDlg)

public:
	CCwarOrgDlg(CWnd* pParent = NULL);   // 标准构造函数
	virtual ~CCwarOrgDlg();

// 对话框数据
	enum { IDD = IDD_DIALOG_CWAR_ORG };

protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV 支持

public:
	BOOL ExeStart;

	DECLARE_MESSAGE_MAP()
public:
	virtual BOOL OnInitDialog();
	CListCtrl m_CListCtrlAcc;
	CListCtrl m_CListCtrlItem;	
	CListCtrl m_CListCtrlConfig;
	CEdit m_CEditFindAcc;
	CEdit m_CEditFindItem;
	CEdit m_CEditDisplayAcc;
	CEdit m_CEditDisplayName;
	CEdit m_CEditItemId;
	CEdit m_CEditItemId2;
	CEdit m_CEditItemId3;
	CEdit m_CEditItemId4;
	CEdit m_CEditItemId5;
	CEdit m_CEditItemName;
	CEdit m_CEditItemName2;
	CEdit m_CEditItemName3;
	CEdit m_CEditItemName4;
	CEdit m_CEditItemName5;
	CEdit m_CEditItemNum;
	CEdit m_CEditItemNum2;
	CEdit m_CEditItemNum3;
	CEdit m_CEditItemNum4;
	CEdit m_CEditItemNum5;
	CComboBox m_CComboBoxCfgID;
	
	BOOL IsCWarRunning;

	afx_msg void OnBnClickedButtonFindAcc();
	afx_msg void OnBnClickedButtonFindNextAcc();
	afx_msg void OnBnClickedButtonFindItem();
	afx_msg void OnBnClickedButtonFindNextItem();
	afx_msg void OnBnClickedButtonAddConfig();
	afx_msg void OnBnClickedButtonDelConfig();
	afx_msg void OnBnClickedButtonSaveConfig();
	afx_msg void OnBnClickedButtonLoadConfig();
	afx_msg void OnBnClickedButtonExe();
	afx_msg void OnBnClickedButtonAddtoswap();
	afx_msg void OnBnClickedButtonInit();

	void DoButtonExe();
	afx_msg LRESULT DoSaveConfig(WPARAM wParam, LPARAM lParam);
	BOOL DoLoadConfig(CString strFilePath);
	afx_msg LRESULT DoResetConfig(WPARAM wParam, LPARAM lParam);

	BOOL enHandle;
	void GetOrganize();
	void StartTimer(CStringArray *StartTimes);
	afx_msg void OnTimer(UINT_PTR nIDEvent);
	void SetStateStatic(BOOL IsStart);
	CXColorStatic m_CStaticCbTime;
	void PaymentOfAwards();
	void PaymentOfAwards_1();
	CComboBox m_CComboBoxStage;
	afx_msg void OnCbnEditchangeComboStage();
	CEdit m_CEditChkTime;

	void LocalConfigSave();
	afx_msg void OnEnChangeEditCheckTime();
	afx_msg void OnEnChangeEditItemId();
	afx_msg void OnEnChangeEditItemId2();
	afx_msg void OnEnChangeEditItemId3();
	afx_msg void OnEnChangeEditItemId4();
	afx_msg void OnEnChangeEditItemId5();
	CComboBox m_CComboBoxType;
};
