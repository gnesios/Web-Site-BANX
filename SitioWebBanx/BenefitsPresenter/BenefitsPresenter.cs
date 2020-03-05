using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

namespace SitioWebBanx.BenefitsPresenter
{
    [ToolboxItemAttribute(false)]
    public class BenefitsPresenter : WebPart
    {
        #region Global variables
        const string TheList = "Beneficios BANX";
        #endregion

        protected override void CreateChildControls()
        {
            try
            {
                string formatedValues = this.RetrieveFormatedValuesFromList(TheList);
                LiteralControl theScript = new LiteralControl();

                if (string.IsNullOrEmpty(formatedValues))
                {
                    theScript.Text = "No existen items que mostrar.";
                }
                else
                {
                    theScript.Text = string.Format(
                        "<ul class='grid cs-style-6'>{0}</ul>",
                        formatedValues);
                }

                this.Controls.Add(theScript);
            }
            catch (Exception ex)
            {
                LiteralControl errorMessage = new LiteralControl();
                errorMessage.Text = "ERROR >> " + ex.Message;
                //errorMessage.Text = "El control no fué configurado correctamente.";

                this.Controls.Clear();
                this.Controls.Add(errorMessage);
            }
        }

        private string RetrieveFormatedValuesFromList(string listName)
        {
            using (SPSite sps = new SPSite(SPContext.Current.Web.Url))
            using (SPWeb spw = sps.OpenWeb())
            {
                string formatedValues = "";

                SPQuery query = new SPQuery();
                query.Query =
                    "<OrderBy><FieldRef Name='ID' Ascending='FALSE' /></OrderBy>" +
                    "<Where><And>" +
                    "<Eq><FieldRef Name='_ModerationStatus' /><Value Type='ModStat'>0</Value></Eq>" +
                    "<Eq><FieldRef Name='Mostrar_x0020_Inicio' /><Value Type='Boolean'>1</Value></Eq>" +
                    "</And></Where>";
                SPListItemCollection items = spw.Lists[listName].GetItems(query);

                foreach (SPListItem item in items)
                {
                    string image = item["Imágen Beneficio"].ToString().Contains(",")
                        ? item["Imágen Beneficio"].ToString().Split(',')[0].Trim() : item["Imágen Beneficio"].ToString();
                    string title = item.Title;
                    string subtitle = item["Características Beneficio"].ToString();
                    string url = "";
                    string id = item.ID.ToString();
                    string lat = "";
                    string lng = "";
                    if (subtitle.Length > 49) { subtitle = subtitle.Substring(0, 49) + "..."; }
                    if (item["Dirección Beneficio"] != null)
                    {
                        try
                        {
                            lat = item["Dirección Beneficio"].ToString().Split('/')[0];
                            lng = item["Dirección Beneficio"].ToString().Split('/')[1];
                        }
                        catch { }
                    }
                    if (item["Página Beneficio"] != null)
                    {
                        url = item["Página Beneficio"].ToString().Contains(",")
                            ? item["Página Beneficio"].ToString().Split(',')[0].Trim() : item["Página Beneficio"].ToString();
                    }
                    else
                    {
                        url = string.Format("/Paginas/BeneficiosBanx/DetalleBeneficio.aspx?Bid={0}&Lat={1}&Lng={2}&Tit={3}",
                            id, lat, lng, title);
                    }

                    formatedValues = formatedValues + string.Format(
                        "<li><figure>" +
                        "<img src='{0}' alt='' />" +
                        "<figcaption>" +
                        "<p class='title'>{1}</p>" +
                        "<p class='subtitle'>{2}</p>" +
                        "<a href='{3}'>Click aquí</a>" +
                        "</figcaption>" +
                        "</figure></li>",
                        image, title, subtitle, url);
                }

                return formatedValues;
            }
        }
    }
}
