using IronQr;
using IronSoftware.Drawing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Globalization;
using System.Net;
using System.Net.Mail;
using System.Reflection;
using Unstop.Models;
using Unstop.Models.DTO;
using Unstop.Models.VM;
using Unstop.Services;
using Unstop.Services.IServices;
using Unstop_Utility;

namespace Unstop.Controllers
{
    [Authorize(Roles = "Candidate")]
    public class CandidateController : Controller
    {
        private readonly IJobService _jobService;
        private readonly IProfileService _profileService;
        private readonly IApplicationService _applicationService;
        private readonly IEmailService _emailService;
        private readonly IPreferenceService _preferenceService;
        private readonly IInterviewService _interviewService;

        public CandidateController(IJobService jobService, IProfileService profileService, IApplicationService applicationService, IEmailService emailService, IPreferenceService preferenceService, IInterviewService interviewService)
        {
            _jobService = jobService;
            _profileService = profileService;
            _applicationService = applicationService;
            _emailService = emailService;
            _preferenceService = preferenceService;
            _interviewService = interviewService;
        }


        #region Dashboard 

        [HttpGet]
        public IActionResult Dashboard()
        {
            return View();
        }

        #endregion


        #region Get Job

        //[HttpGet]
        public async Task<IActionResult> Jobs(string search, string jobType, string jobTiming, int workingDays, bool clear, string sortBy, string sortOrder, bool isAjax, bool isPreferredJobs, int pageNumber = 1, int pageSize = 2)
        {
            int userId = Convert.ToInt32(HttpContext.Session.GetString(StaticDetails.SessionId));
            string userRole = HttpContext.Session.GetString("sessionUserRole");

            List<JobDTO> jobs = new();
            UserPreferenceDTO userPreferenceDTO = null;

            if (isPreferredJobs)
            {
                APIResponse preferenceResponse = await _preferenceService.GetAsync<APIResponse>(token: HttpContext.Session.GetString(StaticDetails.SessionToken), id: userId);

                userPreferenceDTO = JsonConvert.DeserializeObject<UserPreferenceDTO>(Convert.ToString(preferenceResponse.Result));
            }

            if (clear)
            {
                jobType = null;
                jobTiming = null;
                workingDays = 0;
            }

            APIResponse response = await _jobService.GetAllAsync<APIResponse>(token: HttpContext.Session.GetString(StaticDetails.SessionToken), userRole: userRole, userId: userId, search: search, jobType: jobType, jobTime: jobTiming, workingDays: workingDays, sortBy: sortBy, sortOrder: sortOrder, jobStatus: "active", pageNumber: pageNumber, pageSize: pageSize, userPreferenceDTO: userPreferenceDTO);

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
            ViewBag.PreferredJob = isPreferredJobs;
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
            int userId = Convert.ToInt32(HttpContext.Session.GetString(StaticDetails.SessionId));

            APIResponse response = await _jobService.GetAsync<APIResponse>(token: HttpContext.Session.GetString(StaticDetails.SessionToken), jobId);

            if (response != null && response.IsSuccess)
            {
                JobDTO model = JsonConvert.DeserializeObject<JobDTO>(Convert.ToString(response.Result));

                APIResponse favoriteResponse = await _jobService.CheckFavoriteAsync<APIResponse>(token: HttpContext.Session.GetString(StaticDetails.SessionToken), jobId: jobId, userId: userId);

                if (favoriteResponse != null && favoriteResponse.IsSuccess)
                {
                    model.IsFavorite = true;
                }
                else
                {
                    model.IsFavorite = false;
                }

                return View(model);
            }

            TempData["error"] = response.ErrorMessages.FirstOrDefault();
            return View("Jobs");
        }

        #endregion


        #region Profile

        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            int userId = Convert.ToInt32(HttpContext.Session.GetString(StaticDetails.SessionId));

            APIResponse response = await _profileService.GetCandidateAsync<APIResponse>(token: HttpContext.Session.GetString(StaticDetails.SessionToken), userId);

            if (response != null && response.IsSuccess)
            {
                CandidateDTO model = JsonConvert.DeserializeObject<CandidateDTO>(Convert.ToString(response.Result));

                return View(model);
            }

            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Profile(CandidateDTO model)
        {
            DateTime unspecifiedDate = DateTime.ParseExact(model.DateOfBirth.ToString(), "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture);
            DateTime utcDate = DateTime.SpecifyKind(unspecifiedDate, DateTimeKind.Utc);
            model.DateOfBirth = utcDate;

            APIResponse response = await _profileService.UpdateCandidateAsync<APIResponse>(token: HttpContext.Session.GetString(StaticDetails.SessionToken), candidateDTO: model);

            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Profile Updated";
                return RedirectToAction("Dashboard", "Candidate");
            }

            TempData["error"] = response.ErrorMessages.FirstOrDefault();
            return RedirectToAction("Profile", "Candidate");
        }

