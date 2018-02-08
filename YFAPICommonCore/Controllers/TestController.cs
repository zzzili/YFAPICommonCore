using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using EntityFrameworkCore;
using EntityFrameworkCore.UserModel;
using IdentityModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace YFAPICommonCore.Controllers
{
    [Produces("application/json")]
    [Route("api/Test/[action]")]
    public class TestController : Controller
    {
        private readonly MyDBContext dbContext;
        public TestController(MyDBContext _dbContext)
        {
            dbContext = _dbContext;
        }

        /// <summary>
        /// 测试基础
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public string TestHelloWorld()
        {
            return "This is Hello World";
        }

        /// <summary>
        /// 测试身份认证
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public string TestAuth()
        {
            var identity = (ClaimsIdentity)User.Identity;
            var id = identity.Claims.FirstOrDefault(u => u.Type == JwtClaimTypes.Id).Value;
            return "test auth";
        }

        /// <summary>
        /// 测试数据库操作
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public string TestDB()
        {
            User user = new User();
            user.Name = "小明";
            user.Email = "xx@qq.com";
            dbContext.User.Add(user);
            dbContext.SaveChanges();

            return "insert into mysql success";
        }
    }
}