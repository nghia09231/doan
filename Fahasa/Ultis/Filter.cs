using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace Fahasa.Ultis
{
    public class Filter
    {
        public static List<dynamic> Convert(string filterString)
        {
            JArray array = JArray.Parse(filterString);
            var result = new List<dynamic>();

            foreach (JToken token in array)
            {
                if (token.Type == JTokenType.Array)
                {
                    var subList = new List<dynamic>();
                    // Lặp qua mỗi phần tử trong mảng
                    foreach (JToken subToken in token)
                    {
                        if (subToken.Type == JTokenType.Array)
                        {
                            var innerSubList = new List<dynamic>();
                            for (int i = 0; i < subToken.Count(); i++)
                            {

                                Console.WriteLine(subToken[0].ToString());

                                if (subToken[0].ToString().ToLower().Contains("date") && i == 2)
                                {
                                    string dateStr = subToken[i].ToString();
                                    DateTime dateTime;
                                    dateTime = DateTime.ParseExact(dateStr, "MM/dd/yyyy HH:mm:sss", CultureInfo.InvariantCulture);
                                    innerSubList.Add(dateTime);
                                }
                                else
                                {
                                    innerSubList.Add(subToken[i].ToString());
                                }

                            }

                            subList.Add(innerSubList);
                        }
                        else
                        {
                            subList.Add(subToken.ToString());
                        }
                    }
                    result.Add(subList);
                }
                else
                {
                    result.Add(token.ToString());
                }
            }

            return result;
        }
    }
}