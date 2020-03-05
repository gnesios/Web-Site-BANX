using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

namespace SitioWebBanx.ToolsPresenter
{
    [ToolboxItemAttribute(false)]
    public class ToolsPresenter : WebPart
    {
        #region Global variables
        const string TheList = "Productos BANX";
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
                    theScript.Text = formatedValues;
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
                    string id = item.ID.ToString();
                    string image = "/" + item.Url;
                    string title = (item.Title != null) ? title = item.Title : "";
                    string subtitle = (item["Descripción"] != null) ? subtitle = item["Descripción"].ToString() : "";
                    string link = "";
                    if (item["Enlace"] != null)
                    {
                        if (item["Enlace"].ToString().Contains(","))
                            link = item["Enlace"].ToString().Split(',')[0].Trim();
                        else
                            link = item["Enlace"].ToString();
                    }
                    // (item["Enlace"] != null) ? link = item["Enlace"].ToString() : "#";

                    formatedValues = formatedValues + string.Format(
                        "<div class='slide' id='slide{0}' data-anchor='slide{0}' style='background-image:url(\"{1}\")'>" +
                        "<div class='section-container'>" +
                        "<h1>{2}</h1>" +
                        "<p>{3} <a href='{4}'>Click aquí.</a></p>" +
                        "</div></div>",
                        id, image, title, subtitle, link);
                }

                return formatedValues;
            }
        }
    }
}
