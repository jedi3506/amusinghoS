﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using amusinghoS.App.Dto;
using amusinghoS.Services;
using Microsoft.AspNetCore.Mvc;
using amusinghoS.Shared;
using amusinghoS.Redis;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace amusinghoS.App.Controllers
{
    public class ArticleController : Controller
    {
        UnitOfWork _unitWork;
        private readonly IRedisClient _redisclient;
        public ArticleController(UnitOfWork unitOfWork,IRedisClient redisClient)
        {
            _unitWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _redisclient = redisClient ?? throw new ArgumentNullException(nameof(redisClient));
        }
        [Route("/Article/{articleid}")]
        public IActionResult Index(string articleid)
        {
            _redisclient.Set("demo","demostr",300);
            var amusingArticleList = _unitWork.amusingArticleRepository.GetAll();
            var commentList = from article in amusingArticleList
                              join comment in _unitWork.amusingArticleCommentRepository.GetAll()
                              on article.articleId equals comment.amusingArticleId
                              where article.articleId == articleid
                              orderby comment.CreateTime
                              select new
                              {
                                  comment.content,
                                  comment.commentatorName
                              };

            var model = (from details in _unitWork.amusingArticleDeatilsRepository.GetAll()
                         join article in _unitWork.amusingArticleRepository.GetAll()
                         on details.amusingArticleId equals article.articleId
                         where articleid == details.amusingArticleId
                         select new ArticleViewModel()
                         {
                             html = details.Html,
                             formatdetatime = article.CreateTime.ToString() +
                             DatetimeHelper.GetPeriod(article.CreateTime),
                             title = article.Title
                         }).FirstOrDefault();

            ViewData["ViewBinding"] = model;
            return View();
        }
    }
}
