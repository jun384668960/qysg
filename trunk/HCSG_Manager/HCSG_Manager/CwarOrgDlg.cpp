// CwarPlayerDlg.cpp : 实现文件
//

#include "stdafx.h"
#include <afxdb.h>
#include "HCSG_Manager.h"
#include "CwarOrgDlg.h"
#include "afxdialogex.h"

CList <CwarAwards, CwarAwards&> LCwarAwards;

// CCwarPlayerDlg 对话框

IMPLEMENT_DYNAMIC(CCwarOrgDlg, CDialogEx)

	CCwarOrgDlg::CCwarOrgDlg(CWnd* pParent /*=NULL*/)
	: CDialogEx(CCwarOrgDlg::IDD, pParent)
{
	ExeStart = FALSE;
}

CCwarOrgDlg::~CCwarOrgDlg()
{
}

void CCwarOrgDlg::DoDataExchange(CDataExchange* pDX)
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
	DDX_Control(pDX, IDC_STATIC_TIME, m_CStaticCbTime);
	DDX_Control(pDX, IDC_COMBO_STAGE, m_CComboBoxStage);
	DDX_Control(pDX, IDC_EDIT_CHECK_TIME, m_CEditChkTime);
	DDX_Control(pDX, IDC_COMBO_TYPE, m_CComboBoxType);
}


BEGIN_MESSAGE_MAP(CCwarOrgDlg, CDialogEx)
	ON_BN_CLICKED(IDC_BUTTON_FIND_ACC, &CCwarOrgDlg::OnBnClickedButtonFindAcc)
	ON_BN_CLICKED(IDC_BUTTON_FIND_NEXT_ACC, &CCwarOrgDlg::OnBnClickedButtonFindNextAcc)
	ON_BN_CLICKED(IDC_BUTTON_FIND_ITEM, &CCwarOrgDlg::OnBnClickedButtonFindItem)
	ON_BN_CLICKED(IDC_BUTTON_FIND_NEXT_ITEM, &CCwarOrgDlg::OnBnClickedButtonFindNextItem)
	ON_BN_CLICKED(IDC_BUTTON_ADD_CONFIG, &CCwarOrgDlg::OnBnClickedButtonAddConfig)
	ON_BN_CLICKED(IDC_BUTTON_DEL_CONFIG, &CCwarOrgDlg::OnBnClickedButtonDelConfig)
	ON_BN_CLICKED(IDC_BUTTON_SAVE_CONFIG, &CCwarOrgDlg::OnBnClickedButtonSaveConfig)
	ON_BN_CLICKED(IDC_BUTTON_LOAD_CONFIG, &CCwarOrgDlg::OnBnClickedButtonLoadConfig)
	ON_BN_CLICKED(IDC_BUTTON_EXE, &CCwarOrgDlg::OnBnClickedButtonExe)
	ON_BN_CLICKED(IDC_BUTTON_ADDTOSWAP, &CCwarOrgDlg::OnBnClickedButtonAddtoswap)
	ON_BN_CLICKED(IDC_BUTTON_INIT, &CCwarOrgDlg::OnBnClickedButtonInit)
	ON_WM_TIMER()
	ON_CBN_EDITCHANGE(IDC_COMBO_STAGE, &CCwarOrgDlg::OnCbnEditchangeComboStage)
	ON_MESSAGE(WM_SAVECWARORGAWARDS, DoSaveConfig)
	ON_MESSAGE(WM_CWARORGRESET, DoResetConfig)
	ON_EN_CHANGE(IDC_EDIT_CHECK_TIME, &CCwarOrgDlg::OnEnChangeEditCheckTime)
	ON_EN_CHANGE(IDC_EDIT_ITEM_ID, &CCwarOrgDlg::OnEnChangeEditItemId)
	ON_EN_CHANGE(IDC_EDIT_ITEM_ID2, &CCwarOrgDlg::OnEnChangeEditItemId2)
	ON_EN_CHANGE(IDC_EDIT_ITEM_ID3, &CCwarOrgDlg::OnEnChangeEditItemId3)
	ON_EN_CHANGE(IDC_EDIT_ITEM_ID4, &CCwarOrgDlg::OnEnChangeEditItemId4)
	ON_EN_CHANGE(IDC_EDIT_ITEM_ID5, &CCwarOrgDlg::OnEnChangeEditItemId5)
