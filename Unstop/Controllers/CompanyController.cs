using Azure;
using BoldReports.Writer;
using IronQr;
using IronSoftware.Drawing;
using iText.Layout;
using iText.StyledXmlParser.Jsoup.Select;
using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using iTextSharp.tool.xml.html;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Kendo.Mvc.UI.Fluent;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ML.OnnxRuntime;
using Newtonsoft.Json;
using Rotativa.AspNetCore;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.Reflection;
using System.Text;
using Unstop.Models;
using Unstop.Models.DTO;
using Unstop.Models.VM;
using Unstop.Services.IServices;
using Unstop_Utility;
using static iTextSharp.text.pdf.AcroFields;
using static Unstop.Models.VM.QRCodeVM;
using Document = iTextSharp.text.Document;
using PdfWriter = iTextSharp.text.pdf.PdfWriter;

namespace Unstop.Controllers
{
    [Authorize(Roles = "Company")]
    public class CompanyController : Controller
    {
        private readonly IJobService _jobService;
        private readonly IProfileService _profileService;
        private readonly IApplicationService _applicationService;
        private readonly IEmailService _emailService;
        private readonly IInterviewService _interviewService;
        private readonly IJobFairService _jobFairService;
        private readonly ITemplateService _templateService;

        public CompanyController(IJobService jobService, IProfileService profileService, IApplicationService applicationService, IEmailService emailService, IInterviewService interviewService, IJobFairService jobFairService, ITemplateService templateService)
        {
            _jobService = jobService;
            _profileService = profileService;
            _applicationService = applicationService;
            _emailService = emailService;
            _interviewService = interviewService;
            _jobFairService = jobFairService;
            _templateService = templateService;
        }


        #region Dashboard 

        [HttpGet]
        public IActionResult Dashboard()
        {
            //QrCode jobQr = QrWriter.Write("https://pidpen.com/");
            //string qrPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "QRs", "PiDPeN.png");
            //using (AnyBitmap qrBitmap = jobQr.Save())
            //{
            //    qrBitmap.SaveAs(qrPath);
            //}

            //string qrPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "QRs", "PiDPeN.png");
            //var inputBmp = AnyBitmap.FromFile(qrPath);

            //QrImageInput imageInput = new(inputBmp);
            //QrReader reader = new();

            //IEnumerable<QrResult> results = reader.Read(imageInput);
            //foreach (QrResult qrResult in results)
            //{
            //    TempData["success"] = qrResult.Value;
            //}

            return View();
        }

        #endregion


        #region Get Job

