// LoginDlg.cpp : 实现文件
//

#include "stdafx.h"
#include "HCSG_Manager.h"
#include "LoginDlg.h"
#include "afxdialogex.h"

DWORD mPids[11];

// CLoginDlg 对话框

IMPLEMENT_DYNAMIC(CLoginDlg, CDialogEx)

CLoginDlg::CLoginDlg(CWnd* pParent /*=NULL*/)
	: CDialogEx(CLoginDlg::IDD, pParent)
{
	
}

CLoginDlg::~CLoginDlg()
{
}

void CLoginDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialogEx::DoDataExchange(pDX);
	DDX_Control(pDX, IDC_EDIT_ACC, m_EditAcc);
	DDX_Control(pDX, IDC_EDIT_DB, m_EditDB);
	DDX_Control(pDX, IDC_EDIT_LOGIN, m_EditLogin);
	DDX_Control(pDX, IDC_EDIT_VT, m_EditVT);
	DDX_Control(pDX, IDC_EDIT_MAP1, m_EditMap1);
	DDX_Control(pDX, IDC_EDIT_MAP2, m_EditMap2);
	DDX_Control(pDX, IDC_EDIT_MAP3, m_EditMap3);
	DDX_Control(pDX, IDC_EDIT_MAP4, m_EditMap4);
	DDX_Control(pDX, IDC_EDIT_BAK1, m_EditBackup1);
	DDX_Control(pDX, IDC_EDIT_BAK2, m_EditBackup2);
	DDX_Control(pDX, IDC_EDIT_GATE, m_EditGate);
	DDX_Control(pDX, IDC_LIST_ACCOUNT, m_CListCtrlAccount);
	DDX_Control(pDX, IDC_EDIT_ACC_S, m_CEditAcc);
	DDX_Control(pDX, IDC_EDIT_POINTS, m_CEditPoints);
	DDX_Control(pDX, IDC_COMBO_STATE, m_CComboBoxState);
	DDX_Control(pDX, IDC_EDIT_LOGIN_IP, m_CEditLoginIP);
	DDX_Control(pDX, IDC_EDIT_FIND_ACC, m_CEditFindAcc);
	DDX_Control(pDX, IDC_STATIC_ACC_TOTAL_S, m_CStaticAccTotal);
	DDX_Control(pDX, IDC_EDIT_ACC_PW, m_CEditAccPw);
	DDX_Control(pDX, IDC_EDIT_ACC_PW2, m_CEditAccPw2);
}


BEGIN_MESSAGE_MAP(CLoginDlg, CDialogEx)
	ON_BN_CLICKED(ID_START, &CLoginDlg::OnBnClickedStart)
	ON_BN_CLICKED(ID_STOP, &CLoginDlg::OnBnClickedStop)
	ON_BN_CLICKED(IDC_EXIT, &CLoginDlg::OnBnClickedExit)
	ON_BN_CLICKED(IDC_BUTTON_ACC, &CLoginDlg::OnBnClickedButtonAcc)
	ON_BN_CLICKED(IDC_BUTTON_DB, &CLoginDlg::OnBnClickedButtonDb)
	ON_BN_CLICKED(IDC_BUTTON_LOGIN, &CLoginDlg::OnBnClickedButtonLogin)
	ON_BN_CLICKED(IDC_BUTTON_VT, &CLoginDlg::OnBnClickedButtonVt)
	ON_BN_CLICKED(IDC_BUTTON_MAP1, &CLoginDlg::OnBnClickedButtonMap1)
	ON_BN_CLICKED(IDC_BUTTON_MAP2, &CLoginDlg::OnBnClickedButtonMap2)
	ON_BN_CLICKED(IDC_BUTTON_MAP3, &CLoginDlg::OnBnClickedButtonMap3)
	ON_BN_CLICKED(IDC_BUTTON_MAP4, &CLoginDlg::OnBnClickedButtonMap4)
	ON_BN_CLICKED(IDC_BUTTON_BAK1, &CLoginDlg::OnBnClickedButtonBak1)
	ON_BN_CLICKED(IDC_BUTTON_BAK2, &CLoginDlg::OnBnClickedButtonBak2)
	ON_BN_CLICKED(IDC_BUTTON_GATE, &CLoginDlg::OnBnClickedButtonGate)
	ON_NOTIFY(NM_CLICK, IDC_LIST_ACCOUNT, &CLoginDlg::OnNMClickListAccount)
	ON_BN_CLICKED(IDC_BUTTON_UPDATE_S, &CLoginDlg::OnBnClickedButtonUpdate)
	ON_EN_CHANGE(IDC_EDIT_FIND_ACC, &CLoginDlg::OnEnChangeEditFindAcc)
	ON_BN_CLICKED(IDC_BUTTON_FIND_NEXT_ACC, &CLoginDlg::OnBnClickedButtonFindNextAcc)
	ON_BN_CLICKED(ID_ADD_ACCOUNT, &CLoginDlg::OnBnClickedAddAccount)
	ON_BN_CLICKED(ID_ADD_MODIFY_PW, &CLoginDlg::OnBnClickedAddModifyPw)
	ON_BN_CLICKED(IDC_FREEZE, &CLoginDlg::OnBnClickedFreeze)
	ON_BN_CLICKED(IDC_UNFREEZE, &CLoginDlg::OnBnClickedUnfreeze)