END_MESSAGE_MAP()

// CCwarPlayerDlg 消息处理程序


BOOL CCwarOrgDlg::OnInitDialog()
{
	CDialogEx::OnInitDialog();

	// TODO:  在此添加额外的初始化
	m_CListCtrlAcc.SetExtendedStyle(LVS_EX_FULLROWSELECT|LVS_EX_GRIDLINES|LVS_EX_HEADERDRAGDROP);
	m_CListCtrlAcc.InsertColumn(0, _T("军团"), LVCFMT_LEFT, 60);
	m_CListCtrlAcc.InsertColumn(1, _T("团长帐号"), LVCFMT_LEFT, 60);
	m_CListCtrlAcc.InsertColumn(2, _T("团长名称"), LVCFMT_LEFT, 60);//插入列
	m_CListCtrlAcc.InsertColumn(3, _T("占领城池"), LVCFMT_LEFT, 60);
	m_CListCtrlAcc.InsertColumn(4, _T("领奖次数"), LVCFMT_LEFT, 50);

	m_CListCtrlItem.SetExtendedStyle(LVS_EX_FULLROWSELECT|LVS_EX_GRIDLINES|LVS_EX_HEADERDRAGDROP);
	m_CListCtrlItem.InsertColumn(0, _T("ID"), LVCFMT_LEFT, 40);//插入列
	m_CListCtrlItem.InsertColumn(1, _T("物品名称"), LVCFMT_LEFT, 190);

	m_CListCtrlConfig.SetExtendedStyle(LVS_EX_FULLROWSELECT|LVS_EX_GRIDLINES|LVS_EX_HEADERDRAGDROP);
	m_CListCtrlConfig.InsertColumn(0, _T("城市名"), LVCFMT_LEFT, 60);//插入列
	m_CListCtrlConfig.InsertColumn(1, _T("城市ID"), LVCFMT_LEFT, 60);
	m_CListCtrlConfig.InsertColumn(2, _T("奖励类型"), LVCFMT_LEFT, 60);

	CString strTemp;
	for(int i=0;i<5;i++)
	{
		strTemp.Format("物品ID%d", i+1);
		m_CListCtrlConfig.InsertColumn(3*i+3, strTemp, LVCFMT_LEFT, 55);
		strTemp.Format("物品名%d", i+1);
		m_CListCtrlConfig.InsertColumn(3*i+4, strTemp, LVCFMT_LEFT, 100);  // 发放物品
		strTemp.Format("数量%d", i+1);
		m_CListCtrlConfig.InsertColumn(3*i+5, strTemp, LVCFMT_LEFT, 45);
	}

	m_CEditItemNum.SetWindowText("1");
	m_CEditItemNum2.SetWindowText("1");
	m_CEditItemNum3.SetWindowText("1");
	m_CEditItemNum4.SetWindowText("1");
	m_CEditItemNum5.SetWindowText("1");

	m_CComboBoxCfgID.SetCurSel(0);

	m_CListCtrlAcc.DeleteAllItems();
	try
	{
	SetStateStatic(FALSE);
	m_CEditChkTime.SetWindowText(Common::m_CWarAwardsChkTime);
	OnBnClickedButtonInit();

	CString strFilePath="";
	::GetCurrentDirectory(1024,strFilePath.GetBuffer(1024));
	strFilePath.ReleaseBuffer();
	strFilePath += "\\Profile\\AwardsConfig.xls";
	DoLoadConfig(strFilePath);

	OnBnClickedButtonExe();

	}
	catch (_com_error e)
	{
		CString errormessage;
		errormessage.Format("错误信息:%s",e.ErrorMessage());
		Common::Log(Error ,errormessage);
		return FALSE;

	}

	BOOL IsCWarRunning = FALSE;

	return TRUE;  // return TRUE unless you set the focus to a control
}


void CCwarOrgDlg::SetStateStatic(BOOL IsStart)
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


