using MapsterMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NLog;
using PreconIQAPI.Data.DAL;
using PreconIQAPI.DTOs;
using PreconIQAPI.Models;
using System;
using System.Diagnostics;
using System.Net;

namespace PreconIQAPI.Controllers
{
    [Authorize]
    public class ProjectWindBODDataController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private static readonly NLog.ILogger _logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Constructor to create a new ProjectWindBODDataController.
        /// </summary>
        /// <param name="unitOfWork">IUnitOfWork - for interacting with repositories/database.</param>
        /// <param name="mapper">IMapper - Mapster setup to map between model and DTO objects.</param>
        public ProjectWindBODDataController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;

            // Set ActivityID for logging
            Trace.CorrelationManager.ActivityId = Guid.NewGuid();
        }

        /// <summary>
        /// Method to insert project wind BOD data into the database.
        /// </summary>
        /// <param name="projectWindBODDataDto">ProjectWindBODDataDto - project wind BOD data to insert.</param>
        /// <returns>ProjectWindBODDataDto - the project wind BOD data that was inserted.</returns>
        [HttpPost]
        [Route("/projectWindBODData/insertProjectWindBODData")]
        public IActionResult InsertProjectWindBODData([FromBody] ProjectWindBODDataDto projectWindBODDataDto)
        {
            ProjectWindBODData projectWindBODData;

            try
            {
                _logger.Info("Processing insert project wind BOD data request.");

                if (string.IsNullOrWhiteSpace(projectWindBODDataDto.OpportunityId))
                {
                    return new ContentResult { StatusCode = (int)HttpStatusCode.BadRequest, Content = "OpportunityId is required." };
                }

                var project = _unitOfWork.ProjectRepository.Get(filter: x => x.OpportunityId == projectWindBODDataDto.OpportunityId).FirstOrDefault();

                if (project is null)
                {
                    return new ContentResult { StatusCode = (int)HttpStatusCode.NotFound, Content = "Project not found for the provided opportunity id." };
                }

                projectWindBODData = _mapper.Map<ProjectWindBODData>(projectWindBODDataDto);
                projectWindBODData.ProjectId = project.Id;

                _unitOfWork.ProjectWindBODDataRepository.Insert(projectWindBODData);
                _unitOfWork.Save();

                _logger.Info("Finished processing insert project wind BOD data request.");

                // Reload the inserted entity with navigation properties (e.g., Project)
                var reloadedProjectWindBODData = _unitOfWork.ProjectWindBODDataRepository.Get(
                    includeProperties: x => x.Project,
                    filter: x => x.Id == projectWindBODData.Id
                ).FirstOrDefault();

                if (reloadedProjectWindBODData == null)
                {
                    return new ContentResult { StatusCode = (int)HttpStatusCode.NotFound, Content = "Inserted ProjectWindBODData could not be found." };
                }

                return new JsonResult(_mapper.Map<ProjectWindBODDataDto>(reloadedProjectWindBODData));
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "There was an error processing the request.");

                return new ContentResult { StatusCode = (int)HttpStatusCode.InternalServerError, Content = "An internal server error has occurred." };
            }
        }

        /// <summary>
        /// Method to get project wind BOD data from the database based on record id.
        /// </summary>
        /// <returns>ProjectWindBODDataDto - project wind BOD data in the database.</returns>
        [HttpGet]
        [Route("/projectWindBODData/projectWindBODDataById")]
        public IActionResult GetProjectWindBODDataById(int id)
        {
            ProjectWindBODData? projectWindBODData;

            try
            {
                _logger.Info("Processing get project wind BOD data by id request.");

                projectWindBODData = _unitOfWork.ProjectWindBODDataRepository.Get(
                    includeProperties: x => x.Project,
                    filter: x => x.Id == id).FirstOrDefault();

                _logger.Info("Finished processing get project wind BOD data by id request.");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "There was an error processing the request.");

                return new ContentResult { StatusCode = (int)HttpStatusCode.InternalServerError, Content = "An internal server error has occurred." };
            }

            return new JsonResult(_mapper.Map<ProjectWindBODDataDto>(projectWindBODData));
        }

        /// <summary>
        /// Method to get project wind BOD data from the database based on opportunity id.
        /// </summary>
        /// <returns>ProjectWindBODDataDto - project wind BOD data in the database.</returns>
        [HttpGet]
        [Route("/projectWindBODData/projectWindBODDataByOppId")]
        public IActionResult GetProjectWindBODDataByOppId(string oppId)
        {
            ProjectWindBODData? projectWindBODData;

            try
            {
                _logger.Info("Processing get project wind BOD data by opp id request.");

                projectWindBODData = _unitOfWork.ProjectWindBODDataRepository.Get(
                    includeProperties: x => x.Project,
                    filter: x => x.Project.OpportunityId == oppId).FirstOrDefault();

                _logger.Info("Finished processing get project wind BOD data by opp id request.");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "There was an error processing the request.");

                return new ContentResult { StatusCode = (int)HttpStatusCode.InternalServerError, Content = "An internal server error has occurred." };
            }

            return new JsonResult(_mapper.Map<ProjectWindBODDataDto>(projectWindBODData));
        }

        /// <summary>
        /// Method to update project wind BOD data in the database based on record id.
        /// </summary>
        /// <param name="projectWindBODDataDto">ProjectWindBODDataDto - project wind BOD data to update.</param>
        /// <returns>ProjectWindBODDataDto - the project wind BOD data that was updated.</returns>
        [HttpPut]
        [Route("/projectWindBODData/updateProjectWindBODData")]
        public IActionResult UpdateProjectWindBODData([FromBody] ProjectWindBODDataDto projectWindBODDataDto)
        {
            ProjectWindBODData? projectWindBODData;

            try
            {
                _logger.Info("Processing update project wind BOD data request.");

                if (projectWindBODDataDto is null)
                {
                    return new ContentResult { StatusCode = (int)HttpStatusCode.BadRequest, Content = "Request body is required." };
                }

                if (projectWindBODDataDto.Id <= 0)
                {
                    return new ContentResult { StatusCode = (int)HttpStatusCode.BadRequest, Content = "Valid Id is required." };
                }

                var existingProjectWindBODData = _unitOfWork.ProjectWindBODDataRepository.Get(filter: x => x.Id == projectWindBODDataDto.Id).FirstOrDefault();

                if (existingProjectWindBODData is null)
                {
                    return new ContentResult { StatusCode = (int)HttpStatusCode.NotFound, Content = "Project wind BOD data not found for the provided id." };
                }

                // Map the DTO values to the existing tracked entity
                _mapper.Map(projectWindBODDataDto, existingProjectWindBODData);

                _unitOfWork.ProjectWindBODDataRepository.Update(existingProjectWindBODData);
                _unitOfWork.Save();

                _logger.Info("Finished processing update project wind BOD data request.");

                // Reload the updated entity with navigation properties (e.g., Project)
                var reloadedProjectWindBODData = _unitOfWork.ProjectWindBODDataRepository.Get(
                    includeProperties: x => x.Project,
                    filter: x => x.Id == existingProjectWindBODData.Id
                ).FirstOrDefault();

                if (reloadedProjectWindBODData == null)
                {
                    return new ContentResult { StatusCode = (int)HttpStatusCode.NotFound, Content = "Updated ProjectWindBODData could not be found." };
                }

                return new JsonResult(_mapper.Map<ProjectWindBODDataDto>(reloadedProjectWindBODData));
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "There was an error processing the request.");

                return new ContentResult { StatusCode = (int)HttpStatusCode.InternalServerError, Content = "An internal server error has occurred." };
            }
        }
    }
}
