using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterparkChecker
{
    class MainProcessor
    {
        const string _url = @"http://forest.maketicket.co.kr/camp/reserve/calendar.jsp";

        public static string CheckProcessor(CampName type, List<string> dateList)
        {
            string resultText = string.Empty;

            if (dateList == null || dateList.Count == 0)
            {
                throw new Exception("날짜를 1일이상 선택하세요");
            }

            // 년월 목록 조회
            var yyyymmlist = dateList.GroupBy(x => x.Substring(0, 6)).Select(x => x.Key);

            foreach (var yyyymm in yyyymmlist)
            {
                // 파라미터 생성
                List<Querystring> paramList = ParamHelper.GetParamList(type, yyyymm);

                // 조회
                var result = Task.Run<string>(async () => await Scrapper.RequestHttpClient(_url, Method.PORT, paramList));
                result.Wait();
                var text = result.Result;

                // 자리 확인
                resultText = UsableSiteChecker(type, text, dateList.Where(x => x.Substring(0, 6) == yyyymm).ToList());
            }

            return resultText;
        }

        private static string UsableSiteChecker(CampName type, string text, List<string> dateList)
        {
            string resultText = string.Empty;

            //날짜 < strong > 4 </ strong >
            foreach (var date in dateList)
            {
                string searchText = string.Format("<strong>{0}</strong>", Int32.Parse(date.Substring(6, 2)));

                int startIndex = text.IndexOf(searchText);
                startIndex += searchText.Length;

                int endIndex = text.IndexOf("</td>", startIndex);
                endIndex -= 1;

                string selectedText = text.Substring(startIndex, endIndex - startIndex);

                resultText += selectedText + Environment.NewLine;
            }

            return resultText;
        }
    }
}
