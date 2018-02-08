// XubaoDlg.cpp : 实现文件
//

#include "stdafx.h"
#include "HCSG_Manager.h"
#include "HCSG_ManagerDlg.h"
#include "XubaoDlg.h"
#include "afxdialogex.h"

// CXubaoDlg 对话框

IMPLEMENT_DYNAMIC(CXubaoDlg, CDialogEx)

	CXubaoDlg::CXubaoDlg(CWnd* pParent /*=NULL*/)
	: CDialogEx(CXubaoDlg::IDD, pParent)
{
	InitFlag = TRUE;
}

CXubaoDlg::~CXubaoDlg()
{
}

void CXubaoDlg::DoDataExchange(CDataExchange* pDX)
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
}


BEGIN_MESSAGE_MAP(CXubaoDlg, CDialogEx)
	ON_BN_CLICKED(IDC_BUTTON_INIT, &CXubaoDlg::OnBnClickedButtonInit)
	ON_NOTIFY(NM_CLICK, IDC_LIST_ACC, &CXubaoDlg::OnNMClickListAcc)
	ON_BN_CLICKED(IDC_BUTTON_ADDTOSWAP, &CXubaoDlg::OnBnClickedButtonAddtoswap)
	ON_BN_CLICKED(IDC_BUTTON_ADD_CONFIG, &CXubaoDlg::OnBnClickedButtonAddConfig)
	ON_BN_CLICKED(IDC_BUTTON_DEL_CONFIG, &CXubaoDlg::OnBnClickedButtonDelConfig)
	ON_BN_CLICKED(IDC_BUTTON_SAVE_CONFIG, &CXubaoDlg::OnBnClickedButtonSaveConfig)
	ON_BN_CLICKED(IDC_BUTTON_EXE, &CXubaoDlg::OnBnClickedButtonExe)
	ON_BN_CLICKED(IDC_BUTTON_LOAD_CONFIG, &CXubaoDlg::OnBnClickedButtonLoadConfig)
	ON_BN_CLICKED(IDC_BUTTON_FIND_ACC, &CXubaoDlg::OnBnClickedButtonFindAcc)
	ON_BN_CLICKED(IDC_BUTTON_FIND_ITEM, &CXubaoDlg::OnBnClickedButtonFindItem)
	ON_BN_CLICKED(IDC_BUTTON_FIND_NEXT_ACC, &CXubaoDlg::OnBnClickedButtonFindNextAcc)
	ON_BN_CLICKED(IDC_BUTTON_FIND_NEXT_ITEM, &CXubaoDlg::OnBnClickedButtonFindNextItem)
	ON_MESSAGE(WM_SAVEXUBAOCONFIG, DoSaveConfig)
	ON_BN_CLICKED(IDC_BUTTON_READ_ACC, &CXubaoDlg::OnBnClickedButtonReadAcc)
	ON_NOTIFY(NM_DBLCLK, IDC_LIST_CONFIG, &CXubaoDlg::OnNMDblclkListConfig)
	ON_EN_CHANGE(IDC_EDIT_ITEM_ID, &CXubaoDlg::OnEnChangeEditItemId)
	ON_EN_CHANGE(IDC_EDIT_ITEM_ID2, &CXubaoDlg::OnEnChangeEditItemId2)
	ON_EN_CHANGE(IDC_EDIT_ITEM_ID3, &CXubaoDlg::OnEnChangeEditItemId3)
	ON_EN_CHANGE(IDC_EDIT_ITEM_ID4, &CXubaoDlg::OnEnChangeEditItemId4)
	ON_EN_CHANGE(IDC_EDIT_ITEM_ID5, &CXubaoDlg::OnEnChangeEditItemId5)
END_MESSAGE_MAP()


// CXubaoDlg 消息处理程序


