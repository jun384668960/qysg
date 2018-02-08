// CwarDlg.cpp : 实现文件
//

#include "stdafx.h"
#include <afxdb.h>
#include "HCSG_Manager.h"
#include "CwarPlayerDlg.h"
#include "afxdialogex.h"


CList <CwarHeroes, CwarHeroes&> LCwarHeroes;

// CCwarDlg 对话框

IMPLEMENT_DYNAMIC(CCwarPlayerDlg, CDialogEx)

CCwarPlayerDlg::CCwarPlayerDlg(CWnd* pParent /*=NULL*/)
	: CDialogEx(CCwarPlayerDlg::IDD, pParent)
{
	ExeStart = FALSE;
}

CCwarPlayerDlg::~CCwarPlayerDlg()
{
}

void CCwarPlayerDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialogEx::DoDataExchange(pDX);
	DDX_Control(pDX, IDC_LIST_ACC, m_CListCtrlAcc);
	DDX_Control(pDX, IDC_LIST_ITEM, m_CListCtrlItem);
	DDX_Control(pDX, IDC_LIST_CONFIG, m_CListCtrlConfig);
	DDX_Control(pDX, IDC_EDIT_FIND_ACC, m_CEditFindAcc);
	DDX_Control(pDX, IDC_EDIT_FIND_ITEM, m_CEditFindItem);
	DDX_Control(pDX, IDC_EDIT_DISPLAY_ACC, m_CEditDisplayAcc);
	DDX_Control(pDX, IDC_EDIT_DISPLAY_NAME, m_CEditDisplayName);
	DDX_Control(pDX, IDC_EDIT_ITEM_ID, m_CEditItemId);
	DDX_Control(pDX, IDC_EDIT_ITEM_ID2, m_CEditItemId2);
	DDX_Control(pDX, IDC_EDIT_ITEM_ID3, m_CEditItemId3);
	DDX_Control(pDX, IDC_EDIT_ITEM_ID4, m_CEditItemId4);
	DDX_Control(pDX, IDC_EDIT_ITEM_ID5, m_CEditItemId5);
	DDX_Control(pDX, IDC_EDIT_ITEM_NAME, m_CEditItemName);
	DDX_Control(pDX, IDC_EDIT_ITEM_NAME2, m_CEditItemName2);
	DDX_Control(pDX, IDC_EDIT_ITEM_NAME3, m_CEditItemName3);
	DDX_Control(pDX, IDC_EDIT_ITEM_NAME4, m_CEditItemName4);
	DDX_Control(pDX, IDC_EDIT_ITEM_NAME5, m_CEditItemName5);
	DDX_Control(pDX, IDC_EDIT_NUM, m_CEditItemNum);
	DDX_Control(pDX, IDC_EDIT_NUM2, m_CEditItemNum2);
	DDX_Control(pDX, IDC_EDIT_NUM3, m_CEditItemNum3);
	DDX_Control(pDX, IDC_EDIT_NUM4, m_CEditItemNum4);
	DDX_Control(pDX, IDC_EDIT_NUM5, m_CEditItemNum5);
	DDX_Control(pDX, IDC_COMBO_CONFIG_ID, m_CComboBoxCfgID);
	DDX_Control(pDX, IDC_EDIT_RANK1, m_CEditRank1);
	DDX_Control(pDX, IDC_EDIT_RANK2, m_CEditRank2);
	DDX_Control(pDX, IDC_EDIT_MINKILLS, m_CEditMinKills);
	DDX_Control(pDX, IDC_EDIT_MINHONORS, m_CEditMinHonors);
	DDX_Control(pDX, IDC_CHECK_MINKILLS, m_CButtonMinKills);
	DDX_Control(pDX, IDC_CHECK_MINHONORS, m_CButtonMinHonors);
	DDX_Control(pDX, IDC_STATIC_TIME, m_CStaticCbTime);
}


BEGIN_MESSAGE_MAP(CCwarPlayerDlg, CDialogEx)
	ON_BN_CLICKED(IDC_BUTTON_FIND_ACC, &CCwarPlayerDlg::OnBnClickedButtonFindAcc)
	ON_BN_CLICKED(IDC_BUTTON_FIND_NEXT_ACC, &CCwarPlayerDlg::OnBnClickedButtonFindNextAcc)
	ON_BN_CLICKED(IDC_BUTTON_FIND_ITEM, &CCwarPlayerDlg::OnBnClickedButtonFindItem)
	ON_BN_CLICKED(IDC_BUTTON_FIND_NEXT_ITEM, &CCwarPlayerDlg::OnBnClickedButtonFindNextItem)
	ON_BN_CLICKED(IDC_BUTTON_ADD_CONFIG, &CCwarPlayerDlg::OnBnClickedButtonAddConfig)
	ON_BN_CLICKED(IDC_BUTTON_DEL_CONFIG, &CCwarPlayerDlg::OnBnClickedButtonDelConfig)
	ON_BN_CLICKED(IDC_BUTTON_SAVE_CONFIG, &CCwarPlayerDlg::OnBnClickedButtonSaveConfig)
	ON_BN_CLICKED(IDC_BUTTON_LOAD_CONFIG, &CCwarPlayerDlg::OnBnClickedButtonLoadConfig)
	ON_BN_CLICKED(IDC_BUTTON_EXE, &CCwarPlayerDlg::OnBnClickedButtonExe)
	ON_BN_CLICKED(IDC_BUTTON_ADDTOSWAP, &CCwarPlayerDlg::OnBnClickedButtonAddtoswap)
	ON_BN_CLICKED(IDC_BUTTON_INIT, &CCwarPlayerDlg::OnBnClickedButtonInit)
	ON_WM_TIMER()
	ON_EN_CHANGE(IDC_EDIT_RANK1, &CCwarPlayerDlg::OnEnChangeEditRank1)
	ON_MESSAGE(WM_SAVECWARPLAYERAWARDS, DoSaveConfig)
	ON_MESSAGE(WM_CWARPLAYERRESET, DoResetConfig)
	ON_BN_CLICKED(IDC_CHECK_MINKILLS, &CCwarPlayerDlg::OnBnClickedCheckMinkills)
	ON_BN_CLICKED(IDC_CHECK_MINHONORS, &CCwarPlayerDlg::OnBnClickedCheckMinhonors)
	ON_EN_CHANGE(IDC_EDIT_MINKILLS, &CCwarPlayerDlg::OnEnChangeEditMinkills)
	ON_EN_CHANGE(IDC_EDIT_MINHONORS, &CCwarPlayerDlg::OnEnChangeEditMinhonors)
	ON_EN_CHANGE(IDC_EDIT_ITEM_ID, &CCwarPlayerDlg::OnEnChangeEditItemId)
	ON_EN_CHANGE(IDC_EDIT_ITEM_ID2, &CCwarPlayerDlg::OnEnChangeEditItemId2)
	ON_EN_CHANGE(IDC_EDIT_ITEM_ID3, &CCwarPlayerDlg::OnEnChangeEditItemId3)
	ON_EN_CHANGE(IDC_EDIT_ITEM_ID4, &CCwarPlayerDlg::OnEnChangeEditItemId4)
	ON_EN_CHANGE(IDC_EDIT_ITEM_ID5, &CCwarPlayerDlg::OnEnChangeEditItemId5)
