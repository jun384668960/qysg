
#define shp_REPEAT_SKIP				0xfffe			// [num]循環時跳過的開頭張數, 停留
#define shp_REPEAT_REWIND			0xfffd			// [num]循環最後幾張圖, 停留
#define shp_WAIT_NUMBER				0xfffc			// [num]等待經過張數
//#define shp_WAIT_NUMBER_LOOPA		0xfffb			// [num]等待經過張數, 最後一個設定循環
#define shp_REPEAT_REWIND_INVERT	0xfffa			// [num]循環最後幾張圖逆放, 停留
//#define shp_ATTACK_NUMBER			0xfff9			// [times]設定攻擊次數
//#define shp_COPY_ID					0xfff9			// [base shape name][base copy shape name]自動依照某ID設定，類似複製設定，被參考的ID要在前面
#define shp_REPEAT_PAUSE			0xfff8			// 暫停
#define shp_WAIT_SHAPE				0xfff7			// [shape name]等待播放到某張圖形
#define shp_NEW_TABLE				0xfff6			// [shp table id]換其它 shape table ID 設定
#define shp_REPEAT_WAIT				0xfff5			// [loop]放完時等待
#define shp_WAIT_LOOP				0xfff4			// [loop]等待, 時間是 shape_delay * loop
//
#define shp_FLIP_X					0xfff3			// 左右相反
#define shp_FLIP_Y					0xfff2			// 上下相反
#define shp_MOVE_TO					0xfff1			// [dispx,dispy,speed] 移動位移(到固定地點)
//#define shp_MOVE_SUB_SPEED			0xfff0			// [speed,angle,speed_step] 移動並減速
#define shp_SCROLL_TO_TARGET		0xffef			// 將鏡頭移動到目標(等待)


#define DIR_RIGHT			1
#define DIR_RIGHTDOWN		2
#define DIR_DOWN			3
#define DIR_LEFTDOWN		4
#define DIR_LEFT			5
#define DIR_LEFTUP			6
#define DIR_UP				7
#define DIR_RIGHTUP			8
// ......... 人物附加物件 ID .............
#define game_OBJID_SHADOW			0x80000000
#define game_OBJID_COVER			0x40000000
#define game_OBJID_WEAPON			0x20000000
// .............................
#define objattrDESTROYED        0x08000000
// .............................
#define apiSOUNDPLAYLOOP                0x00000001
#define apiSOUNDPLAYSTOPPREV            0x00000002
#define apiSOUNDPLAYNOTEXIST            0x00000004
// ......... 特殊物件 .............
#define object_SPECIAL_CITY_WALL		999			// 城門
#define object_SPECIAL_CITY_STATUE		1000		// 銅像
// ........ 守城位置 ........
#define gameCITYGUARDTYPE_WEST1			1			// 西門外側
#define gameCITYGUARDTYPE_WEST2			2			// 西門內側
#define gameCITYGUARDTYPE_EAST1			3			// 東門外側
#define gameCITYGUARDTYPE_EAST2			4			// 東門內側
#define gameCITYGUARDTYPE_STATUE1		5			// 雕像左側
#define gameCITYGUARDTYPE_STATUE2		6			// 雕像右側

// ........... 戰場捲動中心位置 .......... (960 - 640) / 2 = 160
//#define batBGScrollXMin				10000		// 背景計算用
//#define batBGScrollXMax				10320		// batBGScrollXMin + 320
//#define batBGScrollYMin				0
//#define batBGScrollYMax				10000
////
//#define batBGCenterX				10160		// batBGScrollXMin + 160
//#define batBGCenterY				10000		// batBGScrollYMax
////
//#define batBG_FrontX				10000		// batBGScrollXMin + 0
//#define batBG_FrontY				10329		// batBGScrollYMax + 329
//
#define batBGScrollXMin				0			// 背景計算用
#define batBGScrollXMax				320			// batBGScrollXMin + 320
#define batBGScrollYMin				0
#define batBGScrollYMax				10000
//
#define batBGCenterX				160			// batBGScrollXMin + 160
#define batBGCenterY				10000		// batBGScrollYMax
//
#define batBG_FrontX				0			// batBGScrollXMin + 0
#define batBG_FrontY				10329		// batBGScrollYMax + 329
//
#define batBGMAX_LAYER				6		// 0 - 5 層
#define batBG_PLANE					43		// planeObject40
//
#define batFarDistance				2000		// 遠距離
#define batFlyHeight				24			// 飛行角色高度
#define batGroundY					10299		// 地板位置
// ........... 人物體型 ...........
#define objSIZE_1					1
#define objSIZE_2					2
#define objSIZE_3					3
//#define objSIZE_4					4
//#define objSIZE_9					9
// ........... 人物動作(與 ShapeTable 設定順序配合) ............
#define act_STAND					0
//#define act_WALK					1
//#define act_DEAD					2		// 死亡
//#define act_DAMAGE					2		// 受傷(同死亡)
//#define act_CAST					3
//#define act_USE						4
//#define act_SPECIAL					5
//
//#define act2_STAND2					6
//#define act2_ATTACK					7		// 攻擊
//#define act2_ATTACK_ST1				8		// 氣力技1
//#define act2_ATTACK_ST2				9		// 氣力技2
//#define act2_ATTACK_ST3				10		// 氣力技3
//#define act2_ATTACK_ST4				11		// 氣力技4
//#define act2_SPECIAL_ATTACK1		12		// 必殺技1
//#define act2_SPECIAL_ATTACK2		13		// 必殺技2
//#define act2_SPECIAL_ATTACK3		14		// 必殺技3
//#define act2_SPECIAL_ATTACK4		15		// 必殺技4
//#define act2_SPECIAL_ATTACK5		16		// 必殺技5
//#define act2_SPECIAL_ATTACK6		17		// 必殺技6
//#define act2_SPECIAL_ATTACK7		18		// 必殺技7
//#define act2_SPECIAL_ATTACK8		19		// 必殺技8
//#define act2_DEFENSE				20
//#define act2_HIDE					21		// 閃躲
//#define act2_DAMAGE_LIGHT			22		// 輕傷
//#define act2_DAMAGE_FLYUP			23		// 重傷飛起
//#define act2_DAMAGE_FLYDOWN			24		// 重傷落下
//#define act2_GROUND					25		// 倒地
//#define act2_SKY_ATTACK				26		// 空中攻擊
//// ... 其它 AI 指令 ...
//#define act2_HIDE_ATTACKBACK		50		// 閃躲並反擊
//#define act2_DEFENSE_ATTACKBACK		51		// 防禦並反擊
//
#define act_STAND_LANCE				0		// W02靜止(槍)
#define act_STAND_SWORD				1		// W01靜止(劍)
#define act_STAND_PIKE				2		// W03靜止(矛)
#define act_STAND_MACHETE			3		// W04靜止(大刀)
#define act_DAMAGE					4		// H01受傷
#define act_DEFENSE					5		// ---- 表示是防禦 ----
#define act_DEFENSE1				5		// D01防禦
#define act_DEFENSE2				6		// D02拿盾防禦
#define act_RUN						7		// R01跑
#define act_WALK					8		// R02小跑
#define act_ATTACK					9		// ---- 表示是攻擊命令 ----
#define act_ATTACK1					9		// A01側砍
#define act_ATTACK2					10		// A02單手直劈
#define act_ATTACK3					11		// A03雙手刺
#define act_ATTACK4					12		// A04直刺
#define act_ATTACK5					13		// A05反橫砍
#define act_ATTACK6					14		// A06半月斬
#define act_ATTACK7					15		// A07昇龍
#define act_ATTACK8					16		// A08後砍
#define act_ATTACK9					17		// A09大喝
#define act_ATTACK10				18		// B01氣旋
#define act_ATTACK11				19		// B02生擒
#define act_ATTACK12				20		// B03刀尾撞擊
#define act_ATTACK13				21		// B04弓射
#define act_ATTACK14				22		// B05飛刀投擲
#define act_ATTACK15				23		// B06一擊
#define act_ATTACK16				24		// B07雙手攻擊
#define act_CAST					25		// ----- 表示是施法命令 -----
#define act_ORDER1					25		// L01指揮
#define act_ORDER2					26		// L02指揮2
#define act_DEAD					27		// ----- 表示是死亡命令 -----
#define act_DEAD1					27		// F01落馬(死亡)
#define act_DEAD2					28		// F02後飛倒地(死亡)
#define act_ATTACK17				29		// 青釭
#define act_ATTACK18				30		// 真空
#define act_ATTACK19				31		// 連刺
#define act_ATTACK20				32		// 亂舞
#define act_LAST					33		// 最後一個動作編號

											// X02未騎馬走路
