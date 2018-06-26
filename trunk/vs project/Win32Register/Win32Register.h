#pragma once

#include "resource.h"

#define SERVER_IP				"qysg.52hcby.top"//"192.168.1.100"//"111.67.195.101"//
#define SERVER_PORT				"22119"
#define GAME_VER				"1.0.1.6"
#define CLNT_COPY_RIGHT			"情义"
#define CLNT_TITLE				CLNT_COPY_RIGHT"登录器"

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