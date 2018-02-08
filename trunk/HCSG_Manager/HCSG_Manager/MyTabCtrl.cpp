// MyTabCtrl.cpp : implementation file
//
/////////////////////////////////////////////////////
// This class is provided as is and Ben Hill takes no
// responsibility for any loss of any kind in connection
// to this code.
/////////////////////////////////////////////////////
// Is is meant purely as a educational tool and may
// contain bugs.
/////////////////////////////////////////////////////
// ben@shido.fsnet.co.uk
// http://www.shido.fsnet.co.uk
/////////////////////////////////////////////////////
// Thanks to a mystery poster in the C++ forum on 
// www.codeguru.com I can't find your name to say thanks
// for your Control drawing code. If you are that person 
// thank you very much. I have been able to use some of 
// you ideas to produce this sample application.
/////////////////////////////////////////////////////

#include "stdafx.h"
#include "HCSG_Manager.h"
#include "MyTabCtrl.h"

#include "CwarPlayerDlg.h"
#include "CwarOrgDlg.h"
#include "LoginDlg.h"
#include "AttrDlg.h"
#include "CBDlg.h"
#include "XubaoDlg.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
// CMyTabCtrl

CMyTabCtrl::CMyTabCtrl()
{

}

CMyTabCtrl::~CMyTabCtrl()
{
	for(int nCount=0; nCount < m_nNumberOfPages; nCount++){
		delete m_tabPages[nCount];
	}
}

void CMyTabCtrl::SetNumberOfPages(int TabPagesNum)
{
	m_nNumberOfPages = TabPagesNum;
}

void CMyTabCtrl::SetCurrentPage(int TabIndex)
{
	for(int i=0; i<m_nNumberOfPages; i++)
	{
		if(i == TabIndex)
			m_tabPages[i]->ShowWindow(SW_SHOW);
		else
			m_tabPages[i]->ShowWindow(SW_HIDE);
	}
	m_tabCurrent = TabIndex;
}

HWND CMyTabCtrl::InsetPage(int TabIndex, int PageID, CDialog *PageDlg)
{
	m_tabPages[TabIndex]= PageDlg;
	m_tabPages[TabIndex]->Create(PageID, this);
	SetRectangle(TabIndex);

	return PageDlg->m_hWnd;;
}

void CMyTabCtrl::SetRectangle(int TabIndex)
{
	CRect tabRect, itemRect;
	int nX, nY, nXc, nYc;

	GetClientRect(&tabRect);
	GetItemRect(0, &itemRect);

	nX=itemRect.left;
	nY=itemRect.bottom+1;
	nXc=tabRect.right-itemRect.left-1;
	nYc=tabRect.bottom-nY-1;

	m_tabPages[0]->SetWindowPos(&wndTop, nX, nY, nXc, nYc, SWP_SHOWWINDOW);
	m_tabPages[TabIndex]->SetWindowPos(&wndTop, nX, nY, nXc, nYc, SWP_HIDEWINDOW);
}

BEGIN_MESSAGE_MAP(CMyTabCtrl, CTabCtrl)
	//{{AFX_MSG_MAP(CMyTabCtrl)
	ON_WM_LBUTTONDOWN()
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CMyTabCtrl message handlers

void CMyTabCtrl::OnLButtonDown(UINT nFlags, CPoint point) 
{
	CTabCtrl::OnLButtonDown(nFlags, point);

	if(m_tabCurrent != GetCurFocus()){
		m_tabPages[m_tabCurrent]->ShowWindow(SW_HIDE);
		m_tabCurrent=GetCurFocus();
		m_tabPages[m_tabCurrent]->ShowWindow(SW_SHOW);
		m_tabPages[m_tabCurrent]->SetFocus();
	}
}
