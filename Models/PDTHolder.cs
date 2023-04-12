using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Web;

namespace CoreStoreMVC.Models
{
    public class PDTHolder
    {
        static string authToken, txToken, query, strResponse;

        public static PayPalResponse Success(string tx)
        {

            PayPalConfig payPalConfig = PayPal.GetPayPalConfig();
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            authToken = payPalConfig.AuthToken;
            txToken = tx;
            query = string.Format($"cmd=_notify-synch&tx={txToken}&at={authToken}");
            string url = payPalConfig.PostUrl;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = query.Length;

            StreamWriter strOut = new(request.GetRequestStream(), System.Text.Encoding.ASCII);
            strOut.Write(query);
            strOut.Close();

            StreamReader strIn = new(request.GetResponse().GetResponseStream());
            strResponse = strIn.ReadToEnd();
            strIn.Close();

            if (strResponse.StartsWith("SUCCESS"))
                return PDTHolder.Parse(strResponse);
            return null;
        }

        static PayPalResponse Parse(string postData)
        {
            string sKey, sValue;
            PayPalResponse pr = new();

            try
            {
                string[] stringArray = postData.Split('\n');
                int i;

                for (i = 1; i < stringArray.Length - 1; i++)
                {
                    string[] array1 = stringArray[i].Split('=');

                    sKey = array1[0];
                    sValue = HttpUtility.UrlDecode(array1[1]);

                    switch (sKey)
                    {
                        case "mc_gross":
                            pr.GrossTotal = double.Parse(sValue, CultureInfo.InvariantCulture);
                            break;
                        case "invoce":
                            pr.InvoceNumber = Convert.ToInt32(sValue);
                            break;
                        case "payment_status":
                            pr.PaymentStatus = Convert.ToString(sValue);
                            break;
                        case "first_name":
                            pr.PayerFirstName = Convert.ToString(sValue);
                            break;
                        case "mc_fee":
                            pr.PaymentFee = double.Parse(sValue, CultureInfo.InvariantCulture);
                            break;
                        case "business":
                            pr.BusinessEmail = Convert.ToString(sValue);
                            break;
                        case "payer_email":
                            pr.PayerEmail = Convert.ToString(sValue);
                            break;
                        case "Tx_Token":
                            pr.TxToken = Convert.ToString(sValue);
                            break;
                        case "last_name":
                            pr.PayerLastName = Convert.ToString(sValue);
                            break;
                        case "receiver_email":
                            pr.ReceiverEmail = Convert.ToString(sValue);
                            break;
                        case "item_name":
                            pr.ItemName = Convert.ToString(sValue);
                            break;
                        case "mc_currency":
                            pr.Currency = Convert.ToString(sValue);
                            break;
                        case "txn_id":
                            pr.TransactionId = Convert.ToString(sValue);
                            break;
                        case "custom":
                            pr.Custom = Convert.ToString(sValue);
                            break;
                        case "subscr_id":
                            pr.SubscriberId = Convert.ToString(sValue);
                            break;
                    }
                }
                return pr;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
