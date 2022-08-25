using System;
using System.Collections.Generic;
using System.Text;

namespace SARecorder.SAGAT
{
    interface IQuestion
    {
        public int IntAnswer { get; }
        public string StringAnswer { get; }
        // 이미지 Answer
    }
}
