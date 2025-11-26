using MapsterMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NLog;
using PreconIQAPI.Data.DAL;
using PreconIQAPI.DTOs;
using PreconIQAPI.Models;
using System.Diagnostics;
using System.Net;

namespace PreconIQAPI.Controllers
{
    [Authorize]
    public class ProjectController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private static readonly NLog.ILogger _logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Constructor to create a new ProjectController.
        /// </summary>
        /// <param name="unitOfWork">IUnitOfWork - for interacting with repositories/database.</param>
        /// <param name="mapper">IMapper - AutoMapper setup to map between model and DTO objects.</param>
        public ProjectController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;

            // Set ActivityID for logging
            Trace.CorrelationManager.ActivityId = Guid.NewGuid();
        }

        /// <summary>
        /// Method to get all projects from the database.
        /// </summary>
        /// <returns>List<ProjectDto> - all projects in the database.</returns>
        [HttpGet]
        [Route("/getProjects")]
        public IActionResult GetProjects()
        {
            var projects = new List<Project>();

            try
            {
                _logger.Info($"Processing get projects request.");

                projects = _unitOfWork.ProjectRepository.Get().ToList();

                _logger.Info($"Finished processing get projects request.");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "There was an error processing the request.");

                return new ContentResult { StatusCode = (int)HttpStatusCode.InternalServerError, Content = "An internal server error has occurred." };
            }

            return new JsonResult(_mapper.Map<List<ProjectDto>>(projects));
        }
    }
}