        #endregion


        #region Application

        [HttpGet]
        public async Task<IActionResult> ApplyModal(int jobId)
        {
            int userId = Convert.ToInt32(HttpContext.Session.GetString(StaticDetails.SessionId));

            APIResponse candidateResponse = await _profileService.GetCandidateAsync<APIResponse>(token: HttpContext.Session.GetString(StaticDetails.SessionToken), userId);

            if (candidateResponse != null && candidateResponse.IsSuccess)
            {
                CandidateDTO candidate = JsonConvert.DeserializeObject<CandidateDTO>(Convert.ToString(candidateResponse.Result));

                if (!candidate.IsProfileCompleted)
                {
                    TempData["error"] = "Please Complete Your Profile First";
                    return RedirectToAction("Profile");
                }

                APIResponse response = await _applicationService.CheckAsync<APIResponse>(token: HttpContext.Session.GetString(StaticDetails.SessionToken), jobId: jobId, userId: candidate.CandidateId);

                if (response != null && !response.IsSuccess)
                {
                    TempData["error"] = response.ErrorMessages.FirstOrDefault();
                    return View("Dashboard");
                }

                ApplicationDTO model = new()
                {
                    CandidateId = candidate.CandidateId,
                    JobId = jobId
                };

                return PartialView("_ApplicationModal", model);
            }

            return View("JobInformation", jobId);
        }

        [HttpPost]
        public async Task<IActionResult> PostApplication(ApplicationDTO model)
        {
            if (model.ResumeFile != null && model.ResumeFile.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await model.ResumeFile.CopyToAsync(memoryStream);
                    model.Resume = memoryStream.ToArray();

                    string fileExtension = Path.GetExtension(model.ResumeFile.FileName);
                    string uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";

                    model.ResumeFileName = uniqueFileName;
                    model.ResumeContentType = model.ResumeFile.ContentType;
                }
            }

            APIResponse response = await _applicationService.CrateAsync<APIResponse>(token: HttpContext.Session.GetString(StaticDetails.SessionToken), model);

            if (response != null && response.IsSuccess)
            {
                ApplicationDTO application = JsonConvert.DeserializeObject<ApplicationDTO>(Convert.ToString(response.Result));

                APIResponse userResponse = await _profileService.GetCompanyAsync<APIResponse>(application.Job.UserId);

                if (userResponse != null && userResponse.IsSuccess)
                {
                    CandidateDTO candidate = JsonConvert.DeserializeObject<CandidateDTO>(Convert.ToString(userResponse.Result));

                    string anchorUrl = $"https://localhost:7002/Company/Applicants?jobId={application.JobId}";

                    string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "email", "JobApplication.html");
                    string emailBody = await System.IO.File.ReadAllTextAsync(filePath);

                    emailBody = emailBody.Replace("[Job Title]", application.Job.Title)
                                .Replace("[Company's First Name]", candidate.User.FirstName)
                                .Replace("[url]", anchorUrl);

                    EmailRequestModel emailRequest = new()
                    {
                        To = candidate.User.Email,
                        Subject = $"New Application - {application.Job.Title}",
                        IsBodyHtml = true,
                        Body = emailBody,
                        Attachment = model.Resume,
                        AttachmentName = model.ResumeFileName
                    };

                    await _emailService.SendEmailAsync<APIResponse>(emailRequest);
                }

                TempData["success"] = "Application Submitted";
                return RedirectToAction("Dashboard", "Candidate");
            }

