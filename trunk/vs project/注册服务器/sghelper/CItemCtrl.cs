using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace MainServer
{
    public class CItemCtrl
    {
        public struct item_attr
        {
            public string code;
            public string name;
            public string type;
            public string wait_type;
            public string light;
            public string attack_range;
            public string effect_range;
            public string add_attack_power;
            public string attack_speed;
            public string add_weapon_hit;
            public string eq_magic_attack_type;
            public string limit_level;
            public string weight;
            public string cost;
            public string sell;
            public string cost_level;
            public string icon;
            public string help_string;
            public string super_type;
            public string sfx_hit;
            public string sfx_attack;
            public string comp_times;
            public string add_attr_mind;
            public string add_magic_power;
            public string add_attr_con;
            public string add_attackx2_ratio;
            public string limit_job;
            public string add_attr_int;
            public string add_attr_str;
            public string wid;
            public string add_attr_dex;
            public string add_hp;
            public string sp_effect;
            public string add_mp;
            public string soul_times;
            public string add_attr_leader;
            public string type2;
            public string function;
            public string limit_str;
            public string limit_mind;
            public string limit_dex;
            public string limit_int;
            public string limit_leader;
            public string new_exp;
            public string new_explevel;
            public string kill;
            public string drop_id;
            public string use_magic_id;
            public string drop_num;
            public string add_defense;
            public string eq_magic_resist;
            public string limit_sex;
            public string add_weapon_misshit;
            public string anti_status_ratio;
            public string engineer_type;
            public string duedate_only;
            public string use_duedate;
            public string move;
            public string sfx_run;
            public string sfx_walk;
            public string horse_height;
            public string wawa_only;
            public string no_attack;
            public string other_wid;
            public string use_magic_time;
            public string use_magic_level;
            public string tp_map_code;
            public string tp_pos;
            public string add_st;
            public string add_loyalty;
            public string letto_gold;
            public string resurrect_soldier;
            public string recommand_soldier;
        };

        public struct item_def
        {
            public string name;
            public string id;
        }
        CItemCtrl(string _baseFolder) { baseFolder = _baseFolder; }

        public static string m_lastId = "";
        public static string baseFolder;
        public static string Item_H = "item.h";
        public static string Item_data = "item.txt";
        public static string Item_name = "item_name.txt";
        public static string Item_helper = "ITEM_HELP_STRING.TXT";

        private static List<item_def> m_lstDef = new List<item_def>();
        public static bool isNumberic(string message, out int result)
        {
            //判断是否为整数字符串
            //是的话则将其转换为数字并将其设为out类型的输出值、返回true, 否则为false
            result = -1;   //result 定义为out 用来输出值
            try
            {
                result = Convert.ToInt32(message);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public struct ff
        {
            public string name;
            public List<string> list;
        };
        public static void AnalyzerItemAttr(string baseFolder)
        {
            baseFolder = "F:\\SanOL\\合并防挂成145版本";
            string itemFile = baseFolder + "\\" + Item_data;
            if (!File.Exists(itemFile))
            {
                return;
            }
           

            FileStream rdfs = new FileStream(itemFile, FileMode.Open, FileAccess.Read);
            StreamReader rd = new StreamReader(rdfs, Encoding.ASCII);
            rd.BaseStream.Seek(0, SeekOrigin.Begin);

            rd.DiscardBufferedData();
            rd.BaseStream.Seek(0, SeekOrigin.Begin);
            rd.BaseStream.Position = 0;

            List<string> itemAttr = new List<string>();
            string allLine = "";
            List<string> itemAttr2 = new List<string>();
            string itemAttr2Line = "";

            List<string> itemAttr3 = new List<string> { "type", "wait_type", "eq_magic_attack_type", "super_type", "sfx_hit", "sfx_attack", "limit_job", "sp_effect", "type2"
                , "function", "kill", "eq_magic_resist","limit_sex","anti_status_ratio","sfx_run","sfx_walk","use_magic_id","tp_map_code" };

            int l = itemAttr3.Count();
            string[] itemType3 = new string[l];
            for (int x = 0; x < l; x++ )
            {
                itemType3[x] = "";
            }

            List<ff> attrMore = new List<ff>();
            List<string> attrMoreLine = new List<string>();
            foreach (var i in itemAttr3)
            {
                ff f_t;
                f_t.name = i;
                f_t.list = new List<string>();
                attrMore.Add(f_t);

                attrMoreLine.Add(i+"Line");
            }

            List<string> itemType = new List<string>();
            List<string> itemType2 = new List<string>();

            string strLine = "";
            strLine = "";
            strLine = rd.ReadLine();

            do
            {
                if (strLine.Split('=').Length >= 2)
                {
                    string tmp = strLine.Split('=')[0].Replace(" ", "").Replace("//", "");
                    if (!itemAttr.Contains(tmp))
                    {
                        itemAttr.Add(tmp);
                    }
                    if (!itemAttr2.Contains(tmp))
                    {
                        var itemAttr2_list = strLine.Split('=')[1].Replace(" ", "").Replace("//", "").Replace("\t", "").Split(',');
                        if (itemAttr2_list.Length > 0)
                        {
                            foreach (var _item in itemAttr2_list)
                            {
                                int ret = 0;
                                if (!isNumberic(_item, out ret))
                                {
                                    itemAttr2.Add(tmp);
                                    break;
                                }
                            }
                        }
                    }
                    string tmp2 = strLine.Split('=')[1].Replace(" ", "").Replace("//", "");
                    if (tmp == "type")
                    {
                        var type_list = tmp2.Split(',');
                        foreach(var _type in type_list)
                        {
                            if (!itemType.Contains(_type))
                            {
                                itemType.Add(_type);
                            }
                        }
                    }
                    else if (tmp == "type2")
                    {
                        var type_list = tmp2.Split(',');
                        foreach (var _type in type_list)
                        {
                            if (!itemType2.Contains(_type))
                            {
                                itemType2.Add(_type);
                            }
                        }
                    }
                    foreach (var aa in itemAttr3)
                    { 
                        if(tmp == aa)
                        {
                            int index = itemAttr3.IndexOf(aa);
                            var arrrr = tmp2.Split(',');
                            foreach(var u7 in arrrr)
                            {
                                int y;
                                if (!itemType3[index].Contains(u7) && !isNumberic(u7,out y))
                                {
                                    itemType3[index] += u7 + "\r\n";
                                }
                            }
                            
                        }
                    }
                }

                strLine = "";
                strLine = rd.ReadLine();
            } while (strLine != null);
            rd.Close();
            rdfs.Close();

            allLine = string.Join("\r\n", itemAttr.ToArray());
            File.WriteAllText(baseFolder + "\\" + "itemAttr.txt", allLine);

            itemAttr2Line = string.Join("\r\n", itemAttr2.ToArray());
            File.WriteAllText(baseFolder + "\\" + "itemAttr2.txt", itemAttr2Line);

            //typeLine = string.Join("\r\n", itemType.ToArray());
            //File.WriteAllText(baseFolder + "\\" + "itemType.txt", typeLine);
            //type2Line = string.Join("\r\n", itemType2.ToArray());
            //File.WriteAllText(baseFolder + "\\" + "itemType2.txt", type2Line);

            foreach (var u8 in itemAttr3)
            {
                string file = "itemType_" + u8 + ".TXT";
                int _index = itemAttr3.IndexOf(u8);
                File.WriteAllText(baseFolder + "\\" + file, itemType3[_index]);
            }
        }
        public static List<item_attr> load_Item_Data(string baseFolder)
        {
            List<item_attr> item_attr = new List<item_attr>();

            baseFolder = "F:\\SanOL\\合并防挂成145版本";
            string itemFile = baseFolder + "\\" + Item_data;
            if (!File.Exists(itemFile))
            {
                return item_attr;
            }

            
            
            return item_attr;
        }

        public static string load_Item_Defines(string baseFolder)
        {
            m_lstDef.Clear();
            string allItemDefines = "";
            //baseFolder = "F:\\SanOL\\合并防挂成145版本";
            string itemHFile = baseFolder + "\\" + Item_H;
            if (!File.Exists(itemHFile))
            {
                return allItemDefines;
            }

            FileStream _itemHfs = new FileStream(itemHFile, FileMode.Open, FileAccess.Read);
            StreamReader _itemHReader = new StreamReader(_itemHfs, Encoding.GetEncoding(950));
            _itemHReader.BaseStream.Seek(0, SeekOrigin.Begin);
            _itemHReader.DiscardBufferedData();
            _itemHReader.BaseStream.Seek(0, SeekOrigin.Begin);
            _itemHReader.BaseStream.Position = 0;

            string strLine = "";
            strLine = _itemHReader.ReadLine();

            do
            {
                strLine = strLine.Split('/')[0];
                if (strLine.Contains("#define"))
                {
                    //ToSimplified
                    item_def item = new item_def();
                    string _strLine = CFormat.ToSimplified(strLine);
                    string _nameNum = _strLine.Split(new string[] { "item_" }, StringSplitOptions.RemoveEmptyEntries)[1];
                    string name = _nameNum.Split(' ')[0].Split('\t')[0];
                    string id = _nameNum.Replace(" ", "").Replace("\t", "").Replace(name, "");
                    item.name = name;
                    item.id = id;
                    m_lstDef.Add(item);
                    
                    allItemDefines += name + "," + id + ";";
                    m_lastId = id;
                }
                    
                strLine = "";
                strLine = _itemHReader.ReadLine();

            } while (strLine != null);

            _itemHReader.Close();
            _itemHfs.Close();

            return allItemDefines.Replace("\t","");
        }

        static public string GetNameById(string id)
        {
            string name = string.Empty;
            foreach(var item in m_lstDef)
            {
                if (item.id == id)
                {
                    name = item.name;
                    break;
                }
            }
            return name;
        }

        static public item_attr Get_Attr_ByName(string name, out bool done)
        {
            item_attr attr = new item_attr();
            attr.code = "item_" + name;
            baseFolder = "F:\\SanOL\\合并防挂成145版本";
            string itemDataFile = baseFolder + "\\" + Item_data;
            if (!File.Exists(itemDataFile))
            {
                done = false;
                return attr;
            }

            FileStream _itemDatafs = new FileStream(itemDataFile, FileMode.Open, FileAccess.Read);
            StreamReader _itemDataReader = new StreamReader(_itemDatafs, Encoding.GetEncoding(950));
            _itemDataReader.BaseStream.Seek(0, SeekOrigin.Begin);
            _itemDataReader.DiscardBufferedData();
            _itemDataReader.BaseStream.Seek(0, SeekOrigin.Begin);
            _itemDataReader.BaseStream.Position = 0;

            string strLine = "";
            strLine = _itemDataReader.ReadLine();
            bool get_it = false;
            do
            {
                string _strLine = CFormat.ToSimplified(strLine);
                _strLine = _strLine.Split('/')[0].Replace("\t", "").Replace(" ","");
                if (get_it && _strLine == "[item]")
                {
                    break;
                }
                if(_strLine.Contains("item_" + name))
                {
                    get_it = true;
                }
                if (get_it)
                {
                    string _type = _strLine.Split('=')[0];
                    if (_type == "code"){
                        attr.code = "item_" + name;
                    }
                    else if (_type == "name"){
                        attr.name = _strLine.Split('=')[1];
                    }
                    else if (_type == "help_string")
                    {
                        attr.help_string = _strLine.Split('=')[1];
                    }
                    else if (_type == "type")
                    {
                        attr.type = _strLine.Split('=')[1];
                    }
                    else if (_type == "super_type")
                    {
                        attr.super_type = _strLine.Split('=')[1];
                    }
                    else if (_type == "type2")
                    {
                        attr.type2 = _strLine.Split('=')[1];
                    }
                    else if (_type == "wait_type")
                    {
                        attr.wait_type = _strLine.Split('=')[1];
                    }
                    else if (_type == "attack_range")
                    {
                        attr.attack_range = _strLine.Split('=')[1];
                    }
                    else if (_type == "effect_range")
                    {
                        attr.effect_range = _strLine.Split('=')[1];
                    }
                    else if (_type == "add_attack_power")
                    {
                        attr.add_attack_power = _strLine.Split('=')[1];
                    }
                    else if (_type == "attack_speed")
                    {
                        attr.attack_speed = _strLine.Split('=')[1];
                    }
                    else if (_type == "add_weapon_hit")
                    {
                        attr.add_weapon_hit = _strLine.Split('=')[1];
                    }
                    else if (_type == "add_attr_mind")
                    {
                        attr.add_attr_mind = _strLine.Split('=')[1];
                    }
                    else if (_type == "add_magic_power")
                    {
                        attr.add_magic_power = _strLine.Split('=')[1];
                    }
                    else if (_type == "add_attr_con")
                    {
                        attr.add_attr_con = _strLine.Split('=')[1];
                    }
                    else if (_type == "add_attackx2_ratio")
                    {
                        attr.add_attackx2_ratio = _strLine.Split('=')[1];
                    }
                    else if (_type == "add_attr_int")
                    {
                        attr.add_attr_int = _strLine.Split('=')[1];
                    }
                    else if (_type == "add_attr_str")
                    {
                        attr.add_attr_str = _strLine.Split('=')[1];
                    }
                    else if (_type == "add_attr_dex")
                    {
                        attr.add_attr_dex = _strLine.Split('=')[1];
                    }
                    else if (_type == "add_hp")
                    {
                        attr.add_hp = _strLine.Split('=')[1];
                    }
                    else if (_type == "add_mp")
                    {
                        attr.add_mp = _strLine.Split('=')[1];
                    }
                    else if (_type == "add_attr_leader")
                    {
                        attr.add_attr_leader = _strLine.Split('=')[1];
                    }
                    else if (_type == "add_defense")
                    {
                        attr.add_defense = _strLine.Split('=')[1];
                    }
                    else if (_type == "add_weapon_misshit")
                    {
                        attr.add_weapon_misshit = _strLine.Split('=')[1];
                    }
                    else if (_type == "move")
                    {
                        attr.move = _strLine.Split('=')[1];
                    }
                    else if (_type == "add_st")
                    {
                        attr.add_st = _strLine.Split('=')[1];
                    }
                    else if (_type == "add_loyalty")
                    {
                        attr.add_loyalty = _strLine.Split('=')[1];
                    }
                    else if (_type == "light")
                    {
                        attr.light = _strLine.Split('=')[1];
                    }
                    else if (_type == "eq_magic_attack_type")
                    {
                        if (attr.eq_magic_attack_type != null && attr.eq_magic_attack_type != "")
                        {
                            attr.eq_magic_attack_type += ";"+_strLine.Split('=')[1];
                        }else{
                            attr.eq_magic_attack_type = _strLine.Split('=')[1];
                        }
                    }
                    else if (_type == "limit_level")
                    {
                        attr.limit_level = _strLine.Split('=')[1];
                    }
                    else if (_type == "weight")
                    {
                        attr.weight = _strLine.Split('=')[1];
                    }
                    else if (_type == "cost")
                    {
                        attr.cost = _strLine.Split('=')[1];
                    }
                    else if (_type == "sell")
                    {
                        attr.sell = _strLine.Split('=')[1];
                    }
                    else if (_type == "cost_level")
                    {
                        attr.cost_level = _strLine.Split('=')[1];
                    }
                    else if (_type == "icon")
                    {
                        attr.icon = _strLine.Split('=')[1];
                    }
                    else if (_type == "sfx_hit")
                    {
                        attr.sfx_hit = _strLine.Split('=')[1];
                    }
                    else if (_type == "sfx_attack")
                    {
                        attr.sfx_attack = _strLine.Split('=')[1];
                    }
                    else if (_type == "comp_times")
                    {
                        attr.comp_times = _strLine.Split('=')[1];
                    }
                    else if (_type == "limit_job")
                    {
                        attr.limit_job = _strLine.Split('=')[1];
                    }
                    else if (_type == "wid")
                    {
                        attr.wid = _strLine.Split('=')[1];
                    }
                    else if (_type == "sp_effect")
                    {
                        attr.sp_effect = _strLine.Split('=')[1];
                    }
                    else if (_type == "soul_times")
                    {
                        attr.soul_times = _strLine.Split('=')[1];
                    }
                    else if (_type == "function")
                    {
                        if (attr.function != null && attr.function != "")
                        {
                            attr.function += ";" + _strLine.Split('=')[1];
                        }
                        else
                        {
                            attr.function = _strLine.Split('=')[1];
                        }
                    }
                    else if (_type == "anti_status_ratio")
                    {
                        if (attr.anti_status_ratio != null && attr.anti_status_ratio != "")
                        {
                            attr.anti_status_ratio += ";" + _strLine.Split('=')[1];
                        }
                        else
                        {
                            attr.anti_status_ratio = _strLine.Split('=')[1];
                        }
                    }
                    else if (_type == "limit_str")
                    {
                        attr.limit_str = _strLine.Split('=')[1];
                    }
                    else if (_type == "limit_mind")
                    {
                        attr.limit_mind = _strLine.Split('=')[1];
                    }
                    else if (_type == "limit_dex")
                    {
                        attr.limit_dex = _strLine.Split('=')[1];
                    }
                    else if (_type == "limit_int")
                    {
                        attr.limit_int = _strLine.Split('=')[1];
                    }
                    else if (_type == "limit_leader")
                    {
                        attr.limit_leader = _strLine.Split('=')[1];
                    }
                    else if (_type == "new_exp")
                    {
                        attr.new_exp = _strLine.Split('=')[1];
                    }
                    else if (_type == "new_explevel")
                    {
                        attr.new_explevel = _strLine.Split('=')[1];
                    }
                    else if (_type == "kill")
                    {
                        attr.kill = _strLine.Split('=')[1];
                    }
                    else if (_type == "drop_id")
                    {
                        attr.drop_id = _strLine.Split('=')[1];
                    }
                    else if (_type == "use_magic_id")
                    {
                        attr.use_magic_id = _strLine.Split('=')[1];
                    }
                    else if (_type == "drop_num")
                    {
                        attr.drop_num = _strLine.Split('=')[1];
                    }
                    else if (_type == "eq_magic_resist")
                    {
                        if (attr.eq_magic_resist != null && attr.eq_magic_resist != "")
                        {
                            attr.eq_magic_resist += ";" + _strLine.Split('=')[1];
                        }
                        else
                        {
                            attr.eq_magic_resist = _strLine.Split('=')[1];
                        }                        
                    }
                    else if (_type == "limit_sex")
                    {
                        attr.limit_sex = _strLine.Split('=')[1];
                    }
                    else if (_type == "engineer_type")
                    {
                        attr.engineer_type = _strLine.Split('=')[1];
                    }
                    else if (_type == "duedate_only")
                    {
                        attr.duedate_only = _strLine.Split('=')[1];
                    }
                    else if (_type == "use_duedate")
                    {
                        attr.use_duedate = _strLine.Split('=')[1];
                    }
                    else if (_type == "sfx_run")
                    {
                        attr.sfx_run = _strLine.Split('=')[1];
                    }
                    else if (_type == "sfx_walk")
                    {
                        attr.sfx_walk = _strLine.Split('=')[1];
                    }
                    else if (_type == "horse_height")
                    {
                        attr.horse_height = _strLine.Split('=')[1];
                    }
                    else if (_type == "wawa_only")
                    {
                        attr.wawa_only = _strLine.Split('=')[1];
                    }
                    else if (_type == "no_attack")
                    {
                        attr.no_attack = _strLine.Split('=')[1];
                    }
                    else if (_type == "other_wid")
                    {
                        attr.other_wid = _strLine.Split('=')[1];
                    }
                    else if (_type == "use_magic_time")
                    {
                        attr.use_magic_time = _strLine.Split('=')[1];
                    }
                    else if (_type == "use_magic_level")
                    {
                        attr.use_magic_level = _strLine.Split('=')[1];
                    }
                    else if (_type == "tp_map_code")
                    {
                        attr.tp_map_code = _strLine.Split('=')[1];
                    }
                    else if (_type == "tp_pos")
                    {
                        attr.tp_pos = _strLine.Split('=')[1];
                    }                    
                    else if (_type == "letto_gold")
                    {
                        attr.letto_gold = _strLine.Split('=')[1];
                    }
                    else if (_type == "resurrect_soldier")
                    {
                        attr.resurrect_soldier = _strLine.Split('=')[1];
                    }
                    else if (_type == "recommand_soldier")
                    {
                        attr.recommand_soldier = _strLine.Split('=')[1];
                    }
                }
                strLine = "";
                strLine = _itemDataReader.ReadLine();

            } while (strLine != null);

            _itemDataReader.Close();
            _itemDatafs.Close();

            done = true;
            return attr;
        }

        public static string Get_Item_NameStr(string name)
        {
            string nameString = "";

            baseFolder = "F:\\SanOL\\合并防挂成145版本";
            string itemNameFile = baseFolder + "\\" + Item_name;
            if (!File.Exists(itemNameFile))
            {
                return nameString;
            }

            FileStream _itemNamefs = new FileStream(itemNameFile, FileMode.Open, FileAccess.Read);
            StreamReader _itemNameReader = new StreamReader(_itemNamefs, Encoding.GetEncoding(950));
            _itemNameReader.BaseStream.Seek(0, SeekOrigin.Begin);
            _itemNameReader.DiscardBufferedData();
            _itemNameReader.BaseStream.Seek(0, SeekOrigin.Begin);
            _itemNameReader.BaseStream.Position = 0;

            string strLine = "";
            strLine = _itemNameReader.ReadLine();

            do
            {
                string _strLine = CFormat.ToSimplified(strLine);
                if (_strLine.Contains("item") && _strLine.Contains("="))
                {
                    _strLine = _strLine.Replace(" ", "").Replace("\t", "");
                    string id = _strLine.Split('=')[1].Split(',')[0];
                    if (id == name)
                    {
                        nameString = _strLine.Split('=')[1].Split(',')[1];
                        break;
                    }
                }
                strLine = "";
                strLine = _itemNameReader.ReadLine();
            } while (strLine != null);

            _itemNameReader.Close();
            _itemNamefs.Close();

            return nameString;
        }

        public static string Get_Item_itemHelpStr(string name)
        {
            string helpString = "";

            baseFolder = "F:\\SanOL\\合并防挂成145版本";
            string itemHelperFile = baseFolder + "\\" + Item_helper;
            if (!File.Exists(itemHelperFile))
            {
                return helpString;
            }

            FileStream _itemHelpfs = new FileStream(itemHelperFile, FileMode.Open, FileAccess.Read);
            StreamReader _itemHelpReader = new StreamReader(_itemHelpfs, Encoding.GetEncoding(950));
            _itemHelpReader.BaseStream.Seek(0, SeekOrigin.Begin);
            _itemHelpReader.DiscardBufferedData();
            _itemHelpReader.BaseStream.Seek(0, SeekOrigin.Begin);
            _itemHelpReader.BaseStream.Position = 0;

            string strLine = "";
            strLine = _itemHelpReader.ReadLine();

            do
            {
                string _strLine = CFormat.ToSimplified(strLine);
                if (_strLine.Contains("item") && _strLine.Contains("="))
                {
                    _strLine = _strLine.Replace(" ", "").Replace("\t", "");
                    string id = _strLine.Split('=')[1].Split(',')[0];
                    if (id == name)
                    {
                        helpString = _strLine.Split('=')[1].Split(',')[1];
                        break;
                    }
                }
                strLine = "";
                strLine = _itemHelpReader.ReadLine();
            } while (strLine != null);

            _itemHelpReader.Close();
            _itemHelpfs.Close();

            return helpString;
        }
        public static int LastHelpId = -1;
        public static int LastNameId = -1;
        public static int Get_Item_LastHelpId()
        {
            baseFolder = "F:\\SanOL\\合并防挂成145版本";
            string itemHelperFile = baseFolder + "\\" + Item_helper;
            if (!File.Exists(itemHelperFile))
            {
                return -1;
            }

            if(LastHelpId < 0)
            {
                string[] srr = System.IO.File.ReadAllLines(itemHelperFile);
                string tmp = srr[srr.Length - 1].Replace(" ", "").Replace("\t", "");
                string id = tmp.Split('=')[1].Split(',')[0];
                LastHelpId = int.Parse(id);
            }
            return LastHelpId;
        }

        public static void Set_Item_LastHelpId(int _LastHelpId) 
        {
            LastHelpId = _LastHelpId;
        }

        public static int Get_Item_LastNameId()
        {
            baseFolder = "F:\\SanOL\\合并防挂成145版本";
            string itemNameFile = baseFolder + "\\" + Item_name;
            if (!File.Exists(itemNameFile))
            {
                return -1;
            }

            if(LastNameId < 0)
            {
                string[] srr = System.IO.File.ReadAllLines(CItemCtrl.Item_name);
                string tmp = srr[srr.Length - 1].Replace(" ", "").Replace("\t", "");
                string id = tmp.Split('=')[1].Split(',')[0];
                LastNameId = int.Parse(id);
            }

            return LastNameId;
        }
        public static void Set_Item_LastNameId(int _LastNameId)
        {
            LastNameId = _LastNameId;
        }
    }
}
