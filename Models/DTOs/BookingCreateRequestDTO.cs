namespace apbd_kolos1;

public class BookingCreateRequestDTO
{
    public int BookingId { get; set; } = -1;
    public int GuestId { get; set; } = -1;
    public string EmployeeNumber { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public AttractionCreateRequestDTO[] Attractions { get; set; } = [];
}
