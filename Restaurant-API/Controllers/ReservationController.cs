using Microsoft.AspNetCore.Mvc;
using Restaurant_API.Data.Models.Reservation;
using Restaurant_API.Data.Models.Other;
using Restaurant_API.Services.Interfaces;
using Restaurant_API.Attributes;

namespace Restaurant_API.Controllers
{
    [ApiController]
    [Route("api/r")]
    public class ReservationController : ControllerBase
    {
        private readonly IReservationService reservationService;

        public ReservationController(IReservationService reservationService)
        {
            this.reservationService = reservationService;
        }

        /* ======================================================================== POST */
        // TODO: Add when connecting the website to the mobile app
        // [HttpPost]
        // public async Task<IActionResult> CreateClientReservationAsync(ClientCreateReservationModel createModel)
        // {
        //     await reservationService.CreateClientReservationAsync(createModel);
        //     return NoContent();
        // }

        [HttpPost("confirmed"), AppAuthorize]
        public async Task<IActionResult> CreateConfirmedReservationAsync(ReservationCreateModel createModel)
        {
            await reservationService.CreateConfirmedReservationAsync(HttpContext, createModel);
            return NoContent();
        }

        [HttpPost("{reservationId}/close"), AppAuthorize]
        public async Task<IActionResult> CloseReservationAsync(Guid reservationId)
        {
            await reservationService.CloseReservationAsync(HttpContext, reservationId);
            return NoContent();
        }

        [HttpPost("{reservationId}/reopen"), AppAuthorize]
        public async Task<IActionResult> ReopenReservationAsync(Guid reservationId)
        {
            await reservationService.ReopenReservationAsync(HttpContext, reservationId);
            return NoContent();
        }


        /* ======================================================================== GET */
        [HttpGet("{reservationId}"), AppAuthorize]
        public async Task<IActionResult> GetByIdAsync(Guid reservationId)
        {
            ReservationViewModel reservation = await reservationService.GetByIdAsync(HttpContext, reservationId);
            return Ok(reservation);
        }

        [HttpGet("{reservationId}/floor-plan-preview"), AppAuthorize]
        public async Task<IActionResult> GetFloorPlanReservationPreviewAsync(Guid reservationId)
        {
            var preview = await reservationService.GetFloorPlanReservationPreviewAsync(HttpContext, reservationId);
            return Ok(preview);
        }

        [HttpGet("confirmed"), AppAuthorize]
        public async Task<IActionResult> GetConfirmedReservationsAsync([FromQuery] UtcRangeQuery query)
        {
            var previews = await reservationService.GetConfirmedReservationsWithinUtcRangeAsync(HttpContext, query);
            return Ok(previews);
        }

        [HttpGet("pending"), AppAuthorize]
        public async Task<IActionResult> GetPendingReservationsAsync([FromQuery] PendingReservationsQueryParams queryParams)
        {
            var previews = await reservationService.GetPendingReservationsAsync(HttpContext, queryParams);
            return Ok(previews);
        }


        /* ======================================================================== PATCH */
        [HttpPatch("{reservationId}"), AppAuthorize]
        public async Task<IActionResult> UpdateAsync(Guid reservationId, ReservationUpdateModel updateModel)
        {
            ReservationViewModel reservation = await reservationService.UpdateAsync(HttpContext, reservationId, updateModel);
            return Ok(reservation);
        }


        /* ======================================================================== DELETE */
        [HttpDelete("{reservationId}"), AppAuthorize]
        public async Task<IActionResult> RemoveAsync(Guid reservationId)
        {
            await reservationService.RemoveAsync(HttpContext, reservationId);
            return NoContent();
        }
    }
}
