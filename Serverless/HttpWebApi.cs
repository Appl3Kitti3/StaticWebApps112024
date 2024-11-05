using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SchoolLibrary;
using Serverless.Models.School;

namespace Gabby.Function
{
    public class HttpWebApi
    {
        private readonly ILogger<HttpWebApi> _logger;

        private readonly SchoolContext _context;

        public HttpWebApi(ILoggerFactory loggerFactory, SchoolContext context)
        {
            _logger = loggerFactory.CreateLogger<HttpWebApi>();
            _context = context;
        }

        public HttpWebApi(ILogger<HttpWebApi> logger)
        {
            _logger = logger;
        }

        [Function("HttpWebApi")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            return new OkObjectResult("Welcome to Azure Functions!");
        }

        [Function("GetStudents")]
        public async Task<HttpResponseData> GetStudents(
            // Get is the HTTP method, students is the route like /students
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "students")] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP GET/posts trigger function processed a request in GetStudents().");

            var students = await _context.Students.ToArrayAsync();

            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "application/json");

            await response.WriteStringAsync(JsonConvert.SerializeObject(students));

            return response;
        }

        /// <summary>
        /// Get a student by id
        /// </summary>
        /// <param name="id">student id</param>
        /// <returns>Reponse data with student data</returns>
        [Function("GetStudentById")]
        public async Task<HttpResponseData> GetStudentById
        (
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "students/{id}")] HttpRequestData req,
        int id
        )
        {
            _logger.LogInformation("C# HTTP GET/posts trigger function processed a request.");
            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                var response = req.CreateResponse(HttpStatusCode.NotFound);
                response.Headers.Add("Content-Type", "application/json");
                await response.WriteStringAsync("Not Found");
                return response;
            }
            var response2 = req.CreateResponse(HttpStatusCode.OK);
            response2.Headers.Add("Content-Type", "application/json");
            await response2.WriteStringAsync(JsonConvert.SerializeObject(student));
            return response2;
        }

        /// <summary>
        /// Create a student
        /// </summary>
        /// <returns>response with confirmation or error</returns>
        [Function("CreateStudent")]
        public async Task<HttpResponseData> CreateStudent
        (
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "students")] HttpRequestData req
        )
        {
            _logger.LogInformation("C# HTTP POST/posts trigger function processed a request.");
            var student = JsonConvert.DeserializeObject<Student>(req.ReadAsStringAsync().Result);
            _context.Students.Add(student);
            await _context.SaveChangesAsync();
            var response = req.CreateResponse(HttpStatusCode.Created);
            response.Headers.Add("Content-Type", "application/json");
            await response.WriteStringAsync(JsonConvert.SerializeObject(student));
            return response;
        }


        /// <summary>
        /// Update a student based on id
        /// </summary>
        /// <param name="id">student id</param>
        [Function("UpdateStudent")]
        public async Task<HttpResponseData> UpdateStudent
        (
        [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "students/{id}")] HttpRequestData req,
        int id
        )
        {
            _logger.LogInformation("C# HTTP PUT/posts trigger function processed a request.");
            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                var response = req.CreateResponse(HttpStatusCode.NotFound);
                response.Headers.Add("Content-Type", "application/json");
                await response.WriteStringAsync("Not Found");
                return response;
            }
            var student2 = JsonConvert.DeserializeObject<Student>(req.ReadAsStringAsync().Result);
            student.FirstName = student2.FirstName;
            student.LastName = student2.LastName;
            student.School = student2.School;
            await _context.SaveChangesAsync();
            var response2 = req.CreateResponse(HttpStatusCode.OK);
            response2.Headers.Add("Content-Type", "application/json");
            await response2.WriteStringAsync(JsonConvert.SerializeObject(student));
            return response2;
        }

        /// <summary>
        /// Delete a student
        /// </summary>
        /// <param name="id">student id</param>
        [Function("DeleteStudent")]
        public async Task<HttpResponseData> DeleteStudent
        (
        [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "students/{id}")] HttpRequestData req,
        int id
        )
        {
            _logger.LogInformation("C# HTTP DELETE/posts trigger function processed a request.");
            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                var response = req.CreateResponse(HttpStatusCode.NotFound);
                response.Headers.Add("Content-Type", "application/json");
                await response.WriteStringAsync("Not Found");
                return response;
            }
            _context.Students.Remove(student);
            await _context.SaveChangesAsync();
            var response2 = req.CreateResponse(HttpStatusCode.OK);
            response2.Headers.Add("Content-Type", "application/json");
            await response2.WriteStringAsync(JsonConvert.SerializeObject(student));
            return response2;
        }

    

    }
}
