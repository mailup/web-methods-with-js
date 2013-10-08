using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Net;
using System.IO;
using System.Web.Script.Serialization;
using System.Configuration;

namespace wsAjaxXmlSubscribe
{
	public partial class ajax : System.Web.UI.Page
	{
		#region class private vars
        /* Check and edit web.config ( ConsoleUrl tag ) to set your console URL
         * and make sure frontend web methods are enabled under your console configuration.
         * Read more at http://help.mailup.com/display/mailupUserGuide/Connecting+to+MailUp).
        */
        private string consoleUrl = ConfigurationManager.AppSettings["ConsoleUrl"].ToString();
		#endregion

		protected void Page_Load(object sender, EventArgs e)
		{
			// check if ajax callback type
			if(!String.IsNullOrEmpty(Request.ServerVariables["HTTP_X_REQUESTED_WITH"]) && Request.ServerVariables["HTTP_X_REQUESTED_WITH"].ToLower() == "xmlhttprequest") 
			{
				// make sure posted vars are sent in with POST
				if (Request.RequestType == "POST")
				{
					string Email = Request.Params["Email"] != null ? Request.Params["Email"].ToString() : "";
					string Nome = Request.Params["Nome"] != null ? Request.Params["Nome"].ToString() : "";
					string IDList = Request.Params["Lista"] != null ? Request.Params["Lista"].ToString() : "";
					string IDGroup = Request.Params["Gruppo"] != null ? Request.Params["Gruppo"].ToString() : "";
					string Language = Request.Params["Lingua"] != null ? Request.Params["Lingua"].ToString() : "";
					
					string RequiredConfirmation = Request.Params["RequireConfirmation"] != null ? Request.Params["RequireConfirmation"].ToString() : "";
					
					string csvFldNames = Request.Params["CsvFieldName"] != null ? Request.Params["CsvFieldName"].ToString() : "";
					string csvFldValues = Request.Params["CsvFieldValue"] != null ? Request.Params["CsvFieldValue"].ToString() : "";
					
					string ReturnCode = Request.Params["ReturnCode"] != null ? Request.Params["ReturnCode"].ToString() : "";

					string Number =  String.Empty;
                    //phone number formatting
					if (!String.IsNullOrEmpty(Request.Params["Prefix"]) && !String.IsNullOrEmpty(Request.Params["Number"]))
						Number = String.Format("{0}{1}", Request.Params["Prefix"], Request.Params["Number"]).Replace("+", "00");

                    //remote callback to MailUp frontend method
					string response = CallXmlSubscribeConsole(Email,
															  Nome,
															  IDList,
															  IDGroup,
															  Language,
															  RequiredConfirmation,
															  csvFldNames,
															  csvFldValues,
															  ReturnCode,
															  Number);

					JavaScriptSerializer serializer = new JavaScriptSerializer();

                    //serialize output in json compliance
					string json = serializer.Serialize(new { 
						Message = response
					});

                    //prepare for output
					Response.Clear();
					Response.ContentType = "application/json; charset=utf-8";
                    //output result
					Response.Write(json);
					Response.End();
				}
			}
		}

		private string CallXmlSubscribeConsole(string Email, string Nome, string IDList, string IDGroup, string Language, string RequiredConfirmation, string csvFldNames, string csvFldValues, string ReturnCode, string Number)
		{
			string result = String.Empty;

			try
			{
				String console_url = consoleUrl;

                //prepare query string
				console_url = String.Format("{0}?email={1}&name={2}&list={3}&group={4}&lang={5}&confirm={6}&csvFldNames={7}&csvFldValues={8}&retCode={9}&sms={10}", console_url, Email, Nome, IDList, IDGroup, Language, RequiredConfirmation, csvFldNames, csvFldValues, ReturnCode, Number);
                //init callback
				HttpWebRequest web_req = (HttpWebRequest)WebRequest.Create(console_url);
                //expect result
				HttpWebResponse web_resp = (HttpWebResponse)web_req.GetResponse();

				using (Stream stream_resp = web_resp.GetResponseStream())
				{
					using (StreamReader data = new StreamReader(stream_resp))
					{
                        //read result
						result = data.ReadToEnd();
					}
				}

				return result;
			}
			catch (Exception exc)
			{
				throw exc;
			}
		}
	}
}