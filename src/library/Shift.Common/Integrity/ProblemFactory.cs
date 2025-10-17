using System;
using System.Collections.Generic;

namespace Shift.Common
{
    /// <summary>
    /// Helper class for creating common Problem instances.
    /// </summary>
    public static class ProblemFactory
    {
        public static Problem Create(int statusCode, string detail = null, string instance = null)
        {
            return new Problem(statusCode)
            {
                Detail = detail,
                Instance = instance
            };
        }

        public static Problem BadRequest(string detail = null, string instance = null)
        {
            return new Problem(400)
            {
                Detail = detail,
                Instance = instance
            };
        }

        public static Problem NotFound(string detail = null, string instance = null)
        {
            return new Problem(404)
            {
                Detail = detail,
                Instance = instance
            };
        }

        public static Problem InternalServerError(string detail = null, Uri instance = null)
        {
            return new Problem(500)
            {
                Detail = detail,
                Instance = instance?.ToString()
            };
        }

        public static Problem Unauthorized(string detail = null, string instance = null)
        {
            return new Problem(401)
            {
                Detail = detail,
                Instance = instance
            };
        }

        public static Problem Forbidden(string detail = null, string instance = null)
        {
            return new Problem(403)
            {
                Detail = detail,
                Instance = instance
            };
        }

        public static Problem RequestTimeout(string detail = null, string instance = null)
        {
            return new Problem(408)
            {
                Detail = detail,
                Instance = instance
            };
        }

        public static Problem ValidationError(string detail, Dictionary<string, string[]> errors = null)
        {
            var problem = new Problem(422)
            {
                Detail = detail,
                Type = "https://tools.ietf.org/html/rfc4918#section-11.2"
            };

            if (errors != null)
            {
                problem.Extensions["errors"] = errors;
            }

            return problem;
        }

        public static Problem PaymentRequired(string detail = null, Uri instance = null)
        {
            return new Problem(402)
            {
                Detail = detail,
                Instance = instance?.ToString()
            };
        }
    }
}