#define act_SITEDOWN				30		// X03坐下
#define act_FACEDOWN				31		// X01正面面對

// .... 站立武將與 NPC 動作(內部使用，請使用上面騎馬武將定義) ....
#define act_NORIDE_STAND			0
#define act_NORIDE_DAMAGE			1
#define act_NORIDE_DEFENSE			2
#define act_NORIDE_RUN				3
#define act_NORIDE_ATTACK			4
#define act_NORIDE_CAST				5
#define act_NORIDE_DEAD				6
#define act_NORIDE_FACEDOWN			7
#define act_NORIDE_WALK				8
#define act_NORIDE_SITEDOWN			9
// .............................
// 角色行動狀態定義值(objsData1)
#define CHAR_PROCMODE_MOVETO		20
#define CHAR_PROCMODE_DEAD			30
#define CHAR_PROCMODE_ATTACK		40		// 執行攻擊動作
#define CHAR_PROCMODE_MAGIC			45		// 執行施法動作
#define CHAR_PROCMODE_HURT			50
#define CHAR_PROCMODE_AUTOATTACK	60		// 尋找攻擊位置
#define CHAR_PROCMODE_AUTOMAGIC		70		// 尋找攻擊位置
#define CHAR_PROCMODE_SHOW			80		// 必殺技動作設定等待
#define CHAR_PROCMODE_WAIT			81		// 蹲下等等待
#define CHAR_PROCMODE_CAST_WAIT		100		// 施法後等待
// ............... 物件種類 ..................
#define OBJTYPE_COUNTRY_FLAG		1					// 三國旗幟，會依照地圖國籍換圖
#define OBJTYPE_LIGHT				2					// 光源
#define OBJTYPE_ATTACKPASS			3
#define OBJTYPE_LIGHT_SELF			4
// ........... 場景效果 ..............
#define STAGE_EFFECT_DROP_SNOW		1		// 下雪
#define STAGE_EFFECT_DROP_RAIN		2		// 下雨
#define STAGE_EFFECT_DROP_FLOWER	3		// 櫻花
#define STAGE_EFFECT_DROP_MAPLE		4		// 楓葉

#define STAGE_EFFECT_MOVE_LEFT		1		// 背景左移
#define STAGE_EFFECT_MOVE_RIGHT		2		// 背景右移
// ........... NPC Talk 特殊ID ..............
#define NPCTALK_SYSID_BILLBOARD		1000000	// 告示牌
#define NPCTALK_SYSID_CITYFORCE		1000001	// 城勢力
// ........... 地圖旗號 ..............
//#define gameIconSize				64
//
#define mapFlagWall					0x00004000
#define mapFlagWater				0x00008000
#define mapFlagPoison				0x01000000
#define mapFlagFire					0x02000000
//
#define mapFlagPlayer1				0x00010000			// 三國之一, 魏
#define mapFlagPlayer2				0x00020000			// 三國之一, 蜀
#define mapFlagPlayer3				0x00040000			// 三國之一, 吳
#define mapFlagNoCountry			0x00080000			// 無國籍
#define mapFlagEnemy				0x00100000			// 敵方
#define mapFlagNPC					0x00200000			// NPC
#define mapFlagPlayer4				0x00400000			// 其餘玩家勢力
#define mapFlagTreasure				0x00800000

#define mapFlagPlayer				0x004f0000			// 所有玩家
#define mapFlagPlayerAll			0x007f0000			// 所有角色

#define mapFlagBlock_All			0x005f4000			// 所有會阻擋的(分玩家與小兵)
//#define mapFlagBlock_All			0x005f4000			// 所有會阻擋的(分玩家與小兵)
#define mapFlagBlock_All2			0x007f4000			// 所有會阻擋的
//#define mapFlagBlock_Player1		(mapFlagBlock_All & ~mapFlagPlayer1)
//#define mapFlagBlock_Player2		(mapFlagBlock_All & ~mapFlagPlayer2)
//#define mapFlagBlock_Player3		(mapFlagBlock_All & ~mapFlagPlayer3)
#define mapFlagBlock_All_Player		0x004f0000
#define mapFlagBlock_All_WalkSP		0x00104000			// 所有會阻擋的(不包含玩家)

// countryID
#define ID_COUNTRY_NULL				0	// 不屬於任一方(未初始)
#define ID_COUNTRY_WEI				1	// 魏
#define ID_COUNTRY_SHU				2	// 蜀
#define ID_COUNTRY_WU				3	// 吳
#define ID_COUNTRY_NONE				4	// 無
#define ID_COUNTRY_ENEMY			5
#define ID_COUNTRY_NPC				6
#define ID_COUNTRY_FORCE			7	// 其他玩家勢力(轉換 map flag 時使用)
#define ID_COUNTRY_FORCE01			11
#define ID_COUNTRY_FORCE02			12
#define ID_COUNTRY_FORCE03			13
#define ID_COUNTRY_FORCE04			14
#define ID_COUNTRY_FORCE05			15
#define ID_COUNTRY_FORCE06			16
#define ID_COUNTRY_FORCE07			17
#define ID_COUNTRY_FORCE08			18
#define ID_COUNTRY_FORCE09			19
#define ID_COUNTRY_FORCE10			20
#define ID_COUNTRY_FORCE11			21
#define ID_COUNTRY_FORCE12			22