            TempData["error"] = response.ErrorMessages.FirstOrDefault();
            return View("JobInformation", model.JobId);
        }


        //[HttpGet]
        public async Task<IActionResult> Applications(string search, List<string> status, bool clear, string sortOrder, bool isAjax, int pageNumber = 1, int pageSize = 2)
        {
            int userId = Convert.ToInt32(HttpContext.Session.GetString(StaticDetails.SessionId));
            string userRole = HttpContext.Session.GetString("sessionUserRole");

            if (clear)
            {
                status = null;
            }

            List<ApplicationDTO> applications = new();

            APIResponse response = await _applicationService.GetAllAsync<APIResponse>(token: HttpContext.Session.GetString(StaticDetails.SessionToken), userRole: userRole, userId: userId, jobId: 0, searchApplicants: "", searchApplication: search, status: status, sortOrder: sortOrder, pageNumber: pageNumber, pageSize: pageSize);

            if (response != null && response.IsSuccess)
            {
                applications = JsonConvert.DeserializeObject<List<ApplicationDTO>>(Convert.ToString(response.Result));
            }

            ApplicationsVM model = new()
            {
                Applications = applications,
                Pagination = response.PaginationData
            };

            ViewBag.Search = search;
            ViewBag.Status = status;
            ViewBag.SortOrder = sortOrder;
            ViewBag.PageSize = pageSize;

            if (isAjax)
            {
                return PartialView("_ApplicationsList", model);
            }

            return View(model);
        }

        #endregion


        #region Withdraw Application

        [HttpGet]
        public IActionResult WithdrawApplicationModal(int applicationId)
        {
            ApplicationDTO model = new()
            {
                ApplicationId = applicationId,
            };

            return PartialView("_WithdrawModal", model);
        }

        public async Task<IActionResult> WithdrawApplication(int applicationId)
        {
            APIResponse response = await _applicationService.GetAsync<APIResponse>(token: HttpContext.Session.GetString(StaticDetails.SessionToken), applicationId);

            if (response != null && response.IsSuccess)
            {
                ApplicationDTO model = JsonConvert.DeserializeObject<ApplicationDTO>(Convert.ToString(response.Result));

                model.Status = "Withdrawn";

                APIResponse updateResponse = await _applicationService.UpdateAsync<APIResponse>(token: HttpContext.Session.GetString(StaticDetails.SessionToken), model);

                if (updateResponse != null && updateResponse.IsSuccess)
                {
                    TempData["success"] = "Application Withdrawed";
                    return RedirectToAction("Applications", "Candidate");
                }

            }        

            TempData["error"] = response.ErrorMessages.FirstOrDefault();
            return RedirectToAction("Applications", "Candidate");
        }

        #endregion


        #region Job Offer 

        [HttpGet]
        public async Task<IActionResult> JobOffer(int applicationId)
        {
            APIResponse response = await _applicationService.GetAsync<APIResponse>(token: HttpContext.Session.GetString(StaticDetails.SessionToken), applicationId);

            if (response != null && response.IsSuccess)
            {
                ApplicationDTO model = JsonConvert.DeserializeObject<ApplicationDTO>(Convert.ToString(response.Result));

                if (model.Status == "Accepted" || model.Status == "Declined")
                {
                    TempData["error"] = "Already responded";
                    return RedirectToAction("Login", "Home");
                }

                return View(model);
            }

            TempData["error"] = response.ErrorMessages.FirstOrDefault();
            return RedirectToAction("Login", "Home");
        }

        public async Task<IActionResult> AcceptJobOffer(int applicationId)
        {
            APIResponse response = await _applicationService.GetAsync<APIResponse>(token: HttpContext.Session.GetString(StaticDetails.SessionToken), applicationId);

            if (response != null && response.IsSuccess)
            {
                ApplicationDTO model = JsonConvert.DeserializeObject<ApplicationDTO>(Convert.ToString(response.Result));

                model.Status = "Accepted";

                APIResponse updateResponse = await _applicationService.UpdateAsync<APIResponse>(token: HttpContext.Session.GetString(StaticDetails.SessionToken), model);

                if (updateResponse != null && updateResponse.IsSuccess)
                {
                    TempData["success"] = "Offer Accepted";
                    return RedirectToAction("Applications", "Candidate");
                }

            }

            TempData["error"] = response.ErrorMessages.FirstOrDefault();
            return RedirectToAction("Applications", "Candidate");
        }

        public async Task<IActionResult> DeclineJobOffer(int applicationId)
        {
            APIResponse response = await _applicationService.GetAsync<APIResponse>(token: HttpContext.Session.GetString(StaticDetails.SessionToken), applicationId);

            if (response != null && response.IsSuccess)
            {
                ApplicationDTO model = JsonConvert.DeserializeObject<ApplicationDTO>(Convert.ToString(response.Result));

                model.Status = "Declined";

                APIResponse updateResponse = await _applicationService.UpdateAsync<APIResponse>(token: HttpContext.Session.GetString(StaticDetails.SessionToken), model);

                if (updateResponse != null && updateResponse.IsSuccess)
                {
                    TempData["success"] = "Offer Declined";
                    return RedirectToAction("Applications", "Candidate");
                }

            }

            TempData["error"] = response.ErrorMessages.FirstOrDefault();
            return RedirectToAction("Applications", "Candidate");
        }

        #endregion


        #region Favorite Job

        public async Task<IActionResult> FavoriteJobs(string search, bool isAjax, int pageNumber = 1, int pageSize = 2)
        {
            int userId = Convert.ToInt32(HttpContext.Session.GetString(StaticDetails.SessionId));

            List<FavoriteDTO> favorites = new();

            APIResponse response = await _jobService.GetAllFavoriteAsync<APIResponse>(token: HttpContext.Session.GetString(StaticDetails.SessionToken), userId: userId, search: search, pageNumber: pageNumber, pageSize: pageSize);

            if (response != null && response.IsSuccess)
            {
                favorites = JsonConvert.DeserializeObject<List<FavoriteDTO>>(Convert.ToString(response.Result));
            }

            FavoritesVM model = new()
            {
                Favorites = favorites,
                Pagination = response.PaginationData
            };

            ViewBag.Search = search;

            if (isAjax)
            {
                return PartialView("_FavoriteJobsList", model);
            }

            return View(model);
        }

        public async Task<IActionResult> AddToFavorite(int jobId)
        {
            FavoriteDTO dto = new()
            {
                JobId = jobId,
                UserId = Convert.ToInt32(HttpContext.Session.GetString(StaticDetails.SessionId))
            };

            APIResponse response = await _jobService.CrateFavoriteAsync<APIResponse>(token: HttpContext.Session.GetString(StaticDetails.SessionToken), dto);

            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Added to Favorite Jobs";
            }

            TempData["error"] = response.ErrorMessages.FirstOrDefault();

            return RedirectToAction("JobInformation", new { jobId = jobId });
        }

        public async Task<IActionResult> RemoveFromFavorite(int jobId, int callId)
        {
            int userId = Convert.ToInt32(HttpContext.Session.GetString(StaticDetails.SessionId));

            APIResponse response = await _jobService.RemoveFavoriteAsync<APIResponse>(token: HttpContext.Session.GetString(StaticDetails.SessionToken), jobId: jobId, userId: userId);

            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Removed From Favorite Jobs";
            }

            if (callId == 1)
            {
                return RedirectToAction("FavoriteJobs");
            }

            TempData["error"] = response.ErrorMessages.FirstOrDefault();
            return RedirectToAction("JobInformation", new { jobId = jobId });
        }

        #endregion


        #region Preferences

        [HttpGet]
        public async Task<IActionResult> Preferences()
        {
            int userId = Convert.ToInt32(HttpContext.Session.GetString(StaticDetails.SessionId));

            UserPreferenceDTO model = new()
            {
                UserId = userId,
            };

            APIResponse response = await _preferenceService.GetAsync<APIResponse>(token: HttpContext.Session.GetString(StaticDetails.SessionToken), id: userId);

            if (response != null && response.IsSuccess)
            {
                UserPreferenceDTO userPreference = JsonConvert.DeserializeObject<UserPreferenceDTO>(Convert.ToString(response.Result));

                model.JobType = userPreference.JobType;
                model.JobTime = userPreference.JobTime;
                model.Industry = userPreference.Industry;
                model.CompanySize = userPreference.CompanySize;
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Preferences(UserPreferenceDTO userPreferenceDTO)
        {
            APIResponse getResponse = await _preferenceService.GetAsync<APIResponse>(token: HttpContext.Session.GetString(StaticDetails.SessionToken), id: userPreferenceDTO.UserId);

            if (getResponse.StatusCode == HttpStatusCode.NotFound)
            {
                APIResponse createResponse = await _preferenceService.CreateAsync<APIResponse>(token: HttpContext.Session.GetString(StaticDetails.SessionToken), userPreferenceDTO: userPreferenceDTO);

                if (createResponse != null && createResponse.IsSuccess)
                {
                    TempData["success"] = "Preferences Created";
                    return RedirectToAction("Dashboard");
                }
            }

            if (getResponse.StatusCode == HttpStatusCode.OK)
            {
                UserPreferenceDTO userPreference = JsonConvert.DeserializeObject<UserPreferenceDTO>(Convert.ToString(getResponse.Result));

                userPreferenceDTO.PreferenceId = userPreference.PreferenceId;

                APIResponse updateResponse = await _preferenceService.UpdateAsync<APIResponse>(token: HttpContext.Session.GetString(StaticDetails.SessionToken), userPreferenceDTO: userPreferenceDTO);

                if (updateResponse != null && updateResponse.IsSuccess)
                {
                    TempData["success"] = "Preferences Updated";
                    return RedirectToAction("Dashboard");
                }
            }

            TempData["error"] = getResponse.ErrorMessages.FirstOrDefault();
            return RedirectToAction("Preferences");
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


        #region QR Scan

        public async Task<IActionResult> CompanyInformation(string fileName)
        {
            string qrPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "QRs", fileName);
            var inputBmp = AnyBitmap.FromFile(qrPath);

            QrImageInput imageInput = new(inputBmp);
            QrReader reader = new();
            CompanyDTO model = new();

            IEnumerable<QrResult> results = reader.Read(imageInput);
            foreach (QrResult qrResult in results)
            {
                //TempData["success"] = qrResult.Value;
                APIResponse response = await _profileService.GetCompanyAsync<APIResponse>(Convert.ToInt32(qrResult.Value));

                if(response != null && response.IsSuccess)
                {
                    model = JsonConvert.DeserializeObject<CompanyDTO>(Convert.ToString(response.Result));
                }
            }

            return PartialView("_CompanyInformationModal", model);
        }

        #endregion
    }
}
