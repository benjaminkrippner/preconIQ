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
    public class ProjectNormalsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private static readonly NLog.ILogger _logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Constructor to create a new ProjectController.
        /// </summary>
        /// <param name="unitOfWork">IUnitOfWork - for interacting with repositories/database.</param>
        /// <param name="mapper">IMapper - AutoMapper setup to map between model and DTO objects.</param>
        public ProjectNormalsController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;

            // Set ActivityID for logging
            Trace.CorrelationManager.ActivityId = Guid.NewGuid();
        }

        /// <summary>
        /// Method to get all project normals from the database based on id.
        /// </summary>
        /// <returns>ProjectNormalsDto - project normals in the database.</returns>
        [HttpGet]
        [Route("/projectNormals/projectNormalsById")]
        public IActionResult GetProjectNormalsById(int id)
        {
            var projectNormals = new List<ProjectNormals>();

            try
            {
                _logger.Info($"Processing get project normals by id request.");

                projectNormals = _unitOfWork.ProjectNormalsRepository.Get(
                    includeProperties: x => x.Project,
                    filter: x => x.ProjectId == id,
                    orderBy: x => x.OrderBy(y => y.Month))
                    .ToList();

                _logger.Info($"Finished processing get project normals by id request.");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "There was an error processing the request.");

                return new ContentResult { StatusCode = (int)HttpStatusCode.InternalServerError, Content = "An internal server error has occurred." };
            }

            return new JsonResult(_mapper.Map<List<ProjectNormalsDto>>(projectNormals));
        }

        /// <summary>
        /// Method to get all project normals from the database based on oppId.
        /// </summary>
        /// <returns>ProjectNormalsDto - project normals in the database.</returns>
        [HttpGet]
        [Route("/projectNormals/projectNormalsByOppId")]
        public IActionResult GetProjectNormalsByOppId(string oppId)
        {
            var projectNormals = new List<ProjectNormals>();

            try
            {
                _logger.Info($"Processing get project normals by opp id request.");

                projectNormals = _unitOfWork.ProjectNormalsRepository.Get(
                    includeProperties: x => x.Project,
                    filter: x => x.Project.OpportunityId == oppId,
                    orderBy: x => x.OrderBy(y => y.Month))
                    .ToList();

                _logger.Info($"Finished processing get project normals by opp id request.");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "There was an error processing the request.");

                return new ContentResult { StatusCode = (int)HttpStatusCode.InternalServerError, Content = "An internal server error has occurred." };
            }

            return new JsonResult(_mapper.Map<List<ProjectNormalsDto>>(projectNormals));
        }
    }
}
