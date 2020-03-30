using System;

namespace Base.Domain
{
    public class GuidGen
    {

        private static HashidsNet.Hashids hashIds = new HashidsNet.Hashids();
        private static int serverId = 1;
        private static volatile int conTick = 1;

        public static void Init(string inServerId, string inSalt, string inMinLen)
        {
            var serverId = Convert.ToInt32(inServerId);
            var guidMinLen = Convert.ToInt32(inMinLen);
            Init(serverId, inSalt, guidMinLen);
        }

        public static void Init(int inServerId, string inSalt, int inMinLen)
        {
            serverId = inServerId;
            if (serverId < 1)
                serverId = 1;
            if (string.IsNullOrWhiteSpace(inSalt))
                inSalt = "ThisIsTheDefaultSalt";
            if (inMinLen < 0)
                inMinLen = 8;
            hashIds = new HashidsNet.Hashids(inSalt, inMinLen, "BCDFHJKNPQRSTUVWXYZMEGA0123456789");
        }

        /// <summary>
        /// 分配一个新的全局唯一ID
        /// </summary>
        /// <returns></returns>
        public static string NewGUID()
        {
            return hashIds.EncodeLong(serverId, DateTime.UtcNow.Ticks - 636503616000000000L, System.Threading.Interlocked.Increment(ref conTick)); // 636503616000000000 = ticks of 2018-01-01 00:00:00
        }
    }
}