//#define mapFlagID_SHIFT				16
#define mapFlagID_Player1			ID_COUNTRY_WEI
#define mapFlagID_Player2			ID_COUNTRY_SHU
#define mapFlagID_Player3			ID_COUNTRY_WU
#define mapFlagID_NoCountry			ID_COUNTRY_NONE
#define mapFlagID_Enemy				ID_COUNTRY_ENEMY
#define mapFlagID_NPC				ID_COUNTRY_NPC
// ............... 種族 ..................
#define classUNKNOWN				0					// 不明
#define classHUMAN					1					// 人類
#define classBADMAN					2					// 惡人
#define classTHIEF_MON				3					// 山賊
#define classTHIEF_SEA				4					// 海賊
#define classTHIEF					5					// 盜賊
#define classGODMAN					6					// 仙人
#define classGODANIMAL				7					// 神獸
#define classSPIRIT					8					// 亡靈
#define classEVILANIMAL				9					// 妖魔
#define classCRAWLER				10					// 爬蟲
#define classINSECT					11					// 昆蟲
#define classBEAST					12					// 家畜
#define classWILDBEAST				13					// 猛獸
#define classPIG					14					// 野豬
#define classWOLF					15					// 狼
#define classBEAR					16					// 熊
#define classBIRD					17					// 鳥
#define classMACHINE				18					// 機械
#define classUNDEAD					19					// 無生命
#define classPLANT					20					// 植物
#define classARMY_SU				21					// 蜀軍
#define classARMY_WU				22					// 吳軍
#define classARMY_WEI				23					// 魏軍
#define classYELLOWTHIEF			24					// 黃巾賊
#define classARMY_LON_GO			25					// 涼州軍
#define classARMY_GI_GO				26					// 冀州軍
#define classARMY_GWAN_EAST			27					// 關東軍
#define classARMY_SOUTH_SUN			28					// 南陽軍
#define classARMY_INWA_GO			29					// 揚州軍
#define classARMY_GEN_GO			30					// 荊州軍
#define classARMY_E_GO				31					// 益州軍
#define classARMY_TSU_NWU			32					// 匈奴軍
#define classARMY_SOUTH_MAI			33					// 南蠻軍
#define classARMY_WOO_GO			34					// 倭國軍
#define classARMY_I_GO				35					// 夷州軍
#define classTREASURE				36					// 寶箱
#define classSUMMON_CREATURE		37					// 召喚獸
#define classSIEGE_WEAPON			38					// 攻城兵器
#define classPARTY_BOSS				39					// 活動用頭目(不會被剋)
#define classPARTY_BOSS_01			40					// 活動用頭目1
#define classPARTY_BOSS_02			41					// 活動用頭目2
#define classPARTY_BOSS_03			42					// 活動用頭目3
#define classPARTY_BOSS_04			43					// 活動用頭目4
#define classPARTY_BOSS_05			44					// 活動用頭目5
#define classPARTY_BOSS_06			45					// 活動用頭目6
#define classPARTY_BOSS_07			46					// 活動用頭目7
#define classPARTY_BOSS_08			47					// 活動用頭目8
#define classPARTY_BOSS_09			48					// 活動用頭目9
#define classPARTY_BOSS_10			49					// 活動用頭目10
#define classLAST_ONE				49
// ............ 屬性成長 ..............
#define ATTR_LEVEL_1				0					// 一般
#define ATTR_LEVEL_2				1					// 戰士
#define ATTR_LEVEL_3				2					// 術者
#define ATTR_LEVEL_4				3					// 敏捷
#define ATTR_LEVEL_5				4					// 肉腳
#define ATTR_LEVEL_6				5					// 召喚
// ......... 機關人類型 ............
#define ENGINEER_TYPE_STR			0					// 機關人武力型
#define ENGINEER_TYPE_DEX			1					// 機關人反應型
#define ENGINEER_TYPE_INT			2					// 機關人智力型
// ............... 基本職業、稱號 .................
#define jobUNKNOWN					0					// 不明
#define jobWARLORD					0x00000001			// 猛將
#define jobLEADER					0x00000002			// 全能
#define jobADVISOR					0x00000004			// 軍師
#define jobWIZARD					0x00000008			// 方士
//#define job_SPECIAL_ASSIGN			0x80000000			// 特定編號人員
#define jobASSASSIN					0x00000010			// 衛士
#define jobENGINEER					0x00000020			// 機關士
#define jobMONK						0x00000040			// 武僧
//
//#define jobUSE_ALL					0x0000000f
#define jobUSE_ALL					0x0000003f

#define jobID_WARLORD				1				// 猛將
#define jobID_LEADER				2				// 豪傑
#define jobID_ADVISOR				3				// 軍師
#define jobID_WIZARD				4				// 方士
//
#define jobID_ASSASSIN				5				// 衛士
#define jobID_ENGINEER				6				// 機關士
#define jobID_MONK					7				// 武僧
// 新增職業要修改 limits.h 的 gameMAX_JOB_TYPE
// ............... 性別 ..................
#define sexMALE						0x00000001
#define sexFEMALE					0x00000002
// ............... 魔法屬性 ..................
#define skillAttr_NONE				1					// 預設用
#define skillAttr_SLASH				1					// 斬
#define skillAttr_STING				2					// 刺
#define skillAttr_BREAK				3					// 碎
#define skillAttr_ARROW				4					// 射
#define skillAttr_FIRE				5					// 火
#define skillAttr_WATER				6					// 水
#define skillAttr_GOD				7					// 仙
#define skillAttr_EVIL				8					// 妖
//
#define plrSKILL_GEN_MAGIC			0x00000001			// 武將技
#define plrSKILL_SUPER				0x00000002			// 必殺技
#define plrSKILL_SPECIAL			0x00000004			// 特殊技
////
//#define plrSKILL_START_GEN_MAGIC	1
//#define plrSKILL_END_MAGIC			149
//#define plrSKILL_START_SUPER		150
//#define plrSKILL_END_SUPER			249
//#define plrSKILL_START_SPECIAL		250
//#define plrSKILL_END_SPECIAL		349
// .... 魔法目標選擇定義 .....
#define magicSelectType_NONE		0					// 自身
#define magicSelectType_OBJECT		1					// 選物件
#define magicSelectType_POS			2					// 選位置
//
//#define magicSelectType_PARTY_ALL		3				// 隊伍
//#define magicSelectType_PARTY_OBJECT	4				// 隊員
// ............... 物品種類 ..................
#define itemTypeNone				0x00000000			// dummy

#define itemTypeWeapon				0x00000001			// 武器
#define itemTypeArmor				0x00000002			// 身體防具
#define itemTypeHead				0x00000004			// 頭
#define itemTypeFoot				0x00000008			// 腳
#define itemTypeOther				0x00000010			// 飾物
#define itemTypeItem				0x00000020			// 道具(消耗品)
#define itemTypeShield				0x00000040			// 盾牌(槍不可配合)(劍、刀、扇、短棍、飛刀、火藥)
#define itemTypeSmallShield			0x00000080			// 小盾(含在盾牌類中)(槍可配合)(劍、刀、扇、短棍、飛刀、火藥)
#define itemTypeArm					0x00000100			// 護腕
#define itemTypeP					0x00000200			// 披風
#define itemTypeUnderwear			0x00000400			// 內衣
#define itemTypeHorse				0x00000800			// 馬
#define itemTypeArrow				0x00001000			// 箭 
#define itemTypeSoldier				0x00002000			// 兵符

#define itemTypeComposite			0x00004000			// 配方單
#define itemTypeEffect				0x00008000			// 效果(如鞭炮), itemUseMagicID = 效果
#define itemTypeLetto				0x00010000			// 樂透抽獎(如紅包), itemUseMagicID = drop item id, itemLettoGold = 錢範圍
#define itemTypeSoul				0x00020000			// 武魂
#define itemTypeTicket				0x00040000			// 票
#define itemTypeSiegeWeapon			0x00080000			// 攻城兵器(兵符 itemTypeSoldier)
#define itemTypeCard				0x00100000			// 儲值/包月卡(不可賣給商店、不可丟棄)