BOOL CXubaoDlg::OnInitDialog()
{
	CDialogEx::OnInitDialog();

	// TODO:  在此添加额外的初始化
	m_CListCtrlAcc.SetExtendedStyle(LVS_EX_FULLROWSELECT|LVS_EX_GRIDLINES|LVS_EX_HEADERDRAGDROP);
	m_CListCtrlAcc.InsertColumn(0, _T("注册帐号"), LVCFMT_LEFT, 85);
	m_CListCtrlAcc.InsertColumn(1, _T("角色名称"), LVCFMT_LEFT, 95);//插入列

	m_CListCtrlItem.SetExtendedStyle(LVS_EX_FULLROWSELECT|LVS_EX_GRIDLINES|LVS_EX_HEADERDRAGDROP);
	m_CListCtrlItem.InsertColumn(0, _T("物品ID"), LVCFMT_LEFT, 60);//插入列
	m_CListCtrlItem.InsertColumn(1, _T("物品名称"), LVCFMT_LEFT, 160);

	m_CListCtrlConfig.SetExtendedStyle(LVS_EX_FULLROWSELECT|LVS_EX_GRIDLINES|LVS_EX_HEADERDRAGDROP);
	m_CListCtrlConfig.InsertColumn(0, _T("注册帐号"), LVCFMT_LEFT, 60);//插入列
	m_CListCtrlConfig.InsertColumn(1, _T("角色名称"), LVCFMT_LEFT, 60);

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

	m_CComboBoxCfgID.SetCurSel(0);

	OnBnClickedButtonInit();

	CString strFilePath="";
	::GetCurrentDirectory(1024,strFilePath.GetBuffer(1024));
	strFilePath.ReleaseBuffer();
	strFilePath += "\\Recored.xls";
	DoLoadConfig(strFilePath);

	//SetTimer(TIMER_UPDATE, 60 * 1000, 0);  // 每分钟自动保存一下配置

	InitFlag = FALSE;
	return TRUE;  // return TRUE unless you set the focus to a control
}

void CXubaoDlg::UpdataAcc()
{
	POSITION pos, pos2;
	struct AccAttr TmpAccAttr;
	CList <AccAttr, AccAttr&> *LCurAccAttr = &Common::LAccAttr;
	CList <GameAcc, GameAcc&> *LCurGameAcc = &Common::LGameAcc;
	m_CListCtrlAcc.DeleteAllItems();

	pos = LCurGameAcc->GetHeadPosition();
	for(int i=0;i < LCurGameAcc->GetCount();i++)
	{
		CString AccountID = LCurGameAcc->GetNext(pos).account;

		int nrow = m_CListCtrlAcc.GetItemCount();//取行数
		int nItem = m_CListCtrlAcc.InsertItem(nrow+1, _T(""));
		m_CListCtrlAcc.SetItemText(nItem, 0, AccountID);

		pos2 = LCurAccAttr->GetHeadPosition();
		for (int j=0;j < LCurAccAttr->GetCount();j++)
		{
			TmpAccAttr = LCurAccAttr->GetNext(pos2);
			if(0 == strcmp(TmpAccAttr.Account, AccountID))
				m_CListCtrlAcc.SetItemText(nItem, 1, _T(Common::Big2GB(TmpAccAttr.Name)));
		}
	}	
}

void CXubaoDlg::OnBnClickedButtonInit()
{
	// TODO: 在此添加控件通知处理程序代码
	CString strPathName;
	POSITION pos;
	int ret = 0;
	struct ItemDef TmpItemDef;

	CList <ItemDef, ItemDef&> *LCurItemDef = &Common::LItemDef;

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

	UpdataAcc();

	// 初始化记录表
	CString StrValue;
	CString strFilePath="";
	::GetCurrentDirectory(1024,strFilePath.GetBuffer(1024));
	strFilePath.ReleaseBuffer();
	strFilePath += "\\Recored.xls";
	CDatabase database;//数据库库需要包含头文件 #include <afxdb.h>
	CString sDriver = "MICROSOFT EXCEL DRIVER (*.XLS)"; // Excel驱动
	CString sSql,strInsert;

	DWORD dwRe = GetFileAttributes(strFilePath);
	if ( dwRe == (DWORD)-1 )  // 不存在则创建表头
	{
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
				int i; //列、行循环参数

				iColumnNum = m_CListCtrlConfig.GetHeaderCtrl()->GetItemCount();
				iRowCount = m_CListCtrlConfig.GetItemCount();
				sSql = " CREATE TABLE 发放记录 ( ";
				strInsert = " INSERT INTO 发放记录 ( " ;
				//获得列标题名称
				lvCol.mask = LVCF_TEXT; //必需设置，说明LVCOLUMN变量中pszText参数有效
				lvCol.cchTextMax = 32; //必设，pszText参数所指向的字符串的大小
				lvCol.pszText = strColName.GetBuffer(32); //必设，pszText 所指向的字符串的实际存储位置。
				//以上三个参数设置后才能通过 GetColumn()函数获得列标题的名称
				for( i=0 ; i< iColumnNum ; i++ )
				{
					if ( !(m_CListCtrlConfig.GetColumn(i,&lvCol)) )
						return;
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
			}     
			// 关闭Excel表格文件
			database.Close();
		}
		CATCH_ALL(e)
		{
			//错误类型很多，根据需要进行报错。
			Common::Log(Error, "创建虚宝发放记录表失败。");
		}
		END_CATCH_ALL;
	}
}


