using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Base.Domain.Common
{
    public class MD5Gen
    {
        public static string CalcFile(string localPath)
        {
            string hashMD5 = "";

            //检查文件是否存在，如果文件存在则进行计算，否则返回空值
            if (File.Exists(localPath))
            {
                using (FileStream fs = new FileStream(localPath, FileMode.Open, FileAccess.Read))
                {
                    //计算文件的MD5值
                    using (var calculator = MD5.Create())
                    {
                        byte[] buffer = calculator.ComputeHash(fs);
                        calculator.Clear();
                        //将字节数组转换成十六进制的字符串形式
                        StringBuilder stringBuilder = new StringBuilder();
                        for (int i = 0; i < buffer.Length; i++)
                        {
                            stringBuilder.Append(buffer[i].ToString("x2"));
                        }
                        hashMD5 = stringBuilder.ToString();
                    }
                }//关闭文件流

            }//结束计算

            return hashMD5;
        }

        public static string CalcString(string str)
        {
            using (var calculator = MD5.Create())
            {
                byte[] buffer = calculator.ComputeHash(Encoding.UTF8.GetBytes(str));
                calculator.Clear();
                //将字节数组转换成十六进制的字符串形式
                StringBuilder stringBuilder = new StringBuilder();
                for (int i = 0; i < buffer.Length; i++)
                {
                    stringBuilder.Append(buffer[i].ToString("x2"));
                }
                return stringBuilder.ToString();
            }
        }
    }
}
