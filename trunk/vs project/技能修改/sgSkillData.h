#ifndef SG_SKILLDATA_H
#define SG_SKILLDATA_H

typedef struct SG_SKILLDATA_ST
{
	char use[551];
	char learn[551];
}SGSkillData_T;
#define MAX_USERSKILL_NUM	2048
#define DEFAULT_DATA_FILE	"D:\\sgserver\\dbdata\\skill.dat"
class CSgSkillData
{
public:
	CSgSkillData();
	~CSgSkillData();

	bool InitSgSkillData(char* file=nullptr);
	bool SetSkillState(int userIndex, int skillId, char use, char learn);
	bool GetSkillState(int userIndex, int skillId, char &use, char &learn);

	bool SetAlUserSkillState(int skillId, char use, char learn);
	//删除所有用户都skillId技能
	bool ClearAlUserSkillState(int skillId, char use, char learn);

	//删除所有技能
	bool ClearAlUserSkill(int skillId, char use, char learn);

	bool SaveSgSkillData();
private:
	int				m_fd;
	int				m_usersNum;
	SGSkillData_T	m_skillData[MAX_USERSKILL_NUM];

public:
	static CSgSkillData* instance;
	static CSgSkillData* GetInstance();
};


typedef struct SG_SKILL_ST
{
	int id;
	char name[64];
}SGSkill_T;

#define DEFAULT_MAGIC_FILE1	"D:\\sgserver\\profile\\MAGIC.H"
#define DEFAULT_MAGIC_FILE2	"D:\\sgserver\\profile\\MAGIC_EXP.H"

class CSkillProfile
{
public:
	CSkillProfile();
	~CSkillProfile();

	bool  InitSkillProfile(char* file = nullptr, char* file2 = nullptr);
	char* GetSkillNameById(int id);
	int   GetSkillIdByName(char* name);
public:
	int m_fd;
	int m_fd2;

	static CSkillProfile* instance;
	static CSkillProfile* GetInstance();

private:
	int			m_counts;
	SGSkill_T	m_skills[1024];
};

#endif