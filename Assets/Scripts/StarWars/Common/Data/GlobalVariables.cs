using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StarWars
{
    /// <summary>
    /// 这里放客户端与服务器存在差异的变量值，供各公共模块使用（如果是各模块所需的逻辑数据，则不要放在这里，独立写读表器）。
    /// </summary>
    public class GlobalVariables
    {
        public const int c_TotalPreservedRoomCount = 64;
        public const int c_PreservedRoomCountPerThread = 4;

        public bool IsClient
        {
            get
            {
                return m_IsClient;
            }
            set
            {
                m_IsClient = value;
            }
        }
        public bool IsDebug
        {
            get { return m_IsDebug; }
            set { m_IsDebug = value; }
        }
        public bool IsPublish
        {
            get { return m_IsPublish; }
            set { m_IsPublish = value; }
        }
        public bool IsWaitQuit
        {
            get { return m_IsWaitQuit; }
            set { m_IsWaitQuit = value; }
        }

        public Dictionary<string, string> EncodeTable
        {
            get { return m_EncodeTable; }
        }
        public Dictionary<string, string> DecodeTable
        {
            get { return m_DecodeTable; }
        }

        private static void AddCrypto(string s, string d, Dictionary<string, string> encodeTable, Dictionary<string, string> decodeTable)
        {
            encodeTable.Add(s, d);
            decodeTable.Add(d, s);
        }

        private GlobalVariables()
        {
            AddCrypto("skill", "_1_", m_EncodeTable, m_DecodeTable);
            AddCrypto("section", "_2_", m_EncodeTable, m_DecodeTable);
            AddCrypto("initialization", "_3_", m_EncodeTable, m_DecodeTable);
            AddCrypto("onstop", "_4_", m_EncodeTable, m_DecodeTable);
            AddCrypto("oninterrupt", "_5_", m_EncodeTable, m_DecodeTable);
            AddCrypto("story", "_6_", m_EncodeTable, m_DecodeTable);
            AddCrypto("local", "_7_", m_EncodeTable, m_DecodeTable);
            AddCrypto("onmessage", "_8_", m_EncodeTable, m_DecodeTable);
            AddCrypto("foreach", "_9_", m_EncodeTable, m_DecodeTable);
            AddCrypto("loop", "_10_", m_EncodeTable, m_DecodeTable);
            AddCrypto("looplist", "_11_", m_EncodeTable, m_DecodeTable);
            AddCrypto("while", "_12_", m_EncodeTable, m_DecodeTable);
            AddCrypto("if", "_13_", m_EncodeTable, m_DecodeTable);
            AddCrypto("else", "_14_", m_EncodeTable, m_DecodeTable);
            AddCrypto("inc", "_15_", m_EncodeTable, m_DecodeTable);
            AddCrypto("dec", "_16_", m_EncodeTable, m_DecodeTable);
            AddCrypto("assign", "_17_", m_EncodeTable, m_DecodeTable);
            AddCrypto("propset", "_18_", m_EncodeTable, m_DecodeTable);
            AddCrypto("propget", "_19_", m_EncodeTable, m_DecodeTable);
            AddCrypto("terminate", "_20_", m_EncodeTable, m_DecodeTable);
            AddCrypto("localmessage", "_21_", m_EncodeTable, m_DecodeTable);
            AddCrypto("wait", "_22_", m_EncodeTable, m_DecodeTable);
            AddCrypto("sleep", "_23_", m_EncodeTable, m_DecodeTable);
            AddCrypto("log", "_24_", m_EncodeTable, m_DecodeTable);
            AddCrypto("format", "_25_", m_EncodeTable, m_DecodeTable);
            AddCrypto("substring", "_26_", m_EncodeTable, m_DecodeTable);
            AddCrypto("startstory", "_27_", m_EncodeTable, m_DecodeTable);
            AddCrypto("stopstory", "_28_", m_EncodeTable, m_DecodeTable);
            AddCrypto("firemessage", "_29_", m_EncodeTable, m_DecodeTable);
            AddCrypto("showmissioncomplete", "_30_", m_EncodeTable, m_DecodeTable);
            AddCrypto("missioncomplete", "_31_", m_EncodeTable, m_DecodeTable);
            AddCrypto("list", "_32_", m_EncodeTable, m_DecodeTable);
            AddCrypto("showui", "_33_", m_EncodeTable, m_DecodeTable);
            AddCrypto("showdlg", "_34_", m_EncodeTable, m_DecodeTable);
            AddCrypto("showwall", "_35_", m_EncodeTable, m_DecodeTable);
            AddCrypto("cameralookat", "_36_", m_EncodeTable, m_DecodeTable);
            AddCrypto("camerafollow", "_37_", m_EncodeTable, m_DecodeTable);
            AddCrypto("cameralookatimmediately", "_38_", m_EncodeTable, m_DecodeTable);
            AddCrypto("camerafollowimmediately", "_39_", m_EncodeTable, m_DecodeTable);
            AddCrypto("createnpc", "_40_", m_EncodeTable, m_DecodeTable);
            AddCrypto("destroynpc", "_41_", m_EncodeTable, m_DecodeTable);
            AddCrypto("npcface", "_42_", m_EncodeTable, m_DecodeTable);
            AddCrypto("npcmove", "_43_", m_EncodeTable, m_DecodeTable);
            AddCrypto("npcmovewithwaypoints", "_44_", m_EncodeTable, m_DecodeTable);
            AddCrypto("npcpatrol", "_45_", m_EncodeTable, m_DecodeTable);
            AddCrypto("npcstop", "_46_", m_EncodeTable, m_DecodeTable);
            AddCrypto("npcattack", "_47_", m_EncodeTable, m_DecodeTable);
            AddCrypto("enableai", "_48_", m_EncodeTable, m_DecodeTable);
            AddCrypto("setcamp", "_49_", m_EncodeTable, m_DecodeTable);
            AddCrypto("getcamp", "_50_", m_EncodeTable, m_DecodeTable);
            AddCrypto("isenemy", "_51_", m_EncodeTable, m_DecodeTable);
            AddCrypto("isfriend", "_52_", m_EncodeTable, m_DecodeTable);
            AddCrypto("enableinput", "_53_", m_EncodeTable, m_DecodeTable);
            AddCrypto("playerselfface", "_54_", m_EncodeTable, m_DecodeTable);
            AddCrypto("playerselfmove", "_55_", m_EncodeTable, m_DecodeTable);
            AddCrypto("playerselfmovewithwaypoints", "_56_", m_EncodeTable, m_DecodeTable);
            AddCrypto("objface", "_57_", m_EncodeTable, m_DecodeTable);
            AddCrypto("objmove", "_58_", m_EncodeTable, m_DecodeTable);
            AddCrypto("objmovewithwaypoints", "_59_", m_EncodeTable, m_DecodeTable);
            AddCrypto("time", "_60_", m_EncodeTable, m_DecodeTable);
        }

        private bool m_IsClient = false;
        private bool m_IsDebug = false;
        private bool m_IsPublish = false;
        private bool m_IsWaitQuit = false;
        private Dictionary<string, string> m_EncodeTable = new Dictionary<string, string>();
        private Dictionary<string, string> m_DecodeTable = new Dictionary<string, string>();

        public static GlobalVariables Instance
        {
            get { return s_Instance; }
        }
        private static GlobalVariables s_Instance = new GlobalVariables();

        public static int GetUnitIdByCampId(int campid)
        {
            if (campid == (int)CampIdEnum.Blue)
                return 20001;
            else
                return 20002;
        }
    }
}
