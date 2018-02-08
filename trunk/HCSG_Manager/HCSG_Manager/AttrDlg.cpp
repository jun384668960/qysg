// AttrDlg.cpp : 实现文件
//

#include "stdafx.h"
#include "HCSG_Manager.h"
#include "AttrDlg.h"
#include "afxdialogex.h"
#include <stdio.h>
#include <stdlib.h>
#include <share.h>

CList <SoldierAttr, SoldierAttr&> LSoldierAttr;

// CAttrDlg 对话框

IMPLEMENT_DYNAMIC(CAttrDlg, CDialogEx)

	CAttrDlg::CAttrDlg(CWnd* pParent /*=NULL*/)
	: CDialogEx(CAttrDlg::IDD, pParent)
{
	nCurSelect = 0;
}

CAttrDlg::~CAttrDlg()
{
}

void CAttrDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialogEx::DoDataExchange(pDX);
	DDX_Control(pDX, IDC_LIST_ACC, m_CListCtrlAcc);
	DDX_Control(pDX, IDC_EDIT1, m_CEditAcc);
	DDX_Control(pDX, IDC_EDIT11, m_CEditName);
	DDX_Control(pDX, IDC_EDIT19, m_CEditCorps);
	DDX_Control(pDX, IDC_EDIT_OFFICER, m_CEditOfficer);
	DDX_Control(pDX, IDC_EDIT2, m_CEditLevel);
	DDX_Control(pDX, IDC_EDIT12, m_CEditExp);
	DDX_Control(pDX, IDC_EDIT20, m_CEditSkillExp);
	DDX_Control(pDX, IDC_EDIT28, m_CEditHonor);
	DDX_Control(pDX, IDC_EDIT5, m_CEditHp);
	DDX_Control(pDX, IDC_EDIT13, m_CEditMp);
	DDX_Control(pDX, IDC_EDIT21, m_CEditAnger);
	DDX_Control(pDX, IDC_EDIT29, m_CEditAngerNum);
	DDX_Control(pDX, IDC_EDIT6, m_CEditAttr_str);
	DDX_Control(pDX, IDC_EDIT14, m_CEditAttr_str_up);
	DDX_Control(pDX, IDC_EDIT22, m_CEditAttr_mind);
	DDX_Control(pDX, IDC_EDIT30, m_CEditAttr_mind_up);
	DDX_Control(pDX, IDC_EDIT7, m_CEditAttr_leader);
	DDX_Control(pDX, IDC_EDIT15, m_CEditAttr_leader_up);
	DDX_Control(pDX, IDC_EDIT_ATTR_INT, m_CEditAttr_int);
	DDX_Control(pDX, IDC_EDIT31, m_CEditAttr_int_up);
	DDX_Control(pDX, IDC_EDIT8, m_CEditAttr_con);
	DDX_Control(pDX, IDC_EDIT16, m_CEditAttr_con_up);
	DDX_Control(pDX, IDC_EDIT_ATTR_DEX, m_CEditAttr_dex);
	DDX_Control(pDX, IDC_EDIT32, m_CEditAttr_dex_up);
	DDX_Control(pDX, IDC_EDIT9, m_CEditStoreNum);
	DDX_Control(pDX, IDC_EDIT17, m_CEditPackNum);
	DDX_Control(pDX, IDC_EDIT_ATTR_NUM, m_CEditAttr_Num);
	DDX_Control(pDX, IDC_EDIT33, m_CEditGold);	
	DDX_Control(pDX, ID_Load, m_CButtonLoad);
	DDX_Control(pDX, IDC_LIST_SOLDIER, m_CListCtrlSoldier);
	DDX_Control(pDX, IDC_EDIT36, m_CEditSldName);
	DDX_Control(pDX, IDC_EDIT39, m_CEditSldType);
	DDX_Control(pDX, IDC_EDIT38, m_CEditSldLoyal);
	DDX_Control(pDX, IDC_EDIT18, m_CEditSldLevel);
	DDX_Control(pDX, IDC_EDIT10, m_CEditSldHp);
	DDX_Control(pDX, IDC_EDIT35, m_CEditSldExp);
	DDX_Control(pDX, IDC_EDIT34, m_CEditSldStr);
	DDX_Control(pDX, IDC_EDIT43, m_CEditSldInt);
	DDX_Control(pDX, IDC_EDIT37, m_CEditSldMind);
	DDX_Control(pDX, IDC_EDIT42, m_CEditSldDex);
	DDX_Control(pDX, IDC_EDIT40, m_CEditSldAttack);
	DDX_Control(pDX, IDC_EDIT41, m_CEditSldDefence);
	DDX_Control(pDX, IDC_EDIT_FIND_ACC, m_CEditFindAcc);
	DDX_Control(pDX, IDC_EDIT_COSPLAY, m_CEditCosPlay);
}


