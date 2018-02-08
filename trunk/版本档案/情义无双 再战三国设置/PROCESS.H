#define planeBG3                    0
#define planeBG2                    1
#define planeBG1                    2
#define planeIcon                   3
#define planeShadow                 4
#define planeObject1                5
#define planeObject2                6
#define planeObject3                7
#define planeObject4                8
#define planeObject5                9
#define planeObject6                10
#define planeObject7                11
#define planeObject8                12
#define planeObject9                13
#define planeObject10               14
#define planeObject11               15
#define planeObject12               16
#define planeObject13               17
#define planeObject14               18
#define planeObject15               19
#define planeObject16               20
#define planeObject17               21
#define planeObject18               22
#define planeObject19               23
#define planeObject20               24
#define planeObject21               25
#define planeObject22               26
#define planeObject23               27
#define planeObject24               28
#define planeObject25               29
#define planeObject26               30
#define planeObject27               31
#define planeObject28               32
#define planeObject29               33
#define planeObject30               34
#define planeObject31               35
#define planeObject32               36			// 以下為戰鬥使用
#define planeObject33               37
#define planeObject34               38
#define planeObject35               39
#define planeObject36               40
#define planeObject37               41
#define planeObject38               42
#define planeObject39               43
#define planeObject40               44
#define planeObject41               45
#define planeObject42               46
#define planeObject43               47
#define planeObject44               48
#define planeObject45               49
#define planeObject46               50
#define planeObject47               51
#define planeObject48               52
#define planeObject49               53
#define planeObject50               54
#define planeObject51               55
#define planeObject52               56
#define planeObject53               57
#define planeObject54               58
#define planeObject55               59
#define planeObject56               60
#define planeObject57               61
#define planeObject58               62
#define planeObject59               63
#define planeObject60               64
#define planeObject61               65
#define planeObject62               66
#define planeObject63               67
#define planeObject64               68
#define planeObject65               69
#define planeObject66               70

//#define planeEffect1                71
//#define planeEffect2                72
//#define planeEffect3                73
//#define planeEffect4                74
//#define planeEffect5                75
//#define planeEffect6                76
//#define planeEffect7				77
//#define planeEffect8				78
//#define planeEffect9				79
//#define planeEffect10				80
//#define planeMagicEffect1           81
//#define planeMagicEffect2           82
//#define planeMagicEffect3           83
//#define planeMenu1                  84
//#define planeMenu2                  85
//#define planeMenu3                  86
//#define planeMenu4                  87
//#define planeMenu5                  88
//#define planeMenu6                  89
//#define planeMenu7					90
//#define planeCursor                 91

//#define planeObjectHIGH				33

#define planeEffect1                380
#define planeEffect2                381
#define planeEffect3                382
#define planeEffect4                383
#define planeEffect5                384
#define planeEffect6                385
#define planeEffect7				386
#define planeEffect8				387
#define planeEffect9				388
#define planeEffect10				389
#define planeMagicEffect1           390
#define planeMagicEffect2           391
#define planeMagicEffect3           392
#define planeMenu1                  393
#define planeMenu2                  394
#define planeMenu3                  395
#define planeMenu4                  396
#define planeMenu5                  397
#define planeMenu6                  398
#define planeMenu7					399
#define planeCursor                 400

//#define screenWidth					640
//#define screenHeight				480
// ..................... ENGINE2D ............................
#define engPOSITION		0x80000000
#define engGLASS		0x40000000
#define engMIX			0x20000000
#define engFLASH		0x10000000
#define engZOOM 		0x08000000
#define engADDCOLOR		0x04000000
#define engSUBCOLOR		0x02000000
#define engCHANGEXPOS   0x00400000		// 改變X差距，扭曲
#define engRANGE		0x01000000
#define engFONT 		0x00800000
#define engLIGHTSHADOW			0x00020000
#define engINVERSECOLOR			0x06000000		// AddColor + SubColor，負片
//
#define engASSIGNCOLOR			0x50000000		// 偏色
#define engASSIGNCOLOR_MIX		0x70000000		// 偏色, 有半透明效果
#define engASSIGNCOLOR_ALPHA	0x50008000		// 偏色, 不會有半透明效果
#define engASSIGNCOLOR_ALPHAMIX	0x70008000		// 偏色柔邊, 主要用在字型
#define engGLASSMIX				0x60000000		// objsData = [Level][Color]

