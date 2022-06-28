namespace Cirilla
{
    public class ResMatchInfo
    {
        public string file { get; private set; }
        public string md5 { get; private set; }

        public ResMatchInfo(string file, string md5)
        {
            this.file = file;
            this.md5 = md5;
        }
    }
}