void CXubaoDlg::OnNMClickListAcc(NMHDR *pNMHDR, LRESULT *pResult)
{
	LPNMITEMACTIVATE pNMItemActivate = reinterpret_cast<LPNMITEMACTIVATE>(pNMHDR);

	// TODO: 在此添加控件通知处理程序代码
	int ListIndex = m_CListCtrlAcc.GetSelectionMark();
	if(ListIndex == -1) return;
	m_CEditDisplayAcc.SetWindowText(m_CListCtrlAcc.GetItemText(ListIndex, 0));
	m_CEditDisplayName.SetWindowText(m_CListCtrlAcc.GetItemText(ListIndex, 1));
	*pResult = 0;
}


void CXubaoDlg::OnBnClickedButtonAddtoswap()
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


void CXubaoDlg::OnBnClickedButtonAddConfig()
{
	// TODO: 在此添加控件通知处理程序代码
	CString strAccount, strAccountName;
	CString strItem, strNum, strItem2, strNum2, strItem3, strNum3, strItem4, strNum4, strItem5, strNum5;
	CString strItemName, strItemName2, strItemName3, strItemName4, strItemName5;

	m_CEditDisplayAcc.GetWindowTextA(strAccount);
	m_CEditDisplayName.GetWindowTextA(strAccountName);
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

	if(strAccount.IsEmpty()){	strAccountName = "";}
	if(strItem.IsEmpty()){	strItemName = ""; strNum = "";}
	if(strItem2.IsEmpty()){	strItemName2 = ""; strNum2 = "";}
	if(strItem3.IsEmpty()){	strItemName3 = ""; strNum3 = "";}
	if(strItem4.IsEmpty()){	strItemName4 = ""; strNum4 = "";}
	if(strItem5.IsEmpty()){	strItemName5 = ""; strNum5 = "";}

	if(strAccount.IsEmpty() || (strItem.IsEmpty() && strItem2.IsEmpty() 
		&& strItem3.IsEmpty() && strItem4.IsEmpty() && strItem5.IsEmpty()))
		return;

	int nrow = m_CListCtrlConfig.GetItemCount();//取行数
	int nItem = m_CListCtrlConfig.InsertItem(nrow+1, _T(""));

	m_CListCtrlConfig.SetItemText(nItem, 0, strAccount);
	m_CListCtrlConfig.SetItemText(nItem, 1, _T(strAccountName));
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
}


void CXubaoDlg::OnBnClickedButtonDelConfig()
{
	// TODO: 在此添加控件通知处理程序代码
	int ListIndex  = m_CListCtrlConfig.GetSelectionMark();
	if(ListIndex == -1) return;
	m_CListCtrlConfig.DeleteItem(ListIndex);//删除选中行
}


void CXubaoDlg::OnBnClickedButtonSaveConfig()
{
	// TODO: 在此添加控件通知处理程序代码
	//首先判断列表框中是否有记录
	if ( m_CListCtrlConfig.GetItemCount()<= 0 )
	{
		Common::Log(Info, "列表中没有记录需要保存！");
		return;
	}
	Common::SaveAwardsConfig();
	Common::Log(Info, "保存配置信息到Excel文件成功！");
}


