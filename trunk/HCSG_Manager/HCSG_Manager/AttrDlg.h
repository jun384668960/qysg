#pragma once
#include "afxcmn.h"
#include "afxwin.h"

// CAttrDlg 对话框

class CAttrDlg : public CDialogEx
{
	DECLARE_DYNAMIC(CAttrDlg)

public:
	CAttrDlg(CWnd* pParent = NULL);   // 标准构造函数
	virtual ~CAttrDlg();

// 对话框数据
	enum { IDD = IDD_DIALOG_ATTR };

protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV 支持

	DECLARE_MESSAGE_MAP()
public:
	unsigned __int32 nCurSelect;
	void GetSoldierAttr();
	afx_msg void OnBnClickedLoad();
	virtual BOOL OnInitDialog();
	CListCtrl m_CListCtrlAcc;
	CEdit m_CEditAcc;
	CEdit m_CEditName;
	CEdit m_CEditCorps;
	CEdit m_CEditOfficer;
	CEdit m_CEditLevel;
	CEdit m_CEditExp;
	CEdit m_CEditSkillExp;
	CEdit m_CEditHonor;
	CEdit m_CEditHp;
	CEdit m_CEditMp;
	CEdit m_CEditAnger;
	CEdit m_CEditAngerNum;
	CEdit m_CEditAttr_str;
	CEdit m_CEditAttr_str_up;
	CEdit m_CEditAttr_mind;
	CEdit m_CEditAttr_mind_up;
	CEdit m_CEditAttr_leader;
	CEdit m_CEditAttr_leader_up;
	CEdit m_CEditAttr_int;
	CEdit m_CEditAttr_int_up;
	CEdit m_CEditAttr_con;
	CEdit m_CEditAttr_con_up;
	CEdit m_CEditAttr_dex;
	CEdit m_CEditAttr_dex_up;
	CEdit m_CEditStoreNum;
	CEdit m_CEditPackNum;
	CEdit m_CEditAttr_Num;
	CEdit m_CEditGold;
	void DisplayAttr(POSITION pos, int nIndex=0);
	void DisplaySoldierAttr(POSITION pos);	
	afx_msg void OnNMClickListAcc(NMHDR *pNMHDR, LRESULT *pResult);
	CButton m_CButtonLoad;
	afx_msg void OnBnClickedStore();
	CListCtrl m_CListCtrlSoldier;
	afx_msg void OnNMClickListSoldier(NMHDR *pNMHDR, LRESULT *pResult);
	CEdit m_CEditSldName;
	CEdit m_CEditSldType;
	CEdit m_CEditSldLoyal;
	CEdit m_CEditSldLevel;
	CEdit m_CEditSldHp;
	CEdit m_CEditSldExp;
	CEdit m_CEditSldStr;
	CEdit m_CEditSldInt;
	CEdit m_CEditSldMind;
	CEdit m_CEditSldDex;
	CEdit m_CEditSldAttack;
	CEdit m_CEditSldDefence;
	afx_msg void OnEnChangeEditFindAcc();
	afx_msg void OnBnClickedButtonFindNextAcc();
	CEdit m_CEditFindAcc;
	CEdit m_CEditCosPlay;
	afx_msg void OnBnClickedSetCosplay();
	afx_msg void OnBnClickedClearCosplay();
};
