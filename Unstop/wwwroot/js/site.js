// ********************************************************* filter *********************************************************

function seachJob(search, callId, sortBy, sortOrder, jobType, jobTime, workingDays, jobStatus, clear, isPreferredJobs, pageSize) {
    if (callId == 1) {
        $.ajax({
            method: "GET",
            data: { search: search, sortBy: sortBy, sortOrder: sortOrder, JobType: jobType, JobTiming: jobTime, WorkingDays: workingDays, clear: clear, jobStatus: jobStatus, isAjax: true, pageSize: pageSize },
            url: "/Company/Jobs",

            success: function (result) {
                $('#jobsContainer').html(result);
                if (search.length > 0) {
                    $('#jobSearchInput').focus();
                    $('#jobSearchInput')[0].setSelectionRange(search.length, search.length);
                }
            },

            error: function () {

            }
        });
    }
    else if (callId == 2) {
        $.ajax({
            method: "GET",
            data: { search: search, sortBy: sortBy, sortOrder: sortOrder, JobType: jobType, JobTiming: jobTime, WorkingDays: workingDays, clear: clear, isAjax: true, isPreferredJobs: isPreferredJobs, pageSize: pageSize },
            url: "/Candidate/Jobs",

            success: function (result) {
                $('#candidateJobsContainer').html(result);
                if (search.length > 0) {
                    $('#jobSearchInput').focus();
                    $('#jobSearchInput')[0].setSelectionRange(search.length, search.length);
                }
            },

            error: function () {

            }
        });
    }
}

function filterJob(callId, isPreferredJobs, pageSize) {
    event.preventDefault();

    var formData = $('#jobFilterForm').serializeJSON();

    seachJob(formData.search, callId, formData.sortBy, formData.sortOrder, formData.jobType, formData.jobTiming, formData.workingDays, formData.jobStatus, false, isPreferredJobs, pageSize);
    $('#filterModal').modal('hide');
}

function seachApplication(search, sortOrder, status, clear, pageSize) {
    $.ajax({
        method: "GET",
        url: "/Candidate/Applications",
        data: { search: search, status: status, sortOrder: sortOrder, clear: clear, isAjax: true, pageSize: pageSize },
        traditional: true,

        success: function (result) {
            $('#candidateApplicationsContainer').html(result);
            if (search.length > 0) {
                $('#applicationSearchInput').focus();
                $('#applicationSearchInput')[0].setSelectionRange(search.length, search.length);
            }
        },

        error: function () {

        }
    });
}

function filterApplication(pageSize) {
    event.preventDefault();

    var formData = new FormData($('#applicationFilterForm')[0]);

    var checkedValues = [];
    formData.forEach((value, key) => {
        if (key === "status" && value) {
            checkedValues.push(value);
        }
    });

    seachApplication(formData.get('search'), formData.get('sortOrder'), checkedValues, false, pageSize);
    $('#applicationFilterModal').modal('hide');
}

function seachApplicants(search, jobId, sortOrder, status, clear, pageSize) {
    $.ajax({
        method: "GET",
        url: "/Company/Applicants",
        data: { search: search, jobId: jobId, status: status, sortOrder: sortOrder, clear: clear, isAjax: true, pageSize: pageSize },
        traditional: true,

        success: function (result) {
            $('#applicantsContainer').html(result);
            if (search.length > 0) {
                $('#applicantSearchInput').focus();
                $('#applicantSearchInput')[0].setSelectionRange(search.length, search.length);
            }
        },

        error: function () {

        }
    });
}

function filterApplicant(pageSize) {
    event.preventDefault();

    var formData = new FormData($('#applicantFilterForm')[0]);

    var checkedValues = [];
    formData.forEach((value, key) => {
        if (key === "status" && value) {
            checkedValues.push(value);
        }
    });

    seachApplicants(formData.get('search'), formData.get('jobId'), formData.get('sortOrder'), checkedValues, false, pageSize);
    $('#applicantFilterModal').modal('hide');
}

function seachFavoriteJob(search) {
    $.ajax({
        method: "GET",
        data: { search: search, isAjax: true },
        url: "/Candidate/FavoriteJobs",

        success: function (result) {
            $('#favoriteJobsContainer').html(result);
            if (search.length > 0) {
                $('#favoriteSearchInput').focus();
                $('#favoriteSearchInput')[0].setSelectionRange(search.length, search.length);
            }
        },

        error: function () {

        }
    });
}