LRESULT CXubaoDlg::DoSaveConfig(WPARAM wParam, LPARAM lParam)
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
			sSql = " CREATE TABLE 发放记录 ( ";
			strInsert = " INSERT INTO 发放记录 ( " ;
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
			for ( j=iRowCount-2 ; j<iRowCount ; j++ )   // 插入最后一条
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
		Common::Log(Info, "保存虚宝发放成功！");
	}
	CATCH_ALL(e)
	{
		//错误类型很多，根据需要进行报错。
		Common::Log(Error, "虚宝发放历史记录保存失败。");
	}
	END_CATCH_ALL;

	return 0;
}


void CXubaoDlg::OnBnClickedButtonLoadConfig()
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
		MessageBox("历史记录成功导入系统!","导入成功");
	else
		MessageBox("历史记录导入系统失败!","导入失败");
}


BOOL CXubaoDlg::DoLoadConfig(CString strFilePath)
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
	sSql.Format("SELECT 注册帐号 , 角色名称 , 物品ID1 , 物品名1 , 数量1 , 物品ID2 , 物品名2 , 数量2 ,"
		" 物品ID3 , 物品名3 , 数量3 , 物品ID4 , 物品名4 , 数量4 , 物品ID5 , 物品名5 , 数量5 FROM 发放记录");
	pset.Open(CRecordset::forwardOnly,sSql,CRecordset::readOnly);
	while(!pset.IsEOF())
	{
		pset.GetFieldValue("注册帐号",arr[0]);//前面字段必须与表中的相同，否则出错。
		pset.GetFieldValue("角色名称",arr[1]);

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

		int count = m_CListCtrlConfig.GetItemCount();//插入到ListCtrl中
		m_CListCtrlConfig.InsertItem(count,arr[0]);
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


void CXubaoDlg::OnBnClickedButtonExe()
{
	// TODO: 在此添加控件通知处理程序代码
	_ConnectionPtr m_pConnXb;
	_RecordsetPtr m_pRecordsetXb;
	CString strInsert;
	CString strAccount, strAccName, strItem, strNum, strItem2, strNum2, strItem3, strNum3, strItem4, strNum4, strItem5, strNum5;
	CString strItemName, strItemName2, strItemName3, strItemName4, strItemName5;

	CString SQLInSertCmd;

	m_CEditDisplayAcc.GetWindowTextA(strAccount);
	m_CEditDisplayName.GetWindowTextA(strAccName);

	m_CEditItemId.GetWindowTextA(strItem);
	m_CEditItemNum.GetWindowTextA(strNum);
	m_CEditItemName.GetWindowTextA(strItemName);
	//物品2
	m_CEditItemId2.GetWindowTextA(strItem2);
	m_CEditItemNum2.GetWindowTextA(strNum2);
	m_CEditItemName2.GetWindowTextA(strItemName2);
	//物品3
	m_CEditItemId3.GetWindowTextA(strItem3);
	m_CEditItemNum3.GetWindowTextA(strNum3);
	m_CEditItemName3.GetWindowTextA(strItemName3);
	//物品4
	m_CEditItemId4.GetWindowTextA(strItem4);
	m_CEditItemNum4.GetWindowTextA(strNum4);
	m_CEditItemName4.GetWindowTextA(strItemName4);
	//物品5
	m_CEditItemId5.GetWindowTextA(strItem5);
	m_CEditItemNum5.GetWindowTextA(strNum5);
	m_CEditItemName5.GetWindowTextA(strItemName5);

	if(strItem.IsEmpty())	strNum = "";
	if(strItem2.IsEmpty())	strNum2 = "";
	if(strItem3.IsEmpty())	strNum3 = "";
	if(strItem4.IsEmpty())	strNum4 = "";
	if(strItem5.IsEmpty())	strNum5 = "";

	if(strAccount.IsEmpty() || (strItem.IsEmpty() && strItem2.IsEmpty() 
		&& strItem3.IsEmpty() && strItem4.IsEmpty() && strItem5.IsEmpty()))
		return;
	CString strMsg;
	strMsg.Format("是否向 %s(%s) 发放 ", strAccount, strAccName);
	if(!strItem.IsEmpty())	strMsg += strItemName + "x" + strNum + " ";
	if(!strItem2.IsEmpty())	strMsg += strItemName2 + "x" + strNum2 + " ";
	if(!strItem3.IsEmpty())	strMsg += strItemName3 + "x" + strNum3 + " ";
	if(!strItem4.IsEmpty())	strMsg += strItemName4 + "x" + strNum4 + " ";
	if(!strItem5.IsEmpty())	strMsg += strItemName5 + "x" + strNum5 + " ";
	strMsg += "?";
	if (IDYES != AfxMessageBox(strMsg, MB_YESNO))
		return;

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
		Common::Log(Error, "代币银票失败");
		AfxMessageBox("代币银票失败");
		return;
	}

	OnBnClickedButtonAddConfig();
	Common::SaveAwardsConfig();
}


