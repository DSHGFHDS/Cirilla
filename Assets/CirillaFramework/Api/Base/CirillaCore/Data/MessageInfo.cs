using System;

namespace Cirilla
{
    public class MessageInfo
    {
        public Action<object[]> callBack { get; private set; }
        public object[] args { get; private set; }

        public MessageInfo(Action<object[]> callBack, params object[] args)
        {
            this.callBack = callBack;
            this.args = args;
        }
    }
}