void CCwarOrgDlg::GetOrganize()
{
	CString strPathName;
	CList <AccAttr, AccAttr&> *LCurAccAttr = &Common::LAccAttr;
	CList <OrganizeAttr, OrganizeAttr&> *LCurOrganizeAttr = &Common::LOrganizeAttr;
	CList <StageDef, StageDef&> *LCurStageDef = &Common::LStageDef;
	int ret = 0;
	int i;
	POSITION pos, pos2;
	CString AwardsStages = "|";  //监视城市列表

	//获得受监视的城市列表
	int iRowCountConfig = m_CListCtrlConfig.GetItemCount();
	for(int i=0;i<iRowCountConfig;i++)
	{
		if(m_CListCtrlConfig.GetItemText(i,2) == "时间奖励")
		{
			CString strTemp = "";
			strTemp = m_CListCtrlConfig.GetItemText(i,0);
			AwardsStages += strTemp + "|";
		}		
	}

	Common::GetOrganizeAttr();

	pos = LCurOrganizeAttr->GetHeadPosition();
	for (i=0;i < LCurOrganizeAttr->GetCount();i++)
	{
		struct OrganizeAttr TmpOrganizeAttr = LCurOrganizeAttr->GetNext(pos);  //获得当前军团的属性
		struct CwarAwards TmpCwarAwards;

		pos2 = LCurStageDef->GetHeadPosition();   // 通过地图ID获得地图名称，用来和监视城市进行对比
		for (int j=0;j < LCurStageDef->GetCount();j++)
		{
			struct StageDef TmpStageDef = LCurStageDef->GetNext(pos2);
			if(0 == strcmp(TmpStageDef.ID, Common::convert(TmpOrganizeAttr.StageId)))
			{
				TmpStageDef.Name.Replace("city_", "");
				TmpCwarAwards.StageName = Common::Big2GB((LPSTR)(LPCTSTR)TmpStageDef.Name);
			}
		}

		if(AwardsStages.Find("|" + TmpCwarAwards.StageName + "|") == -1)  //军团所占城市不在奖励范围
			continue;

		TmpCwarAwards.OrganizeName = TmpOrganizeAttr.OrganizeName;
		TmpCwarAwards.OrganizeNameOfLeaderZh = TmpOrganizeAttr.OrganizeLeaderZh;
		pos2 = LCurAccAttr->GetHeadPosition();
		for (int j=0;j < LCurAccAttr->GetCount();j++)
		{
			struct AccAttr TmpAccAttr = LCurAccAttr->GetNext(pos2);
			if(0 == strcmp(TmpAccAttr.Name, TmpOrganizeAttr.OrganizeLeaderZh))
				TmpCwarAwards.OrganizeAccountOfLeaderZh = TmpAccAttr.Account;
		}
		TmpCwarAwards.AwardsTimes = 1;

		BOOL IsFound = FALSE;
		pos2 = LCwarAwards.GetHeadPosition();
		for (int j=0;j < LCwarAwards.GetCount();j++)
		{
			POSITION tmpPos = pos2;
			struct CwarAwards CwarAwardsOld = LCwarAwards.GetNext(pos2);
			if(0 == strcmp(CwarAwardsOld.StageName, TmpCwarAwards.StageName)
				&& 0 == strcmp(CwarAwardsOld.OrganizeName, TmpCwarAwards.OrganizeName))
			{
				CwarAwardsOld.AwardsTimes += TmpCwarAwards.AwardsTimes;
				CwarAwardsOld.OrganizeNameOfLeaderZh = TmpCwarAwards.OrganizeNameOfLeaderZh;
				CwarAwardsOld.OrganizeAccountOfLeaderZh = TmpCwarAwards.OrganizeAccountOfLeaderZh;

				LCwarAwards.SetAt(tmpPos, CwarAwardsOld);
				IsFound = TRUE;
			}
		}
		if(!IsFound)
			LCwarAwards.AddTail(TmpCwarAwards);
	}

	//显示奖励列表
	m_CListCtrlAcc.DeleteAllItems();
	pos = LCwarAwards.GetHeadPosition();
	for (int i=0;i < LCwarAwards.GetCount();i++)
	{
		struct CwarAwards TmpCwarAwards = LCwarAwards.GetNext(pos);
		int nrow = m_CListCtrlAcc.GetItemCount();//取行数
		int nItem = m_CListCtrlAcc.InsertItem(nrow+1, _T(""));
		m_CListCtrlAcc.SetItemText(nItem, 0, _T(Common::Big2GB((LPSTR)(LPCTSTR)TmpCwarAwards.OrganizeName)));	
		m_CListCtrlAcc.SetItemText(nItem, 1, _T(TmpCwarAwards.OrganizeAccountOfLeaderZh));
		m_CListCtrlAcc.SetItemText(nItem, 2, _T(Common::Big2GB((LPSTR)(LPCTSTR)TmpCwarAwards.OrganizeNameOfLeaderZh)));
		m_CListCtrlAcc.SetItemText(nItem, 3, TmpCwarAwards.StageName);
		m_CListCtrlAcc.SetItemText(nItem, 4, _T(Common::convert(TmpCwarAwards.AwardsTimes)));
	}
}