END_MESSAGE_MAP()


// CLoginDlg 消息处理程序

BOOL CLoginDlg::OnInitDialog()
{
	CDialogEx::OnInitDialog();

	m_CListCtrlAccount.SetExtendedStyle(LVS_EX_FULLROWSELECT|LVS_EX_GRIDLINES|LVS_EX_HEADERDRAGDROP);
	m_CListCtrlAccount.InsertColumn(0, _T("注册帐号"), LVCFMT_LEFT, 80);
	m_CListCtrlAccount.InsertColumn(1, _T("角色名称"), LVCFMT_LEFT, 80);
	m_CListCtrlAccount.InsertColumn(2, _T("密码"), LVCFMT_LEFT, 70);
	m_CListCtrlAccount.InsertColumn(3, _T("状态"), LVCFMT_LEFT, 40);
	m_CListCtrlAccount.InsertColumn(4, _T("封号"), LVCFMT_LEFT, 40);//插入列
	m_CListCtrlAccount.InsertColumn(5, _T("权限"), LVCFMT_LEFT, 40);//插入列
	m_CListCtrlAccount.InsertColumn(6, _T("代币"), LVCFMT_LEFT, 50);
	m_CListCtrlAccount.InsertColumn(7, _T("登录IP"), LVCFMT_LEFT, 100);
	m_CListCtrlAccount.InsertColumn(8, _T("登录时间"), LVCFMT_LEFT, 150);
	m_CListCtrlAccount.InsertColumn(9, _T("登出时间"), LVCFMT_LEFT, 150);

	InitCurConfig();
	UpdataAcc();

	return TRUE;
}

void CLoginDlg::InitCurConfig()
{
	m_EditAcc.SetWindowText(Common::mEditAcc);
	m_EditDB.SetWindowText(Common::mEditDB);
	m_EditLogin.SetWindowText(Common::mEditLogin);
	m_EditVT.SetWindowText(Common::mEditVT);
	m_EditMap1.SetWindowText(Common::mEditMap1);
	m_EditMap2.SetWindowText(Common::mEditMap2);
	m_EditMap3.SetWindowText(Common::mEditMap3);
	m_EditMap4.SetWindowText(Common::mEditMap4);
	m_EditBackup1.SetWindowText(Common::mEditBackup1);
	m_EditBackup2.SetWindowText(Common::mEditBackup2);
	m_EditGate.SetWindowText(Common::mEditGate);
}