#define itemTypeNoTrade				0x80000000			// 不可販賣與交易
#define itemTypeNoDrop				0x40000000			// 不可丟
#define itemTypeNoMove				itemTypeNoDrop		// 不可移轉
#define itemTypeSkillBook			0x20000000			// 技能書
#define itemTypeTwoHand				0x10000000			// 兩手(雙手拿同一把武器類)
#define itemTypeSword				0x08000000			// (短兵類)劍類(劍、刀、扇、短棍) -------
#define itemTypeThrow				0x04000000			// (投擲類)飛刀、火藥 ------------------- 必殺技動作同弓類
#define itemTypeBow					0x02000000			// (弓類)射擊類 -------------------------
#define itemTypeLance				0x01000000			// (長兵類)槍類(槍、矛、戟) ------------- (戟有 itemTypeTwoHand 屬性)
#define itemTypeFalchion			0x00800000			// (重兵類)大刀類(大刀、斧、鎚、杖) -----
#define itemTypeTwo					0x00400000			// (雙手類) ----------------------------- 必殺技動作同劍類
#define itemTypeNoUse				0x00200000			// 不可使用
//
#define itemType2MayPutStorage		0x80000000			// 可放到個人倉庫
#define itemType2MayPutArmyStorage	0x40000000			// 可放到軍團倉庫(保留未使用)
#define itemType2NoSell				0x20000000			// 不可賣商店

#define itemType2GEM				0x00800000			// 鑲嵌寶石類
#define itemType2Engineer			0x00400000			// 機關人(兵符)
#define itemType2WAWA				0x002000000			// 新紙娃娃造型裝備
#define itemType2DUEDATE			0x001000000			// 有期限的道具

#define itemType2HandArmor			0x00010000			// 手甲(雙手)
#define itemType2DoubleDagger		0x00020000			// 雙匕(重兵)
#define itemType2Boxing				0x00040000			// 拳套(雙手)
#define itemType2Chain				0x00080000			// 鎖鏈(重兵)

#define itemTypeEQUIP				0x00001fdf			// 可裝備類物品
#define itemTypeSEAL				0x1bc00fde			// 可封印類物品(不含箭、投擲)
#define itemTypeBLESS				0x1bc007ce			// 可祝福類物品(不含馬、箭、投擲、飾品)
#define itemTypeALL_ARMOR			0x000007de			// 所有防具類(不含馬)
#define itemTypeALL_Shield			0x000000c0			// 所有盾牌
#define itemTypeALL_WEAPON			0x1b800001			// 所有武器類(合成用，不包括箭與投擲類)

#define itemType2EQUIP				0x000f0000			// 可裝備類物品
#define itemType2ALL_WEAPON			0x000f0000			// 所有武器類(合成用，不包括箭與投擲類)
//--xiun 04/05/20--
	//左手可裝備：箭、盾、小盾
#define itemTypeLeftHand	(itemTypeArrow | itemTypeShield | itemTypeSmallShield)

	//右手可裝備：對應左手的武器(弓、劍類、投擲類、槍類)或雙手持武器
//#define itemTypeRightHand	(itemTypeBow | itemTypeSword | itemTypeThrow | itemTypeLance | itemTypeFalchion | itemTypeTwoHand | itemTypeTwo)
//右手對應到的都有itemTypeWeapon屬性
#define itemTypeRightHand	itemTypeWeapon

#define itemTypeHand	(itemTypeLeftHand | itemTypeRightHand)
//--end xiun--

//
//#define itemType1Final				12					// 劍: 最後物品號碼
//#define itemType2Final				24					// 杖
//#define itemType3Final				32					// 拳爪
//#define itemType4Final				39					// 小刀
//#define itemType5Final				50					// 槍
//#define itemType6Final				59					// 弓
//#define itemType7Final				66					// 斧
////
//#define itemTypeHelmFinal			101
//#define itemTypeArmorFinal			146
//#define itemTypeFootFinal			175
//#define itemTypeItemStart			250
//#define itemTypeItemFinal			252

// .... 特殊物品定義 ......
#define item_Nothing				0
#define item_Money					0xffffffff
#define item_Exp					0xfffffffe
#define item_SkillExp				0xfffffffd
#define item_Honor					0xfffffffc
// .... 物品 ShowData 中的 flag 定義(只有低WORD) ......
#define itemSHOW_FLAG_NPC			0x0001				// 小兵物品
#define itemSHOW_FLAG_SOUL			0x0002				// 武魂
#define itemSHOW_FLAG_STICKET		0x0004				// 武魂票
#define itemSHOW_FLAG_SIEGE			0x0008				// 攻城兵器(小兵物品)
#define itemSHOW_FLAG_CARD			0x0010				// 儲值月卡
#define itemSHOW_FLAG_BROKEN		0x0020				// 損壞的裝備(狀態)
#define itemSHOW_FLAG_BLESS_20		0x0040				// 祝福上限可到20(狀態)
#define itemSHOW_FLAG_NO_COMMAND	0x0080				// 小兵拒絕效命
#define itemSHOW_FLAG_ENGINEER		0x0100				// 機關人(小兵物品)
#define itemSHOW_FLAG_GEM			0x0200				// 鑲嵌類裝備品
//
#define itemSHOW_FLAG_FUNCTION2		0x8000				// Function2 修正小兵狀態資料

#define itemSHOW_FLAG_ALL_CNPC		0x0109				// 所有小兵類物品
#define itemSHOW_FLAG_ALL			(itemSHOW_FLAG_NPC | itemSHOW_FLAG_SOUL | itemSHOW_FLAG_STICKET | itemSHOW_FLAG_SIEGE | itemSHOW_FLAG_CARD | itemSHOW_FLAG_ENGINEER)
// .... 特殊魔法ID(只有定義) ....
#define magic_ADD_ST_CELL			0xffff				// 氣力增加
#define magic_GOOD_LUCK				0xfffe				// 轉運
#define magic_ADD_EXP				0xfffd				// 經驗
#define magic_ADD_SKILL_EXP			0xfffc				// 技能
#define magic_ADD_HONOR				0xfffb				// 功勳
#define magic_ADD_CNPC_EXP			0xfffa				// 小兵經驗
#define magic_UNDEAD				0xfff9				// 免死符
#define magic_ADD_MAX_ITEM_CARRY	0xfff8				// 加大攜帶物品
#define magic_ADD_MAX_ITEM_STORAGE	0xfff7				// 加大倉庫物品
#define magic_ADD_CHAR_SLOT			0xfff6				// 增加角色
#define magic_ALL_CHANNEL			0xfff5				// 國頻
#define magic_PERSIONAL_SHOP		0xfff4				// 個人商店
#define magic_ADD_MONTH_EXP			0xfff3				// 包月符: 經驗
#define magic_ADD_MONTH_SKILL_EXP	0xfff2				// 包月符: 技能經驗
#define magic_ADD_MONTH_CNPC_EXP	0xfff1				// 包月符: 士兵經驗
#define magic_ANNOUNCEMENT			0xfff0				// 勝利宣言
#define magic_AUTO_FUNCTION			0xffef				// 自動功能
#define magic_RENAME				0xffee				// 改名
#define magic_ADD_CNPC_ROYALTY		0xffed				// 增加忠誠度(物品)
#define magic_GET_PLAYER_POS		0xffec				// 得到他人位置
#define magic_SOLDIER_RESURRECT		0xffeb				// 小兵復活(物品)
#define magic_SOLDIER_UNDEAD		0xffea				// 小兵免死
#define magic_ADD_CNPC_EXP_VAL		0xffe9				// 增加小兵經驗值
#define role_ADD_CNPC_EXP_VAL		0xffe9				// 增加小兵經驗值(為了輸出定義)
#define magic_ADD_MONTH_3_IN_1		0xffe8				// 包月符(三合一): 經驗、士兵經驗、打寶加倍
#define magic_SEAL_ITEM				0xffe7				// 封印物品
#define magic_UNSEAL_ITEM			0xffe6				// 解封物品
#define magic_BLESS_ITEM_20			0xffe5				// 祝福上限可以更多(指定裝備)
#define magic_SOLDIER_RECOMMAND		0xffe4				// 解除小兵拒絕效命狀態
#define magic_CHANGE_SEX			0xffe3				// 變性
#define magic_BLESS_15_TO_20_WEAPON	0xffe2				// 特殊祝福 15~20
#define magic_BLESS_15_TO_20_ARMOR	0xffe1				// 特殊祝福 15~20
#define magic_RESTORE_CRIME_VAL		0xffe0				// 回復懲罰點
#define magic_ADD_EXP_CNPCEXP		0xffdf				// 經驗、士兵經驗加倍
#define magic_CNPC_CHANGE_CODE		0xffde				// 小兵變身
//#define magic_FIX_BROKEN_ITEM		0xffdd				// 修復損壞裝備(未使用)

