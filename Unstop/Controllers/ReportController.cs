using BoldReports.RDL.DOM;
using BoldReports.Web.ReportViewer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration.UserSecrets;
using Newtonsoft.Json;
using Npgsql;
using SkiaSharp;
using Syncfusion.DocIO.DLS;
using System.Data;
using Unstop.Models;
using Unstop.Models.DTO;
using Unstop.Services;
using Unstop.Services.IServices;
using Unstop_Utility;

namespace Unstop.Controllers
{
    [Route("[controller]/[action]")]
    public class ReportController : Controller, IReportController
    {
        private IMemoryCache _memoryCache;
        private readonly IJobService _jobService;
        private readonly IApplicationService _applicationService;
        private readonly string _connectionString;
        private List<Dictionary<string, string>> offerLetterParams = new();
        private SampleData customDatas = new();

        private class SampleData
        {
            public int month { get; set; }
        }

        public ReportController(IMemoryCache memoryCache, IJobService jobService, IApplicationService applicationService, IConfiguration configuration)
        {
            _memoryCache = memoryCache;
            _jobService = jobService;
            _applicationService = applicationService;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        [ActionName("GetResource")]
        [AcceptVerbs("GET")]
        public object GetResource(ReportResource resource)
        {
            return ReportHelper.GetResource(resource, this, _memoryCache);
        }

        [NonAction]
        public void OnInitReportOptions(ReportViewerOptions reportOption)
        {
            reportOption.ReportModel.ProcessingMode = ProcessingMode.Local;

            string basePath = Directory.GetParent(Directory.GetCurrentDirectory()).FullName;
            using (FileStream fileStream = new(basePath + @"\Unstop Utility\" + reportOption.ReportModel.ReportPath, FileMode.Open, FileAccess.Read))
            {
                MemoryStream reportStream = new();
                fileStream.CopyTo(reportStream);
                reportStream.Position = 0;
                reportOption.ReportModel.Stream = reportStream;
            }

            int userId = Convert.ToInt32(HttpContext.Session.GetString(StaticDetails.SessionId));

            if (offerLetterParams != null)
            {
                DataTable offerTable = new() { };

                offerTable.Columns.Add("CompanyName");
                offerTable.Columns.Add("CandidateName");
                offerTable.Columns.Add("SenderName");
                offerTable.Columns.Add("JobTitle");
                offerTable.Columns.Add("JobType");
                offerTable.Columns.Add("JobLocation");
                offerTable.Columns.Add("JoiningDate");
                offerTable.Columns.Add("Salary");

                System.Data.DataRow offerRow;
                offerRow = offerTable.NewRow();

                foreach (var offer in offerLetterParams)
                {
                    if (offer.TryGetValue("name", out var name) && offer.TryGetValue("value", out var value))
                    {
                        offerRow[name] = value;
                    }
                }
                offerTable.Rows.Add(offerRow);

                reportOption.ReportModel.DataSources.Add(new BoldReports.Web.ReportDataSource { Name = "Offer", Value = offerTable });
            }


            reportOption.ReportModel.DataSources.Add(new BoldReports.Web.ReportDataSource { Name = "Jobs", Value = GetJobsAsync(userId) });
            reportOption.ReportModel.DataSources.Add(new BoldReports.Web.ReportDataSource { Name = "Applications", Value = GetApplicationsAsync(userId) });

        }

        private List<JobDTO> GetJobsAsync(int userId)
        {
            List<JobDTO> jobs = new();

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = new NpgsqlCommand(StaticDetails.AllJobsSP, connection))
                {
                    command.Parameters.AddWithValue("@user_id", userId);
                    using var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        var job = new JobDTO
                        {
                            JobId = Convert.ToInt32(reader["JobId"]),
                            Title = reader["Title"].ToString()
                        };
                        jobs.Add(job);
                    }
                }
                connection.Close();
            }

            return jobs;
        }

        private List<ApplicationDTO> GetApplicationsAsync(int userId)
        {
            List<ApplicationDTO> applications = new();

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = new NpgsqlCommand(StaticDetails.MonthWiseApplicationsSP, connection))
                {
                    command.Parameters.AddWithValue("@user_id", userId);
                    command.Parameters.AddWithValue("@month_value", customDatas.month);
                    using var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        var application = new ApplicationDTO
                        {
                            ApplicationId = Convert.ToInt32(reader["ApplicationId"]),
                            JobId = Convert.ToInt32(reader["JobId"]),
                            AppliedDate = (DateTime)reader["AppliedDate"]
                        };
                        applications.Add(application);
                    }
                }
                connection.Close();
            }

            return applications;
        }

        [NonAction]
        public void OnReportLoaded(ReportViewerOptions reportOption)
        {

        }

        [HttpPost]
        public object PostFormReportAction()
        {
            return ReportHelper.ProcessReport(null, this, _memoryCache);
        }

        [HttpPost]
        public object PostReportAction([FromBody] Dictionary<string, object> jsonResult)
        {
            if (jsonResult.ContainsKey("parameters") && jsonResult["parameters"] != null)
            {
                var parametersJson = jsonResult["parameters"].ToString();
                offerLetterParams = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(parametersJson);
            }

            if (jsonResult.ContainsKey("customData"))
            {
                customDatas = JsonConvert.DeserializeObject<SampleData>(jsonResult["customData"].ToString());
            }

            return ReportHelper.ProcessReport(jsonResult, this, _memoryCache);
        }
    }
}