BEGIN_MESSAGE_MAP(CAttrDlg, CDialogEx)
	ON_BN_CLICKED(ID_Load, &CAttrDlg::OnBnClickedLoad)
	ON_NOTIFY(NM_CLICK, IDC_LIST_ACC, &CAttrDlg::OnNMClickListAcc)
	ON_BN_CLICKED(ID_STORE, &CAttrDlg::OnBnClickedStore)
	ON_NOTIFY(NM_CLICK, IDC_LIST_SOLDIER, &CAttrDlg::OnNMClickListSoldier)
	ON_EN_CHANGE(IDC_EDIT_FIND_ACC, &CAttrDlg::OnEnChangeEditFindAcc)
	ON_BN_CLICKED(IDC_BUTTON_FIND_NEXT_ACC, &CAttrDlg::OnBnClickedButtonFindNextAcc)
	ON_BN_CLICKED(ID_SET_COSPLAY, &CAttrDlg::OnBnClickedSetCosplay)
	ON_BN_CLICKED(ID_CLEAR_COSPLAY, &CAttrDlg::OnBnClickedClearCosplay)
END_MESSAGE_MAP()


// CAttrDlg 消息处理程序

BOOL CAttrDlg::OnInitDialog()
{
	CDialogEx::OnInitDialog();

	// TODO:  在此添加额外的初始化
	m_CListCtrlAcc.SetExtendedStyle(LVS_EX_FULLROWSELECT|LVS_EX_GRIDLINES|LVS_EX_HEADERDRAGDROP);
	m_CListCtrlAcc.InsertColumn(0, _T("序号"), LVCFMT_LEFT, 40);
	m_CListCtrlAcc.InsertColumn(1, _T("角色名称"), LVCFMT_LEFT, 80);
	m_CListCtrlAcc.InsertColumn(2, _T("注册帐号"), LVCFMT_LEFT, 90);//插入列

	m_CListCtrlSoldier.SetExtendedStyle(LVS_EX_FULLROWSELECT|LVS_EX_GRIDLINES|LVS_EX_HEADERDRAGDROP);	
	m_CListCtrlSoldier.InsertColumn(0, _T("兵位"), LVCFMT_LEFT, 40);
	m_CListCtrlSoldier.InsertColumn(1, _T("小兵名字"), LVCFMT_LEFT, 95);//插入列

	OnBnClickedLoad();

	return TRUE;  // return TRUE unless you set the focus to a control
}

