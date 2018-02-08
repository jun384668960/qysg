#pragma once

#include "resource.h"

#define SERVER_IP	"183.60.106.159"
#define SERVER_PORT	"21216"
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