        //[HttpGet]
        public async Task<IActionResult> Jobs(string search, string jobType, string jobTiming, int workingDays, bool clear, string sortBy, string sortOrder, string jobStatus, bool isAjax, int pageNumber = 1, int pageSize = 2)
        {
            int userId = Convert.ToInt32(HttpContext.Session.GetString(StaticDetails.SessionId));
            string userRole = HttpContext.Session.GetString(StaticDetails.SessionRole);

            if (clear)
            {
                jobType = null;
                jobTiming = null;
                workingDays = 0;
            }

            List<JobDTO> jobs = new();

            APIResponse response = await _jobService.GetAllAsync<APIResponse>(token: HttpContext.Session.GetString(StaticDetails.SessionToken), userRole: userRole, userId: userId, search: search, jobType: jobType, jobTime: jobTiming, workingDays: workingDays, sortBy: sortBy, sortOrder: sortOrder, jobStatus: jobStatus, pageNumber: pageNumber, pageSize: pageSize);

            if (response != null && response.IsSuccess)
            {
                jobs = JsonConvert.DeserializeObject<List<JobDTO>>(Convert.ToString(response.Result));
            }

            JobsVM model = new()
            {
                Jobs = jobs,
                Pagination = response.PaginationData
            };

            ViewBag.Search = search;
            ViewBag.SortBy = sortBy;
            ViewBag.SortOrder = sortOrder;
            ViewBag.JobType = jobType;
            ViewBag.JobTime = jobTiming;
            ViewBag.WorkingDays = workingDays;
            ViewBag.JobStatus = jobStatus;
            ViewBag.PageSize = pageSize;

            if (isAjax)
            {
                return PartialView("_JobsList", model);
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> JobInformation(int jobId)
        {
            APIResponse response = await _jobService.GetAsync<APIResponse>(token: HttpContext.Session.GetString(StaticDetails.SessionToken), jobId);

            if (response != null && response.IsSuccess)
            {
                JobDTO model = JsonConvert.DeserializeObject<JobDTO>(Convert.ToString(response.Result));

                return View(model);
            }

            TempData["error"] = response.ErrorMessages;
            return View("Jobs");
        }

        #endregion


        #region Post Job

        [HttpGet]
        public async Task<IActionResult> PostJob()
        {
            int userId = Convert.ToInt32(HttpContext.Session.GetString(StaticDetails.SessionId));

            APIResponse companyResponse = await _profileService.GetCompanyAsync<APIResponse>(userId);

            JobDTO model = new()
            {
                UserId = userId,
            };

            if (companyResponse.Result != null && companyResponse.IsSuccess)
            {
                CompanyDTO company = JsonConvert.DeserializeObject<CompanyDTO>(Convert.ToString(companyResponse.Result));

                model.Company = company.CompanyName;
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> PostJob(JobDTO model)
        {
            DateTime unspecifiedDate = DateTime.ParseExact(model.LastDate.ToString(), "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture);
            DateTime utcDate = DateTime.SpecifyKind(unspecifiedDate, DateTimeKind.Utc);
            model.LastDate = utcDate;

            APIResponse response = await _jobService.CrateAsync<APIResponse>(token: HttpContext.Session.GetString(StaticDetails.SessionToken), model);

            if (response != null && response.IsSuccess && response.Result != null)
            {
                JobDTO job = JsonConvert.DeserializeObject<JobDTO>(Convert.ToString(response.Result));

                APIResponse candidateResponse = await _profileService.GetAllCandidateAsync<APIResponse>(token: HttpContext.Session.GetString(StaticDetails.SessionToken));

                if (candidateResponse != null && candidateResponse.IsSuccess)
                {
                    List<CandidateDTO> userList = JsonConvert.DeserializeObject<List<CandidateDTO>>(Convert.ToString(candidateResponse.Result));

                    string imageUrl = "https://images.unsplash.com/photo-1549637642-90187f64f420?q=80&w=2074&auto=format&fit=crop&ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D";
                    string anchorUrl = $"https://localhost:7002/Candidate/JobInformation?jobId={job.JobId}";

                    string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "email", "JobPost.html");
                    string emailBody = await System.IO.File.ReadAllTextAsync(filePath);

                    emailBody = emailBody.Replace("[Job Title]", job.Title)
                                .Replace("[Company Name]", job.Company)
                                .Replace("[image]", imageUrl)
                                .Replace("[url]", anchorUrl);

                    for (int i = 0; i < userList.Count; i++)
                    {
                        emailBody = emailBody.Replace("[Candidate's First Name]", userList[i].FullName);

                        EmailRequestModel emailRequest = new()
                        {
                            To = userList[i].User.Email,
                            Subject = $"{job.Company} is Hiring!",
                            IsBodyHtml = true,
                            Body = emailBody
                        };

                        await _emailService.SendEmailAsync<APIResponse>(emailRequest);
                    }
                }

                TempData["success"] = "Job Posted";
                return RedirectToAction("Dashboard", "Company");
            }

            TempData["error"] = response.ErrorMessages;
            return View();
        }

        #endregion


        #region Edit Job

        [HttpGet]
        public async Task<IActionResult> EditJob(int jobId)
        {
            APIResponse response = await _jobService.GetAsync<APIResponse>(token: HttpContext.Session.GetString(StaticDetails.SessionToken), jobId);

            if (response != null && response.IsSuccess)
            {
                JobDTO model = JsonConvert.DeserializeObject<JobDTO>(Convert.ToString(response.Result));

                return View(model);
            }

            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> EditJob(JobDTO model)
        {
            DateTime unspecifiedDate = DateTime.ParseExact(model.LastDate.ToString(), "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture);
            DateTime utcDate = DateTime.SpecifyKind(unspecifiedDate, DateTimeKind.Utc);
            model.LastDate = utcDate;

            APIResponse response = await _jobService.UpdateAsync<APIResponse>(token: HttpContext.Session.GetString(StaticDetails.SessionToken), model);

            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Job Edited";
                return RedirectToAction("Jobs", "Company");
            }

            TempData["error"] = response.ErrorMessages.FirstOrDefault();
            return View(model.JobId);
        }

        #endregion


        #region Delete Job

        [HttpGet]
        public IActionResult DeleteJob(int jobId)
        {
            JobDTO model = new()
            {
                JobId = jobId,
            };

            return PartialView("_ConfirmationModal", model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteJob(JobDTO model)
        {
            APIResponse response = await _jobService.DeleteAsync<APIResponse>(token: HttpContext.Session.GetString(StaticDetails.SessionToken), model.JobId);

            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Job Deleted";
                return RedirectToAction("Jobs", "Company");
            }

            TempData["error"] = response.ErrorMessages.FirstOrDefault();
            return RedirectToAction("Jobs", "Company");
        }

        #endregion


        #region Profile

        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            int userId = Convert.ToInt32(HttpContext.Session.GetString(StaticDetails.SessionId));

            APIResponse response = await _profileService.GetCompanyAsync<APIResponse>(userId);

            if (response != null && response.IsSuccess)
            {
                CompanyDTO model = JsonConvert.DeserializeObject<CompanyDTO>(Convert.ToString(response.Result));

                return View(model);
            }

            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Profile(CompanyDTO model)
        {
            if (ModelState.IsValid)
            {
                if (model.LogoFile != null && model.LogoFile.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await model.LogoFile.CopyToAsync(memoryStream);
                        model.Logo = memoryStream.ToArray();
                        model.LogoFileName = model.LogoFile.FileName;
                        model.LogoContentType = model.LogoFile.ContentType;
                    }
                }

                APIResponse response = await _profileService.UpdateCompanyAsync<APIResponse>(token: HttpContext.Session.GetString(StaticDetails.SessionToken), model);

                if (response != null && response.IsSuccess)
                {
                    CompanyDTO company = JsonConvert.DeserializeObject<CompanyDTO>(Convert.ToString(response.Result));

                    IronQr.QrCode jobQr = QrWriter.Write($"{company.UserId}");
                    string qrPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "QRs", $"{company.CompanyName}-Details.png");
                    using (AnyBitmap qrBitmap = jobQr.Save())
                    {
                        qrBitmap.SaveAs(qrPath);
                    }

                    TempData["success"] = "Profile Updated";
                    return RedirectToAction("Dashboard", "Company");
                }
            }

            TempData["error"] = "Unexcepted file format";
            return RedirectToAction("Profile", "Company");
        }

        #endregion


        #region Applications

        //[HttpGet]
        public async Task<IActionResult> Applicants(int jobId, string search, List<string> status, bool clear, string sortOrder, bool isAjax, int pageNumber = 1, int pageSize = 2)
        {
            int userId = Convert.ToInt32(HttpContext.Session.GetString(StaticDetails.SessionId));
            string userRole = HttpContext.Session.GetString(StaticDetails.SessionRole);

            if (clear)
            {
                status = null;
            }

            List<ApplicationDTO> applicants = new();
            JobDTO job = new();

            APIResponse applicationResponse = await _applicationService.GetAllAsync<APIResponse>(token: HttpContext.Session.GetString(StaticDetails.SessionToken), userRole: userRole, userId: userId, jobId: jobId, searchApplicants: search, searchApplication: "", status: status, sortOrder: sortOrder, pageNumber: pageNumber, pageSize: pageSize);

            if (applicationResponse != null && applicationResponse.IsSuccess)
            {
                applicants = JsonConvert.DeserializeObject<List<ApplicationDTO>>(Convert.ToString(applicationResponse.Result));
            }

            APIResponse jobResponse = await _jobService.GetAsync<APIResponse>(token: HttpContext.Session.GetString(StaticDetails.SessionToken), jobId);

            if (jobResponse != null && jobResponse.IsSuccess)
            {
                job = JsonConvert.DeserializeObject<JobDTO>(Convert.ToString(jobResponse.Result));
            }

            ApplicantsVM model = new()
            {
                Job = job,
                Applicants = applicants,
                Pagination = applicationResponse.PaginationData
            };

            ViewBag.Search = search;
            ViewBag.Status = status;
            ViewBag.SortOrder = sortOrder;
            ViewBag.PageSize = pageSize;

            if (isAjax)
            {
                return PartialView("_ApplicantsList", model);
            }

            return View(model);
        }

        #endregion


        #region Review Application

        [HttpGet]
        public IActionResult ReviewApplicationModal(int applicationId, int jobId)
        {
            ApplicationDTO model = new()
            {
                ApplicationId = applicationId,
                JobId = jobId
            };

            return PartialView("_ReviewModal", model);
        }

        public async Task<IActionResult> ReviewApplication(int applicationId, int jobId)
        {
            APIResponse response = await _applicationService.GetAsync<APIResponse>(token: HttpContext.Session.GetString(StaticDetails.SessionToken), applicationId);

            if (response != null && response.IsSuccess)
            {
                ApplicationDTO model = JsonConvert.DeserializeObject<ApplicationDTO>(Convert.ToString(response.Result));

                model.Status = "Reviewed";

                APIResponse updateResponse = await _applicationService.UpdateAsync<APIResponse>(token: HttpContext.Session.GetString(StaticDetails.SessionToken), model);

                if (updateResponse != null && updateResponse.IsSuccess)
                {
                    TempData["success"] = "Application Reviewed";
                    return RedirectToAction("Applicants", "Company", new { jobId = jobId });
                }

            }

            TempData["error"] = response.ErrorMessages.FirstOrDefault();
            return RedirectToAction("Applicants", "Company", new { jobId = jobId });
        }

        #endregion


        #region Schedule Interview

        [HttpGet]
        public IActionResult ScheduleInterviewModal(int applicationId, int jobId, string jobTitle, string companyName, string candidateName)
        {
            InterviewDTO model = new()
            {
                ApplicationId = applicationId,
                JobId = jobId,
                InterviewTitle = jobTitle + " @ " + companyName + " - " + candidateName,
            };

            return PartialView("_ScheduleInterviewModal", model);
        }

        [HttpPost]
        public async Task<IActionResult> ScheduleInterview(InterviewDTO interviewDTO)
        {
            DateTime unspecifiedDate = DateTime.ParseExact(interviewDTO.InterviewDate.ToString(), "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture);
            DateTime utcDate = DateTime.SpecifyKind(unspecifiedDate, DateTimeKind.Utc);
            interviewDTO.InterviewDate = utcDate;

            APIResponse interviewResponse = await _interviewService.CreateAsync<APIResponse>(token: HttpContext.Session.GetString(StaticDetails.SessionToken), interviewDTO);

            if (interviewResponse != null && interviewResponse.IsSuccess)
            {
                InterviewDTO interview = JsonConvert.DeserializeObject<InterviewDTO>(Convert.ToString(interviewResponse.Result));

                APIResponse applicationResponse = await _applicationService.GetAsync<APIResponse>(token: HttpContext.Session.GetString(StaticDetails.SessionToken), interview.ApplicationId);

                if (applicationResponse != null && applicationResponse.IsSuccess)
                {
                    ApplicationDTO application = JsonConvert.DeserializeObject<ApplicationDTO>(Convert.ToString(applicationResponse.Result));

                    APIResponse candidateResponse = await _profileService.GetCandidateAsync<APIResponse>(token: HttpContext.Session.GetString(StaticDetails.SessionToken), application.CandidateId);

                    if (candidateResponse != null && candidateResponse.IsSuccess)
                    {
                        CandidateDTO candidate = JsonConvert.DeserializeObject<CandidateDTO>(Convert.ToString(candidateResponse.Result));

                        DateTime interviewTime = DateTime.Today.Add(interview.StartTime);

                        string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "email", "ScheduleInterview.html");
                        string emailBody = await System.IO.File.ReadAllTextAsync(filePath);

                        emailBody = emailBody.Replace("[Candidate's First Name]", candidate.User.FirstName)
                                    .Replace("[Job Title]", application.Job.Title)
                                    .Replace("[Interview Date]", interview.InterviewDate.ToString("dd MMM, yyyy"))
                                    .Replace("[Interview Time]", interviewTime.ToString("hh:mm tt"))
                                    .Replace("[Interview Location]", interview.Location);

                        EmailRequestModel emailRequest = new()
                        {
                            To = candidate.User.Email,
                            Subject = $"Interview Scheduled - {application.Job.Company}",
                            IsBodyHtml = true,
                            Body = emailBody
                        };

                        await _emailService.SendEmailAsync<APIResponse>(emailRequest);

                        APIResponse getResponse = await _applicationService.GetAsync<APIResponse>(token: HttpContext.Session.GetString(StaticDetails.SessionToken), interviewDTO.ApplicationId);

                        if (getResponse != null && getResponse.IsSuccess)
                        {
                            ApplicationDTO model = JsonConvert.DeserializeObject<ApplicationDTO>(Convert.ToString(getResponse.Result));

                            model.Status = "Interview Scheduled";

                            APIResponse updateResponse = await _applicationService.UpdateAsync<APIResponse>(token: HttpContext.Session.GetString(StaticDetails.SessionToken), model);

                            if (updateResponse != null && updateResponse.IsSuccess)
                            {
                                //TempData["success"] = "Interview Scheduled";
                                //return RedirectToAction("Applicants", "Company", new { jobId = interviewDTO.JobId });
                                return Ok(new { jobId = interviewDTO.JobId, success = true });

                            }
                        }
                    }
                }
            }

            //TempData["error"] = interviewResponse.ErrorMessages.FirstOrDefault();
            //return RedirectToAction("Applicants", "Company", new { jobId = interviewDTO.JobId });
            return Ok(new { jobId = interviewResponse, success = false, error = interviewResponse.ErrorMessages.FirstOrDefault() });

        }

        #endregion


        #region Interview Done

        [HttpGet]
        public IActionResult InterviewedModal(int applicationId, int jobId)
        {
            ApplicationDTO model = new()
            {
                ApplicationId = applicationId,
                JobId = jobId
            };

            return PartialView("_InterviewedModal", model);
        }

        public async Task<IActionResult> InterviewDone(int applicationId, int jobId)
        {
            APIResponse getResponse = await _applicationService.GetAsync<APIResponse>(token: HttpContext.Session.GetString(StaticDetails.SessionToken), applicationId);

            if (getResponse != null && getResponse.IsSuccess)
            {
                ApplicationDTO model = JsonConvert.DeserializeObject<ApplicationDTO>(Convert.ToString(getResponse.Result));

                model.Status = "Interviewed";

                APIResponse updateResponse = await _applicationService.UpdateAsync<APIResponse>(token: HttpContext.Session.GetString(StaticDetails.SessionToken), model);

                if (updateResponse != null && updateResponse.IsSuccess)
                {
                    APIResponse getInterviewResponse = await _interviewService.GetAsync<APIResponse>(token: HttpContext.Session.GetString(StaticDetails.SessionToken), applicationId);

                    if (getInterviewResponse != null && getInterviewResponse.IsSuccess)
                    {
                        InterviewDTO interview = JsonConvert.DeserializeObject<InterviewDTO>(Convert.ToString(getInterviewResponse.Result));

                        interview.Complete = true;

                        APIResponse response = await _interviewService.UpdateAsync<APIResponse>(token: HttpContext.Session.GetString(StaticDetails.SessionToken), interview);
                    }

                    TempData["success"] = "Interview Completed";
                    return RedirectToAction("Applicants", "Company", new { jobId = jobId });
                }
            }

            TempData["error"] = getResponse.ErrorMessages.FirstOrDefault();
            return RedirectToAction("Applicants", "Company", new { jobId = jobId });
        }

        #endregion


        #region Offer Job

        [HttpGet]
        public async Task<IActionResult> OfferJobModal(int applicationId)
        {
            OfferDTO model = new();

            APIResponse applicationResponse = await _applicationService.GetAsync<APIResponse>(token: HttpContext.Session.GetString(StaticDetails.SessionToken), applicationId);

            if (applicationResponse != null && applicationResponse.IsSuccess)
            {
                ApplicationDTO application = JsonConvert.DeserializeObject<ApplicationDTO>(Convert.ToString(applicationResponse.Result));

                model.ApplicationId = applicationId;
                model.JobId = application.JobId;
                model.CandidateId = application.CandidateId;

                int userId = Convert.ToInt32(HttpContext.Session.GetString(StaticDetails.SessionId));

                APIResponse userResponse = await _profileService.GetUserAsync<APIResponse>(token: HttpContext.Session.GetString(StaticDetails.SessionToken), userId);

                if (userResponse != null && userResponse.IsSuccess)
                {
                    UserDTO user = JsonConvert.DeserializeObject<UserDTO>(Convert.ToString(userResponse.Result));

                    model.SenderName = user.FirstName + " " + user.LastName;
                }

                APIResponse companyResponse = await _profileService.GetCompanyAsync<APIResponse>(userId);

                if (companyResponse != null && companyResponse.IsSuccess)
                {
                    CompanyDTO company = JsonConvert.DeserializeObject<CompanyDTO>(Convert.ToString(companyResponse.Result));

                    model.CompanyName = company.CompanyName;
                }

                APIResponse candidateResponse = await _profileService.GetAllCandidateAsync<APIResponse>(token: HttpContext.Session.GetString(StaticDetails.SessionToken));

                if (candidateResponse != null && candidateResponse.IsSuccess)
                {
                    List<CandidateDTO> candidateList = JsonConvert.DeserializeObject<List<CandidateDTO>>(Convert.ToString(candidateResponse.Result));

                    CandidateDTO candidate = candidateList.Where(x => x.CandidateId == application.CandidateId).FirstOrDefault();

                    model.CandidateName = candidate.FullName;
                }

                APIResponse jobResponse = await _jobService.GetAsync<APIResponse>(token: HttpContext.Session.GetString(StaticDetails.SessionToken), application.JobId);

                if (jobResponse != null && jobResponse.IsSuccess)
                {
                    JobDTO job = JsonConvert.DeserializeObject<JobDTO>(Convert.ToString(jobResponse.Result));

                    model.JobTitle = job.Title;
                    model.JobType = job.JobType;
                    model.JobLocation = job.Location;
                }
            }

            return PartialView("_OfferJobModal", model);
        }

        public IActionResult PreviewOfferLetter(OfferDTO model)
        {
            return PartialView("_PreviewOfferLetter", model);
        }

        public IActionResult GenerateOfferLetter(OfferDTO model)
        {
            FileStream inputStream = new(Directory.GetParent(Directory.GetCurrentDirectory()).FullName + @"\Unstop Utility\OfferLetterReport.rdlc", FileMode.Open, FileAccess.Read);
            MemoryStream reportStream = new();
            inputStream.CopyTo(reportStream);
            reportStream.Position = 0;
            inputStream.Close();
            ReportWriter writer = new();
            writer.ReportProcessingMode = ProcessingMode.Local;

            DataTable offerTable = new() { };

            offerTable.Columns.Add("CompanyName");
            offerTable.Columns.Add("CandidateName");
            offerTable.Columns.Add("SenderName");
            offerTable.Columns.Add("JobTitle");
            offerTable.Columns.Add("JobType");
            offerTable.Columns.Add("JobLocation");
            offerTable.Columns.Add("JoiningDate");
            offerTable.Columns.Add("Salary");

            DataRow offerRow;
            offerRow = offerTable.NewRow();

            offerRow["CompanyName"] = model.CompanyName;
            offerRow["CandidateName"] = model.CandidateName;
            offerRow["SenderName"] = model.SenderName;
            offerRow["JobTitle"] = model.JobTitle;
            offerRow["JobType"] = model.JobType;
            offerRow["JobLocation"] = model.JobLocation;
            offerRow["JoiningDate"] = model.JoiningDate;
            offerRow["Salary"] = model.Salary;

            offerTable.Rows.Add(offerRow);

            writer.DataSources.Clear();
            writer.DataSources.Add(new BoldReports.Web.ReportDataSource { Name = "Offer", Value = offerTable });

            string fileName = "OfferLetter.pdf";
            string type = "pdf";
            WriterFormat format = WriterFormat.PDF;

            writer.LoadReport(reportStream);
            MemoryStream memoryStream = new();
            writer.Save(memoryStream, format);

            memoryStream.Position = 0;
            FileStreamResult fileStreamResult = new(memoryStream, "application/" + type);
            fileStreamResult.FileDownloadName = fileName;
            return fileStreamResult;
        }

        public async Task<IActionResult> OfferJob(int applicationId, int jobId, IFormFile file)
        {
            APIResponse applicationResponse = await _applicationService.GetAsync<APIResponse>(token: HttpContext.Session.GetString(StaticDetails.SessionToken), applicationId);

            if (applicationResponse != null && applicationResponse.IsSuccess)
            {
                ApplicationDTO application = JsonConvert.DeserializeObject<ApplicationDTO>(Convert.ToString(applicationResponse.Result));

                APIResponse candidateResponse = await _profileService.GetCandidateAsync<APIResponse>(token: HttpContext.Session.GetString(StaticDetails.SessionToken), application.CandidateId);

                if (candidateResponse != null && candidateResponse.IsSuccess)
                {
                    CandidateDTO candidate = JsonConvert.DeserializeObject<CandidateDTO>(Convert.ToString(candidateResponse.Result));

                    byte[] offerLetter = null;

                    if (file != null && file.Length > 0)
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            await file.CopyToAsync(memoryStream);
                            offerLetter = memoryStream.ToArray();
                        }
                    }

                    string offerLink = $"https://localhost:7002/Candidate/JobOffer?applicationId={application.ApplicationId}";

                    string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "email", "JobOffer.html");
                    string emailBody = await System.IO.File.ReadAllTextAsync(filePath);

                    emailBody = emailBody.Replace("[Candidate's First Name]", candidate.User.FirstName)
                                .Replace("[Job Title]", application.Job.Title)
                                .Replace("[Company Name]", application.Job.Company)
                                .Replace("[url]", offerLink);

                    EmailRequestModel emailRequest = new()
                    {
                        To = candidate.User.Email,
                        Subject = $"Application Updates - {application.Job.Title} @ {application.Job.Company}",
                        IsBodyHtml = true,
                        Body = emailBody,
                        Attachment = offerLetter,
                        AttachmentName = candidate.FullName + "_Offer Letter.pdf"
                    };

                    await _emailService.SendEmailAsync<APIResponse>(emailRequest);

                    APIResponse getResponse = await _applicationService.GetAsync<APIResponse>(token: HttpContext.Session.GetString(StaticDetails.SessionToken), applicationId);

                    if (getResponse != null && getResponse.IsSuccess)
                    {
                        ApplicationDTO model = JsonConvert.DeserializeObject<ApplicationDTO>(Convert.ToString(getResponse.Result));

                        model.Status = "Job Offered";

                        APIResponse updateResponse = await _applicationService.UpdateAsync<APIResponse>(token: HttpContext.Session.GetString(StaticDetails.SessionToken), model);

                        if (updateResponse != null && updateResponse.IsSuccess)
                        {
                            //TempData["success"] = "Job Offered";
                            //return RedirectToAction("Applicants", "Company", new { jobId = jobId });
                            return Ok(new { jobId = jobId, success = true });
                        }
                    }
                }
            }

            //TempData["error"] = applicationResponse.ErrorMessages.FirstOrDefault();
            //return RedirectToAction("Applicants", "Company", new { jobId = jobId });
            return Ok(new { jobId = jobId, success = false, error = applicationResponse.ErrorMessages.FirstOrDefault() });

        }

        #endregion


        #region Reject Application

        [HttpGet]
        public IActionResult RejectApplicationModal(int applicationId, int jobId)
        {
            ApplicationDTO model = new()
            {
                ApplicationId = applicationId,
                JobId = jobId
            };

            return PartialView("_RejectModal", model);
        }

        public async Task<IActionResult> RejectApplication(int applicationId, int jobId)
        {
            APIResponse applicationResponse = await _applicationService.GetAsync<APIResponse>(token: HttpContext.Session.GetString(StaticDetails.SessionToken), applicationId);

            if (applicationResponse != null && applicationResponse.IsSuccess)
            {
                ApplicationDTO application = JsonConvert.DeserializeObject<ApplicationDTO>(Convert.ToString(applicationResponse.Result));

                APIResponse candidateResponse = await _profileService.GetCandidateAsync<APIResponse>(token: HttpContext.Session.GetString(StaticDetails.SessionToken), application.CandidateId);

                if (candidateResponse != null && candidateResponse.IsSuccess)
                {
                    CandidateDTO candidate = JsonConvert.DeserializeObject<CandidateDTO>(Convert.ToString(candidateResponse.Result));

                    string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "email", "JobOffer.html");
                    string emailBody = await System.IO.File.ReadAllTextAsync(filePath);

                    emailBody = emailBody.Replace("[Candidate's First Name]", candidate.User.FirstName)
                                .Replace("[Job Title]", application.Job.Title)
                                .Replace("[Company Name]", application.Job.Company);

                    EmailRequestModel emailRequest = new()
                    {
                        To = candidate.User.Email,
                        Subject = $"Application Updates - {application.Job.Title} @ {application.Job.Company}",
                        IsBodyHtml = true,
                        Body = emailBody
                    };

                    await _emailService.SendEmailAsync<APIResponse>(emailRequest);

                    APIResponse getResponse = await _applicationService.GetAsync<APIResponse>(token: HttpContext.Session.GetString(StaticDetails.SessionToken), applicationId);

                    if (getResponse != null && getResponse.IsSuccess)
                    {
                        ApplicationDTO model = JsonConvert.DeserializeObject<ApplicationDTO>(Convert.ToString(getResponse.Result));

                        model.Status = "Rejected";

                        APIResponse updateResponse = await _applicationService.UpdateAsync<APIResponse>(token: HttpContext.Session.GetString(StaticDetails.SessionToken), model);

                        if (updateResponse != null && updateResponse.IsSuccess)
                        {
                            TempData["success"] = "Application Rejected";
                            return RedirectToAction("Applicants", "Company", new { jobId = jobId });
                        }
                    }
                }
            }

            TempData["error"] = applicationResponse.ErrorMessages.FirstOrDefault();
            return RedirectToAction("Applicants", "Company", new { jobId = jobId });
        }