// ****************************************************** confirmation ******************************************************

function openDeleteConfirmation(jobId) {
    $.ajax({
        method: "GET",
        data: { jobId: jobId },
        url: "/Company/DeleteJob",

        success: function (result) {
            $('#modalContainer').html(result);
            $('#deleteModal').modal('show');
        },

        error: function () {

        }
    });
}

function openReviewConfirmation(applicationId, jobId) {
    $.ajax({
        method: "GET",
        data: { applicationId: applicationId, jobId: jobId },
        url: "/Company/ReviewApplicationModal",

        success: function (result) {
            $('#applicantsModalContainer').html(result);
            $('#reviewModal').modal('show');
        },

        error: function () {

        }
    });
}

function openInterviewDone(applicationId, jobId) {
    $.ajax({
        method: "GET",
        data: { applicationId: applicationId, jobId: jobId },
        url: "/Company/InterviewedModal",

        success: function (result) {
            $('#applicantsModalContainer').html(result);
            $('#interviewedModal').modal('show');
        },

        error: function () {

        }
    });
}

function openOfferJobModal(applicationId) {
    $.ajax({
        method: "GET",
        data: { applicationId: applicationId },
        url: "/Company/OfferJobModal",

        success: function (result) {
            $('#applicantsModalContainer').html(result);
            $('#offerJobModal').modal('show');
        },

        error: function () {

        }
    });
}

function openWithdrawConfirmation(applicationId) {
    $.ajax({
        method: "GET",
        data: { applicationId: applicationId },
        url: "/Candidate/WithdrawApplicationModal",

        success: function (result) {
            $('#candidateModalContainer').html(result);
            $('#withdrawModal').modal('show');
        },

        error: function () {

        }
    });
}

function openRejectConfirmation(applicationId, jobId) {
    $.ajax({
        method: "GET",
        data: { applicationId: applicationId, jobId: jobId },
        url: "/Company/RejectApplicationModal",

        success: function (result) {
            $('#applicantsModalContainer').html(result);
            $('#rejectModal').modal('show');
        },

        error: function () {

        }
    });
}



// ********************************************************** form **********************************************************

function openApplyModal(jobId) {
    $.ajax({
        method: "GET",
        url: "/Candidate/ApplyModal",
        data: { jobId: jobId },

        success: function (result) {
            $('#applicationModalContainer').html(result);
            $('#applicationModal').modal('show');
        },

        error: function () {

        }
    });
}

function openResume(applicationId, jobId) {
    $.ajax({
        method: "GET",
        url: "/Home/GetResume",
        data: { applicationId: applicationId, jobId: jobId },
        xhrFields: {
            responseType: 'blob'
        },

        success: function (result, status, xhr) {
            var contentType = xhr.getResponseHeader('Content-Type');
            var blob = new Blob([result], { type: contentType });
            //var blob = new Blob([result], { type: 'application/pdf' });

            var url = window.URL.createObjectURL(blob);
            window.open(url, "_blank");
            window.URL.revokeObjectURL(url);
        },

        error: function () {
        }
    });
}

function openInterviewSchedulingModal(applicationId, jobId, jobTitle, companyName, candidateName) {
    $.ajax({
        method: "GET",
        data: { applicationId: applicationId, jobId: jobId, jobTitle: jobTitle, companyName: companyName, candidateName: candidateName },
        url: "/Company/ScheduleInterviewModal",

        success: function (result) {
            $('#applicantsModalContainer').html(result);
            $('#scheduleInterviewModal').modal('show');
        },

        error: function () {

        }
    });
}

function scheduleInterview() {
    event.preventDefault();

    if ($('#scheduleInterviewForm').valid()) {
        $.ajax({
            method: "POST",
            url: "/Company/ScheduleInterview",
            data: $('#scheduleInterviewForm').serialize(),

            success: function (response) {
                $('#scheduleInterviewModal').modal('hide');
                if (response.success == true) {
                    Swal.fire({
                        icon: "success",
                        title: "Yay...",
                        text: "Interview Scheduled",
                        showConfirmButton: false,
                        timer: 3000,
                        timerProgressBar: true
                    });
                }
                else {
                    Swal.fire({
                        icon: "error",
                        title: "Oops...",
                        text: response.error,
                        showConfirmButton: false,
                        timer: 3000,
                        timerProgressBar: true
                    });
                }
                setInterval(function () {
                    window.location.reload();
                }, 3000)
            },

            error: function () {

            }
        });
    }
}

