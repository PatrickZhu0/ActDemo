using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace StarWarsSpatial
{
    public sealed class TiledData
    {
        public bool IsPolygon
        {
            get { return m_IsPolygon; }
        }
        public List<UnityEngine.Vector3> GetPoints()
        {
            return m_PointList;
        }

        public bool CollectDataFromXml(XmlNode node)
        {
            float baseX = 0;
            float baseY = 0;
            List<float> datas = null;
            if (node.Name != "object")
            {
                return false;
            }
            XmlElement nodeElement = (XmlElement)node;
            baseX = (float)Convert.ToDouble(nodeElement.GetAttribute("x"));
            baseY = (float)Convert.ToDecimal(nodeElement.GetAttribute("y"));

            datas = new List<float>();
            XmlNode polylineData = node.SelectSingleNode("polyline");
            if (polylineData != null)
            {
                XmlElement polylineElement = (XmlElement)polylineData;
                string pointsData = polylineElement.GetAttribute("points");
                datas = ConvertNumericList<float>(pointsData);
                m_IsPolygon = false;
            }
            else
            {
                XmlNode polygonData = node.SelectSingleNode("polygon");
                if (polygonData != null)
                {
                    XmlElement polygonElement = (XmlElement)polygonData;
                    string pointsData = polygonElement.GetAttribute("points");
                    datas.AddRange(ConvertNumericList<float>(pointsData));
                    m_IsPolygon = true;
                }
            }
            ParseData(datas, baseX, baseY);
            return true;
        }

        public TiledData(float w, float h)
        {
            map_width_ = w;
            map_height_ = h;
        }

        private void ParseData(List<float> datas, float baseX, float baseY)
        {
            for (int i = 0; i < datas.Count - 1; ++i)
            {
                UnityEngine.Vector3 pos = new UnityEngine.Vector3();
                pos.x = (float)(baseX + datas[i]);
                pos.z = map_height_ - (float)(baseY + datas[i + 1]);
                m_PointList.Add(pos);
                i++;
            }
            if (m_PointList.Count > 2)
            {
                //检查是否是逆时针顺序
                if (!StarWars.Geometry.IsCounterClockwise(m_PointList, 0, m_PointList.Count))
                {
                    List<UnityEngine.Vector3> pts = m_PointList;
                    m_PointList = new List<UnityEngine.Vector3>();
                    for (int ix = pts.Count - 1; ix >= 0; --ix)
                    {
                        m_PointList.Add(pts[ix]);
                    }
                }
            }
        }

        private List<T> ConvertNumericList<T>(string vec)
        {
            List<T> list = new List<T>();
            string strPos = vec;
            string[] resut = strPos.Split(new string[] { ",", " " }, StringSplitOptions.None);

            try
            {
                if (resut != null && resut.Length > 0 && resut[0] != "")
                {
                    for (int index = 0; index < resut.Length; index++)
                    {
                        list.Add((T)Convert.ChangeType(resut[index], typeof(T)));
                    }
                }
            }
            catch (System.Exception ex)
            {
                string info = string.Format("ExtractNumeric Error vec:{0} ex:{1} stacktrace:{2}", vec, ex.Message, ex.StackTrace);
                StarWars.LogSystem.Debug(info);
                list.Clear();
            }
            return list;
        }

        private float map_width_ = 0;
        private float map_height_ = 0;
        private bool m_IsPolygon = false;
        private List<UnityEngine.Vector3> m_PointList = new List<UnityEngine.Vector3>();
    }
}