void CAttrDlg::GetSoldierAttr()
{
	CString strPathName;
	struct SoldierAttr TmpSoldierAttr;
	char Head[32];
	int ret = 0;

	LSoldierAttr.RemoveAll();

	strPathName = Common::ServerPath + "DataBase\\saves\\npc.dat";
	//检查文件是否存在
	DWORD dwRe = GetFileAttributes(strPathName);
	if ( dwRe != (DWORD)-1 )
	{
		//ShellExecute(NULL, NULL, strFilePath, NULL, NULL, SW_RESTORE); 
	}
	else 
	{
		CString errormessage;
		errormessage.Format(strPathName + " 文件不存在！");
		Common::Log(Info, errormessage);
		return;
	}

	CopyFile(strPathName, ".\\Temp\\npc.dat", FALSE);

	CFile iFile(".\\Temp\\npc.dat", CFile::modeRead | CFile::modeNoTruncate | CFile::shareDenyNone);  
	CArchive iar(&iFile, CArchive::load); 

	ret = sizeof(TmpSoldierAttr);
	ret = iar.Read(Head,sizeof(Head));
	for(;ret > 0;)
	{
		ret = iar.Read(&TmpSoldierAttr,sizeof(TmpSoldierAttr));
		if(ret <= 0) break;		
		if(strlen(TmpSoldierAttr.Name) == 0)  
		{
			strcpy_s(TmpSoldierAttr.Name, "无兵");
		}
		LSoldierAttr.AddTail(TmpSoldierAttr);
	}

	iar.Close();
	iFile.Close();
}

void CAttrDlg::OnBnClickedLoad()
{
	// TODO: 在此添加控件通知处理程序代码
	CString strPathName;
	struct AccAttr TmpAccAtrr;
	CList <AccAttr, AccAttr&> *LCurAccAttr = &Common::LAccAttr;
	int ret = 0;

	Common::GetAccAttr();
	GetSoldierAttr();

	m_CListCtrlAcc.DeleteAllItems();

	POSITION pos = LCurAccAttr->GetHeadPosition();
	for (int i=0;i < LCurAccAttr->GetCount();i++)
	{
		TmpAccAtrr = LCurAccAttr->GetNext(pos);

		int nrow = m_CListCtrlAcc.GetItemCount();//取行数
		int nItem = m_CListCtrlAcc.InsertItem(nrow+1, _T(""));
		m_CListCtrlAcc.SetItemText(nItem, 0, Common::convert(TmpAccAtrr.nIndex));
		m_CListCtrlAcc.SetItemText(nItem, 1, _T(Common::Big2GB(TmpAccAtrr.Name)));
		m_CListCtrlAcc.SetItemText(nItem, 2, _T(TmpAccAtrr.Account));
	}

	m_CButtonLoad.SetWindowText("重读资料");
	pos = Common::LAccAttr.GetHeadPosition();
	if(pos != NULL)
		DisplayAttr(pos);
}

