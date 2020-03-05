using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using System.Collections.Generic;

namespace SitioWebBanx.RankingPresenter
{
    [ToolboxItemAttribute(false)]
    public class RankingPresenter : WebPart
    {
        #region Web part parameters
        private string spTextFile;
        [Personalizable(PersonalizationScope.Shared),
        WebBrowsable(true),
        WebDisplayName("Archivo fuente"),
        WebDescription("Dirección y nombre del archivo de texto a ser leido."),
        Category("Configuración")]
        public string SpTextFile
        {
            get { return spTextFile; }
            set { spTextFile = value; }
        }
        #endregion

        public RankingPresenter() : base()
        {
            SpTextFile = @"c:\ranking_promos.txt";
        }

        protected override void CreateChildControls()
        {
            try
            {
                List<string> fileContent = this.GetFileContent(SpTextFile.Trim());
                string chartScript = this.GetChartScript(fileContent);
                
                LiteralControl formatedScript = new LiteralControl();
                formatedScript.Text =
                    "<div class='filter-bar'>" +
                    "<button type='button' class='action filter__item' data-filter='COCHABAMBA'>COCHABAMBA</button>" +
                    "<button type='button' class='action filter__item' data-filter='LA PAZ'>LA PAZ</button>" +
                    "<button type='button' class='action filter__item' data-filter='SANTA CRUZ'>SANTA CRUZ</button>" +
                    "<button type='button' class='action filter__item' data-filter='RESTO'>RESTO DEL PAÍS</button>" +
                    "</div>" +
                    "<p id='result' style='display:none'>No existen datos.</p>" +
                    "<canvas id='theChartCBB' style='width:100% !important; display:none' height='170'></canvas>" +
                    "<canvas id='theChartLPZ' style='width:100% !important; display:none' height='170'></canvas>" +
                    "<canvas id='theChartSCZ' style='width:100% !important; display:none' height='170'></canvas>" +
                    "<canvas id='theChartRST' style='width:100% !important; display:none' height='170'></canvas>" +
                    "<br/><br/>" +
                    "<table id='rankTable' style='width:100%'></table>" +
                    chartScript;

                this.Controls.Add(formatedScript);
            }
            catch (Exception ex)
            {
                LiteralControl errorMessage = new LiteralControl();
                errorMessage.Text = "ERROR >> " + ex.Message;

                this.Controls.Clear();
                this.Controls.Add(errorMessage);
            }
        }

        private List<string> GetFileContent(string theFile)
        {
            List<string> fileContent = new List<string>();

            #region Read the file
            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                System.IO.StreamReader file = null;
                string line;

                try
                {
                    file = new System.IO.StreamReader(theFile, System.Text.Encoding.GetEncoding("iso-8859-1"));
                    while ((line = file.ReadLine()) != null)
                    {
                        if (!string.IsNullOrWhiteSpace(line))
                            fileContent.Add(line);
                    }
                }
                finally { file.Close(); }
            });
            #endregion

