﻿namespace UnstopAPI.Models.DTO
{
    public class ForgotPasswordModel
    {
        public string Email { get; set; }

        public string OtpCode { get; set; }

        public string NewPassword { get; set; }
    }
}
