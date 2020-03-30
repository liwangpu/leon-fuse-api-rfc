using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Base.API.ActionResults
{
    public class MultiStatusObjectResult
    {
        private List<ResultObject> results = new List<ResultObject>();


        public void AddResult(string href, int status, string message = "")
        {
            results.Add(new ResultObject(href, status, message));
        }

        public ObjectResult Transfer()
        {

            Func<ObjectResult> _200StatusCodeResult = () =>
            {
                var res = new ObjectResult(results);
                res.StatusCode = StatusCodes.Status200OK;
                return res;
            };

            Func<ObjectResult> _207Result = () =>
            {
                var res = new ObjectResult(results);
                res.StatusCode = StatusCodes.Status207MultiStatus;
                return res;
            };

            if (results == null || results.Count == 0)
            {
                return _200StatusCodeResult();
            }
            else
            {
                var bNot2xxStatusCode = results.Count(x => x.Status >= 300) > 0;
                if (bNot2xxStatusCode)
                    return _207Result();
                return _200StatusCodeResult();
            }
        }

        private class ResultObject
        {
            public string Href { get; set; }
            public int Status { get; set; }
            public string Message { get; set; }

            public ResultObject(string href, int status, string message)
            {
                Href = href;
                Status = status;
                Message = message;
            }
        }

    }
}
