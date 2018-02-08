
// HCSG_Manager.h : PROJECT_NAME 应用程序的主头文件
//

#pragma once

#ifndef __AFXWIN_H__
	#error "在包含此文件之前包含“stdafx.h”以生成 PCH 文件"
#endif

#include "resource.h"		// 主符号
#include "Local_com.h"

#include "CwarPlayerDlg.h"
#include "CwarOrgDlg.h"
#include "LoginDlg.h"
#include "AttrDlg.h"
#include "CBDlg.h"
#include "XubaoDlg.h"
#include "LogDlg.h"
#include "Vip.h"
#include "PlayerListDlg.h"
#include "ItemDlg.h"
#include "StoreDlg.h"

// CHCSG_ManagerApp:
// 有关此类的实现，请参阅 HCSG_Manager.cpp
//

class CHCSG_ManagerApp : public CWinApp
{
public:
	CHCSG_ManagerApp();

// 重写
public:
	virtual BOOL InitInstance();

// 实现

	DECLARE_MESSAGE_MAP()
};

extern CHCSG_ManagerApp theApp;