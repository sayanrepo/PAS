using System;
using System.Collections.Generic;
using System.Linq;


namespace BaseSite.Services.SmsService
{
    public class SmsKavenegar : ISmsService
    {
        public SmsKavenegar() : base()
        {
        }
        public async Task SendSms(string receptor, string tokenOrText, string templateName = "", string token2 = "", string token3 = "", string token10 = "", string token20 = "")
        {
            if (!string.IsNullOrWhiteSpace(receptor))
            {
                //string excludedReceptors = System.Configuration.ConfigurationManager.AppSettings["SmsExcluded"] == null ? "" : System.Configuration.ConfigurationManager.AppSettings["SmsExcluded"].ToString();
                //if (!string.IsNullOrEmpty(excludedReceptors) && excludedReceptors.Split(',').Contains(receptor))
                //    return;

                //string smsapikey = System.Configuration.ConfigurationManager.AppSettings["SmsApiKey"].ToString();
                string smsapikey = "5430706550703379576B70344E6943452B554868764C3069737351552B65572F372F4C2F6A686C59724A6B3D";

                try
                {
                    Kavenegar.KavenegarApi api = new Kavenegar.KavenegarApi(smsapikey);
                    if (string.IsNullOrWhiteSpace(templateName))
                    {
                        var res = api.Send("10002200020200", receptor, tokenOrText);
                        foreach (var mobile in SupportPhones)
                            await api.Send("10002200020200", mobile, tokenOrText);
                    }
                    else
                    {
                        Kavenegar.Core.Models.SendResult res;
                        if (string.IsNullOrWhiteSpace(token2) && string.IsNullOrWhiteSpace(token3) && string.IsNullOrWhiteSpace(token10) && string.IsNullOrWhiteSpace(token20))
                        {
                            res = await api.VerifyLookup(receptor, tokenOrText, templateName);
                            foreach (var mobile in SupportPhones)
                                await api.VerifyLookup(mobile, tokenOrText, templateName);
                        }
                        else if (string.IsNullOrWhiteSpace(token10) && string.IsNullOrWhiteSpace(token20))
                        {
                            res = await api.VerifyLookup(receptor, tokenOrText, token2, token3, templateName);
                            foreach (var mobile in SupportPhones)
                                await api.VerifyLookup(mobile, tokenOrText, token2, token3, templateName);
                        }
                        else if (string.IsNullOrWhiteSpace(token20))
                        {
                            res = await api.VerifyLookup(receptor, tokenOrText, token2, token3, token10, templateName);
                            foreach (var mobile in SupportPhones)
                                await api.VerifyLookup(mobile, tokenOrText, token2, token3, token10, templateName);
                        }
                        else
                        {
                            res = await api.VerifyLookup(receptor, tokenOrText, token2, token3, token10, token20, templateName, Kavenegar.Core.Models.Enums.VerifyLookupType.Sms);
                            foreach (var mobile in SupportPhones)
                                await api.VerifyLookup(mobile, tokenOrText, token2, token3, token10, token20, templateName, Kavenegar.Core.Models.Enums.VerifyLookupType.Sms);
                        }
                        //foreach (Kavenegar.Models.SendResult r in res)
                        {
                            Console.Write(res.Messageid.ToString());  //Collect MessageId(s) and store them
                        }
                    }
                }
                catch (Kavenegar.Core.Exceptions.ApiException ex)
                {
                    // در صورتی که خروجی وب سرویس 200 نباشد این خطارخ می دهد.
                    Console.Write("Message : " + ex.Message);
                }
                catch (Kavenegar.Core.Exceptions.HttpException ex)
                {
                    // در زمانی که مشکلی در برقرای ارتباط با وب سرویس وجود داشته باشد این خطا رخ می دهد
                    Console.Write("Message : " + ex.Message);
                }
            }
        }
    }
}