void CXubaoDlg::OnBnClickedButtonFindAcc()
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


void CXubaoDlg::OnBnClickedButtonFindNextAcc()
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


void CXubaoDlg::OnBnClickedButtonFindItem()
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


void CXubaoDlg::OnBnClickedButtonFindNextItem()
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


void CXubaoDlg::LocalConfigSave()
{
	;
}


void CXubaoDlg::OnBnClickedButtonReadAcc()
{
	// TODO: 在此添加控件通知处理程序代码
	Common::GetUpdateCLists();
	UpdataAcc();
}


void CXubaoDlg::OnNMDblclkListConfig(NMHDR *pNMHDR, LRESULT *pResult)
{
	LPNMITEMACTIVATE pNMItemActivate = reinterpret_cast<LPNMITEMACTIVATE>(pNMHDR);
	// TODO: 在此添加控件通知处理程序代码
	*pResult = 0;
	int ListIndex = m_CListCtrlConfig.GetSelectionMark();
	if(ListIndex == -1) return;

	m_CEditDisplayAcc.SetWindowText(m_CListCtrlConfig.GetItemText(ListIndex, 0));
	m_CEditDisplayName.SetWindowText(m_CListCtrlConfig.GetItemText(ListIndex, 1));

	m_CEditItemId.SetWindowText(m_CListCtrlConfig.GetItemText(ListIndex, 2));
	m_CEditItemName.SetWindowText(m_CListCtrlConfig.GetItemText(ListIndex, 3));
	m_CEditItemNum.SetWindowText(m_CListCtrlConfig.GetItemText(ListIndex, 4));

	m_CEditItemId2.SetWindowText(m_CListCtrlConfig.GetItemText(ListIndex, 5));
	m_CEditItemName2.SetWindowText(m_CListCtrlConfig.GetItemText(ListIndex, 6));
	m_CEditItemNum2.SetWindowText(m_CListCtrlConfig.GetItemText(ListIndex, 7));

	m_CEditItemId3.SetWindowText(m_CListCtrlConfig.GetItemText(ListIndex, 8));
	m_CEditItemName3.SetWindowText(m_CListCtrlConfig.GetItemText(ListIndex, 9));
	m_CEditItemNum3.SetWindowText(m_CListCtrlConfig.GetItemText(ListIndex, 10));

	m_CEditItemId4.SetWindowText(m_CListCtrlConfig.GetItemText(ListIndex, 11));
	m_CEditItemName4.SetWindowText(m_CListCtrlConfig.GetItemText(ListIndex, 12));
	m_CEditItemNum4.SetWindowText(m_CListCtrlConfig.GetItemText(ListIndex, 13));

	m_CEditItemId5.SetWindowText(m_CListCtrlConfig.GetItemText(ListIndex, 14));
	m_CEditItemName5.SetWindowText(m_CListCtrlConfig.GetItemText(ListIndex, 15));
	m_CEditItemNum5.SetWindowText(m_CListCtrlConfig.GetItemText(ListIndex, 16));
}


void CXubaoDlg::OnEnChangeEditItemId()
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


void CXubaoDlg::OnEnChangeEditItemId2()
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


void CXubaoDlg::OnEnChangeEditItemId3()
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


void CXubaoDlg::OnEnChangeEditItemId4()
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


void CXubaoDlg::OnEnChangeEditItemId5()
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
