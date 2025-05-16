namespace apbd_kolos1;

public class BookingDTO
{
    public DateTime Date { get; set; }

    public GuestDTO? Guest { get; set; }
    public EmployeeDTO? Employee { get; set; }
    public IList<BookingAttractionDTO> BookingAttractions { get; set; } = [];
}