void CLoginDlg::UpdataAcc()
{
	POSITION pos, pos2;
	struct AccAttr TmpAccAttr;
	CList <AccAttr, AccAttr&> *LCurAccAttr = &Common::LAccAttr;
	CList <GameAcc, GameAcc&> *LCurGameAcc = &Common::LGameAcc;

	m_CListCtrlAccount.DeleteAllItems();

	Common::GetGameAccFormDB();

	pos = LCurGameAcc->GetHeadPosition();
	for(int i=0;i < LCurGameAcc->GetCount();i++)
	{
		GameAcc TmpGameAcc = LCurGameAcc->GetNext(pos);
		CString AccountID = TmpGameAcc.account;

		int nrow = m_CListCtrlAccount.GetItemCount();//取行数
		int nItem = m_CListCtrlAccount.InsertItem(nrow+1, _T(""));
		m_CListCtrlAccount.SetItemText(nItem, 0, AccountID);

		pos2 = LCurAccAttr->GetHeadPosition();
		for (int j=0;j < LCurAccAttr->GetCount();j++)
		{
			TmpAccAttr = LCurAccAttr->GetNext(pos2);
			if(0 == strcmp(TmpAccAttr.Account, AccountID))
				m_CListCtrlAccount.SetItemText(nItem, 1, _T(Common::Big2GB(TmpAccAttr.Name)));
		}

		m_CListCtrlAccount.SetItemText(nItem, 2, TmpGameAcc.password);
		m_CListCtrlAccount.SetItemText(nItem, 3, TmpGameAcc.status);
		m_CListCtrlAccount.SetItemText(nItem, 4, TmpGameAcc.enable);
		m_CListCtrlAccount.SetItemText(nItem, 5, TmpGameAcc.privilege);
		m_CListCtrlAccount.SetItemText(nItem, 6, TmpGameAcc.point);
		m_CListCtrlAccount.SetItemText(nItem, 7, TmpGameAcc.ip);
		m_CListCtrlAccount.SetItemText(nItem, 8, TmpGameAcc.LastLoginTime);
		m_CListCtrlAccount.SetItemText(nItem, 9, TmpGameAcc.LastLogoutTime);
	}

	m_CStaticAccTotal.SetWindowText(Common::convert(m_CListCtrlAccount.GetItemCount()));
}

CString CLoginDlg::setEditVlaue()
{
	CFileDialog dlg(TRUE,NULL,NULL,OFN_HIDEREADONLY|OFN_OVERWRITEPROMPT,_T("All Files(*.exe)|*.exe||"));
	if(dlg.DoModal()==IDOK){
		CString PathName=dlg.GetPathName();
		if(PathName.IsEmpty()==0){
			return PathName;
		}
	}
	return "";
}

void CLoginDlg::OnBnClickedButtonAcc()
{
	// TODO: 在此添加控件通知处理程序代码
	m_EditAcc.SetWindowText(setEditVlaue());
}

void CLoginDlg::OnBnClickedButtonDb()
{
	// TODO: 在此添加控件通知处理程序代码
	m_EditDB.SetWindowText(setEditVlaue());
}

void CLoginDlg::OnBnClickedButtonLogin()
{
	// TODO: 在此添加控件通知处理程序代码	
	m_EditLogin.SetWindowText(setEditVlaue());
}

void CLoginDlg::OnBnClickedButtonVt()
{
	// TODO: 在此添加控件通知处理程序代码
	m_EditVT.SetWindowText(setEditVlaue());
}

void CLoginDlg::OnBnClickedButtonMap1()
{
	// TODO: 在此添加控件通知处理程序代码
	m_EditMap1.SetWindowText(setEditVlaue());
}


void CLoginDlg::OnBnClickedButtonMap2()
{
	// TODO: 在此添加控件通知处理程序代码
	m_EditMap2.SetWindowText(setEditVlaue());
}

void CLoginDlg::OnBnClickedButtonMap3()
{
	// TODO: 在此添加控件通知处理程序代码
	m_EditMap3.SetWindowText(setEditVlaue());
}

void CLoginDlg::OnBnClickedButtonMap4()
{
	// TODO: 在此添加控件通知处理程序代码
	m_EditMap4.SetWindowText(setEditVlaue());
}


void CLoginDlg::OnBnClickedButtonBak1()
{
	// TODO: 在此添加控件通知处理程序代码
	m_EditBackup1.SetWindowText(setEditVlaue());
}


void CLoginDlg::OnBnClickedButtonBak2()
{
	// TODO: 在此添加控件通知处理程序代码
	m_EditBackup2.SetWindowText(setEditVlaue());
}


void CLoginDlg::OnBnClickedButtonGate()
{
	// TODO: 在此添加控件通知处理程序代码
	m_EditGate.SetWindowText(setEditVlaue());
}


