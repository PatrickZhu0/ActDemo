using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace StarWars
{
    public class BlackBoard
    {
        public TypedDataCollection BlackBoardDatas
        {
            get { return m_BlackBoardDatas; }
        }
        public void Reset()
        {
            m_BlackBoardDatas.Clear();
        }

        private TypedDataCollection m_BlackBoardDatas = new TypedDataCollection();
    }
}