void CCwarOrgDlg::OnBnClickedButtonFindAcc()
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


void CCwarOrgDlg::OnBnClickedButtonFindNextAcc()
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


void CCwarOrgDlg::OnBnClickedButtonFindItem()
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


void CCwarOrgDlg::OnBnClickedButtonFindNextItem()
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


void CCwarOrgDlg::OnBnClickedButtonAddConfig()
{
	// TODO: 在此添加控件通知处理程序代码
	CString strCityName, strCityId, strType;
	CString strItem, strNum, strItem2, strNum2, strItem3, strNum3, strItem4, strNum4, strItem5, strNum5;
	CString strItemName, strItemName2, strItemName3, strItemName4, strItemName5;

	// 城市名
	m_CComboBoxStage.GetWindowTextA(strCityName);
	// 类型
	m_CComboBoxType.GetWindowTextA(strType);

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

	//if(strCityName.IsEmpty()){	strCityId = "";}
	if(strItem.IsEmpty()){	strItemName = ""; strNum = "";}
	if(strItem2.IsEmpty()){	strItemName2 = ""; strNum2 = "";}
	if(strItem3.IsEmpty()){	strItemName3 = ""; strNum3 = "";}
	if(strItem4.IsEmpty()){	strItemName4 = ""; strNum4 = "";}
	if(strItem5.IsEmpty()){	strItemName5 = ""; strNum5 = "";}

	if(strCityName.IsEmpty() || strType.IsEmpty() || (strItem.IsEmpty() && strItem2.IsEmpty() 
		&& strItem3.IsEmpty() && strItem4.IsEmpty() && strItem5.IsEmpty()))
		return;

	int nrow = m_CListCtrlConfig.GetItemCount();//取行数
	int nItem = m_CListCtrlConfig.InsertItem(nrow+1, _T(""));
	m_CListCtrlConfig.SetItemText(nItem, 0, strCityName);
	m_CListCtrlConfig.SetItemText(nItem, 2, strType);
	m_CListCtrlConfig.SetItemText(nItem, 3, strItem);
	m_CListCtrlConfig.SetItemText(nItem, 4, strItemName);
	m_CListCtrlConfig.SetItemText(nItem, 5, strNum);
	m_CListCtrlConfig.SetItemText(nItem, 6, strItem2);
	m_CListCtrlConfig.SetItemText(nItem, 7, strItemName2);
	m_CListCtrlConfig.SetItemText(nItem, 8, strNum2);
	m_CListCtrlConfig.SetItemText(nItem, 9, strItem3);
	m_CListCtrlConfig.SetItemText(nItem, 10, strItemName3);
	m_CListCtrlConfig.SetItemText(nItem, 11, strNum3);
	m_CListCtrlConfig.SetItemText(nItem, 12, strItem4);
	m_CListCtrlConfig.SetItemText(nItem, 13, strItemName4);
	m_CListCtrlConfig.SetItemText(nItem, 14, strNum4);
	m_CListCtrlConfig.SetItemText(nItem, 15, strItem5);
	m_CListCtrlConfig.SetItemText(nItem, 16, strItemName5);
	m_CListCtrlConfig.SetItemText(nItem, 17, strNum5);

	OnBnClickedButtonSaveConfig();
}


void CCwarOrgDlg::OnBnClickedButtonDelConfig()
{
	// TODO: 在此添加控件通知处理程序代码
	int ListIndex  = m_CListCtrlConfig.GetSelectionMark();
	if(ListIndex == -1) return;
	m_CListCtrlConfig.DeleteItem(ListIndex);//删除选中行

	OnBnClickedButtonSaveConfig();
}