            return fileContent;
        }

        private string GetChartScript(List<string> fileContent)
        {
            string theScript = "";

            #region Chart and Table format
            string cbb_points = "", lpz_points = "", scz_points = "", rst_points = "";
            string cbb_promos = "", lpz_promos = "", scz_promos = "", rst_promos = "";
            string cbb_table = "", lpz_table = "", scz_table = "", rst_table = "";
            int cbb_position = 1, lpz_position = 1, scz_position = 1, rst_position = 1;

            foreach (string item in fileContent)
	        {
                switch (item.Split('|')[4].ToUpper().Trim())
                {
                    case "COCHABAMBA":
                        if (cbb_position <= 10)
                        {
                            cbb_points += item.Split('|')[1] + ",";
                            cbb_promos += "'" + item.Split('|')[0] + "',";
                        }
                        cbb_table += string.Format(
                            "<tr><td>{0}</td>" +
                            "<td>{1}</td>" +
                            "<td>{2}</td>" +
                            "<td>{3}</td>" +
                            "<td>{4}</td>" +
                            "<td>{5}</td></tr>",
                            cbb_position, item.Split('|')[0], item.Split('|')[3], item.Split('|')[2], item.Split('|')[1], item.Split('|')[4]);
                        cbb_position++;
                        break;
                    case "LA PAZ":
                        if (lpz_position <= 10)
                        {
                            lpz_points += item.Split('|')[1] + ",";
                            lpz_promos += "'" + item.Split('|')[0] + "',";
                        }
                        lpz_table += string.Format(
                            "<tr><td>{0}</td>" +
                            "<td>{1}</td>" +
                            "<td>{2}</td>" +
                            "<td>{3}</td>" +
                            "<td>{4}</td>" +
                            "<td>{5}</td></tr>",
                            lpz_position, item.Split('|')[0], item.Split('|')[3], item.Split('|')[2], item.Split('|')[1], item.Split('|')[4]);
                        lpz_position++;
                        break;
                    case "SANTA CRUZ":
                        if (scz_position <= 10)
                        {
                            scz_points += item.Split('|')[1] + ",";
                            scz_promos += "'" + item.Split('|')[0] + "',";
                        }
                        scz_table += string.Format(
                            "<tr><td>{0}</td>" +
                            "<td>{1}</td>" +
                            "<td>{2}</td>" +
                            "<td>{3}</td>" +
                            "<td>{4}</td>" +
                            "<td>{5}</td></tr>",
                            scz_position, item.Split('|')[0], item.Split('|')[3], item.Split('|')[2], item.Split('|')[1], item.Split('|')[4]);
                        scz_position++;
                        break;
                    default:
                        if (rst_position <= 10)
                        {
                            rst_points += item.Split('|')[1] + ",";
                            rst_promos += "'" + item.Split('|')[0] + "',";
                        }
                        rst_table += string.Format(
                            "<tr><td>{0}</td>" +
                            "<td>{1}</td>" +
                            "<td>{2}</td>" +
                            "<td>{3}</td>" +
                            "<td>{4}</td>" +
                            "<td>{5}</td></tr>",
                            rst_position, item.Split('|')[0], item.Split('|')[3], item.Split('|')[2], item.Split('|')[1], item.Split('|')[4]);
                        rst_position++;
                        break;
                }
		    }
            if (cbb_points != "")
            {
                cbb_points = cbb_points.Remove(cbb_points.LastIndexOf(","));
                cbb_promos = cbb_promos.Remove(cbb_promos.LastIndexOf(","));
            }
            if (lpz_points != "")
            {
                lpz_points = lpz_points.Remove(lpz_points.LastIndexOf(","));
                lpz_promos = lpz_promos.Remove(lpz_promos.LastIndexOf(","));
            }
            if (scz_points != "")
            {
                scz_points = scz_points.Remove(scz_points.LastIndexOf(","));
                scz_promos = scz_promos.Remove(scz_promos.LastIndexOf(","));
            }
            if (rst_points != "")
            {
                rst_points = rst_points.Remove(rst_points.LastIndexOf(","));
                rst_promos = rst_promos.Remove(rst_promos.LastIndexOf(","));
            }
            #endregion

            theScript = 
                "<script src='/_catalogs/masterpage/banx/js/chart.min.js'></script>" +
                "<script src='/_catalogs/masterpage/banx/form/classie.js'></script>" +
                "<script>" +
                "var filterCtrls = [].slice.call(document.querySelectorAll('.filter-bar > button'));" +
                "function InitEvents() {" +
                "filterCtrls.forEach(function(filterCtrl) {" +
                "filterCtrl.addEventListener('click', function() {" +
                "try {" +
                "classie.remove(filterCtrl.parentNode.querySelector('.filter__item--selected'), 'filter__item--selected');" +
                "} catch(err) {}" +
                "classie.add(filterCtrl, 'filter__item--selected');" +
                "var city = filterCtrl.getAttribute('data-filter');" +
                "GetRanking(city);" +
                "});" +
                "});" +
                "} InitEvents();" +
                "function GetRanking(city) {" +
                "var promoPointsCBB = [" + cbb_points + "];" +
                "var promoNamesCBB = [" + cbb_promos + "];" +
                "var promoPointsLPZ = [" + lpz_points + "];" +
                "var promoNamesLPZ = [" + lpz_promos + "];" +
                "var promoPointsSCZ = [" + scz_points + "];" +
                "var promoNamesSCZ = [" + scz_promos + "];" +
                "var promoPointsRST = [" + rst_points + "];" +
                "var promoNamesRST = [" + rst_promos + "];" +
                "var promoNames; var promoPoints; var promoRows; var ctx;" +
                "var rankTable = document.getElementById('rankTable');" +
                "rankTable.innerHTML = '';" +
                "if (city == 'COCHABAMBA') {" +
                "promoNames = promoNamesCBB;" +
                "promoPoints = promoPointsCBB;" +
                "promoRows = '" + cbb_table + "';" +
                "$('#theChartCBB').show();" +
                "$('#theChartLPZ').hide();" +
                "$('#theChartSCZ').hide();" +
                "$('#theChartRST').hide();" +
                "ctx = document.getElementById('theChartCBB').getContext('2d'); }" +
                "else if (city == 'LA PAZ') {" +
                "promoNames = promoNamesLPZ;" +
                "promoPoints = promoPointsLPZ;" +
                "promoRows = '" + lpz_table + "';" +
                "$('#theChartCBB').hide();" +
                "$('#theChartLPZ').show();" +
                "$('#theChartSCZ').hide();" +
                "$('#theChartRST').hide();" +
                "ctx = document.getElementById('theChartLPZ').getContext('2d'); }" +
                "else if (city == 'SANTA CRUZ') {" +
                "promoNames = promoNamesSCZ;" +
                "promoPoints = promoPointsSCZ;" +
                "promoRows = '" + scz_table + "';" +
                "$('#theChartCBB').hide();" +
                "$('#theChartLPZ').hide();" +
                "$('#theChartSCZ').show();" +
                "$('#theChartRST').hide();" +
                "ctx = document.getElementById('theChartSCZ').getContext('2d'); }" +
                "else {" +
                "promoNames = promoNamesRST;" +
                "promoPoints = promoPointsRST;" +
                "promoRows = '" + rst_table + "';" +
                "$('#theChartCBB').hide();" +
                "$('#theChartLPZ').hide();" +
                "$('#theChartSCZ').hide();" +
                "$('#theChartRST').show();" +
                "ctx = document.getElementById('theChartRST').getContext('2d'); }" +
                "if (promoRows == '') { $('#result').show(); return; }" +
                "else { $('#result').hide(); }" +
                "var barChartData = {" +
                "labels: promoNames," +
                "datasets: [{" +
                "fillColor : 'rgba(38,101,135,0.7)', strokeColor : 'rgba(17,35,46,0.9)', highlightFill: 'rgba(38,101,135,1)', highlightStroke: 'rgba(17,35,46,0.9)', data: promoPoints" +
                "}]};" +
                "var myBarChart = new Chart(ctx).Bar(barChartData, {" +
                "responsive: true, maintainAspectRatio: true, scaleShowGridLines: true, scaleShowVerticalLines: false, barShowStroke: false, scaleFontFamily: 'planerregular', animationSteps: 90, scaleFontSize: 16, tooltipFillColor: 'rgba(17,35,46,1)', tooltipFontFamily: 'planerregular'" +
                "});" +
                "try {" +
                "myBarChart.datasets[0].bars[0].fillColor = 'rgba(59,37,19,1)';" +
                "myBarChart.datasets[0].bars[1].fillColor = 'rgba(74,46,24,1)';" +
                "myBarChart.datasets[0].bars[2].fillColor = 'rgba(89,55,29,1)';" +
                "myBarChart.datasets[0].bars[3].fillColor = 'rgba(104,64,34,1)';" +
                "myBarChart.datasets[0].bars[4].fillColor = 'rgba(118,74,38,1)';" +
                "myBarChart.datasets[0].bars[5].fillColor = 'rgba(133,83,43,1)';" +
                "myBarChart.datasets[0].bars[6].fillColor = 'rgba(148,92,48,1)';" +
                "myBarChart.datasets[0].bars[7].fillColor = 'rgba(163,101,53,1)';" +
                "myBarChart.datasets[0].bars[8].fillColor = 'rgba(178,110,58,1)';" +
                "myBarChart.datasets[0].bars[9].fillColor = 'rgba(192,120,62,1)';" +
                "} catch (err) {}" +
                "try {" +
                "myBarChart.datasets[0].bars[0].highlightFill = 'rgba(19,59,57,1)';" +
                "myBarChart.datasets[0].bars[1].highlightFill = 'rgba(24,74,71,1)';" +
                "myBarChart.datasets[0].bars[2].highlightFill = 'rgba(29,89,85,1)';" +
                "myBarChart.datasets[0].bars[3].highlightFill = 'rgba(34,104,99,1)';" +
                "myBarChart.datasets[0].bars[4].highlightFill = 'rgba(38,118,114,1)';" +
                "myBarChart.datasets[0].bars[5].highlightFill = 'rgba(43,133,128,1)';" +
                "myBarChart.datasets[0].bars[6].highlightFill = 'rgba(48,148,142,1)';" +
                "myBarChart.datasets[0].bars[7].highlightFill = 'rgba(53,163,156,1)';" +
                "myBarChart.datasets[0].bars[8].highlightFill = 'rgba(58,178,170,1)';" +
                "myBarChart.datasets[0].bars[9].highlightFill = 'rgba(62,192,185,1)';" +
                "} catch (err) {}" +
                "myBarChart.update();" +
                "rankTable.innerHTML = \"<tr style='font-weight:bold'><td>Posición</td><td>Promoción</td><td>Colegio</td><td>Turno</td><td>Puntos</td><td>Departamento</td></tr>\" + promoRows; }" +
                "</script>";

            return theScript;
        }
    }
}
