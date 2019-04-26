using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterparkChecker
{
    class ParamHelper
    {
        public static List<Querystring>  GetParamList(CampName type, string yyyymm = null)
        {
            List<Querystring> paramList = new List<Querystring>();

            if (string.IsNullOrEmpty(yyyymm))
                yyyymm = DateTime.Now.ToString("yyyyMM");

            switch (type)
            {
                case CampName.용자휴:
                    paramList.Add(new Querystring("idkey", "5M4200"));
                    paramList.Add(new Querystring("gd_seq", "GD70"));

                    break;
                case CampName.초막골:
                    paramList.Add(new Querystring("idkey", "5M4240"));
                    paramList.Add(new Querystring("gd_seq", "GD84"));

                    break;
                case CampName.왕송호수:
                    break;


            }
            paramList.Add(new Querystring("yyyymmdd", string.Format("{0}01", yyyymm)));
            paramList.Add(new Querystring("sd_date", string.Format("{0}01", yyyymm)));

            return paramList;
        }

        public static List<string> GetSiteList(CampName type)
        {
            List<string> list = null;

            switch (type)
            {
                case CampName.용자휴:
                    list = new List<string>() { "느티골", "가마골", "밤티골", "체험골", "목조주택(한옥)", "목조주택(핀란드)", "목조주택(몽골)", "인디언텐트", "캐빈하우스", "데크(4X4)", "데크(6x6)" };

                    break;
                case CampName.초막골:
                    list = new List<string>() { "글램-고급", "글램-일반", "야영장(자갈)", "야영장(테크)" };

                    break;
                case CampName.왕송호수:
                    list = new List<string>() { "글램핑", "카라반", "데크" };

                    break;


            }
            return list;
        }

    }

    enum CampName { 용자휴, 초막골, 왕송호수 }
}