void CAttrDlg::DisplayAttr(POSITION pos, int nIndex)
{
	struct AccAttr TmpAccAtrr;
	CList <AccAttr, AccAttr&> *LCurAccAttr = &Common::LAccAttr;
	TmpAccAtrr = LCurAccAttr->GetAt(pos);

	m_CEditAcc.SetWindowText(TmpAccAtrr.Account);
	m_CEditName.SetWindowText(Common::Big2GB(TmpAccAtrr.Name));
	m_CEditCorps.SetWindowText(Common::Big2GB(TmpAccAtrr.Corps));
	m_CEditOfficer.SetWindowText(Common::convert(TmpAccAtrr.Officer));
	m_CEditLevel.SetWindowText(Common::convert(TmpAccAtrr.Level));

	m_CEditExp.SetWindowText(Common::convert(TmpAccAtrr.Exp));
	m_CEditSkillExp.SetWindowText(Common::convert(TmpAccAtrr.SkillExp));
	m_CEditHonor.SetWindowText(Common::convert(TmpAccAtrr.Honor));
	m_CEditHp.SetWindowText(Common::convert(TmpAccAtrr.Hp));
	m_CEditMp.SetWindowText(Common::convert(TmpAccAtrr.Mp));

	m_CEditAnger.SetWindowText(Common::convert(TmpAccAtrr.Anger));
	m_CEditAngerNum.SetWindowText(Common::convert(TmpAccAtrr.AngerNum));
	m_CEditAttr_str.SetWindowText(Common::convert(TmpAccAtrr.Attr_str));
	m_CEditAttr_str_up.SetWindowText(Common::convert(TmpAccAtrr.Attr_str_up));
	m_CEditAttr_mind.SetWindowText(Common::convert(TmpAccAtrr.Attr_mind));

	m_CEditAttr_mind_up.SetWindowText(Common::convert(TmpAccAtrr.Attr_mind_up));
	m_CEditAttr_leader.SetWindowText(Common::convert(TmpAccAtrr.Attr_leader));
	m_CEditAttr_leader_up.SetWindowText(Common::convert(TmpAccAtrr.Attr_leader_up));
	m_CEditAttr_int.SetWindowText(Common::convert(TmpAccAtrr.Attr_int));
	m_CEditAttr_int_up.SetWindowText(Common::convert(TmpAccAtrr.Attr_int_up));

	m_CEditAttr_con.SetWindowText(Common::convert(TmpAccAtrr.Attr_con));
	m_CEditAttr_con_up.SetWindowText(Common::convert(TmpAccAtrr.Attr_con_up));
	m_CEditAttr_dex.SetWindowText(Common::convert(TmpAccAtrr.Attr_dex));
	m_CEditAttr_dex_up.SetWindowText(Common::convert(TmpAccAtrr.Attr_dex_up));
	m_CEditStoreNum.SetWindowText(Common::convert(TmpAccAtrr.StoreNum));

	m_CEditPackNum.SetWindowText(Common::convert(TmpAccAtrr.PackNum));
	m_CEditAttr_Num.SetWindowText(Common::convert(TmpAccAtrr.Attr_Num));
	m_CEditGold.SetWindowText(Common::convert(TmpAccAtrr.Gold));

	m_CListCtrlSoldier.DeleteAllItems();
	struct SoldierAttr TmpSoldierAttr;
	POSITION pos2 = LSoldierAttr.FindIndex(nIndex*10);
	for(int i=0; i<10; i++)
	{
		TmpSoldierAttr = LSoldierAttr.GetNext(pos2);

		int nrow = m_CListCtrlSoldier.GetItemCount();//取行数
		int nItem = m_CListCtrlSoldier.InsertItem(nrow, _T(""));
		m_CListCtrlSoldier.SetItemText(nItem, 0, Common::convert(i+1));
		if(strcmp(TmpSoldierAttr.Name, "无兵") == 0)
			m_CListCtrlSoldier.SetItemText(nItem, 1, "无兵");
		else
			m_CListCtrlSoldier.SetItemText(nItem, 1, Common::Big2GB(TmpSoldierAttr.Name));
	}

	nCurSelect = TmpAccAtrr.nIndex;
}

void CAttrDlg::DisplaySoldierAttr(POSITION pos)
{
	struct SoldierAttr TmpSoldierAttr;
	TmpSoldierAttr = LSoldierAttr.GetAt(pos);

	if(strcmp(TmpSoldierAttr.Name, "无兵") == 0)
		return;

	m_CEditSldName.SetWindowText(Common::Big2GB(TmpSoldierAttr.Name));
	m_CEditSldType.SetWindowText(Common::convert(TmpSoldierAttr.Type));
	m_CEditSldLoyal.SetWindowText(Common::convert(TmpSoldierAttr.Loyal));
	m_CEditSldLevel.SetWindowText(Common::convert(TmpSoldierAttr.Level));

	m_CEditSldHp.SetWindowText(Common::convert(TmpSoldierAttr.Hp));
	m_CEditSldExp.SetWindowText(Common::convert(TmpSoldierAttr.Exp));

	m_CEditSldStr.SetWindowText(Common::convert(TmpSoldierAttr.Attr_str));
	m_CEditSldInt.SetWindowText(Common::convert(TmpSoldierAttr.Attr_int));

	m_CEditSldMind.SetWindowText(Common::convert(TmpSoldierAttr.Attr_mind));

	m_CEditSldAttack.SetWindowText(Common::convert(TmpSoldierAttr.Attack));
	m_CEditSldDefence.SetWindowText(Common::convert(TmpSoldierAttr.Defence));
}