        #endregion



        /// <summary>
        /// RDLC Report
        /// </summary>
        /// <returns></returns>
        #region Report

        public IActionResult Report()
        {
            return View();
        }

        #endregion



        #region Calendar

        public async Task<IActionResult> Calendar()
        {
            int userId = Convert.ToInt32(HttpContext.Session.GetString(StaticDetails.SessionId));
            string userRole = HttpContext.Session.GetString(StaticDetails.SessionRole);

            List<InterviewDTO> interviews = new();

            APIResponse interviewResponse = await _interviewService.GetAllAsync<APIResponse>(token: HttpContext.Session.GetString(StaticDetails.SessionToken), userId: userId, userRole: userRole);

            if (interviewResponse != null && interviewResponse.IsSuccess)
            {
                interviews = JsonConvert.DeserializeObject<List<InterviewDTO>>(Convert.ToString(interviewResponse.Result));
            }

            return View(interviews);
        }

        [HttpGet]
        public async Task<IActionResult> ViewInterviewDetailsModal(int interviewId, string title)
        {
            InterviewDTO model = new()
            {
                InterviewId = interviewId,
                InterviewTitle = title,
            };

            APIResponse interviewResponse = await _interviewService.GetAsync<APIResponse>(token: HttpContext.Session.GetString(StaticDetails.SessionToken), id: interviewId);

            if (interviewResponse != null && interviewResponse.IsSuccess)
            {
                InterviewDTO interview = JsonConvert.DeserializeObject<InterviewDTO>(Convert.ToString(interviewResponse.Result));

                model.StartTime = interview.StartTime;
                model.EndTime = interview.EndTime;
                model.InterviewDate = interview.InterviewDate;
                model.Location = interview.Location;
                model.Complete = interview.Complete;
                model.Application = interview.Application;
            }

            return PartialView("_InterviewDetailsModal", model);
        }

