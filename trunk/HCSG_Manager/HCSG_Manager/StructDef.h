#pragma once
#include "afxwin.h"

#pragma pack(1)
struct GameAcc{
	CString account;
	CString password;
	CString password2;
	CString duedate;
	CString enable;
	CString lock_duedate;
	CString logout_time;
	CString ip;
	CString create_time;
	CString privilege;
	CString status;   //状态
	CString sec_pwd;
	CString first_ip;
	CString point;
	CString trade_psw;
	CString IsAdult;
	CString OnlineTime;
	CString OfflineTime;
	CString LastLoginTime;
	CString LastLogoutTime;
};
#pragma pack() 

#pragma pack(1)
struct SoldierAttr
{
	char type[2];    //貌似是死的还是活的意思，只看到了1 和 0的值
	char Name[16];		// 名称
	char A1[3]; // 有值但是不知道是什么
	unsigned __int16 Level;
	unsigned __int32 Hp;
	unsigned __int32 Exp;
	char A2[16]; // 有值但是不知道是什么
	unsigned __int16 Attr_str; //武力
	unsigned __int16 Attr_int;//智力
	unsigned __int16 Type;//兵种
	unsigned __int16 Attr_dex;//体魄
	unsigned __int16 Attr_mind; //反应
	
	char A3[58]; // 有值但是不知道是什么
	unsigned __int8 Loyal; //忠诚
	char A4[3]; // 有值但是不知道是什么
	unsigned __int8 Attack;//附属攻击力
	unsigned __int8 Defence;//附属防御力
	char A5[22]; // 空值
};
#pragma pack() 

#pragma pack(1)
struct AccAttr
{
	char Account[24];  // 人物帐号
	char A1[4];
	unsigned __int32 nIndex;	// 序号
	char Name[15];		// 名称
	char Corps[15];		//军团
	unsigned __int32 Gold;		//金钱
	unsigned __int32 Exp;
	unsigned __int16 SkillExp;
	unsigned __int16 Anger;
	unsigned __int16 AngerNum;
	unsigned __int16 Level;
	char A2[7];
	unsigned __int16 Attr_Num;  //属性点
	unsigned __int32 Honor;	//功勋值
	unsigned __int32 Hp;
	unsigned __int32 Mp;

	char A3[249];

	unsigned __int16 Attr_str_up;   // 属性上限
	unsigned __int16 Attr_int_up;
	unsigned __int16 Attr_con_up;
	unsigned __int16 Attr_dex_up;
	unsigned __int16 Attr_mind_up;
	unsigned __int16 Attr_leader_up;

	unsigned __int16 Attr_str; //武力
	unsigned __int16 Attr_int;//智力
	unsigned __int16 Attr_con;//精神	
	unsigned __int16 Attr_dex;//体魄
	unsigned __int16 Attr_mind; //反应	
	unsigned __int16 Attr_leader;//统御

	char A4[979];

	unsigned __int8 PackNum;//背包格数		
	unsigned __int8 StoreNum; //仓库格数

	char A5[169];

	unsigned __int16 Officer;	//官职

	char A6[232];
};
#pragma pack() 

#pragma pack(1)
struct OrganizeAttr
{
	char A1[2312]; // 空值
	char A2[8]; // 有值，用途未知
	char A3[6]; // 空值
	unsigned __int16  StageId; // 城市代码
	char A4[4]; // 有值，用途未知
	char OrganizeLeader[15]; // 军团长
	char A5[4]; // 有值，用途未知
	char A6[8]; // 有值，用途未知

	char A7[1473]; // 空值
	char A8[2]; // 有值，用途未知
	char A9[7]; //空值
	char OrganizeName[15]; //军团名
	char OrganizeLeaderZh[15]; //团长
	char OrganizeLeaderFu[15]; //副团
	char A10[2]; //猜测为团员数量
	char A11[9]; // 有值，用途未知

	char A12[519]; // 空值
};
#pragma pack()

#pragma pack(1)
struct ItemDef{
	CString ID;
	CString Name;
};
#pragma pack() 

#pragma pack(1)
struct StageDef{
	CString ID;
	CString Name;
};
#pragma pack() 

#pragma pack(1)
struct CwarHeroes
{
	CString Name;	// 角色名
	CString Level;	// 等级
	CString Job;	// 职业
	CString Nationality;	// 国籍
	CString Honor;	// 功勋值
	CString Kills;	// 讨敌数
	CString Corps;
};
#pragma pack() 

#pragma pack(1)
struct CwarCorps
{
	char Name[15];	// 军团名
	char ID[15];	// 唯一标识
	unsigned __int16 Honor;	// 功勋和
};
#pragma pack() 

#pragma pack(1)
struct HistoryCbHeroes
{
	CString Account;	// 角色帐号
	CString Name;	// 角色名
	CString Job;	// 职业
	CString Nationality;	// 国籍
	CString Points;	// 积分
	CString Honor;	// 功勋值
	CString Kills;	// 讨敌数
};
#pragma pack() 

#pragma pack(1)
struct ServerConfigTime
{
	CString Period;  // 持续时间
	CStringArray ModStartTimes;
	CStringArray TuseStartTimes;
	CStringArray WedStartTimes;
	CStringArray ThursStartTimes;
	CStringArray FriStartTimes;
	CStringArray SatStartTimes;
	CStringArray SunStartTimes;
};
#pragma pack()

#pragma pack(1)
struct CwarAwards
{
	CString OrganizeName;  // 军团名
	CString OrganizeNameOfLeaderZh; //团长
	CString OrganizeAccountOfLeaderZh; //团长帐号
	CString StageName;
	CString StageID;
	unsigned int AwardsTimes;
};
#pragma pack()

#pragma pack(1)
struct SaveItem
{
	unsigned __int32 TimeStamp; // 从1970年到现在呢的秒数
	char A1[4];  // 物品产生是的标记号，从服务器启动开始计算
	char PlayerName[15];  // 角色名
	char A2[2];
	unsigned __int16 ItemId;
	char A3[4];
	unsigned __int8 ItemNum;
	char A4[40];
};
#pragma pack()

#pragma pack(1)
struct SaveItems
{
	struct SaveItem Player_1[80];  // 第一个角色的物品
	struct SaveItem Player_2[80];  // 第二个角色的物品
	struct SaveItem Player_3[80];  // 第三个角色的物品
};
#pragma pack()

#pragma pack(1)
struct SaveStore
{
	unsigned __int32 TimeStamp; // 从1970年到现在呢的秒数
	char A1[4];  // 物品产生是的标记号，从服务器启动开始计算
	char PlayerName[15];  // 角色名
	char A2[2];
	unsigned __int16 ItemId;
	char A3[4];
	unsigned __int8 ItemNum;
	char A4[40];
};
#pragma pack()

#pragma pack(1)
struct SaveStores
{
	unsigned __int32 Player_1_Money;  // 存款
	struct SaveStore Player_1[104];  // 第一个角色的物品
	char A1[8];   // 8个字节不知道是干嘛的， 呵呵

	unsigned __int32 Player_2_Money;  // 存款
	struct SaveStore Player_2[104];  // 第二个角色的物品
	char A2[8];   // 8个字节不知道是干嘛的， 呵呵

	unsigned __int32 Player_3_Money;  // 存款
	struct SaveStore Player_3[104];  // 第三个角色的物品
	char A3[8];   // 8个字节不知道是干嘛的， 呵呵
	
};
#pragma pack()