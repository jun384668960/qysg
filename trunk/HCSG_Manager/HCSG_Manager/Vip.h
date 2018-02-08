#pragma once
#include "afxcmn.h"
#include "afxwin.h"

// CVip 对话框

class CVip : public CDialogEx
{
	DECLARE_DYNAMIC(CVip)

public:
	CVip(CWnd* pParent = NULL);   // 标准构造函数
	virtual ~CVip();

// 对话框数据
	enum { IDD = IDD_DIALOG_VIP };

protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV 支持

	DECLARE_MESSAGE_MAP()
public:
	CListCtrl m_CListCtrlAccount;
	CListCtrl m_CListCtrlVipAcc;
	virtual BOOL OnInitDialog();
	CEdit m_CEditAcc;
	CComboBox m_CComboBoxMode;
	CEdit m_CEditPoint;
	CEdit m_CEditNote;
	CEdit m_CEditPoint1;
	CEdit m_CEditNote1;
	CComboBox m_CComboBoxVipConfig;
	CEdit m_CEditVipPoint;
	CEdit m_CEditVipAcc;
	CComboBox m_CComboBoxVip;
	afx_msg void OnBnClickedButtonUpdate();
	CStatic m_CStaticAccTotal;
	afx_msg void OnCbnSelchangeComboVipConfig();
	afx_msg void OnBnClickedSave();

	void UpdataAcc();
	afx_msg void OnNMClickListAccount(NMHDR *pNMHDR, LRESULT *pResult);
	afx_msg void OnCbnSelchangeComboMode();
	afx_msg void OnBnClickedSend();

	int nMode;
	CEdit m_CEditPoints;
	afx_msg void OnBnClickedAddVip();
	afx_msg void OnBnClickedDeleteVip();

	void ReadVipList();
	CEdit m_CEditVipDate;
	afx_msg void OnTimer(UINT_PTR nIDEvent);

	void SendPointToVip();

	CListCtrl m_CListCtrlItem;	
	afx_msg void OnEnChangeEditAcc();
	void InitItemDef();
	CEdit m_CEditVipItemId;
	CEdit m_CEditVipItemId2;
	CEdit m_CEditVipItemId3;
	CEdit m_CEditVipItemId4;
	CEdit m_CEditVipItemId5;
	CEdit m_CEditVipItemName;
	CEdit m_CEditVipItemName2;
	CEdit m_CEditVipItemName3;
	CEdit m_CEditVipItemName4;
	CEdit m_CEditVipItemName5;
	CEdit m_CEditVipItemNum;
	CEdit m_CEditVipItemNum2;
	CEdit m_CEditVipItemNum3;
	CEdit m_CEditVipItemNum4;
	CEdit m_CEditVipItemNum5;
	afx_msg void OnEnChangeEditVipItemId();
	afx_msg void OnEnChangeEditVipItemId2();
	afx_msg void OnEnChangeEditVipItemId3();
	afx_msg void OnEnChangeEditVipItemId4();
	afx_msg void OnEnChangeEditVipItemId5();
	afx_msg void OnNMClickListItem(NMHDR *pNMHDR, LRESULT *pResult);
	afx_msg void OnBnClickedButtonFindItem();
	afx_msg void OnBnClickedButtonFindNextItem();
	CEdit m_CEditFindItem;
	afx_msg void OnEnChangeEditFindAcc();
	afx_msg void OnBnClickedButtonFindNextAcc();
	CEdit m_CEditFindAcc;
	afx_msg void OnEnChangeEditFindItem();
};
