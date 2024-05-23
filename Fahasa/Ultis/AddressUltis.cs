using Fahasa.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Fahasa.Ultis
{
    public class AddressUltis
    {
        public async Task<List<ProvinceGHN>> GetProvinces()
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls13;

                    client.DefaultRequestHeaders.Add("Token", Properties.Setting.Token_API_GHN.ToString());
                    client.DefaultRequestHeaders.Add("ShopId", Properties.Setting.ShopID_API_GHN.ToString());
                    HttpResponseMessage response = await client.GetAsync("https://dev-online-gateway.ghn.vn/shiip/public-api/master-data/province");


                    if (response.IsSuccessStatusCode)
                    {
                        string resStr = await response.Content.ReadAsStringAsync();
                        var res = new HttpResponse<List<ProvinceGHN>>();
                        JsonConvert.PopulateObject(resStr, res);
                        return res.data;
                    }
                    else
                    {
                        return new List<ProvinceGHN>();
                    }
                }
                catch (HttpRequestException ex)
                {
                    return new List<ProvinceGHN>();
                }
            }
        }

        public async Task<List<DistrictGHN>> GetDistricts(int province_id)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls13;

                    client.DefaultRequestHeaders.Add("Token", Properties.Setting.Token_API_GHN.ToString());
                    client.DefaultRequestHeaders.Add("ShopId", Properties.Setting.ShopID_API_GHN.ToString());
                    HttpResponseMessage response = await client.GetAsync("https://dev-online-gateway.ghn.vn/shiip/public-api/master-data/district?province_id=" + province_id.ToString());


                    if (response.IsSuccessStatusCode)
                    {
                        string resStr = await response.Content.ReadAsStringAsync();
                        var res = new HttpResponse<List<DistrictGHN>>();
                        JsonConvert.PopulateObject(resStr, res);
                        return res.data;
                    }
                    else
                    {
                        return new List<DistrictGHN>();
                    }
                }
                catch (HttpRequestException ex)
                {
                    return new List<DistrictGHN>();
                }
            }
        }

        public async Task<List<WardGHN>> GetWards(int district_id)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls13;

                    client.DefaultRequestHeaders.Add("Token", Properties.Setting.Token_API_GHN.ToString());
                    client.DefaultRequestHeaders.Add("ShopId", Properties.Setting.ShopID_API_GHN.ToString());
                    HttpResponseMessage response = await client.GetAsync("https://dev-online-gateway.ghn.vn/shiip/public-api/master-data/ward?district_id=" + district_id.ToString());


                    if (response.IsSuccessStatusCode)
                    {
                        string resStr = await response.Content.ReadAsStringAsync();
                        var res = new HttpResponse<List<WardGHN>>();
                        JsonConvert.PopulateObject(resStr, res);
                        return res.data;
                    }
                    else
                    {
                        return new List<WardGHN>();
                    }
                }
                catch (HttpRequestException ex)
                {
                    return new List<WardGHN>();
                }
            }
        }

        public async Task<ProvinceGHN> GetProvinceByName(string name)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls13;

                    client.DefaultRequestHeaders.Add("Token", Properties.Setting.Token_API_GHN.ToString());
                    client.DefaultRequestHeaders.Add("ShopId", Properties.Setting.ShopID_API_GHN.ToString());
                    HttpResponseMessage response = await client.GetAsync("https://dev-online-gateway.ghn.vn/shiip/public-api/master-data/province");


                    if (response.IsSuccessStatusCode)
                    {
                        string resStr = await response.Content.ReadAsStringAsync();
                        var res = new HttpResponse<List<ProvinceGHN>>();
                        JsonConvert.PopulateObject(resStr, res);
                        var province = res.data.Find(x => x.ProvinceName == name);
                        
                        return province != null ? province : null;
                    }
                    else
                    {
                        return new ProvinceGHN();
                    }
                }
                catch (HttpRequestException ex)
                {
                    return new ProvinceGHN();
                }
            }
        }

        public async Task<DistrictGHN> GetDistrictByName(string name, int province_id)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls13;

                    client.DefaultRequestHeaders.Add("Token", Properties.Setting.Token_API_GHN.ToString());
                    client.DefaultRequestHeaders.Add("ShopId", Properties.Setting.ShopID_API_GHN.ToString());
                    HttpResponseMessage response = await client.GetAsync("https://dev-online-gateway.ghn.vn/shiip/public-api/master-data/district?province_id=" + province_id.ToString());


                    if (response.IsSuccessStatusCode)
                    {
                        string resStr = await response.Content.ReadAsStringAsync();
                        var res = new HttpResponse<List<DistrictGHN>>();
                        JsonConvert.PopulateObject(resStr, res);
                        var district = res.data.Find(x => x.DistrictName == name);

                        return district != null ? district : null;
                    }
                    else
                    {
                        return new DistrictGHN();
                    }
                }
                catch (HttpRequestException ex)
                {
                    return new DistrictGHN();
                }
            }
        }

        public async Task<WardGHN> GetWardByName(string name, int district_id)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls13;

                    client.DefaultRequestHeaders.Add("Token", Properties.Setting.Token_API_GHN.ToString());
                    client.DefaultRequestHeaders.Add("ShopId", Properties.Setting.ShopID_API_GHN.ToString());
                    HttpResponseMessage response = await client.GetAsync("https://dev-online-gateway.ghn.vn/shiip/public-api/master-data/ward?district_id=" + district_id.ToString());


                    if (response.IsSuccessStatusCode)
                    {
                        string resStr = await response.Content.ReadAsStringAsync();
                        var res = new HttpResponse<List<WardGHN>>();
                        JsonConvert.PopulateObject(resStr, res);
                        var ward = res.data.Find(x => x.WardName == name);

                        return ward != null ? ward : null;
                    }
                    else
                    {
                        return new WardGHN();
                    }
                }
                catch (HttpRequestException ex)
                {
                    return new WardGHN();
                }
            }
        }


    }
}