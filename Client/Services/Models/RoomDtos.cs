namespace Client.Services.Models;

public class RoomDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public string? Location { get; set; }
    public bool IsActive { get; set; }
    public int ClassCount { get; set; }
}

public class CreateRoomRequest
{
    public string Name { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public string? Location { get; set; }
    public bool IsActive { get; set; } = true;
}

public class UpdateRoomRequest
{
    public string Name { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public string? Location { get; set; }
    public bool IsActive { get; set; }
}