#define magic_HORSE2				550
// ............... 敵人出現方式 ..................
#define APPEAR_TYPE_DAY				1
#define APPEAR_TYPE_NIGHT			2
// ---------------- 商店類型 ----------------
#define shopType_WeaponArmor		0					// 武器店
#define shopType_Item				1					// 道具店
#define shopType_Gamble				2					// 賭場
#define shopType_Army				3					// 兵營
#define shopType_Storage			4					// 倉庫(錢莊)
//
#define shopType_Factory			5					// 工坊
#define shopType_Arena				6					// 比武場
#define shopType_Market				7					// 市場
#define shopType_Lab				8					// 合成屋
#define shopType_Weapon				9					// 武器舖
#define shopType_CityHall			10					// 宮殿
#define shopType_Hotel				11					// 旅店
#define shopType_Store				12					// 雜貨舖
#define shopType_BookStore			13					// 書店
#define shopType_OrganizePVP		14					// 軍團競技賭場
#define shopType_Any				15					// 隨身商店
//
// ---------------- Item Mode 物品類型 -----------------
#define ishopType_General			1
#define ishopType_Funcion			2
#define ishopType_Special			3
#define ishopType_Cheap				4
// 新的狀態
#define ishopType_Fun_General				1		// 一般
#define ishopType_Double					2		// 加倍
#define ishopType_Help						3		// 輔助
//
#define ishopType_Scroll					4		// 捲軸
#define ishopType_Soldier					5		// 士兵
#define ishopType_Other						6		// 其他
//
#define ishopType_Wawa_Head					7		// 扮裝頭盔
#define ishopType_Wawa_Armor				8		// 扮裝鎧甲
#define ishopType_Wawa_Foot					9		// 扮裝鞋
#define ishopType_Wawa_P					10		// 扮裝披風
#define ishopType_Wawa_Horse				11		// 扮裝座騎
#define ishopType_Wawa_Other				12		// 全身造型
//
#define ishopType_FD_General				13		// 福袋一般
#define ishopType_FD_Equip					14		// 福袋武器裝備
#define ishopType_FD_Composite				15		// 福袋配方材料
#define ishopType_FD_Soldier				16		// 福袋士兵
#define ishopType_FD_Horse					17		// 福袋座騎
//
#define ishopType_Hot_Sale					18		// 熱賣商品

