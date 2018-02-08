#include "string.h"
#include "sgSkillData.h"

CSgSkillData* CSgSkillData::instance = nullptr;
CSgSkillData* CSgSkillData::GetInstance()
{
	if (instance == nullptr)
	{
		instance = new CSgSkillData();
	}
	return instance;
}
CSgSkillData::CSgSkillData()
{

}
CSgSkillData::~CSgSkillData()
{

}

bool CSgSkillData::InitSgSkillData(char* file)
{
	if (file == nullptr)
	{
		file = DEFAULT_DATA_FILE;
	}

	//open data -- skill.dat ------- m_fd = 

	//read m_fd, and fill m_skillData[], set m_usersNum
}
bool CSgSkillData::SetSkillState(int userIndex, int skillId, char use, char learn)
{
	//update m_skillData[userIndex]
}
bool CSgSkillData::GetSkillState(int userIndex, int skillId, char &use, char &learn)
{
	//get m_skillData[userIndex]
}

bool CSgSkillData::SetAlUserSkillState(int skillId, char use, char learn)
{
	//update m_skillData[0-m_usersNum]
}
//删除所有用户的skillId技能
bool CSgSkillData::ClearAlUserSkillState(int skillId, char use, char learn)
{
	//update m_skillData[0-m_usersNum] skills

	//foreach m_skillData[0-m_usersNum] 
	{//item in 0-m_usersNum
		//update m_skillData[item]
	}
}

//删除所有技能
bool CSgSkillData::ClearAlUserSkill(int skillId, char use, char learn)
{
	//delete m_fd>>file or update m_skillData[0-m_usersNum] skills
}

bool CSgSkillData::SaveSgSkillData()
{

}

/*********************************************************************

*********************************************************************/

CSkillProfile* CSkillProfile::instance = nullptr;
CSkillProfile* CSkillProfile::GetInstance()
{
	if (instance == nullptr)
	{
		instance = new CSkillProfile();
	}
	return instance;
}

CSkillProfile::CSkillProfile()
{

}
CSkillProfile::~CSkillProfile()
{

}

bool  CSkillProfile::InitSkillProfile(char* file, char* file2)
{
	if (file == nullptr)
	{
		file = DEFAULT_MAGIC_FILE1;
	}
	if (file2 == nullptr)
	{
		file2 = DEFAULT_MAGIC_FILE2;
	}
	//open file file2, m_fd m_fd2

	//memset m_skills;

	//read file init m_skills, set m_counts
}
char* CSkillProfile::GetSkillNameById(int id)
{
	//foreach m_skills
	for (int i = 0; i < m_counts; i++)
	{
		if (m_skills[i].id == id)
		{
			return m_skills[i].name;
		}
	}
	return nullptr;
}
int CSkillProfile::GetSkillIdByName(char* name)
{
	//foreash 
	for (int i = 0; i < m_counts; i++)
	{
		if (!strncmp(m_skills[i].name, name, strlen(name)))
		{
			return m_skills[i].id;
		}
	}
	return -1;
}