function openInterviewDetailsModal(interviewId, title) {
    $.ajax({
        method: "GET",
        data: { interviewId: interviewId, title: title },
        url: "/Company/ViewInterviewDetailsModal",

        success: function (result) {
            $('#calendarModalContainer').html(result);
            $('#interviewDetailsModal').modal('show');

        },

        error: function () {

        }
    });
}

function openInterviewDetailsModalForCandidate(interviewId, title) {
    $.ajax({
        method: "GET",
        data: { interviewId: interviewId, title: title },
        url: "/Candidate/ViewInterviewDetailsModal",

        success: function (result) {
            $('#candidateCalendarModalContainer').html(result);
            $('#interviewDetailsModal').modal('show');

        },

        error: function () {

        }
    });
}

function previewOfferLetter() {
    var formData = $('#offerJobForm').serializeJSON();

    $.ajax({
        method: "GET",
        data: $('#offerJobForm').serialize(),
        url: "/Company/PreviewOfferLetter",

        success: function (result) {
            $('#offerLetterContainer').html(result);
            $('#previewOfferLetter').modal('show');

            $("#reportViewer").boldReportViewer({
                reportServiceUrl: "/Report",
                reportPath: "OfferLetterReport.rdlc",
                parameters: [
                    { name: "CompanyName", value: formData.CompanyName },
                    { name: "SenderName", value: formData.SenderName },
                    { name: "CandidateName", value: formData.CandidateName },
                    { name: "JobTitle", value: formData.JobTitle },
                    { name: "JobType", value: formData.JobType },
                    { name: "JobLocation", value: formData.JobLocation },
                    { name: "JoiningDate", value: formData.JoiningDate },
                    { name: "Salary", value: formData.Salary },
                ]
            });
        },

        error: function () {

        }
    });
}

function OfferJobWithLetter() {
    event.preventDefault();

    if ($('#offerJobForm').valid()) {
        var formData = $('#offerJobForm').serializeJSON();

        $.ajax({
            method: "GET",
            url: "/Company/GenerateOfferLetter",
            data: formData,
            xhrFields: {
                responseType: 'blob'
            },

            success: function (data, status, xhr) {

                //var blob = new Blob([data], { type: 'application/pdf' });

                //var link = document.createElement('a');
                //link.href = URL.createObjectURL(blob);
                //link.download = "OfferLetter.pdf";
                //link.click();

                //URL.revokeObjectURL(link.href);

                var fileData = new FormData();
                fileData.append("applicationId", formData.ApplicationId);
                fileData.append("jobId", formData.JobId);
                fileData.append("file", data, "OfferLetter.pdf");


                $.ajax({
                    method: 'POST',
                    url: "/Company/OfferJob",
                    data: fileData,
                    processData: false,
                    contentType: false,

                    success: function (response) {
                        if (response.success == true) {
                            Swal.fire({
                                icon: "success",
                                title: "Yay...",
                                text: "Job Offered",
                                showConfirmButton: false,
                                timer: 3000,
                                timerProgressBar: true
                            });
                        }
                        else {
                            Swal.fire({
                                icon: "error",
                                title: "Oops...",
                                text: response.error,
                                showConfirmButton: false,
                                timer: 3000,
                                timerProgressBar: true
                            });
                        }
                        setInterval(function () {
                            window.location.reload();
                        }, 3000)
                    },
                    error: function () {
                    }
                });
            },
            error: function () {
            }
        });
    }
}

function openCompanyInformation(fileName) {
    $.ajax({
        method: "GET",
        data: { fileName: fileName },
        url: "/Candidate/CompanyInformation",

        success: function (result) {
            $('#candidateModalContainer').html(result);
            $('#companyInformationModal').modal('show');
        },

        error: function () {

        }
    });
}


// ******************************************************* Validation *******************************************************

function resetValidation() {
    $('.field-validation-error').html("");
}