// ---------------- Flag Copy ----------------------
#define sysflagExitGame 			0x40000000
#define sysflagReplayGame			0x20000000
// ----------------- AI --------------------
// 目標選擇
#define batAI_TARGET_RANDOM			0				// 隨機
#define batAI_TARGET_LEVELMAX		1				// 等級
#define batAI_TARGET_LEVELMIN		2
#define batAI_TARGET_HPMAX			3				// 生命
#define batAI_TARGET_HPMIN			4
#define batAI_TARGET_ATTACKMAX		5				// 攻擊力
#define batAI_TARGET_ATTACKMIN		6
#define batAI_TARGET_NEAREST		7
#define batAI_TARGET_FAREST			8
#define batAI_TARGET_DYING			9				// 瀕死
//#define batAI_TARGET_SEQUENCE		9				// 依順序
#define batAI_TARGET_STATUSERROR	10				// 狀態異常
#define batAI_TARGET_NEEDRESTOREHP	11				// 需要補血
#define batAI_TARGET_NEEDRESTOREMP	12				// 需要補MPDP
#define batAI_TARGET_NEEDHELP		13				// 其它幫助
// 目標次選擇
#define AIF_SPELLER					1				// 施法者
#define AIF_FAR_ATTACKER			2				// 遠距離攻擊者
#define AIF_FLYER					3				// 會飛行者
// 攻擊方式
#define batAI_ATTACK_NONE			0				// 未設定指令
#define batAI_ATTACK_NORMAL			1				// 目標敵方
//#define batAI_ESCAPE				2
#define batAI_ATTACK_SKILL			3				// 目標敵方
#define batAI_USE_ITEM				4				// 目標敵方
#define batAI_HELP_SKILL			5				// 目標
#define batAI_USE_HELP_ITEM			6				// 目標
//#define batAI_ESCAPE_SELF			7
//#define batAI_DEFENSE				8
//#define batAI_ST					9
//#define batAI_CHECK					10				// 目標
//#define batAI_CKILL					11				// 目標
//#define batAI_GUARD					12				// 目標
//#define batAI_SUMMONBEAST			13
//#define batAI_STEAL					14				// 目標
//#define batAI_HIDDEN				15
//#define batAI_CKILL2				16				// 目標
//#define batAI_CKILL3				17				// 目標
//#define batAI_CKILL4				18				// 目標
//#define batAI_CKILL5				19				// 目標
//#define batAI_ELF					20
//#define batAI_TALK					21
//#define batAI_HELP_SKILL_BY_PITY	22
//#define batAI_ESCAPE_SELF_OK		23				// 100% 自己逃走
//#define batAI_CHANGE_ENEMY			24				// 變身
//#define batAI_SUCK_ENEMY			25				// 敵吸收敵人
// .......................................
#define effRange_SINGLE				0
#define effRange_MULTI				1
// ................. 目標種類 ......................
#define effTARGET_ENEMY				0				// 怪物
#define effTARGET_SELF				1				// 自己
#define effTARGET_SELF_ALL			2				// 所有玩家(含自己，區分國戰期)
#define effTARGET_NONE				3				// 無(如召換系)
#define effTARGET_CARRY_NPC			4				// 攜帶小兵
#define effTARGET_SELF_CNPC_PARTY	5				// 自己隊友小兵
//
#define effTARGET_PARTY_ALL			6				// 隊友全體
#define effTARGET_PARTY_SINGLE		7				// 隊友單體(目前只有傳送)
#define effTARGET_MATE				8				// 伴侶(目前只有傳送)
// ................. 地圖屬性 ......................
#define mapMode_CountryWar			1				// 國戰
#define mapMode_FreeOPK				2				// 亂鬥PK
#define mapMode_OrganizePK			3				// 組織PK
#define mapMode_SoulBattle			4				// 武魂戰場
#define mapMode_HistoryBattle		5				// 歷史戰役
// ................. BOSS傳送屬性 ......................
#define bossTELEPORT_REBORN			1				// 重生之後傳送玩家
#define bossTELEPORT_DIE			2				// 死亡時傳送玩家
// ---------------- 勝利失敗條件 ----------------
//#define wf_ENEMY_ALL_DESTROYED		1
//#define wf_ROLE_DYING				2
//#define wf_ARRIVE_POS				3
//#define wf_WAIT_ROUND				4
////
//#define wff_PLAYER_ALL_DESTROYED	10
//#define wff_ROLE_DYING				11
//#define wff_WAIT_ROUND				12
// --------------------- 特殊屬性 ----------------------
// 特殊技定義
#define spcRUN									 1	// 跑步
#define spcTAKEUP								 2	// 撿拾
#define spcHORSE_START_RUN						 3	// 馬術_起跑
#define spcSITE									 4	// 休息
#define spcHP_RESTORE							 5	// 體力回復
#define spcMP_RESTORE							 6	// 技力回復
#define spcST_RESTORE							 7	// 氣力增加
#define spcAUTO_USEITEM_HP						 8	// 道具補血
#define spcAUTO_USEITEM_MP						 9	// 道具補技
#define spcDELAYFAST_ITEM						10	// 道具熟練
#define spcDELAYFAST_SKILL						11	// 武將技熟練
#define spcDEAD_EXP_LOSE						12	// 死亡經驗值減少
#define spcAUTO_ATTACK							13	// 主動攻擊
#define spcAUTO_ATTACK_BACK						14	// 自動反擊
#define spcBUY_LOW								15	// 低價買進
#define spcSELL_HIGH							16	// 高價賣出
#define spcPERSIONAL_SHOP						17	// 個人商店
#define spcCOMPOSITE_WEAPON						18	// 武器合成
#define spcCOMPOSITE_ARMOR						19	// 防具合成
#define spcCOMPOSITE_ITEM						20	// 道具合成
#define spcCOMPOSITE_SKILL						21	// 合成熟練
#define spcSKILL_FISH							22	// 釣魚
#define spcSKILL_WOOD							23	// 伐木
#define spcSKILL_MINE							24	// 採礦
#define spcSKILL_CHANGE_WEAPON					25	// 武器切換
#define spcSKILL_CRITICAL_HIT					26	// 爆擊
#define spcSKILL_WMASTER_SWORD					27	// 短兵武器熟練 
#define spcSKILL_WMASTER_LANCE					28	// 長兵武器熟練
#define spcSKILL_WMASTER_FALCHION				29	// 重兵武器熟練
#define spcSKILL_WMASTER_BOW					30	// 弓熟練
#define spcSKILL_WMASTER_THROW					31	// 投擲武器熟練
#define spcSKILL_WMASTER_TWO					32	// 雙手武器熟練
#define spcSKILL_MAGIC_DAMAGE					33	// 武將技傷害
#define spcSKILL_MAGIC_EH_ADD_HP				34	// 命療加成
#define spcSKILL_MAGIC_EH_ADD_HITMISS			35	// 迴避加成
#define spcSKILL_MAGIC_EH_ADD_SKILLEXP			36	// 技能點上限提昇
#define spcSKILL_MAGIC_EH_MAX_HP				37	// 體力提昇
#define spcSKILL_MAGIC_EH_MAX_MP				38	// 技力提昇
#define spcSKILL_MAGIC_EH_USE_ITEM				39	// 道具使用加成
#define spcSKILL_MAGIC_ST_DAMAGE_DOUBLE			40	// 怒氣爆擊
#define spcSKILL_MAGIC_ARROW_PACK				41	// 箭袋
#define spcSKILL_MAGIC_EXPEND_DOWN				42	// 消費技力降低
#define spcSKILL_MAGIC_DAMAGEX2_SUPER			43	// 必殺技爆擊
#define spcSKILL_MAGIC_DAMAGEX2_GEN				44	// 武將技爆擊
#define spcSKILL_MAGIC_SOLDIER_ARRAY_ATTR		45	// 陣形熟練
#define spcSKILL_MAGIC_ADD_WEIGHT				46	// 負重提昇
//#define spcSKILL_MAGIC_EH_ADD_SKILLEXP2			44	// 技能點上限提昇

// 特殊旗號定義
#define spcFLAG_GM					0x00008000		// GM 角色(大)
#define spcFLAG_GM2					0x00004000		// GM 角色(小)
#define spcFLAG_INVISIBLE			0x00002000		// GM 角色: 隱形
#define spcFLAG_GODMODE				0x00001000		// GM 角色: 無敵
//
#define spcFLAG_NO_BACKOFF			0x00000800		// 不會被震退
#define spcFLAG_STATUE				0x00000400		// 雕像
#define spcFLAG_GATE				0x00000200		// 城門
#define spcFLAG_STATUE_NO_ORG		0x00000100		// 雕像(無城主)

#define spcFLAG_ADDCOLOR			0x00000001		// 角色加色處理
#define spcFLAG_MIX					0x00000002		// 角色半透明處理
#define spcFLAG_COVER_ONLY			0x00000008		// 上面處理只套用在套色圖上(複合旗號)

#define CHAR_NO_LEVEL_UP			0x00010000		// 不會升級
#define CHAR_NO_ADD_HP				0x00020000		// 不能補血
#define CHAR_TRAP					0x00040000		// 陷阱怪
#define CHAR_NO_NOATTACK			0x00080000		// 不主動攻擊無效
#define CHAR_CLEAR_ALL_SET			0x000f0000		// 清除額外設定
// ------------------ 特殊的魔法 ID -------------------
#define magic_Resurrect				4
#define magic_MP_TO_ST				7
#define magic_AddAttackSpeed		10 //神速法：內掛顯示用
#define magic_AddMoveSpeed			12 //神行：內掛顯示用
#define magic_AddSPSpeed			13 //集氣法：內掛顯示用
#define magic_DefenseShell			14 //結界法：內掛顯示用
#define magic_SoldierStrengthen		15 //兵強化：內掛顯示用
#define magic_ArmySoul				16 //軍魂：內掛顯示用
#define magic_Teleport				18
#define magic_TownPortal			19
#define magic_HeartOfWar			30 //戰鬼之心：內掛顯示用
#define magic_HeartOfDefense		31 //守護之心：內掛顯示用
#define magic_ST_TO_MP				78
#define magic_ST_TO_MP_2			447 //明鏡止水
#define magic_ENH_MAGIC_DMG			85
#define magic_MagicShield			109	//七星轉命
#define magic_EnhanceDefense		110	//羅剎之盾
#define magic_SoldierStrengthen2	111	//行軍強化(兵強化2)：內掛顯示用
#define magic_BossCallSoldier		427
#define magic_ST_ATTACK				402 //狂攻法
#define magic_ST_SLIENT				404 //凝神法
#define magic_NO_SPACE				123	// 真空
#define magic_MATE_TELEPORT			439	// 配偶傳送
#define magic_FIX_CITY_GATE			449 // 城門修復
#define magic_MP_TO_ST2				99
#define magic_MP_TO_ST3				537

