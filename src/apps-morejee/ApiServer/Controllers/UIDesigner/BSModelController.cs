using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using Newtonsoft.Json;
using System.Text;
using ApiServer.Models;
using BambooCore;

namespace ApiServer.Controllers.UIDesigner
{
    [Authorize]
    [Route("/[controller]")]
    public class BSModelController : Controller
    {
        /// <summary>
        /// 当前这一块还在设计,变动很大,不易持久进数据库,临时用json文件模拟数据库
        /// </summary>
        protected string MockDbFilePath
        {
            get
            {
                return Path.Combine(@"D:\Leon\AppCode\bamboo\ApiServer\ApiServer\_for_test_folder", "mock_bsmodel_db.json");
            }
        }


        [HttpGet("{modelName}")]
        public async Task<IActionResult> Get(string modelName)
        {
            var modelList = new List<BSModelDefine>();

            if (System.IO.File.Exists(MockDbFilePath))
            {
                using (var fs = new StreamReader(MockDbFilePath, Encoding.UTF8))
                {
                    var str = fs.ReadToEnd();
                    modelList = JsonConvert.DeserializeObject<List<BSModelDefine>>(str);
                }
            }

            var refer = modelList.FirstOrDefault(x => x.Resource == modelName);
            if (refer != null)
                return Ok(refer);


            //if (modelName == "BSModel")
            //{
            //    var model = new BSModelDefine()
            //    {
            //        Resource = "BSModel",
            //        Icon = "map",
            //        DisplayModel = new List<string>() { "List" },
            //        Fields = new List<BSModelField>()
            //    {
            //        //new BSModelField(){ Id="Icon",Name="Icon",Width=85}
            //        new BSModelField(){ Id="Name",Name="Name",Width=125}
            //        , new BSModelField(){ Id="Description",Name="Description",Width=185}
            //    },
            //        PageSizeOptions = new List<int>() { 15, 25, 500 }
            //    };
            //    return Ok(model);
            //}
            //else
            {
                var model = new BSModelDefine()
                {
                    Resource = modelName,
                    Icon = "map",
                    DisplayModel = new List<string>() { "List" },
                    Fields = new List<BSModelField>()
                {
                    new BSModelField(){ Id="icon",Name="glossary.Icon",Width=85}
                   , new BSModelField(){ Id="name",Name="glossary.Name",Width=125,FormType="text",FormRequire=true,FormIndex=1}
                    , new BSModelField(){ Id="description",Name="glossary.Description",Width=185,FormType="textarea",FormIndex=2}
                },
                    PageSizeOptions = new List<int>() { 15, 25, 500 }
                };
                return Ok(model);
            }



        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] PagingRequestModel model)
        {

            //var model = new
            //{
            //    Resource = modelName,
            //    ModelType = "List",
            //    Icon = "map",
            //    DisplayModel = new List<string>() { "List" },
            //    Fields = new List<BSModelField>()
            //    {
            //        new BSModelField(){ Id="Icon",Name="Icon",Width=85}
            //        ,new BSModelField(){ Id="Name",Name="Name",Width=125}
            //        , new BSModelField(){ Id="Description",Name="Description",Width=185}
            //    },
            //    PageSizeOptions = new List<int>() { 15, 25, 500 }
            //};
            //return Ok(model);

            return Ok(new PagedData<BSModelDefine>() { });
        }
    }

    class BSModelDefine
    {
        public string Id { get; set; }
        public string Resource { get; set; }
        public string Icon { get; set; }
        public List<string> DisplayModel { get; set; }
        public List<BSModelField> Fields { get; set; }
        public List<int> PageSizeOptions { get; set; }
    }


    class BSModelField
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Width { get; set; }
        public string Expression { get; set; }
        public string FormType { get; set; }
        public string FormMin { get; set; }
        public string FormMax { get; set; }
        public bool FormRequire { get; set; }
        public int FormIndex { get; set; }


        //    id: string;
        //name: string;
        //description: string;
        //width: number;
        //expression: string;
    }
}