        #endregion


        #region Post Job Fair


        [HttpGet]
        public async Task<IActionResult> PostJobFair()
        {
            int userId = Convert.ToInt32(HttpContext.Session.GetString(StaticDetails.SessionId));

            APIResponse companyResponse = await _profileService.GetCompanyAsync<APIResponse>(userId);

            JobFairDTO model = new();

            if (companyResponse.Result != null && companyResponse.IsSuccess)
            {
                CompanyDTO company = JsonConvert.DeserializeObject<CompanyDTO>(Convert.ToString(companyResponse.Result));

                model.CompanyId = company.CompanyId;
                model.Organizer = company.CompanyName;
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> PostJobFair(JobFairDTO model)
        {
            DateTime startDate = DateTime.ParseExact(model.StartDate.ToString(), "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture);
            DateTime utcStartDate = DateTime.SpecifyKind(startDate, DateTimeKind.Utc);
            model.StartDate = utcStartDate;

            DateTime endDate = DateTime.ParseExact(model.EndDate.ToString(), "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture);
            DateTime utcEndDate = DateTime.SpecifyKind(endDate, DateTimeKind.Utc);
            model.EndDate = utcEndDate;

            APIResponse jobFairResponse = await _jobFairService.CrateAsync<APIResponse>(token: HttpContext.Session.GetString(StaticDetails.SessionToken), model);

            if (jobFairResponse != null && jobFairResponse.IsSuccess && jobFairResponse.Result != null)
            {
                JobFairDTO jobFair = JsonConvert.DeserializeObject<JobFairDTO>(Convert.ToString(jobFairResponse.Result));

                string imageUrl = "https://images.unsplash.com/photo-1460176449511-ff5fc8e64c35?q=80&w=2074&auto=format&fit=crop&ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D";
                string anchorUrl = $"https://localhost:7002/Home/JobFairInformation?jobFairId={jobFair.JobFairId}";

                IronQr.QrCode jobQr = QrWriter.Write(anchorUrl);
                string qrImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "QRs", $"{jobFair.Organizer}-{jobFair.JobFairId}.png");
                using (AnyBitmap qrBitmap = jobQr.Save())
                {
                    qrBitmap.SaveAs(qrImagePath);
                }

                APIResponse candidateResponse = await _profileService.GetAllCandidateAsync<APIResponse>(token: HttpContext.Session.GetString(StaticDetails.SessionToken));

                if (candidateResponse != null && candidateResponse.IsSuccess)
                {
                    List<CandidateDTO> userList = JsonConvert.DeserializeObject<List<CandidateDTO>>(Convert.ToString(candidateResponse.Result));

                    string qrUrl = $"https://localhost:7002/QRs/{jobFair.Organizer}-{jobFair.JobFairId}.png";

                    string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "email", "JobFairPost.html");
                    string emailBody = await System.IO.File.ReadAllTextAsync(filePath);

                    emailBody = emailBody.Replace("[Job Fair Title]", jobFair.Title)
                                .Replace("[Company Name]", jobFair.Organizer)
                                .Replace("[Image]", imageUrl)
                                .Replace("[Qr]", qrUrl)
                                .Replace("[url]", anchorUrl);

                    for (int i = 0; i < userList.Count; i++)
                    {
                        emailBody = emailBody.Replace("[Candidate's First Name]", userList[i].FullName);

                        EmailRequestModel emailRequest = new()
                        {
                            To = userList[i].User.Email,
                            Subject = $"Don't Miss Out! Join the {jobFair.Title} by {jobFair.Organizer}!",
                            IsBodyHtml = true,
                            Body = emailBody,
                            //Attachment = System.IO.File.ReadAllBytes(qrImagePath),
                            //AttachmentName = $"{jobFair.Organizer}-{jobFair.JobFairId}.png"
                        };

                        await _emailService.SendEmailAsync<APIResponse>(emailRequest);
                    }
                }

                TempData["success"] = "Job Fair Posted";
                return RedirectToAction("Dashboard", "Company");
            }

            TempData["error"] = jobFairResponse.ErrorMessages;
            return View();
        }

        #endregion



        /// <summary>
        /// Kendo UI
        /// </summary>
        /// <returns></returns>
        #region Kendo

        public IActionResult Kendo()
        {
            //Task.Delay(60000).Wait();
            return View();
        }

        public async Task<IActionResult> GetProducts([DataSourceRequest] DataSourceRequest request)
        {
            int userId = Convert.ToInt32(HttpContext.Session.GetString(StaticDetails.SessionId));
            string userRole = HttpContext.Session.GetString(StaticDetails.SessionRole);

            List<JobDTO> jobs = new();

            APIResponse response = await _jobService.GetAllAsync<APIResponse>(token: HttpContext.Session.GetString(StaticDetails.SessionToken), userRole: userRole, userId: userId, search: "", jobType: "", jobTime: "", workingDays: 0, sortBy: "", sortOrder: "", jobStatus: "", pageNumber: 0, pageSize: 0);

            if (response != null && response.IsSuccess)
            {
                jobs = JsonConvert.DeserializeObject<List<JobDTO>>(Convert.ToString(response.Result));
            }

            return Json(jobs.ToDataSourceResult(request));
        }

        [HttpPost]
        public IActionResult Excel_Export(string contentType, string base64, string fileName)
        {
            byte[] fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
        }


        [HttpPost]
        public IActionResult Pdf_Export(string contentType, string base64, string fileName)
        {
            byte[] fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
        }


        public async Task<IActionResult> FilterMenuCustomization_Titles()
        {
            int userId = Convert.ToInt32(HttpContext.Session.GetString(StaticDetails.SessionId));
            string userRole = HttpContext.Session.GetString(StaticDetails.SessionRole);

            List<JobDTO> jobs = new();

            APIResponse response = await _jobService.GetAllAsync<APIResponse>(token: HttpContext.Session.GetString(StaticDetails.SessionToken), userRole: userRole, userId: userId, search: "", jobType: "", jobTime: "", workingDays: 0, sortBy: "", sortOrder: "", jobStatus: "", pageNumber: 0, pageSize: 0);

            if (response != null && response.IsSuccess)
            {
                jobs = JsonConvert.DeserializeObject<List<JobDTO>>(Convert.ToString(response.Result));
            }

            return Json(jobs.Select(x => x.Title).Distinct().ToList());
        }


        [HttpPost]
        public async Task<IActionResult> UpdateProducts([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<JobDTO> jobs)
        {
            foreach (var job in jobs)
            {
                DateTime unspecifiedDate = DateTime.ParseExact(job.LastDate.ToString(), "MM-dd-yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                DateTime utcDate = DateTime.SpecifyKind(unspecifiedDate, DateTimeKind.Utc);
                job.LastDate = utcDate;

                await _jobService.UpdateAsync<APIResponse>(token: HttpContext.Session.GetString(StaticDetails.SessionToken), job);
            }

            return Json(jobs.ToDataSourceResult(request, ModelState));
        }


        [HttpPost]
        public async Task<IActionResult> CreateProducts([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<JobDTO> jobs)
        {
            List<JobDTO> results = new();

            foreach (var job in jobs)
            {
                await _jobService.CrateAsync<APIResponse>(token: HttpContext.Session.GetString(StaticDetails.SessionToken), job);
                results.Add(job);
            }

            return Json(results.ToDataSourceResult(request, ModelState));
        }


        [HttpPost]
        public async Task<IActionResult> DeleteProducts([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<JobDTO> jobs)
        {

            foreach (var job in jobs)
            {
                await _jobService.DeleteAsync<APIResponse>(token: HttpContext.Session.GetString(StaticDetails.SessionToken), job.JobId);
            }

            return Json(jobs.ToDataSourceResult(request, ModelState));
        }


        public IActionResult GetTabContent(int newTabIndex)
        {
            ViewBag.TabIndex = newTabIndex;

            return PartialView("TabStrip/Content");
        }

        public IActionResult Grid_Read([DataSourceRequest] DataSourceRequest request)
        {
            for (int i = 0; i <= 5000; i++)
            {
                for (int j = 0; j <= 50000; j++)
                {
                    //for(int k = 0; k <= 5000; k++)
                    //{
                    if (i == j)
                    {
                        Products.Add(Products[i]);
                    }
                    //}
                }
            }

            Products = Products.Where(x => x.Discontinued == false).ToList();

            //Task.Delay(60000).Wait();

            return Json(Products.ToDataSourceResult(request));
        }

        private List<ProductVM> Products = new() {
            new ProductVM{ ProductID = 1, ProductName = "Chai", UnitPrice = 18, UnitsInStock = 39, Discontinued = false },
            new ProductVM{ ProductID = 2, ProductName = "Chang", UnitPrice = 19, UnitsInStock = 17, Discontinued = false },
            new ProductVM{ ProductID = 3, ProductName = "Aniseed Syrup", UnitPrice = 10, UnitsInStock = 13, Discontinued = false },
            new ProductVM{ ProductID = 4, ProductName = "Chef Anton's Cajun Seasoning", UnitPrice = 21, UnitsInStock = 53, Discontinued = false },
            new ProductVM{ ProductID = 5, ProductName = "Chef Anton's Gumbo Mix", UnitPrice = 18, UnitsInStock = 0, Discontinued = true },
            new ProductVM{ ProductID = 6, ProductName = "Grandma's Boysenberry Spread", UnitPrice = 25, UnitsInStock = 120, Discontinued = false },
            new ProductVM{ ProductID = 7, ProductName = "Uncle Bob's Organic Dried Pears", UnitPrice = 30, UnitsInStock = 15, Discontinued = false },
            new ProductVM{ ProductID = 8, ProductName = "Northwoods Cranberry Sauce", UnitPrice = 40, UnitsInStock = 6, Discontinued = false },
            new ProductVM{ ProductID = 9, ProductName = "Mishi Kobe Niku", UnitPrice = 97, UnitsInStock = 29, Discontinued = true },
            new ProductVM{ ProductID = 10, ProductName = "Ikura", UnitPrice = 31, UnitsInStock = 31, Discontinued = false },
            new ProductVM{ ProductID = 11, ProductName = "Queso Cabrales", UnitPrice = 21, UnitsInStock = 22, Discontinued = false },
            new ProductVM{ ProductID = 12, ProductName = "Queso Manchego La Pastora", UnitPrice = 38, UnitsInStock = 86, Discontinued = false },
        };

        #endregion



        /// <summary>
        /// Dynamic QR
        /// </summary>
        /// <returns></returns>
        #region Dynamic QR

        [HttpGet]
        public async Task<IActionResult> QR()
        {
            QRCodeVM model = new()
            {
                Candidate = new CandidateDTO(),
                Job = new JobDTO()
            };

            int userId = Convert.ToInt32(HttpContext.Session.GetString(StaticDetails.SessionId));
            string userRole = HttpContext.Session.GetString(StaticDetails.SessionRole);

            APIResponse jobResponse = await _jobService.GetAllAsync<APIResponse>(token: HttpContext.Session.GetString(StaticDetails.SessionToken), userRole: userRole, userId: userId, search: "", jobType: "", jobTime: "", workingDays: 0, sortBy: "", sortOrder: "", jobStatus: "", pageNumber: 0, pageSize: 0);

            if (jobResponse != null && jobResponse.IsSuccess)
            {
                model.Jobs = JsonConvert.DeserializeObject<List<JobDTO>>(Convert.ToString(jobResponse.Result));
            }

            APIResponse candidateResponse = await _profileService.GetAllCandidateAsync<APIResponse>(token: HttpContext.Session.GetString(StaticDetails.SessionToken));

            if (candidateResponse != null && candidateResponse.IsSuccess)
            {
                model.Candidates = JsonConvert.DeserializeObject<List<CandidateDTO>>(Convert.ToString(candidateResponse.Result));
            }

            APIResponse templateResponse = await _templateService.GetAllAsync<APIResponse>(token: HttpContext.Session.GetString(StaticDetails.SessionToken));

            if (templateResponse != null && templateResponse.IsSuccess)
            {
                model.Templates = JsonConvert.DeserializeObject<List<TemplateDTO>>(Convert.ToString(templateResponse.Result));
            }


            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> SaveTemplate(int correctionLevel, int inputMargins, string colorGradient, string dataSource, string inpField, string elementsData)
        {
            List<ElementDTO> elements = JsonConvert.DeserializeObject<List<ElementDTO>>(elementsData);

            TemplateDTO templateDTO = new()
            {
                CorrectionLevel = correctionLevel,
                Margin = inputMargins,
                Color = colorGradient,
                DataSource = dataSource,
                TextField = inpField,
                Elements = elements
            };

            APIResponse response = await _templateService.CreateAsync<APIResponse>(token: HttpContext.Session.GetString(StaticDetails.SessionToken), templateDTO);

            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Template Saved";
            }

            return RedirectToAction("QR");
        }

        public async Task<IActionResult> GenerateQR(string QrDataSource, string QrDataSourceField, string QrDataSourceField1, List<int> SelectedJobs, List<int> SelectedCandidates, int QrTemplate)
        {
            APIResponse templateResponse = await _templateService.GetAsync<APIResponse>(token: HttpContext.Session.GetString(StaticDetails.SessionToken), id: QrTemplate);

            TemplateDTO template = JsonConvert.DeserializeObject<TemplateDTO>(Convert.ToString(templateResponse.Result));

            QrOptions options = new()
            {
                ErrorCorrectionLevel = (QrErrorCorrectionLevel)template.CorrectionLevel,
                Version = 20
            };

            StringBuilder html = new();

            if (QrDataSource == "Candidate")
            {
                for (int i = 0; i < SelectedCandidates.Count; i++)
                {
                    APIResponse response = await _profileService.GetCandidateAsync<APIResponse>(token: HttpContext.Session.GetString(StaticDetails.SessionToken), SelectedCandidates[i]);

                    CandidateDTO candidate = JsonConvert.DeserializeObject<CandidateDTO>(Convert.ToString(response.Result));

                    PropertyInfo qrProperty = typeof(CandidateDTO).GetProperty(QrDataSourceField);
                    PropertyInfo txtProperty = typeof(CandidateDTO).GetProperty(template.TextField);

                    object txtValue = null;

                    if (txtProperty != null)
                    {
                        txtValue = txtProperty.GetValue(candidate);
                    }

                    html.Append("<div style='margin: 5px; padding: 10px; height: 400px; width: 700px; position: relative'>");

                    foreach (var element in template.Elements)
                    {
                        string content = "";

                        switch (element.Name)
                        {
                            case "clonedImg1":
                                IronQr.QrCode qrCode = QrWriter.Write(qrProperty.GetValue(candidate).ToString(), options);

                                QrStyleOptions style = new()
                                {
                                    Dimensions = element.Width,
                                    Margins = template.Margin,
                                    Color = new IronSoftware.Drawing.Color(template.Color),
                                };

                                string qrImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "QRs", $"QR-{i}.png");
                                AnyBitmap qrBitmap = qrCode.Save(style);
                                qrBitmap.SaveAs(qrImagePath);

                                byte[] qrImageBytes = System.IO.File.ReadAllBytes(qrImagePath);
                                string qrImageBase64 = Convert.ToBase64String(qrImageBytes);
                                content = $"<img src='data:image/png;base64,{qrImageBase64}' style='width: 100%; height: 100%;' />";
                                //content = $"<img src='{qrImagePath}' style='width: 100%; height: 100%;' />";

                                break;

                            case "clonedImg2":
                                string logoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "user.png");
                                byte[] logoBytes = System.IO.File.ReadAllBytes(logoPath);
                                string logoBase64 = Convert.ToBase64String(logoBytes);
                                content = $"<img src='data:image/png;base64,{logoBase64}' style='width: 100%; height: 100%px;' />";
                                //content = $"<img src='{logoPath}' style='width: 100%; height: 100%px;' />";
                                break;

                            case "clonedImg3":
                                content = $"<div style='width: {element.Width}px; height: {element.Height}px;'>{txtValue}</div>";
                                break;
                        }

                        html.Append($@"
                            <div style='position: absolute; left: {element.Left}px; top: {element.Top}px; width: {element.Width}px; height: {element.Height}px;'>
                                {content}
                            </div>");
                    }
                    html.Append("</div>");
                }
            }
            else if (QrDataSource == "Job")
            {
                for (int i = 0; i < SelectedJobs.Count; i++)
                {
                    APIResponse response = await _jobService.GetAsync<APIResponse>(token: HttpContext.Session.GetString(StaticDetails.SessionToken), SelectedJobs[i]);

                    JobDTO job = JsonConvert.DeserializeObject<JobDTO>(Convert.ToString(response.Result));

                    PropertyInfo qrProperty = typeof(JobDTO).GetProperty(QrDataSourceField1);
                    PropertyInfo txtProperty = typeof(JobDTO).GetProperty(template.TextField);

                    object txtValue = null;

                    if (txtProperty != null)
                    {
                        txtValue = txtProperty.GetValue(job);
                    }

                    html.Append("<div style='margin: 5px; padding: 10px; height: 400px; width: 700px; position: relative'>");

                    foreach (var element in template.Elements)
                    {
                        string content = "";

                        switch (element.Name)
                        {
                            case "clonedImg1":
                                IronQr.QrCode qrCode = QrWriter.Write(qrProperty.GetValue(job).ToString(), options);

                                QrStyleOptions style = new()
                                {
                                    Dimensions = element.Width,
                                    Margins = template.Margin,
                                    Color = new IronSoftware.Drawing.Color(template.Color),
                                };

                                string qrImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "QRs", $"QR-{i}.png");
                                AnyBitmap qrBitmap = qrCode.Save(style);
                                qrBitmap.SaveAs(qrImagePath);

                                byte[] qrImageBytes = System.IO.File.ReadAllBytes(qrImagePath);
                                string qrImageBase64 = Convert.ToBase64String(qrImageBytes);
                                content = $"<img src='data:image/png;base64,{qrImageBase64}' style='width: 100%; height: 100%;' />";
                                //content = $"<img src='{qrImagePath}' style='width: 100%; height: 100%;' />";

                                break;

                            case "clonedImg2":
                                string logoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "user.png");
                                byte[] logoBytes = System.IO.File.ReadAllBytes(logoPath);
                                string logoBase64 = Convert.ToBase64String(logoBytes);
                                content = $"<img src='data:image/png;base64,{logoBase64}' style='width: 100%; height: 100%px;' />";
                                //content = $"<img src='{logoPath}' style='width: 100%; height: 100%px;' />";
                                break;

                            case "clonedImg3":
                                content = $"<div style='width: {element.Width}px; height: {element.Height}px;'>{txtValue}</div>";
                                break;
                        }

                        html.Append($@"
                            <div style='position: absolute; left: {element.Left}px; top: {element.Top}px; width: {element.Width}px; height: {element.Height}px;'>
                                {content}
                            </div>");
                    }
                    html.Append("</div>");
                }
            }

            string newHtml = html.ToString();

            string viewPath = Path.Combine(Directory.GetCurrentDirectory(), "Views", "Company", "DynamicQRs.cshtml");
            //System.IO.File.Delete(viewPath);
            System.IO.File.WriteAllText(viewPath, newHtml);

            return new ViewAsPdf("DynamicQRs")
            {
                FileName = "Dynamic_QRs.pdf"
            };
        }

        public IActionResult DynamicQRs()
        {
            return View();
        }

        public async Task<IActionResult> PreviewTemplate(int templateId)
        {
            APIResponse templateResponse = await _templateService.GetAsync<APIResponse>(token: HttpContext.Session.GetString(StaticDetails.SessionToken), id: templateId);

            TemplateDTO template = JsonConvert.DeserializeObject<TemplateDTO>(Convert.ToString(templateResponse.Result));

            StringBuilder html = new();

            foreach (var element in template.Elements)
            {
                string content = "";

                switch (element.Name)
                {
                    case "clonedImg1":
                        string qrImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "QRs", $"radhe.png");
                        byte[] qrImageBytes = System.IO.File.ReadAllBytes(qrImagePath);
                        string qrImageBase64 = Convert.ToBase64String(qrImageBytes);
                        content = $"<img src='data:image/png;base64,{qrImageBase64}' style='width: 100%; height: 100%;' />";
                        //content = $"<img src='{qrImagePath}' style='width: 100%; height: 100%;' />";

                        break;

                    case "clonedImg2":
                        string logoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "user.png");
                        byte[] logoBytes = System.IO.File.ReadAllBytes(logoPath);
                        string logoBase64 = Convert.ToBase64String(logoBytes);
                        content = $"<img src='data:image/png;base64,{logoBase64}' style='width: 100%; height: 100%px;' />";
                        //content = $"<img src='{logoPath}' style='width: 100%; height: 100%px;' />";
                        break;

                    case "clonedImg3":
                        content = $"<div style='width: {element.Width}px; height: {element.Height}px;'>{template.TextField}</div>";
                        break;
                }

                html.Append($@"
                    <div style='position: absolute; left: {element.Left}px; top: {element.Top}px; width: {element.Width}px; height: {element.Height}px;'>
                        {content}
                    </div>");
            }

            string newHtml = html.ToString();

            return Json(newHtml);
        }

        #endregion

    }
}