void CAttrDlg::OnNMClickListAcc(NMHDR *pNMHDR, LRESULT *pResult)
{
	LPNMITEMACTIVATE pNMItemActivate = reinterpret_cast<LPNMITEMACTIVATE>(pNMHDR);
	// TODO: 在此添加控件通知处理程序代码
	int ListIndex = m_CListCtrlAcc.GetSelectionMark();
	CList <AccAttr, AccAttr&> *LCurAccAttr = &Common::LAccAttr;
	if(ListIndex == -1) return;
	POSITION pos = LCurAccAttr->FindIndex(ListIndex);
	if(pos != NULL)
		DisplayAttr(pos, ListIndex);

	*pResult = 0;
}

void CAttrDlg::OnBnClickedStore()
{
	// TODO: 在此添加控件通知处理程序代码

	AfxMessageBox("没事你点这个干什么？手痒吗？");
	return;


	struct AccAttr TmpAccAtrr;
	CList <AccAttr, AccAttr&> *LCurAccAttr = &Common::LAccAttr;
	int ret = 0, i;
	POSITION pos, pos2 = NULL;
	CString strValue;

	CString strPathName;
	unsigned __int32 nSeek = 0;
	
	pos = LCurAccAttr->GetHeadPosition();
	for (i=0;i < LCurAccAttr->GetCount();i++)
	{
		pos2 = pos;
		TmpAccAtrr = LCurAccAttr->GetNext(pos);

		if(nCurSelect == TmpAccAtrr.nIndex)
		{
// 			strcpy(TmpAccAtrr.Account, TmpAccAtrr.Account);   // 都是不变的
// 			strcpy(TmpAccAtrr.A1, TmpAccAtrr.A1);
// 			TmpAccAtrr.nIndex = TmpAccAtrr.nIndex;

			m_CEditOfficer.GetWindowText(strValue);
			TmpAccAtrr.Officer = _ttoi(strValue);
			m_CEditLevel.GetWindowText(strValue);
			TmpAccAtrr.Level = _ttoi(strValue);

			m_CEditExp.GetWindowText(strValue);
			TmpAccAtrr.Exp = _ttoi(strValue);
			m_CEditSkillExp.GetWindowText(strValue);
			TmpAccAtrr.SkillExp = _ttoi(strValue);
			m_CEditHonor.GetWindowText(strValue);
			TmpAccAtrr.Honor = _ttoi(strValue);
			m_CEditHp.GetWindowText(strValue);
			TmpAccAtrr.Hp = _ttoi(strValue);
			m_CEditMp.GetWindowText(strValue);
			TmpAccAtrr.Mp = _ttoi(strValue);

			m_CEditAnger.GetWindowText(strValue);
			TmpAccAtrr.Anger = _ttoi(strValue);
			m_CEditAngerNum.GetWindowText(strValue);
			TmpAccAtrr.AngerNum = _ttoi(strValue);
			m_CEditAttr_str.GetWindowText(strValue);
			TmpAccAtrr.Attr_str = _ttoi(strValue);
			m_CEditAttr_str_up.GetWindowText(strValue);
			TmpAccAtrr.Attr_str_up = _ttoi(strValue);
			m_CEditAttr_mind.GetWindowText(strValue);
			TmpAccAtrr.Attr_mind = _ttoi(strValue);

			m_CEditAttr_mind_up.GetWindowText(strValue);
			TmpAccAtrr.Attr_mind_up = _ttoi(strValue);
			m_CEditAttr_leader.GetWindowText(strValue);
			TmpAccAtrr.Attr_leader = _ttoi(strValue);
			m_CEditAttr_leader_up.GetWindowText(strValue);
			TmpAccAtrr.Attr_leader_up = _ttoi(strValue);
			m_CEditAttr_int.GetWindowText(strValue);
			TmpAccAtrr.Attr_int = _ttoi(strValue);
			m_CEditAttr_int_up.GetWindowText(strValue);
			TmpAccAtrr.Attr_int_up = _ttoi(strValue);

			m_CEditAttr_con.GetWindowText(strValue);
			TmpAccAtrr.Attr_con = _ttoi(strValue);
			m_CEditAttr_con_up.GetWindowText(strValue);
			TmpAccAtrr.Attr_con_up = _ttoi(strValue);
			m_CEditAttr_dex.GetWindowText(strValue);
			TmpAccAtrr.Attr_dex = _ttoi(strValue);
			m_CEditAttr_dex_up.GetWindowText(strValue);
			TmpAccAtrr.Attr_dex_up = _ttoi(strValue);
			m_CEditStoreNum.GetWindowText(strValue);
			TmpAccAtrr.StoreNum = _ttoi(strValue);

			m_CEditPackNum.GetWindowText(strValue);
			TmpAccAtrr.PackNum = _ttoi(strValue);
			m_CEditAttr_Num.GetWindowText(strValue);
			TmpAccAtrr.Attr_Num = _ttoi(strValue);
			m_CEditGold.GetWindowText(strValue);
			TmpAccAtrr.Gold = _ttoi(strValue);
			break;
		}
	}

	if(i >= LCurAccAttr->GetCount())
	{
		AfxMessageBox("没有找到相应的原人物属性，程序存在逻辑错误！");
		return;
	}

	strPathName = Common::ServerPath + "DataBase\\saves\\players.dat";

	//检查文件是否存在
	DWORD dwRe = GetFileAttributes(strPathName);
	if ( dwRe != (DWORD)-1 )
	{
		//ShellExecute(NULL, NULL, strFilePath, NULL, NULL, SW_RESTORE); 
	}
	else 
	{
		CString errormessage;
		errormessage.Format(strPathName + " 文件不存在！");
		Common::Log(Error, errormessage);
		return;
	}

	nSeek = 32 + (nCurSelect - 1) * sizeof(TmpAccAtrr);

	FILE *fp = NULL;
	//打开文件
	fp = _fsopen((LPSTR)(LPCTSTR)strPathName, "r+b", _SH_DENYNO);

	if(!fp)
	{
		AfxMessageBox("打开文件失败！");
		return;
	}

	fseek(fp, nSeek, SEEK_SET);

	ret = fwrite(&TmpAccAtrr, sizeof(unsigned char), sizeof(TmpAccAtrr), fp);

	if(ret < sizeof(TmpAccAtrr))
	{
		AfxMessageBox("写入失败，一旦出现这个错误，你就哭去吧！！！");
		fclose(fp);
		return;
	}

	fclose(fp);

	if(pos2 != NULL)
	{
		LCurAccAttr->InsertAfter(pos2, TmpAccAtrr);
		LCurAccAttr->RemoveAt(pos2);
		AfxMessageBox("我姑且认为已经修改成功了吧！！到底有没有生效，这看天意！");
	}	
}