END_MESSAGE_MAP()


// CCwarDlg 消息处理程序


BOOL CCwarPlayerDlg::OnInitDialog()
{
	CDialogEx::OnInitDialog();

	// TODO:  在此添加额外的初始化
	m_CListCtrlAcc.SetExtendedStyle(LVS_EX_FULLROWSELECT|LVS_EX_GRIDLINES|LVS_EX_HEADERDRAGDROP);
	m_CListCtrlAcc.InsertColumn(0, _T("排名"), LVCFMT_LEFT, 40);
	m_CListCtrlAcc.InsertColumn(1, _T("注册帐号"), LVCFMT_LEFT, 70);
	m_CListCtrlAcc.InsertColumn(2, _T("角色名称"), LVCFMT_LEFT, 70);//插入列
	m_CListCtrlAcc.InsertColumn(3, _T("功勋"), LVCFMT_LEFT, 40);
	m_CListCtrlAcc.InsertColumn(4, _T("杀人"), LVCFMT_LEFT, 40);

	m_CListCtrlItem.SetExtendedStyle(LVS_EX_FULLROWSELECT|LVS_EX_GRIDLINES|LVS_EX_HEADERDRAGDROP);
	m_CListCtrlItem.InsertColumn(0, _T("ID"), LVCFMT_LEFT, 40);//插入列
	m_CListCtrlItem.InsertColumn(1, _T("物品名称"), LVCFMT_LEFT, 190);

	m_CListCtrlConfig.SetExtendedStyle(LVS_EX_FULLROWSELECT|LVS_EX_GRIDLINES|LVS_EX_HEADERDRAGDROP);
	m_CListCtrlConfig.InsertColumn(0, _T("起始排名"), LVCFMT_LEFT, 60);//插入列
	m_CListCtrlConfig.InsertColumn(1, _T("结束排名"), LVCFMT_LEFT, 60);

	m_CListCtrlConfig.InsertColumn(2, _T("物品ID1"), LVCFMT_LEFT, 55);
	m_CListCtrlConfig.InsertColumn(3, _T("物品名1"), LVCFMT_LEFT, 100);  // 发放物品1
	m_CListCtrlConfig.InsertColumn(4, _T("数量1"), LVCFMT_LEFT, 45);

	m_CListCtrlConfig.InsertColumn(5, _T("物品ID2"), LVCFMT_LEFT, 55);
	m_CListCtrlConfig.InsertColumn(6, _T("物品名2"), LVCFMT_LEFT, 100);  // 发放物品2
	m_CListCtrlConfig.InsertColumn(7, _T("数量2"), LVCFMT_LEFT, 45);

	m_CListCtrlConfig.InsertColumn(8, _T("物品ID3"), LVCFMT_LEFT, 55);
	m_CListCtrlConfig.InsertColumn(9, _T("物品名3"), LVCFMT_LEFT, 100);  // 发放物品3
	m_CListCtrlConfig.InsertColumn(10, _T("数量3"), LVCFMT_LEFT, 45);

	m_CListCtrlConfig.InsertColumn(11, _T("物品ID4"), LVCFMT_LEFT, 55);
	m_CListCtrlConfig.InsertColumn(12, _T("物品名4"), LVCFMT_LEFT, 100);  // 发放物品4
	m_CListCtrlConfig.InsertColumn(13, _T("数量4"), LVCFMT_LEFT, 45);

	m_CListCtrlConfig.InsertColumn(14, _T("物品ID5"), LVCFMT_LEFT, 55);
	m_CListCtrlConfig.InsertColumn(15, _T("物品名5"), LVCFMT_LEFT, 100);  // 发放物品5
	m_CListCtrlConfig.InsertColumn(16, _T("数量5"), LVCFMT_LEFT, 45);

	m_CEditItemNum.SetWindowText("1");
	m_CEditItemNum2.SetWindowText("1");
	m_CEditItemNum3.SetWindowText("1");
	m_CEditItemNum4.SetWindowText("1");
	m_CEditItemNum5.SetWindowText("1");

	m_CEditRank1.SetWindowText("1");
	m_CEditRank2.SetWindowText("3");

	m_CEditMinKills.SetWindowText((Common::m_CWarMinKills.IsEmpty()) ? "5" : Common::m_CWarMinKills);
	m_CEditMinHonors.SetWindowText((Common::m_CWarMinHonors.IsEmpty()) ? "10" : Common::m_CWarMinHonors);
	m_CButtonMinKills.SetCheck((Common::m_CWarChkMinKills.IsEmpty()) ? FALSE : TRUE);
	m_CButtonMinHonors.SetCheck((Common::m_CWarChkMinHonors.IsEmpty()) ? FALSE : TRUE);

	m_CComboBoxCfgID.SetCurSel(0);

	SetStateStatic(FALSE);
	OnBnClickedButtonInit();

	CString strFilePath="";
	::GetCurrentDirectory(1024,strFilePath.GetBuffer(1024));
	strFilePath.ReleaseBuffer();
	strFilePath += "\\Profile\\AwardsConfig.xls";
	DoLoadConfig(strFilePath);

	OnBnClickedButtonExe();

	return TRUE;  // return TRUE unless you set the focus to a control
}