void CLoginDlg::OnBnClickedStart()
{
	// TODO: 在此添加控件通知处理程序代码
	STARTUPINFO mSi;
	PROCESS_INFORMATION mPi;
	int index_temp;

	CString chPath,strTmp;
	CString temp;
	TCHAR FileName[260];
	TCHAR Directory[260];

	CEdit * m_Edits[11];
	m_Edits[0] = (CEdit *)GetDlgItem(IDC_EDIT_ACC);
	m_Edits[1] = (CEdit *)GetDlgItem(IDC_EDIT_DB);
	m_Edits[2] = (CEdit *)GetDlgItem(IDC_EDIT_LOGIN);
	m_Edits[3] = (CEdit *)GetDlgItem(IDC_EDIT_VT);
	m_Edits[4] = (CEdit *)GetDlgItem(IDC_EDIT_MAP1);
	m_Edits[5] = (CEdit *)GetDlgItem(IDC_EDIT_MAP2);
	m_Edits[6] = (CEdit *)GetDlgItem(IDC_EDIT_MAP3);
	m_Edits[7] = (CEdit *)GetDlgItem(IDC_EDIT_MAP4);
	m_Edits[8] = (CEdit *)GetDlgItem(IDC_EDIT_BAK1);
	m_Edits[9] = (CEdit *)GetDlgItem(IDC_EDIT_BAK2);
	m_Edits[10] = (CEdit *)GetDlgItem(IDC_EDIT_GATE);


	ZeroMemory(&mSi, sizeof(mSi));
	mSi.cb = sizeof(mSi);

	chPath ="D:\\sgserver\\";
	ZeroMemory(mPids,sizeof(mPids));
	int i = 0;
	for(int i=0;i<11;i++)
	{
		m_Edits[i]->GetWindowText(temp);
		if(temp.IsEmpty()) continue;
		_stprintf_s(FileName, _T("%s"), temp);
		index_temp = temp.ReverseFind('\\');
		_stprintf_s(Directory, _T("%s"), temp.Left(index_temp + 1));
		//_stprintf_s(Directory, _T(chPath));
		if (!CreateProcess(FileName, NULL, NULL, NULL, FALSE, 0, NULL, Directory, &mSi, &mPi))
		{
			strTmp.Format(_T("启动失败,代码: %d"), GetLastError());
			Common::Log(Error, strTmp);
			OnBnClickedStop();
			return;
		}

		Sleep(500);

		mPids[i] = mPi.dwProcessId;
	}
}


void CLoginDlg::OnBnClickedStop()
{
	// TODO: 在此添加控件通知处理程序代码
	for(int i=10;i>=0;i--)
	{
		if(0 == mPids[i]) continue;
		HANDLE hprocess=OpenProcess(PROCESS_TERMINATE,false,mPids[i]);
		TerminateProcess(hprocess,0);
		CloseHandle(hprocess);
	}
}


void CLoginDlg::OnBnClickedExit()
{
	// TODO: 在此添加控件通知处理程序代码
	m_EditAcc.GetWindowTextA(Common::mEditAcc);
	m_EditDB.GetWindowTextA(Common::mEditDB);
	m_EditLogin.GetWindowTextA(Common::mEditLogin);
	m_EditVT.GetWindowTextA(Common::mEditVT);
	m_EditMap1.GetWindowTextA(Common::mEditMap1);
	m_EditMap2.GetWindowTextA(Common::mEditMap2);
	m_EditMap3.GetWindowTextA(Common::mEditMap3);
	m_EditMap4.GetWindowTextA(Common::mEditMap4);
	m_EditBackup1.GetWindowTextA(Common::mEditBackup1);
	m_EditBackup2.GetWindowTextA(Common::mEditBackup2);
	m_EditGate.GetWindowTextA(Common::mEditGate);
	Common::SaveConfig();
}


void CLoginDlg::OnNMClickListAccount(NMHDR *pNMHDR, LRESULT *pResult)
{
	LPNMITEMACTIVATE pNMItemActivate = reinterpret_cast<LPNMITEMACTIVATE>(pNMHDR);

	// TODO: 在此添加控件通知处理程序代码
	*pResult = 0;
	int ListIndex = m_CListCtrlAccount.GetSelectionMark();
	if(ListIndex == -1) return;
	m_CEditAcc.SetWindowText(m_CListCtrlAccount.GetItemText(ListIndex, 0));
	m_CEditAccPw.SetWindowText(m_CListCtrlAccount.GetItemText(ListIndex, 2));
	m_CComboBoxState.SetCurSel(_ttoi(m_CListCtrlAccount.GetItemText(ListIndex, 4)));
	m_CEditPoints.SetWindowText(m_CListCtrlAccount.GetItemText(ListIndex, 6));
	m_CEditLoginIP.SetWindowText(m_CListCtrlAccount.GetItemText(ListIndex, 7));
}