void CAttrDlg::OnNMClickListSoldier(NMHDR *pNMHDR, LRESULT *pResult)
{
	LPNMITEMACTIVATE pNMItemActivate = reinterpret_cast<LPNMITEMACTIVATE>(pNMHDR);
	// TODO: 在此添加控件通知处理程序代码
	int nIndex1 = m_CListCtrlAcc.GetSelectionMark();
	int nIndex2 = m_CListCtrlSoldier.GetSelectionMark();
	CList <AccAttr, AccAttr&> *LCurAccAttr = &Common::LAccAttr;

	if(nIndex1 == -1 || nIndex2 == -1) return;
	POSITION pos = LSoldierAttr.FindIndex(nIndex1*10 + nIndex2);
	if(pos != NULL)
		DisplaySoldierAttr(pos);

	*pResult = 0;
}

void CAttrDlg::OnEnChangeEditFindAcc()
{
	// TODO:  如果该控件是 RICHEDIT 控件，它将不
	// 发送此通知，除非重写 CDialogEx::OnInitDialog()
	// 函数并调用 CRichEditCtrl().SetEventMask()，
	// 同时将 ENM_CHANGE 标志“或”运算到掩码中。

	// TODO:  在此添加控件通知处理程序代码

	CString strKey;
	int iColumnNum,iRowCount;
	char chTemp[32];
	int ListIndex = 0;

	m_CEditFindAcc.GetWindowText(strKey);
	if(strKey.IsEmpty()) return;

	iColumnNum = m_CListCtrlAcc.GetHeaderCtrl()->GetItemCount();
	iRowCount = m_CListCtrlAcc.GetItemCount();

	for (int j=0 ; j<iRowCount ; j++ )
	{
		memset(chTemp, 0 ,32);
		for ( int i=0 ; i<iColumnNum ; i++ )
		{
			m_CListCtrlAcc.GetItemText(j,i,chTemp, 32);
			if(strstr(chTemp,strKey))
			{
				ListIndex = j;
				goto BreakLoop;
			}
		}
	}
BreakLoop:
	m_CListCtrlAcc.SetFocus();  
	m_CListCtrlAcc.SetItemState(ListIndex, LVIS_SELECTED, LVIS_SELECTED);
	m_CListCtrlAcc.SetSelectionMark(ListIndex);
	m_CListCtrlAcc.EnsureVisible(ListIndex, FALSE);
	::SendMessage(m_CListCtrlAcc.m_hWnd,  LVM_SETEXTENDEDLISTVIEWSTYLE,  
		LVS_EX_FULLROWSELECT,  LVS_EX_FULLROWSELECT); 

	m_CEditFindAcc.SetFocus();
}