void CCwarOrgDlg::OnBnClickedButtonSaveConfig()
{
	// TODO: 在此添加控件通知处理程序代码
	if ( m_CListCtrlConfig.GetItemCount()<= 0 )
	{
		Common::Log(Info, "列表中没有记录需要保存！");
		return;
	}
	Common::SaveAwardsConfig();
}


LRESULT CCwarOrgDlg::DoSaveConfig(WPARAM wParam, LPARAM lParam)
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
			sSql = " CREATE TABLE 国战占城奖励 ( ";
			strInsert = " INSERT INTO 国战占城奖励 ( " ;
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
		Common::Log(Info, "保存国战占城奖励配置成功！");
	}
	CATCH_ALL(e)
	{
		//错误类型很多，根据需要进行报错。
		Common::Log(Error, "国战占城军团奖励配置保存失败。");
	}
	END_CATCH_ALL;

	return 0;
}


void CCwarOrgDlg::OnBnClickedButtonLoadConfig()
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


BOOL CCwarOrgDlg::DoLoadConfig(CString strFilePath)
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
	CString sSql,arr[18];
	sSql.Format("DRIVER={%s};DSN='';FIRSTROWHASNAMES=1;READONLY=FALSE;CREATE_DB=\"%s\";DBQ=%s",sDriver, strFilePath, strFilePath);
	if(!db.OpenEx(sSql,CDatabase::noOdbcDialog))//连接数据源DJB．xls
	{
		//MessageBox("打开配置文件失败!","错误");
		return FALSE;
	}
	//打开EXCEL表
	CRecordset pset(&db);
	m_CListCtrlConfig.DeleteAllItems();
	/* sSql = "SELECT 学号,姓名,成绩 "       
	"FROM EXCELDEMO";      */          
	// "ORDER BY 姓名";
	sSql.Format("SELECT 城市名 , 城市ID , 奖励类型 , 物品ID1 , 物品名1 , 数量1 , 物品ID2 , 物品名2 , 数量2 ,"
		" 物品ID3 , 物品名3 , 数量3 , 物品ID4 , 物品名4 , 数量4 , 物品ID5 , 物品名5 , 数量5 FROM 国战占城奖励");
	pset.Open(CRecordset::forwardOnly,sSql,CRecordset::readOnly);
	while(!pset.IsEOF())
	{
		pset.GetFieldValue("城市名",arr[0]);//前面字段必须与表中的相同，否则出错。
		pset.GetFieldValue("城市ID",arr[1]);
		pset.GetFieldValue("奖励类型",arr[2]);

		pset.GetFieldValue("物品ID1",arr[3]);
		pset.GetFieldValue("物品名1",arr[4]);
		pset.GetFieldValue("数量1",arr[5]);

		pset.GetFieldValue("物品ID2",arr[6]);
		pset.GetFieldValue("物品名2",arr[7]);
		pset.GetFieldValue("数量2",arr[8]);

		pset.GetFieldValue("物品ID3",arr[9]);
		pset.GetFieldValue("物品名3",arr[10]);
		pset.GetFieldValue("数量3",arr[11]);

		pset.GetFieldValue("物品ID4",arr[12]);
		pset.GetFieldValue("物品名4",arr[13]);
		pset.GetFieldValue("数量4",arr[14]);

		pset.GetFieldValue("物品ID5",arr[15]);
		pset.GetFieldValue("物品名5",arr[16]);
		pset.GetFieldValue("数量5",arr[17]);

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
		m_CListCtrlConfig.SetItemText(count,17,arr[17]);

		pset.MoveNext();
	}
	db.Close();
	return TRUE;
}


void CCwarOrgDlg::StartTimer(CStringArray *StartTimes)
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
		//enHandle = TRUE; // for test
		CTime WarStartTime(y, m, d+1, 0, 0, 0);
		CTimeSpan DelayTime = WarStartTime - CurTime;
		SetTimer(TIMER1, (UINT)DelayTime.GetTotalSeconds() * 1000, 0);
		//SetTimer(TIMER1, 3 * 1000, 0); // for test
	}

}


void CCwarOrgDlg::DoButtonExe()
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


