#pragma once

#include "resource.h"

#define SERVER_IP	"114.55.138.137"//"127.0.0.1"//"115.231.220.37"//"117.27.159.42" //"120.77.147.184"//"192.168.1.100"//"120.77.147.184"//"183.60.106.159"
#define SERVER_PORT	"17000"
#define GAME_VER	"1.0.1.9"
#define GAME_START_FILE "qysg.dat"
#define CLNT_UPDATE_FILE "情义微端.exe"
#define CLNT_PROTECT_FILE "Win32Protect.exe"
#define KEY_HELP_FILE "KeyHelp.exe"
#define GAME_START_LINE " fuck you man! 1" //前面空格必须要

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