void CAttrDlg::OnBnClickedButtonFindNextAcc()
{
	// TODO: 在此添加控件通知处理程序代码
	CString strKey;
	int iColumnNum,iRowCount;
	char chTemp[32];
	int ListIndex = 0;
	int iCurListIndex  = m_CListCtrlAcc.GetSelectionMark();

	m_CEditFindAcc.GetWindowText(strKey);
	if(strKey.IsEmpty()) return;

	iColumnNum = m_CListCtrlAcc.GetHeaderCtrl()->GetItemCount();
	iRowCount = m_CListCtrlAcc.GetItemCount();

	int j = iCurListIndex + 1;
	do
	{
		memset(chTemp, 0 ,32);
		for ( int i=0 ; i<2 ; i++ )  // 只查找账户名和角色名
		{
			m_CListCtrlAcc.GetItemText(j,i,chTemp, 32);
			if(strstr(chTemp,strKey))
			{
				ListIndex = j;
				goto BreakLoop;
			}
		}
		if(j == iRowCount) 
			j = -1;
	}while(j != iCurListIndex && (j++ || j!=0));
BreakLoop:
	m_CListCtrlAcc.SetFocus();  
	m_CListCtrlAcc.SetItemState(ListIndex, LVIS_SELECTED, LVIS_SELECTED);
	m_CListCtrlAcc.SetSelectionMark(ListIndex);
	m_CListCtrlAcc.EnsureVisible(ListIndex, FALSE);
	::SendMessage(m_CListCtrlAcc.m_hWnd,  LVM_SETEXTENDEDLISTVIEWSTYLE,  
		LVS_EX_FULLROWSELECT,  LVS_EX_FULLROWSELECT); 
}

void CAttrDlg::OnBnClickedSetCosplay()
{
	// TODO: 在此添加控件通知处理程序代码
}


void CAttrDlg::OnBnClickedClearCosplay()
{
	// TODO: 在此添加控件通知处理程序代码
}
