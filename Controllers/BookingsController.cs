using Microsoft.AspNetCore.Mvc;

namespace apbd_kolos1;

[ApiController]
[Route("/api/bookings")] // /api/[controller] == /api/Bookings (instead of /api/bookings)
public class BookingsController : ControllerBase
{
    protected IDBService dBService;

    public BookingsController(IDBService dBService)
    {
        this.dBService = dBService;
    }

    [HttpGet("/api/bookings/{id}")]
    public async Task<IActionResult> GetBookingById(int id)
    {
        return this.Ok(await this.dBService.GetBookingByIdAsync(id));
    }

    [HttpPut("/api/bookings")]
    public async Task<IActionResult> PutBooking(BookingCreateRequestDTO booking)
    {
        await this.dBService.AddNewBookingAsync(booking);
        return this.Ok();
    }
}

