using System;
using System.Text;
using System.Web.Mvc;
using WebUI.Models;

namespace WebUI.HtmlHelpers
{
    public static class PagingHelpers
    {
        /// <summary>
        /// генерирует Html- разметку для набора ссылок на страницы
        /// </summary>
        /// <param name="html"></param>
        /// <param name="pagingInfo"></param>
        /// <param name="pageUrl">обеспечивает возможность передачи делегата, который будет применяться для генерирование ссылок </param>
        /// <returns></returns>
        public static MvcHtmlString PageLinks(this HtmlHelper html, PagingInfo pagingInfo, Func<int,string> pageUrl )
        {
            var result = new StringBuilder();
            for (int i=0;i<=pagingInfo.TotalPages;i++)
            {
                var tag = new TagBuilder("a"); // Создание дескриптора a
                tag.MergeAttribute("href", pageUrl(i));
                tag.InnerHtml = i.ToString();
                if(i == pagingInfo.CurrentPage)
                    tag.AddCssClass("selected");
                result.Append(tag.ToString());
            }
            return MvcHtmlString.Create(result.ToString());
        }

    }
}