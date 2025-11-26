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
    public class ProjectWindDataController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private static readonly NLog.ILogger _logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Constructor to create a new ProjectWindDataController.
        /// </summary>
        /// <param name="unitOfWork">IUnitOfWork - for interacting with repositories/database.</param>
        /// <param name="mapper">IMapper - Mapster setup to map between model and DTO objects.</param>
        public ProjectWindDataController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;

            // Set ActivityID for logging
            Trace.CorrelationManager.ActivityId = Guid.NewGuid();
        }

        /// <summary>
        /// Method to get project wind data from the database based on wind data id.
        /// </summary>
        /// <returns>ProjectWindDataDto - project wind data in the database.</returns>
        [HttpGet]
        [Route("/projectWindData/projectWindDataById")]
        public IActionResult GetProjectWindDataById(int id)
        {
            List<ProjectWindData> projectWindData;

            try
            {
                _logger.Info($"Processing get project wind data by id request.");

                projectWindData = _unitOfWork.ProjectWindDataRepository.Get(
                    filter: x => x.Id == id,
                    orderBy: x => x.OrderBy(y => y.ValidTime))
                    .ToList();

                _logger.Info($"Finished processing get project wind data by id request.");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "There was an error processing the request.");

                return new ContentResult { StatusCode = (int)HttpStatusCode.InternalServerError, Content = "An internal server error has occurred." };
            }

            return new JsonResult(_mapper.Map<List<ProjectWindDataDto>>(projectWindData));
        }

        /// <summary>
        /// Method to get all project wind data from the database based on project id.
        /// </summary>
        /// <returns>ProjectWindDataDto - project wind data in the database.</returns>
        [HttpGet]
        [Route("/projectWindData/projectWindDataByProjectId")]
        public IActionResult GetProjectWindDataByProjectId(int id)
        {
            List<ProjectWindData> projectWindData;

            try
            {
                _logger.Info($"Processing get project wind data by id request.");

                projectWindData = _unitOfWork.ProjectWindDataRepository.Get(
                    includeProperties: x => x.Project,
                    filter: x => x.ProjectId == id,
                    orderBy: x => x.OrderBy(y => y.ValidTime))
                    .ToList();

                _logger.Info($"Finished processing get project wind data by id request.");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "There was an error processing the request.");

                return new ContentResult { StatusCode = (int)HttpStatusCode.InternalServerError, Content = "An internal server error has occurred." };
            }

            return new JsonResult(_mapper.Map<List<ProjectWindDataDto>>(projectWindData));
        }

        /// <summary>
        /// Method to get all project wind data from the database based on oppId.
        /// </summary>
        /// <returns>ProjectWindDataDto - project wind data in the database.</returns>
        [HttpGet]
        [Route("/projectWindData/projectWindDataByOppId")]
        public IActionResult GetProjectWindDataByOppId(string oppId)
        {
            List<ProjectWindData> projectWindData;

            try
            {
                _logger.Info($"Processing get project wind data by opp id request.");

                projectWindData = _unitOfWork.ProjectWindDataRepository.Get(
                    includeProperties: x => x.Project,
                    filter: x => x.Project.OpportunityId == oppId,
                    orderBy: x => x.OrderBy(y => y.ValidTime))
                    .ToList();

                _logger.Info($"Finished processing get project wind data by opp id request.");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "There was an error processing the request.");

                return new ContentResult { StatusCode = (int)HttpStatusCode.InternalServerError, Content = "An internal server error has occurred." };
            }

            return new JsonResult(_mapper.Map<List<ProjectWindDataDto>>(projectWindData));
        }

        /*
        /// <summary>
        /// Method to get the project wind data entries closest to the specified coordinates.
        /// </summary>
        /// <param name="latitude">decimal - latitude to search near.</param>
        /// <param name="longitude">decimal - longitude to search near.</param>
        /// <returns>ProjectWindDataDto - list of project wind data entries at the closest coordinates.</returns>
        [HttpGet]
        [Route("/projectWindData/projectWindDataByCoordinates")]
        public IActionResult GetProjectWindDataByCoordinates(decimal latitude, decimal longitude)
        {
            ProjectWindData? closestWindData;
            List<ProjectWindData> windDataAtClosestCoordinates = new();

            try
            {
                _logger.Info($"Processing get project wind data by coordinates request.");

                closestWindData = _unitOfWork.ProjectWindDataRepository.Get(
                    orderBy: x => x.OrderBy(y =>
                        Math.Pow((double)(y.Latitude - latitude), 2) +
                        Math.Pow((double)(y.Longitude - longitude), 2)))
                    .FirstOrDefault();

                if (closestWindData is null)
                {
                    return new ContentResult { StatusCode = (int)HttpStatusCode.NotFound, Content = "No project wind data was found." };
                }

                windDataAtClosestCoordinates = _unitOfWork.ProjectWindDataRepository.Get(
                    filter: x => x.Latitude == closestWindData.Latitude && x.Longitude == closestWindData.Longitude,
                    orderBy: x => x.OrderBy(y => y.ValidTime))
                    .ToList();

                _logger.Info($"Finished processing get project wind data by coordinates request.");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "There was an error processing the request.");

                return new ContentResult { StatusCode = (int)HttpStatusCode.InternalServerError, Content = "An internal server error has occurred." };
            }

            return new JsonResult(_mapper.Map<List<ProjectWindDataDto>>(windDataAtClosestCoordinates));
        }
        */

    }
}
