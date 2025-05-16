namespace apbd_kolos1;

public interface IDBService
{
    public Task<BookingDTO> GetBookingByIdAsync(int id);
    public Task AddNewBookingAsync(BookingCreateRequestDTO booking);
}
