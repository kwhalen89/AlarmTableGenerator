namespace AlarmTableGenerator.Api.Struct
{
    public struct Error
    {
        public int LineNumber { get; set; }
        public string Message { get; set; }

        public Error(int lineNum, string errorMessage)
        {
            LineNumber = lineNum;
            Message = errorMessage;
        }
    }
}