void CCwarPlayerDlg::GetHores()
{
	CString strPathName;
	int ret = 0;
	int GetValueStep = 0;
	int index;
	POSITION pos;
	struct AccAttr TmpAccAttr;
	CList <AccAttr, AccAttr&> *LCurAccAttr = &Common::LAccAttr;

	strPathName = Common::ServerPath + "Login\\cwar\\cwar_last.txt";
	CString szLine = "";
	CString strGap = _T(",");
	CStringArray strResult;

	//检查文件是否存在
	DWORD dwRe = GetFileAttributes(strPathName);
	if ( dwRe != (DWORD)-1 )
	{
		//ShellExecute(NULL, NULL, strFilePath, NULL, NULL, SW_RESTORE); 
	}
	else 
	{
		CString errormessage;
		errormessage.Format("%s文件不存在！", strPathName);
		Common::Log(Error, errormessage);
		return;
	}

	LCwarHeroes.RemoveAll();
	//打开文件
	CStdioFile StdFile;
	StdFile.Open(strPathName,CFile::modeRead | CFile::shareDenyNone | CFile::typeBinary);

	//逐行读取字符串
	while( StdFile.ReadString( szLine ) )
	{
		strResult.RemoveAll();
		szLine.Replace('\t', ' ');
		szLine.Replace(_T(" "), _T(""));
		index = szLine.Find(';');
		if(index >= 0)
			szLine.Delete(szLine.Find(';'), szLine.GetLength() - szLine.Find(';'));
		if(0 != GetValueStep && szLine.Find("[last_record]]") == 0) {
			GetValueStep = 0;
			continue;
		}
		if(0 == GetValueStep && szLine.Find("[last_record]") == 0) {
			GetValueStep++;
			continue;
		}

		if(1 == GetValueStep && szLine.Find("item=") == 0)
		{
			szLine.Replace(_T("item="), _T(""));
			int nPos = szLine.Find(strGap);
			CString strLeft = _T("");
			while(0 <= nPos)
			{
				strLeft = szLine.Left(nPos);
				if (!strLeft.IsEmpty())
					strResult.Add(strLeft);

				szLine = szLine.Right(szLine.GetLength() - nPos - 1);
				nPos = szLine.Find(strGap);
			}

			if (!szLine.IsEmpty()) {
				strResult.Add(szLine);
			}

			struct CwarHeroes TmpCwarHeroes;
			TmpCwarHeroes.Name = strResult[0];
			if(TmpCwarHeroes.Name ==  "0") continue;

			TmpCwarHeroes.Level = strResult[1];
			TmpCwarHeroes.Job = strResult[2];
			TmpCwarHeroes.Nationality = strResult[3];
			TmpCwarHeroes.Honor = strResult[4];
			TmpCwarHeroes.Kills = strResult[5];
			TmpCwarHeroes.Corps = strResult[6];

			if(LCwarHeroes.GetCount() == 0)
				LCwarHeroes.AddTail(TmpCwarHeroes);
			else
			{
				int i;
				pos = LCwarHeroes.GetHeadPosition();
				for (i=0;i < LCwarHeroes.GetCount();i++)
				{
					POSITION curPos = pos;
					CwarHeroes temp = LCwarHeroes.GetNext(pos);
					if(_ttoi(TmpCwarHeroes.Honor) > _ttoi(temp.Honor))
					{
						LCwarHeroes.InsertBefore(curPos, TmpCwarHeroes);
						break;
					}
				}
				if(i == LCwarHeroes.GetCount())
					LCwarHeroes.AddTail(TmpCwarHeroes);
			}
		}
	}

	//关闭文件
	StdFile.Close();
	
	m_CListCtrlAcc.DeleteAllItems();	

	pos = LCwarHeroes.GetHeadPosition();
	for (int i=0;i < LCwarHeroes.GetCount();i++)
	{
		struct CwarHeroes TmpCwarHeroes = LCwarHeroes.GetNext(pos);

		int nrow = m_CListCtrlAcc.GetItemCount();//取行数
		int nItem = m_CListCtrlAcc.InsertItem(nrow+1, _T(""));
		CString ForItoStr;
		ForItoStr.Format("%d", i+1);
		m_CListCtrlAcc.SetItemText(nItem, 0, ForItoStr);		
		POSITION pos2 = LCurAccAttr->GetHeadPosition();
		for (int j=0;j < LCurAccAttr->GetCount();j++)
		{
			TmpAccAttr = LCurAccAttr->GetNext(pos2);
			if(0 == strcmp(TmpAccAttr.Name, TmpCwarHeroes.Name))
				m_CListCtrlAcc.SetItemText(nItem, 1, _T(TmpAccAttr.Account));
		}
		m_CListCtrlAcc.SetItemText(nItem, 2, _T(Common::Big2GB((LPSTR)(LPCTSTR)TmpCwarHeroes.Name)));
		m_CListCtrlAcc.SetItemText(nItem, 3, TmpCwarHeroes.Honor);
		m_CListCtrlAcc.SetItemText(nItem, 4, TmpCwarHeroes.Kills);
	}
}


void CCwarPlayerDlg::SetStateStatic(BOOL IsStart)
{
	if(IsStart)
	{
		CString strResult;
		strResult = "国战进行中！";
		m_CStaticCbTime.SetWindowText(strResult);
		CFont* pFont=GetFont();
		LOGFONT logFont={0};
		pFont->GetObject(sizeof(LOGFONT),&logFont);
		logFont.lfHeight=30; //这里设置字体大小
		m_CStaticCbTime.SetBold(TRUE);
		m_CStaticCbTime.SetFont(&logFont, 0);
		m_CStaticCbTime.SetTextColor(RGB(0,205,0));
	}
	else
	{
		CString strResult;
		strResult = "国战未开始！";
		m_CStaticCbTime.SetWindowText(strResult);
		CFont* pFont=GetFont();
		LOGFONT logFont={0};
		pFont->GetObject(sizeof(LOGFONT),&logFont);
		logFont.lfHeight=36; //这里设置字体大小
		m_CStaticCbTime.SetBold(TRUE);
		m_CStaticCbTime.SetFont(&logFont, 0);
		m_CStaticCbTime.SetTextColor(RGB(255,0,0));
	}
}


void CCwarPlayerDlg::OnBnClickedButtonFindAcc()
{
	// TODO: 在此添加控件通知处理程序代码
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
}


void CCwarPlayerDlg::OnBnClickedButtonFindNextAcc()
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
		for ( int i=0 ; i<iColumnNum ; i++ )
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


void CCwarPlayerDlg::OnBnClickedButtonFindItem()
{
	// TODO: 在此添加控件通知处理程序代码
	CString strKey;
	int iColumnNum,iRowCount;
	char chTemp[32];
	int ListIndex = 0;

	m_CEditFindItem.GetWindowText(strKey);
	if(strKey.IsEmpty()) return;

	iColumnNum = m_CListCtrlItem.GetHeaderCtrl()->GetItemCount();
	iRowCount = m_CListCtrlItem.GetItemCount();

	for (int j=0 ; j<iRowCount ; j++ )
	{
		memset(chTemp, 0 ,32);
		for ( int i=0 ; i<iColumnNum ; i++ )
		{
			m_CListCtrlItem.GetItemText(j,i,chTemp, 32);
			if(strstr(chTemp,strKey))
			{
				ListIndex = j;
				goto BreakLoop;
			}
		}
	}
BreakLoop:
	m_CListCtrlItem.SetFocus();  
	m_CListCtrlItem.SetItemState(ListIndex, LVIS_SELECTED, LVIS_SELECTED);
	m_CListCtrlItem.SetSelectionMark(ListIndex);
	m_CListCtrlItem.EnsureVisible(ListIndex, FALSE);
	::SendMessage(m_CListCtrlItem.m_hWnd,  LVM_SETEXTENDEDLISTVIEWSTYLE,  
		LVS_EX_FULLROWSELECT,  LVS_EX_FULLROWSELECT); 
}