void CCwarOrgDlg::OnBnClickedButtonExe()
{
	// TODO: 在此添加控件通知处理程序代码

	DoButtonExe();
	if(ExeStart)
	{
		GetDlgItem(IDC_BUTTON_EXE)->SetWindowText("开启自动模式");
		Common::Log(Info, "国战军团占城奖励检测停止");
		ExeStart = FALSE;
	}
	else
	{
		GetDlgItem(IDC_BUTTON_EXE)->SetWindowText("停止自动模式");
		Common::Log(Info, "国战军团占城奖励检测开始");
		ExeStart = TRUE;
	}
}


void CCwarOrgDlg::OnBnClickedButtonAddtoswap()
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


void CCwarOrgDlg::OnBnClickedButtonInit()
{
	// TODO: 在此添加控件通知处理程序代码
	struct ItemDef TmpItemDef;
	struct StageDef TmpStageDef;
	CList <ItemDef, ItemDef&> *LCurItemDef = &Common::LItemDef;
	CList <StageDef, StageDef&> *LCurStageDef = &Common::LStageDef;
	int ret = 0;
	POSITION pos;

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

	//初始化城市列表
	pos = LCurStageDef->GetHeadPosition();
	for (int i=0;i < LCurStageDef->GetCount();i++)
	{
		TmpStageDef = LCurStageDef->GetNext(pos);
		CString tempName = TmpStageDef.Name;
		tempName.Replace("city_", "");
		m_CComboBoxStage.AddString(_T(Common::Big2GB((LPSTR)(LPCTSTR)tempName)));
	}

	m_CComboBoxStage.SetCurSel(1);
}


void CCwarOrgDlg::OnTimer(UINT_PTR nIDEvent)
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
			IsCWarRunning = TRUE;
			SetStateStatic(TRUE);
			m_CListCtrlAcc.DeleteAllItems();
			KillTimer(TIMER1);
			SetTimer(TIMER2, _ttoi(Common::TimeForCwar.Period) * 60 * 1000 - 10*1000, 0); //提前10秒结算

			//SetTimer(TIMER2, 2 * 60 * 1000 - 10*1000, 0); // for test 

			CString strValue;
			m_CEditChkTime.GetWindowText(strValue);
			SetTimer(TIMER3, _ttoi(strValue) * 60 * 1000, 0); 
			//SetTimer(TIMER3, 30 * 1000 * 1, 0);  // for test
			LCwarAwards.RemoveAll();
			Common::Log(Info, "军团奖励检测 -- 国战开始！");
			break;
		}
	case TIMER2:
		{
			IsCWarRunning = FALSE;			
			SetStateStatic(FALSE);
			KillTimer(TIMER2);
			Common::Log(Info, "军团奖励检测 -- 国战结束！");
			SetTimer(TIMER4, 1 * 60 * 1000, 0);  // 延迟1分钟发放占城奖励
			PaymentOfAwards();
			break;
		}
	case TIMER3:
		{
			if(IsCWarRunning)
			{
				KillTimer(TIMER3);
				GetOrganize();			

				CString strValue;
				m_CEditChkTime.GetWindowText(strValue);
				SetTimer(TIMER3, _ttoi(strValue) * 60 * 1000, 0);
				//SetTimer(TIMER3, 30 * 1000 * 1, 0);  // for test
			}
			else
			{
				KillTimer(TIMER3);
			}
			break;
		}
	case TIMER4:
		{
			KillTimer(TIMER4);
			PaymentOfAwards_1();
			DoButtonExe();
			LCwarAwards.RemoveAll();

			break;
		}
	default:
		break;
	}

	CDialogEx::OnTimer(nIDEvent);
}

