using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace MainServer
{
    public struct Magic_JobLimit_Str
    {
        public int id;
        public string name;
        public int jobWARLORDLevel;
        public int jobLEADERLevel;
        public int jobADVISORLevel;
        public int jobWIZARDLevel;
    };
    public struct Skills_Def_Str
    {
        public string name;
        public int id;
    };
    public class CSkillCtrl
    {
        public static List<Skills_Def_Str> Skill_Defs = new List<Skills_Def_Str>();
        public static string load_Skill_Defines(string baseFolder)
        {
            string allSkillDefines = "";
            Skill_Defs.Clear();
            //baseFolder = "F:\\SanOL\\合并防挂成145版本";
            string itemHFile = baseFolder + "\\" + "MAGIC.H";
            if (!File.Exists(itemHFile))
            {
                return allSkillDefines;
            }

            FileStream _itemHfs = new FileStream(itemHFile, FileMode.Open, FileAccess.Read);
            StreamReader _itemHReader = new StreamReader(_itemHfs, Encoding.GetEncoding(950));
            _itemHReader.BaseStream.Seek(0, SeekOrigin.Begin);
            _itemHReader.DiscardBufferedData();
            _itemHReader.BaseStream.Seek(0, SeekOrigin.Begin);
            _itemHReader.BaseStream.Position = 0;

            Skills_Def_Str def;
            string strLine = "";
            strLine = _itemHReader.ReadLine();

            do
            {
                strLine = strLine.Split('/')[0];
                if (strLine.Contains("#define"))
                {
                    //ToSimplified
                    string _strLine = CFormat.ToSimplified(strLine);
                    string _nameNum = _strLine.Split(new string[] { "magic_" }, StringSplitOptions.RemoveEmptyEntries)[1];
                    string name = _nameNum.Split(' ')[0].Split('\t')[0];
                    string id = _nameNum.Replace(" ", "").Replace("\\t", "").Replace(name, "");

                    allSkillDefines += name + "," + id + ";";
                    def.name = name;
                    def.id = Int32.Parse(id);
                    Skill_Defs.Add(def);
                }

                strLine = "";
                strLine = _itemHReader.ReadLine();

            } while (strLine != null);

            _itemHReader.Close();
            _itemHfs.Close();

            //--------------------------------------------------------------------------------------------------
            itemHFile = baseFolder + "\\" + "MAGIC_EXP.H";
            if (!File.Exists(itemHFile))
            {
                return allSkillDefines;
            }

            _itemHfs = new FileStream(itemHFile, FileMode.Open, FileAccess.Read);
            _itemHReader = new StreamReader(_itemHfs, Encoding.GetEncoding(950));
            _itemHReader.BaseStream.Seek(0, SeekOrigin.Begin);
            _itemHReader.DiscardBufferedData();
            _itemHReader.BaseStream.Seek(0, SeekOrigin.Begin);
            _itemHReader.BaseStream.Position = 0;

            strLine = "";
            strLine = _itemHReader.ReadLine();

            do
            {
                strLine = strLine.Split('/')[0];
                if (strLine.Contains("#define"))
                {
                    //ToSimplified
                    string _strLine = CFormat.ToSimplified(strLine);
                    string _nameNum = _strLine.Split(new string[] { "magic_" }, StringSplitOptions.RemoveEmptyEntries)[1];
                    string name = _nameNum.Split(' ')[0].Split('\t')[0];
                    string id = _nameNum.Replace(" ", "").Replace("\\t", "").Replace(name, "");

                    allSkillDefines += name + "," + id + ";";
                    def.name = name;
                    def.id = Int32.Parse(id);
                    Skill_Defs.Add(def);
                }

                strLine = "";
                strLine = _itemHReader.ReadLine();

            } while (strLine != null);

            _itemHReader.Close();
            _itemHfs.Close();

            return allSkillDefines.Replace("\t", "");
        }

        static List<int> soul_skill_id = new List<int>{208, 86,187,459,102,207,189,94,89,191,206,91,196,103,201
                                   ,99,93,92,188,202,58,88,190,192,195,193,101,59,96,90,204
                                   ,194,87,97,205,535,499,465,463};
        public static void ClearAllSoulSkills(string baseFolder)
        {
            ClearAllPlayersSkills(soul_skill_id);

            /*
            //string LoginSkillFile = txt_svrForder.Text + "\\DataBase\\saves\\skill.dat";
            string LoginSkillFile = baseFolder + "\\DataBase\\saves\\skill.dat";

            //FileStream rdfs = new FileStream(LoginSkillFile, FileMode.Open, FileAccess.Read);
            //StreamReader rd = new StreamReader(rdfs, Encoding.ASCII);
            //rd.BaseStream.Seek(0, SeekOrigin.Begin);
            string time = DateTime.Now.Ticks.ToString();
            File.Copy(LoginSkillFile, LoginSkillFile + time);

            byte[] skills = File.ReadAllBytes(LoginSkillFile);
            int person_index = 0;
            int skill_index = 0;
            int skill_id = 0;
            for (int i = 33; i < skills.Length; i++)
            {
                byte b = skills[i];
                if (soul_skill_id.Contains(skill_id))
                {
                    if (skill_index <= 551)
                    {
                        skills[i] = 0x01;
                    }
                    else
                    {
                        skills[i] = 0x00;
                    }
                }
                skill_index++;
                skill_id++;
                if (skill_index > 551 * 2)
                {
                    skill_index = 0;
                    person_index++;
                }
                if (skill_id > 551)
                {
                    skill_id = 0;
                }
            }
            File.Delete(LoginSkillFile);
            File.WriteAllBytes(LoginSkillFile, skills);
             * */
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
        public struct Skill_Str 
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 551)]
            public byte[] use;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 551)]
            public byte[] learn;
        };
        private static int SkillData_HeadOffset = 32;
        private static byte[] skillHd = new byte[SkillData_HeadOffset];

        private static List<Skill_Str> skillData = new List<Skill_Str>();
        public static List<Skill_Str> LoadSkillsData(string file)
        {
            List<Skill_Str> skillList = new List<Skill_Str>();
            skillList.Clear();

            if (!File.Exists(file))
            {
                skillList.Clear();
                return skillList;
            }

            if (File.Exists(@".\\Temp\\skill.dat"))
            {
                File.SetAttributes(@".\\Temp\\skill.dat", FileAttributes.Normal);
                File.Copy(file, @".\\Temp\\skill.dat", true);
            }
            else
            {
                File.Copy(file, @".\\Temp\\skill.dat", false);
            }

            FileStream fs = new FileStream(".\\Temp\\skill.dat", FileMode.Open, FileAccess.ReadWrite);

            int ret = 0;
            ret = fs.Read(skillHd, 0, SkillData_HeadOffset);
            while (ret > 0)
            {
                Skill_Str aa = new Skill_Str();
                byte[] bytAccAttrData = new byte[Marshal.SizeOf(aa)];
                ret = fs.Read(bytAccAttrData, 0, Marshal.SizeOf(aa));
                if (ret <= 0)
                {
                    break;
                }
                Skill_Str acc_attr = CStructBytesFormat.BytesToStruct<Skill_Str>(bytAccAttrData);

                skillList.Add(acc_attr);
            }

            fs.Close();

            skillData.Clear();
            skillData = skillList;

            return skillList;
        }

        public static void SaveSkillsData(string file)
        {
            FileStream swList = new FileStream(@"temp\\skill.dat", FileMode.Create, FileAccess.Write, FileShare.None);
            //写头
            swList.Write(skillHd, 0, SkillData_HeadOffset);

            //写数据
            for (int i = 0; i < skillData.Count; i++)
            {
                swList.Write(skillData[i].use, 0, 551);
                swList.Write(skillData[i].learn, 0, 551);
            }

            swList.Close();

            //备份原文件
            string time = DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss");
            File.Copy(file, @"temp\\" + time + "skill.dat");

            //删除原文件
            File.Delete(file);

            //覆盖原文件
            File.Copy(@"temp\\skill.dat", file);
        }

        public static void ClearAllPlayersAllSkills()
        {
            for (int index = 0; index < skillData.Count; index++)
            {
                //技能
                for (int j = 0; j < 551; j++)
                {
                    skillData[index].use[j] = 0x1;
                    skillData[index].learn[j] = 0x0;
                }
            }
        }

        public static void AddAllPlayersAllSkills()
        {
            for (int index = 0; index < skillData.Count; index++ )
            {
                //技能
                for (int j = 0; j < 551; j++)
                {
                    skillData[index].use[j] = 0x1;
                    skillData[index].learn[j] = 0x1;
                }
            }
        }

        public static void ClearPlayerAllSkills(List<int> players)
        {
            foreach (var index in players)
            {
                //技能
                for (int j = 0; j < 551; j++)
                {
                    skillData[index].use[j] = 0x1;
                    skillData[index].learn[j] = 0x0;
                }
            }
        }

        public static void AddPlayerAllSkills(List<int> players)
        {
            foreach (var index in players)
            {
                //技能
                for (int j = 0; j < 551; j++)
                {
                    skillData[index].use[j] = 0x1;
                    skillData[index].learn[j] = 0x1;
                }
            }
        }

        public static void ClearAllPlayersSkills(List<int> skills)
        {
            for (int i = 0; i < skillData.Count; i++)
            {
                //技能
                for (int j = 0; j < 551; j++)
                {
                    if (skills.Contains(j))
                    {
                        skillData[i].use[j] = 0x1;
                        skillData[i].learn[j] = 0x0;
                    }
                }
            }
        }

        public static void AddAllPlayersSkills(List<int> skills)
        {
            for (int i = 0; i < skillData.Count; i++)
            {
                //技能
                for (int j = 0; j < 551; j++)
                {
                    if (skills.Contains(j))
                    {
                        skillData[i].use[j] = 0x1;
                        skillData[i].learn[j] = 0x0;
                    }
                }
            }
        }

        /*
            players   list of players index 
            skills    list of skills id 
         */
        public static void ClearSubSkills(List<int> players, List<int> skills)
        {
            for (int i = 0; i < skillData.Count; i++ )
            {
                if (players.Contains(i + 1))
                {
                    //技能
                    for(int j =0; j<551; j++)
                    {
                        if (skills.Contains(j))
                        {
                            skillData[i].use[j] = 0x1;
                            skillData[i].learn[j] = 0x0;
                        }
                    }
                }
            }
        }

        public static void AddSubSkills(List<int> players, List<int> skills)
        {
            for (int i = 0; i < skillData.Count; i++)
            {
                if (players.Contains(i + 1))
                {
                    //技能
                    for (int j = 0; j < 551; j++)
                    {
                        if (skills.Contains(j))
                        {
                            skillData[i].use[j] = 0x1;
                            skillData[i].learn[j] = 0x1;
                        }
                    }
                }
            }
        }

        public static void AddSubSkills(List<AccAttr> players, List<Magic_JobLimit_Str> skills, bool limitLevel)
        {
            foreach (var player in players)
            {
                int index = (int)player.nIndex;
                int job = player.job;
                
                foreach(var skill in skills)
                {
                    bool learn = true;
                    if (limitLevel)
                    {
                        if (job == 0x1 && (skill.jobWARLORDLevel > player.Level || skill.jobWARLORDLevel == 0)) //猛将
                        {
                            learn = false;
                        }
                        else if (job == 0x2 && (skill.jobLEADERLevel > player.Level || skill.jobLEADERLevel == 0)) //豪杰
                        {
                            learn = false;
                        }
                        else if (job == 0x3 && (skill.jobADVISORLevel > player.Level || skill.jobADVISORLevel == 0)) //军师
                        {
                            learn = false;
                        }
                        else if (job == 0x4 && (skill.jobWIZARDLevel > player.Level || skill.jobWIZARDLevel == 0)) //方士
                        {
                            learn = false;
                        }
                    }

                    if (skill.id < 551)
                    {
                        if (learn)
                        {
                            skillData[index - 1].use[skill.id] = 0x1;
                            skillData[index - 1].learn[skill.id] = 0x1;
                        }
                        else
                        {
                            skillData[index - 1].use[skill.id] = 0x1;
                            skillData[index - 1].learn[skill.id] = 0x0;
                        }
                    }
                }
            }
            
        }

        public static int GetIdByName(string name)
        {
            int id = 0;

            foreach (var item in Skill_Defs)
            {
                if (item.name == CFormat.ToSimplified(name))
                {
                    id = item.id;
                    break;
                }
            }

            return id;
        }

        public static List<int> LoadAllWarloadSkills()
        {
            if (JobLimitSkills.Count == 0)
            {
                LoadAllJobLimits();
            }
            List<int> skills = new List<int>();

            foreach (var item in JobLimitSkills)
            {
                if (item.jobWARLORDLevel > 0 )
                {
                    int id = GetIdByName(item.name);
                    skills.Add(id);
                }
            }

            return skills;
        }

        public static List<int> LoadAllLeaderSkills()
        {
            if (JobLimitSkills.Count == 0)
            {
                LoadAllJobLimits();
            }
            List<int> skills = new List<int>();

            foreach (var item in JobLimitSkills)
            {
                if (item.jobLEADERLevel > 0)
                {
                    int id = GetIdByName(item.name);
                    skills.Add(id);
                }
            }

            return skills;
        }

        public static List<int> LoadAllAdvisorSkills()
        {
            if (JobLimitSkills.Count == 0)
            {
                LoadAllJobLimits();
            }
            List<int> skills = new List<int>();
            foreach (var item in JobLimitSkills)
            {
                if (item.jobADVISORLevel > 0)
                {
                    int id = GetIdByName(item.name);
                    skills.Add(id);
                }
            }
            return skills;
        }

        public static List<int> LoadAllWizardSkills()
        {
            if (JobLimitSkills.Count == 0)
            {
                LoadAllJobLimits();
            }

            List<int> skills = new List<int>();
            foreach (var item in JobLimitSkills)
            {
                if (item.jobWIZARDLevel > 0)
                {
                    int id = GetIdByName(item.name);
                    skills.Add(id);
                }
            }
            return skills;
        }

        public static List<Magic_JobLimit_Str> LoadAllWarloadSkillsDesc()
        {
            if (JobLimitSkills.Count == 0)
            {
                LoadAllJobLimits();
            }
            List<Magic_JobLimit_Str> skills = new List<Magic_JobLimit_Str>();

            foreach (var item in JobLimitSkills)
            {
                if (item.jobWARLORDLevel > 0)
                {
                    skills.Add(item);
                }
            }

            return skills;
        }

        public static List<Magic_JobLimit_Str> LoadAllLeaderSkillsDesc()
        {
            if (JobLimitSkills.Count == 0)
            {
                LoadAllJobLimits();
            }
            List<Magic_JobLimit_Str> skills = new List<Magic_JobLimit_Str>();

            foreach (var item in JobLimitSkills)
            {
                if (item.jobLEADERLevel > 0)
                {
                    skills.Add(item);
                }
            }

            return skills;
        }

        public static List<Magic_JobLimit_Str> LoadAllAdvisorSkillsDesc()
        {
            if (JobLimitSkills.Count == 0)
            {
                LoadAllJobLimits();
            }
            List<Magic_JobLimit_Str> skills = new List<Magic_JobLimit_Str>();
            foreach (var item in JobLimitSkills)
            {
                if (item.jobADVISORLevel > 0)
                {
                    skills.Add(item);
                }
            }
            return skills;
        }

        public static List<Magic_JobLimit_Str> LoadAllWizardSkillsDesc()
        {
            if (JobLimitSkills.Count == 0)
            {
                LoadAllJobLimits();
            }

            List<Magic_JobLimit_Str> skills = new List<Magic_JobLimit_Str>();
            foreach (var item in JobLimitSkills)
            {
                if (item.jobWIZARDLevel > 0)
                {
                    skills.Add(item);
                }
            }
            return skills;
        }

        private static List<Magic_JobLimit_Str> JobLimitSkills = new List<Magic_JobLimit_Str>();
        public static List<Magic_JobLimit_Str> LoadAllJobLimits()
        {
            JobLimitSkills.Clear();
            //List<Magic_JobLimit_Str> skills = new List<Magic_JobLimit_Str>();
            Magic_JobLimit_Str skill;
            skill.name = "";
            skill.id = 0;
            skill.jobWARLORDLevel = 0;
            skill.jobLEADERLevel = 0;
            skill.jobADVISORLevel = 0;
            skill.jobWIZARDLevel = 0;

            FileStream rdfs = new FileStream("profile\\MAGIC.TXT", FileMode.Open, FileAccess.Read);
            StreamReader rd = new StreamReader(rdfs, Encoding.GetEncoding(950));
            rd.DiscardBufferedData();
            rd.BaseStream.Seek(0, SeekOrigin.Begin);
            rd.BaseStream.Position = 0;

            string strLine = null;
            while ((strLine = rd.ReadLine()) != null)
            {
                strLine = strLine.Split(new string[] { "//" }, StringSplitOptions.None)[0];
                //code
                if (strLine.Contains("code") && strLine.Contains("magic_"))
                {
                    if (skill.name != "")
                    {
                        JobLimitSkills.Add(skill);
                    }
                    skill.name = strLine.Split(new string[] { "magic_" }, StringSplitOptions.RemoveEmptyEntries)[1].Replace(" ","").Replace("\t","");
                    skill.id = GetIdByName(skill.name);
                    skill.jobWARLORDLevel = 0;
                    skill.jobLEADERLevel = 0;
                    skill.jobADVISORLevel = 0;
                    skill.jobWIZARDLevel = 0;
                }
                //learn_level
                if (strLine.Contains("learn_level") && strLine.Contains("jobWARLORD"))
                {
                    int level = Int32.Parse(strLine.Split(',')[1].Replace(" ","").Replace("\t",""));
                    skill.jobWARLORDLevel = level;
                }
                if (strLine.Contains("learn_level") && strLine.Contains("jobLEADER"))
                {
                    int level = Int32.Parse(strLine.Split(',')[1].Replace(" ", "").Replace("\t", ""));
                    skill.jobLEADERLevel = level;
                }
                if (strLine.Contains("learn_level") && strLine.Contains("jobADVISOR"))
                {
                    int level = Int32.Parse(strLine.Split(',')[1].Replace(" ", "").Replace("\t", ""));
                    skill.jobADVISORLevel = level;
                }
                if (strLine.Contains("learn_level") && strLine.Contains("jobWIZARD"))
                {
                    int level = Int32.Parse(strLine.Split(',')[1].Replace(" ", "").Replace("\t", ""));
                    skill.jobWIZARDLevel = level;
                }
            }

            rd.Close();
            rdfs.Close();

            if (skill.name != "")
            {
                JobLimitSkills.Add(skill);
            }
            skill.name = "";
            skill.id = 0;
            skill.jobWARLORDLevel = 0;
            skill.jobLEADERLevel = 0;
            skill.jobADVISORLevel = 0;
            skill.jobWIZARDLevel = 0;


            //------------------------------------------------------------------------------------
            rdfs = new FileStream("profile\\MAGIC_EXP.TXT", FileMode.Open, FileAccess.Read);
            rd = new StreamReader(rdfs, Encoding.GetEncoding(950));
            rd.DiscardBufferedData();
            rd.BaseStream.Seek(0, SeekOrigin.Begin);
            rd.BaseStream.Position = 0;

            strLine = null;
            while ((strLine = rd.ReadLine()) != null)
            {
                strLine = strLine.Split(new string[] { "//" }, StringSplitOptions.None)[0];
                //code
                if (strLine.Contains("code") && strLine.Contains("magic_"))
                {
                    if (skill.name != "")
                    {
                        JobLimitSkills.Add(skill);
                    }
                    skill.name = strLine.Split(new string[] { "magic_" }, StringSplitOptions.RemoveEmptyEntries)[1].Replace(" ", "").Replace("\t", "");
                    skill.id = GetIdByName(skill.name);
                    skill.jobWARLORDLevel = 0;
                    skill.jobLEADERLevel = 0;
                    skill.jobADVISORLevel = 0;
                    skill.jobWIZARDLevel = 0;
                }
                //learn_level
                if (strLine.Contains("learn_level") && strLine.Contains("jobWARLORD"))
                {
                    int level = Int32.Parse(strLine.Split(',')[1].Replace(" ", "").Replace("\t", ""));
                    skill.jobWARLORDLevel = level;
                }
                if (strLine.Contains("learn_level") && strLine.Contains("jobLEADER"))
                {
                    int level = Int32.Parse(strLine.Split(',')[1].Replace(" ", "").Replace("\t", ""));
                    skill.jobLEADERLevel = level;
                }
                if (strLine.Contains("learn_level") && strLine.Contains("jobADVISOR"))
                {
                    int level = Int32.Parse(strLine.Split(',')[1].Replace(" ", "").Replace("\t", ""));
                    skill.jobADVISORLevel = level;
                }
                if (strLine.Contains("learn_level") && strLine.Contains("jobWIZARD"))
                {
                    int level = Int32.Parse(strLine.Split(',')[1].Replace(" ", "").Replace("\t", ""));
                    skill.jobWIZARDLevel = level;
                }
            } 

            rd.Close();
            rdfs.Close();

            if (skill.name != "")
            {
                JobLimitSkills.Add(skill);
            }
            skill.name = "";
            skill.id = 0;
            skill.jobWARLORDLevel = 0;
            skill.jobLEADERLevel = 0;
            skill.jobADVISORLevel = 0;
            skill.jobWIZARDLevel = 0;

            return JobLimitSkills;
        }
    }
}
