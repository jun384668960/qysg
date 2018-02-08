#pragma once
#include "afxwin.h"
#include "afxcmn.h"


// LogDlg 对话框

class CLogDlg : public CDialogEx
{
	DECLARE_DYNAMIC(CLogDlg)

public:
	CLogDlg(CWnd* pParent = NULL);   // 标准构造函数
	virtual ~CLogDlg();

// 对话框数据
	enum { IDD = IDD_DIALOG_LOG };

protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV 支持

	DECLARE_MESSAGE_MAP()
public:
	CComboBox m_CComboBoxLogLevel;
	CEdit m_CEditLogPath;
	CRichEditCtrl m_CRichEditCtrlLog;
	virtual BOOL OnInitDialog();

	CString LogDirPath;

	LRESULT PrintMsg(WPARAM wParam, LPARAM lParam);
	void DoPrintMsg(CString csprint);
	void SaveLogs(CString ctime, CString csmsg);

	afx_msg void OnCbnSelchangeComboLogLevel();
	afx_msg void OnBnClickedClear();
};
