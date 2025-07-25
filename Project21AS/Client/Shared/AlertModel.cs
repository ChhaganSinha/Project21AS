﻿namespace Project21AS.Client.Shared
{
    public class AlertModel
    {
        public bool IsSuccess { get; set; }
        public bool IsError { get; set; }
        public string Message { get; set; } = String.Empty;

        public void Clear()
        {
            IsError = false;
            IsSuccess = false;
            Message = "";
        }

        public void SetError(string msg)
        {
            IsSuccess = false;
            IsError = true;
            Message = msg;
        }

        public void SetSuccess(string msg)
        {
            IsSuccess = true;
            IsError = false;
            Message = msg;
        }

    }
}