/* 时间奖励 */
void CCwarOrgDlg::PaymentOfAwards()
{
	if(!Common::SanGuoServerIsRuning)
		return;

	int iColumnNumAcc,iRowCountAcc;
	int iColumnNumConfig,iRowCountConfig;
	CString strAccount, strItem, strNum, strItem2, strNum2, strItem3, strNum3, strItem4, strNum4, strItem5, strNum5;

	Common::Log(Info, "国战军团<时间>奖励发放开始");
	iColumnNumAcc = m_CListCtrlAcc.GetHeaderCtrl()->GetItemCount();
	iRowCountAcc = m_CListCtrlAcc.GetItemCount();

	iColumnNumConfig = m_CListCtrlConfig.GetHeaderCtrl()->GetItemCount();
	iRowCountConfig = m_CListCtrlConfig.GetItemCount();

	for(int i = 0; i < iRowCountAcc; i++)
	{
		CString StageName, AwardsTimes;
		strAccount = m_CListCtrlAcc.GetItemText(i, 1); // 获得团长角色账户
		StageName = m_CListCtrlAcc.GetItemText(i, 3); // 获得占领城市名称
		AwardsTimes = m_CListCtrlAcc.GetItemText(i, 4); // 获得奖励次数

		for (int j=0 ; j<iRowCountConfig ; j++ )
		{
			CString strCityName, strType;
			strCityName = m_CListCtrlConfig.GetItemText(j,0);  //奖励城市名称
			strType = m_CListCtrlConfig.GetItemText(j,2);  //奖励城市名称
			if(StageName == strCityName && strType == "时间奖励") //匹配成功
			{
				strItem = m_CListCtrlConfig.GetItemText(j,3);
				strNum = m_CListCtrlConfig.GetItemText(j,5);

				strItem2 = m_CListCtrlConfig.GetItemText(j,6);
				strNum2 = m_CListCtrlConfig.GetItemText(j,8);

				strItem3 = m_CListCtrlConfig.GetItemText(j,9);
				strNum3 = m_CListCtrlConfig.GetItemText(j,11);

				strItem4 = m_CListCtrlConfig.GetItemText(j,12);
				strNum4 = m_CListCtrlConfig.GetItemText(j,14);

				strItem5 = m_CListCtrlConfig.GetItemText(j,15);
				strNum5 = m_CListCtrlConfig.GetItemText(j,17);

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

				int i = _ttoi(AwardsTimes);
				for(;i>0;i--)
				{
					if(!Common::SendXubao(strAccount, &ItemList))
					{
						Common::Log(Error, "发放国战军团<时间>奖励失败");
					}
				}
			}
		}
	}
	Common::Log(Info, "国战军团<时间>奖励发放结束");
}


