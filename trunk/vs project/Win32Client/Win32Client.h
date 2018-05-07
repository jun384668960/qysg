#pragma once

#include "resource.h"

#define SERVER_IP				"qysg.52hcby.top"//"111.67.195.101"
#define SERVER_PORT				"22119"
#define GAME_VER				"1.0.1.2"
#define GAME_START_FILE			"qysg.dat"
#define CLNT_UPDATE_TITLE		"Win32Update"
#define CLNT_PROTECT_TITLE		"Win32Protect"
#define CLNT_UPDATE_FILE		CLNT_UPDATE_TITLE".exe"
#define CLNT_PROTECT_FILE		CLNT_PROTECT_TITLE".exe"
#define CLNT_COPY_RIGHT			"情义"
#define CLNT_TITLE				CLNT_COPY_RIGHT"登录器"
#define CLNT_FILE				CLNT_TITLE".exe"
#define KEY_HELP_FILE			"KeyHelp.exe"
#define GAME_START_LINE			" fuck you man! 1" //前面空格必须要
#define SERVICE_URL				"http://wpa.qq.com/msgrd?v=3&uin=384668960&site=qq&menu=yes"
#define RECHARGE_URL			"http://m1.libaopay.com:8880/buy/?wid=74838"
#define MAIN_WEB_URL			"jun384668960.github.io/index.htm"
#define DOWNLOAD_WEB_URL		"jun384668960.github.io/download.htm"

//消息类型
#define START_FLAG	"0X21"
#define START_FLAG_LEN	4
#define REGISTER	0X01
#define MODIFY		0X02

struct SG_MSG
{
	char start[START_FLAG_LEN];
	int type;
	int key;
	char buf[128];
};

struct SG_MSG_REG
{
	char name[32];
	char passwd[32];
};

struct Tran_Head
{
	int length;
	int cmd;
};

struct Rg_Info
{
	CHAR name[128];
	CHAR passwd[128];
	CHAR key[64];
};

struct Mdfy_Pwd_Info
{
	CHAR name[128];
	CHAR oldpasswd[128];
	CHAR newpasswd[128];
	CHAR key[64];
};

struct Ver_Info
{
	CHAR ver[64];
};

int InitializeComponent();