void CCwarPlayerDlg::OnBnClickedButtonFindNextItem()
{
	// TODO: 在此添加控件通知处理程序代码
	CString strKey;
	int iColumnNum,iRowCount;
	char chTemp[32];
	int ListIndex = 0;
	int iCurListIndex  = m_CListCtrlItem.GetSelectionMark();

	m_CEditFindItem.GetWindowText(strKey);
	if(strKey.IsEmpty()) return;

	iColumnNum = m_CListCtrlItem.GetHeaderCtrl()->GetItemCount();
	iRowCount = m_CListCtrlItem.GetItemCount();

	int j = iCurListIndex + 1;
	do
	{
		memset(chTemp, 0 ,32);
		for ( int i=0 ; i<iColumnNum ; i++ )
		{
			m_CListCtrlItem.GetItemText(j,i,chTemp, 32);
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
	m_CListCtrlItem.SetFocus();  
	m_CListCtrlItem.SetItemState(ListIndex, LVIS_SELECTED, LVIS_SELECTED);
	m_CListCtrlItem.SetSelectionMark(ListIndex);
	m_CListCtrlItem.EnsureVisible(ListIndex, FALSE);
	::SendMessage(m_CListCtrlItem.m_hWnd,  LVM_SETEXTENDEDLISTVIEWSTYLE,  
		LVS_EX_FULLROWSELECT,  LVS_EX_FULLROWSELECT);  
}


void CCwarPlayerDlg::OnBnClickedButtonAddConfig()
{
	// TODO: 在此添加控件通知处理程序代码
	CString strRank1, strRank2;
	CString strItem, strNum, strItem2, strNum2, strItem3, strNum3, strItem4, strNum4, strItem5, strNum5;
	CString strItemName, strItemName2, strItemName3, strItemName4, strItemName5;

	// 排名
	m_CEditRank1.GetWindowTextA(strRank1);
	m_CEditRank2.GetWindowTextA(strRank2);

	//物品1
	m_CEditItemId.GetWindowTextA(strItem);
	m_CEditItemName.GetWindowTextA(strItemName);
	m_CEditItemNum.GetWindowTextA(strNum);
	//物品2
	m_CEditItemId2.GetWindowTextA(strItem2);
	m_CEditItemName2.GetWindowTextA(strItemName2);
	m_CEditItemNum2.GetWindowTextA(strNum2);
	//物品3
	m_CEditItemId3.GetWindowTextA(strItem3);
	m_CEditItemName3.GetWindowTextA(strItemName3);
	m_CEditItemNum3.GetWindowTextA(strNum3);
	//物品4
	m_CEditItemId4.GetWindowTextA(strItem4);
	m_CEditItemName4.GetWindowTextA(strItemName4);
	m_CEditItemNum4.GetWindowTextA(strNum4);
	//物品5
	m_CEditItemId5.GetWindowTextA(strItem5);
	m_CEditItemName5.GetWindowTextA(strItemName5);
	m_CEditItemNum5.GetWindowTextA(strNum5);

	if(strItem.IsEmpty()){	strItemName = ""; strNum = "";}
	if(strItem2.IsEmpty()){	strItemName2 = ""; strNum2 = "";}
	if(strItem3.IsEmpty()){	strItemName3 = ""; strNum3 = "";}
	if(strItem4.IsEmpty()){	strItemName4 = ""; strNum4 = "";}
	if(strItem5.IsEmpty()){	strItemName5 = ""; strNum5 = "";}

	if(strRank1.IsEmpty() || strRank2.IsEmpty() || (strItem.IsEmpty() && strItem2.IsEmpty() 
		&& strItem3.IsEmpty() && strItem4.IsEmpty() && strItem5.IsEmpty()))
		return;

	int nrow = m_CListCtrlConfig.GetItemCount();//取行数
	int nItem = m_CListCtrlConfig.InsertItem(nrow+1, _T(""));
	m_CListCtrlConfig.SetItemText(nItem, 0, strRank1);
	m_CListCtrlConfig.SetItemText(nItem, 1, strRank2);
	m_CListCtrlConfig.SetItemText(nItem, 2, strItem);
	m_CListCtrlConfig.SetItemText(nItem, 3, strItemName);
	m_CListCtrlConfig.SetItemText(nItem, 4, strNum);
	m_CListCtrlConfig.SetItemText(nItem, 5, strItem2);
	m_CListCtrlConfig.SetItemText(nItem, 6, strItemName2);
	m_CListCtrlConfig.SetItemText(nItem, 7, strNum2);
	m_CListCtrlConfig.SetItemText(nItem, 8, strItem3);
	m_CListCtrlConfig.SetItemText(nItem, 9, strItemName3);
	m_CListCtrlConfig.SetItemText(nItem, 10, strNum3);
	m_CListCtrlConfig.SetItemText(nItem, 11, strItem4);
	m_CListCtrlConfig.SetItemText(nItem, 12, strItemName4);
	m_CListCtrlConfig.SetItemText(nItem, 13, strNum4);
	m_CListCtrlConfig.SetItemText(nItem, 14, strItem5);
	m_CListCtrlConfig.SetItemText(nItem, 15, strItemName5);
	m_CListCtrlConfig.SetItemText(nItem, 16, strNum5);

	OnBnClickedButtonSaveConfig();
}


void CCwarPlayerDlg::OnBnClickedButtonDelConfig()
{
	// TODO: 在此添加控件通知处理程序代码
	int ListIndex  = m_CListCtrlConfig.GetSelectionMark();
	if(ListIndex == -1) return;
	m_CListCtrlConfig.DeleteItem(ListIndex);//删除选中行

	OnBnClickedButtonSaveConfig();
}


void CCwarPlayerDlg::OnBnClickedButtonSaveConfig()
{
	// TODO: 在此添加控件通知处理程序代码
	if ( m_CListCtrlConfig.GetItemCount()<= 0 )
	{
		Common::Log(Info, "列表中没有记录需要保存！");
		return;
	}
	Common::SaveAwardsConfig();
}


LRESULT CCwarPlayerDlg::DoSaveConfig(WPARAM wParam, LPARAM lParam)
{
	CDatabase database;//数据库库需要包含头文件 #include <afxdb.h>
	CString sDriver = "MICROSOFT EXCEL DRIVER (*.XLS)"; // Excel驱动
	CString sSql,strInsert;
	CString strFilePath = *((CString*)lParam);
	TRY
	{
		// 创建进行存取的字符串
		sSql.Format("DRIVER={%s};DSN='';FIRSTROWHASNAMES=1;READONLY=FALSE;CREATE_DB=\"%s\";DBQ=%s",sDriver, strFilePath, strFilePath);

		// 创建数据库 (既Excel表格文件)
		if( database.OpenEx(sSql,CDatabase::noOdbcDialog) )
		{
			//获得列别框总列数
			int iColumnNum,iRowCount;
			LVCOLUMN lvCol;
			CString strColName; //用于保存列标题名称
			int i,j; //列、行循环参数

			iColumnNum = m_CListCtrlConfig.GetHeaderCtrl()->GetItemCount();
			iRowCount = m_CListCtrlConfig.GetItemCount();
			sSql = " CREATE TABLE 国战英雄奖励 ( ";
			strInsert = " INSERT INTO 国战英雄奖励 ( " ;
			//获得列标题名称
			lvCol.mask = LVCF_TEXT; //必需设置，说明LVCOLUMN变量中pszText参数有效
			lvCol.cchTextMax = 32; //必设，pszText参数所指向的字符串的大小
			lvCol.pszText = strColName.GetBuffer(32); //必设，pszText 所指向的字符串的实际存储位置。
			//以上三个参数设置后才能通过 GetColumn()函数获得列标题的名称
			for( i=0 ; i< iColumnNum ; i++ )
			{
				if ( !(m_CListCtrlConfig.GetColumn(i,&lvCol)) )
					return -1;
				if ( i<iColumnNum-1 )
				{
					sSql = sSql + lvCol.pszText + " TEXT , ";
					strInsert = strInsert + lvCol.pszText + " , ";
				}
				else
				{
					sSql = sSql + lvCol.pszText + " TEXT ) ";
					strInsert = strInsert + lvCol.pszText + " )  VALUES ( ";
				}
			}
			//创建Excel表格文件
			database.ExecuteSQL(sSql);
			//循环提取记录并插入到EXCEL中
			sSql = strInsert;
			char chTemp[33];
			for ( j=0 ; j<iRowCount ; j++ )
			{
				memset(chTemp,0,32);
				for ( i=0 ; i<iColumnNum ; i++ )
				{
					m_CListCtrlConfig.GetItemText(j,i,chTemp,33);
					if ( i < (iColumnNum-1) )
					{
						sSql = sSql + "'" + chTemp + "' , ";
					}
					else
					{
						sSql = sSql + "'" + chTemp + "' ) ";
					}
				}
				//将记录插入到表格中
				database.ExecuteSQL(sSql);
				sSql = strInsert; 
			}
		}     
		// 关闭Excel表格文件
		database.Close();
		Common::Log(Info, "保存国战英雄奖励配置成功！");
	}
	CATCH_ALL(e)
	{
		//错误类型很多，根据需要进行报错。
		Common::Log(Error, "国战英雄奖励配置保存失败。");
	}
	END_CATCH_ALL;

	return 0;
}


void CCwarPlayerDlg::OnBnClickedButtonLoadConfig()
{
	// TODO: 在此添加控件通知处理程序代码
	CFileDialog dlg( TRUE, //TRUE或FALSE。TRUE为打开文件；FALSE为保存文件
		"xls", //为缺省的扩展名
		"", //为显示在文件名组合框的编辑框的文件名，一般可选NULL 
		OFN_HIDEREADONLY|OFN_OVERWRITEPROMPT,//为对话框风格，一般为OFN_HIDEREADONLY   |   OFN_OVERWRITEPROMPT,即隐藏只读选项和覆盖已有文件前提示。 
		"Excel 文件(*.xls)|*.xls||"//为下拉列表枢中显示文件类型
		);
	dlg.m_ofn.lpstrTitle = "导入数据";

	if (dlg.DoModal() != IDOK)
		return;

	CString strFilePath;
	//获得文件路径名
	strFilePath = dlg.GetPathName();

	if(DoLoadConfig(strFilePath))
		MessageBox("配置信息成功导入系统!","导入成功");
	else
		MessageBox("配置信息导入系统失败!","导入失败");
}


BOOL CCwarPlayerDlg::DoLoadConfig(CString strFilePath)
{
	//判断文件是否已经存在，存在则打开文件
	DWORD dwRe = GetFileAttributes(strFilePath);
	if ( dwRe != (DWORD)-1 )
	{
		//ShellExecute(NULL, NULL, strFilePath, NULL, NULL, SW_RESTORE); 
	}
	else return FALSE;
	CDatabase db;//数据库库需要包含头文件 #include <afxdb.h>
	CString sDriver = "MICROSOFT EXCEL DRIVER (*.XLS)"; // Excel驱动
	CString sSql,arr[17];
	sSql.Format("DRIVER={%s};DSN='';FIRSTROWHASNAMES=1;READONLY=FALSE;CREATE_DB=\"%s\";DBQ=%s",sDriver, strFilePath, strFilePath);
	if(!db.OpenEx(sSql,CDatabase::noOdbcDialog))//连接数据源DJB．xls
	{
		//MessageBox("打开配置文件失败!","错误");
		return FALSE;
	}
	CRecordset pset(&db);
	m_CListCtrlConfig.DeleteAllItems();
	sSql.Format("SELECT 起始排名 , 结束排名 , 物品ID1 , 物品名1 , 数量1 , 物品ID2 , 物品名2 , 数量2 ,"
		" 物品ID3 , 物品名3 , 数量3 , 物品ID4 , 物品名4 , 数量4 , 物品ID5 , 物品名5 , 数量5 FROM 国战英雄奖励");
	pset.Open(CRecordset::forwardOnly,sSql,CRecordset::readOnly);
	while(!pset.IsEOF())
	{
		pset.GetFieldValue("起始排名",arr[0]);//前面字段必须与表中的相同，否则出错。
		pset.GetFieldValue("结束排名",arr[1]);

		pset.GetFieldValue("物品ID1",arr[2]);
		pset.GetFieldValue("物品名1",arr[3]);
		pset.GetFieldValue("数量1",arr[4]);

		pset.GetFieldValue("物品ID2",arr[5]);
		pset.GetFieldValue("物品名2",arr[6]);
		pset.GetFieldValue("数量2",arr[7]);

		pset.GetFieldValue("物品ID3",arr[8]);
		pset.GetFieldValue("物品名3",arr[9]);
		pset.GetFieldValue("数量3",arr[10]);

		pset.GetFieldValue("物品ID4",arr[11]);
		pset.GetFieldValue("物品名4",arr[12]);
		pset.GetFieldValue("数量4",arr[13]);

		pset.GetFieldValue("物品ID5",arr[14]);
		pset.GetFieldValue("物品名5",arr[15]);
		pset.GetFieldValue("数量5",arr[16]);

		int nItem = m_CListCtrlConfig.GetItemCount();//插入到ListCtrl中
		int count = m_CListCtrlConfig.InsertItem(nItem,arr[0]);
		m_CListCtrlConfig.SetItemText(count,1,arr[1]);

		m_CListCtrlConfig.SetItemText(count,2,arr[2]);
		m_CListCtrlConfig.SetItemText(count,3,arr[3]);
		m_CListCtrlConfig.SetItemText(count,4,arr[4]);

		m_CListCtrlConfig.SetItemText(count,5,arr[5]);
		m_CListCtrlConfig.SetItemText(count,6,arr[6]);
		m_CListCtrlConfig.SetItemText(count,7,arr[7]);

		m_CListCtrlConfig.SetItemText(count,8,arr[8]);
		m_CListCtrlConfig.SetItemText(count,9,arr[9]);
		m_CListCtrlConfig.SetItemText(count,10,arr[10]);

		m_CListCtrlConfig.SetItemText(count,11,arr[11]);
		m_CListCtrlConfig.SetItemText(count,12,arr[12]);
		m_CListCtrlConfig.SetItemText(count,13,arr[13]);

		m_CListCtrlConfig.SetItemText(count,14,arr[14]);
		m_CListCtrlConfig.SetItemText(count,15,arr[15]);
		m_CListCtrlConfig.SetItemText(count,16,arr[16]);

		pset.MoveNext();
	}
	db.Close();

	return TRUE;
}


void CCwarPlayerDlg::StartTimer(CStringArray *StartTimes)
{
	CTime CurTime = CTime::GetCurrentTime();
	int y=CurTime.GetYear(); //获取年份
	int m=CurTime.GetMonth(); //获取当前月份
	int d=CurTime.GetDay(); //获得几号
	int h=CurTime.GetHour(); //获取当前为几时
	int mm=CurTime.GetMinute(); //获取分钟

	CString strCurSystemTime;
	strCurSystemTime.Format("%02d:%02d", h, mm);

	int i;
	for(i = 0; i < StartTimes->GetCount(); i++)
	{
		if(StartTimes->GetAt(i) > strCurSystemTime)
			break;
	}
	if(i < StartTimes->GetCount())
	{
		enHandle = TRUE;
		CString strWarStartTime = StartTimes->GetAt(i);
		Common::Log(Info, "下次国战时间：" + strWarStartTime);
		int  tmp_h, tmp_mm;
		sscanf_s(strWarStartTime.GetBuffer(strWarStartTime.GetLength()),
			"%d:%d", &tmp_h, &tmp_mm); 
		CTime WarStartTime(y, m, d, tmp_h, tmp_mm, 0);
		CTimeSpan DelayTime = WarStartTime - CurTime;
		SetTimer(TIMER1, (UINT)DelayTime.GetTotalSeconds() * 1000, 0);
		//SetTimer(TIMER1, 10 * 1000, 0); // for test
	}
	else
	{
		enHandle = FALSE;
		CTime WarStartTime(y, m, d+1, 0, 0, 0);
		CTimeSpan DelayTime = WarStartTime - CurTime;
		SetTimer(TIMER1, (UINT)DelayTime.GetTotalSeconds() * 1000, 0);
		//SetTimer(TIMER1, 10 * 1000, 0); // for test
	}

}


void CCwarPlayerDlg::DoButtonExe()
{
	ServerConfigTime *CurTimeForCwar = &Common::TimeForCwar;
	CTime CurTime = CTime::GetCurrentTime();
	int DayOfWeek = CurTime.GetDayOfWeek(); //获取星期几，注意1为星期天，7为星期六

	switch (DayOfWeek)
	{
	case 1:
		{
			StartTimer(&CurTimeForCwar->SunStartTimes);
			break;
		}
	case 2:
		{
			StartTimer(&CurTimeForCwar->ModStartTimes);
			break;
		}
	case 3:
		{
			StartTimer(&CurTimeForCwar->TuseStartTimes);
			break;
		}
	case 4:
		{
			StartTimer(&CurTimeForCwar->WedStartTimes);
			break;
		}
	case 5:
		{
			StartTimer(&CurTimeForCwar->ThursStartTimes);
			break;
		}

	case 6:
		{
			StartTimer(&CurTimeForCwar->FriStartTimes);
			break;
		}
	case 7:
		{
			StartTimer(&CurTimeForCwar->SatStartTimes);
			break;
		}
	default:
		break;
	}
}


void CCwarPlayerDlg::OnBnClickedButtonExe()
{
	// TODO: 在此添加控件通知处理程序代码
	DoButtonExe();

	if(ExeStart)
	{
		GetDlgItem(IDC_BUTTON_EXE)->SetWindowText("开启自动模式");
		Common::Log(Info, "国战英雄奖励检测停止");
		ExeStart = FALSE;
	}
	else
	{
		GetDlgItem(IDC_BUTTON_EXE)->SetWindowText("停止自动模式");
		Common::Log(Info, "国战英雄奖励检测开始");
		ExeStart = TRUE;
	}
}


void CCwarPlayerDlg::OnBnClickedButtonAddtoswap()
{
	// TODO: 在此添加控件通知处理程序代码
	int ListIndex = m_CListCtrlItem.GetSelectionMark();
	if(ListIndex == -1) return;
	switch (m_CComboBoxCfgID.GetCurSel())
	{
	case 0:
		{
			m_CEditItemId.SetWindowText(m_CListCtrlItem.GetItemText(ListIndex, 0));
			m_CEditItemName.SetWindowText(m_CListCtrlItem.GetItemText(ListIndex, 1));
			break;
		}
	case 1:
		{
			m_CEditItemId2.SetWindowText(m_CListCtrlItem.GetItemText(ListIndex, 0));
			m_CEditItemName2.SetWindowText(m_CListCtrlItem.GetItemText(ListIndex, 1));
			break;
		}
	case 2:
		{
			m_CEditItemId3.SetWindowText(m_CListCtrlItem.GetItemText(ListIndex, 0));
			m_CEditItemName3.SetWindowText(m_CListCtrlItem.GetItemText(ListIndex, 1));
			break;
		}
	case 3:
		{
			m_CEditItemId4.SetWindowText(m_CListCtrlItem.GetItemText(ListIndex, 0));
			m_CEditItemName4.SetWindowText(m_CListCtrlItem.GetItemText(ListIndex, 1));
			break;
		}
	case 4:
		{
			m_CEditItemId5.SetWindowText(m_CListCtrlItem.GetItemText(ListIndex, 0));
			m_CEditItemName5.SetWindowText(m_CListCtrlItem.GetItemText(ListIndex, 1));
			break;
		}
	default:
		break;
	}
}


void CCwarPlayerDlg::OnBnClickedButtonInit()
{
	// TODO: 在此添加控件通知处理程序代码
	struct ItemDef TmpItemDef;
	CList <ItemDef, ItemDef&> *LCurItemDef = &Common::LItemDef;
	int ret = 0;
	POSITION pos;

	m_CListCtrlItem.DeleteAllItems();

	//将获得的物品显示到ClistCtrl里去
	pos = LCurItemDef->GetHeadPosition();
	for (int i=0;i < LCurItemDef->GetCount();i++)
	{
		TmpItemDef = LCurItemDef->GetNext(pos);

		int nrow = m_CListCtrlItem.GetItemCount();//取行数
		int nItem = m_CListCtrlItem.InsertItem(nrow+1, _T(""));
		m_CListCtrlItem.SetItemText(nItem, 0, _T(TmpItemDef.ID));
		CString tempName = TmpItemDef.Name;
		tempName.Replace("item_", "");
		m_CListCtrlItem.SetItemText(nItem, 1, _T(Common::Big2GB((LPSTR)(LPCTSTR)tempName)));
	}
}


void CCwarPlayerDlg::OnTimer(UINT_PTR nIDEvent)
{
	// TODO: 在此添加消息处理程序代码和/或调用默认值
	if(!enHandle)
	{
		Sleep(1000);
		DoButtonExe();
		CDialogEx::OnTimer(nIDEvent);
		return;
	}

	switch(nIDEvent){
	case TIMER1:
		{
			SetStateStatic(TRUE);
			KillTimer(TIMER1);
			SetTimer(TIMER2, _ttoi(Common::TimeForCwar.Period) * 60 * 1000, 0); 
			//SetTimer(TIMER2, 10 * 1000, 0); // for test
			Common::Log(Info, "英雄奖励检测 -- 国战开始！");
			break;
		}
	case TIMER2:
		{
			SetStateStatic(FALSE);
			KillTimer(TIMER2);
			Common::Log(Info, "英雄奖励检测 -- 国战结束！");
			SetTimer(TIMER3, 3*60*1000, 0); // 推迟3分钟发放奖励
			//SetTimer(TIMER3, 10*1000, 0); // for test
			break;
		}
	case TIMER3:
		{
			KillTimer(TIMER3);
			GetHores();
			PaymentOfAwards();
			DoButtonExe();
			break;
		}
	default:
		break;
	}

	CDialogEx::OnTimer(nIDEvent);
}


void CCwarPlayerDlg::PaymentOfAwards()
{
	if(!Common::SanGuoServerIsRuning)
		return;

	int iColumnNumAcc,iRowCountAcc;
	int iColumnNumConfig,iRowCountConfig;
	CString strAccount, strItem, strNum, strItem2, strNum2, strItem3, strNum3, strItem4, strNum4, strItem5, strNum5;
	CString strTemp, strTemp2;
	int nRanking;

	Common::Log(Info, "国战英雄奖励发放开始");

	iColumnNumAcc = m_CListCtrlAcc.GetHeaderCtrl()->GetItemCount();
	iRowCountAcc = m_CListCtrlAcc.GetItemCount();

	iColumnNumConfig = m_CListCtrlConfig.GetHeaderCtrl()->GetItemCount();
	iRowCountConfig = m_CListCtrlConfig.GetItemCount();

	for(int i = 0; i < iRowCountAcc; i++)
	{
		m_CEditMinKills.GetWindowText(Common::m_CbMinKills);
		m_CEditMinHonors.GetWindowText(Common::m_CbMinHonors);
		
		strAccount = m_CListCtrlAcc.GetItemText(i, 1); // 获得角色账户
		nRanking = _ttoi(m_CListCtrlAcc.GetItemText(i, 0)); // 获得排名

		strTemp = m_CListCtrlAcc.GetItemText(i, 3);  // 功勋值需要大于最低要求
		if(m_CButtonMinHonors.GetCheck() && _ttoi(strTemp) < _ttoi(Common::m_CbMinHonors))
			continue;

		strTemp = m_CListCtrlAcc.GetItemText(i, 4);  // 杀人数需要大于最低要求
		if(m_CButtonMinKills.GetCheck() && _ttoi(strTemp) < _ttoi(Common::m_CbMinKills))
			continue;
		
		for (int j=0 ; j<iRowCountConfig ; j++ )
		{
			strTemp = m_CListCtrlConfig.GetItemText(j,0);
			strTemp2 = m_CListCtrlConfig.GetItemText(j,1);
			if(_ttoi(strTemp) <= nRanking && _ttoi(strTemp2) >= nRanking)
			{
				strItem = m_CListCtrlConfig.GetItemText(j,2);
				strNum = m_CListCtrlConfig.GetItemText(j,4);

				strItem2 = m_CListCtrlConfig.GetItemText(j,5);
				strNum2 = m_CListCtrlConfig.GetItemText(j,7);

				strItem3 = m_CListCtrlConfig.GetItemText(j,8);
				strNum3 = m_CListCtrlConfig.GetItemText(j,10);

				strItem4 = m_CListCtrlConfig.GetItemText(j,11);
				strNum4 = m_CListCtrlConfig.GetItemText(j,13);

				strItem5 = m_CListCtrlConfig.GetItemText(j,14);
				strNum5 = m_CListCtrlConfig.GetItemText(j,16);

				if(strItem.IsEmpty())	strNum = "";
				if(strItem2.IsEmpty())	strNum2 = "";
				if(strItem3.IsEmpty())	strNum3 = "";
				if(strItem4.IsEmpty())	strNum4 = "";
				if(strItem5.IsEmpty())	strNum5 = "";

				if(strAccount.IsEmpty() || (strItem.IsEmpty() && strItem2.IsEmpty() 
					&& strItem3.IsEmpty() && strItem4.IsEmpty() && strItem5.IsEmpty()))
					continue;

				CStringArray ItemList;
				ItemList.Add(strItem);
				ItemList.Add(strNum);
				ItemList.Add(strItem2);
				ItemList.Add(strNum2);
				ItemList.Add(strItem3);
				ItemList.Add(strNum3);
				ItemList.Add(strItem4);
				ItemList.Add(strNum4);
				ItemList.Add(strItem5);
				ItemList.Add(strNum5);

				if(!Common::SendXubao(strAccount, &ItemList))
				{
					Common::Log(Error, "发放国战军团奖励失败");
				}				
			}
		}
	}
	Common::Log(Info, "国战英雄奖励发放结束");
}


void CCwarPlayerDlg::OnEnChangeEditRank1()
{
	// TODO:  如果该控件是 RICHEDIT 控件，它将不
	// 发送此通知，除非重写 CDialogEx::OnInitDialog()
	// 函数并调用 CRichEditCtrl().SetEventMask()，
	// 同时将 ENM_CHANGE 标志“或”运算到掩码中。

	// TODO:  在此添加控件通知处理程序代码
	CString StrValue, StrValue2;
	m_CEditRank1.GetWindowText(StrValue);
	m_CEditRank2.GetWindowText(StrValue2);
	if(_ttoi(StrValue) > _ttoi(StrValue2))
		m_CEditRank2.SetWindowText(StrValue);
}


void CCwarPlayerDlg::OnBnClickedCheckMinkills()
{
	// TODO: 在此添加控件通知处理程序代码
	Common::m_CWarChkMinKills = m_CButtonMinKills.GetCheck() ? "1" : "";
	Common::SaveConfig();
}


void CCwarPlayerDlg::OnBnClickedCheckMinhonors()
{
	// TODO: 在此添加控件通知处理程序代码
	Common::m_CWarChkMinHonors = m_CButtonMinHonors.GetCheck() ? "1" : "";
	Common::SaveConfig();
}


void CCwarPlayerDlg::OnEnChangeEditMinkills()
{
	// TODO:  如果该控件是 RICHEDIT 控件，它将不
	// 发送此通知，除非重写 CDialogEx::OnInitDialog()
	// 函数并调用 CRichEditCtrl().SetEventMask()，
	// 同时将 ENM_CHANGE 标志“或”运算到掩码中。

	// TODO:  在此添加控件通知处理程序代码
	m_CEditMinKills.GetWindowText(Common::m_CWarMinKills);
	Common::SaveConfig();
}


void CCwarPlayerDlg::OnEnChangeEditMinhonors()
{
	// TODO:  如果该控件是 RICHEDIT 控件，它将不
	// 发送此通知，除非重写 CDialogEx::OnInitDialog()
	// 函数并调用 CRichEditCtrl().SetEventMask()，
	// 同时将 ENM_CHANGE 标志“或”运算到掩码中。


	// TODO:  在此添加控件通知处理程序代码
	m_CEditMinHonors.GetWindowText(Common::m_CWarMinHonors);
	Common::SaveConfig();
}


void CCwarPlayerDlg::LocalConfigSave()
{
	
}


void CCwarPlayerDlg::OnEnChangeEditItemId()
{
	// TODO:  如果该控件是 RICHEDIT 控件，它将不
	// 发送此通知，除非重写 CDialogEx::OnInitDialog()
	// 函数并调用 CRichEditCtrl().SetEventMask()，
	// 同时将 ENM_CHANGE 标志“或”运算到掩码中。

	// TODO:  在此添加控件通知处理程序代码
	struct ItemDef TmpItemDef;
	CList <ItemDef, ItemDef&> *LCurItemDef = &Common::LItemDef;
	POSITION pos;
	int i;

	CString strItemId;
	m_CEditItemId.GetWindowText(strItemId);

	//确认物品是否存在
	if(!strItemId.IsEmpty())
	{
		pos = LCurItemDef->GetHeadPosition();
		for (i=0;i < LCurItemDef->GetCount();i++)
		{
			TmpItemDef = LCurItemDef->GetNext(pos);

			if(strItemId == TmpItemDef.ID)
			{
				CString tempName = TmpItemDef.Name;
				tempName.Replace("item_", "");

				m_CEditItemName.SetWindowText(Common::Big2GB((LPSTR)(LPCTSTR)tempName));
				break;
			}
		}

		if(i >= LCurItemDef->GetCount())
		{
			m_CEditItemName.SetWindowText("");
		}
	}
	else
	{
		m_CEditItemName.SetWindowText("");
	}
}


void CCwarPlayerDlg::OnEnChangeEditItemId2()
{
	// TODO:  如果该控件是 RICHEDIT 控件，它将不
	// 发送此通知，除非重写 CDialogEx::OnInitDialog()
	// 函数并调用 CRichEditCtrl().SetEventMask()，
	// 同时将 ENM_CHANGE 标志“或”运算到掩码中。

	// TODO:  在此添加控件通知处理程序代码
	struct ItemDef TmpItemDef;
	CList <ItemDef, ItemDef&> *LCurItemDef = &Common::LItemDef;
	POSITION pos;
	int i;

	CString strItemId;
	m_CEditItemId2.GetWindowText(strItemId);

	//确认物品是否存在
	if(!strItemId.IsEmpty())
	{
		pos = LCurItemDef->GetHeadPosition();
		for (i=0;i < LCurItemDef->GetCount();i++)
		{
			TmpItemDef = LCurItemDef->GetNext(pos);

			if(strItemId == TmpItemDef.ID)
			{
				CString tempName = TmpItemDef.Name;
				tempName.Replace("item_", "");

				m_CEditItemName2.SetWindowText(Common::Big2GB((LPSTR)(LPCTSTR)tempName));
				break;
			}
		}

		if(i >= LCurItemDef->GetCount())
		{
			m_CEditItemName2.SetWindowText("");
		}
	}
	else
	{
		m_CEditItemName2.SetWindowText("");
	}
}


void CCwarPlayerDlg::OnEnChangeEditItemId3()
{
	// TODO:  如果该控件是 RICHEDIT 控件，它将不
	// 发送此通知，除非重写 CDialogEx::OnInitDialog()
	// 函数并调用 CRichEditCtrl().SetEventMask()，
	// 同时将 ENM_CHANGE 标志“或”运算到掩码中。

	// TODO:  在此添加控件通知处理程序代码
	struct ItemDef TmpItemDef;
	CList <ItemDef, ItemDef&> *LCurItemDef = &Common::LItemDef;
	POSITION pos;
	int i;

	CString strItemId;
	m_CEditItemId3.GetWindowText(strItemId);

	//确认物品是否存在
	if(!strItemId.IsEmpty())
	{
		pos = LCurItemDef->GetHeadPosition();
		for (i=0;i < LCurItemDef->GetCount();i++)
		{
			TmpItemDef = LCurItemDef->GetNext(pos);

			if(strItemId == TmpItemDef.ID)
			{
				CString tempName = TmpItemDef.Name;
				tempName.Replace("item_", "");

				m_CEditItemName3.SetWindowText(Common::Big2GB((LPSTR)(LPCTSTR)tempName));
				break;
			}
		}

		if(i >= LCurItemDef->GetCount())
		{
			m_CEditItemName3.SetWindowText("");
		}
	}
	else
	{
		m_CEditItemName3.SetWindowText("");
	}
}


void CCwarPlayerDlg::OnEnChangeEditItemId4()
{
	// TODO:  如果该控件是 RICHEDIT 控件，它将不
	// 发送此通知，除非重写 CDialogEx::OnInitDialog()
	// 函数并调用 CRichEditCtrl().SetEventMask()，
	// 同时将 ENM_CHANGE 标志“或”运算到掩码中。

	// TODO:  在此添加控件通知处理程序代码
	struct ItemDef TmpItemDef;
	CList <ItemDef, ItemDef&> *LCurItemDef = &Common::LItemDef;
	POSITION pos;
	int i;

	CString strItemId;
	m_CEditItemId4.GetWindowText(strItemId);

	//确认物品是否存在
	if(!strItemId.IsEmpty())
	{
		pos = LCurItemDef->GetHeadPosition();
		for (i=0;i < LCurItemDef->GetCount();i++)
		{
			TmpItemDef = LCurItemDef->GetNext(pos);

			if(strItemId == TmpItemDef.ID)
			{
				CString tempName = TmpItemDef.Name;
				tempName.Replace("item_", "");

				m_CEditItemName4.SetWindowText(Common::Big2GB((LPSTR)(LPCTSTR)tempName));
				break;
			}
		}

		if(i >= LCurItemDef->GetCount())
		{
			m_CEditItemName4.SetWindowText("");
		}
	}
	else
	{
		m_CEditItemName4.SetWindowText("");
	}
}


void CCwarPlayerDlg::OnEnChangeEditItemId5()
{
	// TODO:  如果该控件是 RICHEDIT 控件，它将不
	// 发送此通知，除非重写 CDialogEx::OnInitDialog()
	// 函数并调用 CRichEditCtrl().SetEventMask()，
	// 同时将 ENM_CHANGE 标志“或”运算到掩码中。

	// TODO:  在此添加控件通知处理程序代码
	struct ItemDef TmpItemDef;
	CList <ItemDef, ItemDef&> *LCurItemDef = &Common::LItemDef;
	POSITION pos;
	int i;

	CString strItemId;
	m_CEditItemId5.GetWindowText(strItemId);

	//确认物品是否存在
	if(!strItemId.IsEmpty())
	{
		pos = LCurItemDef->GetHeadPosition();
		for (i=0;i < LCurItemDef->GetCount();i++)
		{
			TmpItemDef = LCurItemDef->GetNext(pos);

			if(strItemId == TmpItemDef.ID)
			{
				CString tempName = TmpItemDef.Name;
				tempName.Replace("item_", "");

				m_CEditItemName5.SetWindowText(Common::Big2GB((LPSTR)(LPCTSTR)tempName));
				break;
			}
		}

		if(i >= LCurItemDef->GetCount())
		{
			m_CEditItemName5.SetWindowText("");
		}
	}
	else
	{
		m_CEditItemName5.SetWindowText("");
	}
}


LRESULT CCwarPlayerDlg::DoResetConfig(WPARAM wParam, LPARAM lParam)
{
	KillTimer(TIMER1);
	KillTimer(TIMER2);
	KillTimer(TIMER3);

	SetStateStatic(FALSE);
	DoButtonExe();

	return 0;
}