/* 占城奖励 */
void CCwarOrgDlg::PaymentOfAwards_1()
{
	if(!Common::SanGuoServerIsRuning)
		return;

	CString strAccount, strItem, strNum, strItem2, strNum2, strItem3, strNum3, strItem4, strNum4, strItem5, strNum5;

	Common::Log(Info, "国战军团<占城>奖励发放开始");
	CString strPathName;
	CList <AccAttr, AccAttr&> *LCurAccAttr = &Common::LAccAttr;
	CList <OrganizeAttr, OrganizeAttr&> *LCurOrganizeAttr = &Common::LOrganizeAttr;
	CList <StageDef, StageDef&> *LCurStageDef = &Common::LStageDef;
	int ret = 0;
	POSITION pos, pos2;
	CString AwardsStages = "|";  //监视城市列表

	//获得受监视的城市列表
	int iRowCountConfig = m_CListCtrlConfig.GetItemCount();
	for(int i=0;i<iRowCountConfig;i++)
	{
		if(m_CListCtrlConfig.GetItemText(i,2) == "占城奖励")
		{
			CString strTemp = "";
			strTemp = m_CListCtrlConfig.GetItemText(i,0);
			AwardsStages += strTemp + "|";
		}		
	}

	Common::GetOrganizeAttr();

	pos = LCurOrganizeAttr->GetHeadPosition();
	for (int i=0;i < LCurOrganizeAttr->GetCount();i++)
	{
		struct OrganizeAttr TmpOrganizeAttr = LCurOrganizeAttr->GetNext(pos);  //获得当前军团的属性
		struct CwarAwards TmpCwarAwards;

		pos2 = LCurStageDef->GetHeadPosition();   // 通过地图ID获得地图名称，用来和监视城市进行对比
		for (int j=0;j < LCurStageDef->GetCount();j++)
		{
			struct StageDef TmpStageDef = LCurStageDef->GetNext(pos2);
			if(0 == strcmp(TmpStageDef.ID, Common::convert(TmpOrganizeAttr.StageId)))
			{
				TmpStageDef.Name.Replace("city_", "");
				TmpCwarAwards.StageName = Common::Big2GB((LPSTR)(LPCTSTR)TmpStageDef.Name);
			}
		}

		if(AwardsStages.Find("|" + TmpCwarAwards.StageName + "|") == -1)  //军团所占城市不在奖励范围
			continue;

		TmpCwarAwards.OrganizeName = TmpOrganizeAttr.OrganizeName;
		TmpCwarAwards.OrganizeNameOfLeaderZh = TmpOrganizeAttr.OrganizeLeaderZh;
		pos2 = LCurAccAttr->GetHeadPosition();
		for (int j=0;j < LCurAccAttr->GetCount();j++)
		{
			struct AccAttr TmpAccAttr = LCurAccAttr->GetNext(pos2);
			if(0 == strcmp(TmpAccAttr.Name, TmpOrganizeAttr.OrganizeLeaderZh))
				TmpCwarAwards.OrganizeAccountOfLeaderZh = TmpAccAttr.Account;
		}
		TmpCwarAwards.AwardsTimes = 1;

		for (int j=0 ; j<iRowCountConfig ; j++ )
		{
			CString strCityName, strType;
			strCityName = m_CListCtrlConfig.GetItemText(j,0);  //奖励城市名称
			strType = m_CListCtrlConfig.GetItemText(j,2);  
			if(TmpCwarAwards.StageName == strCityName && strType == "占城奖励") //匹配成功
			{
				strItem = m_CListCtrlConfig.GetItemText(j,3);
				strNum = m_CListCtrlConfig.GetItemText(j,5);

				strItem2 = m_CListCtrlConfig.GetItemText(j,6);
				strNum2 = m_CListCtrlConfig.GetItemText(j,8);

				strItem3 = m_CListCtrlConfig.GetItemText(j,9);
				strNum3 = m_CListCtrlConfig.GetItemText(j,11);

				strItem4 = m_CListCtrlConfig.GetItemText(j,12);
				strNum4 = m_CListCtrlConfig.GetItemText(j,14);

				strItem5 = m_CListCtrlConfig.GetItemText(j,15);
				strNum5 = m_CListCtrlConfig.GetItemText(j,17);

				if(strItem.IsEmpty())	strNum = "";
				if(strItem2.IsEmpty())	strNum2 = "";
				if(strItem3.IsEmpty())	strNum3 = "";
				if(strItem4.IsEmpty())	strNum4 = "";
				if(strItem5.IsEmpty())	strNum5 = "";

				strAccount = TmpCwarAwards.OrganizeAccountOfLeaderZh;

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
					Common::Log(Error, "发放国战军团<占城>奖励失败");
				}
			}
		}
	}

	Common::Log(Info, "国战军团<占城>奖励发放结束");
}


void CCwarOrgDlg::OnCbnEditchangeComboStage()
{
	// TODO: 在此添加控件通知处理程序代码
	CString strEnter;
	m_CComboBoxStage.GetWindowText(strEnter);

	int nCount = m_CComboBoxStage.GetCount();
	for(int i = 0; i < nCount; i++)
	{
		CString strValue;
		m_CComboBoxStage.GetLBText(i,strValue);
		if(strstr(strValue, strEnter))
		{
			m_CComboBoxStage.SetCurSel(i);
			break;
		}
	}
}


void CCwarOrgDlg::OnEnChangeEditCheckTime()
{
	// TODO:  如果该控件是 RICHEDIT 控件，它将不
	// 发送此通知，除非重写 CDialogEx::OnInitDialog()
	// 函数并调用 CRichEditCtrl().SetEventMask()，
	// 同时将 ENM_CHANGE 标志“或”运算到掩码中。

	// TODO:  在此添加控件通知处理程序代码
	m_CEditChkTime.GetWindowText(Common::m_CWarAwardsChkTime);
	Common::SaveConfig();
}


void CCwarOrgDlg::OnEnChangeEditItemId()
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


void CCwarOrgDlg::OnEnChangeEditItemId2()
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


void CCwarOrgDlg::OnEnChangeEditItemId3()
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


void CCwarOrgDlg::OnEnChangeEditItemId4()
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


void CCwarOrgDlg::OnEnChangeEditItemId5()
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


LRESULT CCwarOrgDlg::DoResetConfig(WPARAM wParam, LPARAM lParam)
{
	KillTimer(TIMER1);
	KillTimer(TIMER2);
	KillTimer(TIMER3);
	KillTimer(TIMER4);

	SetStateStatic(FALSE);
	DoButtonExe();
	return 0;
}
