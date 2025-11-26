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
    public class ProjectMetaDataController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private static readonly NLog.ILogger _logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Constructor to create a new ProjectController.
        /// </summary>
        /// <param name="unitOfWork">IUnitOfWork - for interacting with repositories/database.</param>
        /// <param name="mapper">IMapper - AutoMapper setup to map between model and DTO objects.</param>
        public ProjectMetaDataController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;

            // Set ActivityID for logging
            Trace.CorrelationManager.ActivityId = Guid.NewGuid();
        }

        /// <summary>
        /// Method to get all project meta data from the database based on id.
        /// </summary>
        /// <returns>ProjectMetaDataDto - project meta data in the database.</returns>
        [HttpGet]
        [Route("/projectMetaData/projectMetaDataById")]
        public IActionResult GetProjectMetaDataById(int id)
        {
            var projectMetaData = new ProjectMetaData();

            try
            {
                _logger.Info($"Processing get project meta data by id request.");

                projectMetaData = _unitOfWork.ProjectMetaDataRepository.Get(
                    includeProperties: x => x.Project,
                    filter: x => x.ProjectId == id).FirstOrDefault();

                _logger.Info($"Finished processing get project meta data by id request.");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "There was an error processing the request.");

                return new ContentResult { StatusCode = (int)HttpStatusCode.InternalServerError, Content = "An internal server error has occurred." };
            }

            return new JsonResult(_mapper.Map<ProjectMetaDataDto>(projectMetaData));
        }

        /// <summary>
        /// Method to get all project meta data from the database based on oppId.
        /// </summary>
        /// <returns>ProjectMetaDataDto - project meta data in the database.</returns>
        [HttpGet]
        [Route("/projectMetaData/projectMetaDataByOppId")]
        public IActionResult GetProjectMetaDataByOppId(string oppId)
        {
            var projectMetaData = new ProjectMetaData();

            try
            {
                _logger.Info($"Processing get project meta data by opp id request.");

                projectMetaData = _unitOfWork.ProjectMetaDataRepository.Get(
                    includeProperties: x => x.Project,
                    filter: x => x.Project.OpportunityId == oppId).FirstOrDefault();

                _logger.Info($"Finished processing get project meta data by opp id request.");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "There was an error processing the request.");

                return new ContentResult { StatusCode = (int)HttpStatusCode.InternalServerError, Content = "An internal server error has occurred." };
            }

            return new JsonResult(_mapper.Map<ProjectMetaDataDto>(projectMetaData));
        }
    }
}