//#define magic_START_ALL1			// 休息 使用 magic_SITE
//#define magic_START_ALL2			// 跑步 使用 magic_RUN
#define magic_START_ADVISOR			37				// 軍師：地矛
#define magic_START_WIZARD			41				// 方士：赤焰
#define magic_START_ENGINEER		37				// 機關士：地矛
#define magic_Leech_HP				64
#define magic_Leech_MP				65
#define magic_Summon_Start			67				// 含
#define magic_Summon_End			72				// 含

#define magic_ADD_ATTACK			8		// 鐵腕法
#define magic_ADD_DEFENSE			9		// 剛體法
#define magic_ADD_HIT				11		// 心眼法

#define magic_Summon_Spec			95				// 召喚糜鹿
#define magic_Summon_Spec001		105				// 召喚鬼火
#define magic_Summon_Spec002		106				// 召喚鬼魅
#define magic_Summon_Spec003		107				// 召喚鬼刀兵
#define magic_Summon_Spec004		108				// 召喚鬼槍兵
#define magic_Summon_Spec005		112				// 召喚舞獅
#define magic_Summon_Spec006		357				// 召喚金豬
#define magic_Summon_Spec007		358				// 召喚
#define magic_Summon_Spec008		359				// 召喚
#define magic_Summon_Spec009		360				// 召喚

#define magic_Summon_Spec010		432				// 召喚
#define magic_Summon_Spec011		433				// 召喚
#define magic_Summon_Spec012		434				// 召喚

#define magic_Summon_Spec013		540				// 召喚
#define magic_Summon_Spec014		541				// 召喚

#define magic_SUCK_DAMAGE			104		// 傷害吸收後攻擊
// ----------------- 作用類型 -----------------
#define effFun_ATTACK			0x80000000			// 攻擊用, 由程式指定
#define effFun_ANTISTATUS		0x40000000			// [...]消除屬性
#define effFun_ADD_HP			0x20000000			// [power]治療
#define effFun_ADD_MP			0x10000000			// [power]回復MP
// PS 記錄顯示用
#define effFun_GATE_CLOSE		0x80000000			// 城門關閉狀態
#define effFun_STATUE_NO_ORG	0x40000000			// 雕像(無城主)
#define effFun_STATUE			0x20000000			// 雕像
#define effFun_GATE				0x10000000			// 城門
#define effFun_INVISIBLE		0x08000000			// Client效果：隱形(進商店)
#define effFun_DEAD				0x04000000			// Client效果：死亡
#define effFun_SITE				0x02000000			// Client效果：坐下
#define effFun_GM				0x01000000
#define effFun_PERSIONAL_SHOP	0x00000400			// 擺攤狀態
#define effFun_SPECIAL_ALL		0x0f000c00			// 統一清除旗號	0x0f000400
// 登入時判斷是否角色刪除用
#define effFun_DELETED			0x04000000
// 輔助狀態
#define effFun_ADDATTACKPOWER	0x00800000			// [round][power(%)]增加攻擊力, HitRatio 100%
#define effFun_ADDDEFENSEPOWER	0x00400000			// [round][power(%)]增加防禦力, HitRatio 100%
#define effFun_ADDMAGICPOWER	0x00200000			// [attr][round][power(%)]增加魔擊力
#define effFun_ADDMAGICDEFENSE	0x00100000			// [attr][round][power(%)]增加魔法防禦力
#define effFun_ADDATTACKSPEED	0x00080000			// [round][power(%)]增加攻速
#define effFun_ADDMOVESPEED		0x00040000			// [round][power(%)]增加移動
#define effFun_ADDHIT			0x00020000			// [round][power(%)]增加命中率
#define effFun_ADDHITMISS		0x00010000			// [round][power(%)]增加迴避率
#define effFun_FAST_ADD_ST		0x00004000			// [round][power] 加速回氣力
//
#define effFun_RESURRECT		0x00008000			// (資料設定狀態)復活	// 死亡後 n秒可以復活
#define effFun_CITYGUARD		0x00008000			// 守城武將或士兵
#define effFun_ADVISOR_ADDHP	0x00000200			// (資料設定狀態)軍師特別補血
#define effFun_INNER_DELETED	0x00000200			// (Client內部用)
#define effFun_MALE				0x00002000			// 敵人 plrDATASHOW_2 結構：男性
#define effFun_FEMALE			0x00001000			// 敵人 plrDATASHOW_2 結構：女性

#define effFun_MAGIC_ENH_DMG	0x00002000			// 武將技威力加成
#define effFun_GET_FLAG			0x00001000			// 玩家取得旗子
#define effFun_GET_VIP			0x00001000			// 玩家是 VIP
//#define effFun_HISTORY_CHAR		0x00000800			// 有這個才能在歷史戰役PK
//
#define effFun_MARK_MAKE		0x00000800			// 物品：標記用(復活)
#define effFun_MARK_RESURRECT	0x00000800
#define effFun_MARK_TELEPORT	0x00000400			// 物品：傳送到標記處(擺攤)
// 異常狀態
#define effFun_POISON			0x00000001			// [ratio][round][damage]
#define effFun_STUN				0x00000002			// [ratio][round]
#define effFun_WEAKEN			0x00000004			// [ratio][round][damage]
#define effFun_SLOW				0x00000008			// [ratio][round][damage]
#define effFun_NO_MAGIC			0x00000010			// [ratio][round]
#define effFun_NO_SPEC			0x00000020			// [ratio][round]
//
#define effFun_ENH_DEFENSE		0x00000040			// [round]猛豪血牛技能
#define effFun_MAGIC_SHIELD		0x00000080			// [round][power(%)]技力吸收傷害, HitRatio 100%
//
#define effFUN_STATUS_ALL		0x0000003f			// 所有狀態魔法
#define effFUN_STATUS_HELP_ALL	0x00ff60c0			// 所有輔助魔法
//
#define effFun_RED				0x01000000			// 紅人
//
#define effFun_WIZARD_SUMMON	0x00000100			// 方式召喚的召喚獸
// ......... 新的狀態 ........
#define effFun2_NO_MOVE					0x00000001	// 定身，不能移動
#define effFun2_NO_USE_ITEM				0x00000002	// 封印，不能使用物品
#define effFun2_NO_BLESS				0x00000004	// 抹消，不能附加輔助狀態
#define effFun2_REDUCE_DEFENSE			0x00000008	// 弱化，降低防禦力
#define effFun2_REDUCE_ATTACK_POWER		0x00000010	// 虛脫，降低攻擊力、必殺技威力
#define effFun2_REDUCE_MAGIC_POWER		0x00000020	// 減弱，降低武將技威力
#define effFun2_REDUCE_ITEM_ADD_HP		0x00000040	// 劣化，降低道具補血效果
#define effFun2_REDUCE_GEN_ADD_HP		0x00000080	// 鈍化，降低武將技補血效果
#define effFun2_REDUCE_HIT_RATIO		0x00000100	// 盲目，降低一般攻擊命中率
#define effFun2_REDUCE_SPECIAL_HIT		0x00000200	// 失誤，降低必殺技命中率
#define effFun2_REDUCE_GEN_HIT			0x00000400	// 誤算，降低武將技命中率
#define effFun2_SPECIAL_LOST_HP			0x00000800	// 詛咒，使用必殺技時減少體力
#define effFun2_GEN_LOST_MP				0x00001000	// 混亂，使用武將技時減少技力
#define effFun2_BECOME_TURTLE			0x00002000	// 龜形
#define effFun2_BECOME_PERSON			0x00004000	// 人形
//
#define effFun2_HELP_ADD_ATTACK_POWER	0x80000000	// 憤怒，可增加一般攻擊力，但受到傷害加倍
#define effFun2_ANTISTATUS				0x40000000	// [...]消除屬性
#define effFun2_HELP_ADD_MAGIC_POWER	0x20000000	// 憎恨，可增加武將技攻擊力，但受到傷害加倍
#define effFun2_HELP_BOSS_CALL			0x10000000	// 呼叫，BOSS呼叫小兵回到身邊