#define engADDCOLOR_MIX         0x24000000
#define engADDCOLOR_ZOOM        0x0c000000
#define engADDCOLOR_ZOOM_MIX	0x2c000000
#define engSUBCOLOR_MIX         0x22000000
// ...................... OBJECT ...........................
#define objattrLOADPROCESSCALLBACK      0xfffffffd
#define objattrATTACKFLAG       		0x00010000
#define objattrPARENT           		0x40000000
#define objattrINVISIBLE        		0x10000000
#define objattrFLAG1					0x00004000
#define objattrFLAG2					0x00002000
//#define objattrFLAG3					0x00001000
#define objattrLOADRERUNSCRIPT			0x00001000		// 讀取時重建 script(背景音效)
#define objattrFLAG4					0x00000800
#define objattrFLAG5					0x00000400
//#define objattrFLAG6					0x00000200
#define objattrFLAG7            		0x00000100
#define objattrATTACKFLAG_FLAG7 		0x00010100
//
#define objattrEFFECT			 		0x00000400		// 由 option 決定是否顯示
#define objattrYPLANE			 		0x00000100		// 由 Y 決定平面
#define objattrEFFECT_YPLANE	 		0x00000500
// -------------- Mouse Cursor -----------------
#define curmode_NORMAL				10			// 一般, 點選
#define curmode_ATTACK				11			// 攻擊
#define curmode_VIEW				12			// 察看(問號)
#define curmode_TALK				13			// 交談
#define curmode_STOP				14			// 禁止行走
#define curmode_MAGIC				15			// 魔法
#define curmode_ENTER				16			// 進入
// ----------- Big Map -----------
#define bmpmPathPrevView	0x00000001			// 預視路徑
#define bmpmPath			0x00000002			// 行走路徑
#define bmpmNewAppear		0x00000004			// 下次會出現的點
#define bmpmNoEnter			0x00000008			// 無法進入
#define bmpmBattle			0x80000000			// 紅點：固定有戰鬥
#define bmpmGeneral			0x40000000			// 綠點：隨機有戰鬥
#define bmpmTown			0x20000000			// 藍點：城市
#define bmpmFirstVisit		0x10000000			// 第一次訪問
//
#define gameBMHidden		0
#define gameBMShowSlow		1
#define gameBMShow			2

// ------------ Process Code --------------
#define defProcReturn						0	// 不做事東西
#define defProcCursor						1	// 游標
#define defProcMapManager					2	// 場景地圖
#define defProcStageScript					3	// 場景描述控制物件
#define defProcBattleScript					4	// 戰場描述控制物件
#define defProcShapeTable					5	// 換圖物件(影子等)
#define defDebugWindowProcess				6	// 密技處理
#define defProcMenuEditBox					7
#define defProcMenuListItem					8
#define defProcMenuButton					9
#define defProcMenuRadioButton				10
#define defProcMenuCheckButton				11
#define defProcCallBack						12	// 回呼叫物件
//
#define defProcScrollBar					13
#define defProcScrollBlock					14
#define defProcScrollStep					15
//
#define defProcMainBoard					16	// 主面板
//
#define defProcStageBG						20	// 百人戰背景處理
#define defProcPlayer						21
#define defProcNPC							22	// NPC(objsData1 = obj id)(objsData2 = 交談ID)
#define defProcEnemy						23	// 敵人(objsData1 = obj id)
#define defProcFloor						24	// 躺在地板的物件
#define defProcBuild						25	// 建築物件(objsX1 ~ objsY2 = 牆壁不可走範圍)(objsData2 = 交談ID)
#define defProcWarpPoint					26	// 地圖切換跳躍點(evedata2 = new map code, evedata3 = x, evedata4 = y)
#define defProcShop							27
#define defProcCityWall						28	// 城牆
#define defProcCityStatue					29	// 城牆
//#define defProcGambleShop					30	// 賭場(objsData1 = ID)
