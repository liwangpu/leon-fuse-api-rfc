using ApiModel.Consts;
using ApiModel.Entities;
using ApiServer.Models;
using BambooCommon;
using BambooCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace ApiServer.Services
{
    public class AccountMan
    {
        public Data.ApiDbContext context;

        public AccountMan(Data.ApiDbContext context)
        {
            this.context = context;
        }

        //public async Task<AccountEditModel> Register(RegisterAccountModel param)
        //{
        //    var model = new AccountEditModel();
        //    if (param == null)
        //        return null;
        //    if (string.IsNullOrWhiteSpace(param.Mail))
        //        return null;
        //    string mail = param.Mail.Trim().ToLower();

        //    Account acc = await context.Accounts.FirstOrDefaultAsync(d => d.Mail == mail);
        //    if (acc == null)
        //    {
        //        acc = new Account();
        //        acc.Id = GuidGen.NewGUID();
        //        acc.Name = param.Name;
        //        acc.Password = Md5.CalcString(param.Password);
        //        acc.Mail = mail;
        //        acc.Frozened = false;
        //        acc.ActivationTime = DataHelper.ParseDateTime(param.ActivationTime);
        //        acc.ExpireTime = DataHelper.ParseDateTime(param.ExpireTime);
        //        acc.Type = param.Type;
        //        acc.Location = param.Location;
        //        acc.Phone = param.Phone;
        //        acc.OrganizationId = param.OrganizationId;
        //        if (param.Type == AppConst.AccountType_BrandAdmin)
        //            acc.Organization = await context.Organizations.FirstOrDefaultAsync(x => x.Id == param.OrganizationId);
        //        acc.DepartmentId = param.DepartmentId;
        //        //acc.Department = await context.Departments.FirstOrDefaultAsync(x => x.Id == param.DepartmentId);



        //        context.Accounts.Add(acc);
        //        await context.SaveChangesAsync();
        //    }
        //    model.Name = param.Name;
        //    model.Id = acc.Id;
        //    model.Mail = acc.Mail;
        //    //model.Password = acc.Password;
        //    model.Phone = acc.Phone;
        //    model.Location = acc.Location;
        //    model.Type = acc.Type;
        //    model.ActivationTime = DataHelper.FormatDateTime(acc.ActivationTime);
        //    model.ExpireTime = DataHelper.FormatDateTime(acc.ExpireTime);
        //    return model;
        //}

        public async Task<bool> ChangePasswordAsync(string accid, NewPasswordModel param)
        {
            if (param == null)
                return false;
            Account acc = await context.Accounts.FindAsync(accid);
            if (acc == null)
                return false;
            if (param.OldPassword != acc.Password)
                return false;
            acc.Password = param.NewPassword;

            await context.SaveChangesAsync();
            return true;
        }

        public async Task<AccountProfileModel> GetProfile(string accid)
        {
            Account acc = await context.Accounts.FindAsync(accid);
            if (acc == null)
                return null;
            AccountProfileModel p = new AccountProfileModel();
            p.Name = acc.Name;
            p.Avatar = acc.Icon;
            p.Brief = acc.Description;
            p.Location = acc.Location;
            p.OrganizationId = acc.OrganizationId;
            p.DepartmentId = acc.DepartmentId;

            return p;
        }

        public async Task<bool> UpdateProfile(string accid, AccountProfileModel param)
        {
            if (param == null)
                return false;
            Account acc = await context.Accounts.FindAsync(accid);
            if (acc == null)
                return false;
            acc.Name = param.Name;
            acc.Icon = param.Avatar;
            acc.Description = param.Brief;
            acc.Location = param.Location;

            await context.SaveChangesAsync();
            return true;
        }

        public async Task<NavigationModel> GetNavigation(string accid)
        {
            Account acc = await context.Accounts.FindAsync(accid);
            if (acc != null)
            {
                NavigationModel mm;
                if (SiteConfig.Instance.GetItem("navi_" + acc.Type, out mm))
                    return mm;
            }
            return null;
        }
    }
}