void CLoginDlg::OnBnClickedButtonUpdate()
{
	// TODO: 在此添加控件通知处理程序代码
	UpdataAcc();
}


void CLoginDlg::OnEnChangeEditFindAcc()
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

	iColumnNum = m_CListCtrlAccount.GetHeaderCtrl()->GetItemCount();
	iRowCount = m_CListCtrlAccount.GetItemCount();

	for (int j=0 ; j<iRowCount ; j++ )
	{
		memset(chTemp, 0 ,32);
		for ( int i=0 ; i<iColumnNum ; i++ )
		{
			m_CListCtrlAccount.GetItemText(j,i,chTemp, 32);
			if(strstr(chTemp,strKey))
			{
				ListIndex = j;
				goto BreakLoop;
			}
		}
	}
BreakLoop:
	m_CListCtrlAccount.SetFocus();  
	m_CListCtrlAccount.SetItemState(ListIndex, LVIS_SELECTED, LVIS_SELECTED);
	m_CListCtrlAccount.SetSelectionMark(ListIndex);
	m_CListCtrlAccount.EnsureVisible(ListIndex, FALSE);
	::SendMessage(m_CListCtrlAccount.m_hWnd,  LVM_SETEXTENDEDLISTVIEWSTYLE,  
		LVS_EX_FULLROWSELECT,  LVS_EX_FULLROWSELECT); 

	m_CEditFindAcc.SetFocus();
}


void CLoginDlg::OnBnClickedButtonFindNextAcc()
{
	// TODO: 在此添加控件通知处理程序代码
	CString strKey;
	int iColumnNum,iRowCount;
	char chTemp[32];
	int ListIndex = 0;
	int iCurListIndex  = m_CListCtrlAccount.GetSelectionMark();

	m_CEditFindAcc.GetWindowText(strKey);
	if(strKey.IsEmpty()) return;

	iColumnNum = m_CListCtrlAccount.GetHeaderCtrl()->GetItemCount();
	iRowCount = m_CListCtrlAccount.GetItemCount();

	int j = iCurListIndex + 1;
	do
	{
		memset(chTemp, 0 ,32);
		for ( int i=0 ; i<2 ; i++ )  // 只查找账户名和角色名
		{
			m_CListCtrlAccount.GetItemText(j,i,chTemp, 32);
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
	m_CListCtrlAccount.SetFocus();  
	m_CListCtrlAccount.SetItemState(ListIndex, LVIS_SELECTED, LVIS_SELECTED);
	m_CListCtrlAccount.SetSelectionMark(ListIndex);
	m_CListCtrlAccount.EnsureVisible(ListIndex, FALSE);
	::SendMessage(m_CListCtrlAccount.m_hWnd,  LVM_SETEXTENDEDLISTVIEWSTYLE,  
		LVS_EX_FULLROWSELECT,  LVS_EX_FULLROWSELECT); 
}


void CLoginDlg::OnBnClickedAddAccount()
{
	// TODO: 在此添加控件通知处理程序代码
	CString strAcc, strPw;
	m_CEditAcc.GetWindowTextA(strAcc);
	m_CEditAccPw.GetWindowTextA(strPw);

	AfxMessageBox(Common::CreateAccount(strAcc, strPw));

	UpdataAcc();
}


void CLoginDlg::OnBnClickedAddModifyPw()
{
	// TODO: 在此添加控件通知处理程序代码
	CString strAcc, strPw, strPw2;
	m_CEditAcc.GetWindowTextA(strAcc);
	m_CEditAccPw.GetWindowTextA(strPw);
	m_CEditAccPw2.GetWindowTextA(strPw2);

	AfxMessageBox(Common::ModifyPassword(strAcc, strPw, strPw2));
	UpdataAcc();
}


void CLoginDlg::OnBnClickedFreeze()
{
	// TODO: 在此添加控件通知处理程序代码
	CString strAcc;
	m_CEditAcc.GetWindowTextA(strAcc);

	AfxMessageBox(Common::FreezeAccount(strAcc, "GM工具手动封号"));
	UpdataAcc();
}


void CLoginDlg::OnBnClickedUnfreeze()
{
	// TODO: 在此添加控件通知处理程序代码
	CString strAcc;
	m_CEditAcc.GetWindowTextA(strAcc);

	AfxMessageBox(Common::UnFreezeAccount(strAcc, "GM工具手动解封"));
	UpdataAcc();
}