#define effFun2_EQUIP_NO_ATTACK			0x08000000	// 特殊用: 怪不主動攻擊(Client狀態用，主要紀錄是在 plrData->plrFlags)
#define effFun2_SOLDIER_NO_COMMAND		0x04000000	// 特殊用: 士兵拒絕效命

#define effFUN2_STATUS_ALL				0x00007fff	// 所有狀態魔法
#define effFUN2_STATUS_HELP_ALL			0xb0000000	// 所有輔助魔法

// ............... 商城物品狀態 ..................
#define ITEMMALL_ATTR_NEW				0x00000001	// 新品上架
#define ITEMMALL_ATTR_CHEAP				0x00000002	// 特價中
#define ITEMMALL_ATTR_SUGGEST			0x00000004	// 推薦商品
//// ===== Server 內部用的紀錄狀態 =====
//#define effFun_GM_MODE_INVISIBLE	0x80000000		// GM 隱形狀態
// ....... 物品附加屬性 MOD ........
#define iMOD_TYPE_ADDATTR_STR				1
#define iMOD_TYPE_ADDATTR_INT				2
#define iMOD_TYPE_ADDATTR_MIND				3
#define iMOD_TYPE_ADDATTR_CON				4
#define iMOD_TYPE_ADDATTR_DEX				5
#define iMOD_TYPE_ADDATTR_LEADER			6
#define iMOD_TYPE_ADDATTR_ALL				7			// (未使用)
//
#define iMOD_TYPE_MAGICATTACK_SLASH			8			// 斬
#define iMOD_TYPE_MAGICATTACK_STING			9			// 刺
#define iMOD_TYPE_MAGICATTACK_BREAK			10			// 碎
#define iMOD_TYPE_MAGICATTACK_ARROW			11			// 箭
#define iMOD_TYPE_MAGICATTACK_FIRE			12			// 火
#define iMOD_TYPE_MAGICATTACK_WATER			13			// 水
#define iMOD_TYPE_MAGICATTACK_GOD			14			// 仙
#define iMOD_TYPE_MAGICATTACK_EVIL			15			// 妖
#define iMOD_TYPE_MAGICATTACK_ALL			16			// (未使用)
//
#define iMOD_TYPE_MAGICDEFENSE_SLASH		17			// 斬
#define iMOD_TYPE_MAGICDEFENSE_STING		18			// 刺
#define iMOD_TYPE_MAGICDEFENSE_BREAK		19			// 碎
#define iMOD_TYPE_MAGICDEFENSE_ARROW		20			// 箭
#define iMOD_TYPE_MAGICDEFENSE_FIRE			21			// 火
#define iMOD_TYPE_MAGICDEFENSE_WATER		22			// 水
#define iMOD_TYPE_MAGICDEFENSE_GOD			23			// 仙
#define iMOD_TYPE_MAGICDEFENSE_EVIL			24			// 妖
#define iMOD_TYPE_MAGICDEFENSE_ALL			25			// (未使用)
//
#define iMOD_TYPE_BLESS						26			// 祝福(增加攻擊力或防禦力)
#define iMOD_TYPE_ADDHIT					27
#define iMOD_TYPE_ADDMISS					28
#define iMOD_TYPE_ADDATTACKSPEED			29
#define iMOD_TYPE_KILL						30			// (未使用)
#define iMOD_TYPE_SUB_CAST_DELAY			31
#define iMOD_TYPE_NO_STOP_CAST				32
#define iMOD_TYPE_ADDHP						33
#define iMOD_TYPE_ADDMP						34
//
#define iMOD_TYPE_ANTI_POISON				40
#define iMOD_TYPE_ANTI_STUN					41
#define iMOD_TYPE_ANTI_WEAKEN				42
#define iMOD_TYPE_ANTI_SLOW					43
#define iMOD_TYPE_ANTI_NO_MAGIC				44
#define iMOD_TYPE_ANTI_NO_SPEC				45
#define iMOD_TYPE_ANTI_ALL					46
//
#define iMOD_TYPE_DAMAGEX2_RATIO			47
//
#define iMOD_TYPE_SKILLATTACK_SLASH			48			// 技能屬性攻：斬
#define iMOD_TYPE_SKILLATTACK_STING			49			// 技能屬性攻：刺
#define iMOD_TYPE_SKILLATTACK_BREAK			50			// 技能屬性攻：碎
#define iMOD_TYPE_SKILLATTACK_ARROW			51			// 技能屬性攻：箭
#define iMOD_TYPE_SKILLATTACK_FIRE			52			// 技能屬性攻：火
#define iMOD_TYPE_SKILLATTACK_WATER			53			// 技能屬性攻：水
#define iMOD_TYPE_SKILLATTACK_GOD			54			// 技能屬性攻：仙
#define iMOD_TYPE_SKILLATTACK_EVIL			55			// 技能屬性攻：妖
// 套裝功能
#define iMOD_TYPE_ADD_ATTACK				56			// 攻擊力
#define iMOD_TYPE_ADD_DEFENSE				57			// 防禦力
#define iMOD_TYPE_ADD_MAGIC_POWER			58			// 魔法攻擊力
#define iMOD_TYPE_LEECH_HP					59			// 吸血
#define iMOD_TYPE_LEECH_MP					60			// 吸魔
#define iMOD_TYPE_LEECH_ST					61			// 吸氣
// new 2010/09/27
#define iMOD_TYPE_REDUCE_SUPER_DMG			62			// 減少必殺技傷害 = 總防禦加持
#define iMOD_TYPE_REDUCE_GEN_MAGIC_DMG		63			// 減少武將技傷害

// 任務狀態
#define gameMISSION_STATE_NONE			0
#define gameMISSION_STATE_SUCCESS		1
#define gameMISSION_STATE_FAIL			2

#define		PRIVATE_MSG_CHAR	'"' //密語時用以指定人名的前置字元，人名後空一格
#define		FACE_SYMBOL_CHAR	'!' //使用表情符號的前置字元，後接表情符號id

//#define		SYMBOL_CHAR_RESPONSE	']'		// 直接回復密語
#define		SYMBOL_CHAR_PARTY	'#'		// 隊伍頻道
#define		SYMBOL_CHAR_ORG		'@'		// 軍團頻道
#define		SYMBOL_CHAR_CHAT	'&'		// 聊天頻道(公頻)
#define		SYMBOL_CHAR_ORG_ALLY	'$'		// 軍團共同頻道
