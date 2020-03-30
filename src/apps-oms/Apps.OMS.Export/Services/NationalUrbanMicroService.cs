using Apps.Base.Common;
using Apps.OMS.Export.DTOs;
using Flurl.Http;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Apps.OMS.Export.Services
{
    public class NationalUrbanMicroService : MicroServiceBase
    {

        #region 构造函数
        public NationalUrbanMicroService(string server)
         : base(server)
        {

        }
        public NationalUrbanMicroService(string server, string token)
            : base(server, token)
        {

        }
        #endregion

        #region GetById 根据Id获取省市区信息
        /// <summary>
        /// 根据Id获取省市区信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<NationalUrbanDTO> GetById(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return null;
            var dto = await $"{Server}/NationalUrban/{id}".AllowAnyHttpStatus().GetJsonAsync<NationalUrbanDTO>();
            return dto;
        }
        #endregion

        #region GetNameById 根据id获取省市区名称
        /// <summary>
        /// 根据id获取省市区名称
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<string> GetNameById(string id)
        {
            var name = await $"{Server}/NationalUrban/GetNameById?id={id}".AllowAnyHttpStatus().GetStringAsync();
            return name;
        }
        #endregion

        #region GetNameByIds 根据用户Ids获取省市区集合
        /// <summary>
        /// 根据用户Ids获取省市区集合
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public async Task<List<string>> GetNameByIds(string ids)
        {
            var names = await $"{Server}/NationalUrban/GetNameByIds?ids={ids}".AllowAnyHttpStatus().GetJsonAsync<List<string>>();
            return names;
        }
        #endregion

        #region GetNameByIds 根据用户Ids获取省市区集合
        /// <summary>
        /// 根据用户Ids获取省市区集合
        /// </summary>
        /// <param name="provinceId"></param>
        /// <param name="cityId"></param>
        /// <param name="countyId"></param>
        /// <param name="assign"></param>
        /// <returns></returns>
        public async Task GetNameByIds(string provinceId, string cityId, string countyId, Action<string, string, string> assign)
        {
            provinceId = string.IsNullOrEmpty(provinceId) ? string.Empty : provinceId;
            cityId = string.IsNullOrEmpty(cityId) ? string.Empty : cityId;
            countyId = string.IsNullOrEmpty(countyId) ? string.Empty : countyId;
            var ids = $"{provinceId},{cityId},{countyId}";
            var names = await GetNameByIds(ids);
            assign(names[0], names[1], names.Count > 2 ? names[2] : "");
        }
        #endregion

    }
}
