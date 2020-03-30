using ApiModel.Entities;
using ApiServer.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;


namespace ApiServer.Controllers.Global
{
    
    [Route("/[controller]")]
    public class SettingsController : Controller
    {
        private readonly ISettingRepository settingRepository;

        public SettingsController(ISettingRepository settingRepository)
        {
            this.settingRepository = settingRepository;
        }


        [HttpGet("{key}")]
        public async Task<IActionResult> Get(string key)
        {
            var entity = await settingRepository.GetByKey(key);
            if (entity == null) return NotFound();
            return Ok(entity.Value);
        }


        [HttpPost]
        public async Task<IActionResult> Post([FromBody]SettingCreateCommand data)
        {
            if (ModelState.IsValid == false)
                return BadRequest(ModelState);

            await settingRepository.CreateOrUpdateAsync(data.ToEntity());
            return Ok();
        }

        public class SettingCreateCommand
        {
            [Required]
            public string Key { get; set; }
            [Required]
            public string Value { get; set; }

            public SettingsItem ToEntity()
            {
                return new SettingsItem
                {
                    Key = Key,
                    Value = Value
                };
            }
        }

    }
}
