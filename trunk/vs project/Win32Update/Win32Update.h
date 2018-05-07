#pragma once

#include "resource.h"

#define FTP_SERVER_IP			"qysg.52hcby.top"//"111.67.195.101"//"52hcby.top"
#define FTP_SERVER_PORT			21
#define FTP_USER_NAME			"donyj"
#define FTP_USER_PWD			"5a588a"
#define GAME_START_FILE			"qysg.dat"
#define CLNT_UPDATE_TITLE		"Win32Update"
#define CLNT_PROTECT_TITLE		"Win32Protect"
#define CLNT_UPDATE_FILE		CLNT_UPDATE_TITLE".exe"
#define CLNT_PROTECT_FILE		CLNT_PROTECT_TITLE".exe"
#define CLNT_COPY_RIGHT			"情义"
#define CLNT_TITLE				CLNT_COPY_RIGHT"登录器"
#define CLNT_FILE				CLNT_TITLE".exe"
#define GAME_START_LINE			" fuck you man! 1" //前面空格必须要
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

int InitializeComponent();