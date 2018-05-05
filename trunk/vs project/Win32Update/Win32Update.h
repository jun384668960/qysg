#pragma once

#include "resource.h"

#define FTP_SERVER_IP	"192.168.1.102"//"52hcby.top"
#define FTP_SERVER_PORT	21
#define FTP_USER_NAME	"donyj"
#define FTP_USER_PWD	"5a588a"
#define GAME_START_FILE "online.dat"
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

int InitializeComponent();