using Microsoft.AspNetCore.Mvc;
using Restaurant_API.Attributes;
using Restaurant_API.Data.Models.Table;
using Restaurant_API.Services.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace Restaurant_API.Controllers
{
    [ApiController]
    [Route("api")]
    public class TableController : ControllerBase
    {
        private readonly ITableService tableService;

        public TableController(ITableService tableService)
        {
            this.tableService = tableService;
        }

        /* ======================================================================== GET */
        [HttpGet("floor-plan"), AppAuthorize]
        public async Task<IActionResult> GetRestaurantFloorPlanAsync([FromQuery, Required] DateOnly date, [FromQuery] Guid? shiftId)
        {
            var floorPlanView = await tableService.GetRestaurantFloorPlanAsync(HttpContext, date, shiftId);
            return Ok(floorPlanView);
        }

        [HttpGet("r/{reservationId}/tables"), AppAuthorize]
        public async Task<IActionResult> GetTablesAvailabilityForReservationAsync(Guid reservationId)
        {
            var reservationShiftView = await tableService.GetTablesAvailabilityForReservationAsync(HttpContext, reservationId);
            return Ok(reservationShiftView);
        }

        /* ======================================================================== PATCH */
        [HttpPatch("r/{reservationId}/tables"), AppAuthorize]
        public async Task<IActionResult> UpdateReservationTablesAsync(Guid reservationId, ReservationTablesUpdate updateModel)
        {
            var reservationShiftView = await tableService.UpdateReservationTablesAsync(HttpContext, reservationId, updateModel);

            // No changes were made
            if (reservationShiftView == null)
                return NoContent();

            return Ok(reservationShiftView);
        }
    }
}
