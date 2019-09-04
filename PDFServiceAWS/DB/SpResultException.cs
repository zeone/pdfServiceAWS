using System;

namespace PDFServiceAWS.DB
{
    public class SpResultException : Exception
    {
        public SpResultException(int returnCode, string errorMsg, string spName)
        {
            ReturnCode = returnCode;
            ErrorMessage = errorMsg;
            SpName = spName;
        }

        public SpResultException(SpExecResult spExecResult)
        {
            if (spExecResult == null)
            {
                // SP execution result not defined
                throw new ArgumentNullException("spExecResult");
            }

            ReturnCode = spExecResult.ReturnCode;
            SpName = spExecResult.StoredProcedureName;
            ErrorMessage = spExecResult.ErrorMessage;
        }

        public int ReturnCode
        {
            get;
            private set;
        }

        public string ErrorMessage
        {
            get;
            private set;
        }

        public string SpName
        {
            get;
            private set;
        }

        public override string Message
        {
            get
            {
                string errorMsg = @"Stored procedure '{0}' was executed with errors! 
                                    Return code: {1}
                                    Error message: {2}";
                return string.Format(errorMsg, SpName, ReturnCode, ErrorMessage);
            }
        }
    }
}