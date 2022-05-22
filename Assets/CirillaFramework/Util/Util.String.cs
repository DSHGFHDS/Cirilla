using System.Text;
using System.Text.RegularExpressions;

namespace Cirilla
{
    public partial class Util
    {
        public static bool IsNumeric(string str)
        {
            if (str == null || str.Length == 0)
                return false;

            ASCIIEncoding ascii = new ASCIIEncoding();
            byte[] bytestr = ascii.GetBytes(str);

            foreach (byte c in bytestr)
            {
                if (c != 46)
                {
                    if (c < 48 || c > 57)
                        return false;

                    continue;
                }
                return true;
            }
            return true;
        }

        public static bool IsMatchStatementRule(string str)
        {
            if (str[0] >= 0 && str[0] <= 57)
                return false;

            Regex regExp = new Regex("[ \\[ \\] \\^ \\-*×――(^)$%~!＠@＃#$…&%￥—+=<>《》!！??？:：•`·、。，；,.;/\'\"{}（）‘’“”-]");
            if (regExp.IsMatch(str))
                return false;

            return true;